using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ATM
{
    class Program
    {
        static void printChangeWays(int[] nominals, int sum,int[] currentCombination, int level)
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
                }
            } else
            {
                int maxVal = sum / nominals[level];
                for(int i= maxVal; i >= 0; i--)
                {
                    currentCombination[level] = i;
                    printChangeWays(nominals, sum - currentCombination[level] * nominals[level], currentCombination, level - 1);
                }
            }
        }            
        static void Main(string[] args)
        {
            Console.Write("Enter sum: ");
            int sum = Int32.Parse(Console.ReadLine());
        
            Console.WriteLine("Enter nominals: ");            
            SortedSet<string> nominals = new SortedSet<string>();
            string input = "";
            
            while (input != "0")
            {   
                input = Console.ReadLine();
                if(input != "0")
                {
                    nominals.Add(input);
                }
            }
             int[] intNominals = new int[nominals.Count];
            int i = 0;
            foreach (var elem in nominals)
            {
                intNominals[i] = Int32.Parse(elem);
                i++;
            }
            int[] currentCombination = new int[intNominals.Length];
                   
            printChangeWays(intNominals, sum, currentCombination, intNominals.Length - 1);
            Console.ReadLine();
        }
    }
}
