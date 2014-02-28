#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AppProperties.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Enumeration of the applications properties.
    /// </summary>
    public enum AppProperties
    {
        /// <summary>
        /// Palette Index
        /// </summary>
        PaletteIndex = 0,

        /// <summary>
        /// Minimum Intensity
        /// </summary>
        MinIntensity,

        /// <summary>
        /// Maximum Intensity
        /// </summary>
        MaxIntensity,

        /// <summary>
        /// Current Mass 
        /// </summary>
        CurrentMass
    }
}