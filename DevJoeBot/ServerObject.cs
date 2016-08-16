using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DevJoeBot
{
    [Serializable]
    class ServerObject
    {

        public ulong ID;
        private bool moderated = false;
        public ulong modRole = 0;

        public ServerObject(Server s)
        {
            ID = s.Id;
        }

        public Server getServer()
        {
            Server[] servs = Program.c.Servers.ToArray();
            for(int i=0;i<servs.Length;i++)
            {
                if(servs[i].Id == ID)
                {
                    return servs[i];
                }
            }
            return null;
        }

        public bool isModerated()
        {
            return moderated;
        }

        public void toggleModerated()
        {
            moderated = !moderated;
        }

    }
}
