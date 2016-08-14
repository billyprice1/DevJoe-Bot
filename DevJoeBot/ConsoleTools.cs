using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJoeBot
{
    class ConsoleTools
    {

        private static string currentinput = "";

        public void commandInput()
        {
            List<string> accept = new List<string>();
            string[] pre = new string[]{ "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            accept.AddRange(pre);
            Console.Write("COMMAND> ");
            while (true)
            {
                while(true)
                {
                    ConsoleKeyInfo i = Console.ReadKey();
                    if(accept.Contains((i.KeyChar+"").ToLower()))
                    {
                        currentinput += "" + i.KeyChar;
                    } else if(i.Key == ConsoleKey.Backspace)
                    {
                        currentinput = currentinput.Substring(0, currentinput.ToCharArray().Length - 1);
                    } else if(i.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    int y = Console.CursorTop;
                    Console.CursorLeft = 0;
                    Console.Write("                                                                                                                                                                                                                                                                          ");
                    Console.CursorLeft = 0;
                    Console.CursorTop = y;
                    Console.Write("COMMAND> "+currentinput);
                }
                Log("Console command '" + currentinput + "' ran..");
                command(currentinput);
                int yy = Console.CursorTop;
                Console.CursorLeft = 0;
                Console.Write("                                                                                                                                                                                                                                                                          ");
                Console.CursorLeft = 0;
                Console.CursorTop = yy;
                Console.Write("COMMAND> ");
            }
        }

        public static void Log(string r)
        {
            int y = Console.CursorTop;
            Console.CursorLeft = 0;
            Console.Write("                                                                                                                                                                                                                                                                          ");
            Console.CursorLeft = 0;
            Console.CursorTop = y;
            Console.WriteLine(r);
            Console.Write("COMMAND> "+currentinput);
        }

        public void command(string s)
        {
            currentinput = "";
        }
    }
}
