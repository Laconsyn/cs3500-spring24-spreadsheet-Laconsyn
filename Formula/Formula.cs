// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    /// 
  public class Formula
  {

        private List<string> tokens;
        private Func<string, string> normalize;
        private Func<string, bool> isValid;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
        this(formula, s => s, s => true)
    {
    }

    /// <summary>
    /// Creates a Formula from a string that consists of an infix expression written as
    /// described in the class comment.  If the expression is syntactically incorrect,
    /// throws a FormulaFormatException with an explanatory Message.
    /// 
    /// The associated normalizer and validator are the second and third parameters,
    /// respectively.  
    /// 
    /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
    /// throws a FormulaFormatException with an explanatory message. 
    /// 
    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
    /// throws a FormulaFormatException with an explanatory message.
    /// 
    /// Suppose that N is a method that converts all the letters in a string to upper case, and
    /// that V is a method that returns true only if a string consists of one letter followed
    /// by one digit.  Then:
    /// 
    /// new Formula("x2+y3", N, V) should succeed
    /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
    /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
    /// </summary>
    public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
    {
            tokens = new List<string>();

            IEnumerable<string> rawTokens = GetTokens(formula);
            foreach (string s in rawTokens)
            {
                string token = normalize(s);
                if(isVariable(token)&&!isValid(token))
                    throw new FormulaFormatException("Token is invalid: " + token);

                if (token.Length != 0) //ignore empty tokens
                    tokens.Add(token);
            }

            if (!checkSyntax())
                throw new FormulaFormatException("Syntax incorrect! ");

            this.normalize = normalize;
            this.isValid = isValid;
    }

        private bool checkSyntax()
        {
            //Specific Token Rule
            string[] validstrings = {"(",")","+","-","*","/"};
            foreach(string token in tokens)
            {
                if (validToken(token, validstrings))
                    continue; //is number or variables or given string
                else
                    return false;
            }
            //One Token Rule
            if (tokens.Count < 1)
                return false;
            //Right Parentheses Rule
            int left = 0,right = 0;
            foreach (string token in tokens)
            {
                if(token.Equals("("))
                    left++;
                else if(token.Equals(")"))
                    right++;

                if(right > left)
                    return false;
            }
            //Balanced Parentheses Rule
            if (left != right)
                return false;
            //Starting Token Rule
            string first = tokens[0];
            if (!validToken(first, new string[] { "(" }))
                return false;
            //Ending Token Rule
            string last = tokens[tokens.Count-1];
            if (!validToken(last, new string[] { ")" }))
                return false;
            //Parenthesis/Operator Following Rule
            bool follows = false;
            foreach (string token in tokens)
            {
                //immediately follows an opening parenthesis
                if (follows && !validToken(token, new string[] {"("})) 
                    return false;

                follows = false;

                //is being followed
                validstrings = new string[]{"(","+","-","*","/"};
                foreach (string s in validstrings) //is special char
                    if (token.Equals(s))
                        follows = true;
            }
            //Extra Following Rule 
            follows = false;
            foreach (string token in tokens)
            {
                validstrings = new string[] { ")", "+", "-", "*", "/" };
                //immediately follows an opening parenthesis
                if (follows)
                {
                    bool valid = false; //is special char
                    foreach (string s in validstrings)
                    {
                        if (token.Equals(s))
                        {
                            valid = true;
                            break;
                        }
                    }
                    if (!valid)
                        return false;
                }
                follows = false;

                //is being followed
                if (validToken(token, new string[] { ")" })) { follows = true;  }

            }
            return true;
        }

        private bool validToken(string token, string[] validStrings)
        {
            if (isVariable(token))//is variable
                return true;

            double value; //is number 
            if (double.TryParse(token, out value))
                return true;

            //is valid char
            foreach (string s in validStrings)//is special char
            {
                if (token.Equals(s))
                    return true;
            }

            return false; //anything else
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
    {
            try
            {
                //stack the tokens
                Stack<string> tokenStack = new Stack<string>();
                for (int i = tokens.Count - 1; i >= 0; i--)
                    tokenStack.Push(tokens[i]);

                //initialize operator stack and value stack
                Stack<string> operatorStack = new Stack<string>();
                Stack<double> valueStack = new Stack<double>();

                //iterate through the tokens
                while (tokenStack.Count > 0)
                {
                    //get token
                    string token = tokenStack.Pop();

                    //check conditions
                    //number
                    double value;
                    string top = "";
                    if (double.TryParse(token, out value))
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
                    else if (isVariable(token))
                    {
                        try
                        {
                            ////get looked up value
                            double lookedUpValue = lookup(token);
                            tokenStack.Push(lookedUpValue.ToString());
                        }
                         catch (ArgumentException e) //variable undefined
                        {
                            return new FormulaError("Undefined token: " + token);
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
                        //check the operators at the top in order
                        bool hasTop = operatorStack.TryPeek(out top);
                        if (hasTop && (top == "+" || top == "-"))
                            calculate(operatorStack, valueStack); //calculate and push the result
                        operatorStack.Pop(); //pop "("
                        hasTop = operatorStack.TryPeek(out top);
                        if (hasTop && (top == "*" || top == "/"))
                            calculate(operatorStack, valueStack); //calculate and push the result
                    }
                }
                //the last token has been processed
                if (operatorStack.Count == 0) //Operator stack is empty
                {
                    //Pop the last value as the result
                    return valueStack.Pop();
                }
                else //Operator stack is not empty
                {
                    calculate(operatorStack, valueStack); //Apply the operator
                    return valueStack.Pop(); //report the result as the value
                }
            } //division zero occurs
            catch (ArgumentException e) { return new FormulaError(e.Message); }
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
        private static void calculate(Stack<string> operatorStack, Stack<double> valueStack)
        {

            //get two values and the operator
            double i2 = valueStack.Pop();
            double i1 = valueStack.Pop();
            string oper = operatorStack.Pop();
            double result = 0;

            if (oper == "/" && i2 == 0)
                throw new ArgumentException("A division by zero occurs.");
            //calculate using i1 and i2 by the operator
            switch (oper)
            {
                case "+":
                    result = i1 + i2; break;
                case "-":
                    result = i1 - i2; break;
                case "*":
                    result = i1 * i2; break;
                case "/":
                    result = i1 / i2; break;
            }

            //push the calculation to the stack
            valueStack.Push(result);
        }

        /// <summary>
        /// check if the given string follows the rule to name a variable
        /// </summary>
        /// <param name="variable_name">given string to check if it is a variable name</param>
        /// <returns>true if the given string is a variable name</returns>
        private static bool isVariable(string variable_name)
        {
            if (Char.IsDigit(variable_name[0])) //not var if first char is digit
                return false;

            foreach (Char ch in variable_name) //not var if contains anything than digit/letter/'_'
                if (!(Char.IsDigit(ch) || Char.IsLetter(ch) || ch == '_'))
                    return false;

            return true;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
    {
            List<string> list = new List<string>();

            foreach(string token in tokens)
                if(isVariable(token))
                    list.Add(token);

            return list;
    }

    /// <summary>
    /// Returns a string containing no spaces which, if passed to the Formula
    /// constructor, will produce a Formula f such that this.Equals(f).  All of the
    /// variables in the string should be normalized.
    /// 
    /// For example, if N is a method that converts all the letters in a string to upper case:
    /// 
    /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
    /// new Formula("x + Y").ToString() should return "x+Y"
    /// </summary>
    public override string ToString()
    {
            string output = "";
            foreach (string token in tokens)
                output += token;

            return output;
    }

    /// <summary>
    ///  <change> make object nullable </change>
    ///
    /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
    /// whether or not this Formula and obj are equal.
    /// 
    /// Two Formulae are considered equal if they consist of the same tokens in the
    /// same order.  To determine token equality, all tokens are compared as strings 
    /// except for numeric tokens and variable tokens.
    /// Numeric tokens are considered equal if they are equal after being "normalized" 
    /// by C#'s standard conversion from string to double, then back to string. This 
    /// eliminates any inconsistencies due to limited floating point precision.
    /// Variable tokens are considered equal if their normalized forms are equal, as 
    /// defined by the provided normalizer.
    /// 
    /// For example, if N is a method that converts all the letters in a string to upper case:
    ///  
    /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
    /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
    /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
    /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
    /// </summary>
    public override bool Equals(object? obj)
    {
            if(obj == null || !(obj is Formula)) return false;

            return ToString().Equals(((Formula)obj).ToString());
    }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    /// Reports whether f1 == f2, using the notion of equality from the Equals method.
    /// 
    /// </summary>
    public static bool operator == (Formula f1, Formula f2)
    {
      return f1.Equals(f2);
    }

    /// <summary>
    ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
    ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
    ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
    /// </summary>
    public static bool operator !=(Formula f1, Formula f2)
    {
      return !(f1 == f2);
    }

    /// <summary>
    /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
    /// randomly-generated unequal Formulae have the same hash code should be extremely small.
    /// </summary>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
            
    }

    /// <summary>
    /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
    /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
    /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
    /// match one of those patterns.  There are no empty tokens, and no token contains white space.
    /// </summary>
    private static IEnumerable<string> GetTokens(String formula)
    {
      // Patterns for individual tokens
      String lpPattern = @"\(";
      String rpPattern = @"\)";
      String opPattern = @"[\+\-*/]";
      String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
      String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
      String spacePattern = @"\s+";

      // Overall pattern
      String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                      lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

      // Enumerate matching tokens that don't consist solely of white space.
      foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
      {
        if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
        {
          yield return s;
        }
      }

    }
  }

  /// <summary>
  /// Used to report syntactic errors in the argument to the Formula constructor.
  /// </summary>
  public class FormulaFormatException : Exception
  {
    /// <summary>
    /// Constructs a FormulaFormatException containing the explanatory message.
    /// </summary>
    public FormulaFormatException(String message)
        : base(message)
    {
    }
  }

  /// <summary>
  /// Used as a possible return value of the Formula.Evaluate method.
  /// </summary>
  public struct FormulaError
  {
    /// <summary>
    /// Constructs a FormulaError containing the explanatory reason.
    /// </summary>
    /// <param name="reason"></param>
    public FormulaError(String reason)
        : this()
    {
      Reason = reason;
    }

    /// <summary>
    ///  The reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
  }
}


// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
