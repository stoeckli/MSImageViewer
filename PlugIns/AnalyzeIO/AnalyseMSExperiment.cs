#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyseMsExperiment.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel 13/02/2012 - Analyse Loader Updates
// Updated: Markus Stoeckli & Jayesh Patel 08/03/2012 - Making import more effcient & full Binning
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.AnalyzeIO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Linq;

    using Novartis.Msi.Core;

    /// <summary>
    /// This implements the AnalyseMsExperiment class
    /// </summary>
    public class AnalyseMsExperiment : AnalyseExperiment
    {
        #region Fields

        /// <summary>
        /// Used to comparing floats 
        /// </summary>
        private const float Epsilon = (float)1E-10;

        /// <summary>
        /// Path of Input file
        /// </summary>
        private string filePath;

        /// <summary>
        /// AnalyseFile Header Object 
        /// </summary>
        private AnalyseFileHeaderObject analyseFileHeaderObject;

        /// <summary>
        /// List of floats
        /// </summary>
        private List<float[][]> massspecdatalist;

        /// <summary>
        /// Gets or sets array of the Mass range
        /// </summary>
        private float[] masssteps;

        /// <summary>
        /// Min Mass in Range
        /// </summary>
        private float minmass;

        /// <summary>
        /// Max Mass in Range
        /// </summary>
        private float maxmass;

        /// <summary>
        /// MassStep Size
        /// </summary>
        private float massstepsize;
        
        /// <summary>
        /// Number of points in spectrum range
        /// </summary>
        private long massspecdatapoints;

        /// <summary>
        /// Mean Value
        /// </summary>
        private float meanValue;

        /// <summary>
        /// Median Value
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

        /// <summary>
        /// Mass range name
        /// </summary>
        private string massRangeName;

        /// <summary>
        /// Bin Size
        /// </summary>
        private int binsize;

        /// <summary>
        /// Mass Calibration
        /// </summary>
        private float[] masscal;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyseMsExperiment"/> class.
        /// </summary>
        /// <param name="analysefilecontent">analyseFileContent Object</param>
        /// <param name="filePath">Path of file that is being read</param>
        /// <param name="analyseFileHeaderObject">Header Object</param>
        public AnalyseMsExperiment(
            AnalyseFileContent analysefilecontent, string filePath, AnalyseFileHeaderObject analyseFileHeaderObject)
            : base(analysefilecontent, filePath, analyseFileHeaderObject)
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the Mass Calibration array
        /// </summary>
        public float[] MassCal
        {
            get
            {
                return this.masscal;
            }
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Reads the Data file
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
                string header = "Missing File";
                string error = string.Format("{0} is missing", this.filePath);
                MessageBox.Show(error, header);
                return;
            }

            // 2) ReadCalibrationFile (Calc MinMass, MaxMass, StepSize and MassSpecDataPoints)
            this.ReadCalibrationFile();

            // 3) Set Binsize
            this.binsize = 1;

            var fileinfo = new FileInfo(this.filePath);
            long imgFileSize = fileinfo.Length;

            string appPath = Application.StartupPath;
            appPath += "\\ApplicationSettings.xml";

            var filesizelimit = this.GetPathFileSizeLimit(appPath);
            var minMassDp = 0;
            var maxMassDp = (int)(this.massspecdatapoints - 1);

            // We only want to do binning for files bigger than 100MB filesize > pathfilesize 
            if (imgFileSize != filesizelimit)
            {
                // Open up BinNumberWindow
                var binNumberWindow = new BinNumberWindow(this.analyseFileHeaderObject.NumberOfXPoints, this.analyseFileHeaderObject.NumberOfYPoints, this.MassCal);
                if (binNumberWindow.ShowDialog() == true)
                {
                    this.binsize = binNumberWindow.BinSize;
                    minMassDp = binNumberWindow.MinMassDP;
                    maxMassDp = binNumberWindow.MaxMassDP;
                }
            }

            this.massstepsize = 1;

            // 4) Fill out meta rawdata
            var metaData = new ImageMetaData();

            try
            {
                metaData.Add("Sample Name", typeof(string), this.analyseFileHeaderObject.Name, false);
                metaData.Add("Mass Range", typeof(string), this.massRangeName, false);
                metaData.Add("Mass Step Size", typeof(string), 1, false);
                metaData.Add("X1 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.X1 - (int)this.analyseFileHeaderObject.X1) < Epsilon) ? this.analyseFileHeaderObject.X1.ToString("0.0") : this.analyseFileHeaderObject.X1.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Y1 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.Y1 - (int)this.analyseFileHeaderObject.Y1) < Epsilon) ? this.analyseFileHeaderObject.Y1.ToString("0.0") : this.analyseFileHeaderObject.Y1.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("X2 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.X2 - (int)this.analyseFileHeaderObject.X2) < Epsilon) ? this.analyseFileHeaderObject.X2.ToString("0.0") : this.analyseFileHeaderObject.X2.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Y2 (mm)", typeof(string), (Math.Abs(this.analyseFileHeaderObject.Y2 - (int)this.analyseFileHeaderObject.Y2) < Epsilon) ? this.analyseFileHeaderObject.Y2.ToString("0.0") : this.analyseFileHeaderObject.Y2.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Data Points in X", typeof(string), this.analyseFileHeaderObject.NumberOfXPoints.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Data Points in Y", typeof(string), this.analyseFileHeaderObject.NumberOfYPoints.ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Point Width (mm)", typeof(string), Math.Round(this.analyseFileHeaderObject.Dx, 2).ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Point Height (mm)", typeof(string), Math.Round(this.analyseFileHeaderObject.Dy, 2).ToString(CultureInfo.InvariantCulture), false);
                metaData.Add("Bin Size", typeof(string), this.binsize.ToString(CultureInfo.InvariantCulture), false);
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }

            // 5) Declare Median List
            var valuesForMedian = new List<float>();
            this.minIntensityValue = 0;
            this.maxIntensityValue = 0;

            var minIntList = new float[maxMassDp - minMassDp + 1];
            var maxIntList = new float[maxMassDp - minMassDp + 1];
            
            // 6) Declare and Initialise the dataTic
            var dataTic = new float[analysefileheaderobject.NumberOfXPoints][];
            for (int i = 0; i < dataTic.Length; i++)
            {
                dataTic[i] = new float[analysefileheaderobject.NumberOfYPoints];
            }

            // 7) Declare mass spectrum datat list
            this.massspecdatalist = new List<float[][]>();

            // 8) Declare and Initialise the dataList
            var data = new float[analysefileheaderobject.NumberOfXPoints][];
            for (int pointOnXAxis = 0; pointOnXAxis < analysefileheaderobject.NumberOfXPoints; pointOnXAxis++)
            {
                data[pointOnXAxis] = new float[analysefileheaderobject.NumberOfYPoints];
            }

            this.massspecdatalist.Add(data);

            // 9) Declare and populate the image data list - Fill it with some default data - this will be over written later
            Cursor.Current = Cursors.WaitCursor;
            AppContext.ProgressStart("Generating Empty Imagelist...");
            var imageDataList = new List<ImageData>();
            try
            {
                for (int massSpecDataPt = 0; massSpecDataPt < ((maxMassDp - minMassDp + 1) / this.binsize); massSpecDataPt++)
                {
                    var specData = new float[analysefileheaderobject.NumberOfXPoints][];
                    for (int i = 0; i < specData.Length; i++)
                    {
                        specData[i] = new float[analysefileheaderobject.NumberOfYPoints];
                    }

                    // ok, now create the imageData and add to list...
                    var imageData = new ImageData(
                        analysefilecontent.Document,
                        specData,
                        this.analyseFileHeaderObject.Name,
                        metaData,
                        0,
                        10000,
                        1000,
                        1000,
                        this.analyseFileHeaderObject.Dx,
                        this.analyseFileHeaderObject.Dy,
                        this.masscal,
                        this.analyseFileHeaderObject.ExperimentType);

                    // Add the imageData to the list
                    imageDataList.Add(imageData);
                    AppContext.ProgressSetValue(100.0 * massSpecDataPt / ((maxMassDp - minMassDp + 1) / this.binsize));
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
            finally
            {
                AppContext.ProgressClear();
                Cursor.Current = Cursors.Default;
            }

            // 10) Read binary file 
            Cursor.Current = Cursors.WaitCursor;
            AppContext.ProgressStart("Populating ImageList...");

            using (var imgReader = new BinaryReader(File.Open(this.filePath, FileMode.Open)))
            {
                int xp = analysefileheaderobject.NumberOfXPoints;
                int yp = analysefileheaderobject.NumberOfYPoints;
                int dp = (maxMassDp - minMassDp + 1) / this.binsize;
                int dd = this.binsize;
                int dl = minMassDp;
                int xf = analysefileheaderobject.NumberOfXPoints;
                int df = this.masscal.Length;
                int ps = this.analyseFileHeaderObject.DataByteSize;
                int y;
                var meanSumList = new double[dp];

                for (int i = 0; i < dp; i++)
                {
                    minIntList[i] = float.MaxValue;
                    maxIntList[i] = float.MinValue;
                    meanSumList[i] = 0.0;
                }

                for (y = 0; y < yp; y++)
                {
                    // for the progress bar
                    AppContext.ProgressSetValue(y * 100 / yp);

                    // refresh counter here;
                    for (int x = 0; x < xp; x++)
                    {
                        long fp = ((((y * xf) + x) * df) + dl) * ps;
                        imgReader.BaseStream.Position = fp;

                        for (int d = 0; d < dp; d++)
                        {
                            float average = 0;
                            
                            for (int di = 0; di < dd; di++)
                            {
                                float intensity;
                                switch (this.analyseFileHeaderObject.DataType)
                                {
                                    case 4:
                                        intensity = imgReader.ReadInt16();
                                        break;
                                    case 8:
                                    case 16:
                                        intensity = imgReader.ReadInt32();
                                        break;
                                    case 64:
                                        intensity = imgReader.ReadInt64();
                                        break;
                                    default:
                                        intensity = imgReader.ReadInt32();
                                        break;
                                }

                                average += intensity;
                            }

                            average = average / dd;
                            imageDataList[d].Data[x][y] = average;
                            dataTic[x][y] += average;
                            if (average < minIntList[d])
                            {
                                minIntList[d] = average;
                            }

                            if (average > maxIntList[d])
                            {
                                maxIntList[d] = average;
                            }

                            // mean and median calculation... sum up the value to calculate the mean
                            meanSumList[d] += average;
                        }
                    }
                }

                // and cleanup
                imgReader.Close();
            }

            AppContext.ProgressClear();
            Cursor.Current = Cursors.Default;

            // 11) create new masscal array
            var newmasscal = new float[(maxMassDp - minMassDp + 1) / this.binsize];

            for (int i = 0; i < ((maxMassDp - minMassDp + 1) / this.binsize); i++)
            {
                newmasscal[i] = this.masscal[minMassDp + (i * this.binsize)];
            }

            this.masscal = newmasscal;

            // 12) Mean/Median for Tic
            double meanSum = 0.0;

            int numDataPoint = dataTic.Length;

            for (int y = 0; y < analysefileheaderobject.NumberOfYPoints; y++)
            {
                for (int x = 0; x < analysefileheaderobject.NumberOfXPoints; x++)
                {
                    float intensity = dataTic[x][y];

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

            if (numDataPoint != 0)
            {
                this.meanValue = (float)(meanSum / numDataPoint);
            }

            valuesForMedian.Sort();
            this.medianValue = ((valuesForMedian.Count % 2) == 0)
                                   ? (valuesForMedian[(valuesForMedian.Count / 2) - 1]
                                   + valuesForMedian[valuesForMedian.Count / 2]) / 2.0f
                                   : valuesForMedian[valuesForMedian.Count / 2];

            // 13) Fill out imageTic
            var imageTic = new ImageData(
                analysefilecontent.Document,
                dataTic,
                this.analyseFileHeaderObject.Name,
                metaData,
                this.minIntensityValue,
                this.maxIntensityValue,
                this.meanValue,
                this.medianValue,
                this.analyseFileHeaderObject.Dx,
                this.analyseFileHeaderObject.Dy,
                this.masscal,
                this.analyseFileHeaderObject.ExperimentType);

            // 14) Fill out ImageSpectrumData and add to analysefilecontent
            analysefilecontent.Add(
                new ImageSpectrumData(
                    analysefilecontent.Document,
                    this.analyseFileHeaderObject.Name,
                    metaData,
                    this.masscal,
                    imageDataList,
                    this.analyseFileHeaderObject.ExperimentType)
                {
                    ImageTic = imageTic
                });
        }

        /// <summary>
        /// Reads the calibration file for MS Experiments - used for Mass Range info
        /// </summary>
        private void ReadCalibrationFile()
        {
            // 1) Declare and Open up the t2mReader
            using (var calibt2MReader = new BinaryReader(File.Open(this.filePath.Replace(".img", ".t2m"), FileMode.Open)))
            {
                // 2 Position and length variables.
                int pos = 0;
                int arraypos = 0;

                // 3 Use BaseStream.
                var length = (int)calibt2MReader.BaseStream.Length;

                int numberofpoints = length / sizeof(float);
                this.masssteps = new float[numberofpoints];

                while (pos < length)
                {
                    // 4) Read intensity
                    float massstep = calibt2MReader.ReadSingle();
                    this.masssteps[arraypos] = massstep;

                    // 5) Advance our position variable.
                    pos += sizeof(float);
                    arraypos++;
                }

                // and cleanup
                calibt2MReader.Close();
            }

            // A) First element gives the MinMass
            this.minmass = this.masssteps[0];

            // B) Diff between first and second element gives Mass Step
            this.massstepsize = this.masssteps[1] - this.masssteps[0];

            // C) Last element Max Mass
            this.maxmass = this.masssteps[this.masssteps.Length - 1];

            // D) Define the Mass Spec Data Points
            this.massspecdatapoints = this.masssteps.Length - 1;                

            // E) Here were are defining the masscal array with even points
            this.masscal = new float[this.massspecdatapoints];

            for (int i = 0; i < this.massspecdatapoints; i++)
            {
                this.masscal[i] = this.minmass + (i * this.massstepsize);
            }

            // F) Mass range name
            this.massRangeName = "TIC " + this.minmass.ToString(CultureInfo.InvariantCulture) + " - " + this.maxmass.ToString(CultureInfo.InvariantCulture);
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
                    if (itemvalue == "FileSizeLimit")
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
        
        #endregion Methods
    }
}