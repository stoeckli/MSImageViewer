#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ISpecFileLoader.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
//
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System.IO;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Defines the common interface for all spectrometry file loading classes.
    /// </summary>
    public interface ISpecFileLoader : ILoader
    {
        #region Methods

        /// <summary>
        /// Loads the content of a spectrometry file specified by filename.
        /// </summary>
        /// <param name="filePath">The files path as a <see cref="string"/>.</param>
        /// <returns>The content of the spectrometry file represented by a <see cref="ISpecFileContent"/> implementation.</returns>
        ISpecFileContent Load(string filePath);

        /// <summary>
        /// Loads the content of a spectrometry file provided as a stream.
        /// </summary>
        /// <param name="inStream">A <see cref="Stream"/> to the files content.</param>
        /// <returns>ISpecFileContent Object</returns>
        ISpecFileContent Load(Stream inStream);

        #endregion Methods
    }
}
