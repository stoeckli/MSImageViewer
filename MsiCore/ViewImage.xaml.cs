#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ViewImage.xaml.cs" company="Novartis Pharma AG.">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using Path = System.Windows.Shapes.Path;

    /// <summary>
    /// Interaction logic for ViewImage.xaml
    /// </summary>
    public partial class ViewImage : IView, IBitmapSource, IImagingSource
    {
        #region Fields

            #region Command Fields

                /// <summary>
                /// The StartRoi command.
                /// </summary>
                private static readonly RoutedCommand NewRoiCmd = new RoutedCommand("NewRoi", typeof(ViewImage), null);

                /// <summary>
                /// The CloseRoi command.
                /// </summary>
                private static readonly RoutedCommand CloseRoiCmd = new RoutedCommand("CloseRoi", typeof(ViewImage), null);

                /// <summary>
                /// The StopRoi command.
                /// </summary>
                private static readonly RoutedCommand FixRoiCmd = new RoutedCommand("FixRoi", typeof(ViewImage), null);

                /// <summary>
                /// The DeleteRoi command.
                /// </summary>
                private static readonly RoutedCommand DeleteRoiCmd = new RoutedCommand("DeleteRoi", typeof(ViewImage), null);

                /// <summary>
                /// The DeleteRoi command.
                /// </summary>
                private static readonly RoutedCommand EditRoiCmd = new RoutedCommand("EditRoi", typeof(ViewImage), null);

                /// <summary>
                /// The ViewRoiDataRoi command
                /// </summary>
                private static readonly RoutedCommand ViewRoiDataCmd = new RoutedCommand("ViewRoiData", typeof(ViewImage), null);

                /// <summary>
                /// The Zoom command
                /// </summary>
                private static readonly RoutedCommand ZoomCmd = new RoutedCommand("Zoom", typeof(ViewImage), null);

            #endregion Command Fields
            
            #region General Fields

                /// <summary>
                /// The title of this view as a <see cref="string"/>.
                /// </summary>
                private readonly string title;

                /// <summary>
                /// The <see cref="Document"/> object this view belongs to.
                /// </summary>
                private Document document;

                /// <summary>
                /// The image data to be displayed in this view.
                /// </summary>
                private Imaging imageData;

                /// <summary>
                /// Parameter used to control the mousewheelmode
                /// </summary>
                private MouseWheelMode mousewheelmode;

                /// <summary>
                /// The <see cref="ViewController"/> object of this view.
                /// </summary>
                private ViewImageController viewController;

                /// <summary>
                /// Point used for panning feature
                /// </summary>
                private Point start;
        
                /// <summary>
                /// Point used for panning feature
                /// </summary>
                private Point origin;

                /// <summary>
                /// Point used for centering when view resized
                /// </summary>
                private Point viewcenter;

                /// <summary>
                /// Line drawn by the mouse before connection established 
                /// </summary>
                private LineGeometry link;
        
                /// <summary>
                /// Geometry Group used to hold connectors between thumbs
                /// </summary>
                private GeometryGroup connectors;

            #endregion General Fields

            #region ROI Fields

                /// <summary>
                /// Path used to defining the lines between thumbs
                /// </summary>
                private Path roiPath;

                /// <summary>
                /// First thumb
                /// </summary>
                private RoiThumb startthumb;

                /// <summary>
                /// variable to hold the thumb drawing started from
                /// </summary>
                private RoiThumb sourcethumb;

                /// <summary>
                /// target thumb to connect to from the source thumb
                /// </summary>
                private RoiThumb targetthumb;

                /// <summary>
                /// List contain thumbs
                /// </summary>
                private List<RoiThumb> currentselectedroithumbs;

                /// <summary>
                /// Marker for current thumb ID
                /// </summary>
                private int currentthumbId;

                /// <summary>
                /// Contains a list of ROI objects
                /// </summary>
                private List<RegionOfInterest> roiObjects;

                /// <summary>
                /// Holder for current selected regionofinterest
                /// </summary>
                private RegionOfInterest currentselectedroiobject;

                /// <summary>
                /// Current selected polygon
                /// </summary>
                private Polygon currentselectedroipolygon;

                /// <summary>
                /// List of the current roi polygons (closed/fixed ROI's) in the view 
                /// </summary>
                private List<Polygon> currentroipolygonsinview;

                /// <summary>
                /// Flag used to indicate if we're inside a polygon
                /// </summary>
                private bool insidePolygon;

            #endregion ROI Fields

            #region Flag Fields

                /// <summary>
                /// Using this flag to load the Tic Image on start up
                /// </summary>
                private bool showTicFlag;

                /// <summary>
                /// Flag to show if we're inside the image control
                /// </summary>
                private bool inImageCtrlFlag;

                /// <summary>
                /// Flag used to define if we're in draw mode
                /// </summary>
                private bool isRoiDrawMode;

                /// <summary>
                /// flag used for defining the first thumb position and the rest of the thumbs
                /// </summary>
                private bool firsttimepass;

                /// <summary>
                /// Flag to set if we're in panning mode for mouse move event
                /// </summary>
                private bool panningmode;

                /// <summary>
                /// Flag for zoom feature
                /// </summary>
                private bool zoomflag;

            #endregion Flags Fields

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewImage"/> class 
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> object this view belongs to.</param>
        /// <param name="imageData">The <see cref="Imaging"/> data to be displayed in this view.</param>
        /// <param name="title">The title of this view as a <see cref="string"/>.</param>
        /// <param name="experimenttype">Experiment Type <see cref="ExperimentType"/>.</param>
        public ViewImage(Document doc, Imaging imageData, string title, ExperimentType experimenttype)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }

            if (imageData == null)
            {
                throw new ArgumentNullException("imageData");
            }

            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            InitializeComponent();

            // 1) Input parameters
            this.document = doc;
            this.imageData = imageData;
            this.title = title;
            this.ViewExperimentType = experimenttype;
            
            // 2) Events
            AppContext.PropertyChanged += this.AppContextPropertyChanged;
            AppContext.ActiveViewChanged += this.AppContextActiveViewChanged;

            // 3) Various flags
            ClipToBounds = true;
            this.panningmode = false;
            this.zoomflag = false;
           
            this.showTicFlag = true;
            this.mousewheelmode = MouseWheelMode.None;
            
            // 4) Create List of ROI's
            this.roiObjects = new List<RegionOfInterest>();

            // 5) Create List container for fixed ROI's in the view
            this.currentroipolygonsinview = new List<Polygon>();
        }

        #endregion Constructor

        #region Enums

        /// <summary>
        /// This enum is used to set which feature we are using with the mousewheel action 
        /// </summary>
        public enum MouseWheelMode
        {
            /// <summary>
            /// Default No mode set
            /// </summary>
            None = 0,

            /// <summary>
            /// Intensity scroller
            /// </summary>
            Intensity,

            /// <summary>
            /// Zoom Mode 
            /// </summary>
            ZoomMode
        }

        #endregion enums

        #region ROI Routed Commands

        /// <summary>
        /// Gets Routed startRoiCommand
        /// </summary>
        public static RoutedCommand NewRoiCommand
        {
            get { return NewRoiCmd; }
        }

        /// <summary>
        /// Gets Routed closeRoiCommand
        /// </summary>
        public static RoutedCommand CloseRoiCommand
        {
            get { return CloseRoiCmd; }
        }

        /// <summary>
        /// Gets Routed fixRoiCommand
        /// </summary>
        public static RoutedCommand FixRoiCommand
        {
            get { return FixRoiCmd; }
        }

        /// <summary>
        /// Gets Routed deleteRoiCommand
        /// </summary>
        public static RoutedCommand DeleteRoiCommand
        {
            get { return DeleteRoiCmd; }
        }

        /// <summary>
        /// Gets Routed editRoiCommand
        /// </summary>
        public static RoutedCommand EditRoiCommand
        {
            get { return EditRoiCmd; }
        }

        /// <summary>
        /// Gets Routed ZoomCommand
        /// </summary>
        public static RoutedCommand ZoomCommand
        {
            get { return ZoomCmd; }
        }

        /// <summary>
        /// Gets Routed ViewRoiDataRoiCommand
        /// </summary>
        public static RoutedCommand ViewRoiDataCommand
        {
            get { return ViewRoiDataCmd; }
        }

        #endregion Routed Commands
        
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the Document object this view belongs to.
        /// </summary>
        public Document Document
        {
            get
            {
                return this.document;
            }
        }

        /// <summary>
        /// Gets the ViewController object of this view.
        /// </summary>
        /// <value>
        /// A <see cref="ViewController"/> reference.
        /// </value>
        public ViewController ViewController
        {
            get
            {
                return this.viewController;
            }
        }

        /// <summary>
        /// Gets the name of this view.
        /// </summary>
        /// <value> A <see cref="string"/> object containing the name.</value>
        public string Title
        {
            get
            {
                return this.title;
            }
        }

        /// <summary>
        /// Gets or sets the viewexperimenttype
        /// </summary>
        public ExperimentType ViewExperimentType { get; set; }

        #endregion Public Properties

        #region IView Properties

        /// <summary>
        /// Gets the view object of this view. A view object is an object characterizes a view.
        /// There is a 1 to 1 relationship between IView and IViewObject.
        /// </summary>
        /// <value>The views view object as IViewObject reference.</value>
        public IViewObject ViewObject
        {
            get
            {
                return this.imageData;
            }
        }

        /// <summary>
        /// Gets the drawing area for this 2D-view object.
        /// </summary>
        /// <value>
        /// A <see cref="Grid"/> object.
        /// </value>
        public Canvas DrawingArea
        {
            get
            {
                return drawingArea;
            }
        }

        #endregion IView Properties

        #region ROI Properties

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

        #endregion ROI Properties

        #endregion Properties

        #region Methods

        #region IView Methods
        /// <summary>
        /// Close this view finally.
        /// </summary>
        public void Close()
        {
            Visibility = Visibility.Collapsed;
            this.viewController = null;
            this.imageData = null;
            this.document = null;
            AppContext.PropertyChanged -= this.AppContextPropertyChanged;
            AppContext.ActiveViewChanged -= this.AppContextActiveViewChanged;
        }

        /// <summary>
        /// Update the views content.
        /// </summary>
        public void Update()
        {
            this.Refresh();
        }

        /// <summary>
        /// Sets the <see cref="ViewController"/>-object that connects this
        /// view with the model data.
        /// </summary>
        /// <param name="viewCtrl">View Controller Object</param>
        public void SetViewController(ViewController viewCtrl)
        {
            this.viewController = viewCtrl as ViewImageController;

            if (this.viewController == null)
            {
                throw new ArgumentException("viewCtrl is of unexpected subclass");
            }
        }

        #endregion IView Methods

        #region IBitmapSource Methods

        /// <summary>
        /// Get the currently displayed bitmap.
        /// </summary>
        /// <returns>A BitmapSource instance.</returns>
        public BitmapSource GetBitmap()
        {
            return imageCtrl.Source as BitmapSource;
        }

        #endregion IBitmapSource Member

        #region IImagingSource Methods

        /// <summary>
        /// Retrieve the image data object currently associated with this view.
        /// </summary>
        /// <returns>An Imaging object representing the image data.</returns>
        public Imaging GetImagingData()
        {
            return this.imageData;
        }

        #endregion IImagingSource Methods

        #region ROI Methods

        /// <summary>
        /// Call this routine after the ViewImage has been constructed
        /// </summary>
        public void RoiInitialSetup()
        {
            // Some initialisation for ROI Objects
            this.currentselectedroithumbs = new List<RoiThumb>();
            this.currentselectedroiobject = new RegionOfInterest { PointsInsideRoi = new PointCollection(), ThumbPositions = new PointCollection() };

            this.firsttimepass = true;
            this.isRoiDrawMode = false;
            this.currentthumbId = 0;
            this.inImageCtrlFlag = false;
            this.zoomflag = false;
            this.insidePolygon = false;
            
            this.connectors = new GeometryGroup();
            this.roiPath = new Path { Stroke = Brushes.Red, StrokeThickness = 1, Data = this.connectors };
            view2D.drawingArea.Children.Add(this.roiPath);

            this.CreateContextMenus();
            this.DrawRoiPolygons();
        }

        /// <summary>
        /// Create context menus at runtime
        /// </summary>
        private void CreateContextMenus()
        {
            foreach (RegionOfInterest regionofinterest in this.RoiObjects)
            {
                var newroiMenuItem = new MenuItem { Header = regionofinterest.Title, IsCheckable = false };
                newroiMenuItem.Click += this.OnCmdSubNewRoi;
                this.mnuNew.Items.Add(newroiMenuItem);
                this.UpdateLayout();
            }
        }

        /// <summary>
        /// Event hanlder for dragging functionality support
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">DragDelta Event Args</param>
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = e.Source as RoiThumb;

            if (thumb != null)
            {
                double xpos = Canvas.GetLeft(thumb) + e.HorizontalChange;
                double ypos = Canvas.GetTop(thumb) + e.VerticalChange;

                Canvas.SetLeft(thumb, xpos);
                Canvas.SetTop(thumb, ypos);

                var newPosition = new Point(xpos, ypos);

                thumb.Position = newPosition;

                // Update links' layouts for active thumb
                thumb.UpdateLinks();
            }
        }

        /// <summary>
        /// Method to display the postion of the thumb
        /// </summary>
        /// <param name="e">Mouse Event Args</param>
        private void ShowThumbPos(MouseEventArgs e)
        {
            int currentthumbid = 0;

            var currentThumb = e.Source as RoiThumb;
            if (currentThumb != null)
            {
                currentthumbid = currentThumb.ThumbId;
            }

            Point pntImage = e.GetPosition(imageCtrl);
            Point pntCanvas = e.GetPosition(drawingArea);

            string pos = string.Format("ThumbID: {0:F2}, ImageX: {1:F2}, ImageY: {2:F2} , CanvasX: {3:F2} CanvasY: {4:F2}", currentthumbid, pntImage.X, pntImage.Y, pntCanvas.X, pntCanvas.Y);

            AppContext.StatusCoord = pos;
        }

        /// <summary>
        /// This routine resets local ROI drawing object ready for the next
        /// 'New' ROI object. 
        /// </summary>
        private void CleanUpRoi()
        {
            // 1) Delete all thumbs currentselectedroithumbs
            foreach (RoiThumb t in this.currentselectedroithumbs)
            {
                this.drawingArea.Children.Remove(t);
            }

            // 2) Reset all the thumb drawing parameters
            this.currentselectedroithumbs.Clear();
            this.startthumb = null;
            this.targetthumb = null;
            this.firsttimepass = true;
            this.currentthumbId = 0;

            // 3) Rest the currentselectedroiobject
            this.currentselectedroiobject.ThumbPositions.Clear();
            this.currentselectedroiobject.PointsInsideRoi.Clear();
            this.currentselectedroiobject.ImageFilePath = string.Empty;
            this.currentselectedroiobject.RoiStrokeColor = null;
            this.currentselectedroiobject.RoiFillColor = null;
            this.currentselectedroiobject.MeanIntensity = 0;
            this.currentselectedroiobject.Area = 0;
            this.currentselectedroiobject.NumberOfPointsInsideRoi = 0;
            
            // 4) Delete all connectors 
            this.connectors.Children.Clear();
        }

        /// <summary>
        /// Closes the current ROI. Links the first and last thumb
        /// </summary>
        private void CloseCurrentRoi()
        {
            try
            {
                this.isRoiDrawMode = false;
                Point firstPoint = this.currentselectedroithumbs[0].Position;
                Point lastPoint = this.currentselectedroithumbs[this.currentselectedroithumbs.Count - 1].Position;

                this.link = new LineGeometry(lastPoint, lastPoint);
                this.connectors.Children.Add(this.link);

                this.link.EndPoint = firstPoint;
                this.sourcethumb.LinkTo(this.startthumb, this.link);
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Calculates are the Points inside ROI We will create a temporary Canvas and draw 
        /// the ROI Polygon onto it (Canvas is white, polygon is black)
        /// Steps:-
        /// 1) Create a canvas in memory, with a white background
        /// 2) Make a bitmap image of this canvas - save it to memory (file)
        /// 3) Generate a bitmap of that canvas
        /// 4) Iterate through the bitmap and find the position of all black pixels - store them
        /// 5) Calculate the NumberOfPointsInsideRoi
        /// 6) Clean up objects
        /// </summary>
        private void CalculatePointsInRoi()
        {
            try
            {
                // Render or actual size?
                var drawingAreaWidth = (int)this.drawingArea.ActualWidth;
                var drawingAreaHeight = (int)this.drawingArea.ActualHeight;
                
                // 1) Create a canvas in memory, with a white background
                var roiCanvasForBitmap = new Canvas
                {
                    Width = drawingAreaWidth,
                    Height = drawingAreaHeight,
                    Background = Brushes.White,
                    Name = "roiCanvas"
                };

                roiCanvasForBitmap.UpdateLayout();
                roiCanvasForBitmap.Measure(new Size(drawingAreaWidth, drawingAreaHeight));
                roiCanvasForBitmap.Arrange(new Rect(0, 0, drawingAreaWidth, drawingAreaHeight));

                // 2) Create a polygon on that canvas and colour it black
                var roiPolygon = new Polygon
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = Brushes.Black,
                };

                // Having to put in this offset to match the drawn ROI
                const int Polygonoffset = 3;
                var thumbpositions = new PointCollection();
                
                // get the thumb positons for the polygon points
                foreach (RoiThumb myThumb in this.currentselectedroithumbs)
                {
                    var thumbPos = new Point(myThumb.Position.X + Polygonoffset, myThumb.Position.Y + Polygonoffset);
                    thumbpositions.Add(thumbPos);
                }

                roiPolygon.Stretch = Stretch.None;
                roiPolygon.Points = thumbpositions;
                roiPolygon.HorizontalAlignment = HorizontalAlignment.Center;
                roiPolygon.VerticalAlignment = VerticalAlignment.Center;

                roiCanvasForBitmap.Children.Add(roiPolygon);
                roiCanvasForBitmap.UpdateLayout();

                // 3) Generate a bitmap of that canvas
                var height = (int)roiCanvasForBitmap.Height;
                var width = (int)roiCanvasForBitmap.Width;

                var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(roiCanvasForBitmap);

                // Stick the image in a jpg file.                
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                using (var ms = new MemoryStream())
                {
                    encoder.Save(ms);

                    // 4) Loop through all the pixels in that bitmap and pick out black ones, record the points that are black. 
                    // TODO 4b) Could work out the extreme points of the polygon and reduce the search area (min and max X, Y)
                    var bmap = new System.Drawing.Bitmap(ms);

                    ms.Close();
                    ms.Dispose();

                    this.currentselectedroiobject.PointsInsideRoi = new PointCollection();

                    for (int x = 0; x < bmap.Width; x++)
                    {
                        for (int y = 0; y < bmap.Height; y++)
                        {
                            System.Drawing.Color clr = bmap.GetPixel(x, y);

                            int red = clr.R;
                            int green = clr.G;
                            int blue = clr.B;

                            if (red == 0 && green == 0 && blue == 0)
                            {
                                var pointinside = new Point(x, y);
                                
                                this.currentselectedroiobject.PointsInsideRoi.Add(pointinside);
                            }
                        }
                    }

                    // 5) Calculate the NumberOfPointsInsideRoi
                    this.currentselectedroiobject.NumberOfPointsInsideRoi = this.currentselectedroiobject.PointsInsideRoi.Count;

                    // 6) Clean up code
                    bmap.Dispose();
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Fixes the ROI and does various bits of processing
        /// Step 1 Check to see if regionofinterest is closed. If it isn't close it.
        /// Step 2 Create a polygon for the area around
        /// Step 3 Disable the Roithumbs - so they can't be moved.
        /// Step 4 Find all points inside of the polygon and save them to pointsInsideRoi
        /// Step 5 Copy currentselectedroithumbs into ThumbPositions
        /// Step 6 Calculate the area of the ROI
        /// Step 7 Calculate the MeanIntensity of the ROI 
        /// Step 8 Copy the contents of regionofinterest Objects to the List of ROI Objects for this view
        /// Step 9 Enable Menus
        /// Step 10 Clean up Local ROI objects
        /// </summary>
        private void FixRoi()
        {
            try
            {
                imageCtrl.Cursor = Cursors.Wait;

                // 1) Check to see if the current regionofinterest is closed. If not, then close it
                Point firstPoint = this.currentselectedroithumbs[0].Position;
                Point lastPoint = this.currentselectedroithumbs[this.currentselectedroithumbs.Count - 1].Position;

                if (firstPoint != lastPoint)
                {
                    this.CloseCurrentRoi();
                }

                if (this.currentselectedroiobject != null)
                {
                    // 2) Draw a polygon based on the connecition points in this.thumbPositions Fill the polygon with a colour
                    var roiPolygon = new Polygon
                    {
                        Stroke = this.currentselectedroiobject.RoiStrokeColor,
                        StrokeThickness = 1,
                        Fill = this.currentselectedroiobject.RoiFillColor,
                        Opacity = 0.25,
                        Name = this.currentselectedroiobject.Title
                    };

                    // Having to put in this offset to match the drawn ROI
                    const int Polygonoffset = 3;
                    var thumbpositions = new PointCollection();

                    // 3) disable the roithumbs
                    foreach (RoiThumb myThumb in this.currentselectedroithumbs)
                    {
                        var thumbPos = new Point(myThumb.Position.X + Polygonoffset, myThumb.Position.Y + Polygonoffset);
                        thumbpositions.Add(thumbPos);
                        myThumb.IsEnabled = false;
                    }

                    roiPolygon.Stretch = Stretch.None;
                    roiPolygon.Points = thumbpositions;
                    roiPolygon.HorizontalAlignment = HorizontalAlignment.Center;
                    roiPolygon.VerticalAlignment = VerticalAlignment.Center;

                    roiPolygon.MouseLeftButtonDown += this.OnMouseLeftButtonPolygonDown;
                    roiPolygon.MouseEnter += this.OnMousePolygonEnter;
                    roiPolygon.MouseLeave += this.OnMousePolygonLeave;
                    roiPolygon.MouseMove += this.OnMousePolygonMove;
                    roiPolygon.MouseRightButtonDown += this.OnMousePolygonRightButtonDown;
                    
                    this.drawingArea.Children.Add(roiPolygon);
                    this.currentroipolygonsinview.Add(roiPolygon);

                    // Step 4 Find all points inside of the polygon and save them to pointsInsideRoi
                    this.CalculatePointsInRoi();

                    // Step 5 Copy currentselectedroithumbs into ThumbPositions
                    foreach (RoiThumb roithumb in this.currentselectedroithumbs)
                    {
                        this.currentselectedroiobject.ThumbPositions.Add(roithumb.Position);  
                    }

                    // Step 6 Calculate the area of the ROI
                    this.currentselectedroiobject.Area = this.CalculateArea(this.currentselectedroiobject);

                    // Step 7 Calculate the MeanIntensity of the ROI 
                    this.currentselectedroiobject.MeanIntensity = this.CalculateMeanIntensity(this.currentselectedroiobject);

                    // Step 8 Copy the contents of regionofinterest Objects to the List of ROI Objects for this view
                    int index = this.roiObjects.FindIndex(roi => roi.Title == this.currentselectedroiobject.Title);

                    this.roiObjects[index].ImageFilePath = this.currentselectedroiobject.ImageFilePath;
                    this.roiObjects[index].RoiStrokeColor = this.currentselectedroiobject.RoiStrokeColor;
                    this.roiObjects[index].RoiFillColor = this.currentselectedroiobject.RoiFillColor;
                    this.roiObjects[index].MeanIntensity = this.currentselectedroiobject.MeanIntensity;
                    this.roiObjects[index].Area = this.currentselectedroiobject.Area;
                    this.roiObjects[index].NumberOfPointsInsideRoi = this.currentselectedroiobject.NumberOfPointsInsideRoi;

                    this.roiObjects[index].ThumbPositions.Clear();
                    foreach (Point thumbpoint in this.currentselectedroiobject.ThumbPositions)
                    {
                        this.roiObjects[index].ThumbPositions.Add(thumbpoint);
                    }

                    this.roiObjects[index].PointsInsideRoi.Clear();
                    foreach (Point pointinsideroi in this.currentselectedroiobject.PointsInsideRoi)
                    {
                        this.roiObjects[index].PointsInsideRoi.Add(pointinsideroi);
                    }

                    // Step 09 Clean up Local ROI objects
                    this.CleanUpRoi();
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
            finally
            {
                imageCtrl.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// This Method takes an array of points from ROI and calculates the area
        /// </summary>
        /// <param name="regionofinterest">Region of Interest who's area we want to calculate</param>
        /// <returns>returns the area of the regionofinterest</returns>
        private double CalculateArea(RegionOfInterest regionofinterest)
        {
            double area = 0;

            if (regionofinterest != null)
            {
                if (regionofinterest.ImageData != null)
                {
                    double areaofonepixel = regionofinterest.ImageData.Dx * regionofinterest.ImageData.Dy;

                    area = regionofinterest.PointsInsideRoi.Count * areaofonepixel;
                }
            }

            return area;
        }

        /// <summary>
        /// This method calculates the mean intensity in the currentroi
        /// and updates the currentroi with the mean intensity value
        /// </summary>
        /// <param name="regionofinterest">Region oif Interest</param>
        /// <returns>Mean Intensity</returns>
        private double CalculateMeanIntensity(RegionOfInterest regionofinterest)
        {
            double meanIntensity = 0;

            if (regionofinterest.ImageData != null)
            {
                ImageData actImage = null;

                if (regionofinterest.ImageData is ImageData)
                {
                    actImage = regionofinterest.ImageData as ImageData;
                }
                else if (regionofinterest.ImageData is ImageSpectrumData)
                {
                    actImage = (regionofinterest.ImageData as ImageSpectrumData).CurrentImage;
                }

                if (actImage != null)
                {
                    foreach (Point pixelpoint in regionofinterest.PointsInsideRoi)
                    {
                        var pixelpointint = this.CalculatedPixelPointRelative(pixelpoint, actImage);
                        meanIntensity += (long)actImage.Data[pixelpointint.X][pixelpointint.Y];
                    }
                }
            }

            meanIntensity /= regionofinterest.PointsInsideRoi.Count;
            
            return meanIntensity;
        }

        /// <summary>
        /// This routines draws the polygons from the contents of this.RoiObjects
        /// Used when Opening a ROI Project where ROI's are already defined.
        /// </summary>
        private void DrawRoiPolygons()
        {
            foreach (RegionOfInterest regionofinterest in this.RoiObjects)
            {
                // 1) Check to to see if there are some ThumbPositions
                if (regionofinterest.ThumbPositions != null && regionofinterest.ThumbPositions.Count > 0)
                {
                    // 2) Draw a polygon based on the thumbPositions. Fill the polygon with a colour
                    var roiPolygon = new Polygon
                        {
                            Stroke = regionofinterest.RoiStrokeColor,
                            StrokeThickness = 1,
                            Fill = regionofinterest.RoiFillColor,
                            Opacity = 0.25,
                            Name = regionofinterest.Title,
                            Stretch = Stretch.None,
                            Points = regionofinterest.ThumbPositions,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                    roiPolygon.MouseLeftButtonDown += this.OnMouseLeftButtonPolygonDown;
                    roiPolygon.MouseEnter += this.OnMousePolygonEnter;
                    roiPolygon.MouseLeave += this.OnMousePolygonLeave;
                    roiPolygon.MouseMove += this.OnMousePolygonMove;
                    roiPolygon.MouseRightButtonDown += this.OnMousePolygonRightButtonDown;
                    
                    this.drawingArea.Children.Add(roiPolygon);
                    this.currentroipolygonsinview.Add(roiPolygon);
                }
            }
        }
        
        /// <summary>
        /// Creates a New Roi
        /// </summary>
        /// <param name="chosenRoiHeader">Chosen Menu item</param>
        private void NewRoi(string chosenRoiHeader)
        {
            try
            {
                // 1) Given the chosen ROI, set the current ROI to appropirate value
                RegionOfInterest chosenRoi = this.roiObjects.Find(roi => roi.Title == chosenRoiHeader);

                this.currentselectedroiobject.Title = chosenRoi.Title;
                this.currentselectedroiobject.ImageFilePath = chosenRoi.ImageFilePath;
                this.currentselectedroiobject.Area = chosenRoi.Area;
                this.currentselectedroiobject.MeanIntensity = chosenRoi.MeanIntensity;

                this.currentselectedroiobject.RoiStrokeColor = chosenRoi.RoiStrokeColor;
                this.currentselectedroiobject.RoiFillColor = chosenRoi.RoiFillColor;

                this.currentselectedroiobject.NumberOfPointsInsideRoi = chosenRoi.NumberOfPointsInsideRoi;
                this.currentselectedroiobject.ImageData = chosenRoi.ImageData;

                foreach (Point thumbpoint in chosenRoi.ThumbPositions)
                {
                    this.currentselectedroiobject.ThumbPositions.Add(thumbpoint);
                }

                foreach (Point pointinsideroi in chosenRoi.PointsInsideRoi)
                {
                    this.currentselectedroiobject.PointsInsideRoi.Add(pointinsideroi);
                }

                // 2) Set the various Editing flags
                this.isRoiDrawMode = true;
                Mouse.OverrideCursor = Cursors.Pen;
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Deletes a Roi
        /// </summary>
        private void DeleteRoi()
        {
            try
            {
                // 1) We have a fixed polygon 
                if (this.currentselectedroipolygon != null)
                {
                    // a) delete polygon from ROI Object (Remove pologon)
                    this.drawingArea.Children.Remove(this.currentselectedroipolygon);
                    this.currentroipolygonsinview.Remove(this.currentselectedroipolygon);

                    // b) Clean up RoiObjects
                    int index = this.roiObjects.FindIndex(roi => roi.Title == this.currentselectedroipolygon.Name);

                    this.roiObjects[index].MeanIntensity = 0.0;
                    this.roiObjects[index].Area = 0.0;
                    this.roiObjects[index].NumberOfPointsInsideRoi = 0;
                    this.roiObjects[index].ThumbPositions.Clear();
                    this.roiObjects[index].PointsInsideRoi.Clear();
                }
                else
                {
                    // 2) We have an open or closed Roi but not polygon. Clean up thumbs for closed polygon
                    this.CleanUpRoi();
                }

                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Edits a Roi
        /// </summary>
        private void EditRoi()
        {
            try
            {
                // 1) See if we have a valid selected polygon.
                if (this.currentselectedroipolygon != null)
                {
                    // 2) Use the currentselectedroipolygon name
                    int index = this.roiObjects.FindIndex(roi => roi.Title == this.currentselectedroipolygon.Name);

                    this.currentselectedroiobject.Title = this.roiObjects[index].Title;
                    this.currentselectedroiobject.MeanIntensity = this.roiObjects[index].MeanIntensity;
                    this.currentselectedroiobject.Area = this.roiObjects[index].Area;
                    this.currentselectedroiobject.ImageFilePath = this.roiObjects[index].ImageFilePath;
                    this.currentselectedroiobject.ImageData = this.roiObjects[index].ImageData;
                    this.currentselectedroiobject.RoiFillColor = this.roiObjects[index].RoiFillColor;
                    this.currentselectedroiobject.RoiStrokeColor = this.roiObjects[index].RoiStrokeColor;
                    this.currentselectedroiobject.NumberOfPointsInsideRoi = this.roiObjects[index].NumberOfPointsInsideRoi;

                    foreach (var thumbpoint in this.roiObjects[index].ThumbPositions)
                    {
                        this.currentselectedroiobject.ThumbPositions.Add(thumbpoint);
                    }

                    // 3) Delete the polygon
                    this.drawingArea.Children.Remove(this.currentselectedroipolygon);
                    this.currentroipolygonsinview.Remove(this.currentselectedroipolygon);

                    // 4) For each Thumbposition in currentselectedroiobject. Add the thumb to the canvas
                    for (int i = 0; i < this.currentselectedroiobject.ThumbPositions.Count; i++)
                    {
                        Point thumbposition = this.currentselectedroiobject.ThumbPositions[i];

                        if (i == 0)
                        {
                            var newThumb = new RoiThumb(
                                                        this.currentthumbId,
                                                        this.Resources["BasicShape1"] as ControlTemplate,
                                                        "/MsiCore;component/Images/thumb6x6.png",
                                                        thumbposition,
                                                        this.OnDragDelta);

                            newThumb.MouseEnter += this.OnMouseThumbEnter;
                            newThumb.MouseLeave += this.OnMouseThumbLeave;
                            this.currentthumbId += 1;
                            this.sourcethumb = newThumb;
                            this.startthumb = newThumb;
                            this.currentselectedroithumbs.Add(newThumb);

                            this.drawingArea.Children.Add(newThumb);
                        }
                        else
                        {
                            var newThumb = new RoiThumb(
                                                        this.currentthumbId,
                                                        this.Resources["BasicShape1"] as ControlTemplate,
                                                        "/MsiCore;component/Images/thumb6x6.png",
                                                        thumbposition,
                                                        this.OnDragDelta);

                            newThumb.MouseEnter += this.OnMouseThumbEnter;
                            newThumb.MouseLeave += this.OnMouseThumbLeave;

                            this.currentthumbId += 1;
                            this.targetthumb = newThumb;
                            this.currentselectedroithumbs.Add(newThumb);

                            // Put newly created thumb on the canvas
                            this.drawingArea.Children.Add(newThumb);

                            this.link = new LineGeometry(thumbposition, thumbposition);
                            this.connectors.Children.Add(this.link);

                            this.link.EndPoint = thumbposition;
                            this.sourcethumb.LinkTo(this.targetthumb, this.link);

                            this.sourcethumb = newThumb;
                        }
                    }

                    this.CloseCurrentRoi();
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// This routine enables and disable the context menu's depending on the status off the view
        /// 1) mnuNew.items: Check to see which polygons exist in the view. Disable all menu options which have valid polygons
        ///                  Enable ones which don't have valid polygons. ** Note Can ONLY have one polygon per ROI **
        /// 2) mnuFix, mnuClose: If there is a Open of closed path enable otherwise disable
        /// 3) mnuDelete: If there is a currentselectedroipolygon or currentselectedroiobject enable Otherwise disable
        /// 4) mnuEdit: If we have a currently selected polygon enable the Edit menu, otherwise disable
        /// 5) mnuViewRoiData: If there are 'any' polygons on the current view enable mnuViewRoiData, otherwise disable
        /// </summary>
        private void SetContextMenuStatus()
        {
            // 1) mnuNew.items
            foreach (MenuItem item in this.mnuNew.Items)
            {
                item.IsEnabled = this.currentroipolygonsinview.FindIndex(polygon => polygon.Name == (string)item.Header) == -1;
            }

            this.mnuNew.IsEnabled = true;

            // 2) mnuFix, mnuClose: 
            if (this.currentselectedroithumbs.Count > 1)
            {
                this.mnuFix.IsEnabled = true;
                this.mnuClose.IsEnabled = true;
            }
            else
            {
                this.mnuFix.IsEnabled = false;
                this.mnuClose.IsEnabled = false;
            }
            
            // 3) mnuDelete:
            if (this.currentselectedroipolygon != null || this.currentselectedroithumbs.Count > 0) 
            {
                this.mnuDelete.IsEnabled = true;
            }
            else
            {
                this.mnuDelete.IsEnabled = false;
            }

            // 4) mnuEdit:
            this.mnuEdit.IsEnabled = this.currentselectedroipolygon != null;

            // 5) mnuViewRoiData:
            this.mnuViewRoiData.IsEnabled = this.currentroipolygonsinview.Count > 0;
        }

        #endregion
        
        #region Other(Private)Methods

        /// <summary>
        /// Refresh Method
        /// </summary>
        private void Refresh()
        {
            if (this.imageData != null)
            {
                BitmapSourceList bitmaps = this.imageData.GetBitmaps();
                if (bitmaps != null && bitmaps.Count > 0)
                {
                    imageCtrl.Source = bitmaps[0];
                }
            }
        }
       
        /// <summary>
        /// This methods takes the input cursor point and returns the actual position in mm
        /// </summary>
        /// <param name="cursorpoint">Point of current Mouse Cursor</param>
        /// <param name="actImage">ImageData Object</param>
        /// <returns>Calculated Pixel Point</returns>
        private PixelPointDbl CalculatedPixelPointActualPosition(Point cursorpoint, ImageData actImage)
        {
            PixelPointDbl pixelpoint;
            pixelpoint.X = 0;
            pixelpoint.Y = 0;

            if (actImage != null)
            {
                Size viewSize = imageCtrl.RenderSize;
                viewSize.Height /= imageCtrl.Source.Height;
                viewSize.Width /= imageCtrl.Source.Width;

                pixelpoint.X = ((cursorpoint.X / viewSize.Width) / (actImage.Dx / actImage.Dy)) * actImage.Dx;
                pixelpoint.Y = ((imageCtrl.RenderSize.Height - cursorpoint.Y) / viewSize.Height) * actImage.Dy;
            }

            return pixelpoint;
        }

        /// <summary>
        /// This methods takes the input cursor point and returns the calculated the relative pixel point
        /// </summary>
        /// <param name="cursorpoint">Point of current Mouse Cursor</param>
        /// <param name="actImage">ImageData Object</param>
        /// <returns>Calculated Pixel Point</returns>
        private PixelPointInt CalculatedPixelPointRelative(Point cursorpoint, ImageData actImage)
        {
            PixelPointInt pixelpoint;
            pixelpoint.X = 0;
            pixelpoint.Y = 0;

            if (actImage != null)
            {
                // calculate pixel point...
                Size viewSize = imageCtrl.RenderSize;
                viewSize.Height /= imageCtrl.Source.Height;
                viewSize.Width /= imageCtrl.Source.Width;
                    
                var y = (int)(cursorpoint.Y / viewSize.Height);
                var x = (int)((cursorpoint.X / viewSize.Width) / (actImage.Dx / actImage.Dy));

                if (x < 0)
                {
                    x = 0;
                }

                if (y < 0)
                {
                    y = 0;
                }

                if (x >= this.imageData.XPoints)
                {
                    x = actImage.XPoints - 1;
                }

                if (y >= actImage.YPoints)
                {
                    y = actImage.YPoints - 1;
                }

                pixelpoint.X = x;
                pixelpoint.Y = y;
            }

            return pixelpoint;
        }

        /// <summary>
        /// Sets information about the the position of the cursor in the status bar
        /// </summary>
        /// <param name="point"> Point defining the cursor position</param>
        private void SetStatusInfos(Point point)
        {
            try
            {
                if (this.imageData != null)
                {
                    ImageData actImage = null;

                    if (this.imageData is ImageData)
                    {
                        actImage = this.imageData as ImageData;
                    }
                    else if (this.imageData is ImageSpectrumData)
                    {
                        actImage = (this.imageData as ImageSpectrumData).CurrentImage;
                    }

                    if (actImage != null)
                    {
                        var pixelpointint = this.CalculatedPixelPointRelative(point, actImage);

                        var intensity = (long)actImage.Data[pixelpointint.X][pixelpointint.Y];
                        string intens = intensity.ToString(CultureInfo.InvariantCulture);

                        var pixelpointdbl = this.CalculatedPixelPointActualPosition(point, actImage);

                        string pos = string.Format("x: {0:F2} mm, y: {1:F2} mm, (PixX: {2:F2}) , (PixY: {3:F2})", pixelpointdbl.X, pixelpointdbl.Y, point.X, imageCtrl.RenderSize.Height - point.Y);

                        AppContext.StatusCoord = pos;
                        AppContext.StatusIntensity = intens;
                    }
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// Takes mouse input position and brings up graph of all intensity points
        /// This routine assumes that we have a Imagedata of ImageSpectrumData
        /// </summary>
        /// <param name="point">Position of the Mouse</param>
        private void ShowIntensityPoints(Point point)
        {
            if (this.imageData != null)
            {
                ImageData actImage = null;

                if (this.imageData is ImageData)
                {
                    actImage = this.imageData as ImageData;
                }
                else if (this.imageData is ImageSpectrumData)
                {
                    actImage = (this.imageData as ImageSpectrumData).CurrentImage;
                }

                if (actImage != null)
                {
                    // 1) Convert Point into position in array (x,y)
                    var pixelpointint = this.CalculatedPixelPointRelative(point, actImage);

                    // 2) For (x,y) in each scan (imageDataList), extract the intensity
                    var imageSpectrumData = this.imageData as ImageSpectrumData;
                    if (imageSpectrumData != null)
                    {
                        List<ImageData> myImageDataList = imageSpectrumData.DataSets;
                        int numberOfMasses = myImageDataList.Count;

                        var intensityData = new float[numberOfMasses];

                        // Cycle through the list of ImageDatas and pick out the intensity point for each x, y position 
                        for (int scanNumber = 0; scanNumber < numberOfMasses; scanNumber++)
                        {
                            actImage = imageSpectrumData.DataSets[scanNumber];
                            var intensity = actImage.Data[pixelpointint.X][pixelpointint.Y];
                            intensityData[scanNumber] = intensity;
                        }

                        // 3) Bring up Intensity Graph
                        var intGraph = new IntensityGraph(intensityData);
                        
                        if (intGraph.ShowDialog() == true)
                        {
                        }
                    }
                }
            }
        }

        #endregion Other (Private) Methods
        
        #endregion Methods

        #region Events

        /// <summary>
        /// Property Change Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void AppContextPropertyChanged(object sender, ApplicationEventArgs e)
        {
            // if this is the active view, than react on the palette change...
            if (AppContext.Application.ActiveView == this && e != null)
            {
                IntensitySettings settings;
                switch (e.Property)
                {
                    case AppProperties.PaletteIndex:
                        this.imageData.PaletteIndex = (int)e.PropertyValue;
                        this.Refresh();
                        break;
                    case AppProperties.MinIntensity:
                        settings = AppContext.Application.MinIntensitySettings;
                        this.imageData.CurrentMinIntensity = (float)settings.CurrentIntensity;
                        this.Refresh();
                        break;
                    case AppProperties.MaxIntensity:
                        settings = AppContext.Application.MaxIntensitySettings;
                        this.imageData.CurrentMaxIntensity = (float)settings.CurrentIntensity;
                        this.Refresh();
                        break;
                     case AppProperties.CurrentMass:
                        var imageSpecData = this.imageData as ImageSpectrumData;
                        var currentmassNumber = (double)e.PropertyValue;

                        if (imageSpecData != null)
                        {
                            // currentmassNumber = 0 means we hit the TIC button
                            if ((int)currentmassNumber == 0)
                            {
                                this.showTicFlag = true;
                            }

                            // First time through we want to show the TIC image and when TIC button is clicked
                            if (this.showTicFlag)
                            {
                                imageSpecData.CurrentImage = imageSpecData.ImageTic;
                                this.showTicFlag = false;
                            }
                            else if (currentmassNumber >= imageSpecData.MassCal[0]
                                        && currentmassNumber < imageSpecData.MassCal[imageSpecData.MassCal.Length - 1] && this.showTicFlag == false) 
                            {
                                float stepsize = 0.0f;
                                float startmass = imageSpecData.MassCal[0]; // imageSpecData.MinMass;
                                int binsize = 1;

                                foreach (MetaDataItem mdi in imageSpecData.MetaData)
                                {
                                    switch (mdi.Name)
                                    {
                                            // Get Step Size from meta data
                                        case "Mass Step Size":
                                            stepsize = float.Parse(mdi.ValueString);
                                            break;

                                        // Get Bin Size from meta data
                                        case "Bin Size":
                                            binsize = int.Parse(mdi.ValueString);
                                            break;
                                    }
                                }

                                // Make sure we're not dividing by 0
                                if (binsize > 0 && stepsize > 0)
                                {
                                    var scanNumber = (int)(((currentmassNumber - startmass) / stepsize) / binsize);
                                    
                                    List<ImageData> imageDataList = imageSpecData.DataSets;
                                    (this.imageData as ImageSpectrumData).CurrentImage = imageDataList[scanNumber];
                                }
                                else
                                {
                                    {
                                        MessageBox.Show("Binsize or Stepsize is 0, divide by zero error!", "Slider Control Error");
                                    }
                                }
                            }

                            this.Refresh();
                            UpdateLayout();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Active view changed Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void AppContextActiveViewChanged(IView sender, EventArgs e)
        {
            // if we are the freshly activated view then reflect our properties in the UI
            if (sender == this)
            {
                if (this.imageData.PaletteIndex != AppContext.Application.PaletteIndex)
                {
                    AppContext.Application.PaletteIndex = this.imageData.PaletteIndex;
                }

                IntensitySettings intensity;

                // settings for the intensities lower bound
                intensity.CurrentIntensity = this.imageData.CurrentMinIntensity;
                intensity.MinIntensity = 0.0;
                intensity.MaxIntensity = this.imageData.ImageMaxIntensity;
                intensity.StepLarge = this.imageData.ImageMaxIntensity / 100.0f;
                intensity.StepSmall = intensity.StepLarge / 10.0f;
                AppContext.Application.MinIntensitySettings = intensity;

                // settings for the intensities upper bound
                intensity.CurrentIntensity = this.imageData.CurrentMaxIntensity;
                intensity.MinIntensity = 0.0;
                intensity.MaxIntensity = this.imageData.ImageMaxIntensity;
                intensity.StepLarge = this.imageData.ImageMaxIntensity / 100.0f;
                intensity.StepSmall = intensity.StepLarge / 10.0f;
                AppContext.Application.MaxIntensitySettings = intensity;
            }
        }

        /// <summary>
        /// ViewImage Load Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void ViewImageLoaded(object sender, RoutedEventArgs e)
        {
            this.Refresh();
            UpdateLayout();
            imageCtrl.Cursor = Cursors.Cross;
            this.viewcenter.X = border.ActualWidth;
            this.viewcenter.Y = border.ActualHeight;
        }

        /// <summary>
        /// DrawingArea Loaded Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void DrawingAreaLoaded(object sender, RoutedEventArgs e)
        {
            var canvArea = sender as Canvas;
            if (canvArea != null && canvArea.Name == "drawingArea")
            {
                canvArea.UpdateLayout();
            }

            this.Focus();
        }

        /// <summary>
        /// ViewImage MouseWheel Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void ViewImageMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.mousewheelmode == MouseWheelMode.ZoomMode)
            {
                // scale image
                Point p = e.MouseDevice.GetPosition(drawingArea);

                Matrix m = drawingArea.RenderTransform.Value;
                if (e.Delta > 0)
                {
                    m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
                }
                else
                {
                    m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);
                }

                drawingArea.RenderTransform = new MatrixTransform(m);
            }
            else if (this.mousewheelmode == MouseWheelMode.Intensity)
            {
                // Code for changing image intensity
                ImageData actImage = null;

                if (this.imageData is ImageData)
                {
                    actImage = this.imageData as ImageData;
                }
                else if (this.imageData is ImageSpectrumData)
                {
                    actImage = (this.imageData as ImageSpectrumData).CurrentImage;
                }

                if (actImage != null) 
                {
                    float delta = ((Keyboard.Modifiers & ModifierKeys.Control) > 0) ? actImage.IntensityDelta / 10f : actImage.IntensityDelta;

                    if (e != null && e.Delta > 0)
                    {
                        actImage.CurrentMaxIntensity += delta;
                        actImage.ImageMaxIntensity = Math.Max(actImage.ImageMaxIntensity, actImage.CurrentMaxIntensity);
                        this.Refresh();
                    }
                    else if (e != null && e.Delta < 0)
                    {
                        if (actImage.CurrentMaxIntensity - delta >= 0.0f)
                        {
                            actImage.CurrentMaxIntensity -= delta;
                        }
                        else
                        {
                            actImage.CurrentMaxIntensity = 0;
                        }

                        this.Refresh();
                    }

                    // settings for the intensities upper bound
                    IntensitySettings intensity;
                    intensity.CurrentIntensity = actImage.CurrentMaxIntensity;
                    intensity.MinIntensity = 0.0;
                    intensity.MaxIntensity = actImage.ImageMaxIntensity;
                    intensity.StepLarge = actImage.ImageMaxIntensity / 100.0;
                    intensity.StepSmall = intensity.StepLarge / 10.0f;
                    AppContext.Application.MaxIntensitySettings = intensity;
                }
            }
        }

        /// <summary>
        /// ViewImage KeyDown Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void ViewImageKeyDown(object sender, KeyEventArgs e)
        {
            // JP 12/12/2011 Add Code in here to capture event to set chosen point of interest
            // Get position of the Mouse Pointer (or current X, Y Pos)
            // Call this.ShowIntensityPoints(Point);
            if (this.viewController != null)
            {
                this.viewController.OnKeyDown(sender, e);
            }

            switch (e.Key)
            {
                case Key.LeftShift:
                    this.mnuZoom.IsChecked = true;
                    this.zoomflag = true;
                    this.mousewheelmode = MouseWheelMode.ZoomMode;
                    break;
                case Key.RightShift:
                    this.mnuZoom.IsChecked = true;
                    this.zoomflag = true;
                    this.mousewheelmode = MouseWheelMode.ZoomMode;
                    break;
            }
        }

        /// <summary>
        /// This is key up event for handling panning and zooming 
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Key Event Args</param>
        private void ViewImageKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    this.mnuZoom.IsChecked = false;
                    this.zoomflag = false;
                    this.mousewheelmode = MouseWheelMode.Intensity;
                    break;
                case Key.RightShift:
                    this.mnuZoom.IsChecked = false;
                    this.zoomflag = false;
                    this.mousewheelmode = MouseWheelMode.Intensity;
                    break;
            }
        }

        /// <summary>
        /// ViewImage MouseDown Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void ViewImageLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.insidePolygon)
            {
                if (this.currentselectedroipolygon != null)
                {
                    this.currentselectedroipolygon.Opacity = 0.25;
                    this.currentselectedroipolygon = null;
                }
            }
        }

        /// <summary>
        /// imageCtrl MouseEnter
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void ImageCtrlMouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                if (e != null && imageCtrl != null)
                {
                    this.inImageCtrlFlag = true;
                    Point pt = e.GetPosition(imageCtrl);
                    this.SetStatusInfos(pt);
                    if (this.isRoiDrawMode)
                    {
                        imageCtrl.Cursor = Cursors.Pen;
                    }
                    else
                    {
                        imageCtrl.Cursor = Cursors.Cross;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// imageCtrl MouseLeave Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void ImageCtrlMouseLeave(object sender, MouseEventArgs e)
        {
            AppContext.StatusCoord = string.Empty;
            AppContext.StatusIntensity = string.Empty;
            imageCtrl.Cursor = Cursors.Arrow;
            this.inImageCtrlFlag = false;
        }

        /// <summary>
        /// ImageCtrlMouseMove Event Handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void ImageCtrlMouseMove(object sender, MouseEventArgs e)
        {
            if (this.panningmode)
            {
                if (imageCtrl.IsMouseCaptured)
                {
                    Point p = e.GetPosition(border);
                    Matrix imageMatrix = drawingArea.RenderTransform.Value;
                    imageMatrix.OffsetX = this.origin.X + (p.X - this.start.X);
                    imageMatrix.OffsetY = this.origin.Y + (p.Y - this.start.Y);
                    drawingArea.RenderTransform = new MatrixTransform(imageMatrix);
                }
            }
            else
            {
                try
                {
                    if (e != null && imageCtrl != null)
                    {
                        Point pt = e.GetPosition(imageCtrl);
                        this.SetStatusInfos(pt);
                    }
                }
                catch (Exception ex)
                {
                    Util.ReportException(ex);
                }
            }
        }

        /// <summary>
        /// This event is used for the panning feature
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event args</param>
        private void ImageCtrlMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.zoomflag)
            {
                if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                {
                    Matrix imageMatrix = imageCtrl.RenderTransform.Value;
                    imageMatrix.OffsetX = 0;
                    imageMatrix.OffsetY = 0;
                    imageMatrix.M11 = 1;
                    imageMatrix.M22 = 1;
                    drawingArea.RenderTransform = new MatrixTransform(imageMatrix);
                    
                    imageCtrl.Cursor = Cursors.Arrow;
                }
                else
                {
                    this.panningmode = true;
                    imageCtrl.CaptureMouse();

                    this.start = e.GetPosition(border);
                    this.origin.X = drawingArea.RenderTransform.Value.OffsetX;
                    this.origin.Y = drawingArea.RenderTransform.Value.OffsetY;
                 }
            }
        }

        /// <summary>
        /// This event is used for the panning feature flag
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event args</param>
        private void ImageCtrlMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.zoomflag)
            {
                this.panningmode = false;
                imageCtrl.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Right Mouse Down Event. Used to bring up the intensity graph.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event args</param>
        private void ImageCtrlMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Enable or disable context menus items
            if (this.roiObjects.Count > 0)
            {
                this.SetContextMenuStatus();
            }

            // We only do this is we have a MS type Image
             if (this.imageData is ImageSpectrumData)
             {
                 try
                 {
                     if (e != null && imageCtrl != null)
                     {
                         this.ShowIntensityPoints(e.GetPosition(imageCtrl));
                     }
                 }
                 catch (Exception ex)
                 {
                     Util.ReportException(ex);
                 }
             }

             // Do we Need this?
             if (this.viewController != null)
             {
                 this.viewController.OnMouseDown(sender, e);
             }
        }

        /// <summary>
        /// Handles the newROI command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e">object specifying the event.</param>
        private void OnCmdNewRoi(object sender, ExecutedRoutedEventArgs e)
        {
            // No Longer used - Sub menu's
        }

        /// <summary>
        /// Sub menu Item Clicked
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Args</param>
        private void OnCmdSubNewRoi(object sender, RoutedEventArgs e)
        {
            // 1) Find out which menu item we have chosen
            string chosenRoiHeader = ((MenuItem)sender).Header.ToString();
            
            this.NewRoi(chosenRoiHeader);
        }

        /// <summary>
        /// Handles the CloseRoi command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e">object specifying the event.</param>
        private void OnCmdCloseRoi(object sender, RoutedEventArgs e)
        {
            Point firstPoint = this.currentselectedroithumbs[0].Position;
            Point lastPoint = this.currentselectedroithumbs[this.currentselectedroithumbs.Count - 1].Position;

            if (firstPoint != lastPoint)
            {
                this.CloseCurrentRoi();
            }
        }

        /// <summary>
        /// Handles the StopRoi command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e">object specifying the event.</param>
        private void OnCmdFixRoi(object sender, RoutedEventArgs e)
        {
            // Adding this here in the case where an image file is open outside the Roi Project Window
            if (this.currentselectedroiobject.ImageFilePath == string.Empty)
            {
                this.currentselectedroiobject.ImageFilePath = this.document.FileName;
            }
            
            this.FixRoi();
            
            Mouse.OverrideCursor = null;
        }
        
        /// <summary>
        /// Handles the DeleteRoi command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e">object specifying the event.</param>
        private void OnCmdDeleteRoi(object sender, RoutedEventArgs e)
        {
            this.DeleteRoi();
        }

        /// <summary>
        /// Handles the EditRoi command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e">object specifying the event.</param>
        private void OnCmdEditRoi(object sender, RoutedEventArgs e)
        {
            this.EditRoi();
        }
        
        /// <summary>
        /// Handles the Zoom command event.
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e">object specifying the event.</param>
        private void OnCmdZoom(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.mnuZoom.IsChecked)
                {
                    this.mnuZoom.IsChecked = false;
                    this.zoomflag = false;
                    this.mousewheelmode = MouseWheelMode.None;
                }
                else
                {
                    this.mnuZoom.IsChecked = true;
                    this.zoomflag = true;
                    this.mousewheelmode = MouseWheelMode.ZoomMode;
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// View Roi Data
        /// </summary>
        /// <param name="sender">The UI-Element sending the event.</param>
        /// <param name="e"><see cref="RoutedEventArgs"/>object specifying the event.</param>
        private void OnCmdViewRoiDataCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create RegionOfInterestDataSetView and Populate RoiObjects
                var roiDataSetView = new RegionOfInterestDataSetView();

                foreach (RegionOfInterest roiobjects in this.RoiObjects)
                {
                    roiDataSetView.ObservableRoiObjects.Add(roiobjects);
                }

                roiDataSetView.ShowDialog();
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }
        
        /// <summary>
        /// Event Handler for Preview Mouse Left Button down - this will contain
        /// Code for handling the ROI drawing routines.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Handler</param>
        private void WindowPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.isRoiDrawMode)
            {
                Point thumbPosition = e.GetPosition(this.drawingArea);
                
                if (this.firsttimepass)
                {
                    if (this.inImageCtrlFlag)
                    {
                        var newThumb = new RoiThumb(
                                                    this.currentthumbId, 
                                                    this.Resources["BasicShape1"] as ControlTemplate, 
                                                    "/MsiCore;component/Images/thumb6x6.png", 
                                                    thumbPosition,
                                                    this.OnDragDelta);
                        
                        newThumb.MouseEnter += this.OnMouseThumbEnter;
                        newThumb.MouseLeave += this.OnMouseThumbLeave;
                        this.currentthumbId += 1;
                        this.sourcethumb = newThumb;
                        this.startthumb = newThumb;
                        this.currentselectedroithumbs.Add(newThumb);

                        this.drawingArea.Children.Add(newThumb);

                        this.firsttimepass = false;

                        e.Handled = true;
                    }
                }
                else
                {
                    if (this.inImageCtrlFlag)
                    {
                        var newThumb = new RoiThumb(
                            this.currentthumbId,
                            this.Resources["BasicShape1"] as ControlTemplate,
                            "/MsiCore;component/Images/thumb6x6.png",
                            thumbPosition,
                            this.OnDragDelta);

                        newThumb.MouseEnter += this.OnMouseThumbEnter;
                        newThumb.MouseLeave += this.OnMouseThumbLeave;

                        this.currentthumbId += 1;
                        this.targetthumb = newThumb;
                        this.currentselectedroithumbs.Add(newThumb);

                        // Put newly created thumb on the canvas
                        this.drawingArea.Children.Add(newThumb);

                        this.link = new LineGeometry(thumbPosition, thumbPosition);
                        this.connectors.Children.Add(this.link);

                        this.link.EndPoint = thumbPosition;
                        this.sourcethumb.LinkTo(this.targetthumb, this.link);

                        this.sourcethumb = newThumb;

                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Event Handler for entering thumb area
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Args</param>
        private void OnMouseThumbEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            this.ShowThumbPos(e);
        }

        /// <summary>
        /// Event Handler for leaving thumb area
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Args</param>
        private void OnMouseThumbLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Cross;
        }
        
        /// <summary>
        /// Used to get out of ROI Draw Mode
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Handler</param>
        private void MenuItemMouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
            this.isRoiDrawMode = false;
        }

        /// <summary>
        ///  Mouse move event inside the polygon
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Handler</param>
        private void OnMousePolygonMove(object sender, MouseEventArgs e)
        {
            Point pntImage = e.GetPosition(imageCtrl);
            Point pntCanvas = e.GetPosition(drawingArea);

            string pos = string.Format("ImageX: {0:F2}, ImageY: {1:F2} , CanvasX: {2:F2} CanvasY: {3:F2} ", pntImage.X, pntImage.Y, pntCanvas.X, pntCanvas.Y);

            AppContext.StatusCoord = pos;
        }

        /// <summary>
        ///  Mouse Enter event for entering a polygon
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Handler</param>
        private void OnMousePolygonEnter(object sender, MouseEventArgs e)
        {
            this.insidePolygon = true;
        }

        /// <summary>
        ///  Mouse Leave event for entering a polygon
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Handler</param>
        private void OnMousePolygonLeave(object sender, MouseEventArgs e)
        {
            this.insidePolygon = false;
        }

        /// <summary>
        ///  Mouse left down entering a polygon
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Handler</param>
        private void OnMouseLeftButtonPolygonDown(object sender, MouseEventArgs e)
        {
            var selectedpolygon = sender as Polygon;

            // Reset the previous selectedroipolygon back to old opacity
            if (this.currentselectedroipolygon != null)
            {
                this.currentselectedroipolygon.Opacity = 0.25;
            }

            if (selectedpolygon != null)
            {
                selectedpolygon.Opacity = 0.5;
                this.currentselectedroipolygon = selectedpolygon;
            }
        }

        /// <summary>
        /// Mouse rightdown inside a polygon event handler
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Mouse Event Handler</param>
        private void OnMousePolygonRightButtonDown(object sender, MouseEventArgs e)
        {
            this.SetContextMenuStatus();
        }
        
        /// <summary>
        /// This routine has been written to deal with window resizing
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Size Change Event Args</param>
        private void DrawingAreaSizeChanged(object sender, SizeChangedEventArgs e)
        {
           // GeneralTransform genTransformImageToCanvas = drawingArea.TransformToDescendant(imageCtrl);
           // Point currentPoint = genTransformImageToCanvas.Transform(new Point(0, 0));
           Matrix imageMatrix = drawingArea.RenderTransform.Value;

           if (e.PreviousSize.Height != 0.0)
           {
               var imaging = this.imageData;
               if (imaging != null && imaging.XPoints / imaging.YPoints * e.NewSize.Height / e.NewSize.Width > 1)
               {
                   imageMatrix.OffsetX = imageMatrix.OffsetX / e.PreviousSize.Width * e.NewSize.Width;
                   imageMatrix.OffsetY = imageMatrix.OffsetY - ((((e.NewSize.Height - (e.NewSize.Width / this.imageData.XPoints * this.imageData.YPoints)) / 2)
                       - ((e.PreviousSize.Height - (e.PreviousSize.Width / this.imageData.XPoints * this.imageData.YPoints)) / 2)) * imageMatrix.M11);
               }
               else
               {
                   imageMatrix.OffsetY = imageMatrix.OffsetY / e.PreviousSize.Height * e.NewSize.Height;
                   imageMatrix.OffsetX = imageMatrix.OffsetX - ((((e.NewSize.Width - (e.NewSize.Height / this.imageData.YPoints * this.imageData.XPoints)) / 2)
                       - ((e.PreviousSize.Width - (e.PreviousSize.Height / this.imageData.YPoints * this.imageData.XPoints)) / 2)) * imageMatrix.M11);
               }
               
               drawingArea.RenderTransform = new MatrixTransform(imageMatrix);
           }
        }

        #endregion Events
    }
}
