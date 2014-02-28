#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="XmlFilePlugInProvider.cs" company="Novartis Pharma AG.">
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
using System.IO;
using System.Xml;

namespace Novartis.Msi.PlugInSystem
{
  /// <summary>
  /// Loads plugins by reading a specific xml file. 
  /// The file should contain for each plugin
  /// an "add" XML item with a "type" attribute that specifies
  /// the full name of the class , a comma, 
  /// and the name of the encapsulating assembly.
  /// </summary>
  public class XmlFilePlugInProvider : IPlugInProvider
  {
    #region Fields

    /// <summary>
    /// PlugIn List
    /// </summary>
    private readonly PlugInDataList plugIns = new PlugInDataList();

    /// <summary>
    /// File Name
    /// </summary>
    private string fileName = string.Empty;

    #endregion Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlFilePlugInProvider"/> class.
    /// </summary>
    /// <param name="xmlFile">Xml File Name</param>
    public XmlFilePlugInProvider(string xmlFile)
    {
        if (!File.Exists(xmlFile))
        {
            throw new FileNotFoundException("Could not find file", xmlFile);
      }

      this.fileName = xmlFile;
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Gets or sets the filename of the XML file.
    /// </summary>
    public string FileName
    {
        get
        {
            return this.fileName;
        }

        set
        {
            this.fileName = value;
        }
    }

    /// <summary>
    /// Gets the collection of loaded PlugIns.
    /// </summary>
    public PlugInDataList LoadedPlugIns
    {
        get
        {
            return this.plugIns;
        }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Loads the PlugIns.
    /// </summary>
    /// <param name="alreadyLoadedPlugIns"> List of Plugins </param>
    public void LoadPlugIns(PlugInDataList alreadyLoadedPlugIns)
    {
      this.plugIns.Clear();
      var textReader = new XmlTextReader(this.FileName);

      while (textReader.Read())
      {
        if (textReader.Name == "add")
        {
          string type = textReader.GetAttribute("type");
          IPlugIn plugIn = null;

        if (type != null)
        {
            Type plugInType = Type.GetType(type);
            if (plugInType != null)
            {
                object plugInInstance = Activator.CreateInstance(plugInType);

                plugIn = plugInInstance as IPlugIn;
            }
        }

          if (plugIn != null)
          {
            PlugInData plugInData = new PlugInData(plugIn);
            bool alreadyLoaded = false;

            foreach (PlugInData loadedPlugInData in this.LoadedPlugIns)
            {
              if (loadedPlugInData.AssemblyFullName == plugInData.AssemblyFullName)
              {
                alreadyLoaded = true;
                break;
              }
            }

            if (!alreadyLoaded)
            {
              this.LoadedPlugIns.Add(plugInData);
            }
          }
        }
      }

      // create a plugIn list to collect invalid PlugIns
      var invalidPlugIns = new PlugInDataList();
      
      // try and get the baseobjects published by the PlugIn
      foreach (PlugInData plugInData in this.plugIns)
      {
        IPlugIn plugIn = plugInData.PlugIn;
        Type plugInType = plugIn.GetType();

        // JP 22/12/2011 - This does not do anything
        System.Reflection.Assembly plugInAssembly = plugInType.Assembly;
      }
      
      // Remove all invalid PlugIns from the PlugIns list
      foreach (PlugInData plugInData in invalidPlugIns)
      {
        this.plugIns.Remove(plugInData);
      }
    }

    #endregion Methods
  }
}
