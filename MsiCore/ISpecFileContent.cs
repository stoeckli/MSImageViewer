#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ISpecFileContent.cs" company="Novartis Pharma AG.">
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
    /// Common interface for classes holding spectrometry file data.
    /// </summary>
    public interface ISpecFileContent : IContent
    {
        /// <summary>
        /// Retrieve the loaded content in a list of <see cref="BaseObject"/>s.
        /// </summary>
        /// <returns>The imaging data the object implementing this interface contains.</returns>
        BaseObjectList GetContent();
    }
}
