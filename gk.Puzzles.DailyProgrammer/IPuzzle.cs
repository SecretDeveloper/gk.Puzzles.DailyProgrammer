using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk.Puzzles.DailyProgrammer
{
    public interface IPuzzle
    {
        object Run(params object[] parameters);
    }
}
