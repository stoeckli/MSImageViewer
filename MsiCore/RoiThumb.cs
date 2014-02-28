#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="RoiThumb.cs" company="Novartis Pharma AG.">
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
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Class set up for Region of Interest thumb
    /// </summary>
    public class RoiThumb : Thumb
    {
        #region Fields

        /// <summary>
        /// Property definition for ThumbId
        /// </summary>
        private Point position;

        /// <summary>
        /// Property definition for ThumbId
        /// Will be used later for adding new thumbs in the middle of 2 existing thumbs
        /// </summary>
        private int thumbid;

        /// <summary>
        /// Property definition for Image Source
        /// </summary>
        private string imagesource;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RoiThumb"/> class
        /// </summary>
        public RoiThumb()
        {
            this.StartLines = new List<LineGeometry>();
            this.EndLines = new List<LineGeometry>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoiThumb"/> class
        /// </summary>
        /// <param name="thumbid">thumb id</param>
        /// <param name="template">Control Template </param>
        /// <param name="imageSource">image source</param>
        /// <param name="position">position of thumb</param>
        public RoiThumb(int thumbid, ControlTemplate template, string imageSource, Point position)
            : this()
        {
            this.position = position;
            this.thumbid = thumbid;
            this.Template = template;
            this.imagesource = imageSource ?? string.Empty;
            this.SetPosition(position);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoiThumb"/> class
        /// </summary>
        /// <param name="thumbid">thumb id</param>
        /// <param name="template">Control Template </param>
        /// <param name="imageSource">image source</param>
        /// <param name="position">position of thumb</param>
        /// <param name="dragDelta">drag delta</param>
        public RoiThumb(int thumbid, ControlTemplate template, string imageSource, Point position, DragDeltaEventHandler dragDelta)
            : this(thumbid, template, imageSource, position)
        {
            this.DragDelta += dragDelta;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Handle the content of the textblock element taken from control template
        /// </summary>
        public Point Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// Handle the content of the textblock element taken from control template
        /// </summary>
        public int ThumbId
        {
            get
            {
                return this.thumbid;
            }

            set
            {
                this.thumbid = value;
            }
        }
        
        /// <summary>
        /// This property will handle the content of the image element taken from control template
        /// </summary>
        public string ImageSource
        {
            get
            {
                return this.imagesource;
            }

            set
            {
                this.imagesource = value;
            }
        }

        /// <summary>
        /// List of End lines
        /// </summary>
        public List<LineGeometry> EndLines
        {
            get;
            set;
        }

        /// <summary>
        /// List of Start lines
        /// </summary>
        public List<LineGeometry> StartLines
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Helper method for setting the position of our thumb 
        /// </summary>
        /// <param name="value"> Position to set the thumb</param>
        public void SetPosition(Point value)
        {
            Canvas.SetLeft(this, value.X);
            Canvas.SetTop(this, value.Y);
        }

        /// <summary>
        /// Returns a line geometry with updated positions to be processed outside. 
        /// This method establishes a link between current thumb and specified thumb.
        /// </summary>
        /// <param name="target">target thumb</param>
        /// <returns>Line Geometry </returns>
        public LineGeometry LinkTo(RoiThumb target)
        {
            // Create new line geometry
            var line = new LineGeometry();

            // Save as starting line for current thumb
            this.StartLines.Add(line);

            // Save as ending line for target thumb
            target.EndLines.Add(line);

            // Ensure both tumbs the latest layout
            this.UpdateLayout();
            target.UpdateLayout();

            // Update line position
            line.StartPoint = new Point(Canvas.GetLeft(this) + (this.ActualWidth / 2), Canvas.GetTop(this) + (this.ActualHeight / 2));

            line.EndPoint = new Point(Canvas.GetLeft(target) + (target.ActualWidth / 2), Canvas.GetTop(target) + (target.ActualHeight / 2));

            // return line for further processing
            return line;
        }

        /// <summary>
        /// This method establishes a link between current thumb and target thumb using a predefined line geometry
        /// Note: this is commonly to be used for drawing links with mouse when the line object is predefined outside this class
        /// </summary>
        /// <param name="target">target thumb</param>
        /// <param name="line">Line Geometry </param>
        /// <returns>true or false</returns>
        public bool LinkTo(RoiThumb target, LineGeometry line)
        {
            // Save as starting line for current thumb
            this.StartLines.Add(line);

            // Save as ending line for target thumb
            target.EndLines.Add(line);

            // Ensure both tumbs the latest layout
            this.UpdateLayout();

            target.UpdateLayout();

            // Update line position
            line.StartPoint = new Point(Canvas.GetLeft(this) + (this.ActualWidth / 2), Canvas.GetTop(this) + (this.ActualHeight / 2));

            line.EndPoint = new Point(Canvas.GetLeft(target) + (target.ActualWidth / 2), Canvas.GetTop(target) + (target.ActualHeight / 2));

            return true;
        }

        /// <summary>
        /// This method updates all the starting and ending lines assigned for the given thumb 
        /// according to the latest known thumb position on the canvas
        /// </summary>
        public void UpdateLinks()
        {
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);

            foreach (LineGeometry t in this.StartLines)
            {
                t.StartPoint = new Point(left + (this.ActualWidth / 2), top + (this.ActualHeight / 2));
            }

            foreach (LineGeometry t in this.EndLines)
            {
                t.EndPoint = new Point(left + (this.ActualWidth / 2), top + (this.ActualHeight / 2));
            }
        }

        /// <summary>
        /// Upon applying template we apply the "Title" and "ImageSource" properties to the template elements.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Access the image element of our custom template and assign it if ImageSource property defined
            if (this.ImageSource != string.Empty)
            {
                var img = this.Template.FindName("tplImage", this) as Image;

                if (img != null)
                {
                    img.Source = new BitmapImage(new Uri(this.ImageSource, UriKind.Relative));
                }
            }
        }

        #endregion
    }
}
