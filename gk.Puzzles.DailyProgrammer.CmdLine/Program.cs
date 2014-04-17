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

                while(param[0] != "Q")
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
                    
                    param = GatherInput();
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
                case "158_I":
                    return "j3f3e3e3d3d3c3cee3c3c3d3d3e3e3f3fjij3f3f3e3e3d3d3c3cee3c3c3d3d3e3e3fj";
                default:
                    throw new ApplicationException("Unknown puzzle id");
            }
        }
    }
}
