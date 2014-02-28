#region Copyright © 2010 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
//
// © 2010 Novartis AG. All rights reserved.
//
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
//
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2010 Novartis AG

using System;
using System.Collections.ObjectModel;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// A list of <see cref="WiffSample"/> objects.
    /// </summary>
    [Serializable]
    public class WiffSampleList : Collection<WiffSample>
    {
    }
}
