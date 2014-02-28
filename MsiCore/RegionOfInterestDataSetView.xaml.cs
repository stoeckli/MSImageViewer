#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="RegionOfInterestDataSetView.xaml.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel - Style Cop Updates, Addition of ROI functions
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
   using System.Collections.ObjectModel;
    
    /// <summary>
    /// Interaction logic for RegionOfInterestDataSetView.xaml
    /// </summary>
    public partial class RegionOfInterestDataSetView
    {
        #region Fields
        
        /// <summary>
        /// Contains a list of ROI objects
        /// </summary>
        private ObservableCollection<RegionOfInterest> observableroiobjects = new ObservableCollection<RegionOfInterest>();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionOfInterestDataSetView"/> class 
        /// </summary>
        public RegionOfInterestDataSetView()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// Gets or Sets the list of roiObjects
        /// </summary>
        public ObservableCollection<RegionOfInterest> ObservableRoiObjects
        {
            get
            {
                return this.observableroiobjects;
            }

            set
            {
                this.observableroiobjects = value;
            }
        }

        #endregion Property

        #region Event Handlers

        #endregion Event Handlers
    }
}
