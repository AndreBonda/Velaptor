﻿// <copyright file="GPUDataTypeExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ExtensionMethods;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using OpenGL;
using OpenGL.GPUData;

/// <summary>
/// Provides extensions methods for GPU data related types.
/// </summary>
[SuppressMessage("csharpsquid", "S101", Justification = "Acronym is acceptable .")]
internal static class GPUDataTypeExtensions
{
        /// <summary>
    /// Updates the <see cref="RectVertexData.VertexPos"/> using the given <paramref name="vertexNumber"/> for the given <paramref name="gpuData"/>.
    /// The end result will also be converted to NDC coordinates.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="pos">The position to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetVertexPos(this RectGPUData gpuData, Vector2 pos, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            pos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsSolid,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.VertexPos"/> using the given <paramref name="vertexNumber"/> for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="pos">The position to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static LineGPUData SetVertexPos(this LineGPUData gpuData, Vector2 pos, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new LineVertexData(
            pos,
            oldVertex.Color);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new LineGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new LineGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new LineGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new LineGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Rectangle"/> of a vertex using the given <paramref name="vertexNumber"/> for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="rect">The rectangle to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetRectangle(this RectGPUData gpuData, Vector4 rect, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            rect,
            oldVertex.Color,
            oldVertex.IsSolid,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Rectangle"/> for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="rectangle">The rectangle to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetRectangle(this RectGPUData gpuData, Vector4 rectangle)
    {
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.One);
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.Two);
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.Three);
        gpuData = gpuData.SetRectangle(rectangle, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.IsSolid"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="isSolid">The solid setting to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetAsSolid(this RectGPUData gpuData, bool isSolid, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            isSolid,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.IsSolid"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="isSolid">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetAsSolid(this RectGPUData gpuData, bool isSolid)
    {
        gpuData = gpuData.SetAsSolid(isSolid, VertexNumber.One);
        gpuData = gpuData.SetAsSolid(isSolid, VertexNumber.Two);
        gpuData = gpuData.SetAsSolid(isSolid, VertexNumber.Three);
        gpuData = gpuData.SetAsSolid(isSolid, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BorderThickness"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="borderThickness">The border thickness to apply to the vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBorderThickness(this RectGPUData gpuData, float borderThickness, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsSolid,
            borderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BorderThickness"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="borderThickness">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBorderThickness(this RectGPUData gpuData, float borderThickness)
    {
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.One);
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.Two);
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.Three);
        gpuData = gpuData.SetBorderThickness(borderThickness, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopLeftCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topLeftCornerRadius">The top left corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopLeftCornerRadius(this RectGPUData gpuData, float topLeftCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsSolid,
            oldVertex.BorderThickness,
            topLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopLeftCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topLeftCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopLeftCornerRadius(this RectGPUData gpuData, float topLeftCornerRadius)
    {
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetTopLeftCornerRadius(topLeftCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomLeftCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomLeftCornerRadius">The bottom left corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomLeftCornerRadius(this RectGPUData gpuData, float bottomLeftCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsSolid,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            bottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomLeftCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomLeftCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomLeftCornerRadius(this RectGPUData gpuData, float bottomLeftCornerRadius)
    {
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetBottomLeftCornerRadius(bottomLeftCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomRightCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomRightCornerRadius">The bottom right corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomRightCornerRadius(this RectGPUData gpuData, float bottomRightCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsSolid,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            bottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.BottomRightCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="bottomRightCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetBottomRightCornerRadius(this RectGPUData gpuData, float bottomRightCornerRadius)
    {
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetBottomRightCornerRadius(bottomRightCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopRightCornerRadius"/> setting of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topRightCornerRadius">The top right corner radius to apply to a vertex.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopRightCornerRadius(this RectGPUData gpuData, float topRightCornerRadius, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            oldVertex.Color,
            oldVertex.IsSolid,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            topRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.TopRightCornerRadius"/> setting for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="topRightCornerRadius">The setting to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetTopRightCornerRadius(this RectGPUData gpuData, float topRightCornerRadius)
    {
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.One);
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.Two);
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.Three);
        gpuData = gpuData.SetTopRightCornerRadius(topRightCornerRadius, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Color"/> of a vertex using the given <paramref name="vertexNumber"/>
    /// for the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="color">The color to set the vertex to.</param>
    /// <param name="vertexNumber">The vertex to update.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetColor(this RectGPUData gpuData, Color color, VertexNumber vertexNumber)
    {
        var oldVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
            _ => throw new ArgumentOutOfRangeException(nameof(vertexNumber), "The vertex number is invalid.")
        };

        var newVertexData = new RectVertexData(
            oldVertex.VertexPos,
            oldVertex.Rectangle,
            color,
            oldVertex.IsSolid,
            oldVertex.BorderThickness,
            oldVertex.TopLeftCornerRadius,
            oldVertex.BottomLeftCornerRadius,
            oldVertex.BottomRightCornerRadius,
            oldVertex.TopRightCornerRadius);

#pragma warning disable CS8524
        return vertexNumber switch
        {
            VertexNumber.One => new RectGPUData(newVertexData, gpuData.Vertex2, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Two => new RectGPUData(gpuData.Vertex1, newVertexData, gpuData.Vertex3, gpuData.Vertex4),
            VertexNumber.Three => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, newVertexData, gpuData.Vertex4),
            VertexNumber.Four => new RectGPUData(gpuData.Vertex1, gpuData.Vertex2, gpuData.Vertex3, newVertexData),
        };
#pragma warning restore CS8524
    }

    /// <summary>
    /// Updates the <see cref="RectVertexData.Color"/> for all of the vertex data in the given <paramref name="gpuData"/>.
    /// </summary>
    /// <param name="gpuData">The GPU data to update.</param>
    /// <param name="color">The color to apply to all vertex data.</param>
    /// <returns>The updated GPU data.</returns>
    public static RectGPUData SetColor(this RectGPUData gpuData, Color color)
    {
        gpuData = gpuData.SetColor(color, VertexNumber.One);
        gpuData = gpuData.SetColor(color, VertexNumber.Two);
        gpuData = gpuData.SetColor(color, VertexNumber.Three);
        gpuData = gpuData.SetColor(color, VertexNumber.Four);

        return gpuData;
    }

    /// <summary>
    /// Sets the color of the <see cref="LineGPUData"/> to the given <paramref name="color"/>.
    /// </summary>
    /// <param name="gpuData">The line GPU data.</param>
    /// <param name="color">The color to set.</param>
    /// <returns>The <see cref="LineGPUData"/> with the new color applied.</returns>
    public static LineGPUData SetColor(this LineGPUData gpuData, Color color)
    {
        var newVertex1 = new LineVertexData(gpuData.Vertex1.VertexPos, color);
        var newVertex2 = new LineVertexData(gpuData.Vertex2.VertexPos, color);
        var newVertex3 = new LineVertexData(gpuData.Vertex3.VertexPos, color);
        var newVertex4 = new LineVertexData(gpuData.Vertex4.VertexPos, color);

        return new LineGPUData(newVertex1, newVertex2, newVertex3, newVertex4);
    }
}
