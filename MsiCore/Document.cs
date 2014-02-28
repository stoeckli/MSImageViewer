#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="Document.cs" company="Novartis Pharma AG.">
//      Copyright © 2011 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 28/11/2011 - Style Cop Updates
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2011 Novartis AG

using System;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// A document.
    /// </summary>
    public class Document
    {
        #region Fields

        /// <summary>
        /// The document's globally unique identifier.
        /// </summary>
        private readonly Guid docGuid;

        /// <summary>
        /// List of <see cref="IView"/>-references representing the views of this
        /// document.
        /// </summary>
        private ViewCollection viewCollection;

        /// <summary>
        /// The list of <see cref="ViewController"/>-objects connecting this document with
        /// the views of this document.
        /// </summary>
        private ViewControllerList viewControllerList;

        /// <summary>
        /// List of <see cref="BaseObject"/>s this document holds.
        /// </summary>
        private BaseObjectList docObjects;

        /// <summary>
        /// List of currently selected <see cref="BaseObject"/>s .
        /// </summary>
        private BaseObjectList selectedObjects;

        /// <summary>
        /// A name of the file this document was loaded from.
        /// </summary>
        private string fileName;

        /// <summary>
        /// A <see langword="string"/> representing the document's name.
        /// </summary>
        private string docName;

        /// <summary>
        /// The documents content.
        /// </summary>
        private IContent docContent;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class
        /// </summary>
        /// <remarks>
        /// Creates a new document.
        /// </remarks>
        public Document()
        {
            this.docObjects = new BaseObjectList();
            this.viewCollection = new ViewCollection();
            this.viewControllerList = new ViewControllerList();
            this.selectedObjects = new BaseObjectList();
            this.docName = string.Empty;
            this.fileName = string.Empty;
            this.docGuid = Guid.NewGuid();
            AppContext.ViewClosed += this.OnViewClosed;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the content of this document.
        /// </summary>
        public IContent Content
        {
            get
            {
                return this.docContent;
            }

            set
            {
                this.docContent = value;
            }
        }

        /// <summary>
        /// Gets this document's globaly unique identifier.
        /// </summary>
        /// <value>A <see cref="Guid"/> value.</value>
        public Guid DocumentGuid
        {
            get
            {
                return this.docGuid;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file this document was loaded from.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.fileName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this document.
        /// </summary>
        public string DocumentName
        {
            get
            {
                return this.docName;
            }

            set
            {
                this.docName = value;
            }
        }

        /// <summary>
        /// Gets the list of references representing the views of this document.
        /// </summary>
        public ViewCollection ViewCollection
        {
            get
            {
                return this.viewCollection;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="ViewController"/>-objects connecting this document with
        /// the views of this document.
        /// </summary>
        public ViewControllerList ViewControllerList
        {
            get
            {
                return this.viewControllerList;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="BaseObject"/>s contained in this document.
        /// </summary>
        public BaseObjectList BaseObjects
        {
            get
            {
                return this.docObjects;
            }
        }

        /// <summary>
        /// Gets the list of currently selected <see cref="BaseObject"/>s in this document.
        /// </summary>
        public BaseObjectList SelectedObjects
        {
            get
            {
                return this.selectedObjects;
            }
        }

        #endregion Public Properties

        #region Events
        /// <summary>
        /// React on the event that a view has been closed
        /// </summary>
        /// <param name="sender">The view that has been closed.</param>
        /// <param name="e">The event's arguments.</param>
        public void OnViewClosed(IView sender, EventArgs e)
        {
            try
            {
                if (sender != null && this.viewCollection.Contains(sender)) 
                {
                    // the view belongs to this document!
                    this.viewCollection.Remove(sender);
                    this.viewControllerList.Remove(sender.ViewController);
                    if (this.viewCollection.Count == 0)
                    {
                        // the last view has been closed
                        // let this document and all associated data vanish
                        sender.Close();
                        System.Diagnostics.Debug.Assert(this.viewControllerList.Count == 0, "No Views in the List");
                        this.viewCollection = null;
                        this.viewControllerList = null;
                        AppContext.DocumentList.Remove(this);
                        AppContext.ViewClosed -= this.OnViewClosed;
                        this.docContent = null;
                        this.docObjects = null;
                        this.selectedObjects = null;
                        this.docName = string.Empty;
                        this.fileName = string.Empty;
                        
                        GC.Collect(); // this a justifiable use of GC.Collect() since the last view of an document is closed and rather big amounts of data being freed.
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ReportException(ex);
            }
        }

        #endregion Events

        #region Public Methods

        /// <summary>
        /// Add the given <paramref name="baseObject"/> to the selection list and tell the
        /// view controllers to redraw it.
        /// </summary>
        /// <param name="baseObject"><see cref="BaseObject"/> to select.</param>
        /// <param name="deselectCurrent"><see langword="bool"/> indicating if the existing
        /// selection is to be cancelled.</param>
        public void SelectObject(BaseObject baseObject, bool deselectCurrent)
        {
            if (baseObject == null)
            {
                throw new ArgumentNullException("baseObject");
            }

            var selectees = new BaseObjectList { baseObject };
            this.SelectObjects(selectees, deselectCurrent);
        }

        /// <summary>
        /// Add the given <paramref name="baseObjects"/> to the selection list and tell the
        /// view controllers to redraw it.
        /// </summary>
        /// <param name="baseObjects"><see cref="BaseObjectList"/> holding the objects to
        /// select.</param>
        /// <param name="deselectCurrent"><see langword="bool"/> indicating if the existing
        /// selection is to be cancelled.</param>
        public void SelectObjects(BaseObjectList baseObjects, bool deselectCurrent)
        {
            var objectsToDeselect = new BaseObjectList();
            var objectsToSelect = new BaseObjectList();

            if (baseObjects == null)
            {
                throw new ArgumentNullException("baseObjects");
            }

            if (deselectCurrent)
            {
                foreach (BaseObject selectedObject in this.selectedObjects)
                {
                    selectedObject.IsSelected = false;
                    objectsToDeselect.Add(selectedObject);
                }

                this.selectedObjects.Clear();
            }

            if (this.selectedObjects.Count == 0)
            {
                foreach (BaseObject baseObject in baseObjects)
                {
                    baseObject.IsSelected = true;
                    this.selectedObjects.Add(baseObject);
                    objectsToSelect.Add(baseObject);
                }
            }
            else
            {
                foreach (BaseObject baseObject in baseObjects)
                {
                    if (this.selectedObjects.Contains(baseObject))
                    {
                        this.selectedObjects.Remove(baseObject);
                        baseObject.IsSelected = false;
                        objectsToDeselect.Add(baseObject);
                    }
                    else
                    {
                        this.selectedObjects.Add(baseObject);
                        baseObject.IsSelected = true;
                        objectsToSelect.Add(baseObject);
                    }
                }
            }

            foreach (BaseObject deselectee in objectsToDeselect)
            {
                foreach (ViewController viewCtrl in this.viewControllerList)
                {
                    deselectee.Deselect(viewCtrl);
                }
            }

            foreach (BaseObject selectee in objectsToSelect)
            {
                foreach (ViewController viewCtrl in this.viewControllerList)
                {
                    selectee.Select(viewCtrl);
                }
            }
        }

        /// <summary>
        /// Searches and returns the <see cref="BaseObject"/> matching the given
        /// <paramref name="objectId"/> or null if no such object is found.
        /// </summary>
        /// <param name="objectId">A <see cref="Guid"/>-value as object identifier.
        /// </param>
        /// <returns>A <see cref="BaseObject"/>-reference if the objects is found,
        /// <see langword="null"/> otherwise.</returns>
        public BaseObject FindByObjectId(Guid objectId)
        {
            if (objectId == Guid.Empty)
            {
                throw new ArgumentException("Guid is empty");
            }

            foreach (BaseObject baseObject in this.docObjects)
            {
                if (baseObject.ObjectGuid == objectId)
                {
                    return baseObject;
                }
            }

            return null;
        }

        /// <summary>
        /// Inserts the given <paramref name="baseObject"/> into this document.
        /// </summary>
        /// <param name="baseObject">
        /// A <see cref="BaseObject"/> reference of the object to be inserted.
        /// </param>
        public void Insert(BaseObject baseObject)
        {
            if (baseObject == null)
            {
                throw new ArgumentNullException("baseObject");
            }

            this.docObjects.Add(baseObject);
        }

        #endregion Public Methods
    }
}
