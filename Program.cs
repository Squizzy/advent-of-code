using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO.Enumeration;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using icecream;


// using icecream;
using static icecream.IceCream;


namespace AoC2024
{
    class Day1
    {
        private static readonly List<int> _data1 = [];
        private static readonly List<int> _data2 = [];

        public Day1()
        {
            try
            {
                string? line;
                // StreamReader sr = new("2024\\01_path_sample.txt");
                StreamReader sr = new("2024\\01_path.txt");
                line = sr.ReadLine();
                while (line != null)
                {
                    // _data.Add(line);
                    string[] linedata = line.Trim().Split("   ");
                    _data1.Add(int.Parse(linedata[0]));
                    _data2.Add(int.Parse(linedata[1]));

                    line = sr.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Problem loading the data for 2024-day01");
            }
        }

        public void displayDay1Data()
        {
            Console.WriteLine("Printing data1, data2");
            for (int i = 0; i < _data1.Count(); i++)
            {
                Console.WriteLine($"{_data1[i]}\t{_data2[i]}");
            }
        }

        public void Part1()
        {
            // Pair up the smallest number in the left list with the smallest number in the right list, 
            // then the second-smallest left number with the second-smallest right number, and so on.

            // Within each pair, figure out how far apart the two numbers are;
            //  you'll need to add up all of those distances. 
            // For example, if you pair up a 3 from the left list with a 7 from the right list, 
            // the distance apart is 4; if you pair up a 9 with a 3, the distance apart is 6.

            List<int> list1_sorted = [.. _data1];
            List<int> list2_sorted = [.. _data2];

            list1_sorted.Sort();
            list2_sorted.Sort();

            int dist = 0;

            for (int i = 0; i < list1_sorted.Count(); i++)
            {
                dist += Math.Abs(list2_sorted[i] - list1_sorted[i]);
            }

            Console.WriteLine($"2024 - Day 01 Part 1: {dist}");
        }

        public void Part2()
        {
            // Calculate a total similarity score by adding up each number in the left list 
            // after multiplying it by the number of times that number appears in the right list.

            int distance_error = 0;

            foreach (int location in _data1)
            {
                int duplicate_count = _data2.Where(x => x.Equals(location)).Count();
                distance_error += location * duplicate_count;
            }

            Console.WriteLine($"2024 - Day 01 Part 2: {distance_error}");
        }
    }

    class Day2
    {
        private static readonly List<int[]> reports_list = [];

        public Day2()
        {
            try
            {
                string? line;
                // StreamReader sr = new("2024\\02_reports_sample.txt");
                StreamReader sr = new("2024\\02_reports.txt");
                line = sr.ReadLine();
                while (line != null)
                {

                    int[] lineData = [.. line.Trim().Split(" ").Select(n => Convert.ToInt32(n))];
                    reports_list.Add(lineData);

                    line = sr.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Problem loading the data for 2024-day02");
            }
        }

        public void DisplayDay2Data()
        {
            foreach (var data in reports_list)
            {
                string values = "";
                foreach (var value in data)
                {
                    values += value + " ";
                }
                Console.WriteLine(values);
            }
        }

        private static bool _is_safe(int[] report)
        {
            bool increase = (report[1] - report[0]) > 0;

            for (int i = 0; i < report.Count() - 1; i++)
            {
                if (report[i + 1] == report[i]) return false;

                if ((report[i + 1] - report[i]) > 0 != increase) return false;

                if (Math.Abs(report[i + 1] - report[i]) > 3) return false;
            }

            return true;
        }

        public void Part1()
        {
            // a report only counts as safe if both of the following are true:

            // The levels are either all increasing or all decreasing.
            // Any two adjacent levels differ by at least one and at most three.

            int safe_count = 0;

            foreach (int[] report in reports_list)
            {
                if (_is_safe(report))
                    safe_count += 1;
            }

            Console.WriteLine($"2024 - Day 02 Part 1: {safe_count}");
        }

        private static bool _is_safe_with_one_less_level(int[] report)
        {
            for (int i = 0; i < report.Count(); i++)
            {
                List<int> reduced_report = [.. report];
                reduced_report.RemoveAt(i);

                if (_is_safe(reduced_report.ToArray()))
                    return true;
            }
            return false;
        }

        public void Part2()
        {
            // Now, the same rules apply as before, 
            // except if removing a single level from an unsafe report would make it safe, 
            // the report instead counts as safe.

            int safe_count = 0;
            int report_num = 0;

            foreach (int[] report in reports_list)
            {
                if (_is_safe(report))
                    safe_count += 1;
                else
                {
                    if (_is_safe_with_one_less_level(report))
                        safe_count += 1;
                }
                report_num++;
            }

            Console.WriteLine($"2024 - Day 02 Part 2: {safe_count}");

        }
    }

    class Day3
    {
        private static readonly List<string> _corrupted_data = [];
        public Day3()
        {
            try
            {
                string? line;
                // StreamReader sr = new("2024\\03_corrupted_mem_sample.txt");
                StreamReader sr = new("2024\\03_corrupted_mem.txt");
                line = sr.ReadLine();
                while (line != null)
                {
                    _corrupted_data.Add(line);

                    line = sr.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Problem loading the data for 2024-day03");
            }

        }

        public void DisplayDay3Data()
        {
            _corrupted_data.ic();
        }

        private static string[] _extract_uncorrupted_muls(string line)
        {
            List<string> identified_strings = [];

            Regex regex_mul = new Regex(@"mul\(\d{1,3},\d{1,3}\)");

            var c = regex_mul.Matches(line);

            foreach (Match match in c)
            {
                string to_store = $"{match.Index}".PadLeft(5, '0') + "|" + match.Value;
                identified_strings.Add(to_store);
            }

            return identified_strings.ToArray();
        }

        private static int _get_mul_result(string mul)
        {
            // extract the numbers
            int[] nums = mul[10..(mul.Length - 1)].Split(',').Select(x => int.Parse(x)).ToArray();

            return nums[0] * nums[1];
        }

        public void Part1()
        {
            int mul_total = 0;
            List<string> mul_strings = [];

            foreach (string line in _corrupted_data)
            {
                mul_strings.AddRange(_extract_uncorrupted_muls(line));
            }

            foreach (string mul in mul_strings)
            {
                mul_total += _get_mul_result(mul);
            }

            Console.WriteLine($"2024 - Day 03 Part 1: {mul_total}");
        }

        private static List<string> _extract_do_dont(string line)
        {
            List<string> do_dont = [];

            Regex regex_do = new(@"do\(\)");
            var do_matches = regex_do.Matches(line);
            foreach (Match match in do_matches)
            {
                do_dont.Add($"{match.Index}".PadLeft(5, '0') + "|" + match.Value);
            }

            Regex regex_dont = new(@"don\'t\(\)");
            var dont_matches = regex_dont.Matches(line);
            foreach (Match match in dont_matches)
            {
                do_dont.Add($"{match.Index}".PadLeft(5, '0') + "|" + match.Value);
            }

            return do_dont;
        }

        private static (List<string>, bool) _process_dos_donts(List<string> mul_string, bool do_dont)
        {
            List<string> processed = [];
            bool do_mul = do_dont;

            mul_string.Sort();

            foreach (string line in mul_string)
            {
                if (line[6..].ToString() == "do()")
                {
                    do_mul = true;
                    continue;
                }
                else if (line[6..].ToString() == "don't()")
                {
                    do_mul = false;
                    continue;
                }
                else
                {
                    if (do_mul) processed.Add(line);
                }
            }

            return (processed, do_mul);
        }

        public void Part2()
        {
            int mul_total = 0;
            List<string> mul_strings = [];
            bool do_dont = true;

            foreach (string line in _corrupted_data)
            {
                List<string> mul_string = [];
                mul_string.AddRange(_extract_uncorrupted_muls(line));
                mul_string.AddRange(_extract_do_dont(line));

                (List<string> a, do_dont) = _process_dos_donts(mul_string, do_dont);
                mul_strings.AddRange(a);
            }

            foreach (string mul in mul_strings)
            {
                mul_total += _get_mul_result(mul);
            }

            Console.WriteLine($"2024 - Day 03 Part 2: {mul_total}");
        }
    }

    class Day4
    {
        private static readonly List<string> _xmas_puzzle = [];

        public Day4()
        {
            try
            {
                string? line;
                // StreamReader sr = new("2024\\crossword_sample.txt");
                StreamReader sr = new("2024\\crossword.txt");
                line = sr.ReadLine();
                while (line != null)
                {
                    _xmas_puzzle.Add(line);

                    line = sr.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Problem loading the data for 2024-day04");
            }
        }

        public void DisplayDay4Data()
        {
            _xmas_puzzle.ic();
        }

        private static int _find_xmas_count_from_x(int row, int col)
        {
            int xmas_count = 0;

            List<(int, int)> directions = [(-1, -1), (-1, 0), (-1, 1),
                                           ( 0, -1),          ( 0, 1),
                                           ( 1, -1), ( 1, 0), ( 1, 1)];

            foreach ((int, int) direction in directions)
            {
                if (((row < 3) && (direction.Item1 == -1)) ||
                    ((row > _xmas_puzzle.Count() - 4) && (direction.Item1 == 1)) ||
                    ((col < 3) && (direction.Item2 == -1)) ||
                    ((col > _xmas_puzzle[0].Length - 4) && (direction.Item2 == 1)))

                    continue;

                if ((_xmas_puzzle[row + 1 * direction.Item1][col + 1 * direction.Item2] == 'M') &&
                    (_xmas_puzzle[row + 2 * direction.Item1][col + 2 * direction.Item2] == 'A') &&
                    (_xmas_puzzle[row + 3 * direction.Item1][col + 3 * direction.Item2] == 'S'))
                    xmas_count += 1;
            }

            return xmas_count;
        }

        public void Part1()
        {
            // This word search allows words to be horizontal, vertical, diagonal, written backwards, or even overlapping other words. 
            // It's a little unusual, though, as you don't merely need to find one instance of XMAS - you need to find all of them.
            int xmas_count = 0;

            // find each X location
            for (int row = 0; row < _xmas_puzzle.Count(); row++)
            {
                for (int col = 0; col < _xmas_puzzle[0].Length; col++)
                {
                    if (_xmas_puzzle[row][col] == 'X')
                    {
                        // check how many xmas can be found around it
                        xmas_count += _find_xmas_count_from_x(row, col);
                    }
                }
            }

            Console.WriteLine($"2024 - Day 04 Part 1: {xmas_count}");
        }

        private static int _find_x_mas_count_from_x(int row, int col)
        {
            // stip if letters would fall outside of the table
            if ((row < 1) || (row > _xmas_puzzle.Count() - 2) ||
                (col < 1) || (col > _xmas_puzzle[0].Length - 2))
            {
                return 0;
            }

            // skip if not forming the word MAS in one diagonal, either direction
            if (!((_xmas_puzzle[row + 1][col + 1] == 'M') && (_xmas_puzzle[row - 1][col - 1] == 'S') ||
                  (_xmas_puzzle[row - 1][col - 1] == 'M') && (_xmas_puzzle[row + 1][col + 1] == 'S')))
            {
                return 0;
            }


            // skip if not forming the word MAS in the other diagonal, either direction
            if (!((_xmas_puzzle[row + 1][col - 1] == 'M') && (_xmas_puzzle[row - 1][col + 1] == 'S') ||
                    (_xmas_puzzle[row - 1][col + 1] == 'M') && (_xmas_puzzle[row + 1][col - 1] == 'S')))
            {
                return 0;
            }

            // If we reach here we have an X-MAS
            return 1;
        }

        public void Part2()
        {
            // it's an X-MAS puzzle in which you're supposed to find two MAS in the shape of an X
            //Within the X, each MAS can be written forwards or backwards.
            int x_mas_count = 0;

            // find each A location
            for (int row = 0; row < _xmas_puzzle.Count(); row++)
            {
                for (int col = 0; col < _xmas_puzzle[0].Length; col++)
                {
                    if (_xmas_puzzle[row][col] == 'A')
                    {
                        // check how many xmas can be found around it
                        x_mas_count += _find_x_mas_count_from_x(row, col);
                    }
                }
            }

            Console.WriteLine($"2024 - Day 04 Part 2: {x_mas_count}");
        }
    }

    class Day5
    {
        // private static readonly List<string> _manual_pages = [];
        private static readonly List<List<int>> _rules = [];
        private static readonly List<List<int>> _manuals_pages = [];

        private readonly List<int> _correct_manuals = [];
        private readonly List<int> _incorrect_manuals = [];

        public Day5()
        {
            List<int> page1_rule_list = [];
            List<int> page2_rule_list = [];

            try
            {
                string? line;
                // string sample = "_sample";
                string sample = "";
                string filename = Path.Join("2024", $"05_manual_pages{sample}.txt");
                StreamReader sr = new(filename);
                line = sr.ReadLine();

                while (line != null)
                {
                    if (line == "\n")
                        continue;

                    else if (line.Contains('|'))
                    {
                        // A rule is found, first list of _rules is the page that comes first, second list of _rules is the page that must come after
                        List<int> rule = line.Trim().Split('|').ToList().ConvertAll(int.Parse);
                        page1_rule_list.Add(rule[0]);
                        page2_rule_list.Add(rule[1]);
                    }

                    else if (line.Contains(','))
                    {
                        _manuals_pages.Add(line.Trim().Split(',').ToList().ConvertAll(int.Parse));
                    }

                    line = sr.ReadLine();
                }

                _rules.Add(page1_rule_list);
                _rules.Add(page2_rule_list);
            }
            catch
            {
                Console.WriteLine("Problem loading the data for 2024-day05");
            }
        }

        public void DisplayDay5Data()
        {
            // _manual_pages.ic();
            _rules.ic();
            _manuals_pages.ic();
        }

        public void Part1()
        {
            // For each manuals list
            // take one list at a time
            // for each list, take a page 
            // if this page is in the rules for the first pages, note its index and that of the rule
            // then for this rule, check if the rule for the second page appears in this manual list.
            // if it does, note its index.
            // if this index of the page for the second rule is < than the index for the page of the first rule, this manual is not in the right order
            // if no rule is incorrect, record the middle number in _correct_manuals

            int first_index, second_index;

            foreach (List<int> manual_pages in _manuals_pages)
            {
                bool pages_are_in_correct_order = true;
                foreach (int page in manual_pages)
                {
                    if (!pages_are_in_correct_order) break;

                    for (int rule_index = 0; rule_index < _rules[0].Count && pages_are_in_correct_order; rule_index++)
                    {

                        if (_rules[0][rule_index] == page)
                        {
                            if (manual_pages.Contains(_rules[1][rule_index]))
                            {
                                first_index = manual_pages.IndexOf(page);
                                second_index = manual_pages.IndexOf(_rules[1][rule_index]);

                                if (second_index < first_index) pages_are_in_correct_order = false;
                            }

                        }
                    }
                }

                if (pages_are_in_correct_order)
                {
                    _correct_manuals.Add(manual_pages[manual_pages.Count / 2]);
                }
            }

            int total_pagenums = 0;
            foreach (int page_num in _correct_manuals) total_pagenums += page_num;

            Console.WriteLine($"2024 - Day 05 Part 1: {total_pagenums}");
        }

        public void Part2()
        {

        }

    }


    class AoC2024
    {
        public static void Main()
        {
            // Day1 day1 = new();
            // // day1.displayDay1Data();
            // day1.Part1();
            // day1.Part2();

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

            Day5 day5 = new();
            // day5.DisplayDay5Data(); 
            day5.Part1();
            day5.Part2();

        }
    }
}