﻿#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="PlugInDataList.cs" company="Novartis Pharma AG.">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Novartis.Msi.PlugInSystem
{
    /// <summary>
    /// A list of <see cref="PlugInData"/> objects.
    /// </summary>
    [Serializable]
    public class PlugInDataList : Collection<PlugInData>
    {
        /// <summary>
        /// Adds the elements of the specified collection to the end of this list.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of this list.
        /// The collection itself cannot be null, but it can contain elements that are null</param>
        public void AddRange(IEnumerable<PlugInData> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (PlugInData element in collection)
            {
                this.Add(element);
            }
        }
    }
}
