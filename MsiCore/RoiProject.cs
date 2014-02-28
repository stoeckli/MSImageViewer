#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="RoiProject.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoiProject"/> class
    /// </summary>
    public class RoiProject
    {
        #region Fields

        /// <summary>
        /// Name of Roi Project
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

        #endregion Fields

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RoiProject"/> class
        /// Default constructor
        /// </summary>
        public RoiProject()
        {
            this.roiObjects = new List<RegionOfInterest>();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or Sets the roiprojectname property
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
        /// Gets or Sets the roiprojectname property
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

        #endregion Properties
    }
}