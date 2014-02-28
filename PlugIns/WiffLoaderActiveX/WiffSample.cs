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
using System.IO;
using System.Windows;

using NETExploreDataObjects;

using Novartis.Msi.Core;

namespace Novartis.Msi.PlugIns.WiffLoader
{
    /// <summary>
    /// This class encapsulates a wiff file sample.
    /// </summary>
    public class WiffSample
    {
		#region Fields (7) 

        private double x1, x2, y1, y2, width, height;
        /// <summary>
        /// The <b>one-based</b> index of this sample in the wiff file.
        /// </summary>
        private int index;
        /// <summary>
        /// The name of this sample.
        /// </summary>
        private string name;
        /// <summary>
        /// The list of periods this sample consists of.
        /// </summary>
        private WiffPeriodList periods = new WiffPeriodList();
        private uint[,] positionData;
        private long positionDataLength;
        /// <summary>
        /// >The <see cref="WiffFileContent"/> instance this sample belongs to.
        /// </summary>
        private WiffFileContent wiffFileContent;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The source from where to
        /// read the samples properties and data.</param>
        /// <param name="wiffFileContent">The <see cref="WiffFileContent"/> instance this sample belongs to.</param>
        /// <param name="sampleIndex">A <see cref="int"/> value specifying the <b>one-based</b>index of this sample in the <paramref name="wiffFile"/>.</param>
        /// <remarks>Please take note, that the the counting of samples in the wiff file start with 1( in contrary to most ather indexes).</remarks>
        public WiffSample(FMANWiffFileClass wiffFile, WiffFileContent wiffFileContent, int sampleIndex)
        {
            if (wiffFile == null)
                throw new ArgumentNullException("wiffFile");

            if (wiffFileContent == null)
                throw new ArgumentNullException("wiffFileContent");

            if (sampleIndex < 0)
                throw new ArgumentOutOfRangeException("sampleIndex");

            index = sampleIndex;
            this.wiffFileContent = wiffFileContent;

            Initialize(wiffFile);
        }

		#endregion Constructors 

		#region Properties (9) 

        /// <summary>
        /// The height of this sample.
        /// </summary>
        public double Height
        {
            get { return height; }
        }

        /// <summary>
        /// The <b>one-based</b> index of this sample in the wiff file.
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// The name of this sample.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The first point (X1,Y1) of this sample as a <see cref="Point"/> structure.
        /// </summary>
        public Point P1
        {
            get
            {
                Point p1 = new Point(x1, y1);
                return p1;
            }

        }

        /// <summary>
        /// The last point (X1,Y1) of this sample as a <see cref="Point"/> structure.
        /// </summary>
        public Point P2
        {
            get
            {
                Point p2 = new Point(x2, y2);
                return p2;
            }

        }

        /// <summary>
        /// The position data of this sample. 
        /// The data layout is: uint[PositionDataLength, 3];
        /// </summary>
        public uint[,] PositionData
        {
            get { return positionData; }
        }

        /// <summary>
        /// The count of data triplets in <see cref="PositionData"/>.
        /// </summary>
        public long PositionDataLength
        {
            get { return positionDataLength; }
        }

        /// <summary>
        /// The width of this sample.
        /// </summary>
        public double Width
        {
            get { return width; }
        }

        /// <summary>
        /// The <see cref="WiffFileContent"/> instance this sample belongs to.
        /// </summary>
        public WiffFileContent WiffFileContent
        {
            get { return wiffFileContent; }
        }

        /// <summary>
        /// The periods contained in this sample
        /// </summary>
        public WiffPeriodList Periods
        {
            get { return periods; }
        }

		#endregion Properties 

		#region Methods (1) 

		// Private Methods (1) 

        /// <summary>
        /// Fills this instance data members with the information read from <paramref name="wiffFile"/>.
        /// </summary>
        /// <param name="wiffFile">A <see cref="FMANWiffFileClass"/> instance. The data source.</param>
        private void Initialize(FMANWiffFileClass wiffFile)
        {
            // retrieve this sample's name
            name = wiffFile.GetSampleName(index);

            // get the position information for the selected sample
            // the position data exists in a seperate file ( one per sample )
            // and is read from that file 'manually'
            AppContext.ProgressStart("reading path file");
            try 
	        {	        
                string pathFile = wiffFileContent.FileName + " (" + index.ToString() + ").path";
                FileStream positionStream = new FileStream(@pathFile, FileMode.Open, FileAccess.Read);

                // calculate the count of position entries
                positionDataLength = positionStream.Length / 12; // 12 Byte per position entry

                // the array to hold the position information
                positionData = new uint[positionDataLength, 3];
                BinaryReader positionReader = new BinaryReader(positionStream);
                for (long i = 0; i < positionDataLength; i++)
                {
                    positionData[i, 0] = positionReader.ReadUInt32();
                    positionData[i, 1] = positionReader.ReadUInt32();
                    positionData[i, 2] = positionReader.ReadUInt32();
                    if((i % (positionDataLength / 100.0)) == 0) // hundred progress ticks
                        AppContext.ProgressSetValue(100.0 * i / positionDataLength);
                }

                positionReader.Close();
                positionStream.Close();
            }
            finally
            {
                AppContext.ProgressClear();
            }

#if(false)
            string csvPosFile = wiffFileContent.FileName + " (" + index.ToString() + ").path" + ".csv";
            using (FileStream csvPosStream = new FileStream(@csvPosFile, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(csvPosStream))
                {
                    for (long i = 0; i < positionDataLength; i++)
                    {
                        sw.Write(positionData[i, 0]);
                        sw.Write(',');
                        sw.Write(positionData[i, 1]);
                        sw.Write(',');
                        sw.Write(positionData[i, 2]);
                        sw.WriteLine();
                    }
                }
            }
#endif


            // get user data of the selected sample
            string userData = wiffFile.GetUserData(index);

            if (!string.IsNullOrEmpty(userData))
            {
                //width in mm
                x2 = float.Parse(userData.Split(',')[2]);
                x1 = float.Parse(userData.Split(',')[0]);
                width = x2 - x1;

                //height in mm
                y2 = float.Parse(userData.Split(',')[3]);
                y1 = float.Parse(userData.Split(',')[1]);
                height = y2 - y1;
            }


            // get the number of periods
            int nrPeriods = wiffFile.GetActualNumberOfPeriods(index);

            // loop through the periods; the index of the periods is zero based!!
            for (int actPeriod = 0; actPeriod < nrPeriods; actPeriod++)
            {
                WiffPeriod period = new WiffPeriod(wiffFile, this, actPeriod);
                periods.Add(period);
            }
        }

		#endregion Methods 
    }
}
