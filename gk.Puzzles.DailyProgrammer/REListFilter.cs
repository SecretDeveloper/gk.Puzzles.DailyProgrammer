using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gk.Puzzles.DailyProgrammer
{
    [Puzzle("RE")]
    public class REListFilter : IPuzzle
    {

        /// <summary>
        /// Creates a regular expression that matches all items in the first set and none of the items in the second set.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Run(params object[] parameters)
        {
            if (parameters == null
                || parameters.Length == 0
                || string.IsNullOrEmpty(parameters[0].ToString()))
                throw new ArgumentNullException("parameters");

            string re = "";

            List<string> SetA = parameters[0].ToString().Replace("\r","").Split('\n').ToList(); // inclusion
            List<string> SetB = parameters[1].ToString().Replace("\r", "").Split('\n').ToList(); // exclusion

            // In case there are items in both sets we remove them from SetB to form 2 non intersecting sets.
            SetB = SetB.Except(SetA).ToList();

            Dictionary<string, int> scores = new Dictionary<string, int>();

            foreach (string item in SetA)
            {
                 foreach(var reg in GenerateRegexs(item))
                     scores[reg.Key] = reg.Value;
            }
            scores = ExpandStringWithSetVariants(scores);
            scores = ExpandStringWithAllDotVariants(scores);

            scores = calculateScore(scores, SetA, SetB); // rank the generated regexs by most successful SetA matches which have 0 SetB matches.

            var remaining = SetA;
            foreach (var score in scores.OrderByDescending(key => key.Value))
            {
                if (!Contains(score.Key, remaining, SetB)) continue;
                remaining = remaining.Where(x => !Regex.IsMatch(x, score.Key)).ToList();

                re = re + score.Key;
                if (Validate(re, SetA, SetB)) break;
                re += "|";
            }

            if (re.Substring(re.Length - 1, 1) == "|")
                re = re.Substring(0, re.Length - 1);

            return re;
        }

        private Dictionary<string, int> ExpandStringWithSetVariants(Dictionary<string, int> scores)
        {
            var result = new Dictionary<string, int>();
            foreach (var re in scores)
            {
                if (re.Key.Contains("^") || re.Key.Contains("$") || re.Key.Contains("-")) continue;
                
                result[re.Key] = re.Value;
                result["[" + re.Key + "]"] = re.Value;
            }
            return result;
        }

        private Dictionary<string, int> calculateScore(Dictionary<string,int> res, List<string> SetA, List<string> SetB)
        {
            Dictionary<string,int> results = new Dictionary<string, int>();

            var remaining = SetA;
            foreach (var re in res)
            {
                if (!Contains(re.Key, SetA, SetB)) continue;
                int score = SetA.Count(s => Regex.IsMatch(s, re.Key));

                if (Contains(re.Key, remaining, SetB))
                {
                    remaining = remaining.Where(x => !Regex.IsMatch(x, re.Key)).ToList();
                    score += remaining.Count(s => Regex.IsMatch(s, re.Key));
                }

                results[re.Key] = score;
            }
            return results;
        }

        public Dictionary<string,int> GenerateRegexs(string item)
        {
            // generate a 5 character long regex using the inputted item as source.
            int range = 4;
            string prepared = "^" + item + "$";

            Dictionary<string,int> results = new Dictionary<string, int>();

            // TEST = > ['T','E','S','T']
            for (int x = 0; x < prepared.Length; x++)
            {
                for (int i = 1; i <= range && i + x <= prepared.Length; i++)
                {
                    // ['T','TE',TES','TEST'....]
                    results[prepared.Substring(x, i)]=0;
                }
            }

            return results;
        }

        public Dictionary<string, int> ExpandStringWithAllDotVariants(Dictionary<string, int> res)
        {
            var result = new Dictionary<string,int>();
            foreach (var re in res)
            {
                ExpandStringWithAllDotVariants(result, re.Key);
            }
            return result;
        }

        
        public Dictionary<string, int> ExpandStringWithAllDotVariants(Dictionary<string,int> result, string item, int ndx = 0)
        {
            result[item] = 0;
            if (ndx >= item.Length) return result;
            
            var tmp = item.ToCharArray();
            if (item.Contains("[") || item.Contains("]")) return result;

            if (tmp[ndx] != '^' && tmp[ndx] != '$') // skip start and end line characters.
                tmp[ndx] = '.';
            string n = new string(tmp);
            result[n] = 0;
            ndx++;

            ExpandStringWithAllDotVariants(result, item, ndx);
            ExpandStringWithAllDotVariants(result, n, ndx);
            
            return result;
        }


        public List<string> SplitStringToListString(string item)
        {
            var result = new List<string>();
            foreach (var i in item.ToArray())
            {
                result.Add(i.ToString());   
            }
            return result;
        }

        /// <summary>
        /// Takes a regular expression and validates that it matches all of SetA and none of SetB.
        /// </summary>
        /// <param name="re"></param>
        /// <param name="SetA"></param>
        /// <param name="SetB"></param>
        /// <returns></returns>
        public bool Validate(string re, List<string> SetA, List<string> SetB)
        {
            Regex regex = new Regex(re);

            if (SetA.Any(s => !regex.IsMatch(s))) return false;  // if we dont match all of SetA return false.
            if (SetB.Any(regex.IsMatch)) return false; // if we match any of SetB return false

            return true;
        }

        /// <summary>
        /// Takes a regular expression and validates that it matches any of SetA and none of SetB.
        /// </summary>
        /// <param name="re"></param>
        /// <param name="SetA"></param>
        /// <param name="SetB"></param>
        /// <returns></returns>
        public bool Contains(string re, List<string> SetA, List<string> SetB)
        {
            Regex regex = new Regex(re);

            if (SetA.All(s => !regex.IsMatch(s))) return false;  // if we dont match any of SetA return false.
            if (SetB.Any(regex.IsMatch)) return false; // if we match any of SetB return false

            return true;
        }
    }
}
