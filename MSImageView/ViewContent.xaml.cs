#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ViewContent.xaml.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 06/12/2012 - Style Cop Updates
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.MSImageView
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Novartis.Msi.Core;

    /// <summary>
    /// Interaction logic for ViewContent.xaml
    /// </summary>
    public partial class ViewContent
    {
        #region Fields

        /// <summary>
        /// The View
        /// </summary>
        private IView view;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewContent"/> class
        /// </summary>
        /// <param name="viewControl">User Control to be viewed</param>
        public ViewContent(UserControl viewControl)
        {
            try
            {
                if (viewControl == null)
                {
                    throw new ArgumentNullException("viewControl");
                }

                this.view = viewControl as IView;

                InitializeComponent();

                LayoutRoot.Children.Add(viewControl);
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Gets the View
        /// </summary>
        public IView View
        {
            get { return this.view; }
        }

        /// <summary>
        /// Detaches the View
        /// </summary>
        /// <returns>True or False Depending on successful detachment of view</returns>
        public bool DetachView()
        {
            try
            {
                foreach (UIElement element in LayoutRoot.Children)
                {
                    var viewControl = element as IView;
                    if (viewControl == this.view)
                    {
                        LayoutRoot.Children.Remove(element);
                        this.view = null;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }

            return false;
        }

        /// <summary>
        /// Document Closing method
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Argument</param>
        private void DocumentContentClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try 
            {
                if (this == sender)
                {
                    var appWindow = AppContext.MainWindow as MainWindow;
                    if (appWindow != null)
                    {
                        appWindow.OnViewClosing(this, e);
                        this.view = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        #endregion
    }
}
