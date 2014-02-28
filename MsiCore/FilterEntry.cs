#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="FilterEntry.cs" company="Novartis Pharma AG.">
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
namespace Novartis.Msi.Core
{
    /// <summary>
    /// A entry in the collections of filter of the common dialog.
    /// </summary>
    public class FilterEntry
    {
        #region Fields

        /// <summary>
        /// File Type
        /// </summary>
        private readonly string fileType;

        /// <summary>
        /// File Extension
        /// </summary>
        private readonly string extension;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEntry"/> class.
        /// Constructor. Instantiates a filter object and initializes the instance with the
        /// given parameters.
        /// </summary>
        /// <param name="fileType">Descriptive string specifying the filetyp of the filter.</param>
        /// <param name="extension">The file extension this filter applies to. </param>
        public FilterEntry(string fileType, string extension)
        {
            this.fileType = fileType;
            this.extension = extension;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the displayed string describing the filetyp this filter belongs to.
        /// </summary>
        public string FileType
        {
            get
            {
                return this.fileType;
            }
        }
        
        /// <summary>
        /// Gets the file extension this filter belongs to.
        /// </summary>
        public string Extension
        {
            get
            {
                return this.extension;
            }
        }

        #endregion Properties
    }
}