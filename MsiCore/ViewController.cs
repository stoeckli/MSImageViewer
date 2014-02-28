#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ViewController.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
//
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Mandator base class of all view controller objects.
    /// </summary>
    public abstract class ViewController
    {
        #region Fields

        /// <summary>
        /// The document this view controller belongs to.
        /// </summary>
        private Document document;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewController"/> class 
        /// </summary>
        /// <param name="doc">
        /// The <see cref="Document"/> object this view controller belongs to.
        /// </param>
        /// <param name="view">
        /// The <see cref="IView"/> object this view controller belongs to.
        /// </param>
        protected ViewController(Document doc, IView view)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }

            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            this.document = doc;
            this.document.ViewControllerList.Add(this);
            this.document.ViewCollection.Add(view);
            this.View = view;
            view.SetViewController(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the View attached to this object
        /// </summary>
        public IView View { get; set; }

        /// <summary>
        /// Gets or sets the Document attached to this object
        /// </summary>
        public Document Document
        {
            get
            {
                return this.document;
            }

            set
            {
                this.document = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Insert the baseobjects representation.
        /// </summary>
        /// <param name="baseObject">
        /// Reference to the <see cref="BaseObject"/> instance.
        /// </param>
        public abstract void InsertRepresentation(BaseObject baseObject);

        /// <summary>
        /// Removes the given <paramref name="baseObject"/> from the associated view.
        /// </summary>
        /// <param name="baseObject">The <see cref="BaseObject"/>-instance to be removed.</param>
        public abstract void RemoveBaseObject(BaseObject baseObject);

        /// <summary>
        /// Insert the representation of all objects contained in the document
        /// into the view (via this viewcontroller).
        /// </summary>
        public virtual void InsertAllObjects()
        {
            BaseObjectList objects = this.document.BaseObjects;
            foreach (BaseObject baseObject in objects)
            {
                this.InsertRepresentation(baseObject);
            }
        }

        /// <summary>
        /// The given object will be drawn in it's selected representation.
        /// </summary>
        /// <param name="baseObject">A <see cref="BaseObject"/>-instance.</param>
        public abstract void SelectObject(BaseObject baseObject);

        /// <summary>
        /// The given object will be drawn in it's deselected (normal) representation.
        /// </summary>
        /// <param name="baseObject">A <see cref="BaseObject"/>-instance.</param>
        public abstract void DeselectObject(BaseObject baseObject);

        /// <summary>
        /// Print the views content.
        /// </summary>
        public virtual void Print()
        {
            // default implementation intentionally left empty...
        }

        #endregion Public Methods

        #region Event-Handling

        /// <summary>
        /// React on the view's 'MouseWheel' event.
        /// </summary>
        /// <param name="sender"><see cref="object"/>-reference to the sender of this event.</param>
        /// <param name="e"><see cref="MouseWheelEventArgs"/>-object specifying the event.</param>
        public virtual void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        /// <summary>
        /// React on the view's 'MouseWheel' event.
        /// </summary>
        /// <param name="sender"><see cref="object"/>-reference to the sender of this event.</param>
        /// <param name="e"><see cref="MouseButtonEventArgs"/>-object specifying the event.</param>
        public virtual void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// React on the view's 'KeyDown' event.
        /// </summary>
        /// <param name="sender"><see cref="object"/>-reference to the sender of this event.</param>
        /// <param name="e"><see cref="KeyEventArgs"/>-object specifying the event.</param>
        public virtual void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        #endregion Event-Handling
    }
}
