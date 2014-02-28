#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="NewRoiDataSet.xaml.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.MSImageView
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Xml.Linq;
    
    using Novartis.Msi.Core;

    /// <summary>
    /// Interaction logic for NewRoiDataSet.xaml
    /// </summary>
    public partial class NewRoiDataSet
    {
        #region Fields

        /// <summary>
        /// Field used to define the name of the RoiProject
        /// </summary>
        private string roiprojectname;
        
        /// <summary>
        /// Field used to define the RoiProject type
        /// </summary>
        private string roiprojecttype;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NewRoiDataSet"/> class  
        /// </summary>
        public NewRoiDataSet()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the RoiProject Name
        /// </summary>
        public string RoiProjectName
        {
            get
            {
                return this.roiprojectname;
            }
        }

        /// <summary>
        /// Gets the RoiProject Type
        /// </summary>
        public string RoiProjectType
        {
            get
            {
                return this.roiprojecttype;
            }
        }

        #endregion Properties
        
        #region Methods

        /// <summary>
        /// This routine populates the DataSet type Combobox with contents
        /// from the RoiDataSetTypes.xml file
        /// </summary>
        private void PopulateComboBox()
        {
            try
            {
                // 1) Open the RoiProjectTypes.xml file
                string roiprojectTypesXmlFile = System.Windows.Forms.Application.StartupPath;
                roiprojectTypesXmlFile += "\\RoiProjects.xml";

                if (string.IsNullOrEmpty(roiprojectTypesXmlFile))
                {
                    MessageBox.Show("Application Path is empty", "New ROI Dataset");
                    return;
                }

                if (!File.Exists(roiprojectTypesXmlFile))
                {
                    MessageBox.Show("RoiDataSetTypes.xml does not exist!", "New ROI Dataset");
                    return;
                }

                // 2) Use the ROIProjectTypeItem to populate the combobox
                XDocument roiDataSetTypesXml = XDocument.Load(roiprojectTypesXmlFile);

                XElement root = roiDataSetTypesXml.Root;

                if (root == null || root.Name != "RoiProjects")
                {
                    return;
                }

                foreach (XElement roiDataSetTypeItem in root.Nodes())
                {
                    if (roiDataSetTypeItem == null)
                    {
                        continue;
                    }

                    // retrieve the "FileSizeLimit" of the Application Setting Item
                    XAttribute attrName = roiDataSetTypeItem.Attribute("Name");
                    if (attrName != null)
                    {
                        string itemvalue = attrName.Value;
                        RoiProjectTypeCombo.Items.Add(itemvalue);
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
        /// Window Loaded - Initialise various things
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // 1) Populate the combobox
            this.PopulateComboBox();
            this.RoiProjectNameTB.Focus();
        }

        /// <summary>
        /// Cancel Button Click Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event</param>
        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Ok Button Click Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event</param>
        private void OkBtnClick(object sender, RoutedEventArgs e)
        {
            // 1) Check to is if roiprojectname != null
            if (string.IsNullOrEmpty(this.roiprojectname))
            {
                MessageBox.Show("The Roi Project Name field is empty, please put in a valid name", "Roi Project Name");
                return;
            }

            // 2) Check to is if roiprojecttype != null
            if (string.IsNullOrEmpty(this.roiprojecttype))
            {
                MessageBox.Show("No RoiProject Type field is selected, please select a valid Data Type", "Roi Project Type");
                return;
            }

            DialogResult = true;
        }

        /// <summary>
        /// Combo Box Selection changed event.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event</param>
        private void RoiProjectTypeComboSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.roiprojecttype = RoiProjectTypeCombo.Items[this.RoiProjectTypeCombo.SelectedIndex].ToString();
        }

        /// <summary>
        /// Text Change Event for DataSetName Text Biox
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event</param>
        private void RoiProjectNameTbTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.roiprojectname = RoiProjectNameTB.Text;
        }

        #endregion Event Handlers
    }
}
