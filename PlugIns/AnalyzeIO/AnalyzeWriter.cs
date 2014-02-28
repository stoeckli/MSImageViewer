#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyzeWriter.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.AnalyzeIO
{
    using System;
    using System.IO;
    using Novartis.Msi.Core;

    /// <summary>
    /// AnalyzeWriter facilitates the output of <see cref="Imaging"/> data in "Analyze"
    /// format.
    /// </summary>
    public class AnalyzeWriter : IImagingWriter
    {
        #region Private Fields

        /// <summary>
        /// Supported File Types 
        /// </summary>
        private readonly FileTypeDescriptorList supportedFileTypes;

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeWriter"/> class 
        /// </summary>
        public AnalyzeWriter()
        {
            this.supportedFileTypes = new FileTypeDescriptorList
                {
                    new FileTypeDescriptor(Strings.AnalyzeFileDescription, Strings.AnalyzeFileExtension)
                };
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the Name of this writer.
        /// </summary>
        public string Name
        {
            get
            {
                return Strings.WriterName;
            }
        }

        /// <summary>
        /// Gets a List of supported file types.
        /// </summary>
        public virtual FileTypeDescriptorList SupportedFileTypes
        {
            get
            {
                return this.supportedFileTypes;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="Imaging"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="imagingData">The image data to be written to file.</param>
        /// <param name="fileName">The file to which the image data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        public bool Write(Imaging imagingData, string fileName, bool showProgress)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            try
            {
                if (imagingData is ImageData)
                {
                    return this.Write((ImageData)imagingData, fileName);
                }

                if (imagingData is ImageSpectrumData)
                {
                    return this.Write((ImageSpectrumData)imagingData, fileName);
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }

            return false;
        }

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="Imaging"/> object
        /// to the given stream.
        /// </summary>
        /// <param name="imagingData">The image data to be written to file.</param>
        /// <param name="outStream">The stream to which the image data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        public bool Write(Imaging imagingData, Stream outStream, bool showProgress)
        {
            throw new NotImplementedException("AnalyzeWriter.Write(Imaging imagingData, Stream outStream, bool showProgress)");
        }

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="ImageData"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="imgData"><see cref="ImageData"/> object to be written.</param>
        /// <param name="fileName">The file to which the image data will be written.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        private bool Write(ImageData imgData, string fileName)
        {
            if (imgData == null)
            {
                throw new ArgumentNullException("imgData");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            // setup the three stream of an Analyze file...
            var imgStream = new FileStream(fileName,                         FileMode.Create, FileAccess.Write);
            var hdrStream = new FileStream(fileName.Replace(".img", ".hdr"), FileMode.Create, FileAccess.Write);
            var ttmStream = new FileStream(fileName.Replace(".img", ".t2m"), FileMode.Create, FileAccess.Write);
             
            // and assign to writers...
            var imgWriter = new BinaryWriter(imgStream);
            var hdrWriter = new BinaryWriter(hdrStream);
            var ttmWriter = new BinaryWriter(ttmStream);

            // write the header file.
            // this is dark, obscure and intrinsic... 
            // ToDo JP 08/02/12 replace the stuff below with AnalyseFileHeaderObject
            hdrWriter.BaseStream.Position = 0;
            hdrWriter.Write(384);

            hdrWriter.BaseStream.Position = 4;
            hdrWriter.Write("        16");

            hdrWriter.BaseStream.Position = 32;
            hdrWriter.Write((short)16384);

            hdrWriter.BaseStream.Position = 38;
            hdrWriter.Write((byte)114);

            hdrWriter.BaseStream.Position = 40;
            hdrWriter.Write((short)4);

            hdrWriter.BaseStream.Position = 42;
            hdrWriter.Write((short)1);

            hdrWriter.BaseStream.Position = 44;
            hdrWriter.Write((short)imgData.XPoints);

            hdrWriter.BaseStream.Position = 46;
            hdrWriter.Write((short)imgData.YPoints);

            hdrWriter.BaseStream.Position = 48;
            hdrWriter.Write((short)1);

            hdrWriter.BaseStream.Position = 70;
            hdrWriter.Write((short)16);

            hdrWriter.BaseStream.Position = 72;
            hdrWriter.Write((short)16);

            hdrWriter.BaseStream.Position = 76;
            hdrWriter.Write((float)1);

            hdrWriter.BaseStream.Position = 80;
            hdrWriter.Write(imgData.Dx);

            hdrWriter.BaseStream.Position = 84;
            hdrWriter.Write(imgData.Dy);

            hdrWriter.BaseStream.Position = 88;
            hdrWriter.Write((float)1);

            foreach (MetaDataItem mdi in imgData.MetaData)
            {
                switch (mdi.Name)
                {
                    // JP replaced UInt16 with ushort
                    case "X1 (mm)":
                        hdrWriter.BaseStream.Position = 253;
                        hdrWriter.Write((ushort)float.Parse(mdi.ValueString));
                        break;
                    case "Y2 (mm)":
                        hdrWriter.BaseStream.Position = 255;
                        hdrWriter.Write((ushort)(82 - float.Parse(mdi.ValueString)));
                        break;
                }
            }

            hdrWriter.BaseStream.Position = 383;
            hdrWriter.Write((byte)0);
            
            // and cleanup
            hdrWriter.Close();

            // write the image formatted MS data...
            // for (int y = imgData.YPoints - 1; y >= 0; y--) // Old Way delete when new regime is working properly
            for (int y = 0; y < imgData.YPoints; y++)
            {
                for (int x = 0; x < imgData.XPoints; x++)
                {
                    float intensity = imgData.Data[x][y];
                    imgWriter.Write(intensity);
                }
            }
            
            // and cleanup
            imgWriter.Close();

            // write the "calibration" info
            ttmWriter.Write(1.0f);

            // and cleanup
            ttmWriter.Close();

            return true;
        }

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="ImageSpectrumData"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="specData"><see cref="ImageSpectrumData"/> object to be written.</param>
        /// <param name="fileName">The file to which the image data will be written.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        private bool Write(ImageSpectrumData specData, string fileName)
        {
            if (specData == null)
            {
                throw new ArgumentNullException("specData");
            }

            if (string.IsNullOrEmpty(fileName))
            {                
                throw new ArgumentException("fileName");
            }

            // setup the three stream of an Analyze file...
            var imgStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            var hdrStream = new FileStream(fileName.Replace(".img", ".hdr"), FileMode.Create, FileAccess.Write);
            var ttmStream = new FileStream(fileName.Replace(".img", ".t2m"), FileMode.Create, FileAccess.Write);

            // and assign to writers...
            var imgWriter = new BinaryWriter(imgStream);
            var hdrWriter = new BinaryWriter(hdrStream);
            var ttmWriter = new BinaryWriter(ttmStream);

            // write the header file.
            // this is dark, obscure and intrinsic... 
            // ToDo JP 08/02/12 replace the stuff below with AnalyseFileHeaderObject
            hdrWriter.BaseStream.Position = 0;
            hdrWriter.Write(384);

            hdrWriter.BaseStream.Position = 4;
            hdrWriter.Write("        16");

            hdrWriter.BaseStream.Position = 32;
            hdrWriter.Write((short)16384);

            hdrWriter.BaseStream.Position = 38;
            hdrWriter.Write((byte)114);

            hdrWriter.BaseStream.Position = 40;
            hdrWriter.Write((short)4);

            hdrWriter.BaseStream.Position = 42;
            hdrWriter.Write((short)specData.DataSets.Count); // There will be files that contain parameter greater than 16 bit

            hdrWriter.BaseStream.Position = 44;
            hdrWriter.Write((short)specData.XPoints);

            hdrWriter.BaseStream.Position = 46;
            hdrWriter.Write((short)specData.YPoints);

            hdrWriter.BaseStream.Position = 48;
            hdrWriter.Write((short)1);

            hdrWriter.BaseStream.Position = 70; // this read the data type
            hdrWriter.Write((short)16);

            hdrWriter.BaseStream.Position = 72;
            hdrWriter.Write((short)16);

            hdrWriter.BaseStream.Position = 76; // step size x
            hdrWriter.Write((float)1);

            hdrWriter.BaseStream.Position = 80;
            hdrWriter.Write(specData.Dx);

            hdrWriter.BaseStream.Position = 84;
            hdrWriter.Write(specData.Dy);

            hdrWriter.BaseStream.Position = 88;
            hdrWriter.Write((float)1);

            foreach (MetaDataItem mdi in specData.MetaData)
            {
                switch (mdi.Name)
                {
                    case "X1 (mm)":
                        hdrWriter.BaseStream.Position = 253;
                        hdrWriter.Write((ushort)float.Parse(mdi.ValueString));
                        break;
                    case "Y1 (mm)":
                        hdrWriter.BaseStream.Position = 255;
                        hdrWriter.Write((ushort)float.Parse(mdi.ValueString));
                        break;
                }
            }

            // and cleanup
            hdrWriter.BaseStream.Position = 383;
            hdrWriter.Write((byte)0);

            hdrWriter.Close();

            // write the image formatted MS data...
            for (int y = 0; y < specData.YPoints; y++)
            {
                for (int x = 0; x < specData.XPoints; x++)
                {
                    foreach (ImageData imgData in specData.DataSets)
                    {
                        float intensity = imgData.Data[x][y];
                        imgWriter.Write(intensity);
                    }
                }
            }

            // and cleanup
            imgWriter.Close();

            // write calibration file
            for (int i = 0; i < specData.DataSets.Count; i++)
            {
                // TODO JP - should this contain the  contents of specData.MassCal?
                // specData.MassStep is now 1 [specData.MinMass + (specData.MassStep * i)]
                ttmWriter.Write(specData.MassCal[0] + i);  //specData.MinMass
            }

            // and cleanup
            ttmWriter.Close();

            return true;
        }

        #endregion Methods
    }
}
