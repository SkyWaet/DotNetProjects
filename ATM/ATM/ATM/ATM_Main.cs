using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ATM
{
    class ATM
    {
        static int PrintChangeWays(int[] nominals, int sum,int[] currentCombination, int level)
        {
            if(level == 0)
            {
                if(sum % nominals[0] == 0)
                {
                    Console.Write("New Combination: ");
                    currentCombination[0] = sum / nominals[0];
                    for(int i=0; i < nominals.Length; i++)
                    {
                        if (currentCombination[i] != 0)
                        {
                            Console.Write($"{currentCombination[i]} of {nominals[i]}$; ");
                        }
                    }
                    Console.WriteLine();
                    return 1;                    
                }
                return 0;
            } else
            {
                int maxVal = sum / nominals[level];
                int ways = 0;
                for(int i= maxVal; i >= 0; i--)
                {                 
                    currentCombination[level] = i;
                    ways += PrintChangeWays(nominals, sum - currentCombination[level] * nominals[level], currentCombination, level - 1);
                }
                return ways;
            }
        }       

        static int GetSum()
        {
            Console.Write("Enter sum: ");
            int sum = 0;
            string rawSum = "";
            try
            {
                rawSum = Console.ReadLine();
                sum = Int32.Parse(rawSum);
            }
            catch
            {
                Console.WriteLine($"String {rawSum} has frong format. Try again");
                return GetSum();
            }
            return sum;
        }

        static int[] GetNominals()
        {
            Console.WriteLine("Enter nominals: ");
            SortedSet<string> nominals = new SortedSet<string>();
            string input = "";

            while (input != "0")
            {
                input = Console.ReadLine();
                if (input != "0")
                {
                    nominals.Add(input);
                }
            }

            List<int> intNominals = new List<int>();
            int i = 0;
            foreach (var elem in nominals)
            {
                try
                {
                    int value = Int32.Parse(elem);
                    if(value <= 0)
                    {
                        Console.WriteLine($"Nominal {elem} is less or equal zero. It will not be included");
                    }
                    else
                    {
                        intNominals.Add(value);
                    }                    
                    
                }
                catch
                {
                    Console.WriteLine($"Nominal {elem} has wrong format. It will not be included");
                }
               
            }
            return intNominals.ToArray();
        }

        static void Main(string[] args)
        {
            int sum = GetSum();
            int[] nominals = GetNominals();
            
            int[] currentCombination = new int[nominals.Length];
            if (nominals.Length == 0)
            {
                Console.WriteLine("Nominals array is empty");
            }
            else
            {              
                int result = PrintChangeWays(nominals, sum, currentCombination, nominals.Length - 1);
                if(result > 0)
                {
                    Console.WriteLine($"In total, there are {result} ways to exchange the given sum");
                }
                else
                {
                    Console.WriteLine("There are no ways to exchange the given sum by given nominals");
                }
            }
            Console.ReadLine();
        }
    }
}
