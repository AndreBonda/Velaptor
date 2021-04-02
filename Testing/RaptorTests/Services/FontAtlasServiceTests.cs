﻿// <copyright file="FontAtlasServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.IO.Abstractions;
    using System.Runtime.InteropServices;
    using FreeTypeSharp.Native;
    using Moq;
    using Raptor;
    using Raptor.Graphics;
    using Raptor.Hardware;
    using Raptor.Services;
    using RaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontAtlasService"/> class.
    /// </summary>
    public unsafe class FontAtlasServiceTests
    {
        private const string FontFilePath = @"C:\temp\test-font.ttf";
        private const int GlyphWidth = 5;
        private const int GlyphHeight = 6;
        private readonly Mock<IFreeTypeInvoker> mockFreeTypeInvoker;
        private readonly Mock<IImageService> mockImageService;
        private readonly Mock<ISystemMonitorService> mockMonitorService;
        private readonly Mock<IPlatform> mockPlatform;
        private readonly IntPtr freeTypeLibPtr = new IntPtr(1234);
        // This represents out it would be layed out in the atlas
        private readonly char[] glyphChars = new[]
        {
            'h', 'e',
            'l', 'o',
            'w', 'r',
            'd',
        };
        private readonly Mock<IFile> mockFile;
        private FT_FaceRec faceRec = default;
        private FT_GlyphSlotRec glyphSlotRec = default;
        private FT_SizeRec sizeRec = default;
        private IntPtr facePtr;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontAtlasServiceTests"/> class.
        /// </summary>
        public FontAtlasServiceTests()
        {
            TestHelpers.SetupTestResultDirPath();

            this.mockFreeTypeInvoker = new Mock<IFreeTypeInvoker>();
            this.mockFreeTypeInvoker.Setup(m => m.FT_Init_FreeType()).Returns(this.freeTypeLibPtr);
            this.mockFreeTypeInvoker.Setup(m => m.FT_New_Face(It.IsAny<IntPtr>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(() =>
                {
                    // TODO: Eventually put this mocked glyph buffer data into a helper method
                    var faceBitmap = CreateGlyphBMPData(GlyphWidth, GlyphHeight);

                    this.glyphSlotRec.bitmap = faceBitmap;

                    // Setup the size metric data
                    this.sizeRec.metrics = CreateSizeMetrics(8, 5);

                    this.faceRec.size = TestHelpers.ToUnsafePointer(ref this.sizeRec);

                    // Setup the glyph metrics
                    this.glyphSlotRec.metrics = CreateGlypMetrics(5, 6, 13, 15, 7);

                    this.faceRec.glyph = TestHelpers.ToUnsafePointer(ref this.glyphSlotRec);

                    this.facePtr = TestHelpers.ToIntPtr(ref this.faceRec);

                    return this.facePtr;
                });

            this.mockImageService = new Mock<IImageService>();

            this.mockMonitorService = new Mock<ISystemMonitorService>();
            this.mockMonitorService.SetupGet(p => p.MainMonitor)
                .Returns(() =>
                {
                    return new SystemMonitor(this.mockPlatform.Object)
                    {
                        HorizontalScale = 1,
                        VerticalScale = 1,
                    };
                });

            this.mockPlatform = new Mock<IPlatform>();
            this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.Exists(FontFilePath)).Returns(true);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_InitializesFreeType()
        {
            // Act
            var service = CreateService();

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Init_FreeType(), Times.Once());
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateFontAtlas_WithNullOrEmptyFilePath_ThrowsException(string fontFilePath)
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                service.CreateFontAtlas(fontFilePath, It.IsAny<int>());
            }, "The font file path argument must not be null. (Parameter 'fontFilePath')");
        }

        [Fact]
        public void CreateFontAtlas_WhenFontFilePathDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            var service = CreateService();

            // Act & Assert
            AssertHelpers.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                service.CreateFontAtlas(FontFilePath, It.IsAny<int>());
            }, $"The file '{FontFilePath}' does not exist.");
        }

        [Fact]
        public void CreateFontAtlas_WhenInvoked_LoadsFontFace()
        {
            // Arrange
            var fontFilePath = $@"C:\temp\test-font.ttf";
            var service = CreateService();

            // Act
            var (actualAtlasTexture, atlasData) = service.CreateFontAtlas(fontFilePath, It.IsAny<int>());

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_New_Face(this.freeTypeLibPtr, fontFilePath, 0), Times.Once());
        }

        [Fact]
        public void CreateFontAtlas_WhenInvoked_SetsCharacterSize()
        {
            // Arrange
            var fontSize = 12;
            var sizeInPointsPtr = (IntPtr)(fontSize << 6);

            var service = CreateService();

            // Act
            service.CreateFontAtlas(FontFilePath, fontSize);

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Set_Char_Size(
                this.facePtr,
                sizeInPointsPtr,
                sizeInPointsPtr,
                96,
                96), Times.Once());
        }

        [Fact]
        public void CreateFontAtlas_WhenInvoked_CreatesAllGlyphImages()
        {
            // NOTE: The glyph index of '4' for the FT_Load_Glyph() call is FT_LOAD_RENDER

            // Arrange
            this.mockImageService.Setup(m => m.Draw(It.IsAny<ImageData>(), It.IsAny<ImageData>(), It.IsAny<Point>()))
                .Returns<ImageData, ImageData, Point>((src, dest, location) =>
                {
                    return TestHelpers.Draw(src, dest, location);
                });
            var totalGlyphs = 7;
            var service = CreateService();

            // Act
            var (actualImage, actualData) = service.CreateFontAtlas(FontFilePath, 12);

            // Save the the results
            TestHelpers.SaveImageForTest(actualImage);

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Get_Char_Index(this.facePtr, It.IsAny<uint>()), Times.Exactly(totalGlyphs));
            this.mockFreeTypeInvoker.Verify(m => m.FT_Load_Glyph(this.facePtr, It.IsAny<uint>(), 4), Times.Exactly(totalGlyphs));
            this.mockImageService.Verify(m => m.Draw(It.IsAny<ImageData>(), It.IsAny<ImageData>(), It.IsAny<Point>()), Times.Exactly(totalGlyphs));

            AssertHelpers.Equals(16, actualImage.Width, $"The resulting font atlas texture width is invalid.");
            AssertHelpers.Equals(36, actualImage.Height, $"The resulting font atlas texture height is invalid.");
            AssertHelpers.Equals(576, actualImage.Pixels.Length, $"The number of atlas image pixels is invalid.");
            Assert.Equal(this.glyphChars.Length, actualData.Length);
        }

        [Fact]
        public void Dispose_WhenInvoked_ProperlyDisposesOfFreeType()
        {
            // Arrange
            var service = CreateService();
            service.CreateFontAtlas(FontFilePath, It.IsAny<int>());

            // Act
            service.Dispose();
            service.Dispose();

            // Assert
            this.mockFreeTypeInvoker.Verify(m => m.FT_Done_Face(this.facePtr), Times.Once());
            this.mockFreeTypeInvoker.Verify(m => m.FT_Done_Glyph(TestHelpers.ToIntPtr(ref this.glyphSlotRec)), Times.Once());
            this.mockFreeTypeInvoker.Verify(m => m.FT_Done_FreeType(this.freeTypeLibPtr), Times.Once());
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="FontAtlasService"/> for the purposes of testing.
        /// </summary>
        /// <returns>An instance to use for testing.</returns>
        private FontAtlasService CreateService()
        {
            var result = new FontAtlasService(
                this.mockFreeTypeInvoker.Object,
                this.mockImageService.Object,
                this.mockMonitorService.Object,
                this.mockFile.Object);

            result.SetAvailableCharacters(this.glyphChars);

            return result;
        }

        [ExcludeFromCodeCoverage]
        private FT_Bitmap CreateGlyphBMPData(int width, int height)
        {
            var bmpBufferBytes = new List<byte>();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    bmpBufferBytes.Add(255);
                }
            }

            // Setup the face data required to satisfy the test
            var faceBitmap = default(FT_Bitmap);
            faceBitmap.width = 5;
            faceBitmap.rows = 6;

            fixed (byte* bufferPtr = bmpBufferBytes.ToArray())
            {
                faceBitmap.buffer = (IntPtr)bufferPtr;
            }

            return faceBitmap;
        }

        [ExcludeFromCodeCoverage]
        private FT_Size_Metrics CreateSizeMetrics(int width, int height)
        {
            FT_Size_Metrics sizeMetrics = default;
            sizeMetrics.ascender = new IntPtr(width << 6);
            sizeMetrics.descender = new IntPtr(height << 6);

            return sizeMetrics;
        }

        // TODO: Add code docs
        private FT_Glyph_Metrics CreateGlypMetrics(int width, int height, int horiAdvance, int horiBearingX, int horiBearingY)
        {
            FT_Glyph_Metrics glyphMetrics = default;
            glyphMetrics.width = new IntPtr(width << 6);
            glyphMetrics.height = new IntPtr(height << 6);
            glyphMetrics.horiAdvance = new IntPtr(horiAdvance << 6);
            glyphMetrics.horiBearingX = new IntPtr(horiBearingX << 6);
            glyphMetrics.horiBearingY = new IntPtr(horiBearingY << 6);

            return glyphMetrics;
        }
    }
}
