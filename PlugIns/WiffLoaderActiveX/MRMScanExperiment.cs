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

using System;
using System.Collections.Generic;
using System.Drawing;

using NETExploreDataObjects;
using NETMSMethodSvr;

using Novartis.Msi.Core;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class encapsulates the specific experiment type
    /// 'MRM Scan'.
    /// </summary>
    public class MRMScanExperiment : WiffExperiment
    {
        #region Fields

        private int nrDataPoints;
        private float[,] rawData;
        private float[] timeData;
        private double[] dwellTime;
        private string[] massRangeName;
        private double[] massRangeMax;
        private double[] massRangeMin;
        private int massRanges;
        private double timeOffset;
        private int xPoints;
        private int yPoints;
        private double xSpeed;
        private double xTime;
        private double xDist;
        private double yDist;
        // private double lineOffset;
        private double lineBreak;
        private float[] meanValues;
        private float[] medianValues;

        #endregion

        /// <summary>
        /// Constructor. Creates and initializes a Q1ScanExperiment instance.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The source from where to
        /// read the periods properties and data.</param>
        /// <param name="wiffPeriod">The <see cref="WiffPeriod"/> instance this experiment belongs to.</param>
        /// <param name="experimentIndex">The <b>zero-based</b> index of this experiment in the associated period.</param>
        public MRMScanExperiment(FMANWiffFileClass wiffFile, WiffPeriod wiffPeriod, int experimentIndex)
            : base(wiffFile, wiffPeriod, experimentIndex)
        {
        }

        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="wiffFile"/>.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The data source.</param>
        protected override void Initialize(FMANWiffFileClass wiffFile)
        {
            // set the status information
            AppContext.StatusInfo = "Getting wiff-data";

            // get the experiment parameters and the experiment object
            ITripleQuadMALDI experimentParams = (ITripleQuadMALDI)wiffFile.GetExperimentObject(WiffPeriod.Sample.Index, WiffPeriod.Index, Index);
            Experiment experiment = (Experiment)wiffFile.GetExperimentObject(WiffPeriod.Sample.Index, WiffPeriod.Index, Index);

            // get the number of mass MRM transitions
            massRanges = experiment.MassRangesCount;

            // select 1. MRM trace
            FMANChromData anaChrom = new FMANChromData();
            anaChrom.WiffFileName = wiffFile.GetWiffFileName();
            anaChrom.SetToXICZeroWidth(WiffPeriod.Sample.Index, WiffPeriod.Index, Index, 0);

            // dimension the data arrray, the number "data points"
            nrDataPoints = anaChrom.GetNumberOfDataPoints();

            rawData = new float[massRanges, nrDataPoints];
            timeData = new float[nrDataPoints];
            dwellTime = new double[massRanges];
            massRangeName = new string[massRanges];
            massRangeMax = new double[massRanges];
            massRangeMin = new double[massRanges];
            meanValues = new float[massRanges];
            medianValues = new float[massRanges];

            // helper for mean and median
            double meanSum = 0;
            List<float> valuesForMedian = new List<float>(); 


            // loop through the mass ranges
            double[] startMass = new double[massRanges];
            double[] stopMass = new double[massRanges];
            double[] stepMass = new double[massRanges];
            for (int actMassRange = 0; actMassRange < massRanges; actMassRange++)
            {
                // get mass range object
                MassRange anaMassRange = (MassRange)experiment.GetMassRange(actMassRange);
                startMass[actMassRange] = anaMassRange.QstartMass;
                stopMass[actMassRange]  = anaMassRange.QstopMass;
                stepMass[actMassRange]  = anaMassRange.QstepMass;

                massRangeName[actMassRange] = Math.Round(startMass[actMassRange], 2).ToString() + " > " + Math.Round(stepMass[actMassRange], 2).ToString();

                dwellTime[actMassRange] = anaMassRange.DwellTime;

                // select MRM trace
                anaChrom.SetToXICZeroWidth(WiffPeriod.Sample.Index, WiffPeriod.Index, Index, (short)actMassRange);

                // loop throug the spectrum
                massRangeMax[actMassRange] = double.MinValue;
                massRangeMin[actMassRange] = double.MaxValue;
                meanSum = 0.0;
                valuesForMedian.Clear();
                for (int actDataPoint = 1; actDataPoint <= nrDataPoints; actDataPoint++)
                {
                    // get the actual value
                    double anaXValue = anaChrom.GetDataPointXValue(actDataPoint);
                    double anaYValue = anaChrom.GetDataPointYValue(actDataPoint);
                    if (anaYValue < massRangeMin[actMassRange])
                        massRangeMin[actMassRange] = anaYValue;
                    if (anaYValue > massRangeMax[actMassRange])
                        massRangeMax[actMassRange] = anaYValue;

                    // now get the actual data
                    rawData[actMassRange, actDataPoint - 1] = (float)anaYValue;
                    timeData[actDataPoint - 1] = (float)anaXValue * 60;

                    // mean and median calculation...
                    // sum up the value to calculate the mean
                    meanSum += anaYValue;
                    // fill an extra array to calculate the median
                    valuesForMedian.Add((float)anaYValue);

                }

                // calculate the mean
                meanValues[actMassRange] = (float)(meanSum / nrDataPoints);
                // calculate the median
                valuesForMedian.Sort();
                medianValues[actMassRange] = ((valuesForMedian.Count % 2) == 0) ? (float)(valuesForMedian[(valuesForMedian.Count / 2) - 1] + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f : valuesForMedian[valuesForMedian.Count / 2];
            }

            // get MALDI parmas and assign variables
            string strMALDIParams = experimentParams.TripleQuadMALDIParameters;

            // x speed in mm/s
            xSpeed = float.Parse(strMALDIParams.Split(sepMALDIParams, System.StringSplitOptions.None)[6]);

            //line distance in mm
            yDist = float.Parse(strMALDIParams.Split(sepMALDIParams, System.StringSplitOptions.None)[8]) / 1000;

            //time per point in s
            xTime = (float)((anaChrom.GetDataPointXValue(nrDataPoints) - anaChrom.GetDataPointXValue(2)) * 60 / (nrDataPoints - 2));
            xDist = (float)(int)(xSpeed * xTime * 1000) / 1000;

            anaChrom.SetToTIC(WiffPeriod.Sample.Index, WiffPeriod.Index, Index);

            // fetch the x1, x2, y1, y2, width and height from the WiffSample instance this experiment belongs to.
            double x1 = WiffPeriod.Sample.P1.X;
            double y1 = WiffPeriod.Sample.P1.Y;
            double x2 = WiffPeriod.Sample.P2.X;
            double y2 = WiffPeriod.Sample.P2.Y;
            double width = WiffPeriod.Sample.Width;
            double height = WiffPeriod.Sample.Height;

            //number of poins in x
            xPoints = (int)((width) / xSpeed / xTime);

            //number of points in y
            yPoints = (int)((height) / yDist + 0.5) + 1;

            // lineOffset = 1;

            // fetch the position data from the WiffSample instance this experiment belongs to.
            uint[,] posData = WiffPeriod.Sample.PositionData;
            long posDataLength = WiffPeriod.Sample.PositionDataLength;

            // try and find the first and last valid indices in the posData, where valid means a nonzero time value...
            long firstNonZeroTimeInPos = -1;
            for (long t = 0; t < posDataLength - 1; t++)
            {
                if (posData[t, 0] > 0)
                {
                    firstNonZeroTimeInPos = t+1;
                    // ok, we're done...
                    break;
                }
            }

            long lastNonZeroTimeInPos = -1;
            for (long t = posDataLength - 1; t >= 0; t++)
            {
                if (posData[t, 0] > 0)
                {
                    lastNonZeroTimeInPos = t-1;
                    // ok, we're done...
                    break;
                }
            }

            if (firstNonZeroTimeInPos < 0 || lastNonZeroTimeInPos < 0)
            {
                // haven't found a valid posData triplet. All time values are zero or less...
                // bail out
                return;
            }

            // y2 from the wiff file is not the actual y2 from the stage - replace with value from path file...
            y2 = (double)Math.Round( (decimal)posData[lastNonZeroTimeInPos, 2]/1000,2);
            
            // ...and this has an effect of the ypoints
            yPoints = (int)Math.Round((decimal)((y2 - y1) / yDist))+1;
            
#if(false)  // experimental method to format the ms-data to the x,y suitable for an image representation 
            // fet two time stamps from the start and end of chrom file
            double tFirstMSDataPoint = anaChrom.GetDataPointXValue(10)*60*1000;
            double tLastMSDataPoint = anaChrom.GetDataPointXValue(nrDataPoints-10)*60*1000;
            int positionMarker = 1;
            
            // get the corresponding position from the path file
            for (int i=0; posData[i, 0] < tFirstMSDataPoint; i++)
            {
                positionMarker = i;
            }

            double xFirstMSDataPoint =                   
                 ((tFirstMSDataPoint - posData[positionMarker, 0])/(posData[positionMarker + 1 , 0]- posData[positionMarker, 0]) 
                 * (int)(posData[positionMarker + 1 , 1]- posData[positionMarker, 1])
                 + posData[positionMarker, 1])/1000;
            double yFirstMSDataPoint = (double)Math.Round((decimal)posData[positionMarker, 2]/1000,2);

            for (int i = 0; posData[i, 0] < tLastMSDataPoint; i++)
            {
                positionMarker = i;
            }
            double xLastMSDataPoint =
                 ((tLastMSDataPoint - posData[positionMarker, 0]) / (posData[positionMarker + 1, 0] - posData[positionMarker, 0])
                 * (int)(posData[positionMarker + 1, 1] - posData[positionMarker, 1])
                 + posData[positionMarker, 1]) / 1000;
            double yLastMSDataPoint = (double)Math.Round((decimal)posData[positionMarker, 2] / 1000, 2);

            lineOffset = 10 - (xFirstMSDataPoint - x1) / xDist;

            if (yPoints % 2 == 0)
            {
                // even number of scanlines
                lineBreak = (((nrDataPoints - 10 - 10)
                - (x2 - (float)xFirstMSDataPoint)/xDist
                - (yPoints - 2) * (x2 - x1) /xDist
                - (x2 - (float)xLastMSDataPoint)/xDist))
                / (yPoints - 1);
            }
            else
            {
                // odd number of scanlines
                lineBreak = (((nrDataPoints - 10 - 10)
                 - (x2 - (float)xFirstMSDataPoint)/xDist
                 - (yPoints - 2) * (x2 - x1) / xDist
                 - ((float)xLastMSDataPoint-x1) / xDist))
                 / (yPoints - 1);
            }


            //timeOffset = (float)posData[5, 0] / 1000 - (((float)posData[5, 1] / 1000 - x1) / xSpeed);


            for (int actMassRange = 0; actMassRange < massRanges; actMassRange++)
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                // format the data into a rectangular, 2 dimensional array of floats that will represent the image data
                //

                // prepare the data structure
                float[][] imageData = new float[xPoints][];
                for (int i = 0; i < imageData.Length; i++)
                {
                    imageData[i] = new float[yPoints];
                }

                // copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
                // the line break timings.
                for (int y = 0; y < yPoints; y++)

                {
                    int currentPoint;
                    // currentLine is the offset to the start of the current line in the linear datastream (rawData)
                    int currentLine = (int)Math.Floor(lineOffset + ((x2 - x1) / xDist + lineBreak) * y);
                    for (int x = 0; x < xPoints; x++)
                    {
                        if (y % 2 == 0)
                        {
                            // even y: scan direction: -->
                            currentPoint = (int)(currentLine + x);
                        }
                        else
                        {
                            // odd y:  scan direction: <--
                            currentPoint = (int)(currentLine + xPoints - 1 - x);
                        }
                        if (currentPoint < nrDataPoints)
                        {
                            imageData[x][y] = rawData[actMassRange, currentPoint];
                        }
                        else
                        {
                            imageData[x][y] = 0;
                        }
                    }
                   
                }
             lineOffset += ((double)dwellTime[actMassRange] + 5) /1000 * xSpeed / xDist;
#endif

            double timeSpanPos = posData[lastNonZeroTimeInPos, 0] / 1000.0 - posData[firstNonZeroTimeInPos, 0] / 1000.0;


            double syncTime1 = (float)posData[firstNonZeroTimeInPos, 0] / 1000;
            double syncTime2 = (float)posData[lastNonZeroTimeInPos, 0] / 1000;
            if (yPoints % 2 == 0)
            {
                // even number of scanlines
                lineBreak = ((syncTime2 - syncTime1)
                - (x2 - (float)posData[firstNonZeroTimeInPos, 1] / 1000) / xSpeed
                - (yPoints - 2) * (x2 - x1) / xSpeed
                - (x2 - (float)posData[lastNonZeroTimeInPos, 1] / 1000) / xSpeed)
                / (yPoints - 1);
                //timeOffset = (float)posData[posDataLength - 1, 0] / 1000 + (((float)posData[posDataLength - 1, 1] / 1000 - x1) / xSpeed) - (float)timeData[nrDataPoints - 1] + (float)timeData[0] - 2*xTime; //testing new method
            }
            else
            {
                // odd number of scanlines
                lineBreak = ((syncTime2 - syncTime1)
                - (x2 - (float)posData[firstNonZeroTimeInPos, 1] / 1000) / xSpeed
                - (yPoints - 2) * (x2 - x1) / xSpeed
                - ((float)posData[lastNonZeroTimeInPos, 1] / 1000 - x1) / xSpeed)
                / (yPoints - 1);
                //timeOffset = (float)posData[posDataLength - 1, 0] / 1000 + ((x2 - (float)posData[posDataLength - 1, 1] / 1000) / xSpeed) - (float)timeData[nrDataPoints - 1] + (float)timeData[0] - xTime;//testing new method
            }
            // MSt lineBreak = lineBreak / xTime;
            

            timeOffset = (float)posData[firstNonZeroTimeInPos, 0] / 1000 - (((float)posData[firstNonZeroTimeInPos, 1] / 1000 - x1) / xSpeed); // original method of calculation
            //MSt lineOffset = (timeData[0]-timeOffset) * xSpeed / xDist + .5;

            double dwellTimeSum = 0;

            // reset the status information
            AppContext.StatusInfo = "";

            for (int actMassRange = 0; actMassRange < massRanges; actMassRange++)
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                // format the data into a rectangular, 2 dimensional array of floats that will represent the image data
                //

                // prepare the data structure
                float[][] imageData = new float[xPoints][];
                for (int i = 0; i < imageData.Length; i++)
                {
                    imageData[i] = new float[yPoints];
                }

                dwellTimeSum -= (dwellTime[actMassRange]/1000 / 2);

                
                double optTimeOffset = timeOffset;
                double optLineBreak = lineBreak;
                if (AppContext.UseApproximation)
                {
                    // Perform the optimization...
                    HeuristicOptimization(imageData, actMassRange, (x2 - x1), dwellTimeSum, ref optLineBreak, ref optTimeOffset);
                }

                FormatImageFuzzy(imageData, actMassRange, (x2 - x1), dwellTimeSum, optLineBreak, optTimeOffset);

                dwellTimeSum -= (dwellTime[actMassRange]/1000 / 2);
                

                // create the appropriate dataset and add it to the WiffFileContent object...
                string imgName = WiffPeriod.Sample.Name + " : " + massRangeName[actMassRange];
                Document doc = WiffPeriod.Sample.WiffFileContent.Document;
                //create the meta information data structure and populate with relevant information...
                ImageMetaData metaData = new ImageMetaData();
                try
                {
                    metaData.Add("Sample Name", typeof(string),  WiffPeriod.Sample.Name, false);
                    metaData.Add("Mass Range", typeof(string), massRangeName[actMassRange], false);
                    metaData.Add("X1 (mm)", typeof(string), x1.ToString(), false);
                    metaData.Add("X2 (mm)", typeof(string), x2.ToString(), false);
                    metaData.Add("Y1 (mm)", typeof(string), y1.ToString(), false);
                    metaData.Add("Y2 (mm)", typeof(string), y2.ToString(), false);
                    metaData.Add("Data Points in X", typeof(string), xPoints.ToString(), false);
                    metaData.Add("Data Points in Y", typeof(string), yPoints.ToString(), false);
                    metaData.Add("Point Width (mm)", typeof(string), xDist.ToString(), false);
                    metaData.Add("Point Height (mm)", typeof(string), yDist.ToString(), false);
                    string[] splitted = strMALDIParams.Split(sepMALDIParams, System.StringSplitOptions.None);
                    metaData.Add("Laser Frequency (Hz)", typeof(string), splitted[1], false);
                    metaData.Add("Laser Power (%)", typeof(string), splitted[2], false);
                    metaData.Add("Ablation Mode", typeof(string), splitted[3], false);
                    metaData.Add("Skimmer Voltage (V)", typeof(string), splitted[4], false);
                    metaData.Add("Source Gas", typeof(string), splitted[5], false);
                    metaData.Add("Raster Speed (mm/s)", typeof(string), splitted[6], false);
                    metaData.Add("Line Distance (µm)", typeof(string), splitted[8], false);
                }
                catch (Exception e) { Util.ReportException(e); }

                WiffPeriod.Sample.WiffFileContent.Add(new ImageData(doc, imageData, imgName, metaData, (float)massRangeMin[actMassRange], (float)massRangeMax[actMassRange],
                    (float)meanValues[actMassRange], (float)medianValues[actMassRange], (float)xDist, (float)yDist, (float)startMass[actMassRange], (float)stepMass[actMassRange]));
            }

            anaChrom.WiffFileName = "";
            anaChrom = null;
        }

        /// <summary>
        /// Copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
        /// the line break timings.
        /// </summary>
        /// <param name="imageData">The allocate 2-dimensional array to receive the formated image.</param>
        /// <param name="curMassRange">The index of the massrange for which generate an image.</param>
        /// <param name="width">The width of the sample.</param>
        /// <param name="dwellTimeSum">The dwelltime of the datastream.</param>
        /// <param name="lineBreak">The calculated time a linebreak consumes.</param>
        /// <param name="timeOffset">The calculated time offset to the first valid intensity value.</param>
        /// <returns>A double value indicating the quality of the image. This is only in effect in the process of heuristic optimization.</returns>
        private double FormatImageFuzzy(float[][] imageData, int curMassRange, double width, double dwellTimeSum, double lineBreak, double timeOffset)
        {
            AppContext.ProgressStart("formating image data...");
            try
            {

                // copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
                // the line break timings.

                int diffPixelCount = 0;
                double imageDiff = 0.0;
                bool analyzeImage = AppContext.UseApproximation;

                for (int y = 0; y < yPoints; y++)
                {
                    int currentPoint;
                    double currentTime = timeOffset + xDist / xSpeed / 2 + ((width / xSpeed + lineBreak) * y) + 0;

                    // currentLine is the offset to the start of the current line in the linear datastream (rawData)
                    // MSt int currentLine = (int)Math.Floor(Math.Abs(lineOffset + ((x2 - x1) / xSpeed / xTime + lineBreak) * y));

                    // instead of the closest data value, calc an average of overlapping points
                    int currentLine = (int)Math.Round((currentTime + dwellTimeSum - timeData[0]) / xTime);// MSt 
                    double currentLineWeight = ((currentTime + dwellTimeSum - timeData[0]) / xTime - currentLine);//MSt
                    int currentLineUpDown = (int)Math.Sign(currentLineWeight);
                    currentLineWeight = Math.Abs(currentLineWeight);

                    for (int x = 0; x < xPoints; x++)
                    {
                        if (y % 2 == 0)
                        {
                            // even y: scan direction: -->
                            currentPoint = (int)(currentLine + x);
                        }
                        else
                        {
                            // odd y:  scan direction: <--
                            currentPoint = (int)(currentLine + xPoints - 1 - x);
                        }

                        // if ((currentPoint < nrDataPoints - 1) & (currentPoint >= 10)) // in average mode, ensure that both points are within range

                        if ((currentPoint < nrDataPoints - 2) & (currentPoint >= 1))
                        {
                            //MSt imageData[x][y] = rawData[curMassRange, currentPoint];
                            //imageData[x][y] = (float)(rawData[curMassRange, currentPoint] * (1 - currentLineWeight) + imageData[x][y] + rawData[curMassRange, currentPoint + currentLineUpDown] * (currentLineWeight));
                            imageData[x][y] = (float)(rawData[curMassRange, currentPoint] * (1 - currentLineWeight) + rawData[curMassRange, currentPoint + currentLineUpDown] * (currentLineWeight));
                        }
                        else
                        {
                            imageData[x][y] = 0;
                        }

                        // perform image analyzation if heuristic optimization is in progress...
                        if (analyzeImage)
                        {
                            if (y > 0)
                            {
                                if ((imageData[x][y] + imageData[x][y - 1]) > 100)
                                {
                                    imageDiff += Math.Abs(imageData[x][y] - imageData[x][y - 1]) / (imageData[x][y] + imageData[x][y - 1]);
                                    diffPixelCount++;
                                }
                            }
                        }

                        AppContext.ProgressSetValue(100.0 * y / yPoints);
                    }
                }

                if (diffPixelCount > 0)
                {
                    imageDiff /= diffPixelCount;
                }
                return imageDiff;

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
        /// <param name="lineBreak">The calculated time a linebreak consumes.</param>
        /// <param name="timeOffset">The calculated time offset to the first valid intensity value.</param>
        /// <returns>A double value indicating the quality of the image. This is only in effect in the process of heuristic optimization.</returns>
        private double FormatImageSharp(float[][] imageData, int curMassRange, double width, double dwellTimeSum, double lineBreak, double timeOffset)
        {
            // copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
            // the line break timings.

            int diffPixelCount = 0;
            double imageDiff = 0.0;
            bool analyzeImage = AppContext.UseApproximation;

            for (int y = 0; y < yPoints; y++)
            {
                int currentPoint;
                double currentTime = timeOffset + xDist / xSpeed / 2 + ((width / xSpeed + lineBreak) * y) + 0;

                int currentLine = (int)Math.Round((currentTime + dwellTimeSum - timeData[0]) / xTime);

                for (int x = 0; x < xPoints; x++)
                {
                    if (y % 2 == 0)
                    {
                        // even y: scan direction: -->
                        currentPoint = (int)(currentLine + x);
                    }
                    else
                    {
                        // odd y:  scan direction: <--
                        currentPoint = (int)(currentLine + xPoints - 1 - x);
                    }

                    if ((currentPoint < nrDataPoints - 1) & (currentPoint >= 0))
                    {
                        imageData[x][y] = rawData[curMassRange, currentPoint];
                    }
                    else
                    {
                        imageData[x][y] = 0;
                    }

                    // perform image analyzation if heuristic optimization is in progress...
                    if (analyzeImage)
                    {
                        if (y > 0)
                        {
                            if ((imageData[x][y] + imageData[x][y - 1]) > 100)
                            {
                                imageDiff += Math.Abs(imageData[x][y] - imageData[x][y - 1]) / (imageData[x][y] + imageData[x][y - 1]);
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
        /// <param name="lineBreak">The calculated time a linebreak consumes. This value will be optimized.</param>
        /// <param name="timeOffset">The calculated time offset to the first valid intensity value. This value will be optimized.</param>
        private void HeuristicOptimization(float[][] imageData, int curMassRange, double width, double dwellTimeSum, ref double lineBreak, ref double timeOffset)
        {
            double minDiff;
            double minLineBreak;
            double minTimeOffset;
            minDiff = double.MaxValue;
            timeOffset = 0;
            minTimeOffset = timeOffset;
            minLineBreak = lineBreak;

            AppContext.ProgressStart("optimizing wiff-data...");
            try
            {
                const int kMax = 4;
                const int iMax = 21;
                for (int k = 0; k < kMax; k++)
                {
                    minDiff = double.MaxValue;
                    lineBreak -= Math.Pow(10, -k);
                    for (int i = 0; i < iMax; i++)
                    {
                        lineBreak += Math.Pow(10, -k - 1);
                        for (double j = -2; j <= 2; j += 0.1)
                        {
                            timeOffset = j;
                            double ImageDiff = FormatImageSharp(imageData, curMassRange, width, dwellTimeSum, lineBreak, timeOffset);
                            if (ImageDiff < minDiff)
                            {
                                minDiff = ImageDiff;
                                minLineBreak = lineBreak;
                                minTimeOffset = timeOffset;
                            }
                        }
                        AppContext.ProgressSetValue(100.0 * ((double)((k * iMax) + i) / (double)(kMax * iMax)));
                    }
                    lineBreak = minLineBreak;
                    timeOffset = minTimeOffset;
                }

                if (Math.Abs(lineBreak - minLineBreak) < 2) lineBreak = minLineBreak;
                if (Math.Abs(timeOffset - minTimeOffset) < 1) timeOffset = minTimeOffset;
            }
            finally
            {
                AppContext.ProgressClear();
            }
        }

    }
}
