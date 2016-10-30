using System;
using System.Collections.Generic;

namespace MasterApp.Common
{
    [Serializable]
    public class IniSections
    {
        public Dictionary<string, string> d = new Dictionary<string, string>();

        public string this[string Index]
        {
            get
            {
                if (d.ContainsKey(Index))
                    return d[Index];
                else
                    return null;
            }

            set
            {
                if (d.ContainsKey(Index))
                {
                    d[Index] = value;
                }
                else
                {
                    d.Add(Index, value);
                }
            }
        }
    }

}