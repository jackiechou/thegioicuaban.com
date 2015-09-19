using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary
{
    public static class Settings
    {
        public static string ConnectionString = @"Data Source=23.91.114.136;Initial Catalog=5eagle_VBA;User ID=VBA_USER;Password=VBAVBA;Min Pool Size=20; Max Pool Size=200; Incr Pool Size=10; Decr Pool Size=5;Connection Timeout=216000";
        public static int ConnectionTimeout = 216000;
        public static int CommandTimeout = 900000;
        public static int SessionTimeout = 216000;    
    }   
}
