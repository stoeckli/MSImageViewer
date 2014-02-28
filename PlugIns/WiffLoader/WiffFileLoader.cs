#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="WiffFileLoader.cs" company="Novartis Pharma AG.">
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
    using System.IO;
    using Novartis.Msi.Core;

    /// <summary>
    /// This class implements the <see cref="ISpecFileLoader"/> and
    /// facilitates the reading of a "Wiff-File's" content.
    /// </summary>
    public class WiffFileLoader : ISpecFileLoader
    {
        #region Fields

        /// <summary>
        /// Supported file types
        /// </summary>
        private readonly FileTypeDescriptorList supportedFileTypes;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WiffFileLoader"/> class
        /// </summary>
        public WiffFileLoader()
        {
            // Create file descriptor:
            this.supportedFileTypes = new FileTypeDescriptorList
                {
                    new FileTypeDescriptor(Strings.WiffFileDescription, ".wiff") 
                };
        }

        #endregion Constructors

        #region Properites
        
        /// <summary>
        /// Gets descpiptive name of the loader.
        /// </summary>
        public string Name
        {
            get { return "Wiff-File Loader"; }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Gets file types which this loader can process.
        /// </summary>
        public FileTypeDescriptorList SupportedFileTypes
        {
            get { return this.supportedFileTypes; }
        }
        
        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">A <see cref="string"/> containing the full qualified path to the wiff file.</param>
        /// <returns>A <see cref="ISpecFileContent"/> reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(string filePath)
        {
            var document = new Document
                { FileName = filePath, DocumentName = Path.GetFileNameWithoutExtension(filePath) };
            
            var wiffContent = new WiffFileContent(document, filePath);

            if (!wiffContent.LoadContent())
            {
                return null;
            }

            document.Content = wiffContent;
            return wiffContent;
        }

        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="inStream"/>
        /// </summary>
        /// <param name="inStream">A <see cref="Stream"/> objetc containing the wiff data.</param>
        /// <exception cref="NotImplementedException">Wiff Load Exception</exception>
        /// <returns>A <see cref="ISpecFileContent"/> reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(Stream inStream)
        {
            throw new NotImplementedException("WiffFileLoader Load(Stream inStream)");
        }

        #endregion
    }
}
