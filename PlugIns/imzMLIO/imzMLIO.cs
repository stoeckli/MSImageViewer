#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ImzMLIO.cs" company="Novartis Pharma AG.">
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

using Novartis.Msi.Core;
using Novartis.Msi.PlugInSystem;

namespace Novartis.Msi.PlugIns.ImzMLIO
{
    /// <summary>
    /// This class registers the imzML file reader / writer
    /// </summary>
    public class ImzMlio : IPlugIn
    {
        #region Fields

        /// <summary>
        /// Author of Plugin
        /// </summary>
        private readonly string author;

        /// <summary>
        /// Description of Plugin
        /// </summary>
        private readonly string description;

        /// <summary>
        /// Name of Plugin
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Version of Plugin
        /// </summary>
        private readonly string version;

        /// <summary>
        /// PlugIn Host
        /// </summary>
        private IPlugInHost plugInHost;

        #endregion Fields 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImzMlio"/> class 
        /// </summary>
        public ImzMlio()
        {
            this.author = "Bernhard Rode";
            this.description = "Read/write MSImaging data in the 'imzML' format";
            this.name = "imzMLIO";
            this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #endregion Constructors 

        #region IPlugIn Member

        #region Properties

        /// <summary>
        /// Gets the PlugIn author
        /// </summary>
        public string Author
        {
            get
            {
                return this.author;
            }
        }

        /// <summary>
        /// Gets PlugIn description
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
        /// Gets the PlugIn version.
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
            System.Diagnostics.Debug.WriteLine("Dispose called for 'imzMLIO' PlugIn.");
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
            AppContext.RegisterImagingWriter(new ImzMlWriter());
        }

        #endregion Methods

        #endregion IPlugIn Member
    }
}
