#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="RegionOfInterest.cs" company="Novartis Pharma AG.">
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
    using System.Windows.Media;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RegionOfInterest"/> class
    /// </summary>
    public class RegionOfInterest
    {
        #region Fields

        /// <summary>
        /// This is the string identifier of the body part of interest.
        /// </summary>
        private string title;

        /// <summary>
        /// The is the area occupied in the region of Interest
        /// </summary>
        private double area;

        /// <summary>
        /// This is the mean intensity of the rio
        /// </summary>
        private double meanintensity;

        /// <summary>
        /// This is the path of the image file onto which this ROI is attached
        /// </summary>
        private string imagefilepath;

        /// <summary>
        /// This is the colleciton of Points for the thumb position of the  
        /// </summary>
        private PointCollection thumbpositions;

        /// <summary>
        /// The image data to be displayed in this view.
        /// </summary>
        private Imaging imagedata;

        /// <summary>
        /// The is the color used to fill the ROI when it is closed
        /// </summary>
        private SolidColorBrush roifillcolor;

        /// <summary>
        /// The is the color used to border of the closed ROI
        /// </summary>
        private SolidColorBrush roistrokecolor;

        /// <summary>
        /// Contains all the points inside ROI
        /// </summary>
        private PointCollection pointsinsideroi;

        /// <summary>
        /// Number of points inside Roi
        /// </summary>
        private int numberofpointsinsideroi;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionOfInterest"/> class
        /// Default constructor
        /// </summary>
        public RegionOfInterest()
        {
            this.thumbpositions = new PointCollection();
            this.pointsinsideroi = new PointCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionOfInterest"/> class
        /// </summary>
        /// <param name="title">Title of the ROI Element</param>
        /// <param name="imagefilepath">Path of the file in which the RIO is created</param>
        public RegionOfInterest(string title, string imagefilepath) 
            : this()
        {
            this.Title = title;
            this.ImageFilePath = imagefilepath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionOfInterest"/> class
        /// Constructor used for Roi Data view 
        /// </summary>
        /// <param name="title">Title of the ROI Element</param>
        /// <param name="area">roi area</param>
        /// <param name="meanintensity"> Roi mean intensity</param>
        /// <param name="numberofpointsinsideroi">number of points inside roi</param>
        public RegionOfInterest(string title, double area, double meanintensity,  int numberofpointsinsideroi)
            : this()
        {
            this.Title = title;
            this.Area = area;
            this.MeanIntensity = meanintensity;
            this.numberofpointsinsideroi = numberofpointsinsideroi;
        }
        
        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or Sets the Title property
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
            }
        }

        /// <summary>
        /// Gets or Sets the Area property
        /// </summary>
        public double Area
        {
            get
            {
                return this.area;
            }

            set
            {
                this.area = value;
            }
        }

        /// <summary>
        /// Gets or Sets the MeanIntensity property
        /// </summary>
        public double MeanIntensity
        {
            get
            {
                return this.meanintensity;
            }

            set
            {
                this.meanintensity = value;
            }
        }

        /// <summary>
        /// Gets or Sets the ImageFilePath property
        /// </summary>
        public string ImageFilePath
        {
            get
            {
                return this.imagefilepath;
            }

            set
            {
                this.imagefilepath = value;
            }
        }

        /// <summary>
        /// Gets or Sets the ThumbPositions property
        /// </summary>
        public PointCollection ThumbPositions
        {
            get
            {
                return this.thumbpositions;
            }

            set
            {
                this.thumbpositions = value;
            }
        }
        
        /// <summary>
        /// Gets or Sets the ImageData property
        /// </summary>
        public Imaging ImageData
        {
            get
            {
                return this.imagedata;
            }

            set
            {
                this.imagedata = value;
            }            
        }

        /// <summary>
        /// Gets or Sets the roifillcolor property
        /// </summary>
        public SolidColorBrush RoiFillColor
        {
            get
            {
                return this.roifillcolor;
            }

            set
            {
                this.roifillcolor = value;
            }
        }

        /// <summary>
        /// Gets or Sets the roistrokecolor property
        /// </summary>
        public SolidColorBrush RoiStrokeColor
        {
            get
            {
                return this.roistrokecolor;
            }

            set
            {
                this.roistrokecolor = value;
            }
        }
        
        /// <summary>
        /// Gets or Sets the pointsinsideroi property
        /// </summary>
        public PointCollection PointsInsideRoi
        {
            get
            {
                return this.pointsinsideroi;
            }

            set
            {
                this.pointsinsideroi = value;
            }            
        }

        /// <summary>
        /// Gets or Sets the numberofpointsinsideroi property
        /// </summary>
        public int NumberOfPointsInsideRoi
        {
            get
            {
                return this.numberofpointsinsideroi;
            }

            set
            {
                this.numberofpointsinsideroi = value;
            }            
        }

        #endregion
    }
}
