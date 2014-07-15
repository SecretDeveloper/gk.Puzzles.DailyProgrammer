using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gk.Puzzles.DailyProgrammer;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod0()
        {
            string A = @"AAABBBCCC";
            string B = @"ABC";


            var reg = new REListFilter();
            var o = reg.Run(A, B);

            Assert.IsTrue(reg.Validate(o.ToString(), new List<string>(){A}, new List<string>(){B}));
        }

        [TestMethod]
        public void TestGenerateRegexs()
        {
            var reg = new REListFilter();

            var t = reg.GenerateRegexs("test");
            var result = new List<string>() { "^", "^t", "^te", "^tes", "^test", "t", "te", "tes", "test", "test$", "e", "es", "est", "est$", "s", "st", "st$", "t$", "$", };
            Assert.AreEqual(t.Count, result.Count);

            result.All(x => t.Keys.Contains(x));
        }

        [TestMethod]
        public void TestExpandStringWithAllDotVariants()
        {
            var reg = new REListFilter();

            var dict = new Dictionary<string, int>();
            var t = reg.ExpandStringWithAllDotVariants(dict, "test");
            var result = new List<string>() { "test", ".est", "t.st", "te.t", "tes.", "..st", "t..t", "te..", "...t", ".e.t", "t.s.", "t...", ".e..", "..s.", "...t", "...." };
            Assert.AreEqual(t.Count, result.Count);

            result.All(x => t.Keys.Contains(x));
        }
        

        [TestMethod]
        public void TestStarWars()
        {
            string A = @"A New Hope
The Empire Strikes Back
Return of the Jedi
The Phantom Menace
Attack of the Clones
Revenge of the Sith";


            string B = @"The Wrath of Khan
The Search for Spock
The Voyage Home
The Final Frontier
The Undiscovered Country
Generations
First Contact
Insurrection
Nemesis";


            var reg = new REListFilter();
            var o = reg.Run(A, B);


            Assert.IsTrue(reg.Validate("M | [TN]|B", new List<string>() { A }, new List<string>() { B }));
            Assert.IsTrue(reg.Validate(o.ToString(), new List<string>() { A }, new List<string>() { B }));

        }


        [TestMethod]
        public void TestNames()
        {
            string A = @"jacob 
mason 
ethan 
noah 
william 
liam 
jayden 
michael 
alexander 
aiden";
            string B = @"sophia 
emma 
isabella 
olivia 
ava 
emily 
abigail 
mia 
madison 
elizabeth";


            var reg = new REListFilter();
            var o = reg.Run(A, B);
            
            Assert.IsTrue(reg.Validate(o.ToString(), new List<string>() { A }, new List<string>() { B }));

        }

        [TestMethod]
        public void TestPresidents()
        {
            var A = @"washington adams jefferson jefferson madison madison monroe monroe adams jackson jackson van-buren harrison polk taylor pierce buchanan lincoln lincoln grant grant hayes garfield cleveland harrison cleveland mckinley mckinley roosevelt taft wilson wilson harding coolidge hoover roosevelt roosevelt roosevelt roosevelt truman eisenhower eisenhower kennedy johnson nixon nixon carter reagan reagan bush clinton clinton bush bush obama obama".Replace(" ", "\n");
            var B = @"clinton jefferson adams pinckney pinckney clinton king adams jackson adams clay van-buren van-buren clay cass scott breckinridge mcclellan seymour greeley tilden hancock blaine cleveland harrison bryan bryan parker bryan roosevelt hughes cox davis smith hoover landon wilkie dewey dewey stevenson stevenson nixon goldwater humphrey mcgovern ford carter mondale dukakis bush dole gore kerry mccain romney".Replace(" ", "\n");


            var reg = new REListFilter();
            Assert.IsTrue(reg.Validate("bu|[rn]t|[coy]e|[mtg]a|j|iso|n[hl]|[ae]d|lev|sh|[lnd]i|[po]o|ls"
                , A.Split('\n').ToList()
                , B.Split('\n').ToList().Except(A.Split('\n').ToList()).ToList()));

            Assert.IsTrue(reg.Validate("a.a|a..i|j|oo|a.t|i..o|i..n|bu|n.e|ay.|r.e$|tr|po|v.l"
                , A.Split('\n').ToList()
                , B.Split('\n').ToList().Except(A.Split('\n').ToList()).ToList()));

            var o = reg.Run(A, B);

             var SetA = A.Split('\n').ToList();
            var SetB = B.Split('\n').ToList().Except(A.Split('\n').ToList()).ToList();

            var notMatched = SetA.Where(x => Regex.IsMatch(x, o.ToString()) == false);
            var matchedB = SetB.Where(x => Regex.IsMatch(x, o.ToString()));

            Assert.IsTrue(reg.Validate(o.ToString(), SetA, SetB));

        }
    }
}
