#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="CommonObjects.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.Core
{
    #region Enumerators

    /// <summary>
    /// Defining ExperimentType here as we need to differentiate between experiment types
    /// for UI purposes. 
    /// There is also a definition in WiffLoader (which we must keep in sync with)
    /// But since WiffLoader is a plugin, we don't want to reply on definitions with in that module.
    /// </summary>
    public enum ExperimentType
    {
        /// <summary>
        /// MS Type Experiment
        /// </summary>
        MS = 0,

        /// <summary>
        /// Product Type Experiment
        /// </summary>
        Product = 1,

        /// <summary>
        /// Precursor Type Experiment
        /// </summary>
        Precursor = 2,

        /// <summary>
        /// Neutral Gain Or Loss Type Experiment
        /// </summary>
        NeutralGainOrLoss = 3,

        /// <summary>
        /// SIM Type Experiment
        /// </summary>
        SIM = 4,

        /// <summary>
        /// MRM Type Experiment
        /// </summary>
        MRM = 5,
    }

    #endregion Enumerators

    #region Structs

    /// <summary>
    /// Data describing  a intensity in a way suitable to
    /// setup a slider control.
    /// </summary>
    public struct IntensitySettings
    {
        /// <summary>
        /// The current intensity.
        /// </summary>
        public double CurrentIntensity;

        /// <summary>
        /// The minimum intensity.
        /// </summary>
        public double MinIntensity;

        /// <summary>
        /// The maximum intensity.
        /// </summary>
        public double MaxIntensity;

        /// <summary>
        /// The step width.
        /// </summary>
        public double StepSmall;

        /// <summary>
        /// The large step width.
        /// </summary>
        public double StepLarge;
    }

    /// <summary>
    /// Pixel Point in Double, Used in various views 
    /// </summary>
    public struct PixelPointInt
    {
        /// <summary>
        /// X Position
        /// </summary>
        public int X;

        /// <summary>
        /// X Position
        /// </summary>
        public int Y;
    }

    /// <summary>
    /// Pixel Point in Double
    /// </summary>
    public struct PixelPointDbl
    {
        /// <summary>
        /// X Position
        /// </summary>
        public double X;

        /// <summary>
        /// X Position
        /// </summary>
        public double Y;
    }

    #endregion Structs
}