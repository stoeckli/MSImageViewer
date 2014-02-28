#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="WiffSample.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 28/11/2011 - Implementation of new dot net Assemblies
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System;
using System.IO;
using System.Windows;

using Clearcore2.Data.DataAccess.SampleData;

using Novartis.Msi.Core;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    using System.Globalization;

    /// <summary>
    /// This class encapsulates a wiff file sample.
    /// </summary>
    public class WiffSample
    {
        #region Fields

        /// <summary>
        /// The <b>one-based</b> index of this sample in the wiff file.
        /// </summary>
        private readonly int index;

        /// <summary>
        /// The list of experiments this sample consists of.
        /// </summary>
        private readonly WiffExperimentList experiments = new WiffExperimentList();

        /// <summary>
        /// >The <see cref="WiffFileContent"/> instance this sample belongs to.
        /// </summary>
        private readonly WiffFileContent wiffFileContent;

        /// <summary>
        /// String that contains the Maldi Parameters
        /// </summary>
        private readonly string maldiparametersstring;

        /// <summary>
        /// User Data
        /// </summary>
        private double x1;

        /// <summary>
        /// User Data
        /// </summary>
        private double x2;

        /// <summary>
        /// User Data
        /// </summary>
        private double y1;

        /// <summary>
        /// User Data
        /// </summary>
        private double y2;

        /// <summary>
        /// User Data
        /// </summary>
        private double width;

        /// <summary>
        /// User Data
        /// </summary>
        private double height;

        /// <summary>
        /// The name of this sample.
        /// </summary>
        private string name;

        /// <summary>
        /// Array for all the image data
        /// </summary>
        private uint[,] positionData;

        /// <summary>
        /// The count of data triplets in <see cref="PositionData"/>
        /// </summary>
        private long positionDataLength;

        /// <summary>
        /// Scan File Size
        /// </summary>
        private long scanfilesize;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WiffSample"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="wiffFile"> The source from where to read the samples properties and data </param>
        /// <param name="wiffFileContent"> instance this sample belongs to </param>
        /// <param name="sampleIndex"> value specifying the <b>one-based</b>index of this sample in the  </param>
        /// <param name="maldiparamstr"> String that contains the Maldi Parameters </param>         
        public WiffSample(Batch wiffFile, WiffFileContent wiffFileContent, int sampleIndex, string maldiparamstr)
        {
            if (wiffFile == null)
            {
                throw new ArgumentNullException("wiffFile");
            }

            if (wiffFileContent == null)
            {
                throw new ArgumentNullException("wiffFileContent");
            }

            if (sampleIndex < 0)
            {
                throw new ArgumentOutOfRangeException("sampleIndex");
            }

            this.maldiparametersstring = maldiparamstr;
            this.index = sampleIndex;
            this.wiffFileContent = wiffFileContent;

            this.Initialize(wiffFile);
        }

        #endregion Constructors 

        #region Properties

        /// <summary>
        /// Gets the height of this sample.
        /// </summary>
        public double Height
        {
            get { return this.height; }
        }
          
        /// <summary>
        /// Gets the width of this sample.
        /// </summary>
        public double Width
        {
            get { return this.width; }
        }  
        
        /// <summary>
        /// Gets the one-based index of this sample in the wiff file.
        /// </summary>
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        /// Gets the name of this sample.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the first point (X1,Y1) of this sample as a <see cref="Point"/> structure.
        /// </summary>
        public Point P1
        {
            get
            {
                var p1 = new Point(this.x1, this.y1);
                return p1;
            }
        }

        /// <summary>
        /// Gets the last point (X1,Y1) of this sample as a <see cref="Point"/> structure.
        /// </summary>
        public Point P2
        {
            get
            {
                var p2 = new Point(this.x2, this.y2);
                return p2;
            }
        }

        /// <summary>
        /// Gets the position data of this sample. 
        /// The data layout is: uint[PositionDataLength, 3];
        /// </summary>
        public uint[,] PositionData
        {
            get { return this.positionData; }
        }

        /// <summary>
        /// Gets the count of data triplets in <see cref="PositionData"/>.
        /// </summary>
        public long PositionDataLength
        {
            get { return this.positionDataLength; }
        }

        /// <summary>
        /// Gets the WiffFileContent instance this sample belongs to.
        /// </summary>
        public WiffFileContent WiffFileContent
        {
            get { return this.wiffFileContent; }
        }

        /// <summary>
        /// Gets the Maldi Parameter String
        /// </summary>
        public string MaldiParametersString
        {
            get { return this.maldiparametersstring; } 
        }

        /// <summary>
        /// Gets or sets the size of the scan file, used for binning
        /// </summary>
        public long ScanFileSize
        {
            get
            {
                return this.scanfilesize;
            }

            set
            {
                this.scanfilesize = value;
            }
        }

        #endregion Properties 

        #region Methods

        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="wiffFile"/>.
        /// </summary>
        /// <param name="wiffFile">A <see cref="Batch"/> instance. The data source.</param>
        private void Initialize(Batch wiffFile)
        {
            Sample sample = wiffFile.GetSample(this.index);

            // retrieve this sample's name
            this.name = sample.Details.SampleName;

            // get the position information for the selected sample the position data exists 
            // in a seperate file ( one per sample ) and is read from that file 'manually'
            AppContext.ProgressStart("reading path file");
            try 
            {
                // 1) Set the various WiffSample Parameters
                string pathFile = this.wiffFileContent.FileName + " (" + (this.index + 1).ToString(CultureInfo.InvariantCulture) + ").path";

                // Make sure the path file exists
                if (!File.Exists(pathFile))
                {
                    MessageBox.Show(pathFile + " is missing", "Missing File");
                    return;
                }

                var positionStream = new FileStream(@pathFile, FileMode.Open, FileAccess.Read);

                // calculate the count of position entries (12 bytes per position entry)
                this.positionDataLength = positionStream.Length / 12; 

                this.x1 = 1e10;
                this.x2 = 0.0;
                this.y1 = 1e10;
                this.y2 = 0.0;

                // the array to hold the position information
                this.positionData = new uint[this.positionDataLength, 3];
                var positionReader = new BinaryReader(positionStream);
                for (long posindex = 0; posindex < this.positionDataLength; posindex++)
                {
                    this.positionData[posindex, 0] = positionReader.ReadUInt32();
                    this.positionData[posindex, 1] = positionReader.ReadUInt32();
                    this.positionData[posindex, 2] = positionReader.ReadUInt32();

                    // hundred progress ticks 
                    if (Equals(posindex % (this.positionDataLength / 100.0), 0.0)) 
                    {
                        AppContext.ProgressSetValue(100.0 * posindex / this.positionDataLength);
                    }

                    if (this.positionData[posindex, 1] < this.x1)
                    {
                        this.x1 = this.positionData[posindex, 1];
                    }

                    if (this.positionData[posindex, 1] > this.x2)
                    {
                        this.x2 = this.positionData[posindex, 1];
                    }

                    if (this.positionData[posindex, 2] < this.y1)
                    {
                        this.y1 = this.positionData[posindex, 2];
                    }

                    if (this.positionData[posindex, 2] > this.y2)
                    {
                        this.y2 = this.positionData[posindex, 2];
                    }
                }

                positionReader.Close();
                positionStream.Close();

                this.x1 /= 1000;
                this.x2 /= 1000;
                this.y1 /= 1000;
                this.y2 /= 1000;

                this.width = this.x2 - this.x1;
                this.height = this.y2 - this.y1;
            }
            finally
            {
                AppContext.ProgressClear();
            }

            // 2) Get the size of the scan file  set scanfilesize
            string scanpathFile = this.wiffFileContent.FileName + ".scan";
            var fileinfo = new FileInfo(scanpathFile);
            this.scanfilesize = fileinfo.Length;

            MassSpectrometerSample massSpecSample = sample.MassSpectrometerSample;
            int numberOfExperiments = massSpecSample.ExperimentCount;

            // 3) Get number of experiments and create WiffExperiments for each
            // loop through the experiments; the index of the experiments is zero based!!
            for (int actExperiment = 0; actExperiment < numberOfExperiments; actExperiment++)
            {
                MSExperiment msexperiment = massSpecSample.GetMSExperiment(actExperiment);

                WiffExperiment experiment = WiffExperimentFactory.CreateWiffExperiment(msexperiment, this);
                
                this.experiments.Add(experiment);
            }
        }

        #endregion Methods 
    } 
}
