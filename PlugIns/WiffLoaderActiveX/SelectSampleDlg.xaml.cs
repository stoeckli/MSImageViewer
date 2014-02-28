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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// Interaction logic for SelectSampleDlg.xaml
    /// </summary>
    public partial class SelectSampleDlg : Window
    {
        private int selectedSample = 1; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleNames"></param>
        public SelectSampleDlg(string[] sampleNames)
        {
            if (sampleNames == null)
                throw new ArgumentNullException("sampleNames");

            InitializeComponent();

            //foreach (string sampleName in sampleNames)
            for (int i = 1; i <= sampleNames.Length; i++)
            {
                string sampleName = i.ToString() + " - " + sampleNames[i - 1];

                sampleComboBox.Items.Add(sampleName);
            }

            sampleComboBox.SelectedIndex = selectedSample - 1;
        }

        /// <summary>
        /// Gets the index of the sample selected by the user.
        /// </summary>
        public int SelectedSampleIndex
        {
            get { return selectedSample; }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // Dialog box OKed
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Dialog box canceled
            this.DialogResult = false;
        }

        private void sampleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSample = sampleComboBox.SelectedIndex + 1;
        }
    }
}
