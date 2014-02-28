#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="SectionHandlerPlugInProvider.cs" company="Novartis Pharma AG.">
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
using System.Configuration;
using System.Xml;

namespace Novartis.Msi.PlugInSystem
{
  /// <summary>
  /// This class implements IConfigurationSectionHandler and allows
  /// us to parse the "plugin" XML nodes found inside App.Config
  /// and return a PlugInCollection object.
  /// </summary>
  public class SectionHandlerPlugInProvider : IConfigurationSectionHandler, IPlugInProvider
  {
    #region Fields

    /// <summary>
    /// List of Plugins
    /// </summary>
    private PlugInDataList plugIns;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the collection of loaded plugins.
    /// </summary>
    public PlugInDataList LoadedPlugIns
    {
        get
        {
            return this.plugIns;
        }
    }

    #endregion Properties

    #region IConfigurationSectionHandler Members

    /// <summary>
    /// Iterate through all the child nodes
    ///  of the XMLNode that was passed in and create instances
    ///  of the specified Types by reading the attribute values of the nodes
    ///  we use a try/Catch here because some of the nodes
    ///  might contain an invalid reference to a plugin type.
    /// </summary>
    /// <param name="parent">Parent Object</param>
    /// <param name="configContext">Context configuration</param>
    /// <param name="section">The XML section we will iterate against</param>
    /// <returns>List of Plugins</returns>
    public object Create(object parent, object configContext, XmlNode section)
    {
      var plugins = new PlugInDataList();

        foreach (XmlNode node in section.ChildNodes)
        {
            // Use the Activator class's 'CreateInstance' method
            // to try and create an instance of the plugin by
            // passing in the type name specified in the attribute value
            if (node.Attributes != null && node.Attributes["type"] != null)
            {
                string typename = node.Attributes["type"].Value;
                if (typename != null)
                {
                    Type type = Type.GetType(typename);
                    if (type != null)
                    {
                        object plugObject = Activator.CreateInstance(type);

                    // Cast this to an IPlugIn interface and add to the collection
                    var plugin = plugObject as IPlugIn;
                    if (plugin != null)
                    {
                        plugins.Add(new PlugInData(plugin));
                    }
                    }
                }
            }
        }

      return plugins;
    }

    #endregion

    #region Methods
    /// <summary>
    /// Loads the plugins.
    /// </summary>
    /// <param name="alreadyLoadedPlugIns"> The already Loaded Plug Ins. </param>
    public void LoadPlugIns(PlugInDataList alreadyLoadedPlugIns)
    {
        if (this.plugIns == null)
        {
            try
            {
              this.plugIns = ConfigurationManager.GetSection("plugins") as PlugInDataList;
            }
            finally
            {
                if (this.plugIns == null)
                {
                    this.plugIns = new PlugInDataList();
                }
            }
        }
    }

    #endregion Mehtods
  }
}
