using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevJoeBot
{
    class Settings
    {

        public static SaveableSettings settings = new SaveableSettings();

        public static void LoadSettings()
        {
            if (File.Exists("config\\settings.ini"))
            {
                settings = FileIO.LoadFile("config\\settings.ini");
            }
        }

        public static void SaveSettings()
        {
            FileIO.SaveFile("config\\settings.ini", settings);
        }

    }

    
}
