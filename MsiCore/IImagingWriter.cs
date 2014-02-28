#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IImagingWriter.cs" company="Novartis Pharma AG.">
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

using System.IO;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Common interface for instances suitable to save
    /// <see cref="Imaging"/> instances to a persistent state in a specific
    /// file format.
    /// </summary>
    public interface IImagingWriter : IWriter
    {
        #region Methods

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="Imaging"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="imagingData">The image data to be written to file.</param>
        /// <param name="fileName">The file to which the bitmap data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        bool Write(Imaging imagingData, string fileName, bool showProgress);

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="Imaging"/> object
        /// to the given stream.
        /// </summary>
        /// <param name="imagingData">The image data to be written to file.</param>
        /// <param name="outStream">The stream to which the bitmap data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        bool Write(Imaging imagingData, Stream outStream, bool showProgress);

        #endregion Methods
    }
}
