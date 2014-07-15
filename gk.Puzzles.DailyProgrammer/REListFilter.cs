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
            
            //scores = ExpandStringWithSetVariants(scores);
            scores = ExpandStringWithAllDotVariants(scores);

            //scores = calculateScore(scores, SetA, SetB); // rank the generated regexs by most successful SetA matches which have 0 SetB matches.
            scores = calculateWeightedScore(scores, SetA, SetB); // rank the generated regexs using weighting algorithm

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
                result["[" + re.Key + "]*"] = re.Value;
                result["[" + re.Key + "]+"] = re.Value;
                result["[" + re.Key + "]?"] = re.Value;
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

        private Dictionary<string, int> calculateWeightedScore(Dictionary<string, int> res, List<string> SetA, List<string> SetB)
        {
            /*
             * List all re and count the words they match to.
             * List all words and count the re that matches to them
             * find the word with the lowest number of matches, take re and find the one that matches the highest number of words.
             * refresh remaining words list by removing matched words.
             */
            Dictionary<string, int> results = new Dictionary<string, int>();

            Dictionary<string, int> regexs = new Dictionary<string, int>();
            Dictionary<string, string> wordMatches = new Dictionary<string, string>();

            // Weed out all of those regexes that are not at least partial matches to our solution.
            res = res.Where(x => Contains(x.Key, SetA, SetB)).ToDictionary(i=>i.Key, i=>i.Value);

            var remaining = SetA;

            string re = "";
            while (remaining.Count >0) // keep looping until we find a solution.
            {
                regexs = CalculateWeightedScoreForRegexs(res, SetA, SetB, remaining, regexs);
                wordMatches = CalculateWeightedScoreForWords(res, SetA, SetB, remaining, regexs);

                var lowestMatchers = LowestMatchers(wordMatches); // take the item with the lowest number of matches
                var highestMatchingRE = "";
                int matchingCount = 0;

                foreach (var lowestMatcher in lowestMatchers)
                {
                    foreach (var kvp in lowestMatcher.Value.Split('\t'))
                    {
                        if (regexs.ContainsKey(kvp) && regexs[kvp] > matchingCount)
                        {
                            highestMatchingRE = kvp;
                            matchingCount = regexs[kvp];
                        }
                    }
                }
                if(highestMatchingRE == "") 
                    break;

                results[highestMatchingRE] = matchingCount; // Add to our results
                re = highestMatchingRE + "|" + re;
                remaining = SetA.Where(x => !Regex.IsMatch(x, re.Substring(0, re.Length - 1))).ToList(); // remove matched items
            }

            return results;
        }

        private static Dictionary<string, string> LowestMatchers(Dictionary<string, string> wordMatches)
        {
            var result = new Dictionary<string, string>();
            int highCount = 0;
            foreach(var item in wordMatches.OrderBy(sre => { return sre.Value.Split('\t').Count().ToString(); }))
            {
                int cnt = item.Value.Split('\t').Count();
                if (highCount == 0)
                    highCount = cnt;
                if (highCount != cnt) break; // jump out if we have dropped a cardinality level.
                result[item.Key] = item.Value;
            }
            return result;
        }

        private Dictionary<string, string> CalculateWeightedScoreForWords(Dictionary<string, int> res, List<string> SetA, List<string> SetB, List<string> remaining, Dictionary<string, int> regexs)
        {
            Dictionary<string,string> results = new Dictionary<string, string>();
            foreach (var item in remaining)
            {
                foreach (var re in res)
                {
                    if (Regex.IsMatch(item, re.Key))
                    {
                        results[item] = !results.ContainsKey(item) ? re.Key : results[item] + "\t" + re.Key;
                    }
                }
            }
            return results;
        }

        private Dictionary<string, int> CalculateWeightedScoreForRegexs(Dictionary<string, int> res, List<string> SetA, List<string> SetB, List<string> remaining, Dictionary<string, int> regexs)
        {
            foreach (var re in res)
            {
                if (!Contains(re.Key, SetA, SetB)) continue; // skip if it does not partially match SetA and none of SetB
                int score = remaining.Count(s => Regex.IsMatch(s, re.Key)); // a re score is the number of remaining items it matches.

                regexs[re.Key] = score;
            }
            return regexs;
        }

        public Dictionary<string,int> GenerateRegexs(string item)
        {
            // generate a 5 character long regex using the inputted item as source.
            int range = 4;
            string prepared = "^" + item + "$";

            Dictionary<string,int> results = new Dictionary<string, int>();
            
            for (int x = 0; x < prepared.Length; x++)
            {
                for (int i = 1; i <= range && i + x <= prepared.Length; i++)
                {
                    var p = prepared.Substring(x, i);
                    results[p.ToLower()]= 0;
                    results[p.ToLower() + "+"] = 0;
                    results[p.ToLower() + "*"] = 0;
                    results[p.ToLower() + "?"] = 0;

                    results[p.ToUpper()] = 0;
                    results[p.ToUpper()+"+"] = 0;
                    results[p.ToUpper() + "*"] = 0;
                    results[p.ToUpper() + "?"] = 0;
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
