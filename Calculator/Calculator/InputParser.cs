using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Calculator
{
     class InputParser
    {
        private Regex number = new Regex(@"[-+]?[0-9ABC]*\,?[0-9ABC]+");
        private Regex operation = new Regex(@"[+\-*/]");
        private Regex exceptions = new Regex(@"-\(-[0-9ABC]+\)");
        private Regex innerNumber = new Regex(@"[0-9ABC]+");

        private static Dictionary<char, int> digits = new Dictionary<char, int>()
        {
            { 'A', 10 },
            { 'B', 11 },
            { 'C', 12 },
            { 'D', 13 }
        };

        public bool isOperation(char input)
        {
            return operation.IsMatch(""+input);
        }

        private int GetIntValue(char letter)
        {
            
            if (letter == 'A')
            {
                return 10;
            }
            else if (letter == 'B')
            {
                return 11;
            }
            else if (letter == 'C')
            {
                return 12;
            }
            else
            {
                Console.WriteLine("Letter = " + letter);
                return int.Parse(""+letter);
            }
        }

        private string GetLetter(int digit)
        {
            if (digit < 10)
            {
                return digit.ToString();
            }
            else if (digit == 10)
            {
                return "A";
            }
            else if (digit == 11)
            {
                return "B";
            }
            else 
            {
                return "C";
            }
        }

        private int GetPriority(char operation)
        {
            return (operation == '+' || operation == '-') ? 0 : 1;
        }

        private double ToDecimal(string str)
        {
            string[] number = new string[2];
            if (str.Contains(','))
            {
                number = str.Split(',');
            }
            else
            {
                number[0] = str;
                number[1] = "0";
            }
            int sign;
            if (number[0].Contains('-'))
            {
                sign = -1;
                number[0] = number[0].Replace("-", "");
            }
            else
            {
                sign = 1;
            }
            double decimalNum = 0;
            int c = number[0].Length - 1;

            int tmp = 0;
            for (int i = 0; i < number[0].Length; ++i)
            {
                if (!(int.TryParse(number[0][i].ToString(), out tmp)))
                {
                    tmp = digits[number[0][i]];
                }
                decimalNum += tmp * Math.Pow(13, c);
                c--;
            }
            if (number[1].Length != 0 && number[1] != "0")
            {
                for (int i = 1; i <= number[1].Length; i++)
                {
                    if (!(int.TryParse(number[1][i - 1].ToString(), out tmp)))
                    {
                        tmp = digits[number[1][i - 1]];
                    }
                    decimalNum += tmp * Math.Pow(13, -i);
                }
            }
            return (sign == -1) ? -decimalNum : decimalNum;          
        }

        private string FromDecimal(double num)
        {
            int sign = (num < 0) ? -1 : 1;
            num = Math.Abs(num);
            int zel = (int)Math.Truncate(num);
            double FracVal = num - zel;
            string StrInt = "";
            do
            {
                StrInt = GetLetter(zel % 13) + StrInt;
                zel = zel / 13;
            } while (zel != 0);

            if (FracVal != 0)
            {
                string FracPart = "";
                int tmp;
                while (FracVal > 0 && FracPart.Length <= 5)
                {
                    FracVal = FracVal * 13;
                    tmp = (int)Math.Truncate(FracVal);
                    FracPart = FracPart + GetLetter(tmp);
                    FracVal = FracVal - tmp;
                }
                StrInt = StrInt + "," + Convert.ToString(FracPart);
            }
            return (sign == -1) ? "-" + StrInt : StrInt;            
        }


        private Queue<string> ParseInput(String input)
        {
            Queue<string> answer = new Queue<string>();
            StringBuilder buffer = new StringBuilder();
            int len = input.Length;
            int i = 0;
            int numDots = 0;
            while (i < len)
            {
                char elem = input[i];
                if (number.IsMatch(""+elem))
                {
                    char curr = elem;                  
                    while ((number.IsMatch(""+curr) || curr == ','))
                    {
                        if (curr == ',')
                        {
                            numDots++;
                            /*
                              there should be not more than one dot in the number
                              also two dots could not stand nearby
                             */
                            if (numDots > 1)
                            {
                              throw new ArgumentException("Too many dots in one number = " + numDots);
                            }
                        }
                        buffer.Append(curr);
                        i++;
                        if (i >= len)
                        {
                            break;
                        }
                        curr =input[i];
                    }
                    
                    if (buffer[buffer.Length - 1] == ',')
                    {
                        buffer.Append('0');
                    }
                    answer.Enqueue(buffer.ToString());
                    buffer = new StringBuilder();
                    numDots = 0;
                }
                else
                {
                    answer.Enqueue("" + elem);
                    i++;
                }
            }
            return answer;        
        }

        private Queue<string> PolishNotation(Queue<string> parsedString)
        {
            Queue<string> output = new Queue<string>();
            Stack<char> operators = new Stack<char>();
            while (!(parsedString.Count == 0))
            {
                String elem = parsedString.Dequeue();
                /*
                  numbers should be put to the queue
                 */
                if (number.IsMatch(elem))
                {
                    output.Enqueue(elem);
                }
                else if (operation.IsMatch(elem))
                {
                    /*
                      all operators from operations stack with higher priority
                      than current token should be put to the output queue
                      */
                    while (operators.Count>0 && operation.IsMatch(""+operators.Peek()) && GetPriority(operators.Peek()) >= GetPriority(elem[0]))
                    {
                        output.Enqueue("" + operators.Pop());
                    }
                    /*
                      current token should be pushed to the stack
                      */
                    operators.Push(elem[0]);
                }
                else if (elem[0] == '(')
                {
                    /*
                      left bracket should simply be pushed to the stack
                     */
                    operators.Push(elem[0]);
                }
                else if (elem[0] == ')')
                {
                    /*
                      all operators from the stack should be put
                      to the output queue while '(' is not on the top
                      of the stack
                     */
                    while (operators.Count>0 && operators.Peek() != '(')
                    {
                        output.Enqueue("" + operators.Pop());
                    }
                    /*
                      if operators stack is empty, the '(' was not found
                     */
                    if (operators.Count == 0)
                    {
                       
                        throw new ArgumentException("Unclosed )");
                    }
                    operators.Pop();
                }
                else
                {
                    /*
                      Current element is one of restricted symbols
                     */             
                    throw new ArgumentException("Restricted Symbol: " + elem);
                }
            }
            /*
             * all operators from the stack should be put to the  output queue
             */
            while (operators.Count > 0)
            {
                /*
                 * if '(' is on the top of the stack, there were not corresponding
                 * ')' in the initial string
                 */
                if (operators.Peek() == '(')
                {                  
                    throw new ArgumentException("Unclosed (");
                }
                output.Enqueue("" + operators.Pop());
            }
            return output;
        }       

        private string Calculate(Queue<string> polishNot)
        {
            Stack<string> container = new Stack<string>();
            /*
             * the stack is used to perform calculations
             */
            while (polishNot.Count != 0)
            {
                string elem = polishNot.Dequeue();

                if (number.IsMatch(elem))
                {
                    /*
                     * numbers are pushed directly to the stack
                     */
                    container.Push(elem);
                }              
                else
                {
                    /*
                     * two last elements are popped from the stack in order
                     * to perform operation. The result of the operation is
                     * pushed to the stack
                     */
                    if ((elem == "+" || elem == "-") && container.Count <= 1)
                    {

                        string lastNumber = container.Pop();
                        if(elem == "-" && lastNumber[0] == '-')
                        {
                            container.Push(lastNumber.Substring(1));
                        }
                        else if(elem =="-" && lastNumber[0] != '-')
                        {
                            container.Push("-"+lastNumber);
                        }
                        else
                        {
                            container.Push(lastNumber);
                        }
                        //Console.WriteLine("Container top "+container.Peek());
                    }
                    else
                    {
                        double second = ToDecimal(container.Pop());
                        double first = ToDecimal(container.Pop());
                        //Console.WriteLine("First = " + first);
                       // Console.WriteLine("Second = " + second);
                        double result;
                        switch (elem)
                        {
                            case "+":
                                result = (second + first);
                                container.Push(result.ToString());
                                break;
                            case "-":
                                result = first - second;
                                container.Push(result.ToString());
                                break;
                            case "*":
                                result = first * second;
                                container.Push(result.ToString());
                                break;
                            case "/":
                                if (second == 0)
                                {
                                    throw new ArithmeticException("Division by zero");
                                }
                                result = first / second;
                                container.Push(result.ToString());
                                break;
                        }
                    }
                }
            }
            /*
             * The result is on top of the stack.
             * If result could be casted to integer (=*.0), it should
             * be rounded. Else the original value should be returned
             */
            Console.WriteLine(container.Peek());
            double lastElem = Double.Parse(container.Pop());
            int ans = (int)Math.Round(lastElem);

            return lastElem == (double)ans ? FromDecimal(ans) : FromDecimal(lastElem);
        }

        private string DeleteExceptions(string input)
        {
            string ans = input;
            foreach(Match exception in exceptions.Matches(input))
            {
                Match number = innerNumber.Match(exception.ToString());
                ans = input.Replace(exception.ToString(), "+" + number);
            }
            return ans;
        }

        public string ParserMain(string input)
        {
            if (input == null || input.Length == 0)
            {
                return "";
            }

            /*
             * Parse initial string
             */
            string inputWithoutExceptions = DeleteExceptions(input);
            Queue<string> parsedString;
            try
            {
                parsedString = ParseInput(inputWithoutExceptions);
            } catch (ArgumentException ex)
            {
                return ex.Message;
            }
     
            /*
             * Find the postfix notation of the parsedString
             */
            Queue<string> polishForm;
            try
            {
                polishForm = PolishNotation(parsedString);
            }catch (ArgumentException ex)
            {
                return ex.Message;
            }        

            /*
             * Use polishForm to calculate value
             */
            try
            {
              return Calculate(polishForm);
             
            } catch (Exception ex)
            {
                return ex.Message;
            }    


        } 
    }
}
