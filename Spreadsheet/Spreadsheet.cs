// handles spreadsheet cell contents. Cheuk Yin Lau, 2024-02-09, CS 3500
using SpreadsheetUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{

    /// <summary>
    /// handles contents of spreadsheet cells. 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {

        private DependencyGraph dependency; //depedency between every non-empty cell
        private Hashtable cells; //name of cell as the key, content of cell as the value

        /// <summary>
        /// spreadsheet object with cells constructing based on the abstract spreadsheet. 
        /// </summary>
        public Spreadsheet() 
        {
            dependency = new DependencyGraph();
            cells = new Hashtable();
        }

        /// <summary>
        /// get cell content of the given name
        /// </summary>
        /// <param name="name">given name to search the content</param>
        /// <returns>a double, string or formula object in the cell</returns>
        public override object GetCellContents(string name)
        {
            checkNameValid(name);

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
        /// set the content of cell of given name. create one if not declared yet. 
        /// The given name and content must follows the rules in abstract spreadsheet. 
        /// </summary>
        /// <param name="name">name of the cell</param>
        /// <param name="number">double number as the content of the cell</param>
        /// <returns>all names of dependents of current cell in a set</returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            return set(name, number);
        }

        /// <summary>
        /// set the content of cell of given name. create one if not declared yet. 
        /// The given name and content must follows the rules in abstract spreadsheet. 
        /// </summary>
        /// <param name="name">name of the cell</param>
        /// <param name="number">text string as the content of the cell</param>
        /// <returns>all names of dependents of current cell in a set</returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            return set(name, text);
        }

        /// <summary>
        /// set the content of cell of given name. create one if not declared yet. 
        /// The given name and content must follows the rules in abstract spreadsheet. 
        /// </summary>
        /// <param name="name">name of the cell</param>
        /// <param name="number">a formula as the content of the cell</param>
        /// <returns>all names of dependents of current cell in a set</returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            checkNameValid(name); //exception if name invalid

            if (((object)formula)==null) //exception if formula null
                throw new ArgumentNullException(name);

            //remove original dependency
            if (cells.ContainsKey(name))
            {
                object content = cells[name];
                if((content is Formula)) //remove if any
                {
                    //remove dependees
                    IEnumerable<string> originalDependees = ((Formula)content).GetVariables();
                    foreach (string variable in originalDependees)
                        dependency.RemoveDependency(name, variable);
                }
                    
            }

            //add new dependency
            IEnumerable<string> dependees = formula.GetVariables();
            foreach(string variable in dependees)
            {
                dependency.AddDependency(name, variable);
            }

            //check validity and set content
            ISet<string> output = set(name, formula);

            return output;
        }

        /// <summary>
        /// get any other cells directly depends of the given cell
        /// </summary>
        /// <param name="name">name of the cell as the dependee</param>
        /// <returns>list of names of the cells depends on current cell in IEnumerable</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependency.GetDependees(name);
        }


        /// <summary>
        /// check if the name follows all the naming rules. Does nothing if it follows. 
        /// </summary>
        /// <param name="name">name of the cell to be checked</param>
        /// <exception cref="InvalidNameException">exception if name is invalid</exception>
        private void checkNameValid(string name)
        {
            if(name == null) //name null
                throw new InvalidNameException();

            if(name.Count() == 0) //empty string
                throw new InvalidNameException();

            if (Char.IsDigit(name[0])) //not var if first char is digit
                throw new InvalidNameException();

            foreach (Char ch in name) //not var if contains anything than digit/letter/'_'
                    if (!(Char.IsDigit(ch) || Char.IsLetter(ch) || ch == '_'))
                        throw new InvalidNameException();
        }

        /// <summary>
        /// set a cell to any content with any given type and values
        /// </summary>
        /// <typeparam name="E">generic type of the content</typeparam>
        /// <param name="name">name of the cell to be set </param>
        /// <param name="obj">value of the cell with type E</param>
        /// <returns>all names of dependents of current cell in a set</returns>
        /// <exception cref="ArgumentNullException">exception if content is null</exception>
        private ISet<string> set <E> (string name, E obj)
        {
            checkNameValid(name); //exception if name invalid

            if(obj == null) //exception if obj null
                throw new ArgumentNullException(name);

            cells[name] = obj;

            //get dependents
            HashSet<string> dependents = new HashSet<string>();
            dependents.UnionWith(GetCellsToRecalculate(name));
            return dependents;
        }
    }
}
