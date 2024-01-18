using System.Data;
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
    ///    The file contains the Evaluate method that helps spreadsheet calculation. 
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Gets the value of variable using in expression by its name
        /// </summary>
        /// <param name="variable_name">represents the name of variable using in the expression</param>
        /// <returns>the value of variable of given name</returns>
        public delegate int Lookup(String variable_name);

        /// <summary>
        /// calculate a result for spreadsheet using given expression. 
        /// only accept non-negative integers
        /// variable names must begin with one or more letters and end with one or more digits
        /// can be upper or lower case
        /// white spaces are automatically removed
        /// cannot use special characters
        /// </summary>
        /// <param name="expression"> formula to calculate</param>
        /// <param name="variableEvaluator">assigning values to the variables by their names</param>
        /// <returns>the result of expression</returns>
        /// <exception cref="ArgumentException">
        /// when any stack is empty
        /// when lookup reveals it has no value
        /// when '(' isn't found where expected
        /// when invalid character in expression
        /// There isn't exactly one operator on the operator stack or two numbers on the value stack
        /// after the last token has been processed
        /// </exception>
        public static int Evaluate(String expression,
                                   Lookup variableEvaluator)
        {
            //split the expression to tokens
            string[] tokens = split(expression);

            //stack the tokens
            Stack<string> tokenStack = new Stack<string>();
            for(int i = tokens.Length-1; i >= 0;i--)
                tokenStack.Push(tokens[i]);

            //initialize operator stack and value stack
            Stack<string> operatorStack = new Stack<string>();
            Stack<int> valueStack = new Stack<int>();
            
            //iterate through the tokens
            while(tokenStack.Count > 0)
            {
                //get token
                string token = tokenStack.Pop();
                if (token.Length == 0) //ignore empty tokens
                    continue;

                //check conditions
                //integer
                int value;
                string top = "";
                if (int.TryParse(token, out value))
                {

                    if (valueStack == null)
                        throw new ArgumentException("The value stack is empty.");

                    //if * or / is at the top
                    bool hasTop = operatorStack.TryPeek(out top);
                    if (hasTop && (top == "*" || top == "/"))
                    {
                        valueStack.Push(value);
                        calculate(operatorStack, valueStack); //calculate and push the result
                    }
                    //otherwise, push the value
                    else
                        valueStack.Push(value);
                }
                //variable
                else if (isVariale(token))
                {
                    if (valueStack == null)
                        throw new ArgumentException("the value stack is empty");

                    try ////get looked up value
                    {
                        int lookedUpValue = variableEvaluator(token);
                        tokenStack.Push(lookedUpValue.ToString());
                    } catch (Exception e) //the delegate throws exception
                    {
                        throw new ArgumentException("looking up t reveals it has no value");
                    }
                    
                }
                // "+" or "-"
                else if (token == "+" || token == "-")
                {
                    //if + or - is at the top
                    bool hasTop = operatorStack.TryPeek(out top);
                    if (hasTop && (top == "+" || top == "-"))
                        calculate(operatorStack, valueStack); //calculate and push the result
                    //push t
                        operatorStack.Push(token); 
                }
                // "*" or "/"
                else if (token == "*" || token == "/")
                {
                    operatorStack.Push(token);
                }
                // "("
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                // ")"
                else if (token == ")")
                {
                    //get operator at the top
                    bool hasTop = operatorStack.TryPeek(out top);
                    //If + or - is at the top
                    if (hasTop && (top == "+" || top == "-"))
                        calculate(operatorStack, valueStack); //calculate and push the result
                    //check if "(" is found
                    hasTop = operatorStack.TryPeek(out top);
                    if (!hasTop || (top != "("))
                        throw new ArgumentException("A '(' isn't found where expected");
                    else //pop "("
                        operatorStack.Pop();
                    //If * or / is at the top
                    hasTop = operatorStack.TryPeek(out top);
                    if (hasTop && (top == "*" || top == "/"))
                        calculate(operatorStack, valueStack); //calculate and push the result
                }
                else if (token == " ")
                {
                    continue; //ignore space
                }
                else
                {
                    throw new ArgumentException("Invalid character in expression: " + token);
                }
                Console.WriteLine(token + ", "+ operatorStack.Count + ", " + valueStack.Count);
            }
            //the last token has been processed
            if (operatorStack.Count == 0) //Operator stack is empty
            {
                if (valueStack.Count != 1)
                    throw new ArgumentException("There isn't exactly one value on the value stack");
                //Pop the last value as the result
                return valueStack.Pop();
            }
            else //Operator stack is not empty
            {
                if (operatorStack.Count != 1)
                    throw new ArgumentException("There isn't exactly one operator on the operator stack");
                if (valueStack.Count != 2)
                    throw new ArgumentException("There isn't exactly two numbers on the value stack");

                calculate(operatorStack, valueStack); //Apply the operator
                return valueStack.Pop(); //report the result as the value
            }
        }
        /// <summary>
        /// pops the first and second value in given value stack
        /// pops the first operator in given operator stack
        /// applies the operator to calculate the result
        /// push the result to the value stack
        /// </summary>
        /// <param name="operatorStack">stack that provides operator to calculate</param>
        /// <param name="valueStack">stack that provides two numbers to calculate</param>
        /// <exception cref="ArgumentException">
        /// when the value stack has less than 2 values
        /// or when division zero occurs
        /// or when operator is not +, -, *, /
        /// </exception>
        private static void calculate(Stack<string> operatorStack, Stack<int> valueStack)
        {
            if (valueStack.Count < 2)
                throw new ArgumentException("The value stack contains fewer than 2 values");

            //get two values and the operator
            int i2 = valueStack.Pop();
            int i1 = valueStack.Pop();
            string oper = operatorStack.Pop();
            int result;

            if (oper == "/" && i2 == 0)
                throw new ArgumentException("A division by zero occurs.");
            //calculate using i1 and i2 by the operator
            switch (oper) {
                case "+":
                    result = i1 + i2; break;
                case "-":
                    result = i1 - i2; break;
                case "*":
                    result = i1 * i2; break;
                case "/":
                    result = i1 / i2; break;
                default: //throw argument if operator is anything else
                    throw new ArgumentException("Operator not recognized: " + oper);
            }

            //push the calculation to the stack
            valueStack.Push(result);
        }

        /// <summary>
        /// check if the given string follows the rule to name a variable
        /// </summary>
        /// <param name="variable_name">given string to check if it is a variable name</param>
        /// <returns>true if the given string is a variable name</returns>
        private static bool isVariale(string variable_name)
        {
            int letterCount = 0;
            int digitCount = 0;
            foreach(char character in variable_name)
            {
                //check ascii code
                int ascii = (int) character;
                //letter
                if(ascii >= 65 && ascii <= 122) {
                    //false if check letter after digit
                    if(digitCount != 0){return false;}

                    //update letter count
                    letterCount++;
                }
                //digits
                else if(ascii >= 48 && ascii <= 57)
                {
                    //update digit count
                    digitCount++;
                }
                //neither
                else { return false; }
            }

            //false if no letter or no digit
            if(letterCount == 0 || digitCount == 0) { return false; }

            return true;
        }

        /// <summary>
        /// splts the expression into tokens
        /// </summary>
        /// <param name="expression">string that contains the formula to be splited</param>
        /// <returns>an array of tokens in order</returns>
        private static string[] split(string expression)
        {
            string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return substrings;
        }
    }
}
