#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="FileTypeDescriptor.cs" company="Novartis Pharma AG.">
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

using System.Text;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// This class describes file types and provides filters for the file handling dialogs.
    /// </summary>
    public class FileTypeDescriptor
    {
        #region Fields

        /// <summary>
        /// Description Field
        /// </summary>
        private readonly string description;

        /// <summary>
        /// List of extensions
        /// </summary>
        private readonly string[] extensions;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeDescriptor"/> class
        /// </summary>
        public FileTypeDescriptor()
        {
            this.description = string.Empty;
            this.extensions = new string[1];
            this.extensions[0] = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeDescriptor"/> class
        /// Constructor receiving a description and an extension.
        /// </summary>
        /// <param name="description">The file type's description.</param>
        /// <param name="extension">The file type's extension.<br/>All extensions
        /// should have a leading point (eg. ".bmp") but are not case-sensitive.</param>
        public FileTypeDescriptor(string description, string extension)
        {
            this.description = description;
            this.extensions = new string[1];
            this.extensions[0] = extension;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeDescriptor"/> class 
        /// Constructor receiving a description and the number of extensions
        /// being associated to the file type.
        /// </summary>
        /// <param name="description">The file type's description.</param>
        /// <param name="extensionCount">The number of extensions being associated
        /// to the file type.<br/>All extensions should have a leading point
        /// (eg. ".bmp") but are not case-sensitive.</param>
        public FileTypeDescriptor(string description, int extensionCount)
        {
            this.description = description;
            this.extensions = new string[extensionCount];

            for (int i = 0; i < extensionCount; ++i)
            {
                this.extensions[i] = string.Empty;
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the description of this file type.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Gets the default extension of this file type.
        /// </summary>
        /// <remarks>This implementation returns the first element of the
        /// extensions list as default extension.</remarks>
        public string DefaultExtension
        {
            get
            {
                return this.extensions[0];
            }
        }

        /// <summary>
        /// Gets valid extensions of this file type.
        /// </summary>
        /// <remarks>
        /// All extensions should have a leading point (eg. ".bmp") but are not case-sensitive.
        /// </remarks>
        public string[] Extensions
        {
            get
            {
                return this.extensions;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Retrieves whether this description includes the given extension.
        /// </summary>
        /// <param name="extension">The extension to be checked.</param>
        /// <returns>If the extension is supported <see langword="true"/>, otherwise <see langword="false"/>.</returns>
        public bool IncludesExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            for (int i = 0; i < this.extensions.Length; i++)
            {
                if (string.Compare(extension, this.extensions[i], true) == 0)
                {
                    return true;
                }

                // give "*.ext" a try...
                string allExtension = "*" + extension;
                if (string.Compare(allExtension, this.extensions[i], true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Composes a formated string to be used within a file dialog filter combo.
        /// </summary>
        /// <returns>
        /// A new string object.<br/>
        /// <example>"All supported formats (*.exe,*.dll)</example>
        /// </returns>
        public string ComposeFileDialogFilterText()
        {
            if (this.extensions.Length > 1)
            {
                var filter = new StringBuilder(this.description);
                filter.Append(" (*");
                for (int i = 0; i < this.extensions.Length; ++i)
                {
                    filter.Append(this.extensions[i]);
                    filter.Append(i == this.extensions.Length - 1 ? ")" : ", *");
                }

                return filter.ToString();
            }

            return this.description + " (*" + this.extensions[0] + ")";
        }

        /// <summary>
        /// Composes a string suitable for common file dialogs containing the
        /// description of the file type and its extensions.
        /// </summary>
        /// <returns>
        /// A new string object.<br/>
        /// <example>"Jpeg Files|*.jpg;*.jpeg"</example>
        /// </returns>
        public string GetFileDialogFilter()
        {
            if (this.extensions.Length > 1)
            {
                var sb = new StringBuilder(this.description);
                sb.Append("|*");

                for (int i = 0; i < this.extensions.Length; ++i)
                {
                    sb.Append(this.extensions[i]);
                    if (i != this.extensions.Length - 1)
                    {
                        sb.Append(";*");
                    }
                }

                return sb.ToString();
            }

            return this.description + "|*" + this.extensions[0];
        }

        #endregion Methods
    }
}
