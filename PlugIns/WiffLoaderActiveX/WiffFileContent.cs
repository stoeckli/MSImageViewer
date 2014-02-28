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

// This is an ugly hack due to the inability of the analyst components to open a wiff-file from
// a network location.
// If the following '#define' is in effect, the wiff file will be copied to a local temporary file
// be read from that.
#define COPY_LOCAL

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Novartis.Msi.Core;
using NETExploreDataObjects;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class encapsulates the content read from a "Wiff-File"
    /// </summary>
    public class WiffFileContent : ISpecFileContent
    {
        /// The <see cref="Document"/> this wiff content belongs to.
        private Document document;

        // the full qualified filename
        private string fileName;

        // the list of samples contained in this wiff file
        private WiffSampleList samples = new WiffSampleList();

        // the list of image Data objects and spectrum imaging data objects
        private BaseObjectList contentList = new BaseObjectList();


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="document">The <see cref="Document"/> this wiff content belongs to.</param>
        /// <param name="filePath">The wiff file whose content is to be loaded.</param>
        public WiffFileContent(Document document, string filePath)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (filePath == null)
                throw new ArgumentNullException("filePath");

            if (filePath == "")
                throw new ArgumentException("filePath is empty");

            if (!File.Exists(filePath))
            {
                string message = "file doesn't exists: " + filePath;
                throw new ArgumentException(message);
            }

            this.fileName = filePath;
            this.document = document;

        }


        /// <summary>
        /// The <see cref="Document"/> this wiff content belongs to.
        /// </summary>
        public Document Document
        {
            get { return document; }
        }

        /// <summary>
        /// The path (full qualified name) of the file associated with this object
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// Load the wiff file's content.
        /// </summary>
        /// <returns>A <see cref="bool"/> value indicating the success of the loading.</returns>
        public bool LoadContent()
        {
#if(COPY_LOCAL)
            string tempFile = "";
#endif

            bool result = false;
            FMANWiffFileControlClass wiffFileControl = null;
            FMANWiffFileClass wiffFile = null;

            try
            {
                string fileToOpen = fileName;
#if(COPY_LOCAL)
                // if file is not local, copy it to the users tempdir
                string uncName = Util.PathToUnc(fileName);
                if(!string.IsNullOrEmpty(uncName))
                {
                    string tempDir = Path.GetTempPath();
                    string file = Path.GetFileName(uncName);
                    tempFile = Path.Combine(tempDir, file);
                    fileToOpen = tempFile;
                    File.Copy(fileName, fileToOpen, true);
                    // and the scan file
                    if (File.Exists(fileName + ".scan"))
                    {
                        File.Copy(fileName + ".scan", fileToOpen + ".scan", true);
                    }
                }
#endif

                wiffFileControl = new FMANWiffFileControlClass();
                // get wiff file object from given file
                wiffFile = (FMANWiffFileClass)wiffFileControl.GetWiffFileObject(fileToOpen, 1);

                // get number of samples
                int nrSamples = wiffFile.GetActualNumberOfSamples();

#if(false) // let the user select one sample to be processed

                string[] sampleNames = new string[nrSamples];
                // the index for the samples in the wiff file is based 1 !!
                int actSample;
                for (actSample = 1; actSample <= nrSamples; actSample++)
                {
                    sampleNames[actSample - 1] = wiffFile.GetSampleName(actSample);
                }

                if (nrSamples > 1)
                {
                    // get the sample which is actually to be used...
                    SelectSampleDlg selectSample = new SelectSampleDlg(sampleNames);
                    selectSample.Owner = AppContext.MainWindow;
                    selectSample.ShowDialog();
                    if (selectSample.DialogResult == true)
                    {
                        actSample = selectSample.SelectedSampleIndex;
                    }
                    else if (selectSample.DialogResult == false)
                        return false;
                }
                else
                {
                    // just one sample in wiff file
                    actSample = 1;
                }

                // create and add the singular sample to this objects list of samples...
                WiffSample sample = new WiffSample(wiffFile, actSample);
                samples.Add(sample);


#else      // create sample objects for all the samples present in the wiff file

                // the index for the samples in the wiff file is based 1 !!
                for (int actSample = 1; actSample <= nrSamples; actSample++)
                {
                    // create and add the sample to this objects list of samples...
                    WiffSample sample = new WiffSample(wiffFile, this, actSample);
                    samples.Add(sample);
                }

#endif
                result = true;
            }
            catch (Exception e)
            {
                Util.ReportException(e);
                result = false;
            }
            finally
            {
                try
                {
                    if (wiffFile != null)
                    {
                        wiffFile.CloseWiffFile();
                        wiffFile = null;
                    }

                    wiffFileControl = null;
#if(COPY_LOCAL)
                    if (!string.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                        if (File.Exists(tempFile + ".scan"))
                        {
                            File.Delete(tempFile + ".scan");
                        }
                    }
#endif
                }
                catch (Exception) { }
            }

            return result;
        }

        /// <summary>
        /// Add the image describing object to the list of imageDataSets
        /// </summary>
        /// <param name="imageData">The imaging object.</param>
        public void Add(Imaging imageData)
        {
            if (imageData == null)
                throw new ArgumentNullException("imageData");

            contentList.Add(imageData);
        }

        /// <summary>
        /// Create the views of this content.
        /// </summary>
        /// <param name="doc">The <see cref="Document"/> object this content is associated with.</param>
        /// <returns>A <see cref="ViewCollection"/> object containing the views.</returns>
        public ViewCollection GetContentViews(Document doc)
        {
            ViewCollection views = new ViewCollection();

            foreach (Imaging dataSet in contentList)
            {
                ViewImage view = new ViewImage(doc, dataSet, dataSet.Name);
                ViewImageController viewCtrl = new ViewImageController(doc, view);
                views.Add(view);
            }

            return views;
        }

        /// <summary>
        /// Retrieve the loaded content in a list of <see cref="BaseObject"/>s.
        /// </summary>
        /// <returns>The imaging data this object contains.</returns>
        public BaseObjectList GetContent()
        {
            return contentList;
        }
    }
}
