using System;
using System.Linq;
using ExPresSXR.Interaction.ButtonQuiz;
using UnityEngine;

namespace ExPresSXR.Experimentation.DataGathering
{
    [Serializable]
    public class ExportColumnReplacement
    {
        /// <summary>
        /// The component to be matched against (short name without a namespace)
        /// </summary>
        public string componentName { get; private set; }

        /// <summary>
        /// The member to be matched against (ignoring the return type, the parameters are optional)
        /// </summary>
        public string memberName { get; private set; }

        /// <summary>
        /// CSV column header that will be inserted automatically
        /// </summary>
        public string replacementHeader { get; private set; }

        /// <summary>
        /// An optional message that is printed to the console once match is found.
        /// Will be preceded with 'Special Export Column found: '
        /// </summary>
        public string matchInfoMessage { get; private set; }


        public ExportColumnReplacement(string componentName, string memberName, string replacementHeader, string matchInfoMessage = "")
        {
            this.componentName = componentName;
            this.memberName = memberName;
            this.replacementHeader = replacementHeader;
            this.matchInfoMessage = matchInfoMessage;
        }

        /// <summary>
        /// If the neccessary information was provided. 
        /// The strings `componentName`, `memberName`, `replacementHeader` must be not empty. Does not imply validity.
        /// </summary>
        /// <returns>bool if it was complete</returns>
        public bool IsComplete()
        {
            return !string.IsNullOrEmpty(componentName) && !string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(replacementHeader);
        }

        /// <summary>
        /// Overrides the the `ToString()`-method to print the binging similar to how it is displayed by the DataGatherer.
        /// </summary>
        /// <returns>string representing this column replacement.</returns>
        public override string ToString() => $"`{componentName}/{memberName}` => {replacementHeader}";

        /// <summary>
        /// Returns the default replacements for the DataGatherer. Uses the given separator for separation.
        /// </summary>
        /// <param name="sep">Separator used for columns</param>
        /// <returns>A list of the default ExportColumnReplacements.</returns>
        public static ExportColumnReplacement[] GetStandardReplacements(char sep = CsvUtility.DEFAULT_COLUMN_SEPARATOR)
        {
            return new ExportColumnReplacement[]
            {
                new("ButtonQuiz", "GetLatestRoundDataExportValue", QuizRoundData.GetQuizRoundCsvHeader(sep)),
                new("ButtonQuiz", "GetCurrentQuestionCsvExportValue", ButtonQuizQuestion.GetQuestionCsvHeader(sep)),
                new("ButtonQuiz", "GetFullQuizCsvExportValues", ButtonQuiz.GetFullQuizCsvHeader(sep)),
                new("ButtonQuiz", "GetConfigCsvExportValues", ButtonQuizConfig.GetConfigCsvHeader(sep)),
                new("ButtonQuiz", "GetAllQuestionsCsvExportValues", ButtonQuizQuestion.GetQuestionCsvHeader(sep), 
                        "`GetAllQuestionsCsvExportValues(char? sep)` will export multiple lines of values which might break the formatting of the csv. "
                        + "Also do not export this value with timestamps.")                
            };
        }
    }
}