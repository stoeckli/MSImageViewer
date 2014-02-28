#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="WiffExperiment.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 28/11/2011 - Implementation of new dot net Assemblies
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System;
using Clearcore2.Data.DataAccess.SampleData;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class is the common base class of the different experiments.
    /// </summary>
    public abstract class WiffExperiment
    {
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="WiffExperiment"/> class
        /// </summary>
        static WiffExperiment()
        {
            SepMaldiParams = new[] { "Laser Frequency\t", "\tHz\r\nLaser Power\t", "\t%\r\nAblation Mode\t", "\t\r\nSkimmer Voltage\t", "\tV\r\nSource Gas\t", "\t\r\nRaster Speed\t", " mm/s\t\r\nManual Continuous Direction\t", "\t\r\nRaster Pitch\t", "\t\r\n" };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WiffExperiment"/> class
        /// Base-Constructor. Performs the initalizations common to all experiments.
        /// </summary>
        /// <param name="msexperiment">Mass Spec Experiment</param>
        /// <param name="wiffsample">Wiff Sample</param>
        protected WiffExperiment(MSExperiment msexperiment, WiffSample wiffsample)
        {
            if (msexperiment == null)
            {
                throw new ArgumentNullException("msexperiment");
            }

            this.Init(msexperiment, wiffsample);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SepMaldiParams
        /// </summary>
        public static string[] SepMaldiParams { get; protected set; }

        #endregion

        #region Methods
        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="inputMsExperiment"/>
        /// </summary>
        /// <param name="inputMsExperiment">Mass Spec Experiment</param>
        /// <param name="wiffsample">Wiff Sample</param>
        protected abstract void Initialize(MSExperiment inputMsExperiment, WiffSample wiffsample);

        /// <summary>
        /// Calls the virtual Initialize method
        /// </summary>
        /// <param name="msexperiment">Mass Spec Experiment</param>
        /// <param name="wiffsample">Wiff Sample</param>
        private void Init(MSExperiment msexperiment, WiffSample wiffsample)
        {
            this.Initialize(msexperiment, wiffsample);
        }

        #endregion
    }
}
