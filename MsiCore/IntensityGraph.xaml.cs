#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="IntensityGraph.xaml.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel Ext
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Visifire.Charts;

    /// <summary>
    /// Interaction logic for IntensityGraph.xaml
    /// </summary>
    public partial class IntensityGraph
    {
        #region Fields

        /// <summary>
        /// Intensity Poitns to map
        /// </summary>
        private readonly float[] intensitypoints;

        /// <summary>
        /// Number of Data Points
        /// </summary>
        private readonly int numberofpoints;

        /// <summary>
        /// Min Mass or Chosen Range 
        /// </summary>
        private float minmassrange;

        /// <summary>
        /// Max Mass or Chosen Range 
        /// </summary>
        private float maxmassrange;

        /// <summary>
        /// Chosen mass
        /// </summary>
        private float chosenmass;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="IntensityGraph"/> class 
        /// </summary>
        /// <param name="data">Input Data to be displayed on line chart</param>
        public IntensityGraph(float[] data)
        {
            InitializeComponent();

            this.numberofpoints = data.Length;
            this.intensitypoints = new float[this.numberofpoints];
            this.intensitypoints = data;
            this.PopulateChart();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the chosen mass
        /// </summary>
        public float ChosenMass
        {
              get
              {
                  return this.chosenmass;
              }
        }

        /// <summary>
        /// Gets the Max mass of chosen range
        /// </summary>
        public float MaxMassRange
        {
            get
            {
                return this.maxmassrange;
            }
        }

        /// <summary>
        /// Gets the Min mass of chosen range
        /// </summary>
        public float MinMassRange
        {
            get
            {
                return this.minmassrange;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// This routine populates the main chart
        /// </summary>
        public void PopulateChart()
        {
            try
            {
                // Create a new instance of DataSeries
                var dataSeries = new DataSeries
                {
                    RenderAs = RenderAs.QuickLine,
                    LineStyle = LineStyles.Solid,
                    LineThickness = 1
                };

                var blueBrush = new SolidColorBrush { Color = Colors.Blue };
  
                dataSeries.Color = blueBrush;
                dataSeries.MarkerEnabled = false;
                dataSeries.SelectionEnabled = true;
                dataSeries.SelectionMode = SelectionModes.Multiple;
                dataSeries.LightWeight = true;

                var collection = new DataPointCollection();

                // Create a DataPoint
                for (int point = 0; point < this.numberofpoints; point++)
                {
                    collection.Add(new DataPoint { YValue = this.intensitypoints[point], XValue = point });
                }
                
                dataSeries.DataPoints = new DataPointCollection();
                dataSeries.DataPoints = collection;

                IntensityChart.Series.Add(dataSeries);
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }
        
        #endregion 

        #region Events

        /// <summary>
        /// Event Handler for close button
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arggs</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Right Mouse Button down event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Object</param>
        private void IntensityChartMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // DataPoint dp = e.
            // Need to add a datapoint click event But how?
        }

        /// <summary>
        /// Left Mouse Button down event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Object</param>
        private void IntensityChartMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataPoint = sender as DataPoint;
            if (dataPoint != null)
            {
                // double xvalue  = (double)dataPoint.XValue;
                // double yvalue = (double)dataPoint.YValue;
            }
        }

        /// <summary>
        /// Right Mouse Button up event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Object</param>
        private void IntensityChartMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        #endregion Events
    }
}
