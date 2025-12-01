
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;

namespace AoC2025
{
    interface IAoCDay
    {
        public void DisplayData();

        public void Part1();

        public void Part2();
    }

    class Generics
    {
        public const int AoCYear = 2025;

        public static string DayNumOfAOC(int dayNum)
        {
            return $"{AoCYear} Day {dayNum:D2}";
        }

        public static string InputFileName(int dayNum, int partNum, bool sampleFile)
        {
            return $"{Generics.AoCYear}\\Day{dayNum}_Part{partNum}_{(sampleFile ? "sample" : "values")}.txt";
        }

        public static (List<string>, bool) LoadInputFile(string inputFilePath)
            {
                List<string> inputValues = [];
                try
                {
                    string? line;
                    using StreamReader sr = new(inputFilePath);
                    line = sr.ReadLine();

                    if (line == null)
                    {
                        Console.WriteLine($"File {inputFilePath} is empty");
                        return ([], false);
                    }
                    
                    while (line != null)
                    {
                        inputValues.Add(line);
                        line = sr.ReadLine();
                    }
                }
                catch
                {
                    Console.WriteLine($"Problem loading the data from file {inputFilePath}");
                    return ([], false);
                }
                finally
                {
                    
                    Console.WriteLine(inputValues.Count);
                }
                return (inputValues, true);
            }
    }

    class Day1 : IAoCDay
    {
        private int DayNum {get; set; }
        private int PartNum {get; set; }
        private string DayNumOfAoC {get; set; }
        private string InputFileName {get; set; }
        private bool SampleFile {get; set; }

        private const int dialMin = 0;
        private const int dialMax = 99;
        private const int dialStartingPoint = 50;
        private const int directionL = -1;
        private const int directionR = 1;
        private readonly List<(char, int)> rotations = [];

        public Day1()
        {
            SampleFile = false;

            DayNum = 1;
            PartNum = 1;
            
            // Set the day value
            DayNumOfAoC = Generics.DayNumOfAOC(dayNum:DayNum);
            Console.WriteLine(DayNumOfAoC);

            // set the input file
            InputFileName = Generics.InputFileName(dayNum:DayNum, partNum:PartNum, sampleFile:SampleFile);

            // load the data
            (List<string> inputValues, bool loadedStatus) = Generics.LoadInputFile(inputFilePath:InputFileName);

            // handle problem with loading data
            if (!loadedStatus)
            {
                Console.WriteLine("Error loading data, aborting");
                throw new FileLoadException ($"Error loading data file {InputFileName}. Aborting.");
            }

            // handle problem-specific data pre-parsing
            foreach (string line in inputValues)
            {
                char dir = line.Trim()[0];
                int rot = int.Parse(line.Trim()[1..]);
                rotations.Add((dir, rot));
            }
        }

        public void DisplayData()
        {
            Console.WriteLine($"{DayNumOfAoC} - data:");
            foreach ((char d, int r) in rotations)
            {
                Console.WriteLine($"{d}{r}\t=> dir: {d} - rot: {r}");
            }
            
        }

        public void Part1()
        {
            /*
            The safe has a dial with only an arrow on it; around the dial are the numbers 0 through 99 in order. As you turn the dial, it makes a small click noise as it reaches each number.
            The attached document (your puzzle input) contains a sequence of rotations, one per line, which tell you how to open the safe. A rotation starts with an L or R which indicates whether the rotation should be to the left (toward lower numbers) or to the right (toward higher numbers). Then, the rotation has a distance value which indicates how many clicks the dial should be rotated in that direction.
            So, if the dial were pointing at 11, a rotation of R8 would cause the dial to point at 19. After that, a rotation of L19 would cause it to point at 0.
            Because the dial is a circle, turning the dial left from 0 one click makes it point at 99. Similarly, turning the dial right from 99 one click makes it point at 0.
            So, if the dial were pointing at 5, a rotation of L10 would cause it to point at 95. After that, a rotation of R5 could cause it to point at 0.
            The dial starts by pointing at 50.
            You could follow the instructions, but your recent required official North Pole secret entrance security training seminar taught you that the safe is actually a decoy. The actual password is the number of times the dial is left pointing at 0 after any rotation in the sequence.
            */

            int current_position = dialStartingPoint;

            int count_zeroes = 0;

            foreach ((char d, int r) in rotations)
            {
                // add the current dial value
                int dir = d == 'R' ? directionR : d == 'L' ? directionL : 0;
                if (dir == 0) Console.WriteLine("Error in direction");
                current_position += dir * r;

                // handle rotating over the 0 value
                current_position = (current_position + 100) % (dialMax - dialMin + 1);

                // if 0, then add this to the count
                if (current_position == 0) count_zeroes += 1;
            }

            Console.WriteLine($"{DayNumOfAoC} - Part 1: {count_zeroes}");
        }

        public void Part2()
        {
            /*
            you're actually supposed to count the number of times any click causes the dial to point at 0, regardless of whether it happens during a rotation or at the end of one.
            Following the same rotations as in the above example, the dial points at zero a few extra times during its rotations:
                The dial starts by pointing at 50.
                The dial is rotated L68 to point at 82; during this rotation, it points at 0 once.
                The dial is rotated L30 to point at 52.
                The dial is rotated R48 to point at 0.
                The dial is rotated L5 to point at 95.
                The dial is rotated R60 to point at 55; during this rotation, it points at 0 once.
                The dial is rotated L55 to point at 0.
                The dial is rotated L1 to point at 99.
                The dial is rotated L99 to point at 0.
                The dial is rotated R14 to point at 14.
                The dial is rotated L82 to point at 32; during this rotation, it points at 0 once.
            In this example, the dial points at 0 three times at the end of a rotation, plus three more times during a rotation. So, in this example, the new password would be 6.
            Be careful: if the dial were pointing at 50, a single rotation like R1000 would cause the dial to point at 0 ten times before returning back to 50!
            */

            int new_position = 0;
            int current_position = dialStartingPoint;

            int count_zeroes = 0;

            foreach ((char d, int r) in rotations)
            {
                // add the current dial value
                int dir = d == 'R' ? directionR : d == 'L' ? directionL : 0;
                if (dir == 0) Console.WriteLine("Error in direction");

                int rot = r;
                while (rot >= 100)
                {
                    count_zeroes++;
                    rot -= 100;
                }

                new_position = current_position + dir * r;


// TODO
                if (dir == directionL)
                {
                    // if the new position is less than 0 we went over the 0, add 1
                    if (new_position < 0)
                    {
                        count_zeroes ++;
                        int diff_pos = r - current_position;
                        while (diff_pos > 100)
                        {
                            count_zeroes++;
                            diff_pos -= 100;
                        }
                    }
                }
                else
                {
                    // if (new_position > 0 && new_position < current_position)
                    // {
                    //     count_zeroes ++;
                    //     int diff_pos = r - current_position;
                    //     while (diff_pos > 100)
                    //     {
                    //         count_zeroes++;
                    //         diff_pos -= 100;
                    //     }
                    // }
                }

                // handle rotating over the 0 value
                current_position = (current_position + 100) % (dialMax - dialMin + 1);

                // if 0, then add this to the count
                if (current_position == 0) count_zeroes += 1;
            }

            Console.WriteLine($"{DayNumOfAoC} - Part 2: {count_zeroes}");
        }


    //region templateDate
    // class Day1 : IAoCDay
    // {
    //     private int day_num {get; set; }
    //     private string dayNumOfAoC {get; set; }

    //     public Day1()
    //     {
    //         day_num = 1;
    //         dayNumOfAoC = $"2025 Day {day_num:D2}";
    //         Console.WriteLine(dayNumOfAoC);
    //     }

    //     public void DisplayData()
    //     {
    //         Console.WriteLine($"{dayNumOfAoC}: data");
            
    //     }

    //     public void Part1()
    //     {
    //          Console.WriteLine($"{dayNumOfAoC}: Part 1");
    //     }

    //     public void Part2()
    //     {
    //          Console.WriteLine($"{dayNumOfAoC}: Part 2");
    //     }
    //endregion

    }

}