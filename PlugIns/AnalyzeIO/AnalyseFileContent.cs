#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyseFileContent.cs" company="Novartis Pharma AG.">
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
    using Novartis.Msi.Core;
 
    /// <summary>
    /// This class encapsulates the content read from a "Analyse-File"
    /// </summary>
    public class AnalyseFileContent : ISpecFileContent
    {
        #region Fields

        /// <summary>
        /// The <see cref="Document"/> this wiff content belongs to.
        /// </summary>
        private readonly Document document;

        /// <summary>
        /// The fully qualified filepath
        /// </summary>
        private readonly string filepath;

        /// <summary>
        /// The list of image Data objects and spectrum imaging data objects
        /// </summary>
        private readonly BaseObjectList contentList = new BaseObjectList();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyseFileContent"/> class
        /// </summary>
        /// <param name="document">The <see cref="Document"/> this wiff content belongs to.</param>
        /// <param name="filePath">The wiff file whose content is to be loaded.</param>
        public AnalyseFileContent(Document document, string filePath)
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

            this.filepath = filePath;
            this.document = document;
        }

        #endregion Constructor

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
        public string FilePath
        {
            get
            {
                return this.filepath;
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

            try
            {
                new AnalyseExperiments(this, this.filepath);
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

        #endregion Methods
    }
}
