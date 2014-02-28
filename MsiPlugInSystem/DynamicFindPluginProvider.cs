#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="DynamicFindPlugInProvider.cs" company="Novartis Pharma AG.">
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

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace Novartis.Msi.PlugInSystem
{
    /// <summary>
    /// Dynamically searches for plugins that implement IPlugIn
    /// in a specific location
    /// </summary>
    [Serializable]
    public class DynamicFindPlugInProvider : IPlugInProvider
    {
        #region Fields

        /// <summary>
        /// Plugin Data List
        /// </summary>
        private readonly PlugInDataList plugIns = new PlugInDataList();

        /// <summary>
        /// Type List
        /// </summary>
        private readonly TypeList goodTypes = new TypeList();

        /// <summary>
        /// Object Array
        /// </summary>
        private readonly ArrayList baseObjectTypesList = new ArrayList();

        /// <summary>
        /// Search Directory
        /// </summary>
        private string searchDirectory;

        /// <summary>
        /// File Extension Filter
        /// </summary>
        private string fileExtensionFilter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicFindPlugInProvider"/> class
        /// DynamicFindPlugInProvider.
        /// </summary>
        public DynamicFindPlugInProvider()
        {
            this.searchDirectory = string.Empty;
            this.fileExtensionFilter = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicFindPlugInProvider"/> class
        /// </summary>
        /// <param name="searchDirectory">
        /// The directory where the provider performs the search for plugins.
        /// </param>
        public DynamicFindPlugInProvider(string searchDirectory)
        {
            this.searchDirectory = searchDirectory;
            this.fileExtensionFilter = "*.dll";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the collection of loaded plugins.
        /// </summary>
        public PlugInDataList LoadedPlugIns
        {
            get
            {
                return this.plugIns;
            }
        }

        #endregion Properties

        #region Properties

        /// <summary>
        /// Gets or sets the directory for searching.
        /// </summary>
        public string SearchDirectory
        {
            get
            {
                return this.searchDirectory;
            }

            set
            {
                this.searchDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets file extension filter.
        /// </summary>
        public string FileExtensionFilter
        {
            get
            {
                return this.fileExtensionFilter;
            }

            set
            {
                this.fileExtensionFilter = value;
            }
        }

        #endregion Public Properties

        #region Methods

        /// <summary>
        /// Loads the possible plugin assemblies in a separate AppDomain
        /// in order to allow unloading of unneeded assemblies.
        /// </summary>
        /// <param name="alreadyLoadedPlugIns">List of Plugins</param>
        public void LoadPlugIns(PlugInDataList alreadyLoadedPlugIns)
        {
            this.plugIns.Clear();

            if (this.SearchDirectory == null || !Directory.Exists(this.SearchDirectory))
            {
                throw new DirectoryNotFoundException("Could not find plugin directory");
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Create evidence for new appdomain.
            Evidence adevidence = AppDomain.CurrentDomain.Evidence;

            // Create a setup object for the new application domain.
            var setup = new AppDomainSetup { PrivateBinPath = baseDir + ";" + this.SearchDirectory };

            // Append the relative path

            // The class actually created a new instance of the same class
            // only in a different AppDomain. It then passes it all the needed
            // parameters such as search path and file extensions.
            AppDomain domain = AppDomain.CreateDomain("DynamicPlugInLoader", adevidence, setup);

            string assemblyName = baseDir + "MsiPlugInSystem.dll";

            var finder = (DynamicFindPlugInProvider)domain.CreateInstanceFromAndUnwrap(assemblyName, typeof(DynamicFindPlugInProvider).ToString());
            finder.FileExtensionFilter = this.FileExtensionFilter;
            finder.SearchDirectory = this.SearchDirectory;

            TypeList foundPlugInTypes = finder.SearchPath(this.SearchDirectory);

            if (foundPlugInTypes.Count != finder.baseObjectTypesList.Count)
            {
                throw new InvalidOperationException("Count of PlugIns and BaseObjectLists doesn't match!");
            }

            AppDomain.Unload(domain);

            foreach (Type t in foundPlugInTypes)
            {
                if (t != null)
                {
                    var assemblyFullName = t.Assembly.FullName;

                    bool alreadyLoaded = false;
                    foreach (PlugInData loadedPlugInData in alreadyLoadedPlugIns)
                    {
                        if (loadedPlugInData.AssemblyFullName == assemblyFullName)
                        {
                            alreadyLoaded = true;
                            break;
                        }
                    }

                    if (!alreadyLoaded)
                    {
                        object plugInInstance = Activator.CreateInstance(t);

                        var plugIn = plugInInstance as IPlugIn;

                        if (plugIn != null)
                        {
                            var plugInData = new PlugInData(plugIn);
                            this.plugIns.Add(plugInData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches Path
        /// </summary>
        /// <param name="path">Path Name</param>
        /// <returns>Type List of good paths</returns>
        private TypeList SearchPath(string path)
        {
            this.goodTypes.Clear();

            foreach (string file in Directory.GetFiles(path, this.FileExtensionFilter))
            {
                this.TryLoadingPlugIn(file);
            }

            return this.goodTypes;
        }

        /// <summary>
        /// Add types to collection
        /// </summary>
        /// <param name="goodType">A Good Type</param>
        private void AddToGoodTypesCollection(Type goodType)
        {
            this.goodTypes.Add(goodType);
        }

        /// <summary>
        /// Try to load the Plugin
        /// </summary>
        /// <param name="path">Path of Plugin</param>
        private void TryLoadingPlugIn(string path)
        {
            var file = new FileInfo(path);
            path = file.Name.Replace(file.Extension, string.Empty);
            Assembly asm = AppDomain.CurrentDomain.Load(path);

            var baseObjects = new TypeList();
            bool once = false;

            foreach (Type t in asm.GetTypes())
            {
                foreach (Type iface in t.GetInterfaces())
                {
                    if (iface.Equals(typeof(IPlugIn)))
                    {
                        if (once)
                        {
                            throw new InvalidOperationException("Only one IPlugIn-object per Assembly allowed!");
                        }

                        this.AddToGoodTypesCollection(t);
                        this.baseObjectTypesList.Add(baseObjects);
                        once = true;
                    }
                }
            }
        }

        #endregion Methods
    }
}
