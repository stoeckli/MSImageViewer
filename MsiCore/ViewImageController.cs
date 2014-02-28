#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ViewImageController.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// This class defines a controller for ViewImage objects. Objects of this class
    /// implement the connection of a ViewImage object and the data model (document).
    /// </summary>
    public class ViewImageController : ViewController
    {
        #region Constants

        /// <summary>
        /// The step for rotating the content in degrees.
        /// </summary>
        private const double RotationStep = 2.0;

        #endregion Constants

        #region Fields

        /// <summary>
        /// The associated view as ViewImage-object (polymorphism at work...)
        /// </summary>
        private readonly ViewImage viewImage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ViewImageController class 
        /// </summary>
        /// <param name="doc">Document attached to Object</param>
        /// <param name="view">View attached to Object</param>
        public ViewImageController(Document doc, ViewImage view)
            : base(doc, view)
        {
            this.viewImage = view;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// The given object will be drawn in it's selected representation.
        /// </summary>
        /// <param name="baseObject">A <see cref="BaseObject"/>-instance.</param>
        public override void SelectObject(BaseObject baseObject)
        {
        }

        /// <summary>
        /// The given object will be drawn in it's deselected (normal) representation.
        /// </summary>
        /// <param name="baseObject">A <see cref="BaseObject"/>-instance.</param>
        public override void DeselectObject(BaseObject baseObject)
        {
        }

        /// <summary>
        /// Insert the baseobjects representation.
        /// </summary>
        /// <param name="baseObject">
        /// Reference to the <see cref="BaseObject"/> instance.
        /// </param>
        public override void InsertRepresentation(BaseObject baseObject)
        {
        }

        /// <summary>
        /// Removes the given <paramref name="baseObject"/> from the associated view.
        /// </summary>
        /// <param name="baseObject">The <see cref="BaseObject"/>-instance to be removed.</param>
        public override void RemoveBaseObject(BaseObject baseObject)
        {
        }

        /// <summary>
        /// Adjust the view so that all current content is visible.
        /// </summary>
        public void ShowAll()
        {
        }

        /// <summary>
        /// Perform a rotation about the given <paramref name="rotationAngle"/> around the center
        /// of the associated view.
        /// </summary>
        /// <param name="rotationAngle">A <see langword="double"/>-value defining the rotation.
        /// The rotation angle is expected to be in degrees.
        /// </param>
        public virtual void Rotate(double rotationAngle)
        {
            // create a new matrix containing the desired rotation
            // and apply (concatenate) the new matrix to the existing transformation
            var matrix = new Matrix();
            var matrixTransform = this.viewImage.drawingArea.RenderTransform as MatrixTransform;
            double centerX = this.viewImage.drawingArea.ActualWidth / 2.0;
            double centerY = this.viewImage.drawingArea.ActualHeight / 2.0;
            if (matrixTransform != null)
            {
                matrix.RotateAt(rotationAngle, centerX, centerY);
                Matrix concatMatrix = Matrix.Multiply(matrixTransform.Matrix, matrix);
                matrixTransform = new MatrixTransform(concatMatrix);
            }
            else
            {
                matrix.Rotate(rotationAngle);
                matrixTransform = new MatrixTransform(matrix);
            }

            this.viewImage.drawingArea.RenderTransform = matrixTransform;
        }

        /// <summary>
        /// Perform a rotate left (counterclockwise) in the associated view.
        /// </summary>
        public virtual void RotateLeft()
        {
            this.Rotate(-RotationStep);
        }

        /// <summary>
        /// Perform a rotate right (clockwise) in the associated view.
        /// </summary>
        public virtual void RotateRight()
        {
            this.Rotate(RotationStep);
        }

        /// <summary>
        /// Print the views content.
        /// </summary>
        public override void Print()
        {
            throw new NotImplementedException("Print");
        }

        #endregion Public Methods

        #region Event-Handling

        /// <summary>
        /// React on the view's 'KeyDown' event.
        /// </summary>
        /// <param name="sender"><see cref="object"/>-reference to the sender of this event.</param>
        /// <param name="e"><see cref="KeyEventArgs"/>-object specifying the event.</param>
        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(sender, e);

            switch (e.Key)
            {
                case Key.F4:
                case Key.W:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                    {
                        // Close the view...
                        AppContext.Application.CloseView(this.View);
                    }

                    break;
            }
        }

        /// <summary>
        /// React on the view's 'MouseWheel' event.
        /// </summary>
        /// <param name="sender"><see cref="object"/>-reference to the sender of this event.</param>
        /// <param name="e"><see cref="MouseWheelEventArgs"/>-object specifying the event.</param>
        public override void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        /// <summary>
        /// React on the view's 'MouseWheel' event.
        /// </summary>
        /// <param name="sender"><see cref="object"/>-reference to the sender of this event.</param>
        /// <param name="e"><see cref="MouseButtonEventArgs"/>-object specifying the event.</param>
        public override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Keyboard.Focus(view.im);
        }

        #endregion Event-Handling
    }
}
