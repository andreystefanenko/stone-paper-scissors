using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace task2
{
    class Program
    {

        static void Main(string[] args)
        {

            if (args.Length <= 1 || (args.Length & 1) == 0)
            {
                Console.WriteLine("Invalid parameters! The number of parameters must be odd and more or equal to three.\n" +
                    "Correct parameters for example: rock paper scissors");
                Console.ReadKey();
                return;
            }

            var repeatedValues = args.Where(str => args.Count(s => s == str) > 1).Distinct();
            int lengthOfRepVal = repeatedValues.Count();
            if (lengthOfRepVal != 0)
            {
                Console.WriteLine("Invalid parameters! Some parameters are repeated.\n" +
                    "Correct parameters for example: rock paper scissors lizard Spock");
                Console.ReadKey();
                return;
            }

            int userChoice = 0;
            bool flag = false;
            do {
                Console.Clear();
                
                byte[] randomKey = GenerateRandomKey();
                
                string[] parameters = Environment.GetCommandLineArgs();
                int computerChoice = MakeMove(args.Length - 1);
                string nameOfComputerChoice = parameters[computerChoice + 1];

                string HMACkey = TranslateHMACtoHex(randomKey, nameOfComputerChoice);
                Console.WriteLine("HMAC: " + HMACkey);
                
                printMenu(parameters);
                do
                {
                    if (!int.TryParse(Console.ReadLine(), out userChoice))
                    {
                        Console.WriteLine("Incorrect input! Please use only digits!");
                        flag = true;
                        printMenu(parameters);
                    }
                    else flag = false;

                    if (userChoice < 0 || userChoice > args.Length)
                    {
                        Console.WriteLine("Incorrect input! Out of range!");
                        flag = true;
                        printMenu(parameters);
                    }
                    
                } while (flag);
                Console.WriteLine($"Your move: {parameters[userChoice]}");
                Console.WriteLine($"Computer move: {nameOfComputerChoice}");

                FindWinner(userChoice - 1, computerChoice, args.Length);

                Console.WriteLine("HMAC key: " + HashEncode(randomKey));

                if (userChoice != 0) Console.ReadKey();
            } while (userChoice != 0);

        }
        static void printMenu(string[] parameters)
        {
            Console.WriteLine("Available moves: ");
            for (int i = 1; i<parameters.Length; i++)
            {
                Console.WriteLine($"{i} - {parameters[i]}");
            }
            Console.WriteLine("0 - exit");
            Console.Write("Enter your move: ");
           
        }
        static byte[] GenerateRandomKey()
        {
            byte[] bytes = new byte[16];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return bytes;
        }
        static int MakeMove(int max)
        {
            int min = 0;
            byte[] intBytes = new byte[4];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetNonZeroBytes(intBytes);
            }
            return min + Math.Abs(BitConverter.ToInt32(intBytes, 0)) % (max - min + 1);
           
        }
        static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }
        static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "");
        }

        private static byte[] GenerateHashHMAC(byte[] randomKey, byte[] message)
        {
            var hash = new HMACSHA256(randomKey);
            return hash.ComputeHash(message);
        }
        private static string TranslateHMACtoHex(byte[] key, string message)
        {
            byte[] hash = GenerateHashHMAC(key, StringEncode(message));
            return HashEncode(hash);
        }
        static void FindWinner(int userChoice, int computerChoice, int argsLength)
        {
            int result = (((userChoice - computerChoice) + argsLength) % argsLength);
            int check = (argsLength - 1) / 2;
            if (result == 0)
                Console.WriteLine("Draw!"); 
            else if (result <= check)
                    Console.WriteLine("Computer Win!"); 
                else Console.WriteLine("You Win!"); 
                  
        }

    }
}
