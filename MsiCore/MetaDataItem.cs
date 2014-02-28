#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="MetaDataItem.cs" company="Novartis Pharma AG.">
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
using System.ComponentModel;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// This class defines a atomic metadata item. Such an item has a name and a value.
    /// The value has to corespond to the specified type and will be interpreted of being
    /// </summary>
    public class MetaDataItem : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The name of this item.
        /// </summary>
        private string name;

        /// <summary>
        /// The type of this item.
        /// </summary>
        private Type type;

        /// <summary>
        /// The value of this item.
        /// </summary>
        private object value;

        /// <summary>
        /// Can the item be modified?
        /// </summary>
        private bool mutable;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaDataItem"/> class.
        /// </summary>
        /// <param name="itemName">The name of this item.</param>
        /// <param name="itemType">The type of this item's value.</param>
        /// <param name="itemValue">This item's value.
        /// The <see cref="object"/> will be interpreted as being of the type specified by <paramref name="itemType"/>.</param>
        /// <param name="itemMutable">A <see cref="bool"/> value stating if the item could be modified.</param>
        public MetaDataItem(string itemName, Type itemType, object itemValue, bool itemMutable)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                throw new ArgumentNullException("itemName");
            }

            if (!itemType.IsPrimitive && !itemType.Equals(typeof(string)))
            {
                throw new ArgumentException("Only primitive types and 'string' are accepted as type for metadata items");
            }

            if (itemValue == null)
            {
                throw new ArgumentNullException("itemValue");
            }

            this.name = itemName;
            this.type = itemType;
            this.value = itemValue;
            this.mutable = itemMutable;
        }

        #endregion Constructor

        #region Events
        
        /// <summary>
        /// Event indicating the change of a property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the name of this item.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            
            set
            {
                if (this.name != value)
                {
                    this.name = value; 
                    this.FirePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of this item.
        /// </summary>
        public Type Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.FirePropertyChanged("Type");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of this item.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.type == value.GetType())
                {
                    this.value = value;
                    this.FirePropertyChanged("Value");
                    this.FirePropertyChanged("ValueString");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether item can be modified.
        /// </summary>
        public bool Mutable
        {
            get
            {
                return this.mutable;
            }

            set
            {
                if (value != this.mutable)
                {
                    this.mutable = value;
                    this.FirePropertyChanged("Mutable");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of this item in textual representation.
        /// </summary>
        public string ValueString
        {
            get
            {
                string result;

                try
                {
                    result = this.value.ToString();
                }
                catch (Exception)
                {
                    result = string.Empty;
                }

                return result;
            }

            set
            {
                if (value != null)
                {
                    if (this.type == typeof(string))
                    {
                        this.Value = value;
                    }
                    else
                    {
                        if (this.type == typeof(byte))
                        {
                            byte result;
                            try
                            {
                                result = byte.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(sbyte))
                        {
                            sbyte result;
                            try
                            {
                                result = sbyte.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(ushort))
                        {
                            ushort result;
                            try
                            {
                                result = ushort.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(short))
                        {
                            short result;
                            try
                            {
                                result = short.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(uint))
                        {
                            uint result;
                            try
                            {
                                result = uint.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(int))
                        {
                            int result;
                            try
                            {
                                result = int.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(ulong))
                        {
                            ulong result;
                            try
                            {
                                result = ulong.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(long))
                        {
                            long result;
                            try
                            {
                                result = long.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(bool))
                        {
                            bool result;
                            try
                            {
                                result = bool.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = false;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(char))
                        {
                            char result;
                            try
                            {
                                result = char.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = ' ';
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(float))
                        {
                            float result;
                            try
                            {
                                result = float.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                        else if (this.type == typeof(double))
                        {
                            double result;
                            try
                            {
                                result = double.Parse(value);
                            }
                            catch (Exception)
                            {
                                result = 0;
                            }

                            this.Value = result;
                        }
                    }
                }
            }
        }

        #endregion Properties

        #region Notify Property Changed Members

        /// <summary>
        /// Fire Property Changed Method
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        private void FirePropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}