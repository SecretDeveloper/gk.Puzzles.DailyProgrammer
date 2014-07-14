using System;

namespace gk.Puzzles.DailyProgrammer
{
    public static class Extensions
    {
        public static void Times(this int times, Action action)
        {
            for (int i = 0; i < times; i++)
            {
                action();
            }
        }
    }
}