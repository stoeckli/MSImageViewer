﻿#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="TypeList.cs" company="Novartis Pharma AG.">
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
using System.Collections.ObjectModel;

namespace Novartis.Msi.PlugInSystem
{
    /// <summary>
    /// A list of <see cref="Type"/> objects.
    /// </summary>
    [Serializable]
    public class TypeList : Collection<Type>
    {
    }
}
