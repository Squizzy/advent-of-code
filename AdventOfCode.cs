using System.Linq.Expressions;
using AoC2025;

namespace AdventOfcode
{
    class AdventOfCode
    {
        public static int Main()
        {
            Console.WriteLine("Advent Of Code");

            Day1 day;
            try {
                day = new(); 
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return 1;
            }

            // day.DisplayData();
            day.Part1();
            day.Part2();

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