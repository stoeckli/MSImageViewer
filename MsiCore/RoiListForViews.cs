#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="RoiListForViews.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel 24/06/2012 - New ultilty class
//  
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Simple Utility Class to help sort some objects for some views
    /// </summary>
    public class RoiListForViews
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RoiListForViews"/> class 
        /// </summary>
        public RoiListForViews()
        {
            this.ListOfRois = new List<RegionOfInterest>();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// List of Region of Interest
        /// </summary>
        public List<RegionOfInterest> ListOfRois { get; set; }

        /// <summary>
        /// Path of the Image File
        /// </summary>
        public string ImagePath { get; set; }

        #endregion Properties
    }
}
