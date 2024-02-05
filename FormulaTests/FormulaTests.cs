using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void Constructor()
        {
            Formula formula1 = new Formula("1+1");
            Formula formula3 = new Formula("x+1", s => s.ToUpper(), s => s.Equals("X"));
        }

        [TestMethod]
        public void SyntaxException()
        {
            try { Formula formula = new Formula("?");           Assert.Fail(); } catch (FormulaFormatException e){ }
            try { Formula formula = new Formula("");            Assert.Fail(); } catch (FormulaFormatException e) { }
            try { Formula formula = new Formula("((1+1)))");    Assert.Fail(); } catch (FormulaFormatException e) { }
            try { Formula formula = new Formula("((1)");        Assert.Fail(); } catch (FormulaFormatException e) { }
            try { Formula formula = new Formula("+1");          Assert.Fail(); } catch (FormulaFormatException e) { }
            try { Formula formula = new Formula("1+");          Assert.Fail(); } catch (FormulaFormatException e) { }
            try { Formula formula = new Formula("(+");          Assert.Fail(); } catch (FormulaFormatException e) { }
            try { Formula formula = new Formula("(1)1");        Assert.Fail(); } catch (FormulaFormatException e) { }

            //invalid token
            try { Formula formula = new Formula("x", s=>s, s=>false);               Assert.Fail(); } catch (FormulaFormatException e) { }
            try { Formula formula = new Formula("x", s => s, s => s.Equals("y"));   Assert.Fail(); } catch (FormulaFormatException e) { }
        }

        [TestMethod]
        public void NumbersEvaluate()
        {
            Assert.AreEqual(new Formula("(1+1)-(1-1)*1+1/1").Evaluate(null), 3d);
            Assert.AreEqual(new Formula("(1-(1*1)/(1/1)+1)").Evaluate(null), 1d);
            Assert.AreEqual(new Formula("(0)").Evaluate(null), 0d);
            Assert.AreEqual(new Formula("1.1+0.01").Evaluate(null), 1.11);
            Assert.AreEqual(new Formula("99999999 + 11111111").Evaluate(null), 111111110d);

            //division zero
            object error = new Formula("1/0").Evaluate(null);
            Assert.IsTrue(error is FormulaError);
        }

        [TestMethod]
        public void VariablesEvaluate()
        {
            Assert.AreEqual(new Formula("x+y+1").Evaluate(s => 1), 3d);

            Formula formula = new Formula("x+1");
            Assert.AreEqual(formula.Evaluate(x => 1), 2d);
            Assert.AreEqual(formula.Evaluate(x => 2), 3d);

            //edge names
            Assert.AreEqual(new Formula("aaaa").Evaluate(s => 1), 1d);
            Assert.AreEqual(new Formula("z0").Evaluate(s => 1), 1d);
            Assert.AreEqual(new Formula("a1b2c3d4").Evaluate(s => 1), 1d);
            Assert.AreEqual(new Formula("a_1__2___3").Evaluate(s => 1), 1d);
            Assert.AreEqual(new Formula("____").Evaluate(s => 1), 1d);

            Assert.IsTrue(new Formula("x+1").Evaluate(x=>throw new ArgumentException()) is FormulaError);
        }

        [TestMethod]
        public void Spaces()
        {
            Assert.AreEqual(new Formula("  1  + 1-  (1  ) ").Evaluate(null), 1d);
            Assert.AreEqual(new Formula(" 1 +  x  + yz   ").Evaluate(s=>1), 3d);

            try {new Formula(" x  + y z ").Evaluate(s=>1); Assert.Fail(); } catch (FormulaFormatException e) { }
        }

        [TestMethod]
        public void GetVariables()
        {
            IEnumerable<string> actual = new Formula("(x+y2)+_z_+1+a1a").GetVariables();
            Assert.AreEqual(actual.Count(), 4);

            Formula formula = new Formula("(_y2_+ 2)");
            actual = formula.GetVariables();
            foreach(string s in actual)
                Assert.AreEqual(s, "_y2_");
            
        }

        [TestMethod]
        public void ToString()
        {
            Assert.AreEqual(new Formula("  x  + y1-  (1  ) ").ToString(), "x+y1-(1)");
            Assert.AreEqual(new Formula("x+y", s=>s.ToUpper(),s=>true).ToString(), "X+Y");

        }

        [TestMethod]
        public void Equals()
        {
            Assert.IsTrue(new Formula("  x  + y1-  (1  ) ").Equals (new Formula("x+y1-(1)")));
            Assert.IsTrue(new Formula("x+y",s=>s.ToUpper(),s=>true).Equals(new Formula("X+Y")));

            Assert.IsFalse(new Formula("1+1+1").Equals(new Formula("3")));
            Assert.IsFalse(new Formula("X+Y", s => s.ToLower(), s => true).Equals(new Formula("X+Y")));
        }

        [TestMethod]
        public void Operator()
        {
            Assert.IsTrue(new Formula("  x  + y1-  (1  ) ") == new Formula("x+y1-(1)"));
            Assert.IsTrue(new Formula("x+y", s => s.ToUpper(), s => true) == (new Formula("X+Y")));
            Assert.IsFalse(new Formula("1+1+1") == (new Formula("3")));
            Assert.IsFalse(new Formula("X+Y", s => s.ToLower(), s => true) == (new Formula("X+Y")));


            Assert.IsTrue(new Formula("1+1+1") != (new Formula("3")));
            Assert.IsTrue(new Formula("X+Y", s => s.ToLower(), s => true) != (new Formula("X+Y")));
            Assert.IsFalse(new Formula("  x  + y1-  (1  ) ") != new Formula("x+y1-(1)"));
            Assert.IsFalse(new Formula("x+y", s => s.ToUpper(), s => true) != (new Formula("X+Y")));
        }

        [TestMethod]
        public void GetHashCode()
        {
            Assert.AreEqual(new Formula("  x  + y1-  (1  ) ").GetHashCode(), new Formula("x+y1-(1)").GetHashCode());
            Assert.AreEqual(new Formula("x+y", s => s.ToUpper(), s => true).GetHashCode(), (new Formula("X+Y").GetHashCode()));
            Assert.AreNotEqual(new Formula("1+1+1").GetHashCode(), (new Formula("3").GetHashCode()));
            Assert.AreNotEqual(new Formula("X+Y", s => s.ToLower(), s => true).GetHashCode(), (new Formula("X+Y").GetHashCode()));
        }
    }
}