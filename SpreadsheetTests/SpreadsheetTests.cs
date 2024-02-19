/// <summary>
/// Author:    Cheuk Yin Lau
/// Partner:   None
/// Date:      08-02-2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and [Your Name(s)] - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Cheuk Yin Lau, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents
///
///    tests for spreadsheet class. 
/// </summary>
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.IO;
using System.Xml;

namespace SpreadsheetTests
{
    /// <summary>
    /// tests for the spreadsheet class. 
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// test any errors on calling the empty constructor
        /// </summary>
        [TestMethod]
        public void constructor()
        {
            Spreadsheet spreadsheet0 = new Spreadsheet();
            Spreadsheet spreadsheet3 = new Spreadsheet(x => true, x => x, "first version");
        }

        /// <summary>
        /// loop of dependent and dependee in formulas shoud throw Circular exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void FormulaCircularExceptions()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A2", "=A1+1");
            sheet.SetContentsOfCell("A1", "=A2+1");
        }

        /// <summary>
        /// wrong format of formula should throw formula format exception in formula class
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaFormatExceptions()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=???");
        }

        /// <summary>
        /// test the SetCellContents method with string content param. 
        /// Valid edge strings are tried as content. 
        /// </summary>
        [TestMethod]
        public void SetCellContents()
        {
            //edge string values
            Spreadsheet sheet = new Spreadsheet();
            //double
            sheet.SetContentsOfCell("A1", "0");
            sheet.SetContentsOfCell("A1", "3.1415927");
            sheet.SetContentsOfCell("A1", "-10000.01");
            //string
            sheet.SetContentsOfCell("A1", "");
            sheet.SetContentsOfCell("A1", "1A2B3C");
            sheet.SetContentsOfCell("A1", "A2");
            sheet.SetContentsOfCell("A1", "!:/_");
            //formula
            sheet.SetContentsOfCell("A1", "=1+(1-1)*1/1");
            sheet.SetContentsOfCell("A1", "=1.5e-2*_bc3");
            sheet.SetContentsOfCell("A1", "=0");
            sheet.SetContentsOfCell("A1", "=3/0");

            sheet.SetContentsOfCell("A1", "=A2+A3");
            sheet.SetContentsOfCell("A1", "=B1+B2");
        }

        /// <summary>
        /// test the SetCellContents method with string name as param. 
        /// Valid edge strings are tried as content. 
        /// </summary>
        [TestMethod]
        public void SetCellNames()
        {
            //edge names
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a123", "0");
            sheet.SetContentsOfCell("AA123", "value");
            sheet.SetContentsOfCell("ABCDEF1", "=A1");

        }

        /// <summary>
        /// test set cell method return value
        /// </summary>
        [TestMethod]
        public void SetCellReturn()
        {
            Spreadsheet sheet = new Spreadsheet();
            //return set
            IList<string> actual = sheet.SetContentsOfCell("B1", "=A1+A2");
            Assert.AreEqual(actual.Count, 1); //formula

            actual = sheet.SetContentsOfCell("B2", "1");
            Assert.AreEqual(actual.Count, 1); //double

            actual = sheet.SetContentsOfCell("B3", "abc");
            Assert.AreEqual(actual.Count, 1); //string

            actual = sheet.SetContentsOfCell("A1", "=C1+A2+(3)");
            Assert.AreEqual(actual.Count, 2); //dependent

            actual = sheet.SetContentsOfCell("A2", "=C1*bc23");
            Assert.AreEqual(actual.Count, 3); //multiple dependents

            actual = sheet.SetContentsOfCell("C1", "=2");
            Assert.AreEqual(actual.Count, 4); //indirect dependents

        }

        /// <summary>
        /// null name should throw invalid name exception.
        /// set string content method is tested. 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void stringNameExceptions()
        {
            //string method, null name check
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a_1", "a");
        }

        /// <summary>
        /// name with invalid characters should throw invalid name exception.
        /// set formula content method is tested. 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void doubleNameExceptions()
        {
            //double method, invalid character name check
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1a", "1");
        }

        /// <summary>
        /// name with wrong format should throw invalid name exception.
        /// set double content method is tested. 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void formulaNameExceptions()
        {
            //formula method, name[0] digit check
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("1a", "=1");
        }

        /// <summary>
        /// empty name with length == 0 should throw invalid name exception.
        /// get cell content method is tested. 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void getContentsNameException()
        {
            //get cell contents method, empty string name check
            Spreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("");
        }


        /// <summary>
        /// test the GetCellContents method with double, string, formula type content. 
        /// empty string is also tested. 
        /// </summary>
        [TestMethod]
        public void GetCellContents()
        {

            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1");
            Assert.AreEqual(sheet.GetCellContents("A1"), 1d);
            sheet.SetContentsOfCell("B1", "a");
            Assert.AreEqual(sheet.GetCellContents("B1"), "a");
            sheet.SetContentsOfCell("C1", "=A1+bc2+2");
            Assert.AreEqual(sheet.GetCellContents("C1"), new Formula("A1+bc2+2"));

            //empty
            Assert.AreEqual(sheet.GetCellContents("D1"), "");
        }

        /// <summary>
        /// test the GetNamesOfAllNonemptyCells with double, string and formula type content
        /// Cells are also modified to check any mistakes by the method during the process. 
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCells()
        {

            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=A2"); //get name
            Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().Count(), 1);
            sheet.SetContentsOfCell("B1", "a"); //multiple names
            Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().Count(), 2);
            sheet.SetContentsOfCell("A1", "0"); //repeat setter
            Assert.AreEqual(sheet.GetNamesOfAllNonemptyCells().Count(), 2);
        }

        /// <summary>
        /// test get cell value method
        /// </summary>
        [TestMethod]
        public void GetCellValue()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "1"); //double
            Assert.AreEqual(sheet.GetCellValue("A1"), 1d);
            sheet.SetContentsOfCell("B1", "a"); //string
            Assert.AreEqual(sheet.GetCellValue("B1"), "a");
            sheet.SetContentsOfCell("C1", "=A1+2"); //formula
            Assert.AreEqual(sheet.GetCellValue("C1"), 3d);

            //empty
            Assert.AreEqual(sheet.GetCellValue("D1"), "");
        }

        /// <summary>
        /// test get cell value with wrong input
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetCellValueException()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "a");

            sheet.SetContentsOfCell("A1", "=1+2");

            sheet.SetContentsOfCell("C1", "=A1+B1");
            Console.Write(sheet.GetCellValue("C1"));
        }

        /// <summary>
        /// test getXML method
        /// </summary>
        [TestMethod]
        public void GetXML()
        {
            //encode and decode in xml
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "0");
            sheet.SetContentsOfCell("B1", "a");
            sheet.SetContentsOfCell("C1", "=A1+2");
            string actual = sheet.GetXML();

            Assert.IsTrue(actual.Contains("<Name>A1</Name><Content>0</Content>"));
            Assert.IsTrue(actual.Contains("<Name>B1</Name><Content>a</Content>"));
            Assert.IsTrue(actual.Contains("<Name>C1</Name><Content>=A1+2</Content>"));
        }

        /// <summary>
        /// test save method
        /// </summary>
        [TestMethod]
        public void Save()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("spreadsheet.txt");

        }

        /// <summary>
        /// test get save version method
        /// </summary>
        [TestMethod]
        public void GetSavedVersion()
        {
            //get string version
            Spreadsheet sheet = new Spreadsheet(x=>true, x=>x, "first version");
            sheet.Save("spreadsheet2.txt");
            sheet.GetSavedVersion("spreadsheet2.txt");

            //double version
            sheet = new Spreadsheet(x => true, x => x, "1.1");
            sheet.Save("spreadsheet3.txt");
            sheet.GetSavedVersion("spreadsheet2.txt");
        }

        /// <summary>
        /// stress test for spreadsheet set cell
        /// </summary>
        [TestMethod]
        public void StressTestSet()
        {
            Spreadsheet sheet = new Spreadsheet();
            
            //set with many recursive calls
            for(int i = 1; i < 1000; i++)
            {
                sheet.SetContentsOfCell("A"+i, "=A" + (i-1));
            }

            IList<string> d = sheet.SetContentsOfCell("A0", "0");
        }

        /// <summary>
        /// stress test for spreadsheet get cell value
        /// </summary>
        [TestMethod]
        public void StressTestGet()
        {
            Spreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("A0", "0");

            //get with many recursive calls
            for (int i = 0; i < 1000; i++)
            {
                sheet.SetContentsOfCell("A" + i, "=A" + (i - 1));
            }

            object content = sheet.GetCellContents("A999");
        }
    }
}

