#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="CommonDialog.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author:  Bernhard Rode, wega Informatik AG
// Updated: Jayesh Patel - Style Cop Updates
// Updated: Markus Stoecklie - Filter changes
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Wraps the Vista-Style common Open- and SaveAs-Dialogs
    /// </summary>
    public class CommonDialog
    {
        #region Fields
        
        /// <summary>
        /// Structure used when displaying Open and SaveAs dialogs 
        /// </summary>
        private readonly OpenFileName ofn = new OpenFileName();

        /// <summary>
        /// List of filters to display in the dialog. 
        /// </summary>
        private readonly Collection<FilterEntry> filters = new Collection<FilterEntry>();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDialog"/> class.
        /// </summary>
        public CommonDialog()
        {
            // Initialize structure that is passed to the API functions.
            this.ofn.structSize = Marshal.SizeOf(this.ofn);
            this.ofn.file = new string(new char[260]);
            this.ofn.maxFile = this.ofn.file.Length;
            this.ofn.fileTitle = new string(new char[100]);
            this.ofn.maxFileTitle = this.ofn.fileTitle.Length;
        }

        #endregion Constructor

        #region Enums

        /// <summary>
        /// Enum for OpenFileNameFlags
        /// </summary>
        [Flags]
        private enum OpenFileNameFlags
        {
            OFN_READONLY = 0x00000001,
            OFN_OVERWRITEPROMPT = 0x00000002,
            OFN_HIDEREADONLY = 0x00000004,
            OFN_NOCHANGEDIR = 0x00000008,
            OFN_SHOWHELP = 0x00000010,
            OFN_ENABLEHOOK = 0x00000020,
            OFN_ENABLETEMPLATE = 0x00000040,
            OFN_ENABLETEMPLATEHANDLE = 0x00000080,
            OFN_NOVALIDATE = 0x00000100,
            OFN_ALLOWMULTISELECT = 0x00000200,
            OFN_EXTENSIONDIFFERENT = 0x00000400,
            OFN_PATHMUSTEXIST = 0x00000800,
            OFN_FILEMUSTEXIST = 0x00001000,
            OFN_CREATEPROMPT = 0x00002000,
            OFN_SHAREAWARE = 0x00004000,
            OFN_NOREADONLYRETURN = 0x00008000,
            OFN_NOTESTFILECREATE = 0x00010000,
            OFN_NONETWORKBUTTON = 0x00020000,
            OFN_NOLONGNAMES = 0x00040000,
            OFN_EXPLORER = 0x00080000,
            OFN_NODEREFERENCELINKS = 0x00100000,
            OFN_LONGNAMES = 0x00200000,
            OFN_ENABLEINCLUDENOTIFY = 0x00400000,
            OFN_ENABLESIZING = 0x00800000,
            OFN_DONTADDTORECENT = 0x02000000,
            OFN_FORCESHOWHIDDEN = 0x10000000
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets the list of filers assigned to this CommonDialog-object.
        /// </summary>
        public Collection<FilterEntry> Filters
        {
            get
            {
                return this.filters;
            }
        }

        /// <summary>
        /// Gets or sets the title of this CommonDialog-object.
        /// </summary>
        public string Title
        {
            get
            {
                return this.ofn.title;
            }

            set
            {
                this.ofn.title = value;
            }
        }

        /// <summary>
        /// Gets or sets the initial directory to look for files.
        /// </summary>
        public string InitialDirectory
        {
            get
            {
                return this.ofn.initialDir;
            }

            set
            {
                this.ofn.initialDir = value;
            }
        }

        /// <summary>
        /// Gets or sets the default extension (filter).
        /// </summary>
        public string DefaultExtension
        {
            get
            {
                return this.ofn.defExt;
            }

            set
            {
                this.ofn.defExt = value;
            }
        }

        /// <summary>
        /// Gets or sets (proposal) the filename of the file selected by this CommonDialog.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.ofn.file;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var nc = new char[260];
                    var oc = value.ToCharArray();
                    for (int i = 0; i < oc.Length; i++)
                    {
                        nc[i] = oc[i];
                    }

                    this.ofn.file = new string(nc);
                    this.ofn.maxFile = this.ofn.file.Length;
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the filter selected for the file operation.
        /// </summary>
        public int FilterIndex
        {
            get
            {
                return this.ofn.filterIndex;
            }

            set
            {
                this.ofn.filterIndex = value;
            }
        }

        #endregion
    
        #region Methods

        /// <summary>
        /// Display the Vista-style common Open dialog.
        /// </summary>
        /// <returns>True or False</returns>
        public bool ShowOpen()
        {
            this.SetFilters();
            this.ofn.flags = (int)OpenFileNameFlags.OFN_FILEMUSTEXIST;

            if (Application.Current.MainWindow != null)
            {
                this.ofn.owner = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            }

            return NativeMethods.GetOpenFileName(this.ofn);
        }

        /// <summary>
        /// Display the Vista-style common Save As dialog.
        /// </summary>
        /// <returns> True or False</returns>
        public bool ShowSaveAs()
        {
            this.SetFilters();
            this.ofn.flags = (int)(OpenFileNameFlags.OFN_PATHMUSTEXIST | OpenFileNameFlags.OFN_OVERWRITEPROMPT);
            if (Application.Current.MainWindow != null)
            {
                this.ofn.owner = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            }

            return NativeMethods.GetSaveFileName(this.ofn);
        }
        
        /// <summary>
        /// Applies the filter-entries in filters to the filter property of the common dialog.
        /// </summary>
        private void SetFilters()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}", "All Formats (");
            foreach (FilterEntry entry in this.filters)
            {
                sb.AppendFormat("{0}{1}{2}", "*", entry.Extension, ";");
            }

            sb.Remove(sb.Length - 6, 6);
            sb.AppendFormat("{0}\0", ")");

            foreach (FilterEntry entry in this.filters)
            {
                sb.AppendFormat("{0}{1}{2}", "*", entry.Extension, ";");
            }

            sb.Remove(sb.Length - 6, 5);
            sb.AppendFormat("{0}\0", string.Empty);

            foreach (FilterEntry entry in this.filters)
            {
                sb.AppendFormat("{0}\0{1}{2}\0", entry.FileType, "*", entry.Extension);
            }
            
            sb.Append("\0\0");
            this.ofn.filter = sb.ToString();
        }

        #endregion Methods

        #region Extern OpenFile Details

        /// <summary>
        /// NativeMethods Class
        /// </summary>
        private static class NativeMethods
        {
            [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

            [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
        }

        /// <summary>
        /// Open File Name Class
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class OpenFileName
        {
            internal int structSize;
            internal IntPtr owner;
            internal IntPtr instance;
            internal string filter;
            internal string customFilter;
            internal int maxCustFilter;
            internal int filterIndex;
            internal string file;
            internal int maxFile;
            internal string fileTitle;
            internal int maxFileTitle;
            internal string initialDir;
            internal string title;
            internal int flags;
            internal Int16 fileOffset;
            internal Int16 fileExtension;
            internal string defExt;
            internal IntPtr custData;
            internal IntPtr hook;
            internal string templateName;
        }

        #endregion Extern OpenFile Details
    }
}
