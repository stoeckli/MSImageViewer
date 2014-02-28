#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="ImzMLWriter.cs" company="Novartis Pharma AG.">
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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Novartis.Msi.Core;

namespace Novartis.Msi.PlugIns.ImzMLIO
{
    using System.Globalization;

    /// <summary>
    /// ImzMLWriter facilitates the output of <see cref="Imaging"/> data in "imzML"
    /// format.
    /// </summary>
    public class ImzMlWriter : IImagingWriter
    {
        #region Private Fields
        
        /// <summary>
        /// Supported file types
        /// </summary>
        private readonly FileTypeDescriptorList supportedFileTypes;

        /// <summary>
        /// The xml outputter ;^)
        /// </summary>
        private XmlWriter outXml;

        /// <summary>
        /// The binary outputter ;^)
        /// </summary>
        private BinaryWriter outBin;

        /// <summary>
        /// the unique id of both files identifying correlation
        /// </summary>
        private Guid uuid;

        /// <summary>
        /// containing the binary files sha-1 hash value, used for testing the file integrity
        /// </summary>
        private string sha1Hash;

        /// <summary>
        /// the data to save as imzML
        /// </summary>
        private Imaging data;

        /// <summary>
        /// the offsets in the ibd file
        /// </summary>
        private long mzlArrayStart;
        
        /// <summary>
        /// the offsets in the ibd file
        /// </summary>
        private long mzlDataStart;

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImzMlWriter"/> class 
        /// </summary>
        public ImzMlWriter()
        {
            this.supportedFileTypes = new FileTypeDescriptorList
                {
                    new FileTypeDescriptor(Strings.ImzMLFileDescription, Strings.ImzMLXmlFileExtension)
                };

            this.uuid = Guid.Empty;
            this.sha1Hash = string.Empty;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets Name of this writer.
        /// </summary>
        public string Name
        {
            get
            {
                return Strings.WriterName;
            }
        }

        /// <summary>
        /// Gets List of supported file types.
        /// </summary>
        public virtual FileTypeDescriptorList SupportedFileTypes
        {
            get
            {
                return this.supportedFileTypes;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="Imaging"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="imagingData">The image data to be written to file.</param>
        /// <param name="fileName">The file to which the image data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        public bool Write(Imaging imagingData, string fileName, bool showProgress)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            bool result = false;
            Stream xmlStream = null;
            Stream binStream = null;
            byte[] binUuid = this.uuid.ToByteArray();

            try
            {
                // get us a globally unique identifier
                this.uuid = Guid.NewGuid();

                // create the physical files for the xml and the binary data (.ibd)
                xmlStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                binStream = new FileStream(fileName.Replace(Strings.ImzMLXmlFileExtension, Strings.ImzMLBinFileExtension), FileMode.Create, FileAccess.ReadWrite);

                // create the binary writer for the .ibd file
                this.outBin = new BinaryWriter(binStream);

                // set up the xml writer...
                var outSettings = new XmlWriterSettings
                    {
                        CheckCharacters = false,
                        CloseOutput = false,
                        ConformanceLevel = ConformanceLevel.Document,
                        Encoding = Encoding.GetEncoding("iso-8859-1"),
                        Indent = true,
                        NewLineOnAttributes = false,
                        OmitXmlDeclaration = false
                    };

                // create the xml writer for the .imzML file
                this.outXml = XmlWriter.Create(xmlStream, outSettings);

                // start the document
                this.outXml.WriteStartDocument();

                // the root element including the namespace and schema definitions as attributes
                this.outXml.WriteStartElement("mzML", "http://psi.hupo.org/ms/mzml");
                this.outXml.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                this.outXml.WriteAttributeString("xsi", "schemaLocation", null, "http://psi.hupo.org/ms/mzml http://psidev.info/files/ms/mzML/xsd/mzML1.1.0_idx.xsd");
                
                // the cv list
                this.outXml.WriteStartElement("cvList");
                this.outXml.WriteAttributeString("count", "3");

                this.outXml.WriteStartElement("cv");
                this.outXml.WriteAttributeString("id", "MS");
                this.outXml.WriteAttributeString("fullName", "Proteomics Standards Initiative Mass Spectrometry Ontology");
                this.outXml.WriteAttributeString("version", "1.3.1");
                this.outXml.WriteAttributeString("URI", "http://psidev.info/ms/mzML/psi-ms.obo");
                this.outXml.WriteEndElement();

                this.outXml.WriteStartElement("cv");
                this.outXml.WriteAttributeString("id", "UO");
                this.outXml.WriteAttributeString("fullName", "Unit Ontology");
                this.outXml.WriteAttributeString("version", "1.15");
                this.outXml.WriteAttributeString("URI", "http://obo.cvs.sourceforge.net/obo/obo/ontology/phenotype/unit.obo");
                this.outXml.WriteEndElement();

                this.outXml.WriteStartElement("cv");
                this.outXml.WriteAttributeString("id", "IMS");
                this.outXml.WriteAttributeString("fullName", "Imaging MS Ontology");
                this.outXml.WriteAttributeString("version", "1.0");
                this.outXml.WriteAttributeString("URI", "http://imagingMS.obo");
                this.outXml.WriteEndElement();

                this.outXml.WriteEndElement(); // close the cvList element

                // now we've to proceed with the binary data, because we the calculate hash value
                // but first we write the uuid as 16 byte binary data
                this.outBin.Write(binUuid);

                this.data = imagingData;

                // now delegate the writing in respect to the subclass of the experiment:
                if (imagingData is ImageData)
                {
                    result = this.Write((ImageData)imagingData);
                }
                else if (imagingData is ImageSpectrumData)
                {
                    result = this.Write((ImageSpectrumData)imagingData);
                }

                this.outXml.WriteFullEndElement(); // close the root element (<mzML ...>)
                this.outXml.WriteEndDocument(); // close the document
            }
            catch (Exception e)
            {
                Util.ReportException(e);
            }
            finally
            {
                this.data = null;
                this.sha1Hash = string.Empty;
                this.uuid = Guid.Empty;
                this.mzlArrayStart = 0;
                this.mzlDataStart = 0;

                if (this.outBin != null)
                {
                    this.outBin.Flush();
                    this.outBin.Close();
                    this.outBin = null;
                }

                if (this.outXml != null)
                {
                    this.outXml.Flush();
                    this.outXml.Close();
                    this.outXml = null;
                }

                if (binStream != null)
                {
                    binStream.Close();
                }

                if (xmlStream != null)
                {
                    xmlStream.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="Imaging"/> object
        /// to the given stream.
        /// </summary>
        /// <param name="imagingData">The image data to be written to file.</param>
        /// <param name="outStream">The stream to which the image data will be written.</param>
        /// <param name="showProgress">If true, a progress indicator will be shown.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        public bool Write(Imaging imagingData, Stream outStream, bool showProgress)
        {
            throw new NotImplementedException("AnalyzeWriter.Write(Imaging imagingData, Stream outStream, bool showProgress)");
        }

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="ImageData"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="imgData"><see cref="ImageData"/> object to be written.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        private bool Write(ImageData imgData)
        {
            bool result = true;

            // remember the offset to the mzArray
            this.mzlArrayStart = this.outBin.BaseStream.Position;

            // write the m/z value of the target ion (Note MassCal doesn't exist for MRM Scans - add alternative check maybe?)
            if (imgData.MassCal != null)
            {
                this.outBin.Write(imgData.MassCal[imgData.MassCal.Length - 1]);
            }

            // remember the offset to the intensities
            this.mzlDataStart = this.outBin.BaseStream.Position;

            // write the intensities
            for (int y = 0; y < imgData.YPoints; y++)
            {
                for (int x = 0; x < imgData.XPoints; x++)
                {
                    this.outBin.Write(imgData.Data[x][y]);
                }
            }

            this.outBin.Flush();
            this.outBin.BaseStream.Flush();
            this.outBin.BaseStream.Position = 0;

            // now calculate the sha1 hash
            var sha1 = new SHA1CryptoServiceProvider();
            sha1.ComputeHash(this.outBin.BaseStream);

            this.sha1Hash = string.Empty;

            foreach (byte byteValue in sha1.Hash)
            {
                this.sha1Hash += byteValue.ToString("X2");
            }

            if (!this.WriteFileDescription(true, imgData.ObjectDocument))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Writes the imaging data specified by the given <see cref="ImageSpectrumData"/> object
        /// to a file specified by the file name.
        /// </summary>
        /// <param name="specData"><see cref="ImageSpectrumData"/> object to be written.</param>
        /// <returns>A <see cref="bool"/> value indicating the success of the write operation.</returns>
        private bool Write(ImageSpectrumData specData)
        {
            bool result = true;

            // remember the offset to the mzArray
            this.mzlArrayStart = this.outBin.BaseStream.Position;

            // write the m/z values of the spectrum
            foreach (ImageData item in specData.DataSets)
            {
                this.outBin.Write(item.MassCal[item.MassCal.Length - 1]); // item.MaxMass need to check for null reference
            }

            // remember the offset to the intensities
            this.mzlDataStart = this.outBin.BaseStream.Position;

            // write the intensities
            for (int y = 0; y < specData.YPoints; y++)
            {
                for (int x = 0; x < specData.XPoints; x++)
                {
                    foreach (ImageData item in specData.DataSets)
                    {
                        this.outBin.Write(item.Data[x][y]);
                    }
                }
            }

            this.outBin.Flush();
            this.outBin.BaseStream.Flush();
            this.outBin.BaseStream.Position = 0;

            // now calculate the sha1 hash
            var sha1 = new SHA1CryptoServiceProvider();
            sha1.ComputeHash(this.outBin.BaseStream);

            this.sha1Hash = string.Empty;

            foreach (byte byteValue in sha1.Hash)
            {
                this.sha1Hash += byteValue.ToString("X2");
            }

            if (!this.WriteFileDescription(false, specData.ObjectDocument))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Writes data to file
        /// </summary>
        /// <param name="mrmScan">mrm Scan</param>
        /// <param name="doc">Document Name</param>
        /// <returns>pass or fail</returns>
        private bool WriteFileDescription(bool mrmScan, Document doc)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // FileDescription
            this.outXml.WriteStartElement("fileDescription");

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // FileContent
            this.outXml.WriteStartElement("fileContent");

            // cvParams
            // spectrum definition
            if (mrmScan)
            {
                this.outXml.WriteStartElement("cvParam");
                this.outXml.WriteAttributeString("cvRef", "MS");
                this.outXml.WriteAttributeString("accession", "MS:1000583");
                this.outXml.WriteAttributeString("name", "SRM spectrum");
                this.outXml.WriteAttributeString("value", string.Empty);
                this.outXml.WriteEndElement();

                // cvParams
                // centroid o. profile spectrum ??
                this.outXml.WriteStartElement("cvParam");
                this.outXml.WriteAttributeString("cvRef", "MS");
                this.outXml.WriteAttributeString("accession", "MS:1000128");

                // TODO -- adjust to our type of spectra
                this.outXml.WriteAttributeString("name", "profile spectrum");
                this.outXml.WriteAttributeString("value", string.Empty);
                this.outXml.WriteEndElement();
            }
            else 
            {
                // Q1 Scan
                this.outXml.WriteStartElement("cvParam");
                this.outXml.WriteAttributeString("cvRef", "MS");
                this.outXml.WriteAttributeString("accession", "MS:1000579");
                this.outXml.WriteAttributeString("name", "MS1 spectrum");
                this.outXml.WriteAttributeString("value", string.Empty);
                this.outXml.WriteEndElement();

                // cvParams
                // centroid o. profile spectrum ??
                this.outXml.WriteStartElement("cvParam");
                this.outXml.WriteAttributeString("cvRef", "MS");
                this.outXml.WriteAttributeString("accession", "MS:1000128");
                
                // TODO -- adjust to our type of spectra
                this.outXml.WriteAttributeString("name", "profile spectrum");
                this.outXml.WriteAttributeString("value", string.Empty);
                this.outXml.WriteEndElement();
            }

            // cvParams
            // the uuid
            string ucuuid = this.uuid.ToString();
            ucuuid = ucuuid.ToUpperInvariant();
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000080");
            this.outXml.WriteAttributeString("name", "universally unique identifier");
            this.outXml.WriteAttributeString("value", "{" + ucuuid + "}");
            this.outXml.WriteEndElement();

            // cvParams
            // the sha1 hash
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000091");
            this.outXml.WriteAttributeString("name", "ibd SHA-1");
            this.outXml.WriteAttributeString("value", this.sha1Hash);
            this.outXml.WriteEndElement();

            // cvParams
            // processed vs. continuous
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000030");
            this.outXml.WriteAttributeString("name", "continuous");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // fileContent

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // sourceFileList
            this.outXml.WriteStartElement("sourceFileList");
            this.outXml.WriteAttributeString("count", "1");

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // sourceFile
            this.outXml.WriteStartElement("sourceFile");
            string name = string.Empty;
            string location = string.Empty;
            if (doc != null)
            {
                name = Path.GetFileName(doc.FileName);
                location = Path.GetDirectoryName(doc.FileName);
                location += "\\";
            }

            this.outXml.WriteAttributeString("id", "sf1");
            this.outXml.WriteAttributeString("name", name);
            this.outXml.WriteAttributeString("location", location);

            // cvParams
            // the source file format
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000562");
            this.outXml.WriteAttributeString("name", "ABI WIFF file");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            // cvParams
            // the source file nativeID format
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000770");
            this.outXml.WriteAttributeString("name", "WIFF nativeID format");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // sourceFile

            this.outXml.WriteEndElement(); // sourceFileList

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // contact
            this.outXml.WriteStartElement("contact");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000586");
            this.outXml.WriteAttributeString("name", "contact name");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000590");
            this.outXml.WriteAttributeString("name", "contact organization");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000587");
            this.outXml.WriteAttributeString("name", "contact address");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000588");
            this.outXml.WriteAttributeString("name", "contact URL");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000589");
            this.outXml.WriteAttributeString("name", "contact email");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // contact

            this.outXml.WriteEndElement(); // fileDescription

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // referenceableParamGroupList
            this.outXml.WriteStartElement("referenceableParamGroupList");
            this.outXml.WriteAttributeString("count", "4");

            // referenceableParamGroup - mzArray
            this.outXml.WriteStartElement("referenceableParamGroup");
            this.outXml.WriteAttributeString("id", "mzArray");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000576");
            this.outXml.WriteAttributeString("name", "no compression");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000514");
            this.outXml.WriteAttributeString("name", "m/z array");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteAttributeString("unitCvRef", "MS");
            this.outXml.WriteAttributeString("unitAccession", "MS:1000040");
            this.outXml.WriteAttributeString("unitName", "m/z");
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000101");
            this.outXml.WriteAttributeString("name", "external data");
            this.outXml.WriteAttributeString("value", "true");
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000521");
            this.outXml.WriteAttributeString("name", "32-bit float");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // rreferenceableParamGroup - mzArray

            // referenceableParamGroup - intensityArray
            this.outXml.WriteStartElement("referenceableParamGroup");
            this.outXml.WriteAttributeString("id", "intensityArray");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000576");
            this.outXml.WriteAttributeString("name", "no compression");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000515");
            this.outXml.WriteAttributeString("name", "intensity array");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteAttributeString("unitCvRef", "MS");
            this.outXml.WriteAttributeString("unitAccession", "MS:1000131");
            this.outXml.WriteAttributeString("unitName", "number of counts");
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000101");
            this.outXml.WriteAttributeString("name", "external data");
            this.outXml.WriteAttributeString("value", "true");
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000521");
            this.outXml.WriteAttributeString("name", "32-bit float");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // referenceableParamGroup - intensityArray

            // referenceableParamGroup - scan description
            this.outXml.WriteStartElement("referenceableParamGroup");
            this.outXml.WriteAttributeString("id", "scan1");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000093");
            this.outXml.WriteAttributeString("name", "increasing m/z scan");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000095");
            this.outXml.WriteAttributeString("name", "linear");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // referenceableParamGroup - scan description

            // referenceableParamGroup - spectrum1 description
            this.outXml.WriteStartElement("referenceableParamGroup");
            this.outXml.WriteAttributeString("id", "spectrum1");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000579");
            this.outXml.WriteAttributeString("name", "MS1 spectrum");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000511");
            this.outXml.WriteAttributeString("name", "ms level");
            this.outXml.WriteAttributeString("value", "0");
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000128");
            this.outXml.WriteAttributeString("name", "profile spectrum");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000130");
            this.outXml.WriteAttributeString("name", "positive scan");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // referenceableParamGroup - spectrum1 description

            this.outXml.WriteEndElement(); // referenceableParamGroupList

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // sampleList
            this.outXml.WriteStartElement("sampleList");
            this.outXml.WriteAttributeString("count", "1");

            // sample
            this.outXml.WriteStartElement("sample");
            this.outXml.WriteAttributeString("id", "sample1");
            this.outXml.WriteAttributeString("name", "Sample1");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000001");
            this.outXml.WriteAttributeString("name", "sample number");
            this.outXml.WriteAttributeString("value", "1");
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // sample

            this.outXml.WriteEndElement(); // sampleList

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // softwareList
            this.outXml.WriteStartElement("softwareList");
            this.outXml.WriteAttributeString("count", "1");

            // software
            string outString = "0.2";
            this.outXml.WriteStartElement("software");
            this.outXml.WriteAttributeString("id", "MSImageView");
            this.outXml.WriteAttributeString("version", outString);

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000799");
            this.outXml.WriteAttributeString("name", "custom unreleased software tool");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // software

            this.outXml.WriteEndElement(); // softwareList

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // scanSettingsList
            this.outXml.WriteStartElement("scanSettingsList");
            this.outXml.WriteAttributeString("count", "1");

            // scanSettings
            this.outXml.WriteStartElement("scanSettings");
            this.outXml.WriteAttributeString("id", "scansettings1");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000401");
            this.outXml.WriteAttributeString("name", "top down");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000410");
            this.outXml.WriteAttributeString("name", "meandering");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000480");
            this.outXml.WriteAttributeString("name", "horizontal line scan");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            outString = this.data.XPoints.ToString(CultureInfo.InvariantCulture);
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000042");
            this.outXml.WriteAttributeString("name", "max count of pixel x");
            this.outXml.WriteAttributeString("value", outString);
            this.outXml.WriteEndElement();

            outString = this.data.YPoints.ToString(CultureInfo.InvariantCulture);
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000043");
            this.outXml.WriteAttributeString("name", "max count of pixel y");
            this.outXml.WriteAttributeString("value", outString);
            this.outXml.WriteEndElement();

            outString = ((int)(this.data.Dx * 1000)).ToString(CultureInfo.InvariantCulture);
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000046");
            this.outXml.WriteAttributeString("name", "pixel size x");
            this.outXml.WriteAttributeString("value", outString);
            this.outXml.WriteAttributeString("unitCvRef", "UO");
            this.outXml.WriteAttributeString("unitAccession", "UO:0000017");
            this.outXml.WriteAttributeString("unitName", "micrometer");
            this.outXml.WriteEndElement();

            outString = ((int)(this.data.Dy * 1000)).ToString(CultureInfo.InvariantCulture);
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "IMS");
            this.outXml.WriteAttributeString("accession", "IMS:1000047");
            this.outXml.WriteAttributeString("name", "pixel size y");
            this.outXml.WriteAttributeString("value", outString);
            this.outXml.WriteAttributeString("unitCvRef", "UO");
            this.outXml.WriteAttributeString("unitAccession", "UO:0000017");
            this.outXml.WriteAttributeString("unitName", "micrometer");
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // scanSettings

            this.outXml.WriteEndElement(); // scanSettingsList

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // instrumentConfigurationList
            this.outXml.WriteStartElement("instrumentConfigurationList");
            this.outXml.WriteAttributeString("count", "1");

            // instrumentConfiguration
            this.outXml.WriteStartElement("instrumentConfiguration");
            this.outXml.WriteAttributeString("id", "FlashQuant4000QTRAP0");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000870");
            this.outXml.WriteAttributeString("name", "4000 QTRAP");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000529");
            this.outXml.WriteAttributeString("name", "instrument serial number");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // componentList
            this.outXml.WriteStartElement("componentList");
            this.outXml.WriteAttributeString("count", "1");

            // source
            this.outXml.WriteStartElement("source");
            this.outXml.WriteAttributeString("order", "1");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000075");
            this.outXml.WriteAttributeString("name", "matrix-assisted laser desorption ionization");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();

            this.outXml.WriteEndElement(); // source

            // TODO -- evantually add other components (analyzer, detector...)
            this.outXml.WriteEndElement(); // componentList
            this.outXml.WriteEndElement(); // instrumentConfiguration
            this.outXml.WriteEndElement(); // instrumentConfigurationList

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // dataProcessingList
            this.outXml.WriteStartElement("dataProcessingList");
            this.outXml.WriteAttributeString("count", "1");

            // dataProcessing
            this.outXml.WriteStartElement("dataProcessing");
            this.outXml.WriteAttributeString("id", "MSImageViewConversion");

            // processingMethod
            this.outXml.WriteStartElement("processingMethod");
            this.outXml.WriteAttributeString("order", "1");
            this.outXml.WriteAttributeString("softwareRef", "MSImageView");

            // cvParams
            this.outXml.WriteStartElement("cvParam");
            this.outXml.WriteAttributeString("cvRef", "MS");
            this.outXml.WriteAttributeString("accession", "MS:1000544");
            this.outXml.WriteAttributeString("name", "Conversion to mzML");
            this.outXml.WriteAttributeString("value", string.Empty);
            this.outXml.WriteEndElement();
            this.outXml.WriteEndElement(); // processingMethod
            this.outXml.WriteEndElement(); // dataProcessing
            this.outXml.WriteEndElement(); // dataProcessingList

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // run
            this.outXml.WriteStartElement("run");
            this.outXml.WriteAttributeString("defaultInstrumentConfigurationRef", "FlashQuant4000QTRAP0");
            this.outXml.WriteAttributeString("defaultSourceFileRef", "sf1");
            this.outXml.WriteAttributeString("id", "Experiment01");
            this.outXml.WriteAttributeString("sampleRef", "sample1");
            this.outXml.WriteAttributeString("startTimeStamp", string.Empty);

            if (mrmScan)
            {
                // as spectrum
                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // spectrumList
                int count = this.data.XPoints * this.data.YPoints;
                outString = count.ToString(CultureInfo.InvariantCulture);
                this.outXml.WriteStartElement("spectrumList");
                this.outXml.WriteAttributeString("count", outString);
                this.outXml.WriteAttributeString("defaultDataProcessingRef", "MSImageViewConversion");

                for (int y = 0; y < this.data.YPoints; y++)
                {
                    for (int x = 0; x < this.data.XPoints; x++)
                    {
                        int value = (y * this.data.XPoints) + (x + 1);
                        outString = "Scan=" + value.ToString(CultureInfo.InvariantCulture);
                        
                        // spectrum
                        this.outXml.WriteStartElement("spectrum");
                        this.outXml.WriteAttributeString("id", outString);
                        this.outXml.WriteAttributeString("defaultArrayLength", "0");
                        outString = (value - 1).ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteAttributeString("index", outString);

                        //////////////////////////////////////////////////////////////////////////////////////////////////////
                        // scanList
                        this.outXml.WriteStartElement("scanList");
                        this.outXml.WriteAttributeString("count", "1");

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "MS");
                        this.outXml.WriteAttributeString("accession", "MS:1000795");
                        this.outXml.WriteAttributeString("name", "no combination");
                        this.outXml.WriteAttributeString("value", string.Empty);
                        this.outXml.WriteEndElement();

                        // scan
                        this.outXml.WriteStartElement("scan");
                        this.outXml.WriteAttributeString("instrumentConfigurationRef", "FlashQuant4000QTRAP0");

                        this.outXml.WriteStartElement("referenceableParamGroupRef");
                        this.outXml.WriteAttributeString("ref", "scan1");
                        this.outXml.WriteEndElement();

                        // cvParams
                        outString = (x + 1).ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000050");
                        this.outXml.WriteAttributeString("name", "position x");
                        this.outXml.WriteAttributeString("value", outString);
                        this.outXml.WriteEndElement();

                        // cvParams
                        outString = (y + 1).ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000051");
                        this.outXml.WriteAttributeString("name", "position y");
                        this.outXml.WriteAttributeString("value", outString);
                        this.outXml.WriteEndElement();

                        this.outXml.WriteEndElement(); // scan
                        this.outXml.WriteEndElement(); // scanList

                        //////////////////////////////////////////////////////////////////////////////////////////////////////
                        // binaryDataArrayList
                        this.outXml.WriteStartElement("binaryDataArrayList");
                        this.outXml.WriteAttributeString("count", "2");

                        // binaryDataArray - mzArray
                        this.outXml.WriteStartElement("binaryDataArray");
                        this.outXml.WriteAttributeString("encodedLength", "0");

                        this.outXml.WriteStartElement("referenceableParamGroupRef");
                        this.outXml.WriteAttributeString("ref", "mzArray");
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000103");
                        this.outXml.WriteAttributeString("name", "external array length");
                        this.outXml.WriteAttributeString("value", "1"); // we are MRM! Just one mass
                        this.outXml.WriteEndElement();

                        // cvParams
                        outString = this.mzlArrayStart.ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000102");
                        this.outXml.WriteAttributeString("name", "external offset");
                        this.outXml.WriteAttributeString("value", outString);
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000104");
                        this.outXml.WriteAttributeString("name", "external encoded length");
                        this.outXml.WriteAttributeString("value", "4"); // one float => 4 byte;
                        this.outXml.WriteEndElement();

                        this.outXml.WriteEndElement(); // binaryDataArray

                        // binaryDataArray - intensityArray
                        this.outXml.WriteStartElement("binaryDataArray");
                        this.outXml.WriteAttributeString("encodedLength", "0");

                        this.outXml.WriteStartElement("referenceableParamGroupRef");
                        this.outXml.WriteAttributeString("ref", "intensityArray");
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000103");
                        this.outXml.WriteAttributeString("name", "external array length");
                        this.outXml.WriteAttributeString("value", "1"); // MRM, one intensity per pixel
                        this.outXml.WriteEndElement();

                        // cvParams
                        outString = (this.mzlDataStart + (((y * this.data.XPoints) + x) * 4)).ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000102");
                        this.outXml.WriteAttributeString("name", "external offset");
                        this.outXml.WriteAttributeString("value", outString);
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000104");
                        this.outXml.WriteAttributeString("name", "external encoded length");
                        this.outXml.WriteAttributeString("value", "4"); // one float => 4 byte;
                        this.outXml.WriteEndElement();
                        this.outXml.WriteEndElement(); // binaryDataArray
                        this.outXml.WriteEndElement(); // binaryDataArrayList
                        this.outXml.WriteEndElement(); // spectrum
                    }
                }

                this.outXml.WriteEndElement(); // spectrumList
            }
            else
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////
                // spectrumList
                var specData = this.data as ImageSpectrumData;
                if (specData == null)
                {
                    throw new InvalidOperationException("ImageSpectrumData expected");
                }

                int count = this.data.XPoints * this.data.YPoints;
                int massSteps = specData.DataSets.Count;
                outString = count.ToString(CultureInfo.InvariantCulture);
                this.outXml.WriteStartElement("spectrumList");
                this.outXml.WriteAttributeString("count", outString);
                this.outXml.WriteAttributeString("defaultDataProcessingRef", "MSImageViewConversion");

                for (int y = 0; y < this.data.YPoints; y++)
                {
                    for (int x = 0; x < this.data.XPoints; x++)
                    {
                        int value = (y * this.data.XPoints) + (x + 1);
                        outString = "Scan=" + value.ToString(CultureInfo.InvariantCulture);
                        
                        // spectrum
                        this.outXml.WriteStartElement("spectrum");
                        this.outXml.WriteAttributeString("id", outString);
                        this.outXml.WriteAttributeString("defaultArrayLength", "0");
                        outString = (value - 1).ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteAttributeString("index", outString);

                        //////////////////////////////////////////////////////////////////////////////////////////////////////
                        // scanList
                        this.outXml.WriteStartElement("scanList");
                        this.outXml.WriteAttributeString("count", "1");

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "MS");
                        this.outXml.WriteAttributeString("accession", "MS:1000795");
                        this.outXml.WriteAttributeString("name", "no combination");
                        this.outXml.WriteAttributeString("value", string.Empty);
                        this.outXml.WriteEndElement();

                        // scan
                        this.outXml.WriteStartElement("scan");
                        this.outXml.WriteAttributeString("instrumentConfigurationRef", "FlashQuant4000QTRAP0");

                        this.outXml.WriteStartElement("referenceableParamGroupRef");
                        this.outXml.WriteAttributeString("ref", "scan1");
                        this.outXml.WriteEndElement();

                        // cvParams
                        outString = (x + 1).ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000050");
                        this.outXml.WriteAttributeString("name", "position x");
                        this.outXml.WriteAttributeString("value", outString);
                        this.outXml.WriteEndElement();

                        // cvParams
                        outString = (y + 1).ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000051");
                        this.outXml.WriteAttributeString("name", "position y");
                        this.outXml.WriteAttributeString("value", outString);
                        this.outXml.WriteEndElement();
                        this.outXml.WriteEndElement(); // scan
                        this.outXml.WriteEndElement(); // scanList

                        //////////////////////////////////////////////////////////////////////////////////////////////////////
                        // binaryDataArrayList
                        this.outXml.WriteStartElement("binaryDataArrayList");
                        this.outXml.WriteAttributeString("count", "2");

                        // binaryDataArray - mzArray
                        this.outXml.WriteStartElement("binaryDataArray");
                        this.outXml.WriteAttributeString("encodedLength", "0");
                        this.outXml.WriteStartElement("referenceableParamGroupRef");
                        this.outXml.WriteAttributeString("ref", "mzArray");
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000103");
                        this.outXml.WriteAttributeString("name", "external array length");
                        this.outXml.WriteAttributeString("value", massSteps.ToString(CultureInfo.InvariantCulture));
                        this.outXml.WriteEndElement();

                        // cvParams
                        outString = this.mzlArrayStart.ToString(CultureInfo.InvariantCulture);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000102");
                        this.outXml.WriteAttributeString("name", "external offset");
                        this.outXml.WriteAttributeString("value", outString);
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000104");
                        this.outXml.WriteAttributeString("name", "external encoded length");
                        this.outXml.WriteAttributeString("value", (massSteps * 4).ToString(CultureInfo.InvariantCulture)); // number of masses times 4 Bytes (float);
                        this.outXml.WriteEndElement();

                        this.outXml.WriteEndElement(); // binaryDataArray

                        // binaryDataArray - intensityArray
                        this.outXml.WriteStartElement("binaryDataArray");
                        this.outXml.WriteAttributeString("encodedLength", "0");
                        this.outXml.WriteStartElement("referenceableParamGroupRef");
                        this.outXml.WriteAttributeString("ref", "intensityArray");
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000103");
                        this.outXml.WriteAttributeString("name", "external array length");
                        this.outXml.WriteAttributeString("value", massSteps.ToString(CultureInfo.InvariantCulture));
                        this.outXml.WriteEndElement();

                        // cvParams
                        long offset = this.mzlDataStart + (((y * this.data.XPoints) + x) * massSteps * 4);
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000102");
                        this.outXml.WriteAttributeString("name", "external offset");
                        this.outXml.WriteAttributeString("value", offset.ToString(CultureInfo.InvariantCulture));
                        this.outXml.WriteEndElement();

                        // cvParams
                        this.outXml.WriteStartElement("cvParam");
                        this.outXml.WriteAttributeString("cvRef", "IMS");
                        this.outXml.WriteAttributeString("accession", "IMS:1000104");
                        this.outXml.WriteAttributeString("name", "external encoded length");
                        this.outXml.WriteAttributeString("value", (massSteps * 4).ToString(CultureInfo.InvariantCulture)); // number of masses times 4 Bytes (float);
                        this.outXml.WriteEndElement();
                        this.outXml.WriteEndElement(); // binaryDataArray
                        this.outXml.WriteEndElement(); // binaryDataArrayList

                        this.outXml.WriteEndElement(); // spectrum
                    }
                }

                this.outXml.WriteEndElement(); // spectrumList
            }

            this.outXml.WriteEndElement(); // run

            return true;
        }

        #endregion Methods
    }
}
