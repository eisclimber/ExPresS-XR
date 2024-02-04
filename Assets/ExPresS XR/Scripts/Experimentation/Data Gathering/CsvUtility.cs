using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace ExPresSXR.Experimentation.DataGathering
{
    public class CsvUtility : MonoBehaviour
    {
        /// <summary>
        /// The comma character.
        /// </summary>
        public const char COMMA_COLUMN_SEPARATOR = ',';

        /// <summary>
        /// The comma character.
        /// </summary>
        public const char SEMICOLON_COLUMN_SEPARATOR = ';';

        /// <summary>
        /// The character that is used as default for separating csv columns.
        /// The default is not ',' as it interferes with string representations of vectors and floats.
        /// </summary>
        public const char DEFAULT_COLUMN_SEPARATOR = SEMICOLON_COLUMN_SEPARATOR;

        /// <summary>
        /// The character that is used to separate values in an array.
        /// The default is not ',' as it interferes with string representations of vectors and floats.
        /// </summary>
        public const char DEFAULT_ARRAY_SEPARATOR = ',';

        /// <summary>
        /// The character that is used to escape fields that may contain the separator and would break the format.
        /// </summary>
        public const char DEFAULT_ESCAPE_CHAR = '"';


        /// <summary>
        /// Joins the values into a CSV line using the given separator and csv-escaping (all) values if desired.
        /// </summary>
        /// <param name="values">Values to be converted to a CSV line.</param>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <param name="safe">If true escapes all values using the DEFAULT_ESCAPE_CHARACTER (and replace it in the string).</param>
        /// <typeparam name="T">Type of values to be converted.</typeparam>
        /// <returns>A (csv)-string representation of the values-array.</returns>
        public static string JoinAsCsv<T>(IEnumerable<T> values, char sep = DEFAULT_COLUMN_SEPARATOR, bool safe = true)
        {
            if (safe)
            {
                return string.Join(sep, values.Select(v => GetValueSafe(v, sep)));
            }
            return string.Join(sep, values);
        }

        /// <summary>
        /// Joins the values into a CSV line using the given separator. Allows csv-escaping value individually.
        /// If the lists do not match in lengths, iteration will stop at the shorter one.
        /// </summary>
        /// <param name="values">Values to be converted to a CSV line.</param>
        /// <param name="safeIndividual">A list</param>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR).</param>
        /// <typeparam name="T">Type of values to be converted.</typeparam>
        /// <returns>A (csv)-string representation of the values-array.</returns>
        public static string JoinAsCsv<T>(IEnumerable<T> values, IEnumerable<bool> safeIndividual, char sep = DEFAULT_COLUMN_SEPARATOR)
        {
            var escapedValues = values.Zip(safeIndividual, (value, safe) => safe ? GetValueSafe(value, sep) : value.ToString()).ToList();
            return string.Join(sep, escapedValues);
        }



        /// <summary>
        /// Converts any arbitrary value to a safe CSV column entry with the provided separator and escape char.
        /// This is done by 
        /// </summary>
        /// <param name="values">Values to be converted to a safe CSV column entry. </param>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR). </param>
        /// <typeparam name="T">Type to be converted.</typeparam>
        /// <returns>A (if required CSV-escaped) string.</returns>
        public static string GetValueSafe<T>(T value, char sep = DEFAULT_COLUMN_SEPARATOR)
        {
            string valueString = value.ToString();
            if (!IsEscaped(valueString) && NeedsEscaping(valueString, sep))
            {
                valueString = $"\"{ valueString.Replace("\"", "\"\"") }\"";
            }
            return valueString;
        }


        /// <summary>
        /// Returns true if the given string is properly CSV-escaped (starts and ends with a '"').
        /// </summary>
        /// <param name="value">String to be checked</param>
        /// <returns>If the string is properly escaped.</returns>
        public static bool IsEscaped(string value) => value.StartsWith("\"") && value.EndsWith("\"");


        /// <summary>
        /// Returns true if the given string is properly CSV-escaped (starts and ends with a '"').
        /// </summary>
        /// <param name="value">String to be checked</param>
        /// <returns>If the string is properly escaped.</returns>
        public static bool NeedsEscaping(string value, char sep = DEFAULT_COLUMN_SEPARATOR) => value.Contains(sep) || value.Contains("\"");


        /// <summary>
        /// Joins the values into a string representing an array using the given separator.
        /// </summary>
        /// <param name="values">Values to be converted to a CSV line. </param>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR). </param>
        /// <returns></returns>
        public static string ArrayToString<T>(T[] values, char sep = DEFAULT_ARRAY_SEPARATOR) => $"[{string.Join(sep, values)}]";


        /// <summary>
        /// Returns an CSV having 'num' empty columns using 'sepChar' as separator, i.e. 'num'-1 contains 'sepChar's.
        /// </summary>
        /// <param name="num"> The number of columns</param>
        /// <param name="sepChar"> Separator used. Default: DEFAULT_COLUMN_SEPARATOR = ';'.</param>
        /// <returns></returns>
        public static string EmptyCSVColumns(int numCols, char sepChar = DEFAULT_COLUMN_SEPARATOR) => numCols > 1 ? new string(sepChar, numCols - 1) : "";


        /// <summary>
        /// Returns the name of a video with an optional support for videos-paths from the StreamingAssets-Folder.
        /// Video clips override the streamed path. In case video is null the full path of 'streamedVideo' is returned or and empty string if omitted.
        /// </summary>
        /// <param name="video">Video Clip from which the name should be received. </param>
        /// <param name="streamedVideo">Relative path of a video from the StreamingAssets-Folder. </param>
        /// <returns></returns>
        public static string GetVideoName(VideoClip video, string streamedVideo = "")
        {
            return video != null ? video.name : streamedVideo;
        }
    }
}