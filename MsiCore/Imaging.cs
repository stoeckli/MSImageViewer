#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="Imaging.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

using System;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Common baseclass of Image related baseobjects
    /// </summary>
    public abstract class Imaging : BaseObject, IViewObject
    {
        #region Fields

        /// <summary>
        /// Image Name
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Image Meta Data
        /// </summary>
        private readonly ImageMetaData metaData;

        /// <summary>
        /// Palette Index
        /// </summary>
        private int paletteIndex;

        /// <summary>
        /// Mass Calibration
        /// </summary>
        private float[] masscal;

        /// <summary>
        /// indicating if the object has been modified.
        /// </summary>
        private bool dirty = true;

        #endregion Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Imaging"/> class 
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> this object belongs to.</param>
        /// <param name="name">The name of this instance.</param>
        /// <param name="metaData">The instances metaData.</param>
        /// <param name="massCal">Mass Calibration</param>
        /// <param name="exptype">Experiment Type</param>
        protected Imaging(Document doc, string name, ImageMetaData metaData, float[] massCal, ExperimentType exptype)
            : base(doc)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name is null or empty");
            }

            if (metaData == null)
            {
                throw new ArgumentNullException("metaData");
            }

            this.name = name;
            this.metaData = metaData;
            this.masscal = massCal;

            this.paletteIndex = AppContext.Application.PaletteIndex;
            this.ExperimentType = exptype;
        }

        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the Name of the Image
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the metaData
        /// </summary>
        public virtual ImageMetaData MetaData
        {
            get
            {
                return this.metaData;
            }
        }

        /// <summary>
        /// Gets or sets the Palette index in the list of the BitmapPalette which is used.
        /// </summary>
        public virtual int PaletteIndex
        {
            get
            {
                return this.paletteIndex;
            }

            set
            {
                this.dirty = true;
                this.paletteIndex = value;
            }
        }

        /// <summary>
        /// Gets the number of data points in x direction
        /// </summary>
        public abstract int XPoints
        {
            get;
        }

        /// <summary>
        /// Gets the number of data points in y direction
        /// </summary>
        public abstract int YPoints
        {
            get;
        }

        /// <summary>
        /// Gets The increment / decrement value for the intensity
        /// </summary>
        public abstract float IntensityDelta
        {
            get;
        }

        /// <summary>
        /// Gets or sets the current value for the maximum intensity
        /// </summary>
        public abstract float CurrentMaxIntensity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current value for the minimum intensity
        /// </summary>
        public abstract float CurrentMinIntensity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value for the maximum intensity
        /// </summary>
        public abstract float MaxIntensity
        {
            get;
        }

        /// <summary>
        /// Gets the value for the minimum intensity
        /// </summary>
        public abstract float MinIntensity
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value for the maximum intensity in the image.
        /// </summary>
        /// <remarks>This value may be larger than the maximum intensity from the device data.</remarks>
        public abstract float ImageMaxIntensity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the x-extend of one pixel in mm.
        /// </summary>
        public abstract float Dx
        {
            get;
        }

        /// <summary>
        /// Gets the the y-extend of one pixel in mm.
        /// </summary>
        public abstract float Dy
        {
            get;
        }

        /// <summary>
        /// Gets or sets the viewexperimenttype
        /// </summary>
        public ExperimentType ExperimentType { get; set; }

        /// <summary>
        /// Gets the Mass Calibration array
        /// </summary>
        public float[] MassCal
        {
            get
            {
                return this.masscal;
            }
        }

        /// <summary>
        /// Gets or sets the value for the dirty flag
        /// </summary>
        protected bool Dirty
        {
            get
            {
                return this.dirty;
            }

            set
            {
                this.dirty = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the Bitmaps
        /// </summary>
        /// <returns>return a list of Bitmaps</returns>
        public abstract BitmapSourceList GetBitmaps();

        #endregion Methods
    }
}
