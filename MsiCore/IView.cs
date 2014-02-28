#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IView.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated:Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System;
    using System.ComponentModel;

    #region Delegates
    
    /// <summary>
    /// A delegate type for view related notifications.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">Event Args</param>
    public delegate void ViewEventHandler(IView sender, EventArgs e);

    /// <summary>
    /// A delegate type for view related, cancelable notifications.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">Event Args</param>
    public delegate void ViewCancelEventHandler(IView sender, CancelEventArgs e);

    #endregion  Delegates

    /// <summary>
    /// Mandatory interface for all ImageViewer application's views.
    /// </summary>
    public interface IView
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="Document"/> object this view belongs to.
        /// </summary>
        /// <value>
        /// A <see cref="Document"/> reference.
        /// </value>
        Document Document
        {
            get;
        }

        /// <summary>
        /// Gets the <see cref="ViewController"/> object of this view.
        /// </summary>
        /// <value>
        /// A <see cref="ViewController"/> reference.
        /// </value>
        ViewController ViewController
        {
            get;
        }

        /// <summary>
        /// Gets the name of this view.
        /// </summary>
        /// <value> A <see cref="string"/> object containing the name.</value>
        string Title
        {
            get;
        }

        /// <summary>
        /// Gets the view object of this view. A view object is an object characterizes a view.
        /// There is a 1 to 1 relationship between IView and IViewObject.
        /// </summary>
        /// <value>The views view object as IViewObject reference.</value>
        IViewObject ViewObject
        {
            get;
        }

        /// <summary>
        /// Gets the Experiment Type that we are currently viewing
        /// </summary>
        ExperimentType ViewExperimentType
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the <see cref="ViewController"/>-object that connects this
        /// view with the model data.
        /// </summary>
        /// <param name="viewCtrl">View Controller Object</param>
        void SetViewController(ViewController viewCtrl);

        /// <summary>
        /// Close this view finally.
        /// </summary>
        void Close();

        /// <summary>
        /// Update the views content.
        /// </summary>
        void Update();

        #endregion Methods
    }
}
