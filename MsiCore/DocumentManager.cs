#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="DocumentManager.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel 28/11/2011 - Implementation of new dot net Assemblies
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// This class facilitates document handling.
    /// </summary>
    public class DocumentManager
    {
        #region Public Methods

        /// <summary>
        /// Opens the document specified by the given <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName">The document to be opened.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the attempt to open a document.</returns>
        public bool OpenDocument(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            // identify the loader on the basis of the extension...
            string extension = Path.GetExtension(fileName);
            ISpecFileLoader loader = AppContext.SpecFileLoaders.FindSupportingLoader(extension);
         
            return this.OpenDocument(fileName, loader);
        }

        /// <summary>
        /// Opens a document. A fileopen dialog is brought up.
        /// </summary>
        /// <returns>A <see cref="bool"/> value indicating the success of the attempt to open a document.</returns>
        public bool OpenDocument()
        {
            // Add the filters for all register massspec file loaders
            if (AppContext.SpecFileLoaders != null && AppContext.SpecFileLoaders.Count > 0)
            {
                var openFileDialog = new CommonDialog();

                foreach (ISpecFileLoader loader in AppContext.SpecFileLoaders)
                {
                    FileTypeDescriptorList supportedFileTypes = loader.SupportedFileTypes;

                    if (supportedFileTypes != null)
                    {
                        foreach (FileTypeDescriptor fileType in supportedFileTypes)
                        {
                            if (fileType.Extensions != null)
                            {
                                foreach (string extension in fileType.Extensions)
                                {
                                    var filter = new FilterEntry(fileType.Description, extension);
                                    openFileDialog.Filters.Add(filter);
                                }
                            }
                        }
                    }
                }

                // proceed if at least one loader is ready to accept a file...
                if (openFileDialog.Filters.Count > 0)
                {
                    // Add the all files filter
                    openFileDialog.Filters.Add(new FilterEntry(Strings.AllFilesDesc, Strings.AllFilesExt));

                    openFileDialog.ShowOpen();

                    return this.OpenDocument(openFileDialog.FileName);
                }
            }

            return false;
        }

        /// <summary>
        ///  Saves the current document to hard disk
        /// </summary>
        /// <returns>True or False</returns>
        public bool DocumentSaveAs()
        {
            var writerList = new List<IWriter>();
            IView sourceView = AppContext.Application.ActiveView;
            if (sourceView == null)
            {
                return false;
            }

            // Collect bitmap writer if appropriate
            if (sourceView is IBitmapSource)
            {
                foreach (IBitmapWriter bitmapWriter in AppContext.BitmapWriters)
                {
                    writerList.Add(bitmapWriter);
                }
            }

            // Collect imaging data writer if appropriate
            if (sourceView is IImagingSource)
            {
                foreach (IImagingWriter imagingWriter in AppContext.ImagingWriters)
                {
                    writerList.Add(imagingWriter);
                }
            }
            
            var saveAsFileDialog = new CommonDialog();
            foreach (IWriter writer in writerList)
            {
                FileTypeDescriptorList supportedFileTypes = writer.SupportedFileTypes;
                if (supportedFileTypes != null)
                {
                    foreach (FileTypeDescriptor fileType in supportedFileTypes)
                    {
                        if (fileType.Extensions != null)
                        {
                            foreach (string extension in fileType.Extensions)
                            {
                                var filter = new FilterEntry(fileType.Description, extension);
                                saveAsFileDialog.Filters.Add(filter);
                            }
                        }
                    }
                }
            }

            // proceed if at least one loader is ready to accept a file...
            if (saveAsFileDialog.Filters.Count > 0)
            {
                string filename = sourceView.Title;
                char[] invalidChars = Path.GetInvalidFileNameChars();
                foreach (char ic in invalidChars)
                {
                    filename = filename.Replace(ic, '-');
                }

                filename = sourceView.Document.DocumentName + " - " + filename;
                saveAsFileDialog.FileName = filename.Replace('.', '_');
                saveAsFileDialog.ShowSaveAs();

                string fileName = saveAsFileDialog.FileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    return false;
                }

                // identify writer loader on the basis of the extension...
                string extension = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(extension))
                {
                    if (saveAsFileDialog.FilterIndex > 0 && saveAsFileDialog.FilterIndex <= saveAsFileDialog.Filters.Count + 1)
                    {
                        // Bug Fix - saveAsFileDialog indexing is 1 based and filterEntry is zero based but saveAsFileDialog also adds a header index for all the files types.
                        FilterEntry filterEntry = saveAsFileDialog.Filters[saveAsFileDialog.FilterIndex - 2];
                        if (filterEntry != null)
                        {
                            extension = filterEntry.Extension;
                            if (string.IsNullOrEmpty(extension))
                            {
                                return false;
                            }

                            fileName += extension;
                        }
                    }
                }

                IWriter theWriter = null;
                foreach (IWriter writer in writerList)
                {
                    if (writer.SupportedFileTypes.IncludesExtension(extension))
                    {
                        theWriter = writer;
                        break;
                    }
                }

                if (theWriter != null)
                {
                    if (theWriter is IBitmapWriter)
                    {
                        return ((IBitmapWriter)theWriter).Write(((IBitmapSource)sourceView).GetBitmap(), fileName, false);
                    }

                    if (theWriter is IImagingWriter)
                    {
                        return ((IImagingWriter)theWriter).Write(((IImagingSource)sourceView).GetImagingData(), fileName, false);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Load and convert the file specified by <paramref name="fileName"/> to the format specified by <paramref name="targetExt"/>
        /// and/or create a bitmap of the file. The bitmap format is defined by <paramref name="bitmapExt"/>. The result will be stored
        /// in the directory given by <paramref name="targetDir"/>. 
        /// </summary>
        /// <param name="fileName">The full qualified filename of the file to process.</param>
        /// <param name="targetDir">The directory where the results should be stored.</param>
        /// <param name="targetExt">The target format to which to convert the given file. If empty, no conversion takes place.</param>
        /// <param name="bitmapExt">If not empty a bitmap of the type specified by this extension is created from the files (image) content.</param>
        /// <param name="bitmapDir">Specifies an additional directory where a copy of the bitmap (if any) should be created.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the operation.</returns>
        public bool BatchProcess(string fileName, string targetDir, string targetExt, string bitmapExt, string bitmapDir)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return false;
            }

            // load the file...
            try
            {
                // identify the loader on the basis of the extension...
                string extension = Path.GetExtension(fileName);
                ISpecFileLoader loader = AppContext.SpecFileLoaders.FindSupportingLoader(extension);
                if (loader == null)
                {
                    return false;
                }

                // find the matching image writer
                IImagingWriter imageWriter = null;
                if (!string.IsNullOrEmpty(targetExt))
                {
                    foreach (var candidate in AppContext.ImagingWriters)
                    {
                        if (candidate != null)
                        {
                            bool found = false;
                            foreach (FileTypeDescriptor fileType in candidate.SupportedFileTypes)
                            {
                                if (fileType != null && fileType.IncludesExtension(targetExt))
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found)
                            {
                                imageWriter = candidate;
                                break;
                            }
                        }
                    }
                }

                // find the matching bitmap writer
                IBitmapWriter bitmapWriter = null;
                if (!string.IsNullOrEmpty(bitmapExt))
                {
                    foreach (IBitmapWriter candidate in AppContext.BitmapWriters)
                    {
                        if (candidate != null)
                        {
                            bool found = false;
                            foreach (FileTypeDescriptor fileType in candidate.SupportedFileTypes)
                            {
                                if (fileType != null && fileType.IncludesExtension(bitmapExt))
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (found)
                            {
                                bitmapWriter = candidate as BitmapWriter;
                                break;
                            }
                        }
                    }
                }

                if (imageWriter == null && bitmapWriter == null)
                {
                    // no appropriate writers found for requested operation 
                    return false;
                }

                // let the loader load
                ISpecFileContent specFileContent = loader.Load(fileName);
                if (specFileContent == null)
                {
                    return false;
                }

                BaseObjectList baseObjects = specFileContent.GetContent();
                if (baseObjects == null)
                {
                    return false;
                }

                string outDir = (!string.IsNullOrEmpty(targetDir) && Directory.Exists(targetDir)) ? targetDir : Path.GetDirectoryName(fileName);
                string filename = Path.GetFileNameWithoutExtension(fileName);

                // loop over the images an process
                foreach (BaseObject baseObject in baseObjects)
                {
                    var imaging = baseObject as Imaging;
                    if (imaging == null)
                    {
                        continue;
                    }

                    // perform image conversion if requested...
                    if (imageWriter != null)
                    {
                        // compose filename
                        string targetFile = filename + " - " + imaging.Name;
                        targetFile = Util.SubstitueInvalidPathChars(targetFile, '-');
                        targetFile = targetFile.Replace('.', '_');
                        targetFile = Path.Combine(outDir, targetFile + targetExt);
                        imageWriter.Write(imaging, targetFile, false);
                    }

                    // create a bitmap if required
                    if (bitmapWriter != null)
                    {
                        BitmapSourceList bitmaps = imaging.GetBitmaps();
                        if (bitmaps != null && bitmaps.Count > 0)
                        {
                            System.Windows.Media.Imaging.BitmapSource bitmap = bitmaps[0];
                            if (bitmap != null)
                            {
                                // compose filename
                                string bitmapFileName = filename + " - " + imaging.Name;
                                bitmapFileName = Util.SubstitueInvalidPathChars(bitmapFileName, '-');
                                bitmapFileName = bitmapFileName.Replace('.', '_');
                                string bitmapFilePath = Path.Combine(outDir, bitmapFileName + bitmapExt);
                                bitmapWriter.Write(bitmap, bitmapFilePath, false);
                                if (!string.IsNullOrEmpty(bitmapDir) && Directory.Exists(bitmapDir))
                                {
                                    bitmapFilePath = Path.Combine(bitmapDir, bitmapFileName + bitmapExt);
                                    bitmapWriter.Write(bitmap, bitmapFilePath, false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Util.ReportException(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// This Open document is when you're in the main window and you have an open roiProject
        /// </summary>
        /// <param name="roiObjects">List of RegionOfInterest Object</param>
        /// <returns>value indicating the success of the attempt to open a document</returns>
        public bool OpenDocument(List<RegionOfInterest> roiObjects)
        {
             // Add the filters for all register massspec file loaders
            if (AppContext.SpecFileLoaders != null && AppContext.SpecFileLoaders.Count > 0)
            {
                var openFileDialog = new CommonDialog();

                foreach (ISpecFileLoader loader in AppContext.SpecFileLoaders)
                {
                    FileTypeDescriptorList supportedFileTypes = loader.SupportedFileTypes;

                    if (supportedFileTypes != null)
                    {
                        foreach (FileTypeDescriptor fileType in supportedFileTypes)
                        {
                            if (fileType.Extensions != null)
                            {
                                foreach (string extension in fileType.Extensions)
                                {
                                    var filter = new FilterEntry(fileType.Description, extension);
                                    openFileDialog.Filters.Add(filter);
                                }
                            }
                        }
                    }
                }

                // proceed if at least one loader is ready to accept a file...
                if (openFileDialog.Filters.Count > 0)
                {
                    // Add the all files filter
                    openFileDialog.Filters.Add(new FilterEntry(Strings.AllFilesDesc, Strings.AllFilesExt));

                    openFileDialog.ShowOpen();
                    
                    return this.OpenDocument(openFileDialog.FileName, roiObjects);
                }
            }
            
            return false;
        }

        /// <summary>
        /// A new version of OpenDocument that allows views to be opened via the ROI Project UI
        /// it includes a list of ROI objects associated to this view
        /// </summary>
        /// <param name="fileName">File name/path of file containing the image</param>
        /// <param name="roiObjects">List of RegionOfInterest Object</param>
        /// <returns>value indicating the success of the attempt to open a document</returns>
        public bool OpenDocument(string fileName, List<RegionOfInterest> roiObjects)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            // identify the loader on the basis of the extension...
            string extension = Path.GetExtension(fileName);
            ISpecFileLoader loader = AppContext.SpecFileLoaders.FindSupportingLoader(extension);

            bool returnresult = this.OpenDocument(fileName, loader, roiObjects);

            return returnresult;
        }

        /// <summary>
        /// Overloaded Opens the document specified by the given <paramref name="fileName"/>
        /// and uses the specified  <paramref name="loader"/>.
        /// </summary>
        /// <param name="fileName">The document to be opened.</param>
        /// <param name="loader">The loader associated with the file type.</param>
        /// <param name="roiObjects">list of RegionOfInterest to add to the ImageView</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the attempt to open a document.</returns>
        internal bool OpenDocument(string fileName, ISpecFileLoader loader, List<RegionOfInterest> roiObjects)
        {
            if (loader == null || string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            ISpecFileContent specFileContent = loader.Load(fileName);
            if (specFileContent == null)
            {
                return false;
            }

            Document doc = specFileContent.Document;
            if (doc == null)
            {
                return false;
            }

            AppContext.DocumentList.Add(doc);

            ViewCollection views = specFileContent.GetContentViews(doc);
            
            if (views != null)
            {
                foreach (ViewImage view in views)
                {
                    // Add in the ImageDataObject to each of the roiObjects
                    // Probably not the best place to put this.. but will do for now.
                    foreach (RegionOfInterest regionofinterest in roiObjects)
                    {
                        regionofinterest.ImageData = view.GetImagingData();
                    }
                    
                    view.RoiObjects = roiObjects;
                    view.RoiInitialSetup();
                }

                foreach (IView view in views)
                {
                    AppContext.Application.AddView(view);
                    doc.ViewCollection.Add(view);
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Opens the document specified by the given <paramref name="fileName"/>
        /// and uses the specified  <paramref name="loader"/>.
        /// </summary>
        /// <param name="fileName">The document to be opened.</param>
        /// <param name="loader">The loader associated with the file type.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the attempt to open a document.</returns>
        internal bool OpenDocument(string fileName, ISpecFileLoader loader)
        {
            if (loader == null || string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            ISpecFileContent specFileContent = loader.Load(fileName);
            if (specFileContent == null)
            {
                return false;
            }

            Document doc = specFileContent.Document;
            if (doc == null)
            {
                return false;
            }

            AppContext.DocumentList.Add(doc);

            ViewCollection views = specFileContent.GetContentViews(doc);

            if (views != null)
            {
                foreach (IView view in views)
                {
                    AppContext.Application.AddView(view);
                    doc.ViewCollection.Add(view);
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion Public Methods
    }
}
