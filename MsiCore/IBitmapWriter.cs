#region Copyright © 2010 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
//
// © 2010 Novartis AG. All rights reserved.
//
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
//
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2010 Novartis AG

using System.IO;
using System.Windows.Media.Imaging;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Interface for all bitmap writers.
    /// </summary>
    public interface IBitmapWriter : IWriter
    {
        #region Methods

        /// <summary>
        /// Writes the bimap data specified by the given <see cref="BitmapSource"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="bitmap">The bitmap data to be written to file.</param>
        /// <param name="fileName">The file to which the bitmap data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        bool Write(BitmapSource bitmap, string fileName, bool showProgress);

        /// <summary>
        /// Writes the bimap data specified by the given <see cref="BitmapSource"/> object
        /// to the given stream.
        /// </summary>
        /// <param name="bitmap">The bitmap data to be written to stream.</param>
        /// <param name="outStream">The stream to which the bitmap data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        bool Write(BitmapSource bitmap, Stream outStream, bool showProgress);

        #endregion Methods
    }
}
