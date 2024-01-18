using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Cheuk Yin Lau
    /// Partner:   None
    /// Date:      2023-01-17
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Cheuk Yin Lau - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Cheuk Yin Lau, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    [... and of course you should describe the contents of the 
    ///    file in broad terms here ...]
    /// </summary>
    public static class Evaluator
    {

        public delegate int Lookup(String variable_name);

        public static int Evaluate(String expression,
                                   Lookup variableEvaluator)
        {
            // TODO...
            return 0;
        }

        private static string[] split(string expression)
        {
            string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return substrings;
        }
    }
}
