#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="MRMScanExperiment.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 28/11/2012 - Implementation of new dot net Assemblies
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.WiffLoader
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Forms;
    using Clearcore2.Data;
    using Clearcore2.Data.DataAccess.SampleData;
    using Novartis.Msi.Core;

    /// <summary>
    /// This class encapsulates the specific experiment type
    /// 'MRM Scan'
    /// </summary>
    public class MrmScanExperiment : WiffExperiment
    {
        #region Fields

        /// <summary>
        /// Number of datapoints in Ion Chromatogram
        /// </summary>
        private int numDataPoint;

        /// <summary>
        /// Array for massranges
        /// </summary>
        private float[,] rawData;

        /// <summary>
        /// Array for time values
        /// </summary>
        private float[] timeData;

        /// <summary>
        /// Dwell Time
        /// </summary>
        private double[] dwellTime;

        /// <summary>
        /// Mass range Name
        /// </summary>
        private string[] massRangeName;

        /// <summary>
        /// max intensity value
        /// </summary>
        private double[] massRangeMax;

        /// <summary>
        /// min intensity value
        /// </summary>
        private double[] massRangeMin;

        /// <summary>
        /// Number items in massRange array
        /// </summary>
        private int massRanges;

        /// <summary>
        /// time offset 
        /// </summary>
        private double timeOffset;

        /// <summary>
        /// Number of points on X axis
        /// </summary>
        private int numPointsOnXAxis;

        /// <summary>
        /// Number of points on Y axis
        /// </summary>
        private int numPointsOnYAxis;

        /// <summary>
        /// speed value in x direction
        /// </summary>
        private double speedinXDirection;

        /// <summary>
        /// time value in x direction
        /// </summary>
        private double timeinXDirection;

        /// <summary>
        /// x Distance
        /// </summary>
        private double distInXDirection;

        /// <summary>
        /// y Distance
        /// </summary>
        private double distInYDirection;

        /// <summary>
        /// line break parameter
        /// </summary>
        private double lineBreak;

        /// <summary>
        /// Array for mean values
        /// </summary>
        private float[] meanValues;

        /// <summary>
        /// Array for median values
        /// </summary>
        private float[] medianValues;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MrmScanExperiment"/> class
        /// </summary>
        /// <param name="msexperiment">Mass Spec Experiment</param>
        /// <param name="wiffsample">wiff sample</param>
        public MrmScanExperiment(MSExperiment msexperiment, WiffSample wiffsample)
            : base(msexperiment, wiffsample)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="inputMsExperiment"/>.
        /// </summary>
        /// <param name="inputMsExperiment">instance, the data source</param>
        /// <param name="wiffsample"> Wiff Sample</param>
        protected override void Initialize(MSExperiment inputMsExperiment, WiffSample wiffsample)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
            // set the status information
            AppContext.StatusInfo = "Getting wiff-data";

            // (1) Get Maldi Parameters
            string strMaldiParams = wiffsample.MaldiParametersString;
                
            // (2) get the number of mass MRM transitions
            this.massRanges = inputMsExperiment.Details.MassRangeInfo.Length; 
            
            // (3) select 1. MRM trace
            XYData tic = inputMsExperiment.GetTotalIonChromatogram();

            // (4) Dimension the data arrray, the number "data points"
            this.numDataPoint = tic.NumDataPoints;

            // (5) Set up arrays for info
            this.rawData = new float[this.massRanges, this.numDataPoint];
            this.timeData = new float[this.numDataPoint];
            this.dwellTime = new double[this.massRanges];
            this.massRangeName = new string[this.massRanges];
            this.massRangeMax = new double[this.massRanges];
            this.massRangeMin = new double[this.massRanges];
            this.meanValues = new float[this.massRanges];
            this.medianValues = new float[this.massRanges];
            
            // helper for mean and median
            var valuesForMedian = new List<float>(); 

            // (6) loop through the mass ranges
            for (int actMassRange = 0; actMassRange < this.massRanges; actMassRange++)
            {
                // get mass range object
                this.massRangeName[actMassRange] = inputMsExperiment.Details.MassRangeInfo[actMassRange].Name;

                this.dwellTime[actMassRange] = inputMsExperiment.Details.MassRangeInfo[actMassRange].DwellTime;

                // loop through the spectrum
                this.massRangeMax[actMassRange] = double.MinValue;
                this.massRangeMin[actMassRange] = double.MaxValue;
                double meanSum = 0.0;
                valuesForMedian.Clear();

                // Extract xic Full Scan
                var option = new ExtractedIonChromatogramSettings(actMassRange);
                ExtractedIonChromatogram xic = inputMsExperiment.GetExtractedIonChromatogram(option);

                // JP 23/11 I think there might be a better way to do this.
                for (int actDataPoint = 0; actDataPoint < this.numDataPoint; actDataPoint++)
                {
                    // get the actual value
                    double anaXValue = xic.GetActualXValues()[actDataPoint];
                    double anaYValue = xic.GetActualYValues()[actDataPoint];

                    if (anaYValue < this.massRangeMin[actMassRange])
                    {
                        this.massRangeMin[actMassRange] = anaYValue;
                    }

                    if (anaYValue > this.massRangeMax[actMassRange])
                    {
                        this.massRangeMax[actMassRange] = anaYValue;
                    }

                    // now get the actual data
                    this.rawData[actMassRange, actDataPoint] = (float)anaYValue;
                    this.timeData[actDataPoint] = (float)anaXValue * 60;

                    // mean and median calculation...sum up the value to calculate the mean
                    meanSum += anaYValue;
                    
                    // fill an extra array to calculate the median
                    valuesForMedian.Add((float)anaYValue);
                }
               
                // calculate the mean
                this.meanValues[actMassRange] = (float)(meanSum / this.numDataPoint);

                // calculate the median
                valuesForMedian.Sort();
                this.medianValues[actMassRange] = ((valuesForMedian.Count % 2) == 0) ? (valuesForMedian[(valuesForMedian.Count / 2) - 1] + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f : valuesForMedian[valuesForMedian.Count / 2];
            }

            // (7) time per point in sec 
            this.timeinXDirection = (float)((tic.GetActualXValues()[this.numDataPoint - 1] - tic.GetActualXValues()[1]) * 60 / (this.numDataPoint - 2));
            
            // (8) Calculate distInXDirection: fetch the x1, x2, y1, y2, width and height from the WiffSample instance this experiment belongs to.
            double x1 = wiffsample.P1.X;
            double y2 = 82 - wiffsample.P1.Y;
            double x2 = wiffsample.P2.X;
            double width = wiffsample.Width;

            // (9) fetch the position data from the WiffSample instance this experiment belongs to.
            uint[,] posData = wiffsample.PositionData;
            long posDataLength = wiffsample.PositionDataLength;

            // try and find the first and last valid indices in the posData, where valid means a nonzero time value...
            long firstNonZeroTimeInPos = -1;
            for (long t = 0; t < posDataLength - 1; t++)
            {
                if (posData[t, 0] > 0)
                {
                    firstNonZeroTimeInPos = t + 1;
                    break; // ok, we're done...
                }
            }

            long lastNonZeroTimeInPos = -1;
            for (long t = posDataLength - 1; t >= 0; t++)
            {
                if (posData[t, 0] > 0)
                {
                    lastNonZeroTimeInPos = t - 1;
                    break; // ok, we're done...
                }
            }

            // (10) Make sure we have found valid values, bail out if not
            if (firstNonZeroTimeInPos < 0 || lastNonZeroTimeInPos < 0)
            {
                // haven't found a valid posData triplet. All time values are zero or less...bail out
                return;
            }

            // (11) Calculate distInYDirection
            this.distInYDirection = 0;
            for (long t = firstNonZeroTimeInPos; t < lastNonZeroTimeInPos; t++)
            {
                if ((posData[t, 1] > ((x2 + x1) * 500)) & Equals(this.distInYDirection, 0.0)) 
                {
                    this.distInYDirection = posData[t, 2];
                }

                if ((posData[t, 1] < ((x2 + x1) * 500)) & (this.distInYDirection > 0))
                {
                    this.distInYDirection = (float)Math.Round((decimal)(posData[t, 2] - this.distInYDirection) / 500, (int)(1 - Math.Log10((posData[t, 2] - this.distInYDirection) / 500))) / 2;
                    break;
                }
            }

            // (12) Calculate speed in x direction
            this.speedinXDirection = (float)Math.Round((posData[firstNonZeroTimeInPos + 2, 1] - posData[firstNonZeroTimeInPos + 1, 1]) / (decimal)(posData[firstNonZeroTimeInPos + 2, 0] - posData[firstNonZeroTimeInPos + 1, 0]) * 2, 0) / 2;

            // (13) distInXDirection
            this.distInXDirection = (float)(int)(this.speedinXDirection * this.timeinXDirection * 1000) / 1000;

            // (14) number of points in x
            this.numPointsOnXAxis = (int)(width / this.speedinXDirection / this.timeinXDirection);

            // (15) number of points in y
            // y2 from the wiff file is not the actual y2 from the stage - replace with value from path file...
            // JP added "82 -" so what we convert wiff format to analyse co-ordinate system
            var y1 = 82 - (double)Math.Round((decimal)posData[lastNonZeroTimeInPos, 2] / 1000, 2);
           
            // ...and this has an effect of the numPointsOnYAxis
            this.numPointsOnYAxis = (int)Math.Round((decimal)((y2 - y1) / this.distInYDirection) + 1); 

            // (16) Calc Lines Breaks
            double syncTime1 = (float)posData[firstNonZeroTimeInPos, 0] / 1000;
            double syncTime2 = (float)posData[lastNonZeroTimeInPos, 0] / 1000;

            if (this.numPointsOnYAxis % 2 == 0)
            {
                // even number of scanlines
                this.lineBreak = ((syncTime2 - syncTime1)
                - ((x2 - ((float)posData[firstNonZeroTimeInPos, 1] / 1000)) / this.speedinXDirection) 
                - (((this.numPointsOnYAxis - 2) * (x2 - x1)) / this.speedinXDirection)
                - ((x2 - ((float)posData[lastNonZeroTimeInPos, 1] / 1000)) / this.speedinXDirection)) 
                / (this.numPointsOnYAxis - 1);
            }
            else
            {
                // odd number of scanlines
                 this.lineBreak = ((syncTime2 - syncTime1)
                - ((x2 - ((float)posData[firstNonZeroTimeInPos, 1] / 1000)) / this.speedinXDirection)
                - (((this.numPointsOnYAxis - 2) * (x2 - x1)) / this.speedinXDirection)
                - ((((float)posData[lastNonZeroTimeInPos, 1] / 1000) - x1) / this.speedinXDirection))
                / (this.numPointsOnYAxis - 1);
            }
            
            // (17) time offset
            this.timeOffset = ((float)posData[firstNonZeroTimeInPos, 0] / 1000) - ((((float)posData[firstNonZeroTimeInPos, 1] / 1000) - x1) / this.speedinXDirection); 

            double dwellTimeSum = 0;

            AppContext.StatusInfo = string.Empty;

            // (18) Format Data 
            for (int actMassRange = 0; actMassRange < this.massRanges; actMassRange++)
            {
                // format the data into a rectangular, 2 dimensional array of floats that will represent the image data
                var imageData = new float[this.numPointsOnXAxis][];
                for (int i = 0; i < imageData.Length; i++)
                {
                    imageData[i] = new float[this.numPointsOnYAxis];
                }

                dwellTimeSum -= this.dwellTime[actMassRange] / 1000 / 2;

                double optTimeOffset = this.timeOffset;
                double optLineBreak = this.lineBreak;
                if (AppContext.UseApproximation)
                {
                    // Perform the optimization...
                    this.HeuristicOptimization(imageData, actMassRange, x2 - x1, dwellTimeSum, ref optLineBreak, ref optTimeOffset);
                }

                this.FormatImageSharp(imageData, actMassRange, x2 - x1, dwellTimeSum, optLineBreak, optTimeOffset);

                dwellTimeSum -= this.dwellTime[actMassRange] / 1000 / 2;
                
                // create the appropriate dataset and add it to the WiffFileContent object...
                string imgName = wiffsample.Name + " : " + this.massRangeName[actMassRange];
                Document doc = wiffsample.WiffFileContent.Document;

                // create the meta information data structure and populate with relevant information...
                var metaData = new ImageMetaData();
                try
                {
                    const float Epsilon = (float)1E-10;
                    metaData.Add("Sample Name", typeof(string), wiffsample.Name, false);
                    metaData.Add("Mass Range", typeof(string), this.massRangeName[actMassRange], false);
                    metaData.Add("X1 (mm)", typeof(string), (Math.Abs(x1 - (int)x1) < Epsilon) ? x1.ToString("0.0") : x1.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Y1 (mm)", typeof(string), (Math.Abs(y1 - (int)y1) < Epsilon) ? y1.ToString("0.0") : y1.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("X2 (mm)", typeof(string), (Math.Abs(x2 - (int)x2) < Epsilon) ? x2.ToString("0.0") : x2.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Y2 (mm)", typeof(string), (Math.Abs(y2 - (int)y2) < Epsilon) ? y2.ToString("0.0") : y2.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Data Points in X", typeof(string), this.numPointsOnXAxis.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Data Points in Y", typeof(string), this.numPointsOnYAxis.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Point Width (mm)", typeof(string), Math.Round(this.distInXDirection, 2).ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Point Height (mm)", typeof(string), Math.Round(this.distInYDirection, 2).ToString(CultureInfo.InvariantCulture), false);
                    
                    // Add Maldi Parameters
                    string[] splitstring = strMaldiParams.Split(SepMaldiParams, StringSplitOptions.None);
                    metaData.Add("Laser Frequency (Hz)", typeof(string), splitstring[1], false);
                    metaData.Add("Laser Power (%)", typeof(string), splitstring[2], false);
                    metaData.Add("Ablation Mode", typeof(string), splitstring[3], false);
                    metaData.Add("Skimmer Voltage (V)", typeof(string), splitstring[4], false);
                    metaData.Add("Source Gas", typeof(string), splitstring[5], false);
                    metaData.Add("Raster Speed (mm/s)", typeof(string), splitstring[6], false);
                    metaData.Add("Line Direction", typeof(string), splitstring[7], false);
                    metaData.Add("Rastor Pitch ", typeof(string), splitstring[8], false);
                }
                catch (Exception e) 
                { 
                    Util.ReportException(e); 
                }

                // (19) Create new ImageData and add WiffContent to Wiffsample  
                wiffsample.WiffFileContent.Add(new ImageData(
                                                             doc,
                                                             imageData,
                                                             imgName,
                                                             metaData,
                                                            (float)this.massRangeMin[actMassRange],
                                                            (float)this.massRangeMax[actMassRange],
                                                            this.meanValues[actMassRange],
                                                            this.medianValues[actMassRange],
                                                            (float)this.distInXDirection,
                                                            (float)this.distInYDirection,
                                                            null,
                                                            Core.ExperimentType.MRM)); 
            }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
        /// the line break timings.
        /// </summary>
        /// <param name="imageData">The allocate 2-dimensional array to receive the formated image.</param>
        /// <param name="curMassRange">The index of the massrange for which generate an image.</param>
        /// <param name="width">The width of the sample.</param>
        /// <param name="dwellTimeSum">The dwelltime of the datastream.</param>
        /// <param name="inputLineBreak">The calculated time a linebreak consumes.</param>
        /// <param name="inputTimeOffset">The calculated time offset to the first valid intensity value.</param>
        private void FormatImageFuzzy(float[][] imageData, int curMassRange, double width, double dwellTimeSum, double inputLineBreak, double inputTimeOffset)
        {
            AppContext.ProgressStart("formatting image data...");
            try
            {
                // copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
                // the line break timings.
                int diffPixelCount = 0;
                bool analyzeImage = AppContext.UseApproximation;

                for (int y = 0; y < this.numPointsOnYAxis; y++)
                {
                    double currentTime = inputTimeOffset + ((this.distInXDirection / this.speedinXDirection) / 2) + (((width / this.speedinXDirection) + inputLineBreak) * y) + 0;
                   
                    // currentLine is the offset to the start of the current line in the linear datastream (rawData)
                    // instead of the closest data value, calc an average of overlapping points
                    var currentLine = (int)Math.Round((currentTime + dwellTimeSum - this.timeData[0]) / this.timeinXDirection);
                    double currentLineWeight = ((currentTime + dwellTimeSum - this.timeData[0]) / this.timeinXDirection) - currentLine;
                    var currentLineUpDown = Math.Sign(currentLineWeight);
                    currentLineWeight = Math.Abs(currentLineWeight);

                    for (int x = 0; x < this.numPointsOnXAxis; x++)
                    {
                        int currentPoint;
                        if (y % 2 == 0)
                        {
                            // even y: scan direction: -->
                            currentPoint = currentLine + x;
                        }
                        else
                        {
                            // odd y:  scan direction: <--
                            currentPoint = currentLine + this.numPointsOnXAxis - 1 - x;
                        }

                        if ((currentPoint < this.numDataPoint - 2) & (currentPoint >= 1))
                        {
                            imageData[x][this.numPointsOnYAxis - 1 - y] = (float)((this.rawData[curMassRange, currentPoint] * (1 - currentLineWeight)) + (this.rawData[curMassRange, currentPoint + currentLineUpDown] * currentLineWeight));
                        }
                        else
                        {
                            imageData[x][this.numPointsOnYAxis - 1 - y] = 0;
                        }

                        // perform image analyzation if heuristic optimization is in progress...
                        if (analyzeImage)
                        {
                            if (y > 0)
                            {
                                if ((imageData[x][y] + imageData[x][y - 1]) > 100)
                                {
                                    diffPixelCount++;
                                }
                            }
                        }

                        AppContext.ProgressSetValue(100.0 * y / this.numPointsOnYAxis);
                    }
                }

                if (diffPixelCount > 0)
                {
                }
            }
            finally
            {
                AppContext.ProgressClear();
            }
        }

        /// <summary>
        /// Copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
        /// the line break timings.
        /// </summary>
        /// <param name="imageData">The allocate 2-dimensional array to receive the formated image.</param>
        /// <param name="curMassRange">The index of the massrange for which generate an image.</param>
        /// <param name="width">The width of the sample.</param>
        /// <param name="dwellTimeSum">The dwelltime of the datastream.</param>
        /// <param name="inputLineBreak">The calculated time a linebreak consumes.</param>
        /// <param name="inputTimeOffset">The calculated time offset to the first valid intensity value.</param>
        /// <returns>A double value indicating the quality of the image. This is only in effect in the process of heuristic optimization.</returns>
        private double FormatImageSharp(float[][] imageData, int curMassRange, double width, double dwellTimeSum, double inputLineBreak, double inputTimeOffset)
        {
            // copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
            // the line break timings.
            int diffPixelCount = 0;
            double imageDiff = 0.0;
            bool analyzeImage = AppContext.UseApproximation;

            for (int y = 0; y < this.numPointsOnYAxis; y++)
            {
                double currentTime = inputTimeOffset + ((this.distInXDirection / this.speedinXDirection) / 2) + (((width / this.speedinXDirection) + inputLineBreak) * y) + 0;

                var currentLine = (int)Math.Round((currentTime + dwellTimeSum - this.timeData[0]) / this.timeinXDirection);

                for (int x = 0; x < this.numPointsOnXAxis; x++)
                {
                    int currentPoint;
                    if (y % 2 == 0)
                    {
                        // even y: scan direction: -->
                        currentPoint = currentLine + x;
                    }
                    else
                    {
                        // odd y:  scan direction: <--
                        currentPoint = currentLine + this.numPointsOnXAxis - 1 - x;
                    }

                    if ((currentPoint < this.numDataPoint - 1) & (currentPoint >= 0))
                    {
                        imageData[x][this.numPointsOnYAxis - 1 - y] = this.rawData[curMassRange, currentPoint];
                    }
                    else
                    {
                        imageData[x][this.numPointsOnYAxis - 1 - y] = 0;
                    }

                    // perform image analyzation if heuristic optimization is in progress...
                    if (analyzeImage)
                    {
                        if (y > 0)
                        {
                            if ((imageData[x][this.numPointsOnYAxis - 1 - y] + imageData[x][this.numPointsOnYAxis - 1 - y + 1]) > 100)
                            {
                                imageDiff += Math.Abs(imageData[x][this.numPointsOnYAxis - 1 - y] - imageData[x][this.numPointsOnYAxis - 1 - y + 1]) / (imageData[x][this.numPointsOnYAxis - 1 - y] + imageData[x][this.numPointsOnYAxis - 1 - y + 1]);
                                diffPixelCount++;
                            }
                        }
                    }
                }
            }

            if (diffPixelCount > 0)
            {
                imageDiff /= diffPixelCount;
            }

            return imageDiff;
        }

        /// <summary>
        /// Try to optimize the values for the timeoffset and the the linebreak by varying the values and analyzing the calculated images.
        /// </summary>
        /// <param name="imageData">The allocated 2-dimensional array to receive the formated image.</param>
        /// <param name="curMassRange">The index of the massrange for which generate an image.</param>
        /// <param name="width">The width of the sample.</param>
        /// <param name="dwellTimeSum">The dwelltime of the datastream.</param>
        /// <param name="inputLineBreak">The calculated time a linebreak consumes. This value will be optimized.</param>
        /// <param name="inputoffsetTime">input offsetTime</param>
        private void HeuristicOptimization(float[][] imageData, int curMassRange, double width, double dwellTimeSum, ref double inputLineBreak, ref double inputoffsetTime)
        {
            double minLineBreak = inputLineBreak;
            double minoffsetTime = inputoffsetTime;
                
            AppContext.ProgressStart("optimizing wiff-data...");
            try
            {
                const int Maxk = 4;
                const int Maxi = 21;
                for (int k = 0; k < Maxk; k++)
                {
                    double minDiff = double.MaxValue;
                    inputLineBreak -= Math.Pow(10, -k);
                    for (int i = 0; i < Maxi; i++)
                    {
                        inputLineBreak += Math.Pow(10, -k - 1);
                        for (double j = -2; j <= 2; j += 0.1)
                        {
                            double offsetTime = inputoffsetTime + j;
                            double imageDiff = this.FormatImageSharp(
                                imageData, curMassRange, width, dwellTimeSum, inputLineBreak, offsetTime);
                            if (imageDiff < minDiff)
                            {
                                minDiff = imageDiff;
                                minLineBreak = inputLineBreak;
                                minoffsetTime = offsetTime;
                            }
                        }

                        AppContext.ProgressSetValue(100.0 * (((k * Maxi) + i) / (double)(Maxk * Maxi)));
                    }

                    inputLineBreak = minLineBreak;
                }

                if (Math.Abs(inputLineBreak - minLineBreak) < 2)
                {
                    inputLineBreak = minLineBreak;
                    inputoffsetTime = minoffsetTime;
                }
            }
            finally
            {
                AppContext.ProgressClear();
            }
        }

        #endregion
    }
}
