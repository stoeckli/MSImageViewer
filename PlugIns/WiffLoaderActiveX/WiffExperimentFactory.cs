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

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// Factory for <see cref="WiffExperiment"/> instances. Determines which subclass
    /// of <see cref="WiffExperiment"/> to use and delivers a newly intantiated object.
    /// </summary>
    public sealed class WiffExperimentFactory
    {
        /// <summary>
        /// Determines with what kind of experiment we're dealing with and instatiates
        /// an object of the accurate subclass derived from the abstract baseclass
        /// <see cref="WiffExperiment"/>.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The source from where to
        /// read the experiments properties and data.</param>
        /// <param name="wiffPeriod">The <see cref="WiffPeriod"/> instance this the experiment belongs to.</param>
        /// <param name="experimentIndex">The <b>zero-based</b> index of the experiment in the associated period.</param>
        /// <returns>The newly created, <see cref="WiffExperiment"/> dirived object.</returns>
        public static WiffExperiment CreateWiffExperiment(FMANWiffFileClass wiffFile, WiffPeriod wiffPeriod, int experimentIndex)
        {
            Experiment experiment = (Experiment)wiffFile.GetExperimentObject(wiffPeriod.Sample.Index, wiffPeriod.Index, experimentIndex);
            // get scan type
            short scanType = experiment.ScanType;
            WiffExperiment theExperiment = null;
            switch (scanType)
            {
                case 0: // Q1 scan
                    {
                        theExperiment = new Q1ScanExperiment(wiffFile, wiffPeriod, experimentIndex);
                    }
                    break;

                case 1: // Q1 MI scan
                case 2: // Q3 scan
                case 3: // Q3 MI scan
                    break;
                case 4: // MRM scan
                    {
                        theExperiment = new MRMScanExperiment(wiffFile, wiffPeriod, experimentIndex);
                    }
                    break;
                case 5: // precursor scan
                case 6: // product ion scan
                case 7: // neural loss scan
                    break;
                default:
                    {
                        // TODO -- Check if this is really, really intended...
                        theExperiment = new MRMScanExperiment(wiffFile, wiffPeriod, experimentIndex);
                    }
                    break;
            }

            return theExperiment;
        }

        /// <summary>
        /// Empty private constructor to prevent instanciation.
        /// </summary>
        private WiffExperimentFactory()
        {
        }
    }
}
