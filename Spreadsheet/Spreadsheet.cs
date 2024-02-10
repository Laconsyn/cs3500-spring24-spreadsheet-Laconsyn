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
    public class Spreadsheet : AbstractSpreadsheet
    {

        private DependencyGraph dependency; //depedency between every non-empty cell
        private Hashtable cells; //name of cell as the key, content of cell as the value

        public Spreadsheet() 
        {
            dependency = new DependencyGraph();
            cells = new Hashtable();
        }

        public override object GetCellContents(string name)
        {
            checkNameValid(name);

            if (cells.Contains(name)) 
                return cells[name]; //string, double, or formula
            else
                return ""; //not declared yet

        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            ICollection names = cells.Keys;

            //copy all keys to the output list
            List<string> output = new List<string>();
            foreach (var item in names)
                output.Add((string)item);

            return output;
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            return set(name, number);
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            return set(name, text);
        }

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

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependency.GetDependees(name);
        }


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
