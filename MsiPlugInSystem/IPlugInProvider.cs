#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IPlugInProvider.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.PlugInSystem
{
  /// <summary>
  /// Interface for a plugin provider object
  /// Implement this if you need to search for plugins
  /// in you own custom way
  /// </summary>
  public interface IPlugInProvider
  {
    #region Properties

    /// <summary>
    /// Gets a collection of the plugins that were found
    /// </summary>
    PlugInDataList LoadedPlugIns
    {
      get;
    }

    #endregion Properties

    #region Methods

    /// <summary>
    ///   searches for the plugins
    /// </summary>
    /// <param name="alreadyLoadedPlugIns">Data List</param>
    void LoadPlugIns(PlugInDataList alreadyLoadedPlugIns);

    #endregion Methods
  }

  /// <summary>
  /// Custom-Exception which occured during the loading of an PlugIn.
  /// </summary>
  public class PlugInLoadException : Exception
  {
    #region Fields

    /// <summary>
    /// Plug in Name
    /// </summary>
    private readonly string plugInName = string.Empty;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInLoadException"/> class. 
    /// </summary>
    /// <param name="plugInName">
    /// A <see langword="string"/>-instance defining the name of the PlugIn which
    /// loading has failed.
    /// </param>
    /// <param name="inner">
    /// The <see cref="Exception"/> that has occured during loading of the PlugIn.
    /// </param>
    public PlugInLoadException(string plugInName, Exception inner) : base(plugInName, inner)
    {
      if (plugInName != null)
      {
        this.plugInName = plugInName;
      }
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the name of the PlugIn which loading has failed.
    /// </summary>
    public string PlugInName
    {
      get
      {
        return this.plugInName;
      }
    }

    #endregion Properties
  }
}
