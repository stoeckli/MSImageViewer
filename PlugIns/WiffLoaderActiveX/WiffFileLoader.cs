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
using System.IO;

using Novartis.Msi.Core;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class implements the <see cref="ISpecFileLoader"/> and
    /// facilitates the reading of a "Wiff-File's" content.
    /// </summary>
    public class WiffFileLoader : ISpecFileLoader
    {
        #region Private Fields

        // Supported file types.
        private readonly FileTypeDescriptorList supportedFileTypes = null;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default-Constructor.
        /// </summary>
        public WiffFileLoader()
        {
            // Create file descriptor:
            supportedFileTypes = new FileTypeDescriptorList();
            supportedFileTypes.Add(new FileTypeDescriptor(Strings.WiffFileDescription, ".wiff"));
        }

        #endregion Constructors

        /// <summary>
        /// Descpiptive name of the loader.
        /// </summary>
        public string Name
        {
            get { return "Wiff-File Loader"; }
        }

        /// <summary>
        /// File types which this loader can process.
        /// </summary>
        public FileTypeDescriptorList SupportedFileTypes
        {
            get { return supportedFileTypes; }
        }
        
        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">A <see cref="string"/> containing the full qualified path to the wiff file.</param>
        /// <returns>A <see cref="ISpecFileContent"/> reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(string filePath)
        {
            Document document = new Document();
            document.FileName = filePath;
            document.DocumentName = Path.GetFileNameWithoutExtension(filePath);
            WiffFileContent wiffContent = new WiffFileContent(document, filePath);

            if (!wiffContent.LoadContent())
            {
                document = null;
                return null;
            }

            document.Content = wiffContent;
            return wiffContent;

        }

        /// <summary>
        /// Load the contents of the wiff file specified by <paramref name="inStream"/>
        /// </summary>
        /// <param name="inStream">A <see cref="Stream"/> objetc containing the wiff data.</param>
        /// <returns>A <see cref="ISpecFileContent"/> reference to a object containing the loaded data.</returns>
        public ISpecFileContent Load(Stream inStream)
        {
            throw new NotImplementedException("WiffFileLoader Load(Stream inStream)");
        }

    }
}
