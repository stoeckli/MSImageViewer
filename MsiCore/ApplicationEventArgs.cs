#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ApplicationEventArgs.cs" company="Novartis Pharma AG.">
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

namespace Novartis.Msi.Core
{
    /// <summary>
    /// <see cref="EventArgs"/>-derived class to describe changes to global propery.
    /// </summary>
    public class ApplicationEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// App Properties
        /// </summary>
        private readonly AppProperties property;

        /// <summary>
        /// Property Value
        /// </summary>
        private readonly object propertyValue;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ApplicationEventArgs class 
        /// </summary>
        /// <param name="property">The property that has changed.</param>
        /// <param name="propertyValue">The new value of the property as an <see cref="object"/>.</param>
        public ApplicationEventArgs(AppProperties property, object propertyValue)
        {
            this.property = property;
            this.propertyValue = propertyValue;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the property that has changed.
        /// </summary>
        public AppProperties Property
        {
            get { return this.property; }
        }

        /// <summary>
        /// Gets the new value of the property as an <see cref="object"/>.
        /// </summary>
        /// <value></value>
        public object PropertyValue
        {
            get { return this.propertyValue; }
        }

        #endregion Properties
    }
}