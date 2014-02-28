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


using NETExploreDataObjects;


namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class is the common baseclass of the different experiments.
    /// </summary>
    public abstract class WiffExperiment
    {
        #region Static Fields

        /// <summary>
        /// textual representation of the parameters to retrieve.
        /// </summary>
        static protected string[] sepMALDIParams = { "Laser Frequency\t",
                                                     "\tHz\r\nLaser Power\t",
                                                     "\t%\r\nAblation Mode\t",
                                                     "\t\r\nSkimmer Voltage\t",
                                                     "\tV\r\nSource Gas\t",
                                                     "\t\r\nRaster Speed\t",
                                                     " mm/s\t\r\nManual Continuous Direction\t",
                                                     "\t\r\nRaster Pitch\t",
                                                     "\t\r\n" };

        #endregion

        /// <summary>
        /// The <see cref="WiffPeriod"/> instance this experiment belongs to.
        /// </summary>
        private WiffPeriod wiffPeriod;

        /// <summary>
        /// The <b>zero-based</b> index of this period in the associated sample.
        /// </summary>
        private int index;

        /// <summary>
        /// Base-Constructor. Performs the initalizations common to all experiments.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The source from where to
        /// read the periods properties and data.</param>
        /// <param name="wiffPeriod">The <see cref="WiffPeriod"/> instance this experiment belongs to.</param>
        /// <param name="experimentIndex">The <b>zero-based</b> index of this experiment in the associated period.</param>
        public WiffExperiment(FMANWiffFileClass wiffFile, WiffPeriod wiffPeriod, int experimentIndex)
        {
            if (wiffFile == null)
                throw new ArgumentNullException("wiffFile");

            if (wiffPeriod == null)
                throw new ArgumentNullException("wiffPeriod");

            this.wiffPeriod = wiffPeriod;
            this.index = experimentIndex;

            Initialize(wiffFile);
        }

        /// <summary>
        /// The <see cref="WiffPeriod"/> instance this experiment belongs to.
        /// </summary>
        public WiffPeriod WiffPeriod
        {
            get { return wiffPeriod; }
        }

        /// <summary>
        /// The <b>zero-based</b> index of this period in the associated sample.
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="wiffFile"/>.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The data source.</param>
        protected abstract void Initialize(FMANWiffFileClass wiffFile);

    }
}
