#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ImageMetaData.cs" company="Novartis Pharma AG.">
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
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Novartis.Msi.Core
{
    using System.Globalization;

    /// <summary>
    /// ImageMetaData Class Definition
    /// </summary>
    public class ImageMetaData : ObservableCollection<MetaDataItem>
    {
        #region Fields

        /// <summary>
        /// Metadata Templates
        /// </summary>
        private static readonly string MetaDataTemplates = Path.Combine(AppContext.AppExeDir, Strings.MetaDataTemplates);

        /// <summary>
        /// List of Meta Data Items
        /// </summary>
        private static readonly List<MetaDataItem> Templates = LoadMetaData(MetaDataTemplates);

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMetaData"/> class.
        /// </summary>
        public ImageMetaData()
        {
            if (Templates != null && Templates.Count > 0)
            {
                foreach (MetaDataItem item in Templates)
                {
                    this.Add(new MetaDataItem(item.Name, item.Type, item.Value, item.Mutable));
                }
            }
        }

        #endregion Constructor

        #region Methods 

        /// <summary>
        /// Load meta data from a file.
        /// </summary>
        /// <param name="metaDataFile">The file from which to read the metadata.</param>
        /// <returns>A list of <see cref="MetaDataItem"/> List of instances</returns>
        public static List<MetaDataItem> LoadMetaData(string metaDataFile)
        {
            var result = new List<MetaDataItem>();

            if (string.IsNullOrEmpty(metaDataFile))
            {
                return result;
            }

            if (!File.Exists(metaDataFile))
            {
                return result;
            }

            try
            {
                XDocument xmlMetaData = XDocument.Load(metaDataFile);

                // the root element "<MetaData>"
                XElement root = xmlMetaData.Root;

                if (root == null || root.Name != "MetaData")
                {
                    return result;
                }

                // iterate over unbound number of <MetDataItem> elements
                object itemValue = null; // if this is not present, assume the empty string as default
                
                foreach (XElement metaDataItem in root.Nodes())
                {
                    string itemName;
                    Type itemType; // if this is not present, assume string as default
                    bool itemMutable = true; // if this is not present, assume true as default

                    if (metaDataItem == null)
                    {
                        continue;
                    }

                    // retrieve the "Name" of the metadata item
                    XAttribute attrName = metaDataItem.Attribute("Name");
                    if (attrName != null)
                    {
                        itemName = attrName.Value;
                        if (string.IsNullOrEmpty(itemName))
                        {
                            System.Diagnostics.Debug.WriteLine("<MetaDataItem> with empty Name attribute found! The Element will be ignored!!");
                            continue;
                        }
                    }
                    else
                    {
                        // no unnamed result, sorry
                        System.Diagnostics.Debug.WriteLine("<MetaDataItem> without Name attribute found! The Element will be ignored!!");
                        continue;
                    }

                    // retrieve the "Type"  of the metadata item
                    XAttribute attrType = metaDataItem.Attribute("Type");
                    string typeString = string.Empty; 

                    if (attrType != null)
                    {
                        typeString = attrType.Value;
                    }

                    // now get the "Value" of the metadata item
                    XAttribute attrValue = metaDataItem.Attribute("Value");
                    string valueString = string.Empty;
                    if (attrValue != null)
                    {
                        valueString = attrValue.Value;
                    }

                    switch (typeString)
                    {
                        case "System.Byte":
                            {
                                itemType = typeof(byte);
                                if (valueString != string.Empty)
                                {
                                    byte value = 0;
                                    try 
                                    {
                                        value = byte.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.SByte":
                            {
                                itemType = typeof(sbyte);
                                if (valueString != string.Empty)
                                {
                                    sbyte value = 0;
                                    try 
                                    {
                                        value = sbyte.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.UInt16":
                            {
                                itemType = typeof(ushort);
                                if (valueString != string.Empty)
                                {
                                    ushort value = 0;
                                    try 
                                    {
                                        value = ushort.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.Int16":
                            {
                                itemType = typeof(short);
                                if (valueString != string.Empty)
                                {
                                    short value = 0;
                                    try 
                                    {
                                        value = short.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.UInt32":
                            {
                                itemType = typeof(uint);
                                if (valueString != string.Empty)
                                {
                                    uint value = 0;
                                    try
                                    {
                                        value = uint.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.Int32":
                            {
                                itemType = typeof(int);
                                if (valueString != string.Empty)
                                {
                                    int value = 0;
                                    try
                                    {
                                        value = int.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.UInt64":
                            {
                                itemType = typeof(ulong);
                                if (valueString != string.Empty)
                                {
                                    ulong value = 0;
                                    try 
                                    {
                                        value = ulong.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.Int64":
                            {
                                itemType = typeof(long);
                                if (valueString != string.Empty)
                                {
                                    long value = 0;
                                    try 
                                    {
                                        value = long.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.Boolean":
                            {
                                itemType = typeof(bool);
                                if (valueString != string.Empty)
                                {
                                    bool value = false;
                                    try 
                                    {
                                        value = bool.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.Char":
                            {
                                itemType = typeof(char);
                                if (valueString != string.Empty)
                                {
                                    char value = ' ';
                                    try 
                                    {
                                        value = char.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.Single":
                            {
                                itemType = typeof(float);
                                if (valueString != string.Empty)
                                {
                                    float value = 0;
                                    try 
                                    {
                                        value = float.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        case "System.Double":
                            {
                                itemType = typeof(double);
                                if (valueString != string.Empty)
                                {
                                    double value = 0;
                                    try 
                                    {
                                        value = double.Parse(valueString);
                                    }
                                    catch (Exception e)
                                    {
                                        Util.ReportException(e);
                                    }

                                    itemValue = value;
                                }
                            }

                            break;
                        default:
                            {
                                itemType = typeof(string);
                                itemValue = valueString;
                            }

                            break;
                    }

                    // retrieve the mutable property
                    XAttribute attrMutable = metaDataItem.Attribute("Mutable");
                    if (attrMutable != null)
                    {
                        if (!string.IsNullOrEmpty(attrMutable.Value))
                        {
                            try 
                            {
                                itemMutable = bool.Parse(attrMutable.Value);
                            }
                            catch (Exception e)
                            {
                                Util.ReportException(e);
                            }
                        }
                    }

                    var loadedItem = new MetaDataItem(itemName, itemType, itemValue, itemMutable);
                    result.Add(loadedItem);
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }

            return result;
        }

        /// <summary>
        /// Creates a new metadata item with the given properties and adds it to this list.
        /// </summary>
        /// <param name="itemName">Item Name</param>
        /// <param name="itemType">Item Type</param>
        /// <param name="itemValue">Item Value</param>
        /// <param name="itemMutable">Item Mutable Flag</param>
        public void Add(string itemName, Type itemType, object itemValue, bool itemMutable)
        {
            try
            {
                var item = new MetaDataItem(itemName, itemType, itemValue, itemMutable);
                Add(item);
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// Write the content of this meta data instance to the specified <paramref name="xmlFile"/> in XML format.
        /// </summary>
        /// <param name="xmlFile">A <see cref="string"/> containing the (full) file name of the file to which to write this instance data.</param>
        public void SaveToXml(string xmlFile)
        {
            try
            {
                using (Stream xmlStream = new FileStream(xmlFile, FileMode.Create, FileAccess.Write))
                {
                    // set up the xml writer...
                    var xmlSettings = new XmlWriterSettings { Indent = true };
                    XmlWriter xml = XmlWriter.Create(xmlStream, xmlSettings);

                    // start the document
                    xml.WriteStartDocument();

                    // leave a hint to the author...
                    xml.WriteComment(" Generated by " + AppContext.ApplicationTitle + " " + DateTime.Now.ToString(CultureInfo.InvariantCulture) + " ");

                    // the root element (<MetaData>)
                    xml.WriteStartElement("MetaData");

                    // iterate the list and write the item information
                    foreach (MetaDataItem item in this)
                    {
                        xml.WriteStartElement("MetaDataItem");
                        xml.WriteAttributeString("Name", item.Name);
                        xml.WriteAttributeString("Type", item.Type.ToString());
                        xml.WriteAttributeString("Value", item.ValueString);
                        xml.WriteAttributeString("Mutable", item.Mutable.ToString(CultureInfo.InvariantCulture));
                        xml.WriteEndElement();
                    }

                    xml.WriteFullEndElement(); // close the root element (</MetaData>)
                    xml.WriteEndDocument();    // close the document
                    xml.Close();
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        /// <summary>
        /// Read meta data from the specified <paramref name="xmlFile"/> and add the meta data items to this instance.
        /// </summary>
        /// <param name="xmlFile">A <see cref="string"/> containing the (full) file name of the file to which to write this instance data.</param>
        public void ReadFromXml(string xmlFile)
        {
            try
            {
                List<MetaDataItem> itemsRead = LoadMetaData(xmlFile);
                if (itemsRead != null && itemsRead.Count > 0)
                {
                    foreach (var item in itemsRead)
                    {
                        this.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
        }

        #endregion Methods
    }
}
