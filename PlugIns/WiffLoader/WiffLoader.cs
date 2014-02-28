#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="WiffLoader.cs" company="Novartis Pharma AG.">
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

using Clearcore2.Data.AnalystDataProvider;

using Novartis.Msi.Core;
using Novartis.Msi.PlugInSystem;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class facilitates the registration of the WiffLoader.
    /// </summary>
    public class WiffLoader : IPlugIn
    {
        #region Fields

        /// <summary>
        /// List of code authors of the code
        /// </summary>
        private readonly string author;

        /// <summary>
        /// Description of the object
        /// </summary>
        private readonly string description;
        
        /// <summary>
        /// Name of object
        /// </summary>
        private readonly string name;

        /// <summary>
        /// version of the software
        /// </summary>
        private readonly string version;

        /// <summary>
        /// Plud in Host
        /// </summary>
        private IPlugInHost plugInHost;

        #endregion Fields 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WiffLoader"/> class  
        /// </summary>
        public WiffLoader()
        {
            this.name = "WiffLoader";
            this.author = "Markus Stoeckli / Bernhard Rode / Jayesh Patel";
            this.description = "Loads the content of a 'wiff' massspectrometry data file";
            this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Licenser.LicenseKey = "Novartis Pharma AG|Beta 2011-08-25|0D46864528986E0E58894D41895CFBBB79EC21BE04B2221A441D94E942C75F688D9E94118756C9C1";            
        }

        #endregion Constructors 

        #region Properties

        /// <summary>
        /// Gets the PlugIn author.
        /// </summary>
        public string Author
        {
            get { return this.author; }
        }

        /// <summary>
        /// Gets the PlugIn description.
        /// </summary>
        public string Description
        {
            get { return this.description; }
        }

        /// <summary>
        /// Gets the PlugIn's hosting instance.
        /// </summary>
        public IPlugInHost Host
        {
            get { return this.plugInHost; }
        }

        /// <summary>
        /// Gets the PlugIn name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the PlugIn version.
        /// </summary>
        public string Version
        {
            get { return this.version; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Implements <see cref="IPlugIn.Dispose()"/>.
        /// </summary>
        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("Dispose called for 'WiffLoader' PlugIn.");
        }

        /// <summary>
        /// Implements <see cref="IPlugIn.Initialize(IPlugInHost)"/>.
        /// Registers the Loader for 'wiff'-files.
        /// </summary>
        /// <param name="host">
        /// A <see cref="IPlugInHost"/> instance.
        /// </param>
        public void Initialize(IPlugInHost host)
        {
            this.plugInHost = host;

            // register the 'wiff'-fileloader in the application
            AppContext.RegisterSpecFileLoader(new WiffFileLoader());
        }

        #endregion Methods
    }
}
