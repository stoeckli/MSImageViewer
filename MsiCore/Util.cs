#region Copyright © 2011 Novartis AG
/////////////////////////////////////////////////////////////////////////////////
// <copyright file="Util.cs" company="Novartis Pharma AG.">
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Novartis.Msi.Core
{
    /// <summary>
    /// Contains general helper methods.
    /// </summary>
    public static class Util
    {
        #region Constants

        /// <summary>
        /// Default epsilon. A small <see langword="double"/>-value near zero.
        /// </summary>
        public const double EpsDefault = 1.0E-8;

        /// <summary>
        /// No error
        /// </summary>
        private const int NoError = 0;

        /// <summary>
        /// More data available
        /// </summary>
        private const int ErrorMoreData = 234;

        /// <summary>
        /// Not connected
        /// </summary>
        private const int ErrorNotConnected = 2250;

        /// <summary>
        /// Level 1
        /// </summary>
        private const int UniversalNameInfoLevel = 1;

        #endregion Constants

        #region Math-Helpers

        /// <summary>
        /// Compares two double values for equality with default tolerance.
        /// </summary>
        /// <param name="a">First value to be compared.</param>
        /// <param name="b">Second value to be compared.</param>
        /// <remarks>
        /// Compares double values for equality, taking into consideration that
        /// floating point values should never be directly compared using ==.
        /// 2 doubles could be conceptually equal, but vary by a .0000001 which
        /// would fail in a direct comparison. To circumvent that, a tolerance
        /// value is used to see if the difference between the 2 doubles is less
        /// than the desired amount of accuracy.</remarks>
        /// <returns> true or false </returns>
        public static bool NearEqual(double a, double b)
        {
            return NearEqual(a, b, EpsDefault);
        }

        /// <summary>
        /// Compares two double values for equality.
        /// </summary>
        /// <param name="a">First value to be compared.</param>
        /// <param name="b">Second value to be compared.</param>
        /// <param name="tolerance">The tolerance for equality.</param>
        /// <remarks>
        /// Compares double values for equality, taking into consideration that
        /// floating point values should never be directly compared using ==.
        /// 2 doubles could be conceptually equal, but vary by a .0000001 which
        /// would fail in a direct comparison. To circumvent that, a tolerance
        /// value is used to see if the difference between the 2 doubles is less
        /// than the desired amount of accuracy.</remarks>
        /// <returns> true or false </returns>
        public static bool NearEqual(double a, double b, double tolerance)
        {
            if (Math.Abs(b - a) <= tolerance)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compares a double value for equality with zero by using default tolerance.
        /// </summary>
        /// <param name="a">The value to be compared.</param>
        /// <returns> true or false </returns>
        public static bool NearZero(double a)
        {
            return NearZero(a, EpsDefault);
        }

        /// <summary>
        /// Compares a double value for equality with zero by using the given tolerance.
        /// </summary>
        /// <param name="a">The value to be compared.</param>
        /// <param name="tolerance">The tolerance for equality.</param>
        /// <returns> true or false </returns>
        public static bool NearZero(double a, double tolerance)
        {
            if (Math.Abs(a) <= tolerance)
            {
                return true;
            }

            return false;
        }

        #endregion Math-Helpers

        #region Command-Line-Parsing

        /// <summary>
        /// Transform the command line string array into a Dictionary.
        /// </summary>
        /// <param name="args">The string array, usually string[] args from your main function</param>
        /// <returns>A Dictionary where the arguments are the keys and parameters to the arguments are in a List. Keep in mind that the List may be null!</returns>
        public static Dictionary<string, List<string>> ParseCommandLine(string[] args)
        {
            return ParseCommandLine(args, true, false);
        }

        /// <summary>
        /// Transform the command line string array into a Dictionary.
        /// </summary>
        /// <param name="args">The string array, usually string[] args from your main function</param>
        /// <param name="ignoreArgumentCase">Ignore the case of arguments? (if set to false, then "/beta" and "/Beta" are two different parameters</param>
        /// <returns>A Dictionary where the arguments are the keys and parameters to the arguments are in a List. Keep in mind that the List may be null!</returns>
        public static Dictionary<string, List<string>> ParseCommandLine(string[] args, bool ignoreArgumentCase)
        {
            return ParseCommandLine(args, ignoreArgumentCase, false);
        }

        /// <summary>
        /// Transform the command line string array into a Dictionary.
        /// </summary>
        /// <param name="args">The string array, usually string[] args from your main function</param>
        /// <param name="ignoreArgumentCase">Ignore the case of arguments? (if set to false, then "/beta" and "/Beta" are two different arguments)</param>
        /// <param name="allowMultipleParameters">Allow multiple parameters to one argument.</param>
        /// <returns>A Dictionary where the arguments are the keys and parameters to the arguments are in a List. Keep in mind that the List may be null!</returns>
        /// <remarks>
        /// If allowMultipleParameters is set to true, then "/delta omega kappa" will cause omega and kappa to be two parameters to the argument delta.
        /// If allowMultipleParameters is set to false, then omega will be a parameter to delta, but kappa will be assigned to string.Empty.
        /// </remarks>
        public static Dictionary<string, List<string>> ParseCommandLine(string[] args, bool ignoreArgumentCase, bool allowMultipleParameters)
        {
            var result = new Dictionary<string, List<string>>();
            string currentArgument = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                // Is this an argument?
                if ((args[i].StartsWith("-", StringComparison.OrdinalIgnoreCase) || args[i].StartsWith("/", StringComparison.OrdinalIgnoreCase)) && args[i].Length > 1)
                {
                    currentArgument = args[i].Remove(0, 1);
                    if (ignoreArgumentCase)
                    {
                        currentArgument = currentArgument.ToLowerInvariant();
                    }

                    if (!result.ContainsKey(currentArgument))
                    {
                        result.Add(currentArgument, null);
                    }
                }
                else 
                {
                    // No, it's a parameter
                    List<string> paramValues = null;
                    if (result.ContainsKey(currentArgument))
                    {
                        paramValues = result[currentArgument];
                    }

                    if (paramValues == null)
                    {
                        paramValues = new List<string>();
                    }

                    paramValues.Add(args[i]);
                    result[currentArgument] = paramValues;
                    if (!allowMultipleParameters)
                    {
                        currentArgument = string.Empty;
                    }
                }
            }

            return result;
        }

        #endregion Command-Line-Parsing

        #region String-Helpers

        /// <summary>
        /// Returns true if fileName is a valid local file-name of the form:
        /// X:\, where X is a drive letter from A-Z
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns>true or false</returns>
        public static bool IsValidFilePath(string fileName)
        {
            if (null == fileName || 0 == fileName.Length)
            {
                return false;
            }

            char drive = char.ToUpper(fileName[0]);
            if ('A' > drive || drive > 'Z')
            {
                return false;
            }
            
            if (Path.VolumeSeparatorChar != fileName[1])
            {
                return false;
            }
            
            if (Path.DirectorySeparatorChar != fileName[2])
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Returns the UNC path for a mapped drive or local share.
        /// </summary>
        /// <param name="fileName">The path to map</param>
        /// <returns>The UNC path (if available)</returns>
        public static string PathToUnc(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            fileName = Path.GetFullPath(fileName);
            if (!IsValidFilePath(fileName))
            {
                return fileName;
            }

            var rni = new UniversalNameInfo();
            int bufferSize = Marshal.SizeOf(rni);

            int intReturn = WNetGetUniversalName(fileName, UniversalNameInfoLevel, ref rni, ref bufferSize);

            if (ErrorMoreData == intReturn)
            {
                IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
                try
                {
                    intReturn = WNetGetUniversalName(fileName, UniversalNameInfoLevel, buffer, ref bufferSize);

                    if (NoError == intReturn)
                    {
                        rni = (UniversalNameInfo)Marshal.PtrToStructure(buffer, typeof(UniversalNameInfo));
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }

            switch (intReturn)
            {
                case NoError:
                    return rni.UniversalName;

                case ErrorNotConnected:
                    return string.Empty;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a new string equivalent to the given <paramref name="path"/> but having all
        /// invalid characters(according to <see cref="Path.GetInvalidFileNameChars()"/>) substituted
        /// with the <paramref name="substitute"/> char.
        /// </summary>
        /// <param name="path">The <see cref="string"/> to process.</param>
        /// <param name="substitute">The substituition <see cref="char"/>.</param>
        /// <returns>A <see cref="string"/> equivalent to <paramref name="path"/> but stripped of invalid chars.</returns>
        public static string SubstitueInvalidPathChars(string path, char substitute)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (path == string.Empty)
            {
                return path;
            }

            string result = path;
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char ic in invalidChars)
            {
                result = result.Replace(ic, substitute);
            }

            return result;
        }

        #endregion String-Helpers

        #region Exception Reporting & Logging

        /// <summary>
        /// Produce a entry of the given in the application's logfile.
        /// </summary>
        /// <param name="exception">The exception to be reported and eternalized in the logfile.</param>
        public static void ReportException(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

#if(DEBUG)
                if (!AppContext.BatchMode)
                {
                    System.Windows.MessageBox.Show(
                        exception.GetType() + ": " + exception.Message, "Exception occurred!");
                }
#endif
                var dashes = new string('-', 80);
                Trace.WriteLine(dashes);
                Trace.WriteLine(string.Format("{0}: \"{1}\"", exception.GetType().Name, exception.Message));
                Trace.WriteLine(new string('*', 80));
                if (exception.InnerException != null)
                {
                    Trace.WriteLine("InnerException:");

                    // increase indentation
                    Trace.Indent();
                    try
                    {
                        ReportException(exception.InnerException);
                    }
                    finally
                    {
                        Trace.Unindent();
                    }
                }

                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    foreach (string line in exception.StackTrace.Split(new[] { " at " }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrEmpty(line.Trim()))
                        {
                            continue;
                        }

                        string[] parts = line.Trim().Split(new[] { " in " }, StringSplitOptions.RemoveEmptyEntries);
                        string classInfo = parts[0];
                        if (parts.Length == 2)
                        {
                            parts = parts[1].Trim().Split(new[] { "line" }, StringSplitOptions.RemoveEmptyEntries);
                            string srcFile = parts[0];
                            int lineNr = int.Parse(parts[1]);
                            Trace.WriteLine(string.Format("  {0}({1},1):   {2}", srcFile.TrimEnd(':'), lineNr, classInfo));
                        }
                        else
                        {
                            Trace.WriteLine("  " + classInfo);
                        }
                    }
                }

                Trace.WriteLine(dashes);
            }
        #endregion Exception Reporting & Logging

        #region Extern Methods

        /// <summary>
        /// Get a UNC name
        /// </summary>
        /// <param name="localPath">Local Path</param>
        /// <param name="infoLevel">Info Level</param>
        /// <param name="buffer">Internal Buffer</param>
        /// <param name="bufferSize">Buffer Size</param>
        /// <returns> integer return value</returns>
        [DllImport("mpr", CharSet = CharSet.Auto)]
        private static extern int WNetGetUniversalName(string localPath, int infoLevel, ref UniversalNameInfo buffer, ref int bufferSize);

        /// <summary>
        /// Get a UNC name
        /// </summary>
        /// <param name="localPath">Local Path</param>
        /// <param name="infoLevel">Info Level</param>
        /// <param name="buffer">Internal Buffer</param>
        /// <param name="bufferSize">Buffer Size</param>
        /// <returns> integer return value</returns> 
        [DllImport("mpr", CharSet = CharSet.Auto)]
        private static extern int WNetGetUniversalName(string localPath, int infoLevel, IntPtr buffer, ref int bufferSize);
        
        #endregion Extern Methods

        #region Structs

        /// <summary>
        /// Unc name
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct UniversalNameInfo
        {
            /// <summary>
            /// Universal Name
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public readonly string UniversalName;
        }

        #endregion Structs
    }
}
