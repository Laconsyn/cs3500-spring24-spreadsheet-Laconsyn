global using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void constructor()
        {
            Spreadsheet spreadsheet = new Spreadsheet();
        }

        [TestMethod]
        public void SetCellContentsFormula()
        {
            //edge formula
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("1+(1-1)*1/1"));
            sheet.SetCellContents("A1", new Formula("1.5e-2*_bc3"));
            sheet.SetCellContents("A1", new Formula("0"));
            sheet.SetCellContents("A1", new Formula("3/0"));

            //return set
            ISet<string> actual = sheet.SetCellContents("B1", new Formula("A1+A2"));
            Assert.AreEqual(actual.Count, 1);

            actual = sheet.SetCellContents("A1", new Formula("C1+A2+(3)"));
            Assert.AreEqual(actual.Count, 2); //dependent

            actual = sheet.SetCellContents("A2", new Formula("C1*bc23"));
            Assert.AreEqual(actual.Count, 3); //multiple dependents

            actual = sheet.SetCellContents("C1", new Formula("2"));
            Assert.AreEqual(actual.Count, 4); //indirect dependents


        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormulaNullExceptions()
        {
            Spreadsheet sheet = new Spreadsheet();
            Formula f = null;
            sheet.SetCellContents("A1", f);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void FormulaCircularExceptions()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A2", new Formula("A1+1"));
            sheet.SetCellContents("A1", new Formula("A2+1"));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaFormatExceptions()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("???"));
        }

        [TestMethod]
        public void SetCellContentsDouble()
        {
            //edge number values
            Spreadsheet sheet = new Spreadsheet();
            Assert.AreEqual(sheet.SetCellContents("A1", 0).Count, 1);
            Assert.AreEqual(sheet.SetCellContents("A1", 0.01).Count, 1);
            Assert.AreEqual(sheet.SetCellContents("A1", -100).Count, 1);
            Assert.AreEqual(sheet.SetCellContents("A1", 999999999).Count, 1);

            //return set
            sheet.SetCellContents("B1", 1);

        }

        [TestMethod]
        public void SetCellContentsText()
        {
            //edge string values
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", "");
            sheet.SetCellContents("A1", "1");
            sheet.SetCellContents("A1", "1A2B3C");
            sheet.SetCellContents("A1", "A2");
            sheet.SetCellContents("A1", "!:/_");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void stringNullException()
        {
            Spreadsheet sheet = new Spreadsheet();
            string s = null;
            sheet.SetCellContents("A1", s);
        }

        [TestMethod]
        public void SetCellNames()
        {
            //edge names
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a", 0);
            sheet.SetCellContents("_1a2b3cde", 0);
            sheet.SetCellContents("____", 0);

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void stringNameExceptions()
        {
            //string method, null name check
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, "a"); 
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void doubleNameExceptions()
        {
            //double method, invalid character name check
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("a+1", new Formula("1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void formulaNameExceptions()
        {
            //formula method, name[0] digit check
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("1a", new Formula("1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void getContentsNameException()
        {
            //get cell contents method, empty string name check
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("");
        }


        [TestMethod]
        public void GetCellContents()
        {

            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 1);
            Assert.AreEqual(sheet.GetCellContents("A1"),1d);
            sheet.SetCellContents("B1", "a");
            Assert.AreEqual(sheet.GetCellContents("B1"), "a");
            sheet.SetCellContents("C1", new Formula("A1+bc2+2"));
            Assert.AreEqual(sheet.GetCellContents("C1"), new Formula("A1+bc2+2"));

            //empty
            Assert.AreEqual(sheet.GetCellContents("D1"), "");
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells()
        {

            Spreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", new Formula("A2")); //get name
            Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().Count(),1);
            sheet.SetCellContents("B1", "a"); //multiple names
            Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().Count(), 2);
            sheet.SetCellContents("A1", 0); //repeat setter
            Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().Count(), 2);
        }
    }
}