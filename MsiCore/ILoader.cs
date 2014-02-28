#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ILoader.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.Core
{
    #region ILoader Interface

    /// <summary>
    /// Interface for all loaders.
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// Gets the Name of this loader.
        /// </summary>
        /// <remarks>
        /// Every loader should have a unique name.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Gets a List of supported file types.
        /// </summary>
        /// <remarks>
        /// Retrieves a list of <see cref="FileTypeDescriptor"/>s.
        /// </remarks>
        FileTypeDescriptorList SupportedFileTypes { get; }
    }

    #endregion ILoader Interface
}
