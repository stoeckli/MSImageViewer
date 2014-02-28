#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="SpecFileLoaderList.cs" company="Novartis Pharma AG.">
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

using System.Collections.ObjectModel;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// A list of <see cref="ISpecFileLoader"/> implementing objects.
    /// </summary>
    public class SpecFileLoaderList : Collection<ISpecFileLoader>
    {
        /// <summary>
        /// Finds in this list the first instance supporting the given <paramref name="extension"/>.
        /// </summary>
        /// <param name="extension">A <see cref="string"/> object specifying the extension for which
        /// a loader instance is searched.</param>
        /// <returns>A <see cref="ISpecFileLoader"/> reference to the object found or null,
        /// if no such object is found.</returns>
        public ISpecFileLoader FindSupportingLoader(string extension)
        {
            if (!string.IsNullOrEmpty(extension))
            {
                foreach (ISpecFileLoader loader in this)
                {
                    if (loader != null)
                    {
                        foreach (FileTypeDescriptor fileType in loader.SupportedFileTypes)
                        {
                            if (fileType.IncludesExtension(extension))
                            {
                                return loader;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
