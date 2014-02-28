#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="MetaContent.xaml.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 06/12/2011 - Style Cop Updates
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.MSImageView
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using Novartis.Msi.Core;

    /// <summary>
    /// Interaction logic for MetaContent.xaml
    /// </summary>
    public partial class MetaContent
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaContent"/> class 
        /// </summary>
        public MetaContent()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Update the state of the UI.
        /// </summary>
        /// <param name="enable">Should the controls be enabled?</param>
        public void UpdateUi(bool enable)
        {
            menuItemSave.IsEnabled = enable;
            menuItemLoad.IsEnabled = enable;
        }

        /// <summary>
        /// Method call for SaveToFileClick
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Arg</param>
        private void SaveToFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var view = AppContext.ActiveView as ViewImage;
                if (view != null)
                {
                    Imaging imageData = view.GetImagingData();
                    if (imageData != null)
                    {
                        ImageMetaData imageMetaData = imageData.MetaData;
                        if (imageMetaData != null && imageMetaData.Count > 0)
                        {
                            // get the filename (and path) with the common dialog
                            var saveAsFileDialog = new CommonDialog();
                            var filter = new FilterEntry(Strings.XmlFileDescription, Strings.XmlFileExtension);
                            saveAsFileDialog.Filters.Add(filter);
                            if (imageData.ObjectDocument != null)
                            {
                                if (!string.IsNullOrEmpty(imageData.ObjectDocument.FileName))
                                {
                                    string proposal = Path.GetFileNameWithoutExtension(imageData.ObjectDocument.FileName);
                                    proposal += "_meta" + Strings.XmlFileExtension;
                                    saveAsFileDialog.FileName = proposal;
                                }
                            }

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

                            // write the meta data
                            imageMetaData.SaveToXml(fileName);
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
        /// Load From File Click Event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Argument</param>
        private void LoadFromFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var view = AppContext.ActiveView as ViewImage;
                if (view != null)
                {
                    Imaging imageData = view.GetImagingData();
                    if (imageData != null)
                    {
                        ImageMetaData imageMetaData = imageData.MetaData;
                        if (imageMetaData != null)
                        {
                            // get the filename (and path) with the common dialog
                            var loadFileDialog = new CommonDialog();
                            var filter = new FilterEntry(Strings.XmlFileDescription, Strings.XmlFileExtension);
                            loadFileDialog.Filters.Add(filter);
                            loadFileDialog.ShowOpen();
                            string fileName = loadFileDialog.FileName;
                            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                            {
                                return;
                            }

                            // read the meta data
                            imageMetaData.ReadFromXml(fileName);
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
        /// Delete Click Event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Args</param>
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var imageMetaData = this.DataContext as ImageMetaData;
                if (imageMetaData == null)
                {
                    return;
                }

                // first collect the item to be deleted.
                var toDelete = new List<MetaDataItem>();               
                if (listView.SelectedItems != null && listView.SelectedItems.Count > 0)
                {
                    foreach (var item in listView.SelectedItems)
                    {
                        var metaDataItem = item as MetaDataItem;
                        if (metaDataItem != null)
                        {
                            toDelete.Add(metaDataItem);
                        }
                    }
                }

                // now delete... but ask first!
                if (toDelete.Count > 0)
                {
                    string safetyTitle = Strings.ConfirmDeleteTitle;
                    string safetyQuery = Strings.ConfirmDeleteQuery;
                    if (MessageBox.Show(safetyQuery, safetyTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No, MessageBoxOptions.None) == MessageBoxResult.Yes)
                    {
                        foreach (var item in toDelete)
                        {
                            imageMetaData.Remove(item);
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
        /// ListView Selection Change Event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arg</param>
        private void ListViewSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listView.SelectedItems == null)
            {
                menuItemDelete.IsEnabled = false;
            }
            else if (listView.SelectedItems.Count <= 0)
            {
                menuItemDelete.IsEnabled = false;
            }
            else
            {
                menuItemDelete.IsEnabled = true;
            }
        }

        #endregion
    }
}
