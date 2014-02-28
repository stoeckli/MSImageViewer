#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="BitmapWriter.cs" company="Novartis Pharma AG.">
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

using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Writes bitmaps to file or stream.
    /// </summary>
    public abstract class BitmapWriter : IBitmapWriter
    {
        #region Fields

        /// <summary>
        /// Supported file types.
        /// </summary>
        protected readonly FileTypeDescriptorList supportedFileTypes;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the BitmapWriter class
        /// </summary>
        protected BitmapWriter()
        {
            // Create file descriptor:
            this.supportedFileTypes = new FileTypeDescriptorList();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of this writer.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the list of supported file types.
        /// </summary>
        public virtual FileTypeDescriptorList SupportedFileTypes
        {
            get { return this.supportedFileTypes; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes the bimap data specified by the given <see cref="BitmapSource"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="bitmap">The bitmap data to be written to file.</param>
        /// <param name="fileName">The file to which the bitmap data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        public virtual bool Write(BitmapSource bitmap, string fileName, bool showProgress)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            try
            {
                using (var outStream = new FileStream(fileName, FileMode.Create))
                {
                    return this.Write(bitmap, outStream, showProgress);
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
                return false;
            }
        }

        /// <summary>
        /// Writes the bimap data specified by the given <see cref="BitmapSource"/> object
        /// to the given stream.
        /// </summary>
        /// <param name="bitmap">The bitmap data to be written to stream.</param>
        /// <param name="outStream">The stream to which the bitmap data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        public abstract bool Write(BitmapSource bitmap, Stream outStream, bool showProgress);

        #endregion Methods
    }
}
