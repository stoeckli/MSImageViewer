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

using Novartis.Msi.Core;
using Novartis.Msi.PlugInSystem;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class facilitates the registration of the WiffLoader.
    /// </summary>
    public class WiffLoader : IPlugIn
    {
		#region Fields

        private string author = "Markus Stoeckli / Bernhard Rode";
        private string description = "Loads the content of a 'wiff' massspectrometry data file";
        private string name = "WiffLoader";
        private IPlugInHost plugInHost = null;
        private string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

		#endregion Fields 

		#region Constructors

        /// <summary>
        /// Default-Constructor. Creates a WiffLoader instance.
        /// </summary>
        public WiffLoader()
        {
        }

		#endregion Constructors 



        #region IPlugIn Member

        #region Properties

        /// <summary>
        /// PlugIn author.
        /// </summary>
        public string Author
        {
            get { return author; }
        }

        /// <summary>
        /// PlugIn description.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// The PlugIn's hosting instance.
        /// </summary>
        public IPlugInHost Host
        {
            get { return plugInHost; }
        }

        /// <summary>
        /// PlugIn name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// PlugIn version.
        /// </summary>
        public string Version
        {
            get { return version; }
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
            plugInHost = host;
            // register the 'wiff'-fileloader in the application
            AppContext.RegisterSpecFileLoader(new WiffFileLoader());
        }

        #endregion Methods

        #endregion IPlugIn Member
    }
}
