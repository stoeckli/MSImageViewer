#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ExtensionMethods.cs"  company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 06/12/2011 - StyleCop Updates
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.MSImageView
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Extentsion methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// An Empty Delegate
        /// </summary>
        private static readonly Action EmptyDelegate = delegate { };

        /// <summary>
        /// Force a re-rendering of the given UIElement.
        /// </summary>
        /// <param name="uiElement">Ui Element</param>
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}