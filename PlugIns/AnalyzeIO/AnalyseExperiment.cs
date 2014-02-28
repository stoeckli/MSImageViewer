#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyseExperiment.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel 13/02/2012 - Analyse Loader Updates
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.AnalyzeIO
{
    /// <summary>
    /// This class is the common base class of the different Analyse experiments.
    /// </summary>
    public abstract class AnalyseExperiment
    {
        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyseExperiment"/> class
        /// </summary>
        /// <param name="analysefilecontent">analyse file content Object</param>
        /// <param name="filePath">Path of file </param>
        /// <param name="analyseFileHeaderObject">Header Object</param>
        protected AnalyseExperiment(AnalyseFileContent analysefilecontent, string filePath, AnalyseFileHeaderObject analyseFileHeaderObject)
        {
            this.Init(analysefilecontent, filePath, analyseFileHeaderObject);
        }

        #endregion Constructors
        
        #region Methods

        /// <summary>
        /// Fills this instance data members with the information read from the img file
        /// </summary>
        /// <param name="analysefilecontent">analyse file content Object</param>
        /// <param name="filePath">Path of file </param>
        /// <param name="analyseFileHeaderObject">Header Object</param>
        protected abstract void Initialise(AnalyseFileContent analysefilecontent, string filePath, AnalyseFileHeaderObject analyseFileHeaderObject);

        /// <summary>
        /// Calls the virtual Initialize method
        /// </summary>
        /// <param name="analysefilecontent">analyse file content Object</param>
        /// <param name="filePath">Path of file </param>
        /// <param name="analyseFileHeaderObject">Header Object</param>
        private void Init(AnalyseFileContent analysefilecontent, string filePath, AnalyseFileHeaderObject analyseFileHeaderObject)
        {
            this.Initialise(analysefilecontent, filePath, analyseFileHeaderObject);
        }

        #endregion Methods
    }
}
