#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IBitmapSource.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
//
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System.Windows.Media.Imaging;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// The implementing object can be used as a source for bitmap
    /// </summary>
    public interface IBitmapSource
    {
        #region Methods

        /// <summary>
        /// Return the <see cref="BitmapSource"/> object representing the object implementing this interface.
        /// </summary>
        /// <returns>The <see cref="BitmapSource"/> object.</returns>
        BitmapSource GetBitmap();

        #endregion Methods
    }
}
