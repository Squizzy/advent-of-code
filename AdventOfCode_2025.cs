
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using icecream;

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
                    
                    // Console.WriteLine($"Number of lines read from the input file: {inputValues.Count}");
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

        private static readonly List<(long,long)> IdRanges = [];

        private static void GetRangesList(List<string> rawList)
        {
            foreach (string rawRange in rawList)
            {
                List<string> idRangeList = [.. rawRange.Split(',')];
                foreach (string range in idRangeList)
                {
                    string[] rangeHighAndLow = [.. range.Split('-')];
                    IdRanges.Add((long.Parse(rangeHighAndLow[0]), long.Parse(rangeHighAndLow[1])));
                }
            }
        }

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
            GetRangesList(inputValues);
        }

        public void DisplayData()
        {
            Console.WriteLine($"{DayNumOfAoC} - data:");
            foreach ((long low, long high) in IdRanges)
            {
                Console.WriteLine($"Low: {low}\tHigh: {high}");
            }
            
        }

        public void Part1()
        {
            /*
            you can find the invalid IDs by looking for any ID which is made only of some sequence of digits repeated twice. So, 55 (5 twice), 6464 (64 twice), and 123123 (123 twice) would all be invalid IDs.
            None of the numbers have leading zeroes; 0101 isn't an ID at all. (101 is a valid ID that you would ignore.)
            Your job is to find all of the invalid IDs that appear in the given ranges. In the above example:
                11-22 has two invalid IDs, 11 and 22.
                95-115 has one invalid ID, 99.
                998-1012 has one invalid ID, 1010.
                1188511880-1188511890 has one invalid ID, 1188511885.
                222220-222224 has one invalid ID, 222222.
                1698522-1698528 contains no invalid IDs.
                446443-446449 has one invalid ID, 446446.
                38593856-38593862 has one invalid ID, 38593859.
                The rest of the ranges contain no invalid IDs.
            Adding up all the invalid IDs in this example produces 1227775554.
            */

            long invalidIdsTotal = 0;

            foreach ((long low, long high) in IdRanges)
            {
                for (long currentId = low; currentId < high + 1; currentId++)
                {
                    string stringId = currentId.ToString();
                    int l = stringId.Length;
                    if (stringId[0..(l/2)] == stringId[(l/2)..l])
                    {
                        invalidIdsTotal += currentId;
                    }

                }
            }

            Console.WriteLine($"{DayNumOfAoC} - Part 1: {invalidIdsTotal}");
        }

        public void Part2()
        {
            /*
            Now, an ID is invalid if it is made only of some sequence of digits repeated at least twice. So, 12341234 (1234 two times), 123123123 (123 three times), 1212121212 (12 five times), and 1111111 (1 seven times) are all invalid IDs.
            From the same example as before:
                11-22 still has two invalid IDs, 11 and 22.
                95-115 now has two invalid IDs, 99 and 111.
                998-1012 now has two invalid IDs, 999 and 1010.
                1188511880-1188511890 still has one invalid ID, 1188511885.
                222220-222224 still has one invalid ID, 222222.
                1698522-1698528 still contains no invalid IDs.
                446443-446449 still has one invalid ID, 446446.
                38593856-38593862 still has one invalid ID, 38593859.
                565653-565659 now has one invalid ID, 565656.
                824824821-824824827 now has one invalid ID, 824824824.
                2121212118-2121212124 now has one invalid ID, 2121212121.
            Adding up all the invalid IDs in this example produces 4174379265.
            */

            long invalidIdsTotal = 0;
            HashSet<long> identifiedIllegals = [];

            foreach ((long low, long high) in IdRanges)
            {
                for (long currentId = low; currentId < high + 1; currentId++)
                {
                    string stringId = currentId.ToString();
                    int IdLength = stringId.Length;

                    // identify what the length can be divided into. max is l/2
                    for (int numberOfDigitsInPart = 1; numberOfDigitsInPart <= IdLength / 2; numberOfDigitsInPart++)
                    {
                        // if you can't split the Id in equal number of l length, skip
                        if (IdLength % numberOfDigitsInPart != 0) continue;

                        int numberOfParts = IdLength / numberOfDigitsInPart;

                        // store the parts of the Id separated in l parts into a set
                        HashSet<string> parts = [];
                        for (int partNumber = 0; partNumber < numberOfParts; partNumber++)
                        {
                            // part position start and end (for a division into numberOfParts parts)
                            int start = 0 +  numberOfDigitsInPart * partNumber;
                            int end = numberOfDigitsInPart + numberOfDigitsInPart * partNumber;
                            parts.Add(stringId[start..end]);
                        }

                        // if the set only contains one element, then it is an illegal id
                        if (parts.Count == 1) identifiedIllegals.Add(currentId);

                    }
                }


            }
            invalidIdsTotal = identifiedIllegals.Sum();

            // foreach (long illegal in identifiedIllegals) Console.WriteLine($"Illegal: {illegal}");
            
            Console.WriteLine($"{DayNumOfAoC} - Part 2: {invalidIdsTotal}");
        }
    }

    class Day3 : IAoCDay
    {
        public bool DayDataLoadedSuccessfully  {get; init; }

        private int DayNum {get; init; }
        private int PartNum {get; init; }
        private string DayNumOfAoC {get; init; }
        private string InputFileName {get; set; }
        private bool SampleFile {get; set; }

        private List<string> BatteriesBanks = [];

        public Day3()
        {
            SampleFile = false;

            DayNum = 3;
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
            BatteriesBanks = inputValues;


        }

        public void DisplayData()
        {
            Console.WriteLine($"{DayNumOfAoC} - data:");
            foreach (string batteryBank in BatteriesBanks)
            {
                Console.WriteLine($"Battery Bank: {batteryBank}");
            }
            
        }

        public void Part1()
        {
            /*
            The batteries are arranged into banks; each line of digits in your input corresponds to a 
            single bank of batteries. Within each bank, you need to turn on exactly two batteries; 
            the joltage that the bank produces is equal to the number formed by the digits on the batteries 
            you've turned on. For example, if you have a bank like 12345 and you turn on batteries 2 and 4, 
            the bank would produce 24 jolts. (You cannot rearrange batteries.)
            You'll need to find the largest possible joltage each bank can produce. In the above example:
                In 987654321111111, you can make the largest joltage possible, 98, by turning on the first two batteries.
                In 811111111111119, you can make the largest joltage possible by turning on the batteries labeled 8 and 9, producing 89 jolts.
                In 234234234234278, you can make 78 by turning on the last two batteries (marked 7 and 8).
                In 818181911112111, the largest joltage you can produce is 92.
            The total output joltage is the sum of the maximum joltage from each bank, so in this example, 
            the total output joltage is 98 + 89 + 78 + 92 = 357.
            There are many batteries in front of you. Find the maximum joltage possible from each bank; 
            what is the total output joltage?
            */

            int totalJoltage = 0;
            foreach (string batteryBank in BatteriesBanks)
            {
                int firstMax = 0;
                int secondMax = 0;

                foreach (char battery in batteryBank)
                {
                    int batVal = int.Parse(battery.ToString());
                    if (firstMax == 0)
                    {
                        firstMax = batVal;
                        continue;
                    }
                    if (secondMax == 0)
                    {
                        secondMax = batVal;
                        continue;
                    }

                    if (secondMax > firstMax)
                    {
                        firstMax = secondMax;
                        secondMax = batVal;
                        continue;

                    }
                    if (batVal > secondMax)
                    {
                        if (secondMax > firstMax) firstMax = secondMax;
                            secondMax = batVal;
                    }

                    
                }

                totalJoltage += firstMax * 10 + secondMax;
            }
            Console.WriteLine($"{DayNumOfAoC} - Part 1: {totalJoltage}");
        }

        public void Part2()
        {
            /*
            Now, you need to make the largest joltage by turning on exactly twelve batteries within each bank.
            The joltage output for the bank is still the number formed by the digits of the batteries you've turned on; the only difference is that now there will be 12 digits in each bank's joltage output instead of two.
            Consider again the example from before:
                987654321111111
                811111111111119
                234234234234278
                818181911112111
            Now, the joltages are much larger:
                In 987654321111111, the largest joltage can be found by turning on everything except some 1s at the end to produce 987654321111.
                In the digit sequence 811111111111119, the largest joltage can be found by turning on everything except some 1s, producing 811111111119.
                In 234234234234278, the largest joltage can be found by turning on everything except a 2 battery, a 3 battery, and another 2 battery near the start to produce 434234234278.
                In 818181911112111, the joltage 888911112111 is produced by turning on everything except some 1s near the front.
            The total output joltage is now much larger: 987654321111 + 811111111119 + 434234234278 + 888911112111 = 3121910778619.
            What is the new total output joltage?
            */

            const int NumOfBatteries = 12;
            double totalJoltage = 0;

            foreach (string batteryBank in BatteriesBanks)
            {

                // the container for the max jolt value of the battery bank
                List<int> maxValues = new(NumOfBatteries);
                
                // go through each battery in the battery bank
                foreach (char battery in batteryBank)
                {
                    int batVal = int.Parse(battery.ToString());

                    // If the container of the max jolt value contains empty slots, 
                    // add the jolt value
                    // then go to the next battery
                    if (maxValues.Count != maxValues.Capacity)
                    {
                        maxValues.Add(batVal);
                        continue;
                    }

                    // Here, the container is full. 
                    // Find the first value in the container which is less than the next value.
                    // if it is found, shift overwrite its value with the next's value
                    // and then overwrite all the next other's 
                    
                    bool foundLowerValue = false;

                    for (int pos = 0; pos < NumOfBatteries - 1; pos++)
                    {
                        if (!foundLowerValue)
                        {
                            if (maxValues[pos] < maxValues[pos + 1]) 
                            {
                                foundLowerValue = true;
                            }
                        }

                        if (foundLowerValue) 
                        {
                            maxValues[pos] = maxValues[pos + 1];
                        }
                    }

                    // if there was a shift, then the next value becomes the last digit
                    if (foundLowerValue) 
                    {
                        maxValues[^1] = batVal;
                    }

                    // if there was no shift, then only replace the last digit if the next value the next value higher
                    else
                    {
                        if (maxValues[^1] < batVal)
                        {
                            maxValues[^1] = batVal;
                        }
                    }
                }

                // once all is processed, form the actual jolt value
                double thisMax = 0;

                for (int pos = 0; pos < NumOfBatteries; pos++)
                {
                    thisMax += maxValues[pos] * Math.Pow(10, NumOfBatteries - 1 - pos);
                }

                // and add the value to the totaljoltage
                totalJoltage += thisMax;

            }
            Console.WriteLine($"{DayNumOfAoC} - Part 2: {totalJoltage}");
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