#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="RoiProjectWindow.xaml.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel 09/06/2012 - New Window
//  
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.MSImageView
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    
    using Novartis.Msi.Core;

    /// <summary>
    /// Interaction logic for RoiProjectWindow.xaml
    /// </summary>
    public partial class RoiProjectWindow
    {
        #region Fields

        /// <summary>
        /// Roi project name
        /// </summary>
        private string roiprojectname;

        /// <summary>
        /// Roi Project Dataset Type
        /// </summary>
        private string roiprojecttype;

        /// <summary>
        /// Contains a list of ROI objects
        /// </summary>
        private List<RegionOfInterest> roiObjects;

        /// <summary>
        /// Current selected image path
        /// </summary>
        private string currentimagepath;
        
        /// <summary>
        /// Current selected Area
        /// </summary>
        private string currentarea;

        /// <summary>
        /// Current selected MeanIntensity
        /// </summary>
        private string currentmeanintensity;

        /// <summary>
        /// Holder for current selected Roi
        /// </summary>
        private RegionOfInterest currentRoi;

        /// <summary>
        /// Flag to indicated if project has been udapted
        /// </summary>
        private bool roiprojectchangedflag;
        
        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RoiProjectWindow"/> class 
        /// </summary>
        public RoiProjectWindow()
        {
            this.roiprojectname = string.Empty;
            this.roiprojecttype = string.Empty;
            this.roiObjects = new List<RegionOfInterest>();
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or Sets the roiprojectname
        /// </summary>
        public string RoiProjectName
        {
            get
            {
                return this.roiprojectname;
            }

            set
            {
                this.roiprojectname = value;
            }
        }

        /// <summary>
        /// Gets or Sets the roiprojecttype
        /// </summary>
        public string RoiProjectType
        {
            get
            {
                return this.roiprojecttype;
            }

            set
            {
                this.roiprojecttype = value;
            }
        }

        /// <summary>
        /// Gets or Sets the list of roiObjects
        /// </summary>
        public List<RegionOfInterest> RoiObjects
        {
            get
            {
                return this.roiObjects;
            }

            set
            {
                this.roiObjects = value;
            }            
        }

        /// <summary>
        /// Gets or Sets the currentimagepath
        /// </summary>
        public string CurrentImagePath
        {
            get
            {
                return this.currentimagepath;
            }

            set
            {
                this.currentimagepath = value;
            }
        }

        /// <summary>
        /// Gets or Sets the currentarea
        /// </summary>
        public string CurrentArea 
        {
            get
            {
                return this.currentarea;
            }

            set
            {
                this.currentarea = value;
            }
        }

        /// <summary>
        /// Gets or Sets the currentmeanintensity
        /// </summary>
        public string CurrentMeanIntensity
        {
            get
            {
                return this.currentmeanintensity;
            }

            set
            {
                this.currentmeanintensity = value;
            }
        }

        /// <summary>
        /// Gets or Sets the currentmeanintensity
        /// </summary>
        public bool ProjectChangedFlag
        {
            get
            {
                return this.roiprojectchangedflag;
            }

            set
            {
                this.roiprojectchangedflag = value;
            }
        }
  
        #endregion Properties

        #region Methods

        /// <summary>
        /// Method used to set up the Roi Project Window (tab)
        /// </summary>
        public void SetUpRoiProjectWindow()
        {
            this.roiprojectchangedflag = false;
            this.PopulateNewRoiProjectTree();
            this.EnableControls(true);
        }

        /// <summary>
        /// Routine used to enable or disable controls
        /// </summary>
        /// <param name="enable">flag to indicate whether to enable or disable controls</param>
        private void EnableControls(bool enable)
        {
            ImagePathBtn.IsEnabled = enable;
            ImagePathTB.IsEnabled = enable;
            AreaTB.IsEnabled = enable;
            MeanIntTB.IsEnabled = enable;
            ViewImagesAllBtn.IsEnabled = enable;
        }

        /// <summary>
        /// Method to populate the Project tree with a new Roi
        /// </summary>
        private void PopulateNewRoiProjectTree()
        {
          try
            {
                if (this.roiObjects != null)
                {
                    var headnodeToAdd = new TreeViewItem
                        {
                            Header = "Project: " + this.RoiProjectName,
                            FontSize = 14,
                            Foreground = Brushes.Black,
                            FontWeight = FontWeights.Bold
                        };

                    RoiProjectTreeView.Items.Add(headnodeToAdd);

                    foreach (RegionOfInterest roiSubject in this.roiObjects)
                    {
                        string roisubject = roiSubject.Title;
                        var roisubjectitem = new TreeViewItem { Header = roisubject, Foreground = roiSubject.RoiFillColor, FontSize = 12 };
                        
                        RoiProjectTreeView.Items.Add(roisubjectitem);
                    }
                }
            }
           catch (Exception ex)
           {
               Util.ReportException(ex);
           }
        }

        /// <summary>
        /// This method opens all the Unique Images. It does this by going through 
        /// the list of ROI and finding unique Images. For each Unique image it associates
        /// all the ROI's attached to this image.
        /// Step 1 Find all the unique paths in roiObjects
        /// Step 2 Set the ImageData for each ROI (from open doc)
        /// Step 3 For each generated Imageview set ROI Objects
        /// </summary>
        private void OpenAllViewImages()
        {
            var roilistforview = new List<RoiListForViews>();

            // Step 1 Find all the unique paths in roiObjects
            foreach (RegionOfInterest regionOfInterest in this.roiObjects)
            {
                string imagePath = regionOfInterest.ImageFilePath;

                var roilistforviewitemfound = roilistforview.Find(bk => bk.ImagePath == imagePath);

                // Check to see if we already have the path
                if (roilistforviewitemfound != null && roilistforviewitemfound.ImagePath == imagePath)
                {
                    // Find element that contains ImagePath and add the regionOfInterest to it's list 
                    roilistforviewitemfound.ListOfRois.Add(regionOfInterest);
                    continue;
                }

                var roilistforviewitem = new RoiListForViews { ImagePath = imagePath };
                roilistforviewitem.ListOfRois.Add(regionOfInterest);
                roilistforview.Add(roilistforviewitem);
            }
            
            // Step 2 For each generated Imageview set the ROI Objects
            try
            {
                foreach (RoiListForViews roiListforviewsobject in roilistforview)
                {
                    if (!string.IsNullOrEmpty(roiListforviewsobject.ImagePath) || File.Exists(roiListforviewsobject.ImagePath))
                    {
                        // Step 3 Set the ImageData for each ROI (from open doc)
                        AppContext.DocManager.OpenDocument(roiListforviewsobject.ImagePath, roiListforviewsobject.ListOfRois);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        #endregion Methods

        #region Event Handlers

        /// <summary>
        /// Event Handler for Loaded Window
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event args</param>
        private void RoiProjectWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.EnableControls(false);
            this.currentimagepath = string.Empty;
            this.currentarea = string.Empty;
            this.currentmeanintensity = string.Empty;
        }

        /// <summary>
        /// Event Handler for clicking image path button
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event args</param>
        private void ImagePathBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1) Open File Dialog with wiff/img filter
                string fileName = string.Empty;
                var openFileDialog = new CommonDialog();

                if (AppContext.SpecFileLoaders != null && AppContext.SpecFileLoaders.Count > 0)
                {
                    foreach (ISpecFileLoader loader in AppContext.SpecFileLoaders)
                    {
                        FileTypeDescriptorList supportedFileTypes = loader.SupportedFileTypes;

                        if (supportedFileTypes != null)
                        {
                            foreach (FileTypeDescriptor fileType in supportedFileTypes)
                            {
                                if (fileType.Extensions != null)
                                {
                                    foreach (string extension in fileType.Extensions)
                                    {
                                        var filter = new FilterEntry(fileType.Description, extension);
                                        openFileDialog.Filters.Add(filter);
                                    }
                                }
                            }
                        }
                    }

                    // proceed if at least one loader is ready to accept a file...
                    if (openFileDialog.Filters.Count > 0)
                    {
                        // Add the all files filter
                        openFileDialog.Filters.Add(new FilterEntry(Strings.AllFilesDesc, Strings.AllFilesExt));
                        openFileDialog.ShowOpen();
                        fileName = openFileDialog.FileName;
                    }
                }
            
                if (!string.IsNullOrEmpty(fileName) || File.Exists(fileName))
                {
                    // 2) Update
                    ImagePathTB.Text = fileName;
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Tree view selected Item Change
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event args</param>
        private void RoiProjectTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = (TreeViewItem)this.RoiProjectTreeView.SelectedItem;
            var selectedItemHeader = (string)selectedItem.Header;

            this.currentRoi = this.roiObjects.Find(roi => roi.Title == selectedItemHeader);

            this.ImagePathTB.Text = this.currentRoi.ImageFilePath;
            this.AreaTB.Text = this.currentRoi.Area.ToString(CultureInfo.InvariantCulture);
            this.MeanIntTB.Text = this.currentRoi.MeanIntensity.ToString(CultureInfo.InvariantCulture);
            
            this.UpdateLayout();
        }

        /// <summary>
        /// ImagePath Textbox change event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event args</param>
        private void ImagePathTbTextChanged(object sender, TextChangedEventArgs e)
        {
            this.currentRoi.ImageFilePath = this.ImagePathTB.Text;
            this.roiObjects.Find(roi => roi.Title == this.currentRoi.Title).ImageFilePath = this.currentRoi.ImageFilePath;
            this.roiprojectchangedflag = true;
        }

        /// <summary>
        /// Area Textbox change event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event args</param>
        private void AreaTbTextChanged(object sender, TextChangedEventArgs e)
        {
             this.currentRoi.Area = double.Parse(this.AreaTB.Text);
             this.roiObjects.Find(roi => roi.Title == this.currentRoi.Title).Area = double.Parse(this.AreaTB.Text);
             this.roiprojectchangedflag = true;
        }

        /// <summary>
        /// MeanIntensity Textbox change event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event args</param>
        private void MeanIntTbTextChanged(object sender, TextChangedEventArgs e)
        {
            this.currentRoi.MeanIntensity = double.Parse(this.MeanIntTB.Text);
            this.roiObjects.Find(roi => roi.Title == this.currentRoi.Title).MeanIntensity = double.Parse(this.MeanIntTB.Text);
            this.roiprojectchangedflag = true;
        }

        /// <summary>
        /// This event handler Opens all unique Images (in Views) and assigns them the appropriate ROI's
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">routed event args</param>
        private void ViewAllImagesBtnClick(object sender, RoutedEventArgs e)
        {
            ViewImagesAllBtn.IsEnabled = false;
            this.Cursor = Cursors.Wait;
            this.OpenAllViewImages();
            this.Cursor = Cursors.Arrow;
            ViewImagesAllBtn.IsEnabled = true;
        }

        #endregion Event Handlers
    }
}