using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MasterApp.Common
{
    public static class AppSetting
    {
        private static string _LdapDomain = Read("LdapDomain");
        public static string LdapDomain
        {
            get { return AppSetting._LdapDomain; }
        }

        public static string _Domain = Read("Domain", "toyota");
        public static string Domain
        {
            get
            {
                return _Domain;
            }
        }

        private static string _ApplicationID = Read("ApplicationID");
        public static string ApplicationID
        {
            get { return _ApplicationID; }
        }

        public static int _Debug = Read("DEBUG", "0").Int();
        public static int Debug
        {
            get { return _Debug; }
        }


        public static decimal ReadDecimal(string setting, decimal value = 0)
        {
            string b = ConfigurationManager.AppSettings[setting];
            if (string.IsNullOrEmpty(b))
                return value;
            else
            {
                decimal x = value;
                if (decimal.TryParse(b, out x))
                    return x;
                else
                    return value;
            }
        }

        public static int ReadInt(string setting, int value = 0)
        {
            string b = ConfigurationManager.AppSettings[setting];
            if (string.IsNullOrEmpty(b))
                return value;
            else
            {
                int x = value;
                if (Int32.TryParse(b, out x))
                    return x;
                else
                    return value;
            }
        }

        public static string Read(string setting, string value = "")
        {
            string b = ConfigurationManager.AppSettings[setting];
            if (string.IsNullOrEmpty(b))
                return value;
            else
            {
                return b;
            }
        }

    }
}