#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="JpegWriter.cs" company="Novartis Pharma AG.">
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

using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// A specialized BitmapWriter instance producing jpeg files
    /// </summary>
    public class JpegWriter : BitmapWriter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="JpegWriter"/> class
        /// </summary>
        public JpegWriter()
        {
            this.Initialise();
        }
        
        #endregion Constructor

        #region Properties

        /// <summary>
        /// The name of this BitmapWriter instance.
        /// </summary>
        public override string Name
        {
            get { return "JPEG Writer"; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes the bimap data specified by the given <see cref="BitmapSource"/> object JPEG format
        /// to the given stream.
        /// </summary>
        /// <param name="bitmap">The bitmap data to be written to stream.</param>
        /// <param name="outStream">The stream to which the bitmap data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        public override bool Write(BitmapSource bitmap, Stream outStream, bool showProgress)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }

            if (outStream == null)
            {
                throw new ArgumentNullException("outStream");
            }

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.QualityLevel = 100;
            encoder.Save(outStream);

            return true;
        }

        /// <summary>
        /// Initialises various parameters, called from contructor
        /// </summary>
        private void Initialise()
        {
            this.SupportedFileTypes.Add(new FileTypeDescriptor(Strings.JpegFileDescription, ".jpg"));
        }

        #endregion Methods
    }
}
