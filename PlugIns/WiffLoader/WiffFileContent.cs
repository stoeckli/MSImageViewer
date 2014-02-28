#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="WiffFileContent.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 28/11/2011 - Implementation of new dot net Assemblies
//                                  - Removed hack for network files
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.WiffLoader
{
    using System;
    using System.IO;
    using System.Text;
    using Clearcore2.Data.AnalystDataProvider;
    using Clearcore2.Data.DataAccess.SampleData;
    using Novartis.Msi.Core;

    /// <summary>
    /// This class encapsulates the content read from a "Wiff-File"
    /// </summary>
    public class WiffFileContent : ISpecFileContent
    {
        #region Fields

        /// <summary>
        /// The <see cref="Document"/> this wiff content belongs to.
        /// </summary>
        private readonly Document document;

        /// <summary>
        /// The fully qualified filename
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// The list of samples contained in this wiff file
        /// </summary>
        private readonly WiffSampleList samples = new WiffSampleList();

        /// <summary>
        /// The list of image Data objects and spectrum imaging data objects
        /// </summary>
        private readonly BaseObjectList contentList = new BaseObjectList();

        /// <summary>
        /// String that contains the Maldi Parameters
        /// </summary>
        private string strmaldiparameters;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WiffFileContent"/> class
        /// </summary>
        /// <param name="document">The <see cref="Document"/> this wiff content belongs to.</param>
        /// <param name="filePath">The wiff file whose content is to be loaded.</param>
        public WiffFileContent(Document document, string filePath)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (filePath == string.Empty)
            {
                throw new ArgumentException("filePath is empty");
            }

            if (!File.Exists(filePath))
            {
                string message = "file doesn't exists: " + filePath;
                throw new ArgumentException(message);
            }

            this.fileName = filePath;
            this.document = document;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Document this wiff content belongs to.
        /// </summary>
        public Document Document
        {
            get
            {
                return this.document;
            }
        }

        /// <summary>
        /// Gets the path (full qualified name) of the file associated with this object
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load the wiff file's content.
        /// </summary>
        /// <returns>A <see cref="bool"/> value indicating the success of the loading.</returns>
        public bool LoadContent()
        {
            bool result;
            var dataProvider = new AnalystWiffDataProvider();

            // We're going to extract the Maldi parameters at this point.
            this.strmaldiparameters = this.GetMaldiParameters(this.fileName);

            try
            {
                // get wiff file object from given file
                Batch wiffFile = AnalystDataProviderFactory.CreateBatch(this.fileName, dataProvider);

                // get number of samples
                string[] sampleNames = wiffFile.GetSampleNames();
                int numOfSamples = sampleNames.GetLength(0);

                // create sample objects for all the samples present in the wiff file
                for (int actSample = 0; actSample < numOfSamples; actSample++)
                {
                    // create and add the sample to this objects list of samples...
                    var sample = new WiffSample(wiffFile, this, actSample, this.strmaldiparameters);
                    this.samples.Add(sample);
                }

                result = true;
            }
            catch (Exception e)
            {
                Util.ReportException(e);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Add the image describing object to the list of imageDataSets
        /// </summary>
        /// <param name="imageData">The imaging object.</param>
        public void Add(Imaging imageData)
        {
            if (imageData == null)
            {
                throw new ArgumentNullException("imageData");
            }

            this.contentList.Add(imageData);
        }

        /// <summary>
        /// Create the views of this content.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> object this content is associated with.</param>
        /// <returns>A <see cref="ViewCollection"/> object containing the views.</returns>
        public ViewCollection GetContentViews(Document doc)
        {
            var views = new ViewCollection();
            
            foreach (Imaging dataSet in this.contentList)
            {
                var view = new ViewImage(doc, dataSet, dataSet.Name, dataSet.ExperimentType);
                views.Add(view);
            }

            return views;
        }

        /// <summary>
        /// Retrieve the loaded content in a list of <see cref="BaseObject"/>s.
        /// </summary>
        /// <returns>The imaging data this object contains.</returns>
        public BaseObjectList GetContent()
        {
            return this.contentList;
        }

        /// <summary>
        /// This routine reads the contents of the Wiff file and extracts the Maldi parameters.
        /// This routine is a bit of a "hack" because the current Ab Sciex DLL's do not support the Maldi parametershttp://www.skyscanner.net/flights-to/bsl/cheap-flights-to-basel-mulhouse-freiburg-airport.html
        /// This routine there is only a single experiment in the WIFF file. If there are more, the the ones from the 
        /// first experiment set the values of the Maldi parameters
        /// </summary>
        /// <param name="filePath">File Path</param>
        /// <returns>string containing Maldi Parameters</returns>
        public string GetMaldiParameters(string filePath)
        {
            // open wiff file and retrieve parameters 
            AppContext.ProgressStart("reading maldi parameters");
            string strMaldiParams = string.Empty;
            
            try
            {
                using (var paramStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var paramReader = new StreamReader(paramStream, Encoding.Unicode))
                    {
                        string paramString = paramReader.ReadToEnd();

                        string paramLaserFrequency = "Laser Frequency\t"
                                                     +
                                                     paramString.Substring(paramString.IndexOf("Laser Frequency"), 100).
                                                         Split(new[] { "\t" }, 3, StringSplitOptions.None)[1]
                                                     + "\tHz\r\n";

                        string paramLaserPower = "Laser Power\t"
                                                 +
                                                 paramString.Substring(paramString.IndexOf("Laser Power"), 100).Split(
                                                     new[] { "\t" }, 3, StringSplitOptions.None)[1] + "\t%\r\n";

                        string paramAblationMode = "Ablation Mode\t"
                                                   +
                                                   paramString.Substring(paramString.IndexOf("Ablation Mode"), 100).
                                                       Split(new[] { "\t" }, 3, StringSplitOptions.None)[1] + "\t\r\n";

                        string paramSkimmerVoltage = "Skimmer Voltage\t"
                                                     +
                                                     paramString.Substring(paramString.IndexOf("Skimmer Voltage"), 100).
                                                         Split(new[] { "\t" }, 3, StringSplitOptions.None)[1]
                                                     + "\tV\r\n";

                        string paramSourceGas = "Source Gas\t"
                                                +
                                                paramString.Substring(paramString.IndexOf("Source Gas"), 100).Split(
                                                    new[] { "\t" }, 3, StringSplitOptions.None)[1] + "\t\r\n";

                        string paramRasterSpeed = "Raster Speed\t"
                                                  +
                                                  paramString.Substring(paramString.IndexOf("Raster Speed"), 100).Split(
                                                      new[] { "\t" }, 3, StringSplitOptions.None)[1] + "\t\r\n";

                        string paramDirection = "Manual Continuous Direction\t"
                                                +
                                                paramString.Substring(
                                                    paramString.IndexOf("Manual Continuous Direction"), 100).Split(
                                                        new[] { "\t" }, 3, StringSplitOptions.None)[1] + "\t\r\n";

                        string paramRasterPitch = "Raster Pitch\t"
                                                  +
                                                  paramString.Substring(paramString.IndexOf("Raster Pitch"), 100).Split(
                                                      new[] { "\t" }, 3, StringSplitOptions.None)[1] + "\t\r\n";

                        strMaldiParams = paramLaserFrequency + paramLaserPower + paramAblationMode + paramSkimmerVoltage
                                         + paramSourceGas + paramRasterSpeed + paramDirection + paramRasterPitch;

                        paramStream.Flush();
                        paramReader.Dispose();
                        paramStream.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
            finally
            {
                AppContext.ProgressClear();
                GC.Collect();
            }

            return strMaldiParams;
        }

        #endregion
    }
}