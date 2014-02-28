#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="MsExperimentContent.xaml.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System.Windows;
using Novartis.Msi.Core;

namespace Novartis.Msi.MSImageView
{
    /// <summary>
    /// Interaction logic for MSExperimentContent.xaml
    /// </summary>
    public partial class MsExperimentContent
    {
        #region Static Fields

        /// <summary>
        /// Experiment Name
        /// </summary>
        public static readonly DependencyProperty ExperimentNameProperty;

        /// <summary>
        /// Number of scans
        /// </summary>
        public static readonly DependencyProperty NumberOfMassesProperty;

        #endregion Fields

        #region Static Constructor

        /// <summary>
        /// Initializes static members of the <see cref="MsExperimentContent"/> class. 
        /// </summary>
        static MsExperimentContent()
        {
            ExperimentNameProperty = DependencyProperty.Register(
                                                                "ExperimentName",
                                                                 typeof(string),
                                                                 typeof(MsExperimentContent));

            NumberOfMassesProperty = DependencyProperty.Register(
                                                    "NumberOfMasses",
                                                     typeof(string),
                                                     typeof(MsExperimentContent));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MsExperimentContent"/> class 
        /// </summary>
        public MsExperimentContent()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the experiment Name
        /// </summary>
        public string ExperimentName
        {
            get
            {
                return (string)GetValue(ExperimentNameProperty);
            }

            set
            {
                SetValue(ExperimentNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Number of Scans in the Experiment
        /// </summary>
        public string NumberOfMasses
        {
            get
            {
                return (string)GetValue(NumberOfMassesProperty);
            }

            set
            {
                SetValue(NumberOfMassesProperty, value);
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Event Handler for ShowGraph Button Click Event
        /// </summary>
        /// <param name="sender">sender Object</param>
        /// <param name="e">event args</param>
        private void ShowGraphClick(object sender, RoutedEventArgs e)
        {
            /*var intGraph = new IntensityGraph();
            if (intGraph.ShowDialog() == true)
            {
            }*/
        }

        #endregion Events
    }
}
