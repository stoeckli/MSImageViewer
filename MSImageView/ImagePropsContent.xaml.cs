#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ImagePropsContent.xaml.cs"  company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 06/12/2011 - StyleCop Updates
// Updated: Markus Stoeckli 08/03/2012 
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.MSImageView
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Novartis.Msi.Core;

    /// <summary>
    /// Interaction logic for ImagePropsContent.xaml
    /// </summary>
    public partial class ImagePropsContent
    {
        #region Fields

        /// <summary>
        /// The tooltip of the palette combobox showing the currently selected palette as an image.
        /// </summary>
        private readonly Image paletteToolTip = new Image();

        /// <summary>
        /// palette Index
        /// </summary>
        private int paletteIndex = (AppContext.Palettes.Count > 0) ? 0 : -1;

        /// <summary>
        /// Current Scan Number chosen
        /// </summary>
        private float currentmass;

        /// <summary>
        /// This is the start mass [m/z] in the MSExperiment Mass range
        /// </summary>
        private float startmass;
        
        /// <summary>
        /// This is the end mass [m/z] in the MSExperiment Mass range
        /// </summary>
        private float endmass;
        
        /// <summary>
        /// The step size for the mass range
        /// </summary>
        private float stepsize;

        /// <summary>
        /// Mass Calibration
        /// </summary>
        private float[] masscal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePropsContent"/> class
        /// </summary>
        public ImagePropsContent()
        {
            InitializeComponent();
            this.MsExpControlIsEnabledVisible(false, Visibility.Hidden);
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the current mass
        /// </summary>
        public float CurrentMass
        {
            get
            {
                return this.currentmass;
            }

            set
            {
                this.currentmass = value;
            }
        }

        /// <summary>
        /// Gets or sets the start mass
        /// </summary>
        public float StartMass
        {
            get
            {
                return this.startmass;
            }

            set
            {
                this.startmass = value;
            }
        }

        /// <summary>
        /// Gets or sets the end mass
        /// </summary>
        public float EndMass
        {
            get
            {
                return this.endmass;
            }

            set
            {
                this.endmass = value;
            }
        }

        /// <summary>
        /// Gets or sets the step size
        /// </summary>
        public float StepSize
        {
            get
            {
                return this.stepsize;
            }

            set
            {
                this.stepsize = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum intensity settings currently in effect for the active view.
        /// </summary>
        public IntensitySettings MaxIntensitySettings
        {
            get
            {
                var settings = new IntensitySettings
                    {
                        CurrentIntensity = this.slMaxIntensity.Value,
                        MinIntensity = this.slMaxIntensity.Minimum,
                        MaxIntensity = this.slMaxIntensity.Maximum,
                        StepLarge = this.slMaxIntensity.LargeChange,
                        StepSmall = this.slMaxIntensity.SmallChange
                    };

                return settings;
            }

            set
            {
                bool isEnabled = slMaxIntensity.IsEnabled;
                slMaxIntensity.IsEnabled = false;

                slMaxIntensity.Minimum = value.MinIntensity;
                slMaxIntensity.Maximum = value.MaxIntensity;
                slMaxIntensity.LargeChange = value.StepLarge;
                slMaxIntensity.SmallChange = value.StepSmall;
                slMaxIntensity.Value = value.CurrentIntensity;

                double rounded = Math.Round(slMaxIntensity.Value, 1);
                slMaxIntensity.ToolTip = rounded.ToString(CultureInfo.InvariantCulture);
                maxIntensity.Text = rounded.ToString(CultureInfo.InvariantCulture);

                slMaxIntensity.IsEnabled = isEnabled;
            }
        }

        /// <summary>
        /// Gets or sets the minimum intensity settings currently in effect for the active view.
        /// </summary>
        public IntensitySettings MinIntensitySettings
        {
            get
            {
                var settings = new IntensitySettings
                    {
                        CurrentIntensity = this.slMinIntensity.Value,
                        MinIntensity = this.slMinIntensity.Minimum,
                        MaxIntensity = this.slMinIntensity.Maximum,
                        StepLarge = this.slMinIntensity.LargeChange,
                        StepSmall = this.slMinIntensity.SmallChange
                    };

                return settings;
            }

            set
            {
                bool isEnabled = slMinIntensity.IsEnabled;
                slMinIntensity.IsEnabled = false;

                slMinIntensity.Minimum = value.MinIntensity;
                slMinIntensity.Maximum = value.MaxIntensity;
                slMinIntensity.LargeChange = value.StepLarge;
                slMinIntensity.SmallChange = value.StepSmall;
                slMinIntensity.Value = value.CurrentIntensity;

                double rounded = Math.Round(slMinIntensity.Value, 1);
                slMinIntensity.ToolTip = rounded.ToString(CultureInfo.InvariantCulture);
                minIntensity.Text = rounded.ToString(CultureInfo.InvariantCulture);

                slMinIntensity.IsEnabled = isEnabled;
            }
        }

        /// <summary>
        /// Gets or sets the index of the currently selected palette to use for image representation.
        /// </summary>
        public int PaletteIndex
        {
            get
            {
                return this.paletteIndex;
            }

            set
            {
                this.paletteIndex = value;
                cbImageRepresentation.SelectedIndex = value;
            }
        }

        /// <summary>
        /// Gets the width of the element.
        /// </summary>
        public double ElementWidth
        {
            get
            {
                return stackPanel.Width - cbImageRepresentation.Margin.Left - cbImageRepresentation.Margin.Right;
            }
        }

        /// <summary>
        /// Gets or sets the currentexperimenttype
        /// </summary>
        public ExperimentType CurrentExperimentType { get; set; }

        /// <summary>
        /// Gets or sets the Mass Calibration array
        /// </summary>
        public float[] MassCal
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

        #endregion

        #region Methods

        /// <summary>
        /// Update the state of the UI.
        /// </summary>
        /// <param name="enable">Should the controls be enabled?</param>
        public void UpdateUi(bool enable)
        {
            try
            {
                cbImageRepresentation.IsEnabled = enable;
                slMaxIntensity.IsEnabled = enable;
                slMinIntensity.IsEnabled = enable;
                maxIntensity.IsEnabled = enable;
                minIntensity.IsEnabled = enable;
                cbImageRepresentation.Refresh();
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// Fill the palette combobox with the available palette representations.
        /// </summary>
        public void FillPaletteComboBox()
        {
            try
            {
                Palettes palettes = AppContext.Palettes;
                PixelFormat pixelFormat = PixelFormats.Indexed8;
                int width = (int)cbImageRepresentation.Width - 24;
                var height = (int)cbImageRepresentation.Height;
                int stride = width * (pixelFormat.BitsPerPixel / 8);

                for (int i = 0; i < palettes.Count; i++)
                {
                    // get the palette data
                    KeyValuePair<string, BitmapPalette> kvp = palettes.GetNamePalettePair(i);
                    string paletteName = kvp.Key;
                    var comboItem = new Image();

                    // the bitmapsource...
                    // allocate the bitmap's bits
                    var bits = new byte[height * stride];

                    // fill the bits with colorinformation
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bits[x + (y * stride)] = (byte)((x * 256) / width);
                        }
                    }

                    // finally create the BitmapSource object
                    BitmapSource bitmap = BitmapSource.Create(width, height, 96.0, 96.0, PixelFormats.Indexed8, kvp.Value, bits, stride);
                    comboItem.Source = bitmap;
                    comboItem.ToolTip = paletteName;
                    comboItem.Stretch = Stretch.Fill;
                    comboItem.VerticalAlignment = VerticalAlignment.Stretch;
                    comboItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                    cbImageRepresentation.Items.Add(comboItem);
                    cbImageRepresentation.Items.Add(paletteName);
                }

                this.cbImageRepresentation.SelectedIndex = this.paletteIndex;
                var currentImage = cbImageRepresentation.Items[cbImageRepresentation.SelectedIndex] as Image;
                if (currentImage != null)
                {
                    this.paletteToolTip.Source = currentImage.Source;
                    this.cbImageRepresentation.ToolTip = this.paletteToolTip;
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// This routine calculates the datapoint to a mass
        /// </summary>
        /// <param name="searchMass">Mass to Search</param>
        /// <returns>Search Mass</returns>
        public int Masstodp(float searchMass)
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

        /// <summary>
        /// Sets various properties of various controls related to 
        /// the number of scans (Called when MaxMasses is set)
        /// </summary>
        public void SetMassDetails()
        {
            if (this.masscal != null)
            {
                this.startmass = this.masscal[0];
                this.endmass = this.masscal[this.masscal.Length - 1];
            }

            this.stepsize = 1;

            slCurrMass.Minimum = 0;
            var floats = this.masscal;
            
            if (floats != null)
            {
                this.slCurrMass.Maximum = floats.Length - 1;
            }

            slCurrMass.LargeChange = 10;
            slCurrMass.SmallChange = 1;
            this.slCurrMass.Value = this.Masstodp(this.currentmass);
            this.currMass.Text = Math.Round(this.currentmass, 2).ToString(CultureInfo.InvariantCulture); 
        }

        /// <summary>
        /// This method is used to enable and disable the controls related to
        /// MS Type Experiments
        /// </summary>
        /// <param name="isEnabled">Is Control Enabled Flag</param>
        /// <param name="isVisible">Is Control Visible</param>
        public void MsExpControlIsEnabledVisible(bool isEnabled, Visibility isVisible)
        {
            MinMassLabel.IsEnabled = isEnabled;
            MaxMassLabel.IsEnabled = isEnabled;
            slCurrMass.IsEnabled = isEnabled;
            BrowseMassTB.IsEnabled = isEnabled;
            currMass.IsEnabled = isEnabled;
            currMassRangeTB.IsEnabled = isEnabled;
            ShowTICBtn.IsEnabled = isEnabled;

            MinMassLabel.Visibility = isVisible;
            MaxMassLabel.Visibility = isVisible;
            slCurrMass.Visibility = isVisible;
            BrowseMassTB.Visibility = isVisible;
            currMass.Visibility = isVisible;
            currMassRangeTB.Visibility = isVisible;
            ShowTICBtn.Visibility = isVisible;
        }

        /// <summary>
        /// Call this method when the view is changed, enables/shows the 
        /// controls and sets the current Experiment Type
        /// </summary>
        /// <param name="expType">Experiment Type</param>
        public void ViewChanged(ExperimentType expType)
        {
            switch (expType)
            {
                case ExperimentType.MRM:
                    this.MsExpControlIsEnabledVisible(false, Visibility.Hidden);
                    break;
                case ExperimentType.MS:
                case ExperimentType.NeutralGainOrLoss:
                case ExperimentType.Precursor:
                case ExperimentType.Product:
                case ExperimentType.SIM:
                    this.MsExpControlIsEnabledVisible(true, Visibility.Visible);
                    break;
            }

            this.CurrentExperimentType = expType;
        }

        /// <summary>
        /// Current Mass 
        /// </summary>
        private void CurrentMassChanged()
        {
            string inputString = currMass.Text.Trim();
            float number;
            int newMassNumber;

            bool isNumber = float.TryParse(inputString, out number);

            if (isNumber)
            {
                // Setting the slider value will kick off
                newMassNumber = this.Masstodp(number);
            }
            else
            {
                newMassNumber = 0;
            }

            slCurrMass.Value = newMassNumber;
            this.slCurrMass.ToolTip = this.masscal[newMassNumber].ToString(CultureInfo.InvariantCulture);
            this.currMass.Text = this.masscal[newMassNumber].ToString(CultureInfo.InvariantCulture);
        }

        #endregion Methods

        #region Events 

        /// <summary>
        /// Combobox Image changed
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arg</param>
        private void CbImageRepresentationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.paletteIndex != this.cbImageRepresentation.SelectedIndex)
                {
                    // Fiddling the Index here because names skew the indexing in the combo box
                    if (cbImageRepresentation.SelectedIndex % 2 == 0)
                    {
                        this.paletteIndex = cbImageRepresentation.SelectedIndex / 2;
                    }
                    else
                    {
                        this.paletteIndex = (cbImageRepresentation.SelectedIndex - 1) / 2;
                    }

                    AppContext.FirePaletteIndexChanged(this.paletteIndex);
                    var currentImage = cbImageRepresentation.Items[this.paletteIndex] as Image;
                    if (currentImage != null)
                    {
                        this.paletteToolTip.Source = currentImage.Source;
                    } 
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Minimum Intensity Value Changed
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void SlMinIntensityValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                double currentMinIntensity = e.NewValue;
                AppContext.FireMinimumIntensityChanged(currentMinIntensity);

                double rounded = Math.Round(slMinIntensity.Value, 1);
                slMinIntensity.ToolTip = rounded.ToString(CultureInfo.InvariantCulture);
                minIntensity.Text = rounded.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Maximum Intensity Value Changed
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void SlMaxIntensityValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                double currentMaxIntensity = e.NewValue;
                AppContext.FireMaximumIntensityChanged(currentMaxIntensity);

                double rounded = Math.Round(slMaxIntensity.Value, 1);
                slMaxIntensity.ToolTip = rounded.ToString(CultureInfo.InvariantCulture);
                maxIntensity.Text = rounded.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Maximum Intensity Text Changed
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void MaxIntensityTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                float value = float.Parse(maxIntensity.Text);
                if (value >= 0)
                {
                    slMaxIntensity.Maximum = Math.Max(value, slMaxIntensity.Maximum);
                    slMaxIntensity.Value = value;

                    double rounded = Math.Round(slMaxIntensity.Value, 1);
                    slMaxIntensity.ToolTip = rounded.ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        /// <summary>
        /// Minimum Intensity Text Changed
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void MinIntensityTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                float value = float.Parse(minIntensity.Text);
                if (value >= 0)
                {
                    slMinIntensity.Maximum = Math.Max(value, slMinIntensity.Maximum);
                    slMinIntensity.Value = value;

                    double rounded = Math.Round(slMinIntensity.Value, 1);
                    slMinIntensity.ToolTip = rounded.ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }
        
        /// <summary>
        /// Event for Scan Slider
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Routed Events Args</param>
        private void SlCurrScanNumberValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var massNumber = (int)Math.Round(e.NewValue, 0);
            AppContext.FireScanNumberChanged(this.masscal[massNumber]);
            this.currMass.Text = this.masscal[massNumber].ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This event sets the current Image to the TIC Image
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">TextChanged Events Args</param>
        private void ShowTicBtnClick(object sender, RoutedEventArgs e)
        {
            slCurrMass.Value = 0.00;
            currMass.Text = "0.00";
            AppContext.FireScanNumberChanged(0);
        }

        /// <summary>
        /// Event Handler for Lost Keyboard Focus Event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Args</param>
        private void CurrMassLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.CurrentMassChanged();
        }

        /// <summary>
        /// Event Handler for Key Down Event
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Key Event Arg</param>
        private void CurrMassKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.CurrentMassChanged();
            }
        }

        #endregion Events
    }
}
