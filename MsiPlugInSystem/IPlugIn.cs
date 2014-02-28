#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IPlugIn.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.PlugInSystem
{
    #region Public Interfaces

    /// <summary>
    /// The IPlugIn interface. This interface is used to implement plugins.
    /// </summary>
    public interface IPlugIn
    {
        /// <summary>
        /// Gets Plugin Host
        /// </summary>
        IPlugInHost Host { get; }

        /// <summary>
        ///  Gets the name                   
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets Description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets Author
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets Version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Initialse method
        /// </summary>
        /// <param name="host">PlugIn Host</param>
        void Initialize(IPlugInHost host);

        /// <summary>
        /// Dispose method
        /// </summary>
        void Dispose();
    }

    #endregion Public Interfaces
}
