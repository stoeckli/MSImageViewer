#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="MainWindow.xaml.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.MSImageView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Linq;
    
    using AvalonDock;
    using Microsoft.Win32;
    using Novartis.Msi.Core;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IApplication
    {
        #region Fields

        #region Routed Command

            /// <summary>
            /// The exit command.
            /// </summary>
            private static RoutedCommand exitCommand = new RoutedCommand("Exit", typeof(MainWindow), null);

            /// <summary>
            /// The 'toggle tooltips' command.
            /// </summary>
            private static RoutedCommand toggleToolTipsCommand = new RoutedCommand("ToggleToolTips", typeof(MainWindow), null);

            /// <summary>
            /// The about command.
            /// </summary>
            private static RoutedCommand aboutCommand = new RoutedCommand("About", typeof(MainWindow), null);

            /// <summary>
            /// The 'use approximation' command.
            /// </summary>
            private static RoutedCommand useApproximationCommand = new RoutedCommand("UseApproximation", typeof(MainWindow), null);

            /// <summary>
            /// The newROI command.
            /// </summary>
            private static RoutedCommand newRoiCommand = new RoutedCommand("NewRoi", typeof(MainWindow), null);
        
            /// <summary>
            /// The openROI command.
            /// </summary>
            private static RoutedCommand openRoiCommand = new RoutedCommand("OpenRoi", typeof(MainWindow), null);
        
            /// <summary>
            /// The saveROI command.
            /// </summary>
            private static RoutedCommand saveRoiCommand = new RoutedCommand("SaveRoi", typeof(MainWindow), null);
        
            /// <summary>
            /// The saveROIAs command.
            /// </summary>
            private static RoutedCommand saveRoiAsCommand = new RoutedCommand("SaveRoiAs", typeof(MainWindow), null);

            /// <summary>
            /// The closeROI command.
            /// </summary>
            private static RoutedCommand closeRoiCommand = new RoutedCommand("CloseRoi", typeof(MainWindow), null);

        #endregion Routed Command

        #region Other Fields

        /// <summary>
        /// Lock Object
        /// </summary>
        private readonly object lockObject = new object();

        /// <summary>
        /// Update Progress bar delegate method
        /// </summary>
        private readonly UpdateProgressBarDelegate updateProgressDelegate;

        /// <summary>
        /// List of <see cref="IView"/> objects and there docking containers (<see cref="ViewContent"/>).
        /// </summary>
        private readonly List<KeyValuePair<IView, ViewContent>> views = new List<KeyValuePair<IView, ViewContent>>();

        /// <summary>
        /// The currently active view or null ifn't any view exists.
        /// </summary>
        private IView activeView;

        /// <summary>
        /// View Content
        /// </summary>
        private ViewContent activatePostClose;

        /// <summary>
        /// A roiProject
        /// </summary>
        private RoiProject roiproject;

        /// <summary>
        /// Flag for indicating if current roi project has changed
        /// </summary>
        private bool roiprojectchangedflag;

        #endregion Other Fields

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class  
        /// </summary>
        public MainWindow()
        {
            try
            {
                // make this object known to the core dll to facilitate 'callbacks'.
                AppContext.SetApplication(this);

                InitializeComponent();

                // Insert code required on object creation below this point.
                this.Title = Strings.AppName;
                AppContext.MainWindow = this;

                this.updateProgressDelegate = this.progressBar.SetValue;

                // Initialize the GUI
                this.InitializeUi();
            }
            catch (Exception e) 
            { 
                Util.ReportException(e); 
            }
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Delegate to facilitate progressbar operation (setting the progressbar value).
        /// </summary>
        /// <param name="dp">The property to set (e.g. ProgressBar.Value)</param>
        /// <param name="value">The value to be assigned to the property (e.g. ProgressBar.Value = (double)value).</param>
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

        #endregion

        #region Routed Commands

        /// <summary>
        /// Gets Routed ExitCommand
        /// </summary>
        public static RoutedCommand ExitCommand
        {
            get { return exitCommand; }
        }

        /// <summary>
        /// Gets Routed ToggleToolTipsCommand
        /// </summary>
        public static RoutedCommand ToggleToolTipsCommand
        {
            get { return toggleToolTipsCommand; }
        }

        /// <summary>
        /// Gets Routed AboutCommand
        /// </summary>
        public static RoutedCommand AboutCommand
        {
            get { return aboutCommand; }
        }

        /// <summary>
        /// Gets Routed UseApproximationCommand
        /// </summary>
        public static RoutedCommand UseApproximationCommand
        {
            get { return useApproximationCommand; }
        }

        /// <summary>
        /// Gets Routed NewRoiCmd
        /// </summary>
        public static RoutedCommand NewRoiCommand
        {
            get { return newRoiCommand; }
        }

        /// <summary>
        /// Gets Routed OpenRoiCommand
        /// </summary>
        public static RoutedCommand OpenRoiCommand
        {
            get { return openRoiCommand; }
        }

        /// <summary>
        /// Gets Routed SaveRoiCommand
        /// </summary>
        public static RoutedCommand SaveRoiCommand
        {
            get { return saveRoiCommand; }
        }

        /// <summary>
        /// Gets Routed SaveRoiAsCommand
        /// </summary>
        public static RoutedCommand SaveRoiAsCommand
        {
            get { return saveRoiAsCommand; }
        }

        /// <summary>
        /// Gets Routed CloseCommand
        /// </summary>
        public static RoutedCommand CloseRoiCommand
        {
            get { return closeRoiCommand; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text displayed in the statusbar info item of the application.
        /// </summary>
        /// <value>Status Info Text</value>
        public string StatusInfo
        {
            get
            {
                return infoText.Text;
            }

            set
            {
                if (!AppContext.BatchMode)
                {
                    if (value == null)
                    {
                        value = string.Empty;
                    }

                    infoText.Text = value;
                    infoText.Visibility = string.IsNullOrEmpty(value) ? Visibility.Hidden : Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text displayed in the statusbar coordinate item of the application.
        /// </summary>
        /// <value>A <see langword="string"/> value.</value>
        /// <remarks>
        /// The text-string usually contains the formatted coordinate-values including units.
        /// E.g. 'x: 123.4 mm y: 567.8 mm'
        /// </remarks>
        public string StatusCoord
        {
            get
            {
                return coordinates.Text;
            }

            set
            {
                if (!AppContext.BatchMode)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        coordinates.Text = string.Empty;
                        coordinates.Visibility = Visibility.Hidden;
                        coordinatesLabel.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        coordinates.Text = value;
                        coordinates.Visibility = Visibility.Visible;
                        coordinatesLabel.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the text displayed in the statusbar intensity item of the application.
        /// </summary>
        /// <value>A <see langword="string"/> value.</value>
        public string StatusIntensity
        {
            get
            {
                return intensity.Text;
            }

            set
            {
                if (!AppContext.BatchMode)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        intensity.Text = string.Empty;
                        intensity.Visibility = Visibility.Hidden;
                        intensityLabel.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        intensity.Text = value;
                        intensity.Visibility = Visibility.Visible;
                        intensityLabel.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the currently active view of the application.
        /// </summary>
        /// <value>The active view as <see cref="IView"/> reference.</value>
        public IView ActiveView
        {
            get { return this.activeView; }
        }

        /// <summary>
        /// Gets or sets the index of the currently selected palette to use for image representation.
        /// </summary>
        public int PaletteIndex
        {
            get { return imagePropsContent.PaletteIndex; }
            set { imagePropsContent.PaletteIndex = value; }
        }

        /// <summary>
        /// Gets or sets the minimum intensity settings currently in effect for the active view.
        /// </summary>
        public IntensitySettings MinIntensitySettings
        {
            get
            {
                return imagePropsContent.MinIntensitySettings;
            }

            set
            {
                imagePropsContent.MinIntensitySettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum intensity settings currently in effect for the active view.
        /// </summary>
        public IntensitySettings MaxIntensitySettings
        {
            get
            {
                return imagePropsContent.MaxIntensitySettings;
            }

            set
            {
                imagePropsContent.MaxIntensitySettings = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        /// <param name="args">Args for main</param> 
        [STAThreadAttribute]
        public static void Main(string[] args)
        {
            FileStream logStream = null;
            TextWriterTraceListener logListener = null;
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // set up logging...
                string logFilepath = Path.Combine(Path.GetTempPath(), Strings.AppName + ".log");
                logStream = new FileStream(logFilepath, FileMode.Append, FileAccess.Write);
                logListener = new TextWriterTraceListener(logStream, Strings.AppName + "-Listener");
                Trace.Listeners.Add(logListener);
                Trace.WriteLine("<--! " + Strings.AppName + " started " + DateTime.Now.ToString(CultureInfo.InvariantCulture) + " -->");
                Trace.Indent();
                Trace.Flush();

                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // usage message
                if (args != null && args.Length > 0 && args[0] == "/?")
                {
                    string caption = Strings.AppName + " - " + Strings.CommandLineUsage;
                    string usage = Strings.AppName + " " + Strings.Switches + "\n\n" + Strings.UsageText;
                    MessageBox.Show(usage, caption);
                    return;
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // parse the argument/parameter strings...
                Dictionary<string, List<string>> argsDict = Util.ParseCommandLine(args);

                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // early initializations
                // Initialize the application
                AppContext.Initialize(argsDict);

                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // get the available PlugIns...
                AppContext.CollectPlugIns();

                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // startup procedure
                var app = new App { ShutdownMode = ShutdownMode.OnMainWindowClose };
                app.InitializeComponent();
                app.Run();

                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // program termination
                Trace.Unindent();
                Trace.WriteLine("<--! " + Strings.AppName + " terminated normally  -->");
                Trace.WriteLine(string.Empty);
                Trace.Flush();
            }
            catch (Exception e)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // outmost exception handler!!
                Util.ReportException(e);
                Trace.IndentLevel = 0;
                Trace.WriteLine("<--! " + Strings.AppName + " terminated with exceptional condition -->");
                Trace.WriteLine(string.Empty);
                Trace.Flush();
            }
            finally
            {
                Trace.Flush();
                Trace.Listeners.Clear();
                if (logListener != null)
                {
                    logListener.Flush();
                    logListener.Close();
                }
                else if (logStream != null)
                {
                    logStream.Flush();
                    logStream.Close();
                }
            }
        }

        /// <summary>
        /// Perform all necessary UI initializations...
        /// </summary>
        public void InitializeUi()
        {
            if (!AppContext.BatchMode)
            {
                this.UseApproximation.IsChecked = AppContext.UseApproximation;
                this.ShowStatusBar.IsChecked = statusBar.Visibility == Visibility.Visible;
                this.UpdateUi();
            }
        }

        /// <summary>
        /// Update the state of the UI.
        /// </summary>
        public void UpdateUi()
        {
            try
            {
                if (!AppContext.BatchMode)
                {
                    bool enable = this.activeView != null;

                    // save as button
                    tbbSaveAs.IsEnabled = enable;

                    // controls in the dockable content panes
                    bool enablePanes = enable && this.activeView is ViewImage;
                    metaContent.UpdateUi(enablePanes);
                    imagePropsContent.UpdateUi(enablePanes);

                    if (enable)
                    {
                        // Set some slider and other control properties
                        var imageView = this.activeView as ViewImage;
                        if (imageView != null)
                        {
                            Imaging imageData = imageView.GetImagingData();

                            if (imageData.ExperimentType == ExperimentType.MS)
                            {
                                {
                                    if (imageData.MassCal != null)
                                    {
                                        this.imagePropsContent.MassCal = new float[imageData.MassCal.Length];
                                        this.imagePropsContent.MassCal = imageData.MassCal;
                                        this.imagePropsContent.CurrentMass = imageData.MassCal[0];
                                    }

                                    this.imagePropsContent.SetMassDetails();
                                }
                            }
                        }

                        imagePropsContent.ViewChanged(this.activeView.ViewExperimentType);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Adds the given view to the mdi gui.
        /// </summary>
        /// <param name="view">The view to be added as a <see cref="IView"/> reference.</param>
        /// <returns>A <see cref="bool"/> value indicating the success.</returns>
        public bool AddView(IView view)
        {
            bool result = false;

            try
            {
                if (view == null)
                {
                    throw new ArgumentNullException("view");
                }

                var viewControl = view as UserControl;
                if (viewControl == null)
                {
                    throw new ArgumentException("View isn't a UserControl or derived thereof");
                }

                viewControl.Visibility = Visibility.Visible;

                var viewContent = new ViewContent(viewControl) { Title = view.Title, ToolTip = view.Document.FileName };

                // add to the list of views and their content containers.
                // follow the insertion scheme of the dockingManager. New views will be insert
                // leftmost in the row of tabs => insert the KeyValuePair to the beginning of the list
                // instead of adding to the end (views.Add(new KeyValuePair<...)
                this.views.Insert(0, new KeyValuePair<IView, ViewContent>(view, viewContent));

                viewContent.Show(dockingManager);
                viewContent.Activate();

                viewControl.Focus();

                result = true;
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }

            return result;
        }

        /// <summary>
        /// Removes the given view from the mdi gui.
        /// </summary>
        /// <param name="view">The view to be removed as a <see cref="IView"/> reference.</param>
        /// <returns>A <see cref="bool"/> value indicating the success.</returns>
        public bool RemoveView(IView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            try
            {
                foreach (DocumentContent docContent in dockingManager.Documents)
                {
                    var viewContent = docContent as ViewContent;
                    if (viewContent != null && viewContent.View == view)
                    {
                        viewContent.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }

            return false;
        }

        /// <summary>
        /// Removes the given view from the mdi gui and closes it.
        /// </summary>
        /// <param name="view">The view to be removed as a <see cref="IView"/> reference.</param>
        /// <returns>A <see cref="bool"/> value indicating the success.</returns>
        public bool CloseView(IView view)
        {
            try
            {
                return this.RemoveView(view);
            }
            catch (Exception e)
            {
                Util.ReportException(e);
                return false;
            }
        }

        /// <summary>
        /// Starts a progressbar action. The specified <paramref name="operation"/> identifier and the progressbar are shown and the
        /// progressbar is made ready to receive values.
        /// </summary>
        /// <param name="operation">A identifier describing the current operation.</param>
        public void ProgressStart(string operation)
        {
            try
            {   // no progress in batch mode =8^O
                if (!AppContext.BatchMode)
                {
                    if (progressOperation.Text != string.Empty)
                    {
                        // TODO -- process the condition of nested calls! For the time being this condition will be ignored (no harm)
                    }

                    this.progressOperation.Text = string.IsNullOrEmpty(operation) ? string.Empty : operation;

                    progressBar.Minimum = 0.0;
                    progressBar.MaxHeight = 100.0;
                    progressBar.Value = 0.0;
                    progressOperation.Visibility = Visibility.Visible;
                    progressBar.Visibility = Visibility.Visible;
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// Sets the Progressbar to show the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to be shown in the progressbar. The range of values is expected to be 0.0 to 100.0</param>
        public void ProgressSetValue(double value)
        {
            try
            {
                // no progress in batch mode =8^O
                if (!AppContext.BatchMode)
                {
                    Debug.Assert(0.0 <= value && value <= 100.0, "Value not in range");

                    // just setting progressBar.Value doesn't work... One has to pump the dispatcher
                    this.Dispatcher.Invoke(
                        this.updateProgressDelegate,
                        System.Windows.Threading.DispatcherPriority.Background,
                        new object[] { RangeBase.ValueProperty, value });
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// Stops the current progressbar action. Clears and hides the progressbar.
        /// </summary>
        public void ProgressClear()
        {
            try
            {
                // no progress in batch mode =8^O
                if (!AppContext.BatchMode)
                {
                    progressOperation.Text = string.Empty;
                    progressBar.Value = 0.0;
                    progressOperation.Visibility = Visibility.Hidden;
                    progressBar.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// OnViewClosing() Implementation
        /// </summary>
        /// <param name="viewContent">ViewContent Item</param>
        /// <param name="e">cancel event argument</param>
        internal void OnViewClosing(ViewContent viewContent, CancelEventArgs e)
        {
            try
            {
                // prevent concurrent thread access
                lock (this.lockObject)
                {
                    e.Cancel = false;
                    if (viewContent != null && viewContent.View != null)
                    {
                        int toBeDeleted = -1;
                        for (int index = 0; index < this.views.Count; index++)
                        {
                            if (viewContent.View == this.views[index].Key)
                            {
                                toBeDeleted = index;
                                if (index == this.views.Count - 1)
                                {
                                    // last in list, get the predecessor
                                    if (index > 0)
                                    {
                                        this.activatePostClose = this.views[index - 1].Value;
                                    }
                                }
                                else
                                {
                                    // anywhere but last, get the successor
                                    this.activatePostClose = this.views[index + 1].Value;
                                }

                                break; // we're done here
                            }
                        }

                        if (toBeDeleted >= 0)
                        {
                            this.views.RemoveAt(toBeDeleted);

                            // notify the world, that a view has been closed.
                            AppContext.FireViewClosed(viewContent.View);
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Process the command line by analyzing the arguments and parameter and act accordingly
        /// </summary>
        /// <returns>A <see cref="bool"/> value indicating if the application should continue after the commandline processing.</returns>
        private bool ProcessCommandLine()
        {
            try
            {
                Dictionary<string, List<string>> args = AppContext.AppArgs;
                if (args != null)
                {
                    var fileNames = new List<string>();
                    bool batchMode = false;
                    string bitmapExt = string.Empty;
                    string outDir = string.Empty;
                    string imagingExt = string.Empty;
                    string bitmapDir = string.Empty;

                    foreach (string key in args.Keys)
                    {
                        if (key == null)
                        {
                            continue;
                        }

                        List<string> valueList;
                        switch (key)
                        {
                            case "":
                                {
                                    // this is are one or more parameters without an argument, that means no matching /xxx or -xxx is found
                                    // in our context this should mean filename(s)
                                    valueList = args[key];
                                    if (valueList != null)
                                    {
                                        for (int index = 0; index < valueList.Count; index++)
                                        {
                                            string value = valueList[index];
                                            if (!string.IsNullOrEmpty(value) && File.Exists(value))
                                            {
                                                fileNames.Add(value);
                                            }
                                        }
                                    }
                                }

                                break;
                            case "batch":
                                {
                                    // the application and subsequent operations specified by this set of command line arguments should run in
                                    // batch mode => silent mode, no UI, quit application after finishing the specified operation(s)
                                    batchMode = true;
                                }

                                break;
                            case "outdir":
                                {
                                    // if there is a 'saveas' or 'bitmap' switch specified than the operations results will be placed in the specified directory
                                    valueList = args[key];
                                    if (valueList != null)
                                    {
                                        if (valueList.Count > 0)
                                        {
                                            // just one outDir should be present...
                                            outDir = valueList[0];
                                            
                                            // validate outDir
                                            if (string.IsNullOrEmpty(outDir) || !Directory.Exists(outDir))
                                            {
                                                outDir = string.Empty;
                                            }
                                        }
                                    }
                                }

                                break;
                            case "bitmapdir":
                                {
                                    // if a bitmap switch is in action and bitmapDir is specified than a copy of the bitmap(s) will be created in the given directory
                                    valueList = args[key];
                                    if (valueList != null)
                                    {
                                        if (valueList.Count > 0)
                                        {
                                            // just one bitmapDir should be present...
                                            bitmapDir = valueList[0];
                                            
                                            // validate bitmapDir
                                            if (string.IsNullOrEmpty(bitmapDir) || !Directory.Exists(bitmapDir))
                                            {
                                                bitmapDir = string.Empty;
                                            }
                                        }
                                    }
                                }

                                break;
                            case "bitmap":
                                {
                                    // try and get the demanded type of bitmap (jpg, tif, png)
                                    bitmapExt = string.Empty;
                                    valueList = args[key];
                                    if (valueList != null)
                                    {
                                        foreach (string value in valueList)
                                        {
                                            if (string.IsNullOrEmpty(value))
                                            {
                                                continue;
                                            }

                                            string format = value.ToLowerInvariant();
                                            switch (format)
                                            {
                                                case "jpg":
                                                    bitmapExt = ".jpg";
                                                    break;
                                                case "tif":
                                                    bitmapExt = ".tif";
                                                    break;
                                                case "png":
                                                    bitmapExt = ".png";
                                                    break;
                                                default:
                                                    if (File.Exists(value))
                                                    {
                                                        fileNames.Add(value);
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                }

                                break;
                            case "saveas":
                                {
                                    // try and get the demanded imaging format (img, imzML)
                                    imagingExt = string.Empty;
                                    valueList = args[key];
                                    if (valueList != null)
                                    {
                                        foreach (string value in valueList)
                                        {
                                            if (string.IsNullOrEmpty(value))
                                            {
                                                continue;
                                            }

                                            string format = value.ToLowerInvariant();
                                            switch (format)
                                            {
                                                case "img":
                                                    imagingExt = ".img";
                                                    break;
                                                case "imzML":
                                                    imagingExt = ".imzML";
                                                    break;
                                                default:
                                                    if (File.Exists(value))
                                                    {
                                                        fileNames.Add(value);
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                }

                                break;
                            case "optwiff":
                                {
                                    AppContext.UseApproximation = true;
                                    this.UseApproximation.IsChecked = true;

                                    valueList = args[key];
                                    if (valueList != null)
                                    {
                                        foreach (string value in valueList)
                                        {
                                            if (string.IsNullOrEmpty(value))
                                            {
                                                continue;
                                            }

                                            if (File.Exists(value))
                                            {
                                                fileNames.Add(value);
                                            }
                                        }
                                    }
                                }

                                break;
                        }
                    }

                    if (!batchMode)
                    {
                        // just open the specified files, if any...
                        foreach (string fileName in fileNames)
                        {
                            AppContext.DocManager.OpenDocument(fileName);
                        }

                        return true; // no batch mode, continue with normal operation
                    }

                    AppContext.BatchMode = true;

                    // batch process the requested operations
                    foreach (string fileName in fileNames)
                    {
                        AppContext.DocManager.BatchProcess(fileName, outDir, imagingExt, bitmapExt, bitmapDir);
                    }

                    return false; // batch mode, shutdown application on return...
                }

                return true;
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }

            return false;
        }

        /// <summary>
        /// This method extract the RoiSubjectItems from the RoiProjects.xml and 
        /// generates a list of RegionOfInerests Object
        /// </summary>
        /// <param name="roiProjectType">Project Type we want to Open</param>
        /// <returns>returns true or false depending on success of opening RoiProjects.xml.</returns>
        private bool GetRoiProjectTypes(string roiProjectType) 
        {
            try
            {
                // 1) Open the RoiProjectTypes.xml file
                string roiprojectTypesXmlFile = System.Windows.Forms.Application.StartupPath;
                roiprojectTypesXmlFile += "\\RoiProjects.xml";
                
                if (string.IsNullOrEmpty(roiprojectTypesXmlFile))
                {
                    MessageBox.Show("Application Path is empty", "New ROI Dataset");
                    return false;
                }

                if (!File.Exists(roiprojectTypesXmlFile))
                {
                    MessageBox.Show("RoiDataSetTypes.xml does not exist!", "New ROI Dataset");
                    return false;
                }

                // 2) Use the ROIProjectTypeItem to populate the combobox
                XDocument roiProjectsXml = XDocument.Load(roiprojectTypesXmlFile);

                XElement root = roiProjectsXml.Root;

                if (root == null || root.Name != "RoiProjects")
                {
                    return false;
                }

                // Extract RoiSubject where Name = roiprojecttype
                foreach (XElement roiSubject in root.Nodes())
                {
                    if (roiSubject == null)
                    {
                        continue;
                    }

                    // retrieve the "roiSubject" of the Application Setting Item
                    XAttribute attrName = roiSubject.Attribute("Name");
                    if (attrName != null && attrName.Value == roiProjectType)
                    {
                        if (attrName.Value == roiProjectType)
                        {
                            foreach (XElement roiSubjectItem in roiSubject.Nodes())
                            {
                                XAttribute attrNameItem = roiSubjectItem.Attribute("Name");
                                XAttribute attrFillColorItem = roiSubjectItem.Attribute("FillColor"); 
                                XAttribute attrStrokeColorItem = roiSubjectItem.Attribute("StrokeColor");

                                if (attrNameItem != null && attrFillColorItem != null && attrStrokeColorItem != null)
                                {
                                    string attrnameitem = attrNameItem.Value;
                                    var fillbrush = (SolidColorBrush)new BrushConverter().ConvertFromString(attrFillColorItem.Value);
                                    var strokebrush = (SolidColorBrush)new BrushConverter().ConvertFromString(attrStrokeColorItem.Value);

                                    var regionOfInterest = new RegionOfInterest
                                        {
                                            Title = attrnameitem,
                                            ImageFilePath = string.Empty,
                                            Area = 0,
                                            MeanIntensity = 0,
                                            RoiFillColor = fillbrush,
                                            RoiStrokeColor = strokebrush
                                        };

                                    this.roiproject.RoiObjects.Add(regionOfInterest);
                                }
                            }
                        }
                    }
                }   
           }
           catch (Exception ex)
           {
               Util.ReportException(ex);
           }

           return true;
        }

        /// <summary>
        /// This methods cleans up the current project objects
        /// </summary>
        private void CleanRoiProject()
        {
            if (this.roiproject != null)
            {
                this.roiproject.RoiProjectName = string.Empty;
                this.roiproject.RoiProjectType = string.Empty;
                this.roiproject.RoiObjects.Clear();
                this.roiproject = null;
                this.roiprojectchangedflag = false;
            }
        }

        /// <summary>
        /// Routine to set up a new ROI project
        /// </summary>
        private void NewRoiProject()
        {
            var roiDataSet = new NewRoiDataSet();

            if (roiDataSet.ShowDialog() == true)
            {
                this.roiproject = new RoiProject
                {
                    RoiProjectType = roiDataSet.RoiProjectType,
                    RoiProjectName = roiDataSet.RoiProjectName
                };

                if (this.roiproject != null)
                {
                    // Add Roi Objects based on
                    if (this.GetRoiProjectTypes(this.roiproject.RoiProjectType))
                    {
                        this.roiProjectWindow.RoiProjectName = this.roiproject.RoiProjectName;
                        this.roiProjectWindow.RoiProjectType = this.roiproject.RoiProjectType;
                        this.roiProjectWindow.RoiObjects = this.roiproject.RoiObjects;
                        this.roiProjectWindow.SetUpRoiProjectWindow();
                    }
                }
            }
        }

        /// <summary>
        /// Routine to Save a Roi Project
        /// </summary>
        private void SaveRoiProject()
        {
            this.roiproject.RoiObjects = this.roiProjectWindow.RoiObjects;

            var saveAsFileDialog = new Core.CommonDialog();
            var filter = new FilterEntry(Strings.XmlFileDescription, Strings.XmlFileExtension);
            saveAsFileDialog.Filters.Add(filter);

            saveAsFileDialog.FileName = this.roiproject.RoiProjectName + Strings.XmlFileExtension;
            
            saveAsFileDialog.ShowSaveAs();
            
            string fileName = saveAsFileDialog.FileName;
            
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
            {
                fileName += Strings.XmlFileExtension;
            }

            // 2) Populate the appropriate ROI object fields
            this.SaveRoiToXml(fileName);
        }

        /// <summary>
        /// Routine to Open a Roi Project
        /// </summary>
        private void OpenRoiProject()
        {
            this.roiproject = new RoiProject();

            // 1) OpenFile Dialog to Open a roi xml file get the filename (and path) with the common dialog
            var loadFileDialog = new Core.CommonDialog();
            var filter = new FilterEntry(Strings.XmlFileDescription, Strings.XmlFileExtension);
            loadFileDialog.Filters.Add(filter);
            loadFileDialog.ShowOpen();

            string fileName = loadFileDialog.FileName;

            if (!string.IsNullOrEmpty(fileName) || File.Exists(fileName))
            {
                // 2) Populate the appropriate ROI object fields
                this.OpenRoiFromXml(fileName);
            }

            // 3) Populate the RoiProject Window
            this.roiProjectWindow.RoiObjects = this.roiproject.RoiObjects;
            this.roiProjectWindow.SetUpRoiProjectWindow();
        }
        
        /// <summary>
        /// Save Roi Project data to a File
        /// </summary>
        /// <param name="xmlFile">File Name to which Roi data is to be saved</param>
        private void SaveRoiToXml(string xmlFile)
        {
            try
            {
                using (Stream xmlStream = new FileStream(xmlFile, FileMode.Create, FileAccess.Write))
                {
                    // set up the xml writer...
                    var xmlSettings = new XmlWriterSettings { Indent = true };
                    XmlWriter xml = XmlWriter.Create(xmlStream, xmlSettings);

                        // start the document
                        xml.WriteStartDocument();

                            xml.WriteComment(" Generated by " + this.roiproject.RoiProjectName + " " + DateTime.Now.ToString(CultureInfo.InvariantCulture) + " ");

                            xml.WriteStartElement("RoiProject");

                            // 1) Project Name and Type
                            xml.WriteAttributeString("Name", this.roiproject.RoiProjectName);
                            xml.WriteAttributeString("ProjectType", this.roiproject.RoiProjectType);

                                // 2) iterate the list of oiObjects and write the item information
                                foreach (RegionOfInterest roiitem in this.roiproject.RoiObjects)
                                {
                                    // 3) Title, Area, MeanIntensity, ImageFilePath
                                    xml.WriteStartElement("RoiProjectItem");
                                        xml.WriteAttributeString("Title", roiitem.Title);
                                        xml.WriteAttributeString("Area", roiitem.Area.ToString(CultureInfo.InvariantCulture));
                                        xml.WriteAttributeString("MeanIntensity", roiitem.MeanIntensity.ToString(CultureInfo.InvariantCulture));
                                        xml.WriteAttributeString("ImageFilePath", roiitem.ImageFilePath);
                                        xml.WriteAttributeString("RoiFillColor", roiitem.RoiFillColor.ToString(CultureInfo.InvariantCulture));
                                        xml.WriteAttributeString("RoiStrokeColor", roiitem.RoiStrokeColor.ToString(CultureInfo.InvariantCulture));
                                        xml.WriteAttributeString("NumberOfPointsInsideRoi", roiitem.NumberOfPointsInsideRoi.ToString(CultureInfo.InvariantCulture));

                                        // 4) Thumb Positions
                                        foreach (Point thumbpoint in roiitem.ThumbPositions)        
                                        {
                                            xml.WriteStartElement("ThumbPoint");
                                            xml.WriteAttributeString("x", thumbpoint.X.ToString(CultureInfo.InvariantCulture));
                                            xml.WriteAttributeString("y", thumbpoint.Y.ToString(CultureInfo.InvariantCulture));
                                            xml.WriteEndElement(); // close the ThumbPoint
                                        }

                                    xml.WriteEndElement(); // close the RoiProjectItem
                                }

                            xml.WriteFullEndElement(); // close the root element (</RoiProject>)
                        xml.WriteEndDocument(); // close the document
                    xml.Close(); // close the xml stream
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// Open File and populate Roi Project Data
        /// </summary>
        /// <param name="roifilename">File Name containing Roi data</param>
        private void OpenRoiFromXml(string roifilename)
        {
            try
            {
                XDocument xmlRoiProjectData = XDocument.Load(roifilename);

                // the root element "<MetaData>"
                XElement root = xmlRoiProjectData.Root;

                if (root == null || root.Name != "RoiProject")
                {
                    return;
                }

                // 1) Project Name and Project type
                XAttribute attrProjectName = root.Attribute("Name");
                if (attrProjectName != null)
                {
                    string roiProjectName = attrProjectName.Value;
                    this.roiproject.RoiProjectName = roiProjectName;
                }

                XAttribute attrProjectType = root.Attribute("Type");
                if (attrProjectType != null)
                {
                    string roiProjectType = attrProjectType.Value;
                    this.roiproject.RoiProjectType = roiProjectType;
                }

                // 3) Iterate of nodes and populate List of ROI Object
                foreach (XElement roiProjectItem in root.Nodes())
                {
                    var regionOfInterest = new RegionOfInterest();

                    if (roiProjectItem == null)
                    {
                        continue;
                    }

                    // 4) Title
                    XAttribute attrTitle = roiProjectItem.Attribute("Title");
                    if (attrTitle != null)
                    {
                        string itemTitle = attrTitle.Value;
                        regionOfInterest.Title = itemTitle;
                    }

                    // 5) Area 
                    XAttribute attrArea = roiProjectItem.Attribute("Area");
                    if (attrArea != null)
                    {
                        string itemArea = attrArea.Value;
                        regionOfInterest.Area = !string.IsNullOrEmpty(itemArea) ? Convert.ToDouble(itemArea) : 0.0;
                    }

                    // 6) MeanIntensity
                    XAttribute attrMeanIntensity = roiProjectItem.Attribute("MeanIntensity");
                    if (attrMeanIntensity != null)
                    {
                        string itemMeanIntensity = attrMeanIntensity.Value;
                        regionOfInterest.MeanIntensity = !string.IsNullOrEmpty(itemMeanIntensity) ? Convert.ToDouble(itemMeanIntensity) : 0.0;
                    }

                    // 7) ImageFilePath
                    XAttribute attrImageFilePath = roiProjectItem.Attribute("ImageFilePath");
                    if (attrImageFilePath != null)
                    {
                        string itemImageFilePath = attrImageFilePath.Value;
                        regionOfInterest.ImageFilePath = itemImageFilePath;
                    }

                    // 8) RoiFillColor
                    XAttribute attrRoiFillColor = roiProjectItem.Attribute("RoiFillColor");
                    if (attrRoiFillColor != null)
                    {
                        var conv = new BrushConverter();
                        string roifillcolor = attrRoiFillColor.Value;
                        var roifillcolorbrush = conv.ConvertFromString(roifillcolor) as SolidColorBrush;
                        regionOfInterest.RoiFillColor = roifillcolorbrush;
                    }

                    // 9) RoiStrokeColor
                    XAttribute attrRoiStrokeColor = roiProjectItem.Attribute("RoiStrokeColor");
                    if (attrRoiStrokeColor != null)
                    {
                        var conv = new BrushConverter();
                        string roistrokecolor = attrRoiFillColor.Value;
                        var roistrokecolorbrush = conv.ConvertFromString(roistrokecolor) as SolidColorBrush;
                        regionOfInterest.RoiStrokeColor = roistrokecolorbrush;
                    }

                    // 10) NumberOfPointsInsideRoi
                    XAttribute attrNumberOfPointsInsideRoi = roiProjectItem.Attribute("NumberOfPointsInsideRoi");
                    if (attrNumberOfPointsInsideRoi != null)
                    {
                        string numberofpointsinsideroi = attrNumberOfPointsInsideRoi.Value;
                        regionOfInterest.NumberOfPointsInsideRoi = Convert.ToInt32(numberofpointsinsideroi); 
                    }
                    
                    // 11) Thumb Positions
                    foreach (XElement thumbItem in roiProjectItem.Nodes())
                    {
                        if (thumbItem == null)
                        {
                            continue;
                        }

                        var thumbpoint = new Point();

                        XAttribute attrThumbX = thumbItem.Attribute("x");
                        if (attrThumbX != null)
                        {
                            string pointX = attrThumbX.Value;
                            thumbpoint.X = Convert.ToDouble(pointX);
                        }

                        XAttribute attrThumbY = thumbItem.Attribute("y");
                        if (attrThumbY != null)
                        {
                            string pointY = attrThumbY.Value;
                            thumbpoint.Y = Convert.ToDouble(pointY);
                        }

                        regionOfInterest.ThumbPositions.Add(thumbpoint);
                    }

                    this.roiproject.RoiObjects.Add(regionOfInterest);
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }


        /// <summary>
        /// Closes the current ROI Project
        /// Checks to see if the current project has been saved
        /// </summary>
        private void CloseRoiProject()
        {
            this.CleanRoiProject();   
        }

        #endregion Methods

        #region Event-Handling

        /// <summary>
        /// Handles the <see cref="ApplicationCommands.Open"/> event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdOpen(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.roiproject != null)
                {
                    AppContext.DocManager.OpenDocument(this.roiproject.RoiObjects);
                }
                else
                {
                    AppContext.DocManager.OpenDocument();
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationCommands.SaveAs"/> event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdSaveAs(object sender, RoutedEventArgs e)
        {
            try
            {
                AppContext.DocManager.DocumentSaveAs();
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Defines if the Save As command can be executed in the current context.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="CanExecuteRoutedEventArgs"/>-object specifying the event.</param>
        private void CanExecuteCmdSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                e.CanExecute = this.activeView != null;
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Handles the exit command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdExit(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Handles the about command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdAbout(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Handles the toggle tooltips command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdToggleToolTips(object sender, RoutedEventArgs e)
        {
            try
            {
                AppContext.ShowToolTips = !AppContext.ShowToolTips;
                if (e != null)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Handles the use approximation command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdUseApproximation(object sender, RoutedEventArgs e)
        {
            try
            {
                AppContext.UseApproximation = this.UseApproximation.IsChecked;
                if (e != null)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Event Handler for NewRoi Menu
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdNewRoi(object sender, RoutedEventArgs e)
        {
            if (this.roiproject == null)
            {
                this.NewRoiProject();
            }
            else
            {
                if (this.roiprojectchangedflag)
                {
                    MessageBoxResult msgboxResult = MessageBox.Show("Current ROI Project has not been saved, would you like to save it?", "New Roi Project", MessageBoxButton.YesNoCancel);

                    if (msgboxResult == MessageBoxResult.Yes)
                    {
                        this.SaveRoiProject();
                        this.CleanRoiProject();
                        this.NewRoiProject();
                    }
                    else if (msgboxResult == MessageBoxResult.No)
                    {
                        this.CleanRoiProject();
                        this.NewRoiProject();
                    }

                    // if Cancel Do nothing, reset roiprojectchangedflag 
                    this.roiprojectchangedflag = false;
                }
            }
        }

        /// <summary>
        /// Event Handler for OpenRoi Menu
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdOpenRoi(object sender, RoutedEventArgs e)
        {
            if (this.roiproject == null)
            {
                this.OpenRoiProject();
            }
            else
            {
                if (this.roiprojectchangedflag)
                {
                    MessageBoxResult msgboxResult = MessageBox.Show("Current ROI Project has not been saved, would you like to save it?", "Open Roi Project", MessageBoxButton.YesNoCancel);

                    if (msgboxResult == MessageBoxResult.Yes)
                    {
                        this.SaveRoiProject();
                        this.CleanRoiProject();
                        this.OpenRoiProject();
                    }
                    else if (msgboxResult == MessageBoxResult.No)
                    {
                        this.CleanRoiProject();
                        this.OpenRoiProject();
                    }

                    // if Cancel Do nothing, reset roiprojectchangedflag 
                    this.roiprojectchangedflag = false;
                }
            }
        }

        /// <summary>
        /// Event Handler for SaveRoi Menu
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdSaveRoi(object sender, RoutedEventArgs e)
        {
            if (this.roiproject != null)
            {
                this.SaveRoiProject();
                this.roiprojectchangedflag = false;
            }
        }

        /// <summary>
        /// Event Handler for SaveRoiAs Menu
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>-object specifying the event.</param>
        private void OnCmdSaveRoiAs(object sender, RoutedEventArgs e)
        {
            // TODO JP 28/05/12
            MessageBox.Show("Feature Not Working Yet! ", "Save Roi Project");
            this.roiprojectchangedflag = false;
        }

        /// <summary>
        /// Closes the current ROI Project
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Args</param>
        private void OnCmdCloseRoi(object sender, RoutedEventArgs e)
        {
            if (this.roiprojectchangedflag)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the current Roi Project changes? ", "Close Roi Project", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    this.SaveRoiProject();
                }
                else if (result == MessageBoxResult.No)
                {
                    this.CloseRoiProject();
                }
            }
            else
            {
                this.CloseRoiProject();
            }

            this.roiprojectchangedflag = false;
        }
        
        /// <summary>
        /// Handles the Closing event.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e"><see cref="CancelEventArgs"/>-object specifying the event.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (AppContext.BatchMode)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }

            e.Cancel = false;
        }

        /// <summary>
        /// Handles the Closed event.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e"><see cref="EventArgs"/>-object specifying the event.</param>
        private void WindowClosed(object sender, EventArgs e)
        {
            try
            {
                // HKEY_CURRENT_USER\software\Novartis\MSImageView
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"software\Novartis\MSImageView"))
                {
                    if (key != null)
                    {
                        AppContext.SaveContext(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Handles the Initialized event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event args</param>
        private void WindowInitialized(object sender, EventArgs e)
        {
            try
            {
                // Initialize all PlugIns:
                AppContext.InitializePlugIns();

                // analyze the parsed command line and react accordingly
                if (!this.ProcessCommandLine())
                {
                    // return from batch mode processing, shutdown the application
                    var app = (App)Application.Current;
                    app.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Handles the Loaded event.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e"><see cref="EventArgs"/>-object specifying the event.</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // HKEY_CURRENT_USER\software\Novartis\MSImageView
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"software\Novartis\MSImageView"))
                {
                    if (key != null)
                    {
                        AppContext.LoadContext(key);
                    }
                }

                imagePropsContent.FillPaletteComboBox();
                this.UpdateUi();
                this.roiprojectchangedflag = false;
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// React on the event that the active view has changed.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event args</param>
        private void DockingManagerActiveContentChanged(object sender, EventArgs e)
        {
            try
            {
                if (dockingManager == (sender as DockingManager))
                {
                    // the active content of our dockingManager has changed. The content could document content (like our ViewContent)
                    // as well as other content from dockable panes (like meta data, properties...)
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// React on the event that the active view has changed.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event args</param>
        private void DockingManagerActiveDocumentChanged(object sender, EventArgs e)
        {
            try
            {
                if (dockingManager == (sender as DockingManager))
                {
                    // the active document content of our dockingManager has changed.
                    // The content should just be a ViewContent-object or nil if the last view has been closed
                    if (dockingManager != null)
                    {
                        var viewContent = dockingManager.ActiveDocument as ViewContent;
                        IView newActiveView = null;
                        if (viewContent != null)
                        {
                            // when the last view is closed a superfluos event is send falsely indicating the existence of a no longer present view.
                            // We rely on the list of views to determine when no more view a present.
                            newActiveView = this.views.Count > 0 ? viewContent.View : null;
                        }

                        if (newActiveView != this.activeView)
                        {
                            this.activeView = newActiveView;

                            // fire the event of view change...
                            AppContext.FireActiveViewChanged(this.activeView);
                            if (metaContent != null)
                            {
                                metaContent.DataContext = this.DataContext;
                            }

                            var imageView = this.activeView as ViewImage;
                            if (imageView != null)
                            {
                                Imaging imageData = imageView.GetImagingData();
                                if (imageData != null && metaContent != null)
                                {
                                    metaContent.DataContext = imageData.MetaData;
                                }
                            }

                            // reflect changes in the UI...
                            this.UpdateUi();
                        }
                 }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// React on a view having been closed.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event args</param>
        private void DockingManagerDocumentClosed(object sender, EventArgs e)
        {
            try
            {
                if (this.activatePostClose != null && this.activatePostClose.IsVisible)
                {
                    // dockingManager.ActiveDocument = activatePostClose;
                    this.activatePostClose.Activate();
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
            finally
            {
                this.activatePostClose = null;
            }
        }

        /// <summary>
        /// React on a view being closed.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Cancel Event Args</param>
        private void DockingManagerDocumentClosing(object sender, CancelEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Show/Hide the statusbar regarding the user request
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Args</param>
        private void ShowStatusBarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.statusBar.Visibility = this.ShowStatusBar.IsChecked ? Visibility.Visible : Visibility.Collapsed;

                if (e != null)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Bring up the intensity graph window
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Args</param>
        private void ViewIntensityGraphClick(object sender, RoutedEventArgs e)
        {
            var data = new float[10];
            var intgraph = new IntensityGraphContent(data);
            intgraph.ShowAsFloatingWindow(dockingManager, false);
        }

        #endregion Event-Handling
    }
}
