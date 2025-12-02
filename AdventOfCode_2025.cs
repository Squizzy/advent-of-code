
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Security;
using System.Security.Cryptography;

namespace AoC2025
{
    interface IAoCDay
    {
        public bool DayDataLoadedSuccessfully {get; init; }
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
        public bool DayDataLoadedSuccessfully  {get; init; }

        private int DayNum {get; init; }
        private int PartNum {get; init; }
        private string DayNumOfAoC {get; init; }
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

            DayDataLoadedSuccessfully = loadedStatus;

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

            int currentPosition = dialStartingPoint;

            int zeroesCount = 0;

            foreach ((char d, int r) in rotations)
            {
                // add the current dial value
                int dir = d == 'R' ? directionR : d == 'L' ? directionL : 0;
                if (dir == 0) Console.WriteLine("Error in direction");
                currentPosition += dir * r;

                // handle rotating over the 0 value
                currentPosition = (currentPosition + 100) % (dialMax - dialMin + 1);

                // if 0, then add this to the count
                if (currentPosition == 0) zeroesCount += 1;
            }

            Console.WriteLine($"{DayNumOfAoC} - Part 1: {zeroesCount}");
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

            int newPosition;
            int currentPosition = dialStartingPoint;

            int zeroesCount = 0;

            bool fromZero = false; // when rotating left, flag to indicating starting from 0

            foreach ((char d, int r) in rotations)
            {
                // add the current dial value
                int dir = d == 'R' ? directionR : d == 'L' ? directionL : 0;
                if (dir == 0) Console.WriteLine("Error in direction");

                // if the rotation is more than 100 units, 
                // then add all these spare turns as they will always go over 0
                int spare_turns = r >= 100 ? r/100 : 0;
                zeroesCount += spare_turns;

                // rotate the dial but do not count the spare rotations
                newPosition = currentPosition + dir * (r - spare_turns * 100);

                // if decreasing:
                if (dir == directionL)
                {
                    // if the new position now is 
                    // less or equal to 0 we went over the 0, add 1
                    // if we are starting from 0, then do not count this as it did not step over 0
                    zeroesCount += (newPosition <= 0 && !fromZero) ? 1 : 0;
                }
                // if increasing
                // if this ends up as 100 or more it stopped at 0 or went over
                else
                {
                    zeroesCount += newPosition > 99 ? 1: 0;
                }

                
                // if the new position was less than 0, reset it to the correct dial value
                currentPosition = (newPosition + 100) % (dialMax - dialMin + 1);
                fromZero = currentPosition == 0;
            }

            Console.WriteLine($"{DayNumOfAoC} - Part 2: {zeroesCount}");
        }
    }

    class Day2 : IAoCDay
    {
        public bool DayDataLoadedSuccessfully  {get; init; }

        private int DayNum {get; init; }
        private int PartNum {get; init; }
        private string DayNumOfAoC {get; init; }
        private string InputFileName {get; set; }
        private bool SampleFile {get; set; }

        private const int dialMin = 0;
        private const int dialMax = 99;
        private const int dialStartingPoint = 50;
        private const int directionL = -1;
        private const int directionR = 1;
        private readonly List<(char, int)> rotations = [];

        public Day2()
        {
            SampleFile = false;

            DayNum = 2;
            PartNum = 1;
            
            // Set the day value
            DayNumOfAoC = Generics.DayNumOfAOC(dayNum:DayNum);
            Console.WriteLine(DayNumOfAoC);

            // set the input file
            InputFileName = Generics.InputFileName(dayNum:DayNum, partNum:PartNum, sampleFile:SampleFile);

            // load the data
            (List<string> inputValues, bool loadedStatus) = Generics.LoadInputFile(inputFilePath:InputFileName);

            DayDataLoadedSuccessfully = loadedStatus;

            // handle problem with loading data
            if (!loadedStatus)
            {
                Console.WriteLine("Error loading data, aborting");
                throw new FileLoadException ($"Error loading data file {InputFileName}. Aborting.");
            }

            // handle problem-specific data pre-parsing

            foreach (string line in inputValues)
            {
            //     char dir = line.Trim()[0];
            //     int rot = int.Parse(line.Trim()[1..]);
            //     rotations.Add((dir, rot));
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
            */

            Console.WriteLine($"{DayNumOfAoC} - Part 1: {null}");
        }

        public void Part2()
        {
            /*
            */

            Console.WriteLine($"{DayNumOfAoC} - Part 2: {null}");
        }
    }



    //region templateDate
    // class Day1 : IAoCDay
    // {
    //     public bool DayDataLoadedSuccessfully  {get; init; }

    //     private int DayNum {get; init; }
    //     private int PartNum {get; init; }
    //     private string DayNumOfAoC {get; init; }
    //     private string InputFileName {get; set; }
    //     private bool SampleFile {get; set; }

    //     private const int dialMin = 0;
    //     private const int dialMax = 99;
    //     private const int dialStartingPoint = 50;
    //     private const int directionL = -1;
    //     private const int directionR = 1;
    //     private readonly List<(char, int)> rotations = [];

    //     public Day1()
    //     {
    //         SampleFile = false;

    //         DayNum = 1;
    //         PartNum = 1;
            
    //         // Set the day value
    //         DayNumOfAoC = Generics.DayNumOfAOC(dayNum:DayNum);
    //         Console.WriteLine(DayNumOfAoC);

    //         // set the input file
    //         InputFileName = Generics.InputFileName(dayNum:DayNum, partNum:PartNum, sampleFile:SampleFile);

    //         // load the data
    //         (List<string> inputValues, bool loadedStatus) = Generics.LoadInputFile(inputFilePath:InputFileName);

    //         DayDataLoadedSuccessfully = loadedStatus;

    //         // handle problem with loading data
    //         if (!loadedStatus)
    //         {
    //             Console.WriteLine("Error loading data, aborting");
    //             throw new FileLoadException ($"Error loading data file {InputFileName}. Aborting.");
    //         }

    //         // handle problem-specific data pre-parsing
    //         foreach (string line in inputValues)
    //         {
    //             char dir = line.Trim()[0];
    //             int rot = int.Parse(line.Trim()[1..]);
    //             rotations.Add((dir, rot));
    //         }
    //     }

    //     public void DisplayData()
    //     {
    //         Console.WriteLine($"{DayNumOfAoC} - data:");
    //         foreach ((char d, int r) in rotations)
    //         {
    //             Console.WriteLine($"{d}{r}\t=> dir: {d} - rot: {r}");
    //         }
            
    //     }

    //     public void Part1()
    //     {
    //         /*
    //         */

    //         Console.WriteLine($"{DayNumOfAoC} - Part 1: {null}");
    //     }

    //     public void Part2()
    //     {
    //         /*
    //         */

    //         Console.WriteLine($"{DayNumOfAoC} - Part 2: {null}");
    //     }
    // }

    //endregion


}