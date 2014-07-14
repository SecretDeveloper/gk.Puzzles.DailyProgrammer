using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace gk.Puzzles.DailyProgrammer.CmdLine
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var param = args;

                if (param.Length == 0)
                {
                    param = GatherInput();
                }
                if (param.Length == 0)
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                while(param[0].ToLowerInvariant() != "q")
                {
                    if (param.Length == 0)
                    {
                        param = GatherInput();
                    }
                    string puzzleId = param[0];

                    string puzzleParam = "";
                    if (param.Length > 1)
                        puzzleParam = param[1];
                    else
                    {
                        puzzleParam = DefaultPuzzleParam(puzzleId);
                    }

                    Run(puzzleId, puzzleParam);
                    
                    //param = GatherInput();
                    param[0] = "Q";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
            finally
            {
            }
        }

        private static string[] GatherInput()
        {
            Console.WriteLine();
            Console.WriteLine("Enter the puzzleid and the parameters");
            Console.WriteLine("Enter Q to quit");
            return Console.ReadLine().Split(' ');
        }

        private static bool Run(string puzzleId, string puzzleParam)
        {
            IPuzzle puzzle = GetPuzzle(puzzleId);
            var result = puzzle.Run(puzzleParam);

            Console.Write(result);
            Console.WriteLine();
            return false;
        }

        private static IPuzzle GetPuzzle(string puzzleId)
        {
            var assembly = typeof (IPuzzle).Assembly;
            var puzzleType = GetTypesWithPuzzleAttribute(assembly, puzzleId);
            if(puzzleType == null) throw new ApplicationException("unable to find puzzle with id " + puzzleId);
            IPuzzle instance = (IPuzzle)Activator.CreateInstance(puzzleType);
            return instance;
        }

        static Type GetTypesWithPuzzleAttribute(Assembly assembly, string puzzleId)
        {
            foreach (Type type in assembly.GetTypes())
            {
                var attrib = type.GetCustomAttributes(typeof (PuzzleAttribute), true);
                if (attrib.Length == 1)
                {
                    var puzzleAttrib = attrib[0] as PuzzleAttribute;
                    if(puzzleAttrib != null && puzzleAttrib.PuzzleID.Equals(puzzleId))
                        return type;
                }
            }
            return null;
        }

        private static string DefaultPuzzleParam(string puzzleId)
        {
            switch(puzzleId)
            {
                case "158I":
                    return "j3f3e3e3d3d3c3cee3c3c3d3d3e3e3f3fjij3f3f3e3e3d3d3c3cee3c3c3d3d3e3e3fj";
                case "158H":
                    return @"18
1.6 1.2 7.9 3.1
1.2 1.6 3.4 7.2
2.6 11.6 6.8 14.0
9.6 1.2 11.4 7.5
9.6 1.7 14.1 2.8
12.8 2.7 14.0 7.9
2.3 8.8 2.6 13.4
1.9 4.4 7.2 5.4
10.1 6.9 12.9 7.6
6.0 10.0 7.8 12.3
9.4 9.3 10.9 12.6
1.9 9.7 7.5 10.5
9.4 4.9 13.5 5.9
10.6 9.8 13.4 11.0
9.6 12.3 14.5 12.8
1.5 6.8 8.0 8.0
6.3 4.7 7.7 7.0
13.0 10.9 14.0 14.5";
                default:
                    throw new ApplicationException("Unknown puzzle id");
            }
        }
    }
}
