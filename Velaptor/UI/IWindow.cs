﻿// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the core of an application window which facilitates how the
    /// window behaves, its state and the ability to be used in various types
    /// of applications.
    /// </summary>
    public interface IWindow : IWindowActions, IWindowProps, IDisposable
    {
        /// <summary>
        /// Shows the window.
        /// </summary>
        void Show();

        /// <summary>
        /// Shows the window asynchronously.
        /// </summary>
        /// <param name="afterStart">Executed after the application starts asynchronously.</param>
        /// <param name="afterUnloadAction">Executed after the window has been unloaded.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task ShowAsync(Action? afterStart = null, Action? afterUnloadAction = null);

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
