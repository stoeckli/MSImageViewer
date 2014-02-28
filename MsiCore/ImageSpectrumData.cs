#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ImageSpectrumData.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Image Spectrum Data Class
    /// </summary>
    public class ImageSpectrumData : Imaging
    {
        #region Fields

        /// <summary>
        /// The list of <see cref="ImageData"/> objects which this spectrum is composed of.
        /// The count of elements in this list equates to the number of m/z measures for each point of image
        /// </summary>
        private readonly List<ImageData> imageDataList = new List<ImageData>();

        /// <summary>
        /// References the currentImage of this spectrum. This variable refers to the visualization.
        /// </summary>
        private ImageData currentImage;

        /// <summary>
        /// The TIC image of this spectrum measurement
        /// </summary>
        private ImageData imageTic;

        /// <summary>
        /// Mass Calibration
        /// </summary>
        private float[] masscal;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSpectrumData"/> class 
        /// </summary>
        /// <param name="doc">
        /// The <see cref="Document"/> this object belongs to.
        /// </param>
        /// <param name="name">Image Spec Name</param>
        /// <param name="metaData">Meta Data</param>
        /// <param name="massCal">Mass Cal</param>
        /// <param name="imageDataList">List of ImageData Objects</param>
        /// <param name="experimenttype">Experiment Type</param>
        public ImageSpectrumData(Document doc, string name, ImageMetaData metaData, float[] massCal, List<ImageData> imageDataList, ExperimentType experimenttype)
            : base(doc, name, metaData, massCal, experimenttype)
        {
            if (imageDataList == null)
            {
                throw new ArgumentNullException("imageDataList");
            }

            if (imageDataList.Count <= 0)
            {
                throw new ArgumentException("imageDataList contains no elements...");
            }

            this.imageDataList.AddRange(imageDataList);
            
            // JP By Default we set the current Image to -1 the TIC Image.
            this.masscal = massCal;
            this.NumOfMassPoints = imageDataList.Count;
        }

        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the list of <see cref="ImageData"/> objects which this spectrum is composed of.
        /// The count of elements in this list equates to the number of m/z measures for each point of image
        /// </summary>
        public List<ImageData> DataSets
        {
            get
            {
                return this.imageDataList;
            }
        }

        /// <summary>
        /// Gets or sets the references the currentImage of this spectrum. This variable refers to the visualization.
        /// </summary>
        public ImageData ImageTic
        {
            get
            {
                return this.imageTic;
            }

            set
            {
                if (value != null)
                {
                    this.imageTic = value;
                    this.currentImage = this.imageTic;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current Image
        /// </summary>
        public ImageData CurrentImage
        {
            get
            {
                return this.currentImage;
            }

            set
            {
                this.currentImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of Masses in MS Scan
        /// </summary>
        public int NumOfMassPoints { get; set; }

        /// <summary>
        /// Gets the number of data points in x direction
        /// </summary>
        public override int XPoints
        {
            get
            {
                return this.currentImage.XPoints;
            }
        }

        /// <summary>
        /// Gets the number of data points in y direction
        /// </summary>
        public override int YPoints
        {
            get
            {
                return this.currentImage.YPoints;
            }
        }

        /// <summary>
        /// Gets the increment / decrement value for the intensity
        /// </summary>
        public override float IntensityDelta
        {
            get
            {
                return this.currentImage.IntensityDelta;
            }
        }

        /// <summary>
        /// Gets the current value for the maximum intensity
        /// </summary>
        public override float CurrentMaxIntensity
        {
            get
            {
                return this.currentImage.CurrentMaxIntensity;
            }

            set
            {
                this.currentImage.CurrentMaxIntensity = value;
            }
        }

        /// <summary>
        /// Gets the current value for the minimum intensity
        /// </summary>
        public override float CurrentMinIntensity
        {
            get
            {
                return this.currentImage.CurrentMinIntensity;
            }

            set
            {
                this.currentImage.CurrentMinIntensity = value;
            }
        }

        /// <summary>
        /// Gets the value for the maximum intensity
        /// </summary>
        public override float MaxIntensity
        {
            get
            {
                return this.currentImage.MaxIntensity;
            }
        }

        /// <summary>
        /// Gets the value for the minimum intensity
        /// </summary>
        public override float MinIntensity
        {
            get
            {
                return this.currentImage.MinIntensity;
            }
        }

        /// <summary>
        /// The value for the maximum intensity in the image.
        /// </summary>
        /// <remarks>This value may be larger than the maximum intensity from the device data.</remarks>
        public override float ImageMaxIntensity
        {
            get
            {
                return this.currentImage.ImageMaxIntensity;
            }

            set
            {
                this.currentImage.ImageMaxIntensity = value;
            }
        }

        /// <summary>
        /// The index in the <see cref="Palettes"/> list of the BitmapPalette which is used.
        /// </summary>
        public override int PaletteIndex
        {
            get
            {
                return this.currentImage.PaletteIndex;
            }

            set
            {
                this.currentImage.PaletteIndex = value;
            }
        }

        /// <summary>
        /// Extent in x direction.
        /// </summary>
        public override float Dx
        {
            get
            {
                return this.currentImage.Dx;
            }
        }

        /// <summary>
        /// Extent in y direction.
        /// </summary>
        public override float Dy
        {
            get
            {
                return this.currentImage.Dy;
            }
        }

        /// <summary>
        /// Gets or sets the Mass Calibration array
        /// </summary>
        public new float[] MassCal
        {
            get
            {
                return this.masscal;
            }

            set
            {
                this.masscal = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Inserts this object's image-representation in a image-view.
        /// </summary>
        /// <param name="viewCtrl">
        /// ViewController in which the image-representation is to be inserted.
        /// </param>
        /// <exception cref="NotImplementedException">Insert Image Rep</exception>
        public override void InsertImageRep(ViewImageController viewCtrl)
        {
            throw new NotImplementedException("Insert Image Rep Error");
        }

        /// <summary>
        /// Add one image data object to this spectrum data object.
        /// </summary>
        /// <param name="imageData">Image Data Object</param>
        public void Add(ImageData imageData)
        {
            if (imageData == null)
            {
                throw new ArgumentNullException("imageData");
            }

            this.imageDataList.Add(imageData);
        }

        /// <summary>
        /// Gets the list of all bitmaps from this spectrum image data object.
        /// </summary>
        /// <returns>A BitmapSourceList containing the bitmaps.</returns>
        public override BitmapSourceList GetBitmaps()
        {
            // TODO -- here a BitmapSourceList with ALL bitmaps of all image data objects contained in this spectrum should be returned.
            return this.currentImage.GetBitmaps();
        }

        /// <summary>
        /// This routine generates all the bitmaps for the ImageData Objects in the imageDataList
        /// This routine should only be called for MS scans and only once.
        /// </summary>
        public void GetBitmapsForAllScans()
        {
            ImageData actImage;

            AppContext.ProgressStart("loading Image List Bitmaps...");

            for (int i = 0; i < this.imageDataList.Count; i++)
            {
                actImage = this.imageDataList[i];
                actImage.GetBitmaps();
                AppContext.ProgressSetValue((100.0 * i) / this.imageDataList.Count);
            }

            AppContext.ProgressClear();
        }

        #endregion Methods
    }
}
