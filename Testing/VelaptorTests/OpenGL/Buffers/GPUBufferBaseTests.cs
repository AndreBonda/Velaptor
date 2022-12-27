﻿// <copyright file="GPUBufferBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using Carbonate;
using Fakes;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Initializes a new instance of <see cref="GPUBufferBaseTests"/>.
/// </summary>
public class GPUBufferBaseTests
{
    private const string BufferName = "UNKNOWN BUFFER";
    private const uint VertexArrayId = 1256;
    private const uint VertexBufferId = 1234;
    private const uint IndexBufferId = 5678;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactable> mockReactable;
    private readonly Mock<IDisposable> mockGLInitUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private readonly Mock<IDisposable> mockViewPortSizeUnsubscriber;
    private bool vertexBufferCreated;
    private bool indexBufferCreated;
    private IReactor? glInitReactor;
    private IReactor? shutDownReactor;
    private IReactor? viewPortSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GPUBufferBaseTests"/> class.
    /// </summary>
    public GPUBufferBaseTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGL.Setup(m => m.GenBuffer()).Returns(() =>
        {
            if (!this.vertexBufferCreated)
            {
                this.vertexBufferCreated = true;
                return VertexBufferId;
            }

            if (this.indexBufferCreated)
            {
                return 0;
            }

            this.indexBufferCreated = true;
            return IndexBufferId;
        });

        this.mockGL.Setup(m => m.GenVertexArray()).Returns(VertexArrayId);

        this.mockGLService = new Mock<IOpenGLService>();

        this.mockGLInitUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();
        this.mockViewPortSizeUnsubscriber = new Mock<IDisposable>();

        this.mockReactable = new Mock<IReactable>();
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Returns<IReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.EventId == NotificationIds.GLInitializedId)
                {
                    return this.mockGLInitUnsubscriber.Object;
                }

                if (reactor.EventId == NotificationIds.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
                }

                if (reactor.EventId == NotificationIds.ViewPortSizeChangedId)
                {
                    return this.mockViewPortSizeUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.EventId}' is not recognized or accounted for in the unit test.");
                return null;
            })
            .Callback<IReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.EventId == NotificationIds.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
                else if (reactor.EventId == NotificationIds.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
                else if (reactor.EventId == NotificationIds.ViewPortSizeChangedId)
                {
                    this.viewPortSizeReactor = reactor;
                }
                else
                {
                    Assert.Fail($"The event ID '{reactor.EventId}' is not recognized or accounted for in the unit test.");
                }
            });
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLInvokerParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GPUBufferFake(
                null,
                this.mockGLService.Object,
                this.mockReactable.Object);
        }, "The parameter must not be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GPUBufferFake(
                this.mockGL.Object,
                null,
                this.mockReactable.Object);
        }, "The parameter must not be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullGLInitReactorParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GPUBufferFake(
                this.mockGL.Object,
                this.mockGLService.Object,
                null);
        }, "The parameter must not be null. (Parameter 'reactable')");
    }
    #endregion

    #region Props Tests
    [Fact]
    public void BatchSize_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var buffer = CreateSystemUnderTest();

        // Act
        var actual = buffer.BatchSize;

        // Assert
        Assert.Equal(100u, actual);
    }

    [Fact]
    public void IsInitialized_AfterGLInitializes_ReturnsTrue()
    {
        // Arrange
        var buffer = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        Assert.True(buffer.IsInitialized);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void OpenGLInit_WhenInvoked_CreatesVertexArrayObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        this.mockGL.Verify(m => m.GenVertexArray(), Times.Once);
        this.mockGLService.Verify(m => m.BindVAO(VertexArrayId), Times.Once);
        this.mockGLService.Verify(m => m.UnbindVAO(), Times.Once);
        this.mockGLService.Verify(m => m.LabelVertexArray(VertexArrayId, BufferName));
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_CreatesVertexBufferObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        // These are all invoked once per quad
        this.mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce);
        this.mockGLService.Verify(m => m.BindVBO(VertexBufferId), Times.Once);
        this.mockGLService.Verify(m => m.UnbindVBO(), Times.Once);
        this.mockGLService.Verify(m => m.LabelBuffer(VertexBufferId, BufferName, OpenGLBufferType.VertexBufferObject));
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_CreatesElementBufferObject()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        // First invoke is done creating the Vertex Buffer, the second is the index buffer
        this.mockGL.Verify(m => m.GenBuffer(), Times.AtLeastOnce);
        this.mockGLService.Verify(m => m.BindEBO(IndexBufferId), Times.Once);
        this.mockGLService.Verify(m => m.UnbindEBO(), Times.Once);
        this.mockGLService.Verify(m => m.LabelBuffer(IndexBufferId, BufferName, OpenGLBufferType.IndexArrayObject));
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_GeneratesVertexData()
    {
        // Arrange
        var sut  = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        Assert.True(sut.GenerateDataInvoked, $"The method '{nameof(GPUBufferBase<TextureBatchItem>.GenerateData)}'() has not been invoked.");
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_GeneratesIndicesData()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        Assert.True(sut.GenerateIndicesInvoked, $"The method '{nameof(GPUBufferBase<TextureBatchItem>.GenerateData)}'() has not been invoked.");
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_UploadsVertexData()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        this.mockGL.Verify(m => m.BufferData(GLBufferTarget.ArrayBuffer,
                new[] { 1f, 2f, 3f, 4f },
                GLBufferUsageHint.DynamicDraw),
            Times.Once);
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_UploadsIndicesData()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        this.mockGL.Verify(m => m.BufferData(GLBufferTarget.ElementArrayBuffer,
                new uint[] { 11, 22, 33, 44 },
                GLBufferUsageHint.StaticDraw),
            Times.Once);
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_SetsUpVertexArrayObject()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        Assert.True(sut.SetupVAOInvoked, $"The method '{nameof(GPUBufferBase<TextureBatchItem>.SetupVAO)}'() has not been invoked.");
    }

    [Fact]
    public void OpenGLInit_WhenInvoked_SetsUpProperGLGrouping()
    {
        // Arrange
        var totalInvokes = 0;
        var setupDataGroupName = $"Setup {BufferName} Data";
        var setupDataGroupSequence = 0;
        var uploadVertexDataGroupName = $"Upload {BufferName} Vertex Data";
        var uploadVertexDataGroupSequence = 0;
        var uploadIndicesDataGroupName = $"Upload {BufferName} Indices Data";
        var uploadIndicesDataGroupSequence = 0;
        this.mockGLService.Setup(m => m.BeginGroup(setupDataGroupName))
            .Callback(() =>
            {
                totalInvokes += 1;
                setupDataGroupSequence = totalInvokes;
            });
        this.mockGLService.Setup(m => m.BeginGroup(uploadVertexDataGroupName))
            .Callback(() =>
            {
                totalInvokes += 1;
                uploadVertexDataGroupSequence = totalInvokes;
            });
        this.mockGLService.Setup(m => m.BeginGroup(uploadIndicesDataGroupName))
            .Callback(() =>
            {
                totalInvokes += 1;
                uploadIndicesDataGroupSequence = totalInvokes;
            });

        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnNext();

        // Assert
        this.mockGLService.Verify(m => m.BeginGroup(It.IsAny<string>()), Times.Exactly(3));
        this.mockGLService.Verify(m => m.BeginGroup(setupDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.BeginGroup(uploadVertexDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.BeginGroup(uploadIndicesDataGroupName), Times.Once);
        this.mockGLService.Verify(m => m.EndGroup(), Times.Exactly(3));

        // Check that the setup data group was called first
        Assert.Equal(1, setupDataGroupSequence);
        Assert.Equal(2, uploadVertexDataGroupSequence);
        Assert.Equal(3, uploadIndicesDataGroupSequence);
    }

    [Fact]
    public void UploadData_WhenInvoked_PreparesGPUForDataUpload()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var batchItem = default(TextureBatchItem);

        // Act
        sut.UploadData(batchItem, 0u);

        // Assert
        Assert.True(sut.PrepareForUseInvoked, $"The method '{nameof(GPUBufferBase<TextureBatchItem>.PrepareForUpload)}'() has not been invoked.");
    }

    [Fact]
    public void UploadData_WhenInvoked_UpdatesGPUData()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var batchItem = default(TextureBatchItem);

        // Act
        sut.UploadData(batchItem, 0u);

        // Assert
        Assert.True(sut.UpdateVertexDataInvoked, $"The method '{nameof(GPUBufferBase<TextureBatchItem>.UploadVertexData)}'() has not been invoked.");
    }

    [Fact]
    public void WithShutDownNotification_ShutsDownBuffer()
    {
        // Arrange
        CreateSystemUnderTest();

        this.glInitReactor.OnNext();

        // Act
        this.shutDownReactor?.OnNext();
        this.shutDownReactor?.OnNext();

        // Assert
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockGL.Verify(m => m.DeleteVertexArray(VertexArrayId), Times.Once());
        this.mockGL.Verify(m => m.DeleteBuffer(VertexBufferId), Times.Once());
        this.mockGL.Verify(m => m.DeleteBuffer(IndexBufferId), Times.Once());
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void InitReactable_OnComplete_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnComplete();

        // Assert
        this.mockGLInitUnsubscriber.VerifyOnce(m => m.Dispose());
    }

    [Fact]
    public void ViewPortSizeReactable_WithNullMessage_ThrowsException()
    {
        // Arrange
        var expected = $"There was an issue with the 'GPUBufferBase.Constructor()' subscription source";
        expected += $" for subscription ID '{NotificationIds.ViewPortSizeChangedId}'.";

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<ViewPortSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.viewPortSizeReactor.OnNext(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ViewPortSizeReactable_OnComplete_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.viewPortSizeReactor.OnComplete();

        // Assert
        this.mockViewPortSizeUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates an instance of the type <see cref="GPUBufferFake"/> for the purpose of
    /// testing the abstract class <see cref="GPUBufferBase{TData}"/>.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GPUBufferFake CreateSystemUnderTest() => new (
        this.mockGL.Object,
        this.mockGLService.Object,
        this.mockReactable.Object);
}
