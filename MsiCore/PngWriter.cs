﻿#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="PngWriter.cs" company="Novartis Pharma AG.">
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
    /// A specialized BitmapWriter instance producing png files
    /// </summary>
    public class PngWriter : BitmapWriter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PngWriter"/> class
        /// </summary>
        public PngWriter()
        {
            this.Initialise();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Name of this writer.
        /// </summary>
        /// <remarks>
        /// Every writer should have a unique name.
        /// </remarks>
        public override string Name
        {
            get { return "PNG Writer"; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes the bimap data specified by the given <see cref="BitmapSource"/> object PNG format
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

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(outStream);

            return true;
        }

        /// <summary>
        /// Initialise various parameter called in Constructor
        /// </summary>
        private void Initialise()
        {
            this.SupportedFileTypes.Add(new FileTypeDescriptor(Strings.PngFileDescription, ".png"));
        }

        #endregion Methods
    }
}
