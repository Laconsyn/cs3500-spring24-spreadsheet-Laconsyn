/// <summary>
/// Author:    [Your Name]
/// Partner:   [Partner Name or None]
/// Date:      [Date of Creation]
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and [Your Name(s)] - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, [your name], certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents
///
///    [... and of course you should describe the contents of the 
///    file in broad terms here ...]
/// </summary>
using SpreadsheetUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{

    /// <summary>
    /// handles contents of spreadsheet cells. 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {

        private DependencyGraph dependency; //depedency between every non-empty cell
        private Hashtable cells; //name of cell as the key, cellContent of cell as the value
        private string version; //version of the spreadsheet
        private bool isChanged; //true if the spreadsheet is modified since the last save


        /// <summary>
        /// spreadsheet object with cells constructing based on the abstract spreadsheet. 
        /// </summary>
        public Spreadsheet() : this(x=>true, x=>x, "default")
        {}

        public Spreadsheet(
            Func<string, bool> IsValid, 
            Func<string, string> normalize, 
            string version)
            : base(IsValid, normalize, version)
        {
            dependency = new DependencyGraph();
            cells = new Hashtable();
            this.version = version;
            isChanged = false;
        }

        public Spreadsheet(
            string path, 
            Func<string, bool> IsValid,
            Func<string, string> normalize,
            string version)
            : this(IsValid, normalize, version)
        {
            try
            {
                //read file in path
                using (XmlReader reader = XmlReader.Create(path))
                {
                    string name = "";

                    while (reader.Read())
                    {
                        if (!reader.IsStartElement())
                            continue;

                        switch (reader.Name)
                        {
                            case "Name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "Content":
                                reader.Read();
                                SetContentsOfCell(name, reader.Value);
                                break;
                        }
                    }
                }
            } catch (Exception e) { throw new SpreadsheetReadWriteException(e.Message); }
        }

        public override bool Changed
        {
            get { return isChanged; }
            protected set => isChanged = value;
        }

        /// <summary>
        /// get cell cellContent of the given name
        /// </summary>
        /// <param name="name">given name to search the cellContent</param>
        /// <returns>a double, string or formula object in the cell</returns>
        public override object GetCellContents(string name)
        {
            checkNameValid(name);

            name = Normalize(name);

            if (cells.Contains(name)) 
                return cells[name]; //string, double, or formula
            else
                return ""; //not declared yet

        }

        /// <summary>
        /// get names of all non empty cells. 
        /// </summary>
        /// <returns> list of names of all non empty cells in IEnumerable</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            ICollection names = cells.Keys;

            //copy all keys to the output list
            List<string> output = new List<string>();
            foreach (var item in names)
                output.Add((string)item);

            return output;
        }

        /// <summary>
        /// set the cellContent of cell of given name. create one if not declared yet. 
        /// The given name and cellContent must follows the rules in abstract spreadsheet. 
        /// </summary>
        /// <param name="name">name of the cell</param>
        /// <param name="number">double number as the cellContent of the cell</param>
        /// <returns>all names of dependents of current cell in a list</returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return set(name, number);
        }

        /// <summary>
        /// set the cellContent of cell of given name. create one if not declared yet. 
        /// The given name and cellContent must follows the rules in abstract spreadsheet. 
        /// </summary>
        /// <param name="name">name of the cell</param>
        /// <param name="number">text string as the cellContent of the cell</param>
        /// <returns>all names of dependents of current cell in a list</returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            return set(name, text);
        }

        /// <summary>
        /// set the cellContent of cell of given name. create one if not declared yet. 
        /// The given name and cellContent must follows the rules in abstract spreadsheet. 
        /// </summary>
        /// <param name="name">name of the cell</param>
        /// <param name="number">a formula as the cellContent of the cell</param>
        /// <returns>all names of dependents of current cell in a list</returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            return set(name, formula);
        }

        /// <summary>
        /// get any other cells directly depends of the given cell. 
        /// The input name is validate upon entry
        /// </summary>
        /// <param name="name">name of the cell as the dependee</param>
        /// <returns>list of names of the cells depends on current cell in IEnumerable</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            checkNameValid(name);
            return dependency.GetDependees(Normalize(name));
        }


        /// <summary>
        /// check if the name follows all the naming rules. Does nothing if it follows. 
        /// </summary>
        /// <param name="name">name of the cell to be checked</param>
        /// <exception cref="InvalidNameException">exception if name is invalid</exception>
        private void checkNameValid(string name)
        {

            name = Normalize(name);

            //loose definition of token
            if (name.Count() == 0) //empty string
                throw new InvalidNameException();

            if (Char.IsDigit(name[0])) //not var if first char is digit
                throw new InvalidNameException();

            foreach (Char ch in name) //not var if contains anything than digit/letter/'_'
                if (!(Char.IsDigit(ch) || Char.IsLetter(ch)))
                    throw new InvalidNameException();

            //spreadsheet variable check
            if (Char.IsLetter(name[name.Count()-1])) //no digit
                throw new InvalidNameException();

           //iterate to check each char
            bool firstHalf = true;
            foreach (char ch in name)
            {
                if (Char.IsLetter(ch) && firstHalf) //only letters in first half
                    continue;

                if (Char.IsDigit(ch) && !firstHalf) //only digits in second half
                    continue;

                else if (Char.IsDigit(ch) && firstHalf) //first digit in second half
                {
                    firstHalf = false;
                    continue;
                }

                throw new InvalidNameException();
            }

            //outside program check
            if(!IsValid(name))
                throw new InvalidNameException();

        }

        /// <summary>
        /// set a cell to any cellContent with any given type and values
        /// </summary>
        /// <typeparam name="E">generic type of the cellContent</typeparam>
        /// <param name="name">name of the cell to be set </param>
        /// <param name="obj">value of the cell with type E</param>
        /// <returns>all names of dependents of current cell in a set</returns>
        private IList<string> set <E> (string name, E obj)
        {
            name = Normalize(name); 

            checkNameValid(name); //exception if name invalid

            cells[name] = obj;

            //get dependents
            List<string> dependents = new List<string>();
            dependents.AddRange(GetCellsToRecalculate(name));

            isChanged = true;

            return dependents;
        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            checkNameValid(name); //exception if name invalid

            name = Normalize(name); //normalize

            if (content.Count() == 0) //empty content
                return new List<string>();

            //try to parse to double
            try 
            {
                double value = Double.Parse(content);
                return SetCellContents(name, value);
            } catch (FormatException e) {}

            //check if it is formula
            if (content[0] == '=')
            {
                content = content.Substring(1);

                //propagate IsValid and Normalize to formula
                Formula formula = new Formula(content, Normalize, IsValid);

                //remove original dependency
                if (cells.ContainsKey(name))
                {
                    object cellContent = cells[name];
                    if ((cellContent is Formula)) //remove if any
                    {
                        //remove dependees
                        IEnumerable<string> originalDependees = ((Formula)cellContent).GetVariables();
                        foreach (string variable in originalDependees)
                            dependency.RemoveDependency(name, variable);
                    }
                }

                //add new dependency
                IEnumerable<string> dependees = formula.GetVariables();
                foreach (string variable in dependees)
                    dependency.AddDependency(name, variable);

                return SetCellContents(name, formula);
            }

            //string cellContent
            return SetCellContents(name, content);

        }

        public override string GetSavedVersion(string filename)
        {
            //read file
            using (XmlReader reader = XmlReader.Create(filename))
            {
                try
                {
                    //get version
                    string value = reader.GetAttribute("version");
                    return value;
                } catch (Exception e) { throw new SpreadsheetReadWriteException(""); }
            }
        }

        public override void Save(string filename)
        {
            //write XML
            using(XmlWriter writer = XmlWriter.Create(filename))
            {
                writeXML(writer);
            }

            isChanged = false;
        }

        public override string GetXML()
        {
            StringBuilder sb = new StringBuilder();
            // write XML
            using (XmlWriter writer = XmlWriter.Create(sb))
            {
                writeXML(writer);
            }

            return sb.ToString();
        }

        private void writeXML(XmlWriter writer)
        {
            // write XML
            using (writer)
            {
                writer.WriteStartDocument();
                writer.WriteAttributeString("version", version);

                //convert each cell in order
                IEnumerable<string> names = GetNamesOfAllNonemptyCells(); //name of all non-null cells
                foreach (string name in names)
                {
                    //get content in string
                    object content = GetCellContents(name);
                    string stringContent = content.ToString();

                    //get formula in string
                    if (content is Formula)
                    {
                        stringContent = ((Formula)content).ToString();
                        stringContent = "=" + stringContent;
                    }

                    //write to XML
                    writer.WriteElementString("Name", name);
                    writer.WriteElementString("Content", stringContent);
                }

                writer.WriteEndDocument();
            }
        }

        public override object GetCellValue(string name)
        {
            object content = GetCellContents(name);
            //formula
            try
            {
                Formula formula = (Formula)content;
                return formula.Evaluate
                    (variable =>
                    {
                        object variableContent = GetCellContents(variable);
                        if (variableContent is double) //double
                            return (double)variableContent;
                        else if (variableContent is Formula)//formula
                            return (double)GetCellValue("="+((Formula)variableContent).ToString()); //recursive call
                        else//string
                            throw new SpreadsheetReadWriteException("string content encounted in evaluation");
                    }
                    );
            }
            catch (FormatException e)
            {
                return content; //double or string}

            }
        }

        
    }
}
