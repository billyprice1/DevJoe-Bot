using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJoeBot
{
    [Serializable]
    class SaveableSettings
    {

        public List<Command> usercmds = new List<Command>();
        public bool autosignin = false;
        public string token = "";
        public string owner = "";
        public bool firstrun = true;
        public string joinmsg = "";
        public string leavemsg = "";
        public bool customcmds = false;
        public string modrole = "";

        public SaveableSettings()
        {

        }

    }
}
