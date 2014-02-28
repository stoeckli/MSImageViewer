#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IPlugInHost.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.PlugInSystem
{
    #region Public Delegates

    /// <summary>
    /// A delegate type for hooking up application closing notifications.
    /// </summary>
    /// <param name="e">Event Args</param>
    public delegate void ApplicationClosingEventHandler(System.ComponentModel.CancelEventArgs e);

    #endregion Public Delegates

    #region Public Interfaces

    /// <summary>
    /// The IPlugInHost interface. This interface is used to implement a plugin hosting system
    /// </summary>
    public interface IPlugInHost
    {
        /// <summary>
        /// An event PlugIns can use to be notified when the application is going to be
        /// closed. Registering to this event can be used to execute a PlugIns-cleanup-code
        /// before the application finally exits...
        /// </summary>
        event ApplicationClosingEventHandler ApplicationClosing;
    }

    #endregion Public Interfaces
}
