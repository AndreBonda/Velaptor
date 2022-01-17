﻿// <copyright file="IFont.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using VelFontStyle = Velaptor.Content.Fonts.FontStyle;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// The font to use when rendering text to the screen.
    /// </summary>
    public interface IFont : IContent
    {
        /// <summary>
        /// Gets the source of where the font was loaded.
        /// </summary>
        FontSource Source { get; }

        /// <summary>
        /// Gets the font atlas texture that contains all of the bitmap data for all of the available glyphs for the font.
        /// </summary>
        ITexture FontTextureAtlas { get; }

        /// <summary>
        /// Gets or sets the size of the font in points.
        /// </summary>
        uint Size { get; set; }

        /// <summary>
        /// Gets or sets the style of the font.
        /// </summary>
        VelFontStyle Style { get; set; }

        /// <summary>
        /// Gets a list of all the available font styles for the current font <see cref="FamilyName"/>.
        /// </summary>
        IEnumerable<FontStyle> AvailableStylesForFamily { get; }

        /// <summary>
        /// Gets the name of the font family.
        /// </summary>
        string FamilyName { get; }

        /// <summary>
        /// Gets a value indicating whether or not the font has kerning for text rendering layout.
        /// </summary>
        bool HasKerning { get; }

        /// <summary>
        /// Gets the spacing between lines of text in pixels.
        /// </summary>
        float LineSpacing { get; }

        /// <summary>
        /// Gets the list of metrics for all of the glyphs supported by the font.
        /// </summary>
        /// <returns>The glyph metrics.</returns>
        ReadOnlyCollection<GlyphMetrics> Metrics { get; }

        /// <summary>
        /// Measures the width and height bounds of the give <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>The width and height of the text in pixels.</returns>
        SizeF Measure(string text);

        /// <summary>
        /// Gets the glyph metrics using the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to get the metrics for.</param>
        /// <returns>The metrics of each individual glyph/character.</returns>
        GlyphMetrics[] ToGlyphMetrics(string text);

        /// <summary>
        /// Gets the kerning between 2 glyphs using the given <paramref name="leftGlyphIndex"/> and <paramref name="rightGlyphIndex"/>.
        /// </summary>
        /// <param name="leftGlyphIndex">The index of the left glyph.</param>
        /// <param name="rightGlyphIndex">The index of the right glyph.</param>
        /// <returns>The kerning result between the glyphs.</returns>
        /// <remarks>Refer to https://freetype.org/freetype2/docs/glyphs/glyphs-4.html for more info.</remarks>
        float GetKerning(uint leftGlyphIndex, uint rightGlyphIndex);
    }
}
