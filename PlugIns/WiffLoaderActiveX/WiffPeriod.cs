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
using NETMSMethodSvr;

using Novartis.Msi.Core;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class encapsulates a wiff file sample.
    /// </summary>
    public class WiffPeriod
    {
        /// <summary>
        /// The <see cref="WiffSample"/> instance this period belongs to.
        /// </summary>
        private WiffSample sample;

        /// <summary>
        /// The <b>zero-based</b> index of this period in the associated sample.
        /// </summary>
        private int index;

        /// <summary>
        /// The list of experiments this period consists of.
        /// </summary>
        private WiffExperimentList experiments = new WiffExperimentList();

        private int cycles;
        private double cycleTime;

        /// <summary>
        /// Constructor. Creates and initializes a WiffPeriod instance.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The source from where to
        /// read the periods properties and data.</param>
        /// <param name="wiffSample">The <see cref="WiffSample"/> instance this period belongs to.</param>
        /// <param name="periodIndex">The <b>zero-based</b> index of this period in the associated sample.</param>
        public WiffPeriod(FMANWiffFileClass wiffFile, WiffSample wiffSample, int periodIndex)
        {
            if (wiffSample == null)
                throw new ArgumentNullException("wiffSample");

            sample = wiffSample;

            if (periodIndex < 0)
                throw new ArgumentOutOfRangeException("periodIndex");

            index = periodIndex;

            Initialize(wiffFile);
        }

        /// <summary>
        /// The <see cref="WiffSample"/> instance this period belongs to.
        /// </summary>
        public WiffSample Sample
        {
            get { return sample; }
        }

        /// <summary>
        /// The <b>zero-based</b> index of this period in the associated sample.
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// The <see cref="WiffExperimentList"/> of experiments in this period.
        /// </summary>
        public WiffExperimentList Experiments
        {
            get { return experiments; }
        }

        /// <summary>
        /// The number of cycles for the experiments in this period.
        /// </summary>
        public int Cycles
        {
            get { return Cycles; }
        }

        /// <summary>
        /// The amount of time in seconds 1 cycle requires.
        /// </summary>
        public double CycleTime
        {
            get { return cycleTime / 1000.0; }
        }

        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="wiffFile"/>.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The data source.</param>
        private void Initialize(FMANWiffFileClass wiffFile)
        {

            //get the number of cycles 
            cycles = wiffFile.GetActualNumberOfCycles(sample.Index, index);

            // get the period object
            Period period = (Period)wiffFile.GetPeriodObject(sample.Index, index);

            // get the cycle time
            cycleTime = period.CycleTime;

            // get number of experiments
            int nrExperiments = wiffFile.GetNumberOfExperiments(sample.Index, index);

            // loop through the experiments; the index of the experiments is zero based!!
            for (int actExperiment = 0; actExperiment < nrExperiments; actExperiment++)
            {
                WiffExperiment experiment = WiffExperimentFactory.CreateWiffExperiment(wiffFile, this, actExperiment);
                experiments.Add(experiment);
            }
        }


    }
}
