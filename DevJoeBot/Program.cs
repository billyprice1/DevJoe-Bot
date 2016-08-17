using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using DSG;

namespace DevJoeBot
{
    class Program
    {

        private static ulong OwnerID = 0;
        public static DiscordClient c = null;
        private static Thread CT = null;

        static void Main(string[] args)
        {
            if(args.Length == 1 && args[0] == "GEN")
            {
                Generator.Start();
                return;
            }
            // Console.Title = "DevJoeBot Console";
            if (File.Exists("latest.log")) File.Delete("latest.log");
            Settings.LoadSettings();
            string token = tokenReq();
            c = new DiscordClient();
            Settings.settings.firstrun = false;
            Settings.SaveSettings();
            //Console.Clear();
            Console.WriteLine("###############################");
            Console.WriteLine("# SERVER CONSOLE");
            Console.WriteLine("# Here lies all the logging.");
            Console.WriteLine();
            Console.WriteLine("We're setting up for you..");

            //ConsoleTools l = new ConsoleTools();
            //Thread t = new Thread(new ThreadStart(l.commandInput));
            //CT = t;
            //t.Start();

            //while (!t.IsAlive) { };

            // COMMANDS
            Command ca = new Command(true, "help", AquiredRank.USER);
            ca.description = "Shows all commands.";
            ca.syntax = ";help";
            ca.onCommandRun += A_onCommandRun;

            Command cb = new Command(true, "about", AquiredRank.USER);
            cb.description = "Shows information about the bot.";
            cb.syntax = ";about [bot|dev]";
            cb.onCommandRun += Cb_onCommandRun;

            Command cg = new Command(true, "testcomm", AquiredRank.MOD);
            cg.description = "A dummy command";
            cg.onCommandRun += Cf_onCommandRun1;

            Command cd = new Command(true, "modmode", AquiredRank.OWNER);
            cd.description = "Toggles moderation mode for the current server (Turning this on will allow the bot to moderate the server. Be sure that the bot has the required permissions. [Manage Messages, Kick, Ban])";
            cd.syntax = ";modmode [toggle|options]";
            cd.onCommandRun += Cd_onCommandRun;

            Command ce = new Command(true, "modrole", AquiredRank.OWNER);
            ce.description = "Gets/sets the mod rank for the current server";
            ce.syntax = ";modrole [role_name]";
            ce.onCommandRun += Ce_onCommandRun;

            Command cc = new Command(true, "shutdown", AquiredRank.SUPERUSER);
            cc.description = "Shuts the bot down";
            cc.syntax = ";shutdown";
            cc.onCommandRun += Cc_onCommandRun;

            Command cf = new DevJoeBot.Command(true, "status", AquiredRank.SUPERUSER);
            cf.description = "Changes the bot's status";
            cf.syntax = ";status [Status message]";
            cf.onCommandRun += Cf_onCommandRun;

            OwnerID = ulong.Parse(Settings.settings.owner);

            c.ExecuteAndWait(async() => {
                await c.Connect(token);
                c.MessageReceived += C_MessageReceived;
                c.JoinedServer += C_JoinedServer;
                c.LeftServer += C_LeftServer;
                c.ServerAvailable += C_ServerAvailable;
                ConsoleTools.Log("READY!");
            });
        }

        private static void Cf_onCommandRun1(object sender, string name, string[] args, User user, Channel c)
        {
            c.SendMessage(CF.f(user, "Test success"));
        }

        private static void C_ServerAvailable(object sender, ServerEventArgs e)
        {
            ConsoleTools.Log("Server " + e.Server.Id + " has declared it's ready state.");
            if (getServerObject(e.Server.Id) == null)
            {
                Settings.settings.servers.Add(new ServerObject(e.Server));
            }
        }

        private static void Cf_onCommandRun(object sender, string name, string[] args, User user, Channel cc)
        {
            if (args.Length >= 1)
            {
                string construct = "";
                for(int i=0;i<args.Length;i++)
                {
                    construct += args[i] + " ";
                }
                c.SetGame(construct);
                cc.SendMessage(CF.f(user, "Status updated!"));
            } else
            {
                cc.SendMessage(CF.f(user, "This command requires 1 argument."));
            }
        }

        private static void Ce_onCommandRun(object sender, string name, string[] args, User user, Channel c)
        {
            // ;modrole
            if(args.Length == 0)
            {
                if (getServerObject(c.Server.Id).modRole != 0)
                {
                    ulong modid = getServerObject(c.Server.Id).modRole;
                    Role modrole = c.Server.GetRole(modid);
                    if (modrole != null)
                    {
                        c.SendMessage(CF.f(user, "The moderator role for this server is " + modrole.Name + " (" + modrole.Id + ")"));
                    }
                }
                else
                {
                    c.SendMessage(CF.f(user, "No moderator role for this server is specified"));
                }
            } else if(args.Length == 1)
            {
                Role[] r = c.Server.Roles.ToArray();
                Role role = null;
                for(int i=0;i<r.Length;i++)
                {
                    if (r[i].Name.ToLower() == args[0].ToLower())
                    {
                        role = r[i];
                    }
                }
                if(role == null)
                {
                    c.SendMessage(CF.f(user, "The role you specified does not exist."));
                } else
                {
                    getServerObject(c.Server.Id).modRole = role.Id;
                    c.SendMessage(CF.f(user, "Moderator role changed to " + role.Name + " (" + role.Id + ")"));
                    Settings.SaveSettings();
                }
            } else
            {
                c.SendMessage(CF.f(user, "This command requires 1 argument."));
            }
        }

        private static ServerObject getServerObject(ulong ID)
        {
            List<ServerObject> b = Settings.settings.servers;
            ServerObject[] objects = b.ToArray();
            for(int i=0;i<objects.Length;i++)
            {
                if(objects[i].ID == ID)
                {
                    return objects[i];
                 }
            }
            return null;
        }

        private static void Cd_onCommandRun(object sender, string name, string[] args, User user, Channel c)
        {
            if (args.Length == 1 && args[0] == "toggle")
            {
                getServerObject(c.Server.Id).toggleModerated();

                c.SendMessage(CF.f(user, "Toggled moderation mode. (Current status: " + getServerObject(c.Server.Id).isModerated() + ")"));
                Settings.SaveSettings();
            } else if (args.Length == 0)
            {
                bool m = getServerObject(c.Server.Id).isModerated();
                if(m)
                {
                    c.SendMessage(CF.f(user, "Moderation mode is currently activated."));
                } else
                {
                    c.SendMessage(CF.f(user, "Moderation mode isn't activated."));
                }
            } else
            {
                c.SendMessage(CF.f(user, "This command doesn't require any arguments."));
            }
        }

        private static void Cc_onCommandRun(object sender, string name, string[] args, User user, Channel cc)
        {
            c.Disconnect();
            ConsoleTools.Log("You may exit this window when ready.");
            Application.Exit();
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
                        } else if (e[i].requiredRank == 3)
                        {
                            plevel = "(Required Permission Level: SUPERUSER)";
                        }
                    if (e[i].syntax != "")
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
                        } else if(e[i].requiredRank == 3)
                        {
                            plevel = "(Required Permission Level: SUPERUSER)";
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
            if (getServerObject(e.Server.Id) != null)
            {
                Settings.settings.servers.Remove(getServerObject(e.Server.Id));
            }
        }

        private static void C_JoinedServer(object sender, ServerEventArgs e)
        {
            ConsoleTools.Log("Joined server " + e.Server.Name + " with ID " + e.Server.Id);
            if(getServerObject(e.Server.Id) == null)
            {
                Settings.settings.servers.Add(new ServerObject(e.Server));
            }
        }

        private static void C_MessageReceived(object sender, MessageEventArgs e)
        {

            if(e.User.Id == c.CurrentUser.Id)
            {
                return;
            }

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
                        ConsoleTools.Log("User " + e.User.Name + " (" + e.User.Id + ") executed '" + e.Message.Text + "' on server " + e.Server.Name + " (" + e.User.Id + ")");
                    } else if(c.requiredRank == 1)
                    {
                        if(e.User.HasRole(e.Server.GetRole(getServerObject(e.Server.Id).modRole)) || e.User.Id == OwnerID || e.User.Id == e.Server.Owner.Id)
                        {
                            c.execute(args, e.User, e.Channel);
                            ConsoleTools.Log("User " + e.User.Name + " (" + e.User.Id + ") executed '" + e.Message.Text + "' on server " + e.Server.Name + " (" + e.User.Id + ")");
                        } else
                        {
                            e.Channel.SendMessage(CF.f(e.User, "Hey! You don't have permission to run that! (Required Permission Level: MOD)"));
                            ConsoleTools.Log("User " + e.User.Name + " (" + e.User.Id + ") attempted to execute '" + e.Message.Text + "' on server " + e.Server.Name + " (" + e.User.Id + ") but didn't have permission level MOD");
                        }
                    } else if(c.requiredRank == 2)
                    {
                        if (e.User.Id == OwnerID || e.User.Id == e.Server.Owner.Id)
                        {
                            c.execute(args, e.User, e.Channel);
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") executed '" + e.Message.Text + "' on server " + e.Server.Name + " (" + e.User.Id + ")");
                        }
                        else
                        {
                            e.Channel.SendMessage(CF.f(e.User, "Hey! You don't have permission to run that! (Required Permission Level: OWNER)"));
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") attempted to execute '" + e.Message.Text + "' on server " + e.Server.Name + " (" + e.User.Id + ") but didn't have permission level OWNER");
                        }
                    } else if(c.requiredRank == 3)
                    {
                        if (e.User.Id == OwnerID)
                        {
                            c.execute(args, e.User, e.Channel);
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") executed '" + e.Message.Text + "' on server " + e.Server.Name + " (" + e.User.Id + ")");
                        }
                        else
                        {
                            e.Channel.SendMessage(CF.f(e.User, "Hey! You don't have permission to run that! (Required Permission Level: SUPERUSER)"));
                            Console.WriteLine("User " + e.User.Name + " (" + e.User.Id + ") attempted to execute '" + e.Message.Text + "' on server " + e.Server.Name + " (" + e.User.Id + ") but didn't have permission level SUPERUSER");
                        }
                    }
                }
            }

            bool mm = getServerObject(e.Server.Id).isModerated();
            if(mm)
            {
                if(e.Message.Text.ToCharArray().Length > 150)
                {
                    e.Message.Delete();
                    e.Channel.SendMessage(CF.f(e.User, "You have exceeded the 150 character limit in a message. Your message was removed."));
                }
            }

        }

        public static object[] genargs(string s)
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
                MessageBox.Show("Before you can use this bot, you need to generate a settings file.");
                Application.Exit();
            } else
            {
                return Settings.settings.token;
            }
            return "";
        }
    }
}
