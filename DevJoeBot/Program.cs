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

        private static ulong OwnerID = 0;
        private static DiscordClient c = null;

        static void Main(string[] args)
        {
            Settings.LoadSettings();
            string token = tokenReq();
            c = new DiscordClient();
            Settings.settings.firstrun = false;
            Settings.SaveSettings();
            Console.Clear();
            Console.WriteLine("###############################");
            Console.WriteLine("# SERVER CONSOLE");
            Console.WriteLine("# Here lies all the logging.");
            Console.WriteLine();
            Console.WriteLine("We're setting up for you..");

            // COMMANDS
            Command ca = new Command(true, "help", AquiredRank.USER);
            ca.description = "Shows all commands.";
            ca.syntax = ";help";
            ca.onCommandRun += A_onCommandRun;

            Command cb = new Command(true, "about", AquiredRank.USER);
            cb.description = "Shows information about the bot.";
            cb.syntax = ";about [bot|dev]";
            cb.onCommandRun += Cb_onCommandRun;

            Command cc = new Command(true, "shutdown", AquiredRank.OWNER);
            cc.description = "Shuts the bot down";
            cc.syntax = ";shutdown";
            cc.onCommandRun += Cc_onCommandRun;

            OwnerID = ulong.Parse(Settings.settings.owner);

            c.ExecuteAndWait(async() => {
                await c.Connect(token);
                c.MessageReceived += C_MessageReceived;
                c.JoinedServer += C_JoinedServer;
                c.LeftServer += C_LeftServer;
                Console.WriteLine("READY!");
            });
        }

        private static void Cc_onCommandRun(object sender, string name, string[] args, User user, Channel cc)
        {
            c.Disconnect();
            Console.WriteLine("Cleaing up...");
        }

        private static void Cb_onCommandRun(object sender, string name, string[] args, User user, Channel c)
        {
            if(args.Length == 1)
            {
                if(args[0].ToLower() == "bot")
                {
                    c.SendMessage(CF.f(user, "I'm DevJoeBot, a project started in 8/12/2016. I use Discord.NET and C#."));
                } else if(args[0].ToLower() == "dev")
                {
                    c.SendMessage(CF.f(user, "The dev of this project is DevJoe#0633."));
                } else
                {
                    c.SendMessage(CF.f(user, "Unknown about article."));
                }
            } else
            {
                c.SendMessage(CF.f(user, "This command takes 1 argument. (Syntax: ;about [bot|dev])"));
            }
        }

        private static void A_onCommandRun(object sender, string name, string[] args, User a, Channel c)
        {
            if(args.Length == 0)
            {
                int reglength = Command.defcommands.ToArray().Length;
                int usrlength = Command.user.ToArray().Length;
                string s = "Displaying all commands:";
                if(reglength > 0)
                {
                    s += "\n\n## GENERAL COMMANDS ##\n";
                    Command[] e = Command.defcommands.ToArray();
                    for(int i=0;i<e.Length;i++)
                    {
                        string desc = "No description provided.";
                        if(e[i].description != "")
                        {
                            desc = e[i].description;
                        }
                        string plevel = "(Required Permission Level: USER)";
                        if(e[i].requiredRank == 1)
                        {
                            plevel = "(Required Permission Level: MOD)";
                        }
                        else if (e[i].requiredRank == 2)
                        {
                            plevel = "(Required Permission Level: OWNER)";
                        }
                        if(e[i].syntax != "")
                        {
                            s += "\n" + e[i].syntax + " - " + desc + " " + plevel;
                        } else
                        {
                            s += "\n" + e[i].name + " - " + desc + " " + plevel;
                        }
                    }
                }
                if(usrlength > 0)
                {
                    s += "\n\n## USER-DEFINED COMMANDS ##\n";
                    Command[] e = Command.user.ToArray();
                    for (int i = 0; i < e.Length; i++)
                    {
                        string desc = "No description provided.";
                        if (e[i].description != "")
                        {
                            desc = e[i].description;
                        }
                        string plevel = "(Required Permission Level: USER)";
                        if (e[i].requiredRank == 1)
                        {
                            plevel = "(Required Permission Level: MOD)";
                        }
                        else if(e[i].requiredRank == 2)
                        {
                            plevel = "(Required Permission Level: OWNER)";
                        }
                        if (e[i].syntax != "")
                        {
                            s += "\n" + e[i].syntax + " - " + desc + " " + plevel;
                        }
                        else
                        {
                            s += "\n" + e[i].name + " - " + desc + " " + plevel;
                        }
                    }
                }

                if(usrlength <= 0 && reglength <= 0)
                {
                    s += "\nNo commands to display.";
                }
                c.SendMessage(CF.f(a, "``` "+s+" ```"));
            } else
            {
                c.SendMessage(CF.f(a, "This command takes no arguments."));
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
                    if (c.requiredRank == 0)
                    {
                        c.execute(args, e.User, e.Channel);
                        Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") executed '" + e.Message + "' on server " + e.Server.Name + " (" + e.User.Id + ")");
                    } else if(c.requiredRank == 1)
                    {
                        if(false || e.User.Id == OwnerID)
                        {
                            c.execute(args, e.User, e.Channel);
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") executed '" + e.Message + "' on server " + e.Server.Name + " (" + e.User.Id + ")");
                        } else
                        {
                            e.Channel.SendMessage(CF.f(e.User, "Hey! You don't have permission to run that! (Required Permission Level: MOD)"));
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") attempted to execute '" + e.Message + "' on server " + e.Server.Name + " (" + e.User.Id + ") but didn't have permission level MOD");
                        }
                    } else
                    {
                        if (e.User.Id == OwnerID)
                        {
                            c.execute(args, e.User, e.Channel);
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") executed '" + e.Message + "' on server " + e.Server.Name + " (" + e.User.Id + ")");
                        }
                        else
                        {
                            e.Channel.SendMessage(CF.f(e.User, "Hey! You don't have permission to run that! (Required Permission Level: OWNER)"));
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") attempted to execute '" + e.Message + "' on server " + e.Server.Name + " (" + e.User.Id + ") but didn't have permission level OWNER");
                        }
                    }
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
                    Settings.settings.owner = owner;
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
