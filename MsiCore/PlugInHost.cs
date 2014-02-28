#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="PlugInHost.cs" company="Novartis Pharma AG.">
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

using Novartis.Msi.PlugInSystem;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Implementing the IPlugInHost interface this class realizes the
    /// communication between the PlugIns and the core / application.
    /// </summary>
    public class PlugInHost : IPlugInHost
    {
        #region Static Fields

        /// <summary>
        /// PlugIn Data List
        /// </summary>
        private static readonly PlugInDataList PlugIns = new PlugInDataList();

        #endregion Static Fields

        #region Events

        /// <summary>
        /// An event PlugIns can use to be notified when the application is going to be
        /// closed. Registering to this event can be used to execute a PlugIns-cleanup-code
        /// before the application finally exits...
        /// </summary>
        public event ApplicationClosingEventHandler ApplicationClosing;

        #endregion Events

        #region Methods

        /// <summary>
        /// Finds and collects the PlugIns in the PlugIn-Path.
        /// </summary>
        public void CollectPlugIns()
        {
            // use this for a PlugInProvider which dynamically searches the given PlugIn-Directory for PlugIns...
            // IPlugInProvider PlugInProvider = new DynamicFindPlugInProvider(PlugInPath);
            // use this for a PlugInProvider which uses the 'plugins'-Section of the applications config file for the PlugIn finding...
            // IPlugInProvider PlugInProvider = new SectionHandlerPlugInProvider();
            // use this for a PlugInProvider which uses the PlugIn-XML-Config file in the PlugIns-Directory for the PlugIn finding...
            // string xmlFile = Strings.PlugInsXmlFile;
            // IPlugInProvider PlugInProvider = new XmlFilePlugInProvider(PlugInPath + @"\" + xmlFile);
            PlugIns.Clear();

            // Now let the 'dynamic find'-PlugInProvider try and load the other PlugIn-Assemblies from the 'PlugIns'-directory.
            // (NOT loading those assmblies already loaded by the previous PlugInProvider!)
            IPlugInProvider plugInProvider = new DynamicFindPlugInProvider(AppContext.AppPlugInsDir);
            plugInProvider.LoadPlugIns(PlugIns);
            PlugIns.AddRange(plugInProvider.LoadedPlugIns);
        }

        /// <summary>
        /// Loads the collected PlugIns by calling the <see cref="IPlugIn.Initialize"/> method.
        /// </summary>
        public void InitializePlugIns()
        {
            if (PlugIns != null)
            {
                foreach (PlugInData plugInData in PlugIns)
                {
                    IPlugIn plugIn = plugInData.PlugIn;
                    if (plugIn != null)
                    {
                        try
                        {
                            plugIn.Initialize(this);
                        }
                        catch (System.Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoke the ApplicationClosing event on the interested PlugIns, if any.
        /// </summary>
        /// <param name="e">
        /// A <see cref="System.ComponentModel.CancelEventArgs"/>-reference.
        /// </param>
        internal void FireApplicationClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.ApplicationClosing != null)
            {
                this.ApplicationClosing(e);
            }
        }

        #endregion Methods
    }
}
