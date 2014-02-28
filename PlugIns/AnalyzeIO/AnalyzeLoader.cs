#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyzeLoader.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.AnalyzeIO
{
    using Novartis.Msi.Core;
    using Novartis.Msi.PlugInSystem;

    /// <summary>
    /// This class registers the Analyze file reader / writer
    /// </summary>
    public class AnalyzeLoader : IPlugIn
    {
        #region Fields

        /// <summary>
        /// Author of module
        /// </summary>
        private readonly string author;

        /// <summary>
        /// Description Field
        /// </summary>
        private readonly string description;

        /// <summary>
        /// Module Name
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Version Feild
        /// </summary>
        private readonly string version;

        /// <summary>
        /// PlugIn Interface
        /// </summary>
        private IPlugInHost plugInHost;

        #endregion Fields 

        #region IPlugIn Member

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeLoader"/> class 
        /// </summary>
        public AnalyzeLoader()
        {
            this.name = "AnalyzeIO";
            this.description = "Read/write MSImaging data in the 'analyze' format";
            this.author = "Markus Stoeckli / Bernhard Rode/Jayesh Patel";
            this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        
        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets PlugIn author.
        /// </summary>
        public string Author
        {
            get
            {
                return this.author;
            }
        }

        /// <summary>
        /// Gets PlugIn description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Gets the PlugIn's hosting instance.
        /// </summary>
        public IPlugInHost Host
        {
            get
            {
                return this.plugInHost;
            }
        }

        /// <summary>
        /// Gets PlugIn name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets PlugIn version.
        /// </summary>
        public string Version
        {
            get
            {
                return this.version;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Implements <see cref="IPlugIn.Dispose()"/>.
        /// </summary>
        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("Dispose called for 'AnalyzeIO' PlugIn.");
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

            // register the 'analyze'-fileloader in the application
            AppContext.RegisterSpecFileLoader(new AnalyzeFileLoader());
            
            // register the 'analyze'-fileloader in the application
            AppContext.RegisterImagingWriter(new AnalyzeWriter());
        }

        #endregion Methods

        #endregion IPlugIn Member
    }
}
