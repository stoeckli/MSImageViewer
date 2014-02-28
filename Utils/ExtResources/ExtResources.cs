#region Copyright © 2010 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
//
// © 2010 Novartis AG. All rights reserved.
//
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
//
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2010 Novartis AG

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Novartis.Utils.ExtResources
{
    /// <summary>
    /// The <b>ExtResources</b> provides access to the resources defined in this assembly.
    /// </summary>
    public class ExtResources
    {
        #region Private Static Fields

        private static ImageSource fileOpen256;
        private static ImageSource fileSave256;
        private static ImageSource fileSaveAs256;
        private static ImageSource help256;

        #endregion Private Static Fields


        #region Public Static Properties

        /// <summary>
        /// Returns the 'FileOpen256' resource as a ImageSource reference.
        /// </summary>
        public static ImageSource FileOpen256
        {
            get
            {
                if (fileOpen256 == null)
                    fileOpen256 = FromDrawingImage(Images.FileOpen256);

                return fileOpen256;
            }
        }

        /// <summary>
        /// Returns the 'FileSave256' resource as a ImageSource reference.
        /// </summary>
        public static ImageSource FileSave256
        {
            get
            {
                if (fileSave256 == null)
                    fileSave256 = FromDrawingImage(Images.FileSave256);

                return fileSave256;
            }
        }

        /// <summary>
        /// Returns the 'FileSaveAs256' resource as a ImageSource reference.
        /// </summary>
        public static ImageSource FileSaveAs256
        {
            get
            {
                if (fileSaveAs256 == null)
                    fileSaveAs256 = FromDrawingImage(Images.FileSaveAs256);

                return fileSaveAs256;
            }
        }

        /// <summary>
        /// Returns the 'Help256' resource as a ImageSource reference.
        /// </summary>
        public static ImageSource Help256
        {
            get
            {
                if (help256 == null)
                    help256 = FromDrawingImage(Images.Help256);

                return help256;
            }
        }

        #endregion Public Static Properties


        #region Public Static Methods

        /// <summary>
        /// Converts the given Image object into an object that can be used as ImageSource object.
        /// </summary>
        /// <param name="img">The <see cref="Image"/> to convert.</param>
        /// <returns>A <see cref="BitmapImage"/> equivalent to the given <paramref name="img"/>. This object
        /// can be used as a <see cref="ImageSource"/>-reference common in WPF.</returns>
        public static BitmapImage FromDrawingImage(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                BitmapImage bImg = new BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(ms.ToArray());
                bImg.EndInit();
                return bImg;
            }
        }

        #endregion Public Static Methods

    }
}
