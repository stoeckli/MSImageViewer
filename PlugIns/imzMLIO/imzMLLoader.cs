#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ImzMlLoader.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System;
using System.IO;

using Novartis.Msi.Core;

namespace Novartis.Msi.PlugIns.ImzMLIO
{
    /// <summary>
    /// This class implements the <see cref="ISpecFileLoader"/> and
    /// facilitates the reading of a "imzML-File's" content.
    /// </summary>
    public class ImzMlLoader : ISpecFileLoader
    {
        #region Private Fields

        /// <summary>
        /// Supported File Type
        /// </summary>
        private readonly FileTypeDescriptorList supportedFileTypes;

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImzMlLoader"/> class 
        /// </summary>
        public ImzMlLoader()
        {
            // Create file descriptor:
            this.supportedFileTypes = new FileTypeDescriptorList
                {
                    new FileTypeDescriptor(Strings.ImzMLFileDescription, Strings.ImzMLXmlFileExtension)
                };
        }

        #endregion Constructor

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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">A <see cref="string"/> containing the full qualified path to the wiff file.</param>
        /// <returns>A <see cref="ISpecFileContent"/> reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(string filePath)
        {
            return null;
        }

        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="inStream"/>
        /// </summary>
        /// <param name="inStream">A <see cref="Stream"/> objetc containing the wiff data.</param>
        /// <returns>A <see cref="ISpecFileContent"/> reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(Stream inStream)
        {
            throw new NotImplementedException("ImzMLLoader.Load(Stream inStream)");
        }
        
        #endregion Methods
    }
}
