#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="Palettes.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// This class manages the different palettes used to display the images.
    /// </summary>
    public class Palettes
    {
        #region Fields

        /// <summary>
        /// Palette List
        /// </summary>
        private readonly List<KeyValuePair<string, BitmapPalette>> palettesList;
        
        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Palettes"/> class
        /// </summary>
        public Palettes()
        {
            this.palettesList = new List<KeyValuePair<string, BitmapPalette>>();
            
            // the built-in palettes...
            this.palettesList.Add(new KeyValuePair<string, BitmapPalette>(Strings.GrayScalePalette, BitmapPalettes.Gray256));
            this.palettesList.Add(new KeyValuePair<string, BitmapPalette>(Strings.FalseColorPalette, this.CreateFalseColoredPalette()));
            
            // load palettes from the palette file
            string fullPath = Path.Combine(AppContext.AppExeDir, Strings.PalettesFile);
            this.palettesList.AddRange(this.LoadPalettes(fullPath));
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the count of palettes in this palettes instance.
        /// </summary>
        public int Count
        {
            get { return this.palettesList.Count; }
        }

        #endregion Properties

        #region Indexer

        /// <summary>
        /// Gets the <see cref="BitmapPalette"/> object with the given index.
        /// </summary>
        /// <param name="index">The index of the BitmapPalette to get.</param>
        /// <returns>A <see cref="BitmapPalette"/> reference.</returns>
        public BitmapPalette this[int index]
        {
            get
            {
                if (index < 0 || index >= this.palettesList.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return this.palettesList[index].Value;
            }
        }

        #endregion Indexer

        #region Methods

        /// <summary>
        /// Gets the <see cref="KeyValuePair{TKey,TValue}"/> object with the given index.
        /// </summary>
        /// <param name="index">The index of the <see cref="KeyValuePair{TKey,TValue}"/> object to retrieve.</param>
        /// <returns>A <see cref="KeyValuePair{TKey,TValue}"/> reference.</returns>
        public KeyValuePair<string, BitmapPalette> GetNamePalettePair(int index)
        {
            if (index < 0 || index >= this.palettesList.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return this.palettesList[index];
        }

        /// <summary>
        /// Create the <see cref="BitmapPalette"/> to be used with false color representation.
        /// The used algorhithm is based on dephased run of sinuscurves for the three color channels.
        /// </summary>
        /// <returns>Bitmap Palette</returns>
        private BitmapPalette CreateFalseColoredPalette()
        {
            var colors = new List<Color>();
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                // following is the original algorithm, which isn't useful cause of starting in violet and ending in violet
                // double red = Math.Sin(i * 2 * Math.PI / 255d - Math.PI);
                // double green = Math.Sin(i * 2 * Math.PI / 255d - Math.PI / 2);
                // double blue = Math.Sin(i * 2 * Math.PI / 255d);
                double red = Math.Sin(((i * 1.8 * Math.PI) / 255d) - Math.PI);
                double green = Math.Sin(((i * 1.8 * Math.PI) / 255d) - (Math.PI / 2));
                double blue = Math.Sin(i * 1.8 * Math.PI / 255d);

                red = (red + 1) * 0.5 * 255;
                green = (green + 1) * 0.5 * 255;
                blue = (blue + 1) * 0.5 * 255;

                Color palColor = Color.FromRgb((byte)red, (byte)green, (byte)blue);
                colors.Add(palColor);
            }

            return new BitmapPalette(colors);
        }

        /// <summary>
        /// Try and load all palettes defined in the given <paramref name="paletteFile"/>.
        /// The file is expected to be a correct palettes xml file.
        /// </summary>
        /// <param name="paletteFile">The fullpath xml file from which to read the palettes.</param>
        /// <returns>A list of name - BitmapPalette pairs.</returns>
        private List<KeyValuePair<string, BitmapPalette>> LoadPalettes(string paletteFile)
        {
            // to locally store the palettes
            var palettes = new List<KeyValuePair<string, BitmapPalette>>();

            if (string.IsNullOrEmpty(paletteFile))
            {
                return palettes;
            }

            if (!File.Exists(paletteFile))
            {
                return palettes;
            }

            try
            {
                XDocument xmlPalettes = XDocument.Load(paletteFile);

                // the root element "<palettes>"
                XElement root = xmlPalettes.Root;

                if (root == null || root.Name != "palettes")
                {
                    return palettes;
                }

                // iterate over unbound number of <palette> elements
                foreach (XElement palette in root.Nodes())
                {
                    if (palette == null)
                    {
                        continue;
                    }

                    if (palette.Name == "palette")
                    {
                        string paletteName;
                        var colors = new List<Color>();

                        XAttribute attrName = palette.Attribute("name");
                        if (attrName != null)
                        {
                            paletteName = attrName.Value;
                        }
                        else
                        {
                            // no unnamed palettes, sorry
                            System.Diagnostics.Debug.WriteLine("Unnamed <palette> element found in \"Palettes.xml\"! The Element will be ignored!!");
                            continue;
                        }

                        XElement colorsElement = palette.Element("colors");
                        if (colorsElement == null)
                        {
                            // no <colors> element in palette. proceed to next palette entry.
                            continue;
                        }

                        // loop over the color elements.
                        foreach (XElement color in colorsElement.Nodes())
                        {
                            if (color == null)
                            {
                                continue;
                            }

                            XAttribute attrRed    = color.Attribute("r");
                            XAttribute attrGreen  = color.Attribute("g");
                            XAttribute attrBlue   = color.Attribute("b");
                            XAttribute attrAlpha  = color.Attribute("a");

                            if (attrRed == null || attrGreen == null || attrBlue == null)
                            {
                                // no point in continuing without color information.
                                continue;
                            }

                            byte red;
                            byte green;
                            byte blue;
                            byte alpha;

                            try
                            {
                                red = byte.Parse(attrRed.Value);
                            }
                            catch (Exception)
                            {
                                // no point in continuing without color information.
                                continue;
                            }

                            try
                            {
                                green = byte.Parse(attrGreen.Value);
                            }
                            catch (Exception)
                            {
                                // no point in continuing without color information.
                                continue;
                            }

                            try
                            {
                                blue = byte.Parse(attrBlue.Value);
                            }
                            catch (Exception)
                            {
                                // no point in continuing without color information.
                                continue;
                            }

                            try
                            {
                                alpha = (byte)(attrAlpha == null ? 255 : byte.Parse(attrAlpha.Value));
                            }
                            catch (Exception)
                            {
                                // if no alph value is present set alpha to opaque.
                                alpha = 255;
                            }

                            Color palColor = Color.FromArgb(alpha, red, green, blue);
                            colors.Add(palColor);

                            // add no more then 256 colors as this is the maximum of palette color entries!
                            if (colors.Count >= 256)
                            {
                                // done with this palette
                                break;
                            }
                        }

                        // TODO -- what is the smallest nuber of palette color entries that makes any sense??
                        if (colors.Count > 1)
                        {
                            // create a new BitmapPalette object and add it to the list together with it's name...
                            palettes.Add(new KeyValuePair<string, BitmapPalette>(paletteName, new BitmapPalette(colors)));
                        }
                    }
                }

                return palettes;
            }
            catch (Exception e)
            {
                Util.ReportException(e);
                return palettes;
            }
        }

        #endregion Methods
    }
}
