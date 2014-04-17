using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk.Puzzles.DailyProgrammer
{
    public class PuzzleAttribute:Attribute
    {
        public string PuzzleID { get; set; }

        public PuzzleAttribute(string puzzleID)
        {
            PuzzleID = puzzleID;
        }
    }
}
