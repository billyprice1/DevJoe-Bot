using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DevJoeBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings.LoadSettings();
            string token = tokenReq();
            DiscordClient c = new DiscordClient();
            Settings.settings.firstrun = false;
            Settings.SaveSettings();
            Console.Clear();
            Console.WriteLine("###############################");
            Console.WriteLine("# SERVER CONSOLE");
            Console.WriteLine("# Here lies all the logging.");
            Console.WriteLine();
            Console.WriteLine("We're setting up for you..");

            // COMMANDS
            Command a = new Command(true, "help");
            a.onCommandRun += A_onCommandRun;

            c.ExecuteAndWait(async() => {
                await c.Connect(token);
                c.MessageReceived += C_MessageReceived;
                c.JoinedServer += C_JoinedServer;
                c.LeftServer += C_LeftServer;
                Console.WriteLine("READY!");
            });
        }

        private static void A_onCommandRun(object sender, string name, string[] args, User a, Channel c)
        {
            if(args.Length == 0)
            {
                c.SendMessage("```TEST```");
            }
        }

        private static void C_LeftServer(object sender, ServerEventArgs e)
        {
            
        }

        private static void C_JoinedServer(object sender, ServerEventArgs e)
        {
            Console.WriteLine("[ServerJoin] Joined server " + e.Server.Name + " with ID " + e.Server.Id);
        }

        private static void C_MessageReceived(object sender, MessageEventArgs e)
        {
            if(e.Message.Text.StartsWith(";"))
            {
                string m = e.Message.Text;
                object[] pack = genargs(m);
                string name = (string)pack[0];
                string[] args = (string[])pack[1];
                Command c = Command.getCommand(name);
                if(c != null)
                {
                    c.execute(args, e.User, e.Channel);
                }
            }
        }

        private static object[] genargs(string s)
        {
            string[] a = s.Split(' ');
            List<string> b = new List<string>();
            for(int i=1;i<a.Length;i++)
            {
                b.Add(a[i]);
            }
            return new object[] { a[0], b.ToArray() };
        }

        private static string tokenReq()
        {
            if (!Settings.settings.autosignin)
            {
                Console.WriteLine("############SIGN IN##############");
                Console.WriteLine("# This bot requires a token.");
                Console.WriteLine("# Please enter your token below.");
                Console.WriteLine();
                Console.Write("Bot token: ");
                string s = Console.ReadLine();
                Console.Write("\nWould you like to automatically sign in with this token? (y/n): ");
                ConsoleKeyInfo key = Console.ReadKey();
                if(key.Key == ConsoleKey.Y)
                {
                    Settings.settings.token = s;
                    Settings.settings.autosignin = true;
                    Settings.SaveSettings();
                }
                if (Settings.settings.owner == "")
                {
                    Console.Write("\nOwner user ID: ");
                    string owner = Console.ReadLine();
                    Settings.settings.token = owner;
                }
                if(Settings.settings.firstrun)
                {
                    Console.WriteLine("Additional setup can be done from your server by using !setup");
                    Console.Write("Press any key to start the bot for the first time....");
                    Console.ReadKey();
                }
                return s;
            } else
            {
                return Settings.settings.token;
            }
        }
    }
}
