
using FormulaEvaluator;

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
    ///    The file test the Evaluate method in FormulaEvaluator class. 
    /// </summary>

//Addition and subtractions
//addition
test(Evaluator.Evaluate("3+2", null), 5);

//subtraction
test(Evaluator.Evaluate("3-2", null), 1);

//multiple addition and subtraction
test(Evaluator.Evaluate("3+2+1", null), 6);
test(Evaluator.Evaluate("3-2-1", null), 0);

//addition and subtraction with zeros
test(Evaluator.Evaluate("0+0", null), 0);
test(Evaluator.Evaluate("0-0", null), 0);

//addition and subtraction with large numbers
test(Evaluator.Evaluate("99999999 + 11111111", null), 111111110);
test(Evaluator.Evaluate("99999999 - 11111111", null), 88888888);

//Multiplications and divisions
//multiplication
test(Evaluator.Evaluate("3*2", null), 6);

//division
test(Evaluator.Evaluate("4/2", null), 2);

//division with remainder
test(Evaluator.Evaluate("1/3", null), 0);

//multiple multiplication and division
test(Evaluator.Evaluate("3*2*1", null), 6);
test(Evaluator.Evaluate("3/2/1", null), 1);

//multiplication and division with zeros
test(Evaluator.Evaluate("0*0", null), 0);
try{test(Evaluator.Evaluate("0/0", null), 0);}
catch (ArgumentException e) { Console.WriteLine("test passed! "); }

//Parenthesis
//expression with parenthesis
test(Evaluator.Evaluate("(3+2)", null), 5);
test(Evaluator.Evaluate("(3-2)", null), 1);
test(Evaluator.Evaluate("(3*2)", null), 6);
test(Evaluator.Evaluate("(3/2)", null), 1);

//parenthesis at the start of expression
test(Evaluator.Evaluate("(3/2)+2", null),3);
test(Evaluator.Evaluate("(3-2)*2", null),2);

//parenthesis at the last of expression
test(Evaluator.Evaluate("2+(3/2)", null), 3);
test(Evaluator.Evaluate("2*(3-2)", null), 2);

//parenthesis with single number
test(Evaluator.Evaluate("(2)", null), 2);

//parenthesis at the middle of expression
test(Evaluator.Evaluate("2+(3/2)-2", null), 1);
test(Evaluator.Evaluate("2*(3-2)/2", null), 1);

//multiple parenthesis in expression
test(Evaluator.Evaluate("(2+2)-(2*2)", null), 0);
test(Evaluator.Evaluate("(3/3)-(3-3)", null), 1);

//parenthesis in parenthesis
test(Evaluator.Evaluate("((2))", null), 2);
test(Evaluator.Evaluate("(2/(2+2))", null), 0);
test(Evaluator.Evaluate("((2+2)/2)", null), 2);
test(Evaluator.Evaluate("(2*(2+2)/2)", null), 4);
test(Evaluator.Evaluate("2*(2*(2+2)/2)/2", null), 4);

//multiple parenthesis
test(Evaluator.Evaluate("((((2))))", null), 2);
test(Evaluator.Evaluate("2+(2-(2*(2/2)*2)-2)+2", null), 0);

//Delegates
//expression with delegates
test(Evaluator.Evaluate("x1+1",(x) => 5), 6);
test(Evaluator.Evaluate("3*x2", (x) => 5), 15);
test(Evaluator.Evaluate("x3+x3", (x) => 3), 6);
test(Evaluator.Evaluate("2/(x4-2)+(x4*1)", (x) => 3), 5);

//delegates with edge values
test(Evaluator.Evaluate("x1+1", (x) => 0), 1);
test(Evaluator.Evaluate("2*x2", (x) => 11111111), 22222222);
test(Evaluator.Evaluate("2/x2", (x) => -2), -1);

//delegates with edge names
test(Evaluator.Evaluate("z0", (x) => 2), 2);
test(Evaluator.Evaluate("aaaa00000", (x) => 2), 2);
test(Evaluator.Evaluate("aabbbcc123", (x) => 2), 2);

//multiple delegates
test(Evaluator.Evaluate("x1+x2+1", (x) => 2), 5);
test(Evaluator.Evaluate("x1+x2+x3+x4", (x) => x[1]), 202);

//spaces
test(Evaluator.Evaluate("1  ", null), 1);
test(Evaluator.Evaluate(" 1 + 1 ", null), 2);
test(Evaluator.Evaluate(" (1 * 1 )", null), 1);
test(Evaluator.Evaluate(" x1 + x2 ", (x) => 1), 2);

//Exceptions
//empty expression
try { Evaluator.Evaluate("", null); }
catch (ArgumentException e) { Console.WriteLine("test passed! "); }

//wrong sign formatting
try { Evaluator.Evaluate("+", null);            throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("1-", null);           throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("**1", null);          throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("/1/1", null);         throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }

//wrong use of parenthesis
try { Evaluator.Evaluate("(())", null);         throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("(1", null);           throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("1)", null);           throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("(1))", null);         throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }

//wrong names of delegate
try { Evaluator.Evaluate("x", (x)=>1);          throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("1x", null);           throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("x1x", null);          throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }
try { Evaluator.Evaluate("!+1", null);          throw new Exception("test failed! "); } catch (ArgumentException e) { Console.WriteLine("test passed! "); }

//looking up t reveals it has no value
try { Evaluator.Evaluate("a1", (x) => throw new ArgumentNullException()); }         catch (ArgumentException e) { Console.WriteLine("test passed! "); }


/// <summary>
/// gets the test result and expected result and compares them
/// </summary>
/// <param name="actual">represents the actual test result </param>
/// <param name="expected">represents the expected test result </param>
/// <exception cref="ArgumentException"> when expected and actual result does not matched </exception>
/// <returns></returns>
void test(int actual, int expected)
{
    if (expected != actual)
        throw new ArgumentException("test failed");

    Console.WriteLine("test passed! ");
}


