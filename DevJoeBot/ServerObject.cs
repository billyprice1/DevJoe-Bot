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

        private Server s;
        public ulong ID;
        private bool moderated = false;

        public ServerObject(Server s)
        {
            this.s = s;
            ID = s.Id;
        }

        public Server getServer()
        {
            return s;
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
