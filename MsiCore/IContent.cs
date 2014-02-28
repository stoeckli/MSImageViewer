#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IContent.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Interface to access content specific functionality.
    /// </summary>
    public interface IContent
    {
        #region Properties
        /// <summary>
        /// Gets the this content belongs to.
        /// </summary>
        Document Document { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Create the views of this content.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> object this content is associated with.</param>
        /// <returns>A <see cref="ViewCollection"/> object containing the views.</returns>
        ViewCollection GetContentViews(Document doc);

        #endregion Methods
    }
}
