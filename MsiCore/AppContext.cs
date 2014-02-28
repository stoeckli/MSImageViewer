#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AppContext.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayehs Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using Microsoft.Win32;

    #region Delegates

    /// <summary>
    /// A delegate type for document related notifications.
    /// </summary>
    /// <param name="sender">Sneder Object</param>
    /// <param name="e">App Arguments</param>
    public delegate void ApplicationEventHandler(object sender, ApplicationEventArgs e);

    #endregion Delegates

    /// <summary>
    /// The <b>AppContext</b> class purpose is to hold global information
    /// </summary>
    public sealed class AppContext
    {
        #region Static Fields

        /// <summary>
        /// Application Interface
        /// </summary>
        private static IApplication app;

        /// <summary>
        /// Main Window
        /// </summary>
        private static Window mainWindow;

        /// <summary>
        /// App Title
        /// </summary>
        private static string appTitle;

        /// <summary>
        /// File Loader List
        /// </summary>
        private static SpecFileLoaderList specFileLoaderList;

        /// <summary>
        /// plugIn Host
        /// </summary>
        private static PlugInHost plugInHost;

        /// <summary>
        /// Dcoument List
        /// </summary>
        private static DocumentList docList;

        /// <summary>
        /// App Exectuion Path
        /// </summary>
        private static string appExePath;

        /// <summary>
        /// App Exectuion directory
        /// </summary>
        private static string appExeDir;

        /// <summary>
        /// App Plugin Directroy
        /// </summary>
        private static string appPlugInsDir;

        /// <summary>
        /// Dictionary of App
        /// </summary>
        private static Dictionary<string, List<string>> appArgs;

        /// <summary>
        /// Document Manager
        /// </summary>
        private static DocumentManager docMan;

        /// <summary>
        /// Palettes or App
        /// </summary>
        private static Palettes palettes;

        /// <summary>
        /// bitmap writer List
        /// </summary>
        private static List<IBitmapWriter> bitmapWriterList;

        /// <summary>
        /// Image Writer List
        /// </summary>
        private static List<IImagingWriter> imagingWriterList;

        #endregion Static Fields
        
        #region Constructor

        /// <summary>
        /// Initializes static members of the <see cref="AppContext"/> class
        /// </summary>
        static AppContext()
        {
            // Initialisation
            app = null;
            mainWindow = null;
            appTitle = string.Empty;
            specFileLoaderList = new SpecFileLoaderList();
            plugInHost = new PlugInHost();
            docList = new DocumentList();
            appExePath = Assembly.GetExecutingAssembly().Location;
            appExeDir = Path.GetDirectoryName(appExePath);
            docMan = new DocumentManager();
            palettes = new Palettes();
            bitmapWriterList = new List<IBitmapWriter>();
            imagingWriterList = new List<IImagingWriter>();
            if (appExeDir != null)
            {
                appPlugInsDir = Path.Combine(appExeDir, "PlugIns");
            }

            BatchMode = false;
            // ToDo JP Read this value from a app config file
            UseApproximation = true;
            ShowToolTips = false;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="AppContext"/> class from being created. 
        /// Initializes a new instance of the AppContext class
        /// Empty private constructor preventing the creation of instances of this class.
        /// This class has solely static methods and properties.
        /// </summary>
        private AppContext()
        {
        }

        #endregion Constructor

        #region Static Events

        /// <summary>
        /// This event will be fired and the subscribers notified, when the user changes the value
        /// for the image representation via the GUI.
        /// </summary>
        public static event ApplicationEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// This event will be fired when the currently active view changes.
        /// </summary>
        public static event ViewEventHandler ActiveViewChanged = delegate { };

        /// <summary>
        /// This event will be fired when a view is closed.
        /// </summary>
        public static event ViewEventHandler ViewClosed = delegate { };

        #endregion Static Events

        #region Static Properties

        /// <summary>
        /// Gets or sets this application's main window.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Trying to set a already assigned MainWindow. </exception>
        public static Window MainWindow
        {
            get
            {
                return mainWindow;
            }

            set
            {
                if (mainWindow != null)
                {
                    throw new InvalidOperationException("Field 'mainWindow' already set!");
                }

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                mainWindow = value;
                appTitle = mainWindow.Title;
            }
        }

        /// <summary>
        /// Gets the application this context belongs to.
        /// </summary>
        /// <value>
        /// A <see cref="IApplication"/> reference.
        /// </value>
        public static IApplication Application
        {
            get
            {
                return app;
            }
        }

        /// <summary>
        /// Gets the title of the application.
        /// </summary>
        public static string ApplicationTitle
        {
            get
            {
                return appTitle;
            }
        }

        /// <summary>
        /// Gets the list of documents this application maintains.
        /// </summary>
        /// <value>
        /// A <see cref="DocumentList"/> reference containing this application's documents.
        /// </value>
        public static DocumentList DocumentList
        {
            get
            {
                return docList;
            }
        }

        /// <summary>
        /// Gets the directory where the applications executable is located.
        /// </summary>
        public static string AppExeDir
        {
            get
            {
                return appExeDir;
            }
        }

        /// <summary>
        /// Gets the directory where the applications plugins are located.
        /// </summary>
        public static string AppPlugInsDir
        {
            get
            {
                return appPlugInsDir;
            }
        }

        /// <summary>
        /// Gets the arguments of the application as dictionary.
        /// See <see cref="Util.ParseCommandLine(string[])"/> for more information.
        /// </summary>
        public static Dictionary<string, List<string>> AppArgs
        {
            get { return appArgs; }
        }

        /// <summary>
        /// Gets the list of registered loaders for spec file contents.
        /// </summary>
        public static SpecFileLoaderList SpecFileLoaders
        {
            get { return specFileLoaderList; }
        }

        /// <summary>
        /// Gets the list of registered writers for bitmap content
        /// </summary>
        public static List<IBitmapWriter> BitmapWriters
        {
            get { return bitmapWriterList; }
        }

        /// <summary>
        /// Gets the list of registered writers for imaging data
        /// </summary>
        public static List<IImagingWriter> ImagingWriters
        {
            get { return imagingWriterList; }
        }

        /// <summary>
        /// Gets the application's document management facility.
        /// </summary>
        public static DocumentManager DocManager
        {
            get { return docMan; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tooltips should be shown.
        /// </summary>
        public static bool ShowToolTips { get; set; }

        /// <summary>
        /// Gets the <see cref="Palettes"/> available in this application.
        /// </summary>
        public static Palettes Palettes
        {
            get { return palettes; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether approximation algorithm be used when importing wiff-files
        /// </summary>
        public static bool UseApproximation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application is running in batch mode (no UI)
        /// </summary>
        /// <value><see langword="bool"/></value>
        public static bool BatchMode { get; set; }

        /// <summary>
        /// Gets or sets the text displayed in the statusbar info item of the application.
        /// </summary>
        /// <value><see langword="string"/></value>
        public static string StatusInfo
        {
            get { return app.StatusInfo; }
            set { app.StatusInfo = value; }
        }

        /// <summary>
        /// Gets or sets the text displayed in the statusbar coordinate item of the application.
        /// </summary>
        /// <value>A <see langword="string"/> value.</value>
        /// <remarks>
        /// The text-string usually contains the formatted coordinate-values including units.
        /// E.g. 'x: 123.4 mm y: 567.8 mm'
        /// </remarks>
        public static string StatusCoord
        {
            get { return app.StatusCoord; }
            set { app.StatusCoord = value; }
        }

        /// <summary>
        /// Gets or sets the text displayed in the statusbar intensity item of the application.
        /// </summary>
        /// <value>A <see langword="string"/> value.</value>
        public static string StatusIntensity
        {
            get { return app.StatusIntensity; }
            set { app.StatusIntensity = value; }
        }

        /// <summary>
        /// Gets the currently active view of the application.
        /// </summary>
        /// <value>The current view as <see cref="IView"/> reference. <see langword="null"/> if no view exists.</value>
        public static IView ActiveView
        {
            get { return app.ActiveView; }
        }

        /// <summary>
        /// Gets the PlugIn hosting instance of the application
        /// </summary>
        public PlugInHost PlugInHost
        {
            get
            {
                return plugInHost;
            }
        }

        #endregion Public Static Properties

        #region Static Methods

        /// <summary>
        /// Sets the application this context belongs to.
        /// </summary>
        /// <param name="application">The application</param>
        public static void SetApplication(IApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (app != null)
            {
                throw new InvalidOperationException("Field 'application' already set!");
            }

            app = application;
        }

        /// <summary>
        /// Do the necessary initializations.
        /// </summary>
        /// <param name="argsDict">
        /// The args Dict.
        /// </param>
        public static void Initialize(Dictionary<string, List<string>> argsDict)
        {
            appArgs = argsDict ?? new Dictionary<string, List<string>>();

            bitmapWriterList.Add(new JpegWriter());
            bitmapWriterList.Add(new PngWriter());
            bitmapWriterList.Add(new TiffWriter());
        }

        /// <summary>
        /// Register the given loader so that it is available as a choice in the fileopen dialog.
        /// </summary>
        /// <param name="loader">A <see cref="ISpecFileLoader"/> reference of the loader to be registered.</param>
        public static void RegisterSpecFileLoader(ISpecFileLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }

            specFileLoaderList.Add(loader);
        }

        /// <summary>
        /// Register the given bitmap writer so that it is available as a choice in the file save dialog.
        /// </summary>
        /// <param name="writer">A <see cref="IBitmapWriter"/> reference of the loader to be registered.</param>
        public static void RegisterBitmapWriter(IBitmapWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            bitmapWriterList.Add(writer);
        }

        /// <summary>
        /// Register a imaging writer instance (offered to the user as a choice)
        /// </summary>
        /// <param name="writer">Writer Object</param>
        public static void RegisterImagingWriter(IImagingWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            imagingWriterList.Add(writer);
        }

        /// <summary>
        /// Calls <see cref="PlugInHost"/>.CollectPlugIns().
        /// </summary>
        public static void CollectPlugIns()
        {
            if (plugInHost != null)
            {
                plugInHost.CollectPlugIns();
            }
        }

        /// <summary>
        /// Calls <see cref="PlugInHost"/>.InitializePlugIns().
        /// </summary>
        public static void InitializePlugIns()
        {
            if (plugInHost != null)
            {
                plugInHost.InitializePlugIns();
            }
        }

        /// <summary>
        /// Raise a event signaling that the palette index has changed.
        /// </summary>
        /// <param name="paletteIndex">palette Index</param>
        public static void FirePaletteIndexChanged(int paletteIndex)
        {
            PropertyChanged(MainWindow, new ApplicationEventArgs(AppProperties.PaletteIndex, paletteIndex));
        }

        /// <summary>
        /// Raise a event signaling that the minimum intensity has changed.
        /// </summary>
        /// <param name="minIntensity">Minimum Intensity</param>
        public static void FireMinimumIntensityChanged(double minIntensity)
        {
            PropertyChanged(MainWindow, new ApplicationEventArgs(AppProperties.MinIntensity, minIntensity));
        }

        /// <summary>
        /// Raise a event signaling that the maximum intensity has changed.
        /// </summary>
        /// <param name="maxIntensity">Maximum Intensity</param>
        public static void FireMaximumIntensityChanged(double maxIntensity)
        {
            PropertyChanged(MainWindow, new ApplicationEventArgs(AppProperties.MaxIntensity, maxIntensity));
        }

        /// <summary>
        /// Raise a event signaling that the Scan Number has changed.
        /// </summary>
        /// <param name="massNumber">MassNumber to view</param>
        public static void FireScanNumberChanged(double massNumber)
        {
            PropertyChanged(MainWindow, new ApplicationEventArgs(AppProperties.CurrentMass, massNumber));
        }

        /// <summary>
        /// Raise the event signaling that the currently active view has changed.
        /// </summary>
        /// <param name="newActiveView">The new active view as <see cref="IView"/> reference.</param>
        public static void FireActiveViewChanged(IView newActiveView)
        {
            ActiveViewChanged(newActiveView, new EventArgs());
        }

        /// <summary>
        /// Raise the event signaling that a view has been closed.
        /// </summary>
        /// <param name="closedView">The view that has been closed as a <see cref="IView"/> reference.</param>
        public static void FireViewClosed(IView closedView)
        {
            ViewClosed(closedView, new EventArgs());
        }

        /// <summary>
        /// Saves certain application data to persist it to the next session.
        /// </summary>
        /// <param name="applicationKey">The registry key where to save information.</param>
        public static void SaveContext(RegistryKey applicationKey)
        {
            // TO DO
        }

        /// <summary>
        /// Loads certain application data saved from earlier sessions.
        /// </summary>
        /// <param name="applicationKey">The registry key where to look for data.</param>
        public static void LoadContext(RegistryKey applicationKey)
        {
            // TO DO
        }

        /// <summary>
        /// Starts a progressbar action. The specified <paramref name="operation"/> identifier and the progressbar are shown and the
        /// progressbar is made ready to receive values.
        /// </summary>
        /// <param name="operation">A identifier describing the current operation.</param>
        public static void ProgressStart(string operation)
        {
            app.ProgressStart(operation);
        }

        /// <summary>
        /// Sets the Progressbar to show the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to be shown in the progressbar. The range of values is expected to be 0.0 to 100.0</param>
        public static void ProgressSetValue(double value)
        {
            app.ProgressSetValue(value);
        }

        /// <summary>
        /// Stops the current progressbar action. Clears and hides the progressbar.
        /// </summary>
        public static void ProgressClear()
        {
            app.ProgressClear();
        }

        #endregion Static Methods
    }
}
