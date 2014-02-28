#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyseExperiments.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel 13/02/2012 - Analyse Loader Updates
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.AnalyzeIO
{
    using System;
    using System.IO;
    using System.Windows;
    using Novartis.Msi.Core;

    /// <summary>
    /// This implements the AnalyseExperiments class
    /// </summary>
    public class AnalyseExperiments
    {
        #region Fields

        /// <summary>
        /// The <see cref="AnalyseFileContent"/> instance this sample belongs to.
        /// </summary>
        private readonly AnalyseFileContent analyseFileContent;

        /// <summary>
        /// AnalyseFileDataObject holds all the data for the Analyze file parameters
        /// </summary>
        private readonly AnalyseFileHeaderObject analyseFileHeaderObject;

        /// <summary>
        /// Path of file that is read 
        /// </summary>
        private readonly string filePath;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyseExperiments"/> class. 
        /// </summary>
        /// <param name="analyseFileContent"> instance this sample belongs to </param>
        /// <param name="filePath">Path of Analyse file</param>
        public AnalyseExperiments(AnalyseFileContent analyseFileContent, string filePath)
        {
            if (analyseFileContent == null)
            {
                throw new ArgumentNullException("analyseFileContent");
            }

            this.analyseFileHeaderObject = new AnalyseFileHeaderObject(AnalyseFileHeaderObject.AnalyseReadorWriteEnum.Read);
            this.analyseFileContent = analyseFileContent;
            this.filePath = filePath;

            this.Initialize();
        }

        #endregion Constructors 

        #region Methods

        /// <summary>
        /// Does various things to Initialise the AnalyseFileContent object
        /// </summary>
        private void Initialize()
        {
            AppContext.ProgressStart("reading header file");

            try
            {
                // 1) Read Header
                this.ReadHeaderFile();

                // 2) Process Header
                this.ProcessHeader();

                // 3) Set Experiment Type
                if (this.analyseFileHeaderObject.ExperimentType == ExperimentType.MRM)
                {
                    // MRM scans will only have and load a single Transition 
                    new AnalyseMrmExperiment(this.analyseFileContent, this.filePath, this.analyseFileHeaderObject);
                }
                else
                {
                    new AnalyseMsExperiment(this.analyseFileContent, this.filePath, this.analyseFileHeaderObject);
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
            finally
            {
                AppContext.ProgressClear();
            }
        }

        /// <summary>
        /// Reads the hearder file and populates analyseFileHeaderObject
        /// </summary>
        private void ReadHeaderFile()
        {
            try
            {   // Read in header info
                string headerpath = this.filePath.Replace(".img", ".hdr");
                var hdrStream = new FileStream(headerpath, FileMode.Open, FileAccess.Read);

                var hdrReader = new BinaryReader(hdrStream);

                // Check header size and if it exceeds 384 return an error
                hdrReader.BaseStream.Position = 0;
                this.analyseFileHeaderObject.HeaderSize = hdrReader.ReadUInt16();

                // != 384
                if (this.analyseFileHeaderObject.HeaderSize > 384)
                {
                    MessageBox.Show("Error, header size exceeds 384 bytes", "Analyse Header File Error");
                    return;
                }

                hdrReader.BaseStream.Position = 4;
                this.analyseFileHeaderObject.UnUsedPos4 = hdrReader.ReadString();

                hdrReader.BaseStream.Position = 32;
                this.analyseFileHeaderObject.UnUsedPos32 = hdrReader.ReadUInt16();

                hdrReader.BaseStream.Position = 38;
                this.analyseFileHeaderObject.UnUsedPos38 = hdrReader.ReadChar();

                hdrReader.BaseStream.Position = 40;
                this.analyseFileHeaderObject.UnUsedPos40 = hdrReader.ReadChar();

                hdrReader.BaseStream.Position = 42;
                this.analyseFileHeaderObject.NumberOfScans = hdrReader.ReadInt16();

                hdrReader.BaseStream.Position = 44;
                this.analyseFileHeaderObject.NumberOfXPoints = hdrReader.ReadInt16();

                hdrReader.BaseStream.Position = 46;
                this.analyseFileHeaderObject.NumberOfYPoints = hdrReader.ReadInt16();

                hdrReader.BaseStream.Position = 48;
                this.analyseFileHeaderObject.UnUsedPos48 = hdrReader.ReadInt16();

                hdrReader.BaseStream.Position = 70;
                this.analyseFileHeaderObject.DataType = hdrReader.ReadUInt16();

                hdrReader.BaseStream.Position = 72;
                this.analyseFileHeaderObject.UsedPos72 = hdrReader.ReadInt16();

                hdrReader.BaseStream.Position = 76;
                this.analyseFileHeaderObject.DimensionInZ = hdrReader.ReadSingle();

                hdrReader.BaseStream.Position = 80;
                this.analyseFileHeaderObject.Dx = hdrReader.ReadSingle();

                hdrReader.BaseStream.Position = 84;
                this.analyseFileHeaderObject.Dy = hdrReader.ReadSingle();

                hdrReader.BaseStream.Position = 88;
                this.analyseFileHeaderObject.UsedPos88 = hdrReader.ReadSingle();

                hdrReader.BaseStream.Position = 253;
                this.analyseFileHeaderObject.X1 = hdrReader.ReadUInt16();
                this.analyseFileHeaderObject.X2 = (this.analyseFileHeaderObject.Dx * this.analyseFileHeaderObject.NumberOfXPoints) + this.analyseFileHeaderObject.X1;

                hdrReader.BaseStream.Position = 255;
                this.analyseFileHeaderObject.Y1 = hdrReader.ReadUInt16();
                this.analyseFileHeaderObject.Y2 = (this.analyseFileHeaderObject.Dy * this.analyseFileHeaderObject.NumberOfYPoints) + this.analyseFileHeaderObject.Y1;

                hdrStream.Close();
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// This routine takes the header object and does some post processing
        /// on the data read from the file
        /// </summary>
        private void ProcessHeader()
        {
            // 1) Set experiement type
            this.analyseFileHeaderObject.ExperimentType = this.analyseFileHeaderObject.NumberOfScans <= 1 ? ExperimentType.MRM : ExperimentType.MS;

            // 2) Meta Data
            var fileinfo = new FileInfo(this.filePath);
            this.analyseFileHeaderObject.Name = fileinfo.Name;

            // 3) For MS experiment we want to check how many scans we have.
            var imgfilesize = fileinfo.Length;

            // depending on img file size there will be files that contain parameter greater than 16 bit (NumberOfScans)
            // Test the value of this if 16 - float 64, double, 8 int, 4 short 16
            // DataType, Type, Size (4, int16, 2), (5, int32, 4), (16, single, 4), (64, double, 8)
            // depending on this value is in here we want to use this to open the binary file (img)
            if (imgfilesize > 32767)
            {
                if (this.analyseFileHeaderObject.DataType == 4)
                {
                    this.analyseFileHeaderObject.DataByteSize = 2;
                }
                else if (this.analyseFileHeaderObject.DataType == 8)
                {
                    this.analyseFileHeaderObject.DataByteSize = 4;
                }
                else if (this.analyseFileHeaderObject.DataType == 16)
                {
                    this.analyseFileHeaderObject.DataByteSize = 4;
                }
                else if (this.analyseFileHeaderObject.DataType == 64)
                {
                    this.analyseFileHeaderObject.DataByteSize = 8;
                }

                if (this.analyseFileHeaderObject.ExperimentType == ExperimentType.MS)
                {
                    // Check for div by zero
                    if (this.analyseFileHeaderObject.DataByteSize != 0)
                    {
                        // 4) Reset Number of scans
                        this.analyseFileHeaderObject.NumberOfScans = imgfilesize
                                                                        / this.analyseFileHeaderObject.NumberOfXPoints
                                                                        / this.analyseFileHeaderObject.NumberOfYPoints
                                                                        / this.analyseFileHeaderObject.DataByteSize;
                    }
                }
            }
            else
            {
                this.analyseFileHeaderObject.DataByteSize = 4;
            }
        }

        #endregion Methods
    }
}
