using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace ExPresSXR.Experimentation.DataGathering
{
    public class CsvUtility : MonoBehaviour
    {
        /// <summary>
        /// The character that is used as default for separating csv columns.
        /// The default is not ',' as it interferes with string representations of vectors and floats.
        /// </summary>
        public const char DEFAULT_COLUMN_SEPARATOR = ';';

        /// <summary>
        /// Same as DEFAULT_COLUMN_SEPARATOR but as string so it can be used with static strings.
        /// </summary>
        public const string DEFAULT_COLUMN_SEPARATOR_STRING = ";";

        /// <summary>
        /// The character that is used to separate values in an array.
        /// The default is not ',' as it interferes with string representations of vectors and floats.
        /// </summary>
        public const char DEFAULT_ARRAY_SEPARATOR = ',';


        /// <summary>
        /// Joins the values into a CSV line using the given separator and filtering empty values if desired.
        /// </summary>
        /// <param name="values">Values to be converted to a CSV line. </param>
        /// <param name="sep">Separator character (Default: DataGatherer.DEFAULT_COLUMN_SEPARATOR). </param>
        /// <param name="filterEmpty">If true will filter empty or null-strings, omitting creating an empty column for such. </param>
        /// <returns></returns>
        public static string JoinAsCsv<T>(T[] values, char sep = DEFAULT_COLUMN_SEPARATOR, bool filterEmpty = false)
        {
            if (filterEmpty)
            {
                return string.Join(sep, values.Where(v => !string.IsNullOrEmpty(v.ToString())));
            }
            return string.Join(sep, values);
        }

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
        public static string EmptyCSVColumns(int numCols, char sepChar = DEFAULT_COLUMN_SEPARATOR) => numCols > 1 ?new string(sepChar, numCols - 1) : "";


        /// <summary>
        /// Returns the name of a video with an optional support for videos-paths from the StreamingAssets-Folder.
        /// Video clips override the streamed path. In case video is null the full path of 'streamedVideo' is returned or and empty string if omitted.
        /// </summary>
        /// <param name="video">Video Clip from which the name should be received. </param>
        /// <param name="streamedVideo">Relative path of a video from the StreamingAssets-Folder. </param>
        /// <returns></returns>
        public static string GetVideoName(VideoClip video, string streamedVideo = "")
        {
            return video != null? video.name : streamedVideo;
        }
    }
}