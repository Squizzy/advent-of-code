using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AoC2025;
using Microsoft.VisualBasic;

namespace AdventOfcode
{
    class AdventOfCode
    {

        // private static (T, bool) get_new_day<T>(T day)
        // {
        //     try {
        //         day = new(); 
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine($"Error: {e}");
        //         return (day, null);
        //     }
        //     return (day, null);
        // }

        // public int run_day<T>(T day)
        // {
        //     if (!day.DayDataLoadedSuccessfully) return 1;
        //     // day.DisplayData();
        //     day.Part1();
        //     day.Part2();

        //     return 0;
        // }

        public static int Main()
        {
            Console.WriteLine("Advent Of Code");

            Day1 day1 = new();
            if (!day1.DayDataLoadedSuccessfully) return 1;
            // day.DisplayData();
            day1.Part1();
            day1.Part2();
                
            Day2 day2 = new();
            if (!day2.DayDataLoadedSuccessfully) return 1;
            // day.DisplayData();
            day2.Part1();
            day2.Part2();


            // Day2 day2 = new();
            // // day2.DisplayDay2Data();
            // day2.Part1();
            // day2.Part2();

            // Day3 day3 = new();
            // // day3.DisplayDay3Data();
            // day3.Part1();
            // day3.Part2();

            // Day4 day4 = new();
            // // day4.DisplayDay4Data();
            // day4.Part1();
            // day4.Part2();

            // Day5 day5 = new();
            // // day5.DisplayDay5Data(); 
            // day5.Part1();
            // day5.Part2();

            return 0;

        }
    }
}