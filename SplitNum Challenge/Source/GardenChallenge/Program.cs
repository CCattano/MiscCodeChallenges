using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace SplitNumChallenge
{
    class Program
    {
        static async Task Main(String[] args)
        {
            //Need to find more clever solution.
            //Test sets should finish in 10s or less
            Int32 testSet1DigitLimit = 6;
            Int32 testSet2DigitLimit = 10;
            Int32 testSet3DigitLimit = 100;
            Int32 maxPossibleDigitLimit = 19000;
            Dictionary<String, BigInteger> tests = new Dictionary<String, BigInteger>();
            Enumerable.Range(0, 100).ToList().ForEach(v =>
            {
                BigInteger randomNum;
                do
                {
                    randomNum = GetRandomNum(testSet3DigitLimit);
                } while (!randomNum.ToString().Contains("4") && !tests.Values.Any(val => val == randomNum));
                tests.Add($"Case #{v + 1}", randomNum);
            });
            Stopwatch timer = new Stopwatch();
            timer.Start();
            await Task.WhenAll(tests.AsParallel().AsUnordered().Select(test => SolveProblem(test.Key, test.Value)));
            timer.Stop();
            Console.WriteLine($"All test cases solved in {timer.ElapsedMilliseconds} ms");
        }

        private static async Task SolveProblem(String testCase, BigInteger upperLimit)
        {
            //Stopwatch timer = new Stopwatch(); //DIAGNOSTIC
            //timer.Start(); //DIAGNOSTIC
            Result result = await GetCombinations(upperLimit);
            //timer.Stop(); //DIAGNOSTIC
            //Console.WriteLine($"Solved in {timer.ElapsedMilliseconds} ms - {testCase}: {upperLimit} - {result.CheckOne}, {result.CheckTwo} - Iterations: {result.IterationCount}"); //DIAGNOSTIC
            Console.WriteLine($"{testCase}:\n\t{result.CheckOne}\n\t{result.CheckTwo}");
        }

        private static async Task<Result> GetCombinations(BigInteger numberWith4)
        {
            return await Task.Run(() =>
            {
                Result result = new Result();
                List<Char> digits = numberWith4.ToString().ToCharArray().ToList();
                BigInteger subtractBy = 1;
                Int32 firstNumFourIndex = digits.IndexOf('4');
                digits[firstNumFourIndex] = '3';
                for (Int32 i = firstNumFourIndex + 1; i < digits.Count; i++)
                {
                    digits[i] = '9';
                }
                subtractBy = numberWith4 - BigInteger.Parse(String.Concat(digits));
                BigInteger diff;
                while (result.CheckOne == 0 && result.CheckTwo == 0)
                {
                    //++result.IterationCount; //DIAGNOSTIC
                    diff = numberWith4 - subtractBy;
                    if (!$"{subtractBy}".Contains("4") && !$"{diff}".Contains("4"))
                    {
                        result.CheckOne = subtractBy;
                        result.CheckTwo = diff;
                    }
                    else
                    {
                        ++subtractBy;
                        String subtractByVal = subtractBy.ToString();
                        if (subtractByVal.Contains("4"))
                        {
                            subtractBy = BigInteger.Parse(subtractByVal.Replace("4", "5"));
                        }
                    }
                }
                return result;
            });
        }

        private static BigInteger GetRandomNum(int length)
        {
            Random rng = new Random();
            IEnumerable<Int32> digits = Enumerable.Range(0, length).Select(v => rng.Next(1, 10));
            return BigInteger.Parse(String.Concat(digits));
        }
    }

    public class Result
    {
        public BigInteger CheckOne { get; set; }
        public BigInteger CheckTwo { get; set; }
        //public Int32 IterationCount { get; set; } = 0; //DIAGNOSTIC
    }
}
