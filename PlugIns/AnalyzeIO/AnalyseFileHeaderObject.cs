#region Copyright © 2012 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="AnalyseFileHeaderObject.cs" company="Novartis Pharma AG.">
//      Copyright © 2012 Novartis Pharma AG. All rights reserved.
// </copyright>
// These coded instructions, statements and computer programs contain unpublished
// proprietary information of Novartis AG and are protected by federal  copyright
// law. They may not be disclosed to third parties or copied or duplicated in any
// form, in whole or in part, without the prior written consent of Novartis AG.
//
// Author: Jayesh Patel
/////////////////////////////////////////////////////////////////////////////////
#endregion Copyright © 2012 Novartis AG

namespace Novartis.Msi.PlugIns.AnalyzeIO
{
    /// <summary>
    /// The class is a created for an developer to use as an object for 
    /// analyse file reading/writing
    /// </summary>
    public class AnalyseFileHeaderObject
    {
        #region Fields

        /// <summary>
        /// This is an unused place holder 
        /// Position = 4 ("        16")
        /// </summary>
        private string unusedpos4;

        /// <summary>
        /// Data Type
        /// Position = 32;
        /// </summary>
        private int unusedpos32;

        /// <summary>
        /// This is an unused place holder
        /// Position = 38 114
        /// </summary>
        private int unusedpos38;

        /// <summary>
        /// This is an unused place holder
        /// Position = 40
        /// </summary>
        private int unusedpos40;

        /// <summary>
        /// This is an unused place holder
        /// Position = 48
        /// </summary>
        private int unusedpos48;

        /// <summary>
        /// This is an used parameter
        /// Position = 72
        /// </summary>
        private int usedpos72;

        /// <summary>
        /// Dimension in z direction
        /// Position = 76
        /// </summary>
        private float dimensioninz;

        /// <summary>
        /// This is an used place holder
        /// Position = 88
        /// </summary>
        private float usedpos88;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyseFileHeaderObject"/> class 
        /// </summary>
        /// <param name="analyseReadorWriteEnum">Read or Write call</param>
        public AnalyseFileHeaderObject(AnalyseReadorWriteEnum analyseReadorWriteEnum)
        {
            if (analyseReadorWriteEnum == AnalyseReadorWriteEnum.Read)
            {
                this.SetDefaultsRead();
            }
            else
            {
                this.SetDefaultsWrite();
            }
        }

        #endregion Constructor

        #region Enums
        /// <summary>
        /// Enum to definite read or write Analyse Object
        /// </summary>
        public enum AnalyseReadorWriteEnum
        {
            /// <summary>
            /// Set read defaults
            /// </summary>
            Read = 1,

            /// <summary>
            /// Set write defaults 
            /// </summary>
            Write
        }

        #endregion Enums

        #region Properties

        /// <summary>
        ///  Gets or sets the HeaderSize 
        /// </summary>
        public int HeaderSize { get; set; }

        /// <summary>
        /// Gets or sets unused unusedpos4
        /// </summary>
        public string UnUsedPos4
        {
            get
            {
                return this.unusedpos4;
            }

            set
            {
                this.unusedpos4 = value;
            }
        }

        /// <summary>
        /// Gets or sets unusedpos32
        /// </summary>
        public int UnUsedPos32
        {
            get
            {
                return this.unusedpos32;
            }

            set
            {
                this.unusedpos32 = value;
            }
        }

        /// <summary>
        /// Gets or sets unusedpos38
        /// </summary>
        public int UnUsedPos38
        {
            get
            {
                return this.unusedpos38;
            }

            set
            {
                this.unusedpos38 = value;
            }
        }

        /// <summary>
        /// Gets or sets unusedpos40
        /// </summary>
        public int UnUsedPos40
        {
            get
            {
                return this.unusedpos40;
            }

            set
            {
                this.unusedpos40 = value;
            }
        }

        /// <summary>
        /// Gets or sets Number of Mass scans Pos 42
        /// </summary>
        public long NumberOfScans { get; set; }

        /// <summary>
        /// Gets or sets Number of X Points
        /// </summary>
        public int NumberOfXPoints { get; set; }

        /// <summary>
        /// Gets or sets Number of Y Points
        /// </summary>
        public int NumberOfYPoints { get; set; }

        /// <summary>
        /// Gets or sets unusedpos48 place 
        /// </summary>
        public int UnUsedPos48
        {
            get
            {
                return this.unusedpos48;
            }

            set
            {
                this.unusedpos48 = value;
            }
        }

        /// <summary>
        /// Gets or sets Data Type
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Gets or sets usedpos72 parameter
        /// </summary>
        public int UsedPos72
        {
            get
            {
                return this.usedpos72;
            }

            set
            {
                this.usedpos72 = value;
            }
        }

        /// <summary>
        /// Gets or sets Dimension in z direction
        /// Position = 76
        /// </summary>
        public float DimensionInZ
        {
            get
            {
                return this.dimensioninz;
            }

            set
            {
                this.dimensioninz = value;
            }
        }

        /// <summary>
        /// Gets or sets Extent in x directon 
        /// Position = 80
        /// </summary>
        public float Dx { get; set; }

        /// <summary>
        /// Gets or sets Extent in y directon 
        ///  Position = 84
        /// </summary>
        public float Dy { get; set; }

        /// <summary>
        /// Gets or sets usedpos88 place holder
        /// Position = 88
        /// </summary>
        public float UsedPos88
        {
            get
            {
                return this.usedpos88;
            }

            set
            {
                this.usedpos88 = value;
            }
        }

        /// <summary>
        /// Gets or sets X1
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// Gets or sets Y1 ushort
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// Gets or sets X2
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// Gets or sets X2
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Experiment Type
        /// </summary>
        public Core.ExperimentType ExperimentType { get; set; }

        /// <summary>
        /// Gets or sets the data byte size of data in the image file
        /// </summary>
        public int DataByteSize { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// This method sets the defaults for this class
        /// </summary>
        private void SetDefaultsWrite()
        {
            this.unusedpos4 = "        16";
            this.unusedpos32 = 16384;
            this.unusedpos38 = 114;
            this.unusedpos40 = 4;
            this.unusedpos48 = 1;
            this.usedpos72 = 16;
            this.dimensioninz = 1;
            this.usedpos88 = 1;
        }

        /// <summary>
        /// This method sets the defaults for this class
        /// </summary>
        private void SetDefaultsRead()
        {
            this.unusedpos4 = string.Empty;
            this.unusedpos32 = 0;
            this.unusedpos38 = 0;
            this.unusedpos40 = 0;
            this.unusedpos48 = 0;
            this.usedpos72 = 0;
            this.dimensioninz = 0;
            this.usedpos88 = 0;
        }

        #endregion Methods
    }
}
