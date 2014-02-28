#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="BinNumberWindow.xaml.cs" company="Novartis Pharma AG.">
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
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// Interaction logic for BinNumberWindow.xaml
    /// </summary>
    public partial class BinNumberWindow
    {
        #region Fields

        /// <summary>
        /// Number of Data Points
        /// </summary>
        private readonly int numberofdatapoints;

        /// <summary>
        /// Points in X directions
        /// </summary>
        private readonly float pointsinXdirection;

        /// <summary>
        /// Points in Y directions
        /// </summary>
        private readonly float pointsinYdirection;

        /// <summary>
        /// New mass calibration array
        /// </summary>
        private readonly float[] masscal;

        /// <summary>
        /// Import Data Size
        /// </summary>
        private double importdatasize;
 
        /// <summary>
        /// New Min Mass DP
        /// </summary>
        private int newminmassdp;

        /// <summary>
        /// New Max Mass DP
        /// </summary>
        private int newmaxmassdp;

        /// <summary>
        /// New Number of Data Points
        /// </summary>
        private int newnumberofdatapoints;

        /// <summary>
        /// New Bin Size
        /// </summary>
        private int newbinsize;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BinNumberWindow"/> class 
        /// </summary>
        /// <param name="pointsinXdirection">Points in X dir</param>
        /// <param name="pointsinYdirection">Points in Y dir</param>
        /// <param name="massCal">Mass calibtaion array</param>
        public BinNumberWindow(float pointsinXdirection, float pointsinYdirection, float[] massCal)
        {
            InitializeComponent();

            // Set the various field values with initial (default) values
            this.masscal = massCal;
            this.numberofdatapoints = massCal.GetLength(0);
            this.pointsinXdirection = pointsinXdirection;
            this.pointsinYdirection = pointsinYdirection;
            this.newbinsize = 1;
            this.newminmassdp = 0;
            this.newmaxmassdp = this.numberofdatapoints;

            // 1048576 (1024*1024) bytes in a MB
            this.importdatasize = Math.Round(((this.pointsinXdirection * this.pointsinYdirection * this.numberofdatapoints * 4) / 1048576), 3);
            this.InitialiseWindow();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Bin Size
        /// The BinSize property exposed to external object is set to the newbinsize
        /// </summary>
        public int BinSize
        {
            get
            {
                return this.newbinsize;
            }
        }

        /// <summary>
        /// Gets the Min Mass DP
        /// The MinMass property exposed to external object is set to the mewminmassdp
        /// </summary>
        public int MinMassDP
        {
            get
            {
                return this.newminmassdp;
            }
        }

        /// <summary>
        /// Gets the Max Mass DP
        /// The MAxMass property exposed to external object is set to the newmaxmassdp
        /// </summary>
        public int MaxMassDP
        {
            get
            {
                return this.newmaxmassdp;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This routine Initialises
        /// </summary>
        private void InitialiseWindow()
        {
            this.SetUpInitialParameter();
        }

        /// <summary>
        /// This method sets up the main UI controls with the input data
        /// </summary>
        private void SetUpInitialParameter()
        {
            this.newminmassdp = 0;
            this.newmaxmassdp = this.numberofdatapoints - 1;
            this.newbinsize = 1;

            this.MinmzTB.Text = this.masscal[this.newminmassdp].ToString(CultureInfo.InvariantCulture);
            this.lbMinmz.Content = "Min m/z (" + this.masscal[this.newminmassdp].ToString(CultureInfo.InvariantCulture) + ")";
            this.MaxmzTB.Text = this.masscal[this.newmaxmassdp].ToString(CultureInfo.InvariantCulture);
            this.lbMaxmz.Content = "Max m/z (" + this.masscal[this.newmaxmassdp].ToString(CultureInfo.InvariantCulture) + ")";
            BinSizeTB.Text = this.newbinsize.ToString(CultureInfo.InvariantCulture);
            DataPointsTB.Text = this.numberofdatapoints.ToString(CultureInfo.InvariantCulture);
            ImportDataSizeTB.Text = this.importdatasize.ToString(CultureInfo.InvariantCulture) + " MB";
            FileDimTB.Text = this.pointsinXdirection.ToString(CultureInfo.InvariantCulture) + " x " + this.pointsinYdirection.ToString(CultureInfo.InvariantCulture) + " x " + this.numberofdatapoints.ToString(CultureInfo.InvariantCulture);

            this.NewBinSizeSet();
        }

        /// <summary>
        /// This routine sets the new bin number and calculates the file size
        /// </summary>
        private void NewBinSizeSet()
        {
            int intnumber;
            float floatnumber;

            if (!int.TryParse(BinSizeTB.Text.Trim(), out intnumber))
            {
                BinSizeTB.Text = "1";
            }

            if (float.Parse(BinSizeTB.Text.Trim()) < 1)
            {
                BinSizeTB.Text = "1";
            }

            if (!float.TryParse(this.MinmzTB.Text.Trim(), out floatnumber))
            {
                this.MinmzTB.Text = this.masscal[0].ToString(CultureInfo.InvariantCulture);
            }

            if (float.Parse(this.MinmzTB.Text.Trim()) < this.masscal[0])
            {
                this.MinmzTB.Text = this.masscal[0].ToString(CultureInfo.InvariantCulture);
            }

            if (!float.TryParse(this.MaxmzTB.Text.Trim(), out floatnumber))
            {
                this.MaxmzTB.Text = this.masscal[this.numberofdatapoints - 1].ToString(CultureInfo.InvariantCulture);
            }

            if (float.Parse(this.MaxmzTB.Text.Trim()) > this.masscal[this.numberofdatapoints - 1])
            {
                this.MaxmzTB.Text = this.masscal[this.numberofdatapoints - 1].ToString(CultureInfo.InvariantCulture);
            }

            if (float.Parse(this.MaxmzTB.Text.Trim()) < this.masscal[0])
            {
                this.MaxmzTB.Text = this.masscal[0].ToString(CultureInfo.InvariantCulture);
            }

            if (float.Parse(MaxmzTB.Text.Trim()) < float.Parse(MinmzTB.Text.Trim()))
            {
                this.MinmzTB.Text = this.masscal[this.Masstodp(float.Parse(this.MaxmzTB.Text.Trim()))].ToString(CultureInfo.InvariantCulture);
            }

            // 1) Set binsize
            this.newminmassdp = this.Masstodp(float.Parse(this.MinmzTB.Text.Trim()));
            this.newmaxmassdp = this.Masstodp(float.Parse(this.MaxmzTB.Text.Trim()));
            this.newbinsize = int.Parse(BinSizeTB.Text.Trim());

            // 2) Calc New number of data points
            this.newnumberofdatapoints = this.newmaxmassdp - this.newminmassdp + 1;
            if (this.newbinsize > this.newnumberofdatapoints)
            {
                BinSizeTB.Text = this.newnumberofdatapoints.ToString(CultureInfo.InvariantCulture);
                this.newbinsize = this.newnumberofdatapoints;
                this.newnumberofdatapoints = 1;
            }
            else
            {
                this.newnumberofdatapoints = (this.newmaxmassdp - this.newminmassdp + 1) / this.newbinsize;
            }

            this.newmaxmassdp = this.newminmassdp + (this.newbinsize * this.newnumberofdatapoints) - 1;

            DataPointsTB.Text = this.newnumberofdatapoints.ToString(CultureInfo.InvariantCulture);
            MaxmzTB.Text = this.masscal[this.newmaxmassdp].ToString(CultureInfo.InvariantCulture);

            // 3) calculate new ImportDataSizeTB
            this.importdatasize = Math.Round(((this.pointsinXdirection * this.pointsinYdirection * this.newnumberofdatapoints * 4) / 1024) / 1024, 3);
            ImportDataSizeTB.Text = this.importdatasize.ToString(CultureInfo.InvariantCulture) + " MB";
        }

        /// <summary>
        /// This routine calculates the datapoint to a mass
        /// </summary>
        /// <param name="searchMass">Mass to Search</param>
        /// <returns>Search mass</returns>
        private int Masstodp(float searchMass)
        {
            int i;
            if (searchMass >= this.masscal[this.masscal.Length - 1])
            {
                return this.masscal.Length - 1;
            }

            for (i = 0; (i < this.masscal.Length) & (this.masscal[i] < (searchMass - .005)); i++)
            {
            }

            return i;
        }

        #endregion

        #region Events

        /// <summary>
        /// Reset Button, resets the controls
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Args</param>
        private void ResetBtnClick(object sender, RoutedEventArgs e)
        {
            this.SetUpInitialParameter();
        }

        /// <summary>
        /// OK Button
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Event Args</param>
        private void OkBtnClick(object sender, RoutedEventArgs e)
        {
            this.NewBinSizeSet();
            DialogResult = true;
        }
        
        /// <summary>
        /// Event Handllder for Calculate button
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arg</param>
        private void CalcBtnClick(object sender, RoutedEventArgs e)
        {
            this.NewBinSizeSet();
        }

        #endregion
    }
}
