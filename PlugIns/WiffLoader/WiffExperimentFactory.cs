#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="WiffExperimentFactory.cs" company="Novartis Pharma AG.">
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

using Clearcore2.Data.DataAccess.SampleData;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// Factory for <see cref="WiffExperiment"/> instances. Determines which subclass
    /// of <see cref="WiffExperiment"/> to use and delivers a newly intantiated object.
    /// </summary>
    public static class WiffExperimentFactory
    {
        #region Methods

        /// <summary>
        /// Determines with what kind of experiment we're dealing with and instatiates an object of the accurate 
        /// subclass derived from the abstract baseclass read the experiments properties and data.
        /// WiffScanTypeEnumeration: Q1, MRM, SimQ1, Q3, SimQ3, Undefined, PrecursorIon, ProductIon, NeutralLoss, TofMS, 
        /// TofProductIon, TofPrecursorIon, EnhancedProductIon, EnhancedResolution, MS3, TimeDelayFragmentation, EnhancedMS, EnhancedMulticharge,
        /// </summary>
        /// <param name="msexperiment"> The msexperiment. </param>
        /// <param name="wiffsample"> The wiffsample. </param>
        /// <returns> The newly created, <see cref="WiffExperiment"/> dirived object. </returns>
        public static WiffExperiment CreateWiffExperiment(MSExperiment msexperiment, WiffSample wiffsample)
        {
            ExperimentType expType = msexperiment.Details.ExperimentType;

            WiffExperiment theExperiment;

            switch (expType)
            {
                case ExperimentType.MRM:
                    {
                        theExperiment = new MrmScanExperiment(msexperiment, wiffsample);
                    }

                    break;
                case ExperimentType.MS:
                    {
                        theExperiment = new MsScanExperiment(msexperiment, wiffsample);
                    }

                    break;
                case ExperimentType.Precursor:
                case ExperimentType.NeutralGainOrLoss:
                case ExperimentType.Product:
                case ExperimentType.SIM:
                    {
                        theExperiment = new MsScanExperiment(msexperiment, wiffsample);
                    }

                    break;
                default:
                    {
                        // TODO -- Check if this is really, really intended...??
                        theExperiment = new MrmScanExperiment(msexperiment, wiffsample);
                    }

                    break;
            }
            
            return theExperiment;
        }

        #endregion
    }
}
