using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJoeBot
{

    public delegate void CommandRun(object sender, string name, string[] args, Discord.User user, Discord.Channel c);

    [Serializable]
    class Command
    {

        public static List<Command> defcommands = new List<Command>();
        public static List<Command> user = new List<Command>();

        public string name = "";
        public string description = "";
        public string syntax = "";
        public int requiredRank = 0;
        public event CommandRun onCommandRun;

        public Command(bool a, string name, int requiredRank)
        {
            if(a)
            {
                defcommands.Add(this);
            } else
            {
                user.Add(this);
            }
            this.name = ";"+name;
            this.requiredRank = requiredRank;
        }

        public static Command getCommand(string r)
        {
            Command[] list1 = defcommands.ToArray();
            for(int i=0;i<list1.Length;i++)
            {
                if(list1[i].name.ToLower() == r.ToLower())
                {
                    return list1[i];
                }
            }
            Command[] list2 = user.ToArray();
            for(int i=0;i<list2.Length;i++)
            {
                if (list2[i].name.ToLower() == r.ToLower())
                {
                    return list2[i];
                }
            }
            return null;
        }

        public void execute(string[] args, Discord.User u, Discord.Channel c)
        {
            onCommandRun.Invoke(this, name, args, u, c);
        }
    }
}
