#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="MsScanExperiment.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 28/11/2012 - Style Cop Updates
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.WiffLoader
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Linq;

    using Clearcore2.Data;
    using Clearcore2.Data.DataAccess.SampleData;
    using Novartis.Msi.Core;

    /// <summary>
    /// This class encapsulates the specific experiment type (MS)
    /// </summary>
    public class MsScanExperiment : WiffExperiment
    {
        #region Fields

        /// <summary>
        /// The MSExperiment attached to this object
        /// </summary>
        private MSExperiment msexperiment;

        /// <summary>
        /// Min Mass in Range
        /// </summary>
        private double minMass;

        /// <summary>
        /// Max Mass in Range
        /// </summary>
        private double maxMass;

        /// <summary>
        /// Step size in mass range
        /// </summary>
        private double massStepSize;

        /// <summary>
        /// Number of points in spectrum range
        /// </summary>
        private int massSpecDataPoints;

        /// <summary>
        /// Binned massSpecDataPoints
        /// </summary>
        private int binnedmassSpecDataPoints;

        /// <summary>
        /// Number of data Points
        /// </summary>
        private int numDataPoints;

        /// <summary>
        /// Array for the data points in y axis
        /// </summary>
        private float[] rawData;

        /// <summary>
        /// Array for the time points
        /// </summary>
        private float[] timeData;

        /// <summary>
        /// Mass range name
        /// </summary>
        private string massRangeName;

        /// <summary>
        /// Maximum value for massRange 
        /// </summary>
        private double massRangeMax;

        /// <summary>
        /// Minimum value for massRange
        /// </summary>
        private double massRangeMin;

        /// <summary>
        /// time offset
        /// </summary>
        private double timeOffset;

        /// <summary>
        /// Number of points on X Axis
        /// </summary>
        private int numPointsOnXAxis;

        /// <summary>
        /// Number of points on Y Axis
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
        /// Line off set value
        /// </summary>
        private double lineOffset;

        /// <summary>
        /// Line Break value
        /// </summary>
        private double lineBreak;

        /// <summary>
        /// Mean Value
        /// </summary>
        private float meanValue;

        /// <summary>
        /// Median Value
        /// </summary>
        private float medianValue;

        /// <summary>
        /// x1 pos of sample
        /// </summary>
        private double x1;

        /// <summary>
        /// y1 pos of sample
        /// </summary>
        private double y1;

        /// <summary>
        /// x2 pos of sample
        /// </summary>
        private double x2;

        /// <summary>
        /// y2 pos of sample
        /// </summary>
        private double y2;

        /// <summary>
        /// width of the sample
        /// </summary>
        private double width;

        /// <summary>
        /// List of data arrays that hold data for ImageList
        /// </summary>
        private List<float[][]> dataList;

        /// <summary>
        /// Local array to store points of interest for sample
        /// used to extract the mass spectrum for imagelist
        /// </summary>
        private int[][] sampleDataPos;

        /// <summary>
        /// Bin Size
        /// </summary>
        private int binsize;

        /// <summary>
        /// Mass Calibration
        /// </summary>
        private float[] masscal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MsScanExperiment"/> class
        /// </summary>
        /// <param name="msexperiment">MSScan type Experiment</param>
        /// <param name="wiffsample">Wiff Sample</param>
        public MsScanExperiment(MSExperiment msexperiment, WiffSample wiffsample)
            : base(msexperiment, wiffsample)
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the Bin Size
        /// The BinSize property exposed to external object is set to the newbinsize
        /// </summary>
        public int BinSize
        {
            get
            {
                return this.binsize;
            }
        }

        /// <summary>
        /// Gets or sets the Mass Calibration array
        /// </summary>
        public float[] MassCal
        {
            get
            {
                return this.masscal;
            }

            set
            {
                this.masscal = value;
            }
        }

        #endregion Properties

        #region Methods
        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="inputMsExperiment"/>
        /// </summary>
        /// <param name="inputMsExperiment">Mass Spec Experiment</param>
        /// <param name="wiffsample">Wiff Sample</param>
        protected override void Initialize(MSExperiment inputMsExperiment, WiffSample wiffsample)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                // (1) Get Maldi Parameters
                string strMaldiParams = wiffsample.MaldiParametersString;

                this.msexperiment = inputMsExperiment;

                // (2) Get the number of mass transitions - For MS experiments by default we will look at the first scan ([0])
                MassRange massRange = this.msexperiment.Details.MassRangeInfo[0];
                var fullScanMassRange = massRange as FullScanMassRange;

                if (fullScanMassRange != null)
                {
                    this.minMass = fullScanMassRange.StartMass;
                    this.maxMass = fullScanMassRange.EndMass;
                    this.massStepSize = fullScanMassRange.StepSize;
                }

                this.massSpecDataPoints = (int)(((this.maxMass - this.minMass) / this.massStepSize) + 1);

                // (3) Populate masscal 
                this.masscal = new float[this.massSpecDataPoints];

                for (int i = 0; i < this.massSpecDataPoints; i++)
                {
                    this.masscal[i] = (float)(this.minMass + (i * this.massStepSize));
                }

                // (4) Select 1 trace  
                XYData tic = this.msexperiment.GetTotalIonChromatogram();

                // (5) Dimension the data array, the number "data points"
                this.numDataPoints = tic.NumDataPoints;

                this.rawData = new float[this.numDataPoints];
                this.timeData = new float[this.numDataPoints];

                // (6) mass range info
                this.massRangeName = "TIC " + this.msexperiment.Details.MassRangeInfo[0].Name;

                double actualXValue = tic.GetActualXValues()[0];
                this.massRangeMax = double.MinValue;
                this.massRangeMin = double.MaxValue;
                this.timeOffset = actualXValue * 1000;

                // helper for mean and median
                double meanSum = 0;
                var valuesForMedian = new List<float>();

                // (7) read the data...
                for (int actDataPoint = 0; actDataPoint < this.numDataPoints; actDataPoint++)
                {
                    // get the actual value...
                    actualXValue = tic.GetActualXValues()[actDataPoint];
                    double actualYValue = tic.GetActualYValues()[actDataPoint];

                    // and copy it to our local data structure
                    this.rawData[actDataPoint] = (float)actualYValue;
                    this.timeData[actDataPoint] = (float)actualXValue * 60;

                    // keep track of the extrema
                    if (actualYValue < this.massRangeMin)
                    {
                        this.massRangeMin = actualYValue;
                    }

                    if (actualYValue > this.massRangeMax)
                    {
                        this.massRangeMax = actualYValue;
                    }

                    // mean and median calculation... sum up the value to calculate the mean
                    meanSum += actualYValue;

                    // fill an extra array to calculate the median
                    valuesForMedian.Add((float)actualYValue);
                }

                // (8) Calculate the mean
                this.meanValue = (float)(meanSum / this.numDataPoints);

                // (9) Calculate the median
                valuesForMedian.Sort();
                this.medianValue = ((valuesForMedian.Count % 2) == 0) ? (valuesForMedian[(valuesForMedian.Count / 2) - 1] + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f : valuesForMedian[valuesForMedian.Count / 2];

                // (10) fetch the x1, x2, y1, y2, width and height from the WiffSample instance this experiment belongs to.
                this.x1 = wiffsample.P1.X;
                this.y2 = 82 - wiffsample.P1.Y;
                this.x2 = wiffsample.P2.X;
                this.width = wiffsample.Width;

                // (11) time per point in s
                this.timeinXDirection = (float)((tic.GetActualXValues()[this.numDataPoints - 1] - tic.GetActualXValues()[1]) * 60 / (this.numDataPoints - 2));

                // (12) fetch the position data from the WiffSample instance this experiment belongs to.
                uint[,] posData = wiffsample.PositionData;
                long posDataLength = wiffsample.PositionDataLength;

                // try and find the first and last valid indices in the posData, where valid means a nonzero time value...
                long firstNonZeroTimeInPos = -1;
                for (long t = 0; t < posDataLength - 1; t++)
                {
                    if (posData[t, 0] > 0)
                    {
                        firstNonZeroTimeInPos = t;

                        break;
                    }
                }

                long lastNonZeroTimeInPos = -1;
                for (long t = posDataLength - 1; t >= 0; t++)
                {
                    if (posData[t, 0] > 0)
                    {
                        lastNonZeroTimeInPos = t;

                        break;
                    }
                }

                if (firstNonZeroTimeInPos < 0 || lastNonZeroTimeInPos < 0)
                {
                    // haven't found a valid posData triplet. All time values are zero or less... bail out (Put in an Error Message here?)
                    return;
                }

                // (13) Distance in Y direction
                this.distInYDirection = 0;
                for (long t = firstNonZeroTimeInPos; t < lastNonZeroTimeInPos; t++)
                {
                    if ((posData[t, 1] > ((this.x2 + this.x1) * 500)) & Equals(this.distInYDirection, 0.0))
                    {
                        this.distInYDirection = posData[t, 2];
                    }

                    if ((posData[t, 1] < ((this.x2 + this.x1) * 500)) & (this.distInYDirection > 0))
                    {
                        this.distInYDirection = (float)Math.Round((decimal)(posData[t, 2] - this.distInYDirection) / 500, (int)(1 - Math.Log10((posData[t, 2] - this.distInYDirection) / 500))) / 2;
                        break;
                    }
                }

                // (14) calculate speed in x direction
                this.speedinXDirection = (float)Math.Round(((posData[firstNonZeroTimeInPos + 2, 1] - posData[firstNonZeroTimeInPos + 1, 1]) / (decimal)(posData[firstNonZeroTimeInPos + 2, 0] - posData[firstNonZeroTimeInPos + 1, 0]) * 2), 0) / 2;

                // (15) distInXDirection
                this.distInXDirection = (float)(int)(this.speedinXDirection * this.timeinXDirection * 1000) / 1000;

                // (16) number of points in x
                this.numPointsOnXAxis = (int)(this.width / this.speedinXDirection / this.timeinXDirection);

                // (17) number of points in y
                // y1 from the wiff file is not the actual y1 from the stage - replace with value from path file... 
                this.y1 = 82 - (double)Math.Round(((decimal)posData[lastNonZeroTimeInPos, 2] / 1000), 2);

                // ...and this has an effect of the ypoints
                this.numPointsOnYAxis = (int)Math.Round((decimal)((this.y2 - this.y1) / this.distInYDirection) + 1);

                // (18) calculate the line breaks
                var timeSpanPos = (posData[lastNonZeroTimeInPos, 0] / 1000.0)
                                  - (posData[firstNonZeroTimeInPos, 0] / 1000.0);

                if (this.numPointsOnYAxis % 2 == 0)
                {
                    // even number of scanlines
                    this.lineBreak = (timeSpanPos - ((this.x2 - ((float)posData[firstNonZeroTimeInPos, 1] / 1000)) / this.speedinXDirection)
                                      - (((this.numPointsOnYAxis - 2) * this.width) / this.speedinXDirection)
                                      - ((this.x2 - ((float)posData[lastNonZeroTimeInPos, 1] / 1000)) / this.speedinXDirection)) / (this.numPointsOnYAxis - 1);
                }
                else
                {
                    // odd number of scanlines
                    this.lineBreak = (timeSpanPos - ((this.x2 - ((float)posData[firstNonZeroTimeInPos, 1] / 1000)) / this.speedinXDirection)
                                      - (((this.numPointsOnYAxis - 2) * this.width) / this.speedinXDirection)
                                      - ((((float)posData[lastNonZeroTimeInPos, 1] / 1000) - this.x1) / this.speedinXDirection)) / (this.numPointsOnYAxis - 1);
                }

                this.lineBreak = this.lineBreak / this.timeinXDirection;

                this.timeOffset = ((float)posData[firstNonZeroTimeInPos, 0] / 1000)
                                  - ((((float)posData[firstNonZeroTimeInPos, 1] / 1000) - this.x1) / this.speedinXDirection);

                this.lineOffset = (5 - ((this.timeData[5] - this.timeOffset) / this.timeinXDirection)) + 1;

                // TIC throughout the total massrange...
                // format the data into a rectangular, 2 dimensional array of floats that will represent the image data
                AppContext.ProgressStart("formatting image data...");

                // (19) copy the data from wiff file datastream to the rectangular array. Take account of the line offset and the line break timings
                float[][] dataTic;

                try
                {
                    dataTic = new float[this.numPointsOnXAxis][];
                    this.sampleDataPos = new int[this.numPointsOnXAxis][];

                    for (int i = 0; i < dataTic.Length; i++)
                    {
                        dataTic[i] = new float[this.numPointsOnYAxis];
                        this.sampleDataPos[i] = new int[this.numPointsOnYAxis];
                    }

                    for (int pointOnYAxis = 0; pointOnYAxis < this.numPointsOnYAxis; pointOnYAxis++)
                    {
                        // currentLine is the offset to the start of the current line in the linear datastream (rawData)
                        var currentLine = (int)Math.Floor(Math.Abs(this.lineOffset
                                    + (((((this.x2 - this.x1) / this.speedinXDirection) / this.timeinXDirection)
                                      + this.lineBreak) * pointOnYAxis)));
                        for (int pointOnXAxis = 0; pointOnXAxis < this.numPointsOnXAxis; pointOnXAxis++)
                        {
                            int currentPoint;
                            if (pointOnYAxis % 2 == 0)
                            {
                                // even y: scan direction: -->
                                currentPoint = currentLine + pointOnXAxis;
                            }
                            else
                            {
                                // odd y:  scan direction: <--
                                currentPoint = currentLine + this.numPointsOnXAxis - 1 - pointOnXAxis;
                            }

                            if (currentPoint < this.numDataPoints)
                            {
                                dataTic[pointOnXAxis][this.numPointsOnYAxis - 1 - pointOnYAxis] = this.rawData[currentPoint];

                                // Add in a new array to save these dataPos. We can use them to get the scan number
                                // Scan number is the current point
                                this.sampleDataPos[pointOnXAxis][this.numPointsOnYAxis - 1 - pointOnYAxis] = currentPoint;
                            }
                            else
                            {
                                dataTic[pointOnXAxis][this.numPointsOnYAxis - 1 - pointOnYAxis] = 0;
                                this.sampleDataPos[pointOnXAxis][this.numPointsOnYAxis - 1 - pointOnYAxis] = 0;
                            }
                        }

                        AppContext.ProgressSetValue((100.0 * pointOnYAxis) / this.numPointsOnYAxis);
                    }
                }
                finally
                {
                    AppContext.ProgressClear();
                }

                // (20) create the appropriate dataset and add it to the WiffFileContent object...
                string imgName = wiffsample.Name + " : " + this.massRangeName;
                Document doc = wiffsample.WiffFileContent.Document;

                // (21) Calculate Bin Points
                // Set a default bin size of 1 - Important it is set to 1 and not zero
                this.binsize = 1; 
                this.GetBinSize(wiffsample.ScanFileSize, this.numPointsOnXAxis, this.numPointsOnYAxis);

                // By default we set this to massSpecDataPoints
                this.binnedmassSpecDataPoints = this.massSpecDataPoints;

                if (this.binsize > 1)
                {
                    this.binnedmassSpecDataPoints = this.massSpecDataPoints / this.binsize;
                }

                // create the meta information data structure and populate with relevant information...
                var metaData = new ImageMetaData();
                try
                {
                    const float Epsilon = (float)1E-10;
                    metaData.Add("Sample Name", typeof(string), wiffsample.Name, false);
                    metaData.Add("Mass Range", typeof(string), this.massRangeName, false);
                    metaData.Add("Mass Step Size", typeof(string), this.massStepSize, false);
                    metaData.Add("X1 (mm)", typeof(string), (Math.Abs(this.x1 - (int)this.x1) < Epsilon) ? this.x1.ToString("0.0") : this.x1.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Y1 (mm)", typeof(string), (Math.Abs(this.y1 - (int)this.y1) < Epsilon) ? this.y1.ToString("0.0") : this.y1.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("X2 (mm)", typeof(string), (Math.Abs(this.x2 - (int)this.x2) < Epsilon) ? this.x2.ToString("0.0") : this.x2.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Y2 (mm)", typeof(string), (Math.Abs(this.y2 - (int)this.y2) < Epsilon) ? this.y2.ToString("0.0") : this.y2.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Data Points in X", typeof(string), this.numPointsOnXAxis.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Data Points in Y", typeof(string), this.numPointsOnYAxis.ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Point Width (mm)", typeof(string), Math.Round(this.distInXDirection, 2).ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Point Height (mm)", typeof(string), Math.Round(this.distInYDirection, 2).ToString(CultureInfo.InvariantCulture), false);
                    metaData.Add("Bin Size", typeof(string), this.binsize.ToString(CultureInfo.InvariantCulture), false);

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

                // (22) Create the ImageData
                var imageTic = new ImageData(
                    doc,
                    dataTic,
                    imgName,
                    metaData,
                    (float)this.massRangeMin,
                    (float)this.massRangeMax,
                    this.meanValue,
                    this.medianValue,
                    (float)this.distInXDirection,
                    (float)this.distInYDirection,
                    this.masscal,
                    Core.ExperimentType.MS);

                // (23) Collate the Spectrum Data
                List<ImageData> imageDataList = null;

                try
                {
                    // (24) Create a list of array's to hold the mass spec images (in effect a 3x3 array or a list of 2x2 array)
                    this.dataList = new List<float[][]>();
                    for (int numMassSpecDataPts = 0; numMassSpecDataPts < this.binnedmassSpecDataPoints; numMassSpecDataPts++) 
                    {
                        var data = new float[this.numPointsOnXAxis][];
                        for (int pointOnXAxis = 0; pointOnXAxis < this.numPointsOnXAxis; pointOnXAxis++)
                        {
                            data[pointOnXAxis] = new float[this.numPointsOnYAxis];
                        }

                        this.dataList.Add(data);
                    }

                    // (25) Populate dataList. We can populate this by Scan (PopulateListByScan()) Or MassSpec (PopulateListByMass())
                    // For now we will use by Scan as it is more effcient 
                    this.PopulateListByScan();

                    // (26) Create the list of imageData objects passed to the imageSpectrumData object on it's creation later on...
                    imageDataList = new List<ImageData>();

                    for (int massSpecDataPt = 0; massSpecDataPt < this.binnedmassSpecDataPoints; massSpecDataPt++)
                    {
                        float[][] specData = this.dataList[massSpecDataPt];

                        imgName = wiffsample.Name + " : " + this.massRangeName;

                        // TODO -- rethink if one should really calculate the mean, median, min, max etc. if the images aren't used for imaging but for export...
                        float minInt = float.MaxValue;
                        float maxInt = float.MinValue;

                        // helper for mean and median
                        meanSum = 0;
                        valuesForMedian.Clear();
                        {
                            for (int x = 0; x < this.numPointsOnXAxis; x++)
                            {
                                for (int y = 0; y < this.numPointsOnYAxis; y++)
                                {
                                    float value = specData[x][y];

                                    // keep track of the extrema
                                    if (value < minInt)
                                    {
                                        minInt = value;
                                    }

                                    if (value > maxInt)
                                    {
                                        maxInt = value;
                                    }

                                    // mean and median calculation...
                                    // sum up the value to calculate the mean
                                    meanSum += value;

                                    // fill an extra array to calculate the median
                                    valuesForMedian.Add(value);
                                }
                            }
                        }

                        // calculate the mean
                        var mean = (float)(meanSum / (this.numPointsOnXAxis * this.numPointsOnYAxis));

                        // calculate the median
                        valuesForMedian.Sort();
                        float median = ((valuesForMedian.Count % 2) == 0)
                                           ? (valuesForMedian[(valuesForMedian.Count / 2) - 1]
                                              + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f
                                           : valuesForMedian[valuesForMedian.Count / 2];

                        // ok, now create the imageData and add to list...
                        var imageData = new ImageData(
                            doc,
                            specData,
                            imgName,
                            metaData,
                            minInt,
                            maxInt,
                            mean,
                            median,
                            (float)this.distInXDirection,
                            (float)this.distInYDirection,
                            this.masscal,
                            Core.ExperimentType.MS);
                        imageDataList.Add(imageData);
                    }
                }
                catch (Exception e)
                {
                    Util.ReportException(e);
                }

                // (27) Now everything should be set to create the ImageSpectrumData object...
                imgName = wiffsample.Name + " SPECT " + this.massRangeMin.ToString(CultureInfo.InvariantCulture) + " - " + this.massRangeMax.ToString(CultureInfo.InvariantCulture);

                var imageSpectrum = new ImageSpectrumData(
                    doc,
                    imgName,
                    metaData,
                    this.masscal,
                    imageDataList,
                    Core.ExperimentType.MS) { ImageTic = imageTic };

                wiffsample.WiffFileContent.Add(imageSpectrum);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// This routine gets the full list of Image Data for an MS Scan
        /// Populate dataList with data from GetMassSpectrum()
        /// Routine Requires sampleDataPos[][] to be populated
        /// </summary>
        private void PopulateListByScan()
        {
            AppContext.ProgressStart("Populating Imagelist...");

            try
            {
                if (this.binsize == 1)
                {
                    // Origianal working routine - works for no binning
                    for (int pointOnXAxis = 0; pointOnXAxis < this.numPointsOnXAxis; pointOnXAxis++)
                    {
                        for (int pointOnYAxis = 0; pointOnYAxis < this.numPointsOnYAxis; pointOnYAxis++)
                        {
                            int currspecPoint = this.sampleDataPos[pointOnXAxis][pointOnYAxis];
                            MassSpectrum spec = this.msexperiment.GetMassSpectrum(currspecPoint);
                            int specDataPoints = spec.NumDataPoints;

                            for (int specDataPoint = 0; specDataPoint < specDataPoints; specDataPoint++)
                            {
                                var specIndex = (int)((spec.GetXValue(specDataPoint) - this.minMass) / this.massStepSize);
                                this.dataList[specIndex][pointOnXAxis][pointOnYAxis] = (float)spec.GetYValue(specDataPoint);
                            }
                        }

                        AppContext.ProgressSetValue(100.0 * pointOnXAxis / this.numPointsOnXAxis);
                    } 
                }
                else
                {
                    for (int pointOnXAxis = 0; pointOnXAxis < this.numPointsOnXAxis; pointOnXAxis++)
                    {
                        for (int pointOnYAxis = 0; pointOnYAxis < this.numPointsOnYAxis; pointOnYAxis++)
                        {
                            // 1) Declare local array
                            var fullArray = new float[this.massSpecDataPoints];
                            var binnedArray = new float[this.binnedmassSpecDataPoints];

                            int currspecPoint = this.sampleDataPos[pointOnXAxis][pointOnYAxis];
                            MassSpectrum spec = this.msexperiment.GetMassSpectrum(currspecPoint);
                            int specDataPoints = spec.NumDataPoints;

                            // 1) Fill out a full array 
                            for (int specDataPoint = 0; specDataPoint < specDataPoints; specDataPoint++)
                            {
                                var specIndex = (int)((spec.GetXValue(specDataPoint) - this.minMass) / this.massStepSize);
                                fullArray[specIndex] = (float)spec.GetYValue(specDataPoint);
                            }

                            // 2) Bin the full array
                            for (int binspecDataPoint = 0; binspecDataPoint < this.binnedmassSpecDataPoints; binspecDataPoint++)
                            {
                                float sumofIntensities = 0.0f;
                                for (int binindex = 0; binindex < this.binsize; binindex++)
                                {
                                    sumofIntensities += fullArray[(2 * binspecDataPoint) + binindex];
                                }

                                binnedArray[binspecDataPoint] = sumofIntensities / this.binsize;
                            }

                            // 3) Fill in contents of this.dataList[]
                            for (int binspecDataPoint = 0; binspecDataPoint < this.binnedmassSpecDataPoints; binspecDataPoint++)
                            {
                                this.dataList[binspecDataPoint][pointOnXAxis][pointOnYAxis] = binnedArray[binspecDataPoint];
                            }
                        }

                        AppContext.ProgressSetValue(100.0 * pointOnXAxis / this.numPointsOnXAxis);
                    }
                }
            }
            finally
            {
                AppContext.ProgressClear();
            }
        }

        /// <summary>
        /// This routine gets the full list of Image Data for an MS Scan
        /// It uses the GetExtractedIonChromatogram() method to extract the data
        /// Runs a little slow.. so using the PopulateListByScan() for now.
        /// </summary>
        private void PopulateListByMass()
        {
            AppContext.ProgressStart("Populating Imagelist...");
            
            try
            {
                // (1) Loop through all the masses in the MassRange - We will use size of the dataList to specify number of masses in the range
                foreach (float[][] specDataPoints in this.dataList)
                {
                    // (2) Use GetExtractedIonChromatogram to get a specified mass
                    int currMassIndex = this.dataList.IndexOf(specDataPoints);
                    double massSpecNumber = this.minMass + (currMassIndex * this.massStepSize);

                    // Use startmass = endmass to get the full data set for a specific mass
                    var option = new ExtractedIonChromatogramSettings(massSpecNumber, massSpecNumber);

                    // Use GetExtractedIonChromatogram to get a specified mass
                    ExtractedIonChromatogram xic = this.msexperiment.GetExtractedIonChromatogram(option);
                    var specRawData = new double[this.numDataPoints];

                    // (3) For each massSpectrum collate all the intensities for each X value
                    for (int actDataPoint = 0; actDataPoint < this.numDataPoints; actDataPoint++)
                    {
                        // GetActualYValues Gets the intensity 
                        specRawData[actDataPoint] = xic.GetActualYValues()[actDataPoint];
                    }

                    // (4) Populate this.dataList.massSpecDataPoints 
                    for (int pointOnYAxis = 0; pointOnYAxis < this.numPointsOnYAxis; pointOnYAxis++)
                    {
                        // currentLine is the offset to the start of the current line in the linear datastream (rawData)
                        var currentLine = (int)Math.Floor(Math.Abs(this.lineOffset + (((((this.x2 - this.x1) / this.speedinXDirection) / this.timeinXDirection) + this.lineBreak) * pointOnYAxis)));
                        for (int pointOnXAxis = 0; pointOnXAxis < this.numPointsOnXAxis; pointOnXAxis++)
                        {
                            int currentPoint;
                            if (pointOnYAxis % 2 == 0)
                            {
                                // even y: scan direction: -->
                                currentPoint = currentLine + pointOnXAxis;
                            }
                            else
                            {
                                // odd y:  scan direction: <--
                                currentPoint = currentLine + this.numPointsOnXAxis - 1 - pointOnXAxis;
                            }

                            if (currentPoint < this.numDataPoints)
                            {
                                this.dataList[currMassIndex][pointOnXAxis][this.numPointsOnYAxis - 1 - pointOnYAxis] = (float)specRawData[currentPoint]; 
                            }
                            else
                            {
                                this.dataList[currMassIndex][pointOnXAxis][this.numPointsOnYAxis - 1 - pointOnYAxis] = 0;
                            }
                        }
                    }

                    AppContext.ProgressSetValue((100.0 / this.massSpecDataPoints) * currMassIndex);
                }
            }
            finally
            {
                AppContext.ProgressClear();
            }
        }

        /// <summary>
        /// The function gets the binsize
        /// </summary>
        /// <param name="scanfilesize">Size of the .scan file</param>
        /// <param name="pointsInXDirection">Points in X Direction</param>
        /// <param name="pointsInYDirection">Poitns in Y Direction</param>
        private void GetBinSize(long scanfilesize, float pointsInXDirection, float pointsInYDirection)
        {
            // 1) We only want to do binning for files bigger than 100MB filesize > pathfilesize 
            string appPath  = Application.StartupPath;
            appPath  += "\\ApplicationSettings.xml";
            int filesizelimit = this.GetPathFileSizeLimit(appPath);

            if (scanfilesize > filesizelimit) 
            {
                // 2) Open up BinNumberWindow
                MassRange massRange = this.msexperiment.Details.MassRangeInfo[0];
                var fullScanMassRange = massRange as FullScanMassRange;

                if (fullScanMassRange != null)
                {
                    var binNumberWindow = new BinNumberWindow(pointsInXDirection, pointsInYDirection, this.masscal);

                    if (binNumberWindow.ShowDialog() == true)
                    {
                        if (binNumberWindow.BinSize > 0)
                        {
                            this.binsize = binNumberWindow.BinSize;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// This function gets the path file filesize limit 
        /// </summary>
        /// <param name="applicationSettingsFile">Path for the Application settings file</param>
        /// <returns>size of the file limit</returns>
        private int GetPathFileSizeLimit(string applicationSettingsFile) 
        {
            int result = 0;

            if (string.IsNullOrEmpty(applicationSettingsFile))
            {
                return result;
            }

            if (!File.Exists(applicationSettingsFile))
            {
                return result;
            }

            XDocument xmlApplicationsSettings = XDocument.Load(applicationSettingsFile);

            XElement root = xmlApplicationsSettings.Root;

            if (root == null || root.Name != "ApplicationSettings")
            {
                return result;
            }

            foreach (XElement applicationSettingItem in root.Nodes())
            {
                if (applicationSettingItem == null)
                {
                    continue;
                }

                // retrieve the "FileSizeLimit" of the Application Setting Item
                XAttribute attrName = applicationSettingItem.Attribute("Name"); 
                if (attrName != null)
                {
                    string itemvalue = attrName.Value;
                    if (itemvalue  == "FileSizeLimit")
                    {
                       // now get the "Value" of the item
                       XAttribute attrValue = applicationSettingItem.Attribute("Value");
                       string fileSizeLimitValue = string.Empty;
                       if (attrValue != null)
                       {
                           fileSizeLimitValue = attrValue.Value;
                       }

                       if (string.IsNullOrEmpty(fileSizeLimitValue))
                       {
                           // No limit was found in the ApplicationsSettings file so we will set a default of 100MB (100000000 Bytes)
                           result = 100000000;
                           return result;
                       }

                       result = Convert.ToInt32(fileSizeLimitValue);
                       return result;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
