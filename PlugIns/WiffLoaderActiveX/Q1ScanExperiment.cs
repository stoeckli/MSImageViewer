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
    /// 'Q1 Scan'.
    /// </summary>
    public class Q1ScanExperiment : WiffExperiment
    {
        private double minMass;
        private double maxMass;
        private double massStepSize;
        private int massSpecDataPoints;
        private int nrDataPoints;
        private float[] rawData;
        private float[] timeData;
        private string massRangeName;
        private double massRangeMax;
        private double massRangeMin;
        private double timeOffset;
        private int xPoints;
        private int yPoints;
        private double xSpeed;
        private double xTime;
        private double xDist;
        private double yDist;
        private double lineOffset;
        private double lineBreak;
        private float meanValue;
        private float medianValue;



        /// <summary>
        /// Constructor. Creates and initializes a Q1ScanExperiment instance.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The source from where to
        /// read the periods properties and data.</param>
        /// <param name="wiffPeriod">The <see cref="WiffPeriod"/> instance this experiment belongs to.</param>
        /// <param name="experimentIndex">The <b>zero-based</b> index of this experiment in the associated period.</param>
        public Q1ScanExperiment(FMANWiffFileClass wiffFile, WiffPeriod wiffPeriod, int experimentIndex)
            : base(wiffFile, wiffPeriod, experimentIndex)
        {
        }

        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="wiffFile"/>.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The data source.</param>
        protected override void Initialize(FMANWiffFileClass wiffFile)
        {
            // get experiment object and the experiment parameters
            ITripleQuadMALDI experimentParams = (ITripleQuadMALDI)wiffFile.GetExperimentObject(WiffPeriod.Sample.Index, WiffPeriod.Index, Index);
            Experiment experiment = (Experiment)wiffFile.GetExperimentObject(WiffPeriod.Sample.Index, WiffPeriod.Index, Index);

            MassRange massRange = (MassRange)experiment.GetMassRange(0);

            minMass = massRange.QstartMass; // anaSpec.GetStartMass();
            maxMass = massRange.QstopMass; // anaSpec.GetStopMass();
            massStepSize = massRange.QstepMass; //anaSpec.StepSize;

            // get the number of MS data points
            massSpecDataPoints = (int)((maxMass - minMass) / massStepSize + 1);

            // select 1. data points
            FMANChromData chrom = new FMANChromData();
            chrom.WiffFileName = wiffFile.GetWiffFileName();
            chrom.SetToTIC(WiffPeriod.Sample.Index, WiffPeriod.Index, Index);

            // dimension the data arrray
            nrDataPoints = chrom.GetNumberOfDataPoints();
            rawData = new float[nrDataPoints];
            timeData = new float[nrDataPoints];

            //loop through the mass ranges
            massRangeName = "TIC " + minMass.ToString() + " - " + maxMass.ToString();

            double xValue = chrom.GetDataPointXValue(1);
            double yValue = chrom.GetDataPointYValue(1);
            massRangeMax = double.MinValue;
            massRangeMin = double.MaxValue;
            timeOffset = xValue * 1000;
            // helper for mean and median
            double meanSum = 0;
            List<float> valuesForMedian = new List<float>();
            // read the data...
            for (int actDataPoint = 1; actDataPoint <= nrDataPoints; actDataPoint++)
            {
                // get the actual value...
                xValue = chrom.GetDataPointXValue(actDataPoint);
                yValue = chrom.GetDataPointYValue(actDataPoint);
                // and copy it to our local data structur
                rawData[actDataPoint - 1] = (float)yValue;
                timeData[actDataPoint - 1] = (float)xValue * 60;

                // keep track of the extrema
                if (yValue < massRangeMin)
                    massRangeMin = yValue;
                if (yValue > massRangeMax)
                    massRangeMax = yValue;

                // mean and median calculation...
                // sum up the value to calculate the mean
                meanSum += yValue;
                // fill an extra array to calculate the median
                valuesForMedian.Add((float)yValue);
            }
            // calculate the mean
            meanValue = (float)(meanSum / nrDataPoints);
            // calculate the median
            valuesForMedian.Sort();
            medianValue = ((valuesForMedian.Count % 2) == 0) ? (float)(valuesForMedian[(valuesForMedian.Count / 2) - 1] + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f : valuesForMedian[valuesForMedian.Count / 2];


#if(false) // analyze the timing values in timeData
            List<float> timeSpans = new List<float>();
            for (int i = 1; i < timeData.Length; i++)
            {
                timeSpans.Add(timeData[i] - timeData[i - 1]);
            }
            float tMean = 0;
            float tMedian = 0;
            timeSpans.Sort();
            foreach (float ts in timeSpans) tMean += ts;
            tMean = tMean / timeSpans.Count;
            tMedian = ((timeSpans.Count % 2) == 0) ? (float)(timeSpans[(timeSpans.Count / 2) - 1] + timeSpans[timeSpans.Count / 2]) / 2.0f : timeSpans[timeSpans.Count / 2];
#endif

            // fetch the x1, x2, y1, y2, width and height from the WiffSample instance this experiment belongs to.
            double x1 = WiffPeriod.Sample.P1.X;
            double y1 = WiffPeriod.Sample.P1.Y;
            double x2 = WiffPeriod.Sample.P2.X;
            double y2 = WiffPeriod.Sample.P2.Y;
            double width = WiffPeriod.Sample.Width;
            double height = WiffPeriod.Sample.Height;

            //get MALDI parmas and assign variables
            string strMALDIParams = experimentParams.TripleQuadMALDIParameters;

            // x speed in mm/s
            xSpeed = float.Parse(strMALDIParams.Split(sepMALDIParams, System.StringSplitOptions.None)[6]);

            //line distance in mm
            yDist = float.Parse(strMALDIParams.Split(sepMALDIParams, System.StringSplitOptions.None)[8]) / 1000;

            //time per point in s
            xTime = (float)((chrom.GetDataPointXValue(nrDataPoints) - chrom.GetDataPointXValue(2)) * 60 / (nrDataPoints - 2));
            xDist = (float)(int)(xSpeed * xTime * 1000) / 1000;

            //number of poins in x
            xPoints = (int)((width) / xSpeed / xTime);

            // fetch the position data from the WiffSample instance this experiment belongs to.
            uint[,] posData = WiffPeriod.Sample.PositionData;
            long posDataLength = WiffPeriod.Sample.PositionDataLength;

            // try and find the first and last valid indices in the posData, where valid means a nonzero time value...
            long firstNonZeroTimeInPos = -1;
            for (long t = 0; t < posDataLength - 1; t++)
            {
                if (posData[t, 0] > 0)
                {
                    firstNonZeroTimeInPos = t;
                    // ok, we're done...
                    break;
                }
            }

            long lastNonZeroTimeInPos = -1;
            for (long t = posDataLength - 1; t >= 0; t++)
            {
                if (posData[t, 0] > 0)
                {
                    lastNonZeroTimeInPos = t;
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
            y2 = (double)Math.Round((decimal)posData[lastNonZeroTimeInPos, 2] / 1000, 2);

            // ...and this has an effect of the ypoints
            yPoints = (int)Math.Round((decimal)((y2 - y1) / yDist)) + 1;
            

            double timeSpanPos = posData[lastNonZeroTimeInPos, 0] / 1000.0 - posData[firstNonZeroTimeInPos, 0] / 1000.0;

            if (yPoints % 2 == 0)
            {
                // even number of scanlines
                lineBreak = (timeSpanPos
                - (x2 - (float)posData[firstNonZeroTimeInPos, 1] / 1000) / xSpeed
                - (yPoints - 2) * (width) / xSpeed
                - (x2 - (float)posData[lastNonZeroTimeInPos, 1] / 1000) / xSpeed)
                / (yPoints - 1);
            }
            else
            {
                // odd number of scanlines
                lineBreak = (timeSpanPos
                - (x2 - (float)posData[firstNonZeroTimeInPos, 1] / 1000) / xSpeed
                - (yPoints - 2) * (width) / xSpeed
                - ((float)posData[lastNonZeroTimeInPos, 1] / 1000 - x1) / xSpeed)
                / (yPoints - 1);
            }

            lineBreak = lineBreak / xTime;

            timeOffset = (float)posData[firstNonZeroTimeInPos, 0] / 1000 - (((float)posData[firstNonZeroTimeInPos, 1] / 1000 - x1) / xSpeed);

            lineOffset = (5 - (timeData[5] - timeOffset) / xTime) + 1;

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // TIC throughout the total massrange...

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // format the data into a rectangular, 2 dimensional array of floats that will represent the image data
            //

            AppContext.ProgressStart("formating image data...");

            // prepare the data structure
            float[][] dataTIC;
            try 
	        {
                dataTIC = new float[xPoints][];
                for (int i = 0; i < dataTIC.Length; i++)
                {
                    dataTIC[i] = new float[yPoints];
                }

                // copy the data from wiff file datastream to the rectangular array. Take account of the line offset and
                // the line break timings.
                for (int y = 0; y < yPoints; y++)
                {
                    int currentPoint;
                    // currentLine is the offset to the start of the current line in the linear datastream (rawData)
                    int currentLine = (int)Math.Floor(Math.Abs(lineOffset + ((x2 - x1) / xSpeed / xTime + lineBreak) * y));
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
                            dataTIC[x][y] = rawData[currentPoint];
                        }
                        else
                        {
                            dataTIC[x][y] = 0;
                        }
                    }
                    AppContext.ProgressSetValue(100.0 * y / yPoints);
                }
            }
	        finally
	        {
                AppContext.ProgressClear();
	        }

            // create the appropriate dataset and add it to the WiffFileContent object...
            string imgName = WiffPeriod.Sample.Name + " : " + massRangeName;
            Document doc = WiffPeriod.Sample.WiffFileContent.Document;
            //create the meta information data structure and populate with relevant information...
            ImageMetaData metaData = new ImageMetaData();
            try
            {
                metaData.Add("Sample Name", typeof(string), WiffPeriod.Sample.Name, false);
                metaData.Add("Mass Range", typeof(string), massRangeName, false);
                string[] splitted = strMALDIParams.Split(sepMALDIParams, System.StringSplitOptions.None);
                metaData.Add("Laser Frequency (Hz)", typeof(string), splitted[1], false);
                metaData.Add("Laser Power (%)", typeof(string), splitted[2], false);
                metaData.Add("Ablation Mode", typeof(string), splitted[3], false);
                metaData.Add("Skimmer Voltage (V)", typeof(string), splitted[4], false);
                metaData.Add("Source Gas", typeof(string), splitted[5], false);
                metaData.Add("Raster Speed (mm/s)", typeof(string), splitted[6], false);
                metaData.Add("Raster Pitch", typeof(string), splitted[8], false);
            }
            catch (Exception e) { Util.ReportException(e); }

            ImageData imageTIC = new ImageData(doc, dataTIC, imgName, metaData, (float)massRangeMin, (float)massRangeMax,
                (float)meanValue, (float)medianValue, (float)xDist, (float)yDist, (float)minMass, (float)maxMass);


            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            // Q1 spectrum scan...
            FMANSpecData spec = new FMANSpecData();
            spec.WiffFileName = wiffFile.GetWiffFileName();

            // prepare the data structure
            List<float[][]> dataList;
            List<ImageData> imageDataList;
            AppContext.ProgressStart("formating spectrum data...");
            try
            {
                dataList = new List<float[][]>();
                for (int i = 0; i < massSpecDataPoints; i++)
                {
                    float[][] data = new float[xPoints][];
                    for (int j = 0; j < xPoints; j++)
                    {
                        data[j] = new float[yPoints];
                    }
                    dataList.Add(data);
                }


                for (int y = 0; y < yPoints; y++)
                {
                    int currentPoint;
                    // currentLine is the offset to the start of the current line in the linear datastream (rawData)
                    int currentLine = (int)Math.Floor(Math.Abs(lineOffset + ((x2 - x1) / xSpeed / xTime + lineBreak) * y));
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
                        if (currentPoint >= nrDataPoints)
                        {
                            continue;
                        }

                        float currentTime = (float)chrom.GetXValueInSec(currentPoint + 1);
                        spec.SetSpectrum(WiffPeriod.Sample.Index, WiffPeriod.Index, Index, currentTime, currentTime);
                        int specDataPoints = spec.GetNumberOfDataPoints();

                        for (int k = 1; k <= specDataPoints; k++)
                        {
                            int specIndex = (int)((spec.GetDataPointXValue(k) - minMass) / massStepSize + 0.4);
                            dataList[specIndex][x][y] = (float)spec.GetDataPointYValue(k);
                        }
                    }
                    AppContext.ProgressSetValue(100.0 * y / yPoints);
                }

                // create the list of imageData objects passed to the imageSpectrumData object on it's creation later on...
                imageDataList = new List<ImageData>();
                for (int k = 0; k < massSpecDataPoints; k++)
                {
                    float[][] specData = dataList[k];
                    float mass = (float)(minMass + (massStepSize * k));
                    imgName = WiffPeriod.Sample.Name + " : " + mass.ToString();

                    // TODO -- rethink if one should really calculate the mean, median, min, max etc. if the images aren't used for imaging but for export...
                    float minInt = float.MaxValue;
                    float maxInt = float.MinValue;
                    float mean = 0;
                    float median = 0;
                    // helper for mean and median
                    meanSum = 0;
                    valuesForMedian.Clear();
                    {
                        for (int x = 0; x < xPoints; x++)
                        {
                            for (int y = 0; y < yPoints; y++)
                            {
                                float value = specData[x][y];

                                // keep track of the extrema
                                if (value < minInt)
                                    minInt = value;
                                if (value > maxInt)
                                    maxInt = value;

                                // mean and median calculation...
                                // sum up the value to calculate the mean
                                meanSum += value;
                                // fill an extra array to calculate the median
                                valuesForMedian.Add(value);
                            }
                        }
                    }
                    // calculate the mean
                    mean = (float)(meanSum / (xPoints * yPoints));
                    // calculate the median
                    valuesForMedian.Sort();
                    median = ((valuesForMedian.Count % 2) == 0) ? (float)(valuesForMedian[(valuesForMedian.Count / 2) - 1] + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f : valuesForMedian[valuesForMedian.Count / 2];

                    // ok, now create the imageData and add to list...
                    ImageData imageData = new ImageData(doc, specData, imgName, new ImageMetaData(), minInt, maxInt, mean, median, (float)xDist, (float)yDist, (float)0.0f, (float)mass);
                    imageDataList.Add(imageData);
                }
            }
            finally
            {
                AppContext.ProgressClear();
            }


            // now everything should be set to create the ImageSpectrumData object...
            imgName = WiffPeriod.Sample.Name + " SPECT " + minMass.ToString() + " - " + maxMass.ToString();
            ImageSpectrumData imageSpectrum = new ImageSpectrumData(doc, imgName, new ImageMetaData(), imageDataList, (float)minMass, (float)maxMass, (float)massStepSize);
            imageSpectrum.ImageTIC = imageTIC;
            WiffPeriod.Sample.WiffFileContent.Add(imageSpectrum);


#if(false)
            // TODO -- find out what to do with the 'globalMassMax' value
            float anaTime = (float)chrom.GetXValueInSec(nrDataPoints - 1);
            chrom.SetToBPC(WiffPeriod.Sample.Index, WiffPeriod.Index, Index, 0, anaTime, minMass, maxMass, 2 * massStepSize);
            double tempMass;
            chrom.GetYValueRange(out tempMass, out globalMassMax);

            chrom.SetToTIC(WiffPeriod.Sample.Index, WiffPeriod.Index, Index);
#endif

            chrom.WiffFileName = "";
            chrom = null;
            spec.WiffFileName = "";
            spec = null;
        }
    }
}
