#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="PlugInData.cs" company="Novartis Pharma AG.">
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
using System.Reflection;

namespace Novartis.Msi.PlugInSystem
{
  /// <summary>
  /// This class encapsulates the information of a PlugIn collected during
  /// the search for PlugIns.
  /// </summary>
  [Serializable]
  public class PlugInData
  {
    #region Fields

    /// <summary>
    /// The <see cref="IPlugIn"/>-instance for which the informations are kept.
    /// </summary>
    private readonly IPlugIn plugIn;

    /// <summary>
    /// Full Assembly Name
    /// </summary>
    private readonly string assemblyFullName = string.Empty;

    #endregion  Fields

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInData"/> class.
    /// </summary>
    /// <param name="plugIn">A <see cref="IPlugIn"/>-reference to the PlugIn for which to keep the information.</param>
    public PlugInData(IPlugIn plugIn)
    {
        if (plugIn == null)
        {
            throw new ArgumentNullException("plugIn");
        }

        this.plugIn = plugIn;

        Type type = plugIn.GetType();

        if (type == null)
        {
            throw new InvalidOperationException("Unable to retrieve type of 'plugIn'-instance");
        }

      Assembly asm = type.Assembly;
      if (asm == null)
      {
        throw new InvalidOperationException("Unable to retrieve assembly-information for plugIn-Type");
      }

        this.assemblyFullName = asm.FullName;
    }

    #endregion Constructor

    #region Properties

    /// <summary>
    /// Gets the PlugIn this class belongs to.
    /// </summary>
    /// <value>A <see cref="IPlugIn"/>-reference.</value>
    public IPlugIn PlugIn
    {
      get
      {
        return this.plugIn;
      }
    }

    /// <summary>
    /// Gets the full name <see cref="Assembly.FullName"/> of the assembly hosting the PlugIn-type.
    /// </summary>
    public string AssemblyFullName
    {
      get
      {
        return this.assemblyFullName;
      }
    }

    #endregion Properties
  }
}
