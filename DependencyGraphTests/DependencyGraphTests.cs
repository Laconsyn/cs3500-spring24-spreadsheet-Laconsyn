// Tests for the Dependency Graph class.
// Check edge cases and possible errors. 

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }


        /// <summary>
        ///dependee's size
        ///</summary>
        [TestMethod()]
        public void DependeeSize()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            Assert.AreEqual(0, t["a"]);

            t.AddDependency("b", "a");
            Assert.AreEqual(1, t["a"]);

            t.RemoveDependency("b", "a");
            Assert.AreEqual(0, t["a"]);
        }

        /// <summary>
        ///simple has dependent
        ///</summary>
        [TestMethod()]
        public void HasDependent()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            Assert.IsFalse(t.HasDependents("b"));

            t.AddDependency("a", "b");
            Assert.IsTrue(t.HasDependees("b"));

            t.RemoveDependency("a", "b");
            Assert.IsFalse(t.HasDependees("b"));
        }

        /// <summary>
        ///simple has dependee
        ///</summary>
        [TestMethod()]
        public void HasDependee()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            Assert.IsFalse(t.HasDependees("a"));

            t.AddDependency("a", "b");
            Assert.IsTrue(t.HasDependents("a"));

            t.RemoveDependency("a", "b");
            Assert.IsFalse(t.HasDependents("a"));
        }

        /// <summary>
        ///get dependents
        ///</summary>
        [TestMethod()]
        public void GetDependents()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            //empty
            List<string> actual = (List<string>) t.GetDependents("a");
            Assert.AreEqual(0,actual.Count);

            //same
            t.AddDependency("a", "a");
            actual = (List<string>)t.GetDependents("a");
            Assert.AreEqual("a", actual[0]);

            //multiple
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.RemoveDependency("a", "a");

            actual = (List<string>)t.GetDependents("a");
            Assert.AreEqual("b", actual[0]);
            Assert.AreEqual("c", actual[1]);
            Assert.AreEqual("d", actual[2]);
        }

        /// <summary>
        ///get dependees
        ///</summary>
        [TestMethod()]
        public void GetDependees()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();
            
            //empty
            List<string> actual = (List<string>)t.GetDependees("a");
            Assert.AreEqual(0, actual.Count);

            //same
            t.AddDependency("a", "a");
            actual = (List<string>)t.GetDependees("a");
            Assert.AreEqual("a", actual[0]);

            //multiple
            t.AddDependency("b", "a");
            t.AddDependency("c", "a");
            t.RemoveDependency("a", "a");
            actual = (List<string>)t.GetDependees("a");
            Assert.AreEqual("b", actual[0]);
            Assert.AreEqual("c", actual[1]);
        }

        /// <summary>
        ///get dependees
        ///</summary>
        [TestMethod()]
        public void AddDependency()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            //same
            t.AddDependency("a", "a");
            Assert.AreEqual(1, t["a"]);

            //multiple
            t.AddDependency("b", "a");
            Assert.AreEqual(2, t["a"]);

            //repeated
            t.AddDependency("b", "a");
            Assert.AreEqual(2, t["a"]);

            List<string> actual = (List<string>)t.GetDependees("a");
            Assert.AreEqual("a", actual[0]);
            Assert.AreEqual("b", actual[1]);

            t = new DependencyGraph();

            //multiple keys
            t.AddDependency("c", "b");
            t.AddDependency("a", "b");
            //empty string
            t.AddDependency("", "b");
            //special character
            t.AddDependency("!", "b");
            actual = (List<string>)t.GetDependees("b");
            Assert.AreEqual("c", actual[0]);
            Assert.AreEqual("a", actual[1]);
            Assert.AreEqual("", actual[2]);
            Assert.AreEqual("!", actual[3]);

        }
    }
}
