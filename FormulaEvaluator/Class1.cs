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
    ///    [... and of course you should describe the contents of the 
    ///    file in broad terms here ...]
    /// </summary>
    public static class Evaluator
    {

        public delegate int Lookup(String variable_name);

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
                    tokenStack.Push(variableEvaluator(token).ToString());
                }
                // "+" or "-"
                else if (token == "+" || token == "-")
                {
                    //if + or - is at the top
                    bool hasTop = operatorStack.TryPeek(out top);
                    if (hasTop && (top == "+" || top == "-"))
                        calculate(operatorStack, valueStack); //calculate and push the result
                    else //otherwise, push t
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
                    if (hasTop && (top != "("))
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

        private static void calculate(Stack<string> operatorStack, Stack<int> valueStack)
        {
            //get two values and the operator
            int i2 = valueStack.Pop();
            int i1 = valueStack.Pop();
            string oper = operatorStack.Pop();
            int result;

            //calculate using i1 and i2 by the operator
            switch(oper) {
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

            Console.WriteLine("true: " + variable_name);
            return true;
        }

        private static string[] split(string expression)
        {
            string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            return substrings;
        }
    }
}
