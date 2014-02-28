#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="FileTypeDescriptorList.cs" company="Novartis Pharma AG.">
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
using System.Collections.ObjectModel;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// A list of <see cref="FileTypeDescriptor"/> objects.
    /// </summary>
    [Serializable]
    public class FileTypeDescriptorList : Collection<FileTypeDescriptor>
    {
        /// <summary>
        /// Check whether ths given extension is included in any of the contained
        /// <see cref="FileTypeDescriptor"/>s. 
        /// </summary>
        /// <param name="extension">The extension for which to look as a <see cref="string"/>.</param>
        /// <returns>A <see cref="bool"/> value signaling if the given <paramref name="extension"/> is included.</returns>
        public bool IncludesExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            foreach (FileTypeDescriptor fileType in this)
            {
                if (fileType.IncludesExtension(extension))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
