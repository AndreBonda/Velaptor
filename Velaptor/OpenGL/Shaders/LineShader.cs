﻿// <copyright file="LineShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

using System;
using Carbonate.UniDirectional;
using Factories;
using Guards;
using NativeInterop.OpenGL;
using ReactableData;
using Services;
using Velaptor.Exceptions;

/// <summary>
/// A line shader used to render 2D lines.
/// </summary>
[ShaderName("Line")]
internal sealed class LineShader : ShaderProgram
{
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineShader"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="shaderLoaderService">Loads GLSL shader source code.</param>
    /// <param name="reactableFactory">Sends and receives push notifications.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public LineShader(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IShaderLoaderService<uint> shaderLoaderService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, shaderLoaderService, reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var batchSizeName = this.GetExecutionMemberName(nameof(NotificationIds.BatchSizeSetId));
        this.unsubscriber = batchSizeReactable.Subscribe(new ReceiveReactor<BatchSizeData>(
            eventId: NotificationIds.BatchSizeSetId,
            name: batchSizeName,
            onReceiveMsg: msg =>
            {
                var batchSize = msg.GetData()?.BatchSize;

                if (batchSize is null)
                {
                    throw new PushNotificationException($"{nameof(LineShader)}.Constructor()", NotificationIds.BatchSizeSetId);
                }

                BatchSize = batchSize.Value;
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));
    }
}
