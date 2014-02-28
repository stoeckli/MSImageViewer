#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="BaseObject.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Mandatory base class for all document objects.
    /// </summary>
    public abstract class BaseObject
    {
        #region Fields

        /// <summary>
        /// The object's globally unique identifier.
        /// </summary>
        private readonly Guid guid;

        /// <summary>
        /// The <see cref="Document"/> this object belongs to.
        /// </summary>
        private Document document;

        /// <summary>
        /// Indicates whether the object is selected or not.
        /// </summary>
        private bool selected;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class 
        /// A new <see cref="Guid"/> is assigned to the object.
        /// </summary>
        /// <param name="doc">
        /// The <see cref="Document"/> this object belongs to.
        /// </param>
        protected BaseObject(Document doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }

            this.guid = Guid.NewGuid();
            this.document = doc;
        }
        
        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether Object is selected
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.selected;
            }

            set
            {
                this.selected = value;
            }
        }

        /// <summary>
        /// Gets this object's globaly unique identifier.
        /// </summary>
        /// <value>A <see cref="Guid"/>-value.</value>
        public Guid ObjectGuid
        {
            get
            {
                return this.guid;
            }
        }

        /// <summary>
        /// Gets or sets the document this object belongs to.
        /// </summary>
        /// <value>
        /// A <see cref="Document"/> reference.
        /// </value>
        public Document ObjectDocument
        {
            get
            {
                return this.document;
            }

            set
            {
                if (value != null)
                {
                    this.document = value;
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Select this object and adjust the drawing state in the given view controller.
        /// </summary>
        /// <param name="viewCtrl"><see cref="ViewController"/>-instance in which the
        /// drawing state will be updated.</param>
        public virtual void Select(ViewController viewCtrl)
        {
            this.selected = true;
            viewCtrl.SelectObject(this);
        }

        /// <summary>
        /// Select this object and adjust the drawing state in the given view controller.
        /// </summary>
        /// <param name="viewCtrl"><see cref="ViewController"/>-instance in which the
        /// drawing state will be updated.</param>
        public virtual void Deselect(ViewController viewCtrl)
        {
            this.selected = false;
            viewCtrl.DeselectObject(this);
        }

        /// <summary>
        /// Inserts this object's image-representation in a image-view.
        /// </summary>
        /// <param name="viewCtrl">
        /// ViewController in which the image-representation is to be inserted.
        /// </param>
        public abstract void InsertImageRep(ViewImageController viewCtrl);

        /// <summary>
        /// Removes this object's representation from a view.
        /// </summary>
        /// <param name="viewCtrl">
        /// ViewController from which the representation is to be removed.
        /// </param>
        public virtual void RemoveRepresentation(ViewController viewCtrl)
        {
            viewCtrl.RemoveBaseObject(this);
        }

        /// <summary>
        /// Returns a <see cref="object"/>-instance that describes this object.
        /// </summary>
        /// <returns>A <see cref="object"/>-instance as object-reference.</returns>
        public virtual object GetToolTip()
        {
            return null;
        }

        /// <summary>
        /// React on a ToolTipOpening event associated with this object.
        /// </summary>
        /// <param name="sender"><see cref="object"/>-reference to the sender of this event.</param>
        /// <param name="e"><see cref="System.Windows.Controls.ToolTipEventArgs"/>-object specifying the event.</param>
        public virtual void ToolTipOpening(object sender, System.Windows.Controls.ToolTipEventArgs e)
        {
            return;
        }

        #endregion Public Methods
    }
}
