
// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta
// (Clarified meaning of dependent and dependee.)
// (Clarified names in solution/project structure.)
// Version 1.3 - Cheuk Yin Lau (Complete the content of the methods following the API)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    ///
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings. Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates. If an attempt is made to add an element to a
    /// set, and the element is already in the set, the set remains unchanged.
    ///
    /// Given a DependencyGraph DG:
    ///
    /// (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    /// (The set of things that depend on s)
    ///
    /// (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    /// (The set of things that s depends on)
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    // dependents("a") = {"b", "c"}
    // dependents("b") = {"d"}
    // dependents("c") = {}
    // dependents("d") = {"d"}
    // dependees("a") = {}
    // dependees("b") = {"a"}
    // dependees("c") = {"a"}
    // dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        //fields
        private int size; //The number of ordered pairs in the DependencyGraph.

        //using the name of cell as the string key, and a string list of cells as the value
        private Hashtable dependents; //search for dependents
        private Hashtable dependees; //search for dependees

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            //initialize
            size = 0;
            dependents = new Hashtable();
            dependees = new Hashtable();

        }
        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
        }
        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer. If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                //get the list of dependents of given key
                List<string> list = (List<string>)dependees[s];

                //return the size of the list. 0 if it is empty
                if (list == null)
                    return 0;

                return list.Count;
            }
        }
        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependents.ContainsKey(s);
        }
        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependees.ContainsKey(s);
        }
        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            //get the dependents of s
            IEnumerable<string> output = (List<string>)dependents[s];

            //return the dependents of s. Empty list if it is empty
            if (output == null)
                return new List<string>();
            return output;
        }
        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            //get the dependees of s
            List<string> output = (List<string>)dependees[s];

            //return the dependees of s. Empty list if it is empty
            if (output == null)
                return new List<string>();
            return output;
        }
        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        ///
        /// <para>This should be thought of as:</para>
        ///
        /// t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param> ///
        public void AddDependency(string s, string t)
        {
            //get the list of dependents of s
            List<string> s_dependents = (List<string>)GetDependents(s);
            
            //add t to s if it is not existed yet
            if (!s_dependents.Contains(t))
            {
                s_dependents.Add(t);
                size++; //update size
            }
            else //finish if the pair already exists
                return;

            //update the list of dependents
            if (dependents.ContainsKey(s))
                dependents[s] = s_dependents; //replace the original list
            else
                dependents.Add(s, s_dependents); //assign the new list

            //get the list of dependees
            List<string> t_dependees = (List<string>)GetDependees(t);

            if (!t_dependees.Contains(s))
                t_dependees.Add(s); //add s to t

            //update the list of dependees
            if (dependees.ContainsKey(t))
                dependees[t] = t_dependees; //replace the original list
            else
                dependees.Add(t, t_dependees); //assign the new list
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            //get the list of dependents
            List<string> s_dependents = (List<string>)GetDependents(s);

            //remove t from s if it exists
            if (s_dependents.Contains(t))
            {
                s_dependents.Remove(t);

                //remove s if no dependents left
                if(s_dependents.Count == 0)
                    dependents.Remove(s);

                size--; //update size
            }

            //get the list of dependees
            List<string> t_dependees = (List<string>)GetDependees(t);
            //remove s from t if it exists
            if (t_dependees.Contains(s))
            {
                t_dependees.Remove(s);

                //remove s if no dependents left
                if (t_dependees.Count == 0)
                    dependees.Remove(t);
            }
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r). Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //copy the cells in the list of dependents
            List<string> list = DeepCopy(s, dependents);

            //remove every original dependents
            foreach (string r in list)
            {
                RemoveDependency(s, r);
            }
            //add every given item to hash tables
            foreach (string r in newDependents)
            {
                AddDependency(s, r);
            }
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s). Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //copy the cells in the list of dependents
            List<string> list = DeepCopy(s, dependees);

            //remove every original dependents
            foreach (string t in list)
            {
                RemoveDependency(t, s);
            }
            //add every given item to hash tables
            foreach (string t in newDependees)
            {
                AddDependency(t, s);
            }
        }

        /// <summary>
        /// copy every given values to a new list
        /// </summary>
        /// <param name="s"> key string to get the original data</param>
        /// <param name="table">hash table to search </param>
        /// <returns> a new list containing the same data but not related to the hash table</returns>
        private List<string> DeepCopy(string s, Hashtable table)
        {
            
            List<string> list = new List<string>(); //output list
            List<string> originalDependents = (List<string>)table[s]; //get all cells

            //initialize new list if necessary
            if (originalDependents == null)
                originalDependents = new List<string>();

            //add every item to the output list
            foreach (string item in originalDependents)
                list.Add(item);

            return list;
        }

    }
}
