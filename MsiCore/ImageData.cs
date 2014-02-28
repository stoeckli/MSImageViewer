#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ImageData.cs" company="Novartis Pharma AG.">
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
#endregion Copyright © 2012:) Novartis AG

using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// This class is for the Imagedata part of the imaging class
    /// </summary>
    public class ImageData : Imaging
    {
        #region Fields

        /// <summary>
        /// Extend in x direction
        /// </summary>
        private readonly float dx;

        /// <summary>
        /// Extend in y direction
        /// </summary>
        private readonly float dy;

        /// <summary>
        /// Intensity Delta
        /// </summary>
        private readonly float intensityDelta;

        /// <summary>
        /// Data array
        /// </summary>
        private readonly float[][] data;

        /// <summary>
        /// Minimum Intensity
        /// </summary>
        private readonly float minIntensity;

        /// <summary>
        /// Maximum Intensity
        /// </summary>
        private readonly float maxIntensity;

        /// <summary>
        /// Mean Value
        /// </summary>
        private readonly float meanValue;

        /// <summary>
        /// Median Value
        /// </summary>
        private readonly float medianValue;

        /// <summary>
        /// Max Intensity of Image 
        /// </summary>
        private float imageMaxIntensity;

        /// <summary>
        /// Current Max Intensity
        /// </summary>
        private float currentMaxIntensity;

        /// <summary>
        /// Current Min Intensity
        /// </summary>
        private float currentMinIntensity;

        /// <summary>
        /// Bitmap assocaited to this object
        /// </summary>
        private BitmapSource bitmap;

        /// <summary>
        /// Mass Calibration
        /// </summary>
        private float[] masscal;

        #endregion Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageData"/> class 
        /// </summary>
        /// <param name="doc">
        /// The <see cref="Document"/> this object belongs to.
        /// </param>
        /// <param name="data">Data Array</param>
        /// <param name="name">Image data Name</param>
        /// <param name="metaData">Meta Data</param>
        /// <param name="minIntensity">Min Intensity</param>
        /// <param name="maxIntensity">Max Intensity</param>
        /// <param name="meanValue">Mean Value</param>
        /// <param name="medianValue">Median Value</param>
        /// <param name="dx">point in x</param>
        /// <param name="dy">point in y</param>
        /// <param name="massCal">Mass Calibration</param>
        /// <param name="experimenttype">Experiment Type</param>
        public ImageData(Document doc, float[][] data, string name, ImageMetaData metaData, float minIntensity, float maxIntensity, float meanValue, float medianValue, float dx, float dy, float[] massCal, ExperimentType experimenttype)
            : base(doc, name, metaData, massCal, experimenttype)
        {
            this.data = data;
            this.minIntensity = minIntensity;
            this.maxIntensity = maxIntensity;
            this.imageMaxIntensity = maxIntensity;
            this.meanValue = meanValue;
            this.medianValue = medianValue;
            this.masscal = massCal;

            // try to adjust the intensities which will translate into brightness of the image (for grayscale gradient palette)
            this.currentMaxIntensity = this.imageMaxIntensity;

            if ((2.0f * medianValue > 0.0f) && (2.0f * medianValue > minIntensity))
            {
                // set to 200% of the median...
                this.currentMaxIntensity = 2.0f * medianValue;
            }
            else
            {
                // if median is near zero, the image is rather dark and we try the mean value...
                if (!Util.NearZero(this.imageMaxIntensity) && !Util.NearZero(meanValue))
                {
                    this.currentMaxIntensity = meanValue * 2.0f;
                }
            }

            this.currentMinIntensity = Math.Min(0.0f, minIntensity);
            if (this.currentMaxIntensity <= this.currentMinIntensity)
            {
                this.currentMaxIntensity = this.currentMinIntensity * 2.0f;
            }

            if (this.imageMaxIntensity / Math.Max(100.0f, this.currentMaxIntensity) > 10.0f)
            {
                this.imageMaxIntensity = 10.0f * this.currentMaxIntensity;
            }

            this.dx = dx;
            this.dy = dy;
            this.intensityDelta = this.currentMaxIntensity / 10.0f;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the intensity (data) values.
        /// </summary>
        public float[][] Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// Gets or sets the current value for the maximum intensity
        /// </summary>
        public override float CurrentMaxIntensity
        {
            get
            {
                return this.currentMaxIntensity;
            }

            set
            {
                this.currentMaxIntensity = value;
                this.imageMaxIntensity = Math.Max(this.imageMaxIntensity, this.currentMaxIntensity);
                Dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the current value for the minimum intensity
        /// </summary>
        public override float CurrentMinIntensity
        {
            get
            {
                return this.currentMinIntensity;
            }

            set
            {
                this.currentMinIntensity = value;
                this.imageMaxIntensity = Math.Max(this.imageMaxIntensity, this.currentMinIntensity);
                Dirty = true;
            }
        }

        /// <summary>
        /// Gets the value for the maximum intensity
        /// </summary>
        public override float MaxIntensity
        {
            get
            {
                return this.maxIntensity;
            }
        }

        /// <summary>
        /// Gets the value for the minimum intensity
        /// </summary>
        public override float MinIntensity
        {
            get
            {
                return this.minIntensity;
            }
        }

        /// <summary>
        /// Gets or set the value for the maximum intensity in the image.
        /// </summary>
        /// <remarks>This value may be larger than the maximum intensity from the device data.</remarks>
        public override float ImageMaxIntensity
        {
            get
            {
                return this.imageMaxIntensity;
            }

            set
            {
                this.imageMaxIntensity = value;
            }
        }

        /// <summary>
        /// Gets or sets the delta (step width) when increasing/decreasing the intensity.
        /// </summary>
        public override float IntensityDelta
        {
            get
            {
                return this.intensityDelta;
            }
        }

        /// <summary>
        /// Gets the mean value of all intensities of this image data.
        /// </summary>
        public float MeanValue
        {
            get
            {
                return this.meanValue;
            }
        }

        /// <summary>
        /// Gets the median value of all intensities of this image data.
        /// </summary>
        public float MedianValue
        {
            get
            {
                return this.medianValue;
            }
        }

        /// <summary>
        /// Gets the Extend in x direction.
        /// </summary>
        public override float Dx
        {
            get
            {
                return this.dx;
            }
        }

        /// <summary>
        /// Gets the extend in y direction.
        /// </summary>
        public override float Dy
        {
            get
            {
                return this.dy;
            }
        }

        /// <summary>
        /// The number of data points in x direction
        /// </summary>
        public override int XPoints
        {
            get
            {
                return this.data.Length;
            }
        }

        /// <summary>
        /// The number of data points in y direction
        /// </summary>
        public override int YPoints
        {
            get
            {
                return this.data[0].Length;
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
        /// <exception cref="NotImplementedException">Insert Image Rep Error</exception>
        public override void InsertImageRep(ViewImageController viewCtrl)
        {
            throw new NotImplementedException("Insert Image Rep Error");
        }

        /// <summary>
        /// Gets the bitmap from this image data.
        /// </summary>
        /// <returns>A BitmapSource instance.</returns>
        public BitmapSource GetBitmap()
        {
            if (this.bitmap == null || this.Dirty)
            {
                // create the BitmapSource from the image data
                this.CreateBitmap();
                this.Dirty = false;
            }

            return this.bitmap;
        }

        /// <summary>
        /// Gets the list of all bitmaps from this image data object.
        /// Just one bitmap in this case.
        /// </summary>
        /// <returns>A BitmapSourceList containing the bitmap.</returns>
        public override BitmapSourceList GetBitmaps()
        {
            var bitmaps = new BitmapSourceList();
            var locBbitmap = this.GetBitmap();

            if (locBbitmap != null)
            {
                bitmaps.Add(locBbitmap);
            }

            return bitmaps;
        }

        /// <summary>
        /// Sets Pixels Method
        /// this method will exclusively work for an intended bitmap with 8 BitsPerPixe colorinformation!!
        /// </summary>
        /// <param name="bits">num of bits</param>
        /// <param name="x">extend in x</param>
        /// <param name="y">extend in y</param>
        /// <param name="stride">stride param</param>
        /// <param name="c">colour param</param>
        /// <remarks>this method will exclusively work for an intended bitmap with 8 BitsPerPixel colorinformation!!</remarks>
        private void SetPixel(byte[] bits, int x, int y, int stride, byte c)
        {
            bits[x + (y * stride)] = c;
        }

        /// <summary>
        /// Creates a Bitmap
        /// uses data[x][y] to define the intensity points.
        /// </summary>
        private void CreateBitmap()
        {
            if (this.data != null && this.data[0] != null)
            {
                PixelFormat pixelFormat = PixelFormats.Indexed8;
                int width = this.data.Length;
                int height = this.data[0].Length;
                int stride = width * (pixelFormat.BitsPerPixel / 8);

                // allocate the bitmap's bits
                var bits = new byte[height * stride];

                // populate the bits with colorvalues reflecting the intensities from data...
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var pixelColor = (byte)Math.Min(Math.Max(this.data[x][y] - this.currentMinIntensity, 0) / Math.Max(1, this.currentMaxIntensity - this.currentMinIntensity) * 255, 255);
                        this.SetPixel(bits, x, height - y - 1, stride, pixelColor);
                    }
                }

                BitmapPalette palette = AppContext.Palettes[PaletteIndex];

                this.bitmap = BitmapSource.Create(width, height, 96, 96, pixelFormat, palette, bits, stride);

                // respect the pixel aspect ratio
                float ratio = this.dx / this.dy;
                if (!Util.NearEqual(ratio, 1.0))
                {
                    this.bitmap = new TransformedBitmap(this.bitmap, new ScaleTransform(ratio, 1.0d));
                }
            }
        }

        #endregion Methods
    }
}
