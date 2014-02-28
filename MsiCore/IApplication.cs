#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IApplication.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

namespace Novartis.Msi.Core
{
    #region IApplication Interface

    /// <summary>
    /// Interface implemented by MainWindow to expose some functionality to external entities.
    /// </summary>
    public interface IApplication
    {
        #region Properties

        /// <summary>
        /// Gets or sets the text displayed in the statusbar info item of the application.
        /// </summary>
        /// <value><see langword="string"/></value>
        string StatusInfo { get; set; }

        /// <summary>
        /// Gets or sets the text displayed in the statusbar coordinate item of the application.
        /// </summary>
        /// <value>A <see langword="string"/> value.</value>
        /// <remarks>
        /// The text-string usually contains the two formatted coordinate-values
        /// delimited by a tabulator (<c>\t</c>).
        /// </remarks>
        string StatusCoord { get; set; }

        /// <summary>
        /// Gets or sets the text displayed in the statusbar intensity item of the application.
        /// </summary>
        /// <value>A <see langword="string"/> value.</value>
        string StatusIntensity { get; set; }

        /// <summary>
        /// Gets the currently active view of the application.
        /// </summary>
        /// <value>The active view as <see cref="IView"/> reference.</value>
        IView ActiveView { get; }

        /// <summary>
        /// Gets or sets the index of the currently selected palette to use for image representation.
        /// </summary>
        int PaletteIndex { get; set; }

        /// <summary>
        /// Gets or sets the minimum intensity settings currently in effect for the active view.
        /// </summary>
        IntensitySettings MinIntensitySettings { get; set; }

        /// <summary>
        /// Gets or sets the minimum intensity settings currently in effect for the active view.
        /// </summary>
        IntensitySettings MaxIntensitySettings { get; set; }

        #endregion Properties

        #region View Methods

        /// <summary>
        /// Adds the given view to the mdi gui.
        /// </summary>
        /// <param name="view">The view to be added as a <see cref="IView"/> reference.</param>
        /// <returns>A <see cref="bool"/> value indicating the success.</returns>
        bool AddView(IView view);

        /// <summary>
        /// Removes the given view from the mdi gui.
        /// </summary>
        /// <param name="view">The view to be removed as a <see cref="IView"/> reference.</param>
        /// <returns>A <see cref="bool"/> value indicating the success.</returns>
        bool RemoveView(IView view);

        /// <summary>
        /// Removes the given view from the mdi gui and closes it.
        /// </summary>
        /// <param name="view">The view to be removed as a <see cref="IView"/> reference.</param>
        /// <returns>A <see cref="bool"/> value indicating the success.</returns>
        bool CloseView(IView view);

        /// <summary>
        /// Starts a progressbar action. The specified <paramref name="operation"/> identifier and the progressbar are shown and the
        /// progressbar is made ready to receive values.
        /// </summary>
        /// <param name="operation">A identifier describing the current operation.</param>
        void ProgressStart(string operation);

        /// <summary>
        /// Sets the Progressbar to show the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to be shown in the progressbar. The range of values is expected to be 0.0 to 100.0</param>
        void ProgressSetValue(double value);

        /// <summary>
        /// Stops the current progressbar action. Clears and hides the progressbar.
        /// </summary>
        void ProgressClear();

        #endregion View Methods
    }

    #endregion IApplication Interface


}
