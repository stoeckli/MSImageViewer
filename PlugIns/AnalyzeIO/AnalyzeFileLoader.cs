#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyzeFileLoader.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.AnalyzeIO
{
    using System;
    using System.IO;
    using Novartis.Msi.Core;

    /// <summary>
    /// This class implements the <see cref="ISpecFileLoader"/> and
    /// facilitates the reading of a "Analyze-File's" content.
    /// </summary>
    public class AnalyzeFileLoader : ISpecFileLoader
    {
        #region Fields

        /// <summary>
        /// Supported file types 
        /// </summary>
        private readonly FileTypeDescriptorList supportedFileTypes;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeFileLoader"/> class 
        /// </summary>
        public AnalyzeFileLoader()
        {
            // Create file descriptor:
            this.supportedFileTypes = new FileTypeDescriptorList
                {
                    new FileTypeDescriptor(Strings.AnalyzeFileDescription, Strings.AnalyzeFileExtension)
                };
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the descpiptive name of the loader.
        /// </summary>
        public string Name
        {
            get
            {
                return Strings.LoaderName;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets File types which this loader can process.
        /// </summary>
        public FileTypeDescriptorList SupportedFileTypes
        {
            get
            {
                return this.supportedFileTypes;
            }
        }
        
        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">containing the full qualified path to the wiff file.</param>
        /// <returns>reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(string filePath)
        {
            var document = new Document { FileName = filePath, DocumentName = Path.GetFileNameWithoutExtension(filePath) };

            var analyseFileContent = new AnalyseFileContent(document, filePath);

            if (!analyseFileContent.LoadContent())
            {
                return null;
            }

            document.Content = analyseFileContent;
            
            return analyseFileContent;
        }

        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="inStream"/>
        /// </summary>
        /// <param name="inStream">A <see cref="Stream"/> objetc containing the wiff data.</param>
        /// <exception cref="NotImplementedException">Exception Thrown</exception>
        /// <returns>A <see cref="ISpecFileContent"/> reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(Stream inStream)
        {
            throw new NotImplementedException("AnalyzeFileLoader.Load(Stream inStream)");
        }

        #endregion Methods
    }
}