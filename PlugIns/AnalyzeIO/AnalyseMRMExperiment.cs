#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyseMRMExperiment.cs" company="Novartis Pharma AG.">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using Novartis.Msi.Core;

    /// <summary>
    /// This implements the AnalyseMRMExperiment class
    /// </summary>
    public class AnalyseMrmExperiment : AnalyseExperiment
    {
       #region Fields

        /// <summary>
        /// Path of Input file
        /// </summary>
        private string filePath;

        /// <summary>
        /// AnalyseFile Header Object 
        /// </summary>
        private AnalyseFileHeaderObject analyseFileHeaderObject;

        /// <summary>
        /// data array for intensity points
        /// </summary>
        private float[] data;

        /// <summary>
        /// Array for mean values
        /// </summary>
        private float meanValue;

        /// <summary>
        /// Array for median values
        /// </summary>
        private float medianValue;

        /// <summary>
        /// min intensity value
        /// </summary>
        private float minIntensityValue;

        /// <summary>
        /// max intensity value
        /// </summary>
        private float maxIntensityValue;

        #endregion Fields

       #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyseMrmExperiment"/> class.
        /// </summary>
        /// <param name="analysefilecontent">analyseFileContent Object</param>
        /// <param name="filePath">Path of file that is being read</param>
        /// <param name="analyseFileHeaderObject">Header Object</param>
        public AnalyseMrmExperiment(AnalyseFileContent analysefilecontent, string filePath, AnalyseFileHeaderObject analyseFileHeaderObject)
            : base(analysefilecontent, filePath, analyseFileHeaderObject)
        {
        }

        #endregion Constructor

       #region Methods

       /// <summary>
       /// Reads the image file for an MRM experiment
       /// </summary>
       /// <param name="analysefilecontent">analyse file content Object</param>
       /// <param name="filepath">Path of file </param>
       /// <param name="analysefileheaderobject">Header Object</param>        
       protected override void Initialise(AnalyseFileContent analysefilecontent, string filepath, AnalyseFileHeaderObject analysefileheaderobject)
        {
            this.filePath = filepath;
            this.analyseFileHeaderObject = analysefileheaderobject;

            // 1) Make sure the path file exists
            if (!File.Exists(this.filePath))
            {
                MessageBox.Show(this.filePath + " is missing", "Missing File");
                return;
            }

            // 1) Use BinaryReader  
            this.ReadImgFile();

            // 2) Convert data[]to ImageData[][]  & (3) And calc Mean and Median and minIntensityValue, maxIntensityValue
            var imageData = new float[this.analyseFileHeaderObject.NumberOfXPoints][];
            double meanSum = 0.0;
            var valuesForMedian = new List<float>();

            this.minIntensityValue = 0;
            this.maxIntensityValue = 0;

            int numDataPoint = this.data.Length;

            for (int i = 0; i < imageData.Length; i++)
            {
                imageData[i] = new float[this.analyseFileHeaderObject.NumberOfYPoints];
            }

            // TODO JP If data[i + j] is out of scope, exit loops and continue - remaining array points will be null
            try
            {
                for (int j = 0; j < this.analyseFileHeaderObject.NumberOfYPoints; j++)
                {
                    for (int i = 0; i < this.analyseFileHeaderObject.NumberOfXPoints; i++)
                    {
                        int index;

                        if (j == 0)
                        {
                            index = i;
                        }
                        else
                        {
                            index = i + this.analyseFileHeaderObject.NumberOfXPoints + ((j - 1) * this.analyseFileHeaderObject.NumberOfXPoints); 
                        }

                        float intensity = this.data[index];
                        imageData[i][j] = intensity;
                        meanSum += intensity;
                        valuesForMedian.Add(intensity);
                        
                        if (intensity < this.minIntensityValue)
                        {
                            this.minIntensityValue = intensity;
                        }

                        if (intensity > this.maxIntensityValue)
                        {
                            this.maxIntensityValue = intensity;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }

            // 4) Calculate Mean And Median
            if (numDataPoint != 0)
            {
                this.meanValue = (float)(meanSum / numDataPoint);
            }

            valuesForMedian.Sort();
            this.medianValue = ((valuesForMedian.Count % 2) == 0) ? (valuesForMedian[(valuesForMedian.Count / 2) - 1] + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f : valuesForMedian[valuesForMedian.Count / 2];

            // 5) Fill out meta data
            var metaData = new ImageMetaData();

            try
            {
                const float Epsilon = (float)1E-10;
                metaData.Add("Sample Name", typeof(string), this.analyseFileHeaderObject.Name, false);
                metaData.Add("X1 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.X1 - (int)this.analyseFileHeaderObject.X1) < Epsilon) ? this.analyseFileHeaderObject.X1.ToString("0.0") : this.analyseFileHeaderObject.X1.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Y1 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.Y1 - (int)this.analyseFileHeaderObject.Y1) < Epsilon) ? this.analyseFileHeaderObject.Y1.ToString("0.0") : this.analyseFileHeaderObject.Y1.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("X2 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.X2 - (int)this.analyseFileHeaderObject.X2) < Epsilon) ? this.analyseFileHeaderObject.X2.ToString("0.0") : this.analyseFileHeaderObject.X2.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Y2 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.Y2 - (int)this.analyseFileHeaderObject.Y2) < Epsilon) ? this.analyseFileHeaderObject.Y2.ToString("0.0") : this.analyseFileHeaderObject.Y2.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Data Points in X", typeof(string), this.analyseFileHeaderObject.NumberOfXPoints.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Data Points in Y", typeof(string), this.analyseFileHeaderObject.NumberOfYPoints.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Point Width (mm)", typeof(string), Math.Round(this.analyseFileHeaderObject.Dx, 2).ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Point Height (mm)", typeof(string), Math.Round(this.analyseFileHeaderObject.Dy, 2).ToString(CultureInfo.InvariantCulture), false);
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }

            // 6) Fill out imageData
            analysefilecontent.Add(new ImageData(
                                                analysefilecontent.Document,
                                                imageData,
                                                this.analyseFileHeaderObject.Name,
                                                metaData,
                                                this.minIntensityValue,
                                                this.maxIntensityValue,
                                                this.meanValue,
                                                this.medianValue,
                                                this.analyseFileHeaderObject.Dx,
                                                this.analyseFileHeaderObject.Dy,
                                                null,
                                                this.analyseFileHeaderObject.ExperimentType));
        }

       /// <summary>
       /// Reads the contents of the "img" file and dumps it into data []
       /// </summary>
       private void ReadImgFile()
        {
            // 1) Use BinaryReader - Open the Img file
            using (var imgReader = new BinaryReader(File.Open(this.filePath, FileMode.Open)))
            {
                // 2) Position and length variables.
                int pos = 0;
                int arraypos = 0;

                // 3) Use BaseStream and create the data array for intensity values
                var length = (int)imgReader.BaseStream.Length;
                int numberofpoints = length / this.analyseFileHeaderObject.DataByteSize;

                this.data = new float[numberofpoints];

                try
                {
                    // 4) loop through stream
                    while (pos < length)
                    {
                        // Read intensity
                        // Must be a better way to do this?
                        if (this.analyseFileHeaderObject.DataType == 4)
                        {
                            short intensity = imgReader.ReadInt16();
                            this.data[arraypos] = intensity;
                        }
                        else if (this.analyseFileHeaderObject.DataType == 64)
                        {
                            long intensity = imgReader.ReadInt64();
                            this.data[arraypos] = intensity;
                        }
                        else
                        {
                            // 8, 16 and other
                            float intensity = imgReader.ReadSingle();
                            this.data[arraypos] = intensity;
                        }

                        // 5) Advance our position variable.
                        pos += this.analyseFileHeaderObject.DataByteSize;
                        arraypos++;
                    }
                }
                catch (Exception e)
                {
                    Util.ReportException(e);
                }

                imgReader.Close();
            }
        }

       #endregion Methods
    }
}
