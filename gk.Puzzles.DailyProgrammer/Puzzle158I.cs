using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace gk.Puzzles.DailyProgrammer
{
    /// <summary>
    /// In the far future, demand for pre-manufactured housing, particularly in planets such as Mars, 
    /// has risen very high. In fact, the demand is so much that traditional building planning 
    /// techniques are taking too long, when faced with the "I want it now!" mentality of the denizens 
    /// of the future. You see an opportunity here - if you can cheaply generate building designs, you 
    /// are sure to turn a huge profit.
    /// You decide to use ASCII to design your buildings. However, as you are lazy and wish to churn 
    /// out many designs quickly, you decide to simply give the computer a string, and have the computer 
    /// make the building for you.
    /// </summary>
    /// <remarks>
    /// Input
    /// Input will be to STDIN, or read from a file input.txt located in the working directory of the 
    /// operating system. Input consists of one line between 1 to 231-1 length. The line can be assumed 
    /// to only contain the lowercase letters from a to j, and numbers from 1 to 9. It can also be assumed 
    /// that a number will not immediately follow another number in the string (i.e. if the 4th character 
    /// is a number, the 5th character is guaranteed to be a letter, not a number.)
    /// Output
    /// Output will be to STDOUT, or written to a file output.txt in the working directory. For each 
    /// non-number character of input, the output will contain a vertical line composed as shown here 
    /// http://i.imgur.com/twPajPG.png
    /// [1] :
    /// A letter can also be prefixed by a number n, where n is an integer between 1 and 9. 
    /// In this case, n whitespaces must be at the bottom of the vertical line. For example, 3b would output
    /// +
    /// +
    /// S
    /// S
    /// S
    /// Where spaces are identified with a capital S (In your actual output, it should be actual spaces). Sample Inputs and Outputs
    /// </remarks>
    /// <example>
    /// Sample input:
    /// j3f3e3e3d3d3c3cee3c3c3d3d3e3e3f3fjij3f3f3e3e3d3d3c3cee3c3c3d3d3e3e3fj
    /// 
    /// Sample output:
    /// .                 . .                 .
    /// .*              **...**              *.
    /// .***          ****...****          ***.
    /// *-----      ------***------      -----*
    /// *-------  --------***--------  -------* 
    /// *+++++++**++++++++***++++++++**+++++++*
    /// -+++++++--++++++++---++++++++--+++++++-
    /// -       --        ---        --       -
    /// +       ++        +++        ++       +
    /// +       ++        +++        ++       +  
    /// </example>
    /// <additional>
    /// Try making your own buildings as well instead of just testing the samples. 
    /// Don't forget to include your favourite ASCII construction with your solution!
    /// </additional>
    [Puzzle("158I")]
    public class Puzzle158I : IPuzzle
    {
        private const string _numeric = "123456789";
        private const string _alpha = "abcdefghij";
        private Dictionary<string,string> _symbolMap = new Dictionary<string, string>()
            {
                {"a","+"},
                {"b","++"},
                {"c","++-"},
                {"d","++--"},
                {"e","++--*"},
                {"f","++--**"},
                {"g","++--***"},
                {"h","++--***."},
                {"i","++--***.."},
                {"j","++--***..."}
            };

        public object Run(params object[] parameters)
        {
            if(parameters == null 
                || parameters.Length == 0 
                || string.IsNullOrEmpty(parameters[0].ToString())) 
                throw new ArgumentNullException("parameters");

            var seed = parameters[0].ToString();

            var strings = generateStrings(seed).ToList();

            return transposeStrings(strings);
        }

        private IEnumerable<string> generateStrings(string seed)
        {
            var spaceNumber = 0;
            var strings = new List<string>();
            
            foreach (char c in seed)
            {
                if (_alpha.Contains(c.ToString()) == false
                    && _numeric.Contains(c.ToString()) == false) 
                    throw new ApplicationException(string.Format("encountered invalid character '" + c + "'."));

                string s = "";
                if (_numeric.Contains(c.ToString()))
                {
                    if (spaceNumber != 0) throw new ApplicationException(string.Format("encountered two sequential numeric values when only one is allowed."));

                    spaceNumber = int.Parse(c.ToString());
                    continue;
                }

                if (spaceNumber > 0)
                {
                    spaceNumber.Times(() => s += " ");
                    spaceNumber = 0;
                }
                
                s += _symbolMap[c.ToString()];
                strings.Add(s);
            }
            return strings;
        }

        private string transposeStrings(List<string> strings)
        {
            StringBuilder sb = new StringBuilder();

            int highest = strings.ToList().Max(x => x.Length);
            int row = highest;

            for (int i = 0; i <= highest; i++)
            {
                foreach (var s in strings)
                {
                    if (s.Length-1 < row)
                        sb.Append(" ");
                    else
                        sb.Append(s[row]);
                }
                row--;
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
