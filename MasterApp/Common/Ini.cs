using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MasterApp.Common
{
    public class Ini
    {
        private string ini = "";

        private Dictionary<string, string> kv = new Dictionary<string, string>();
        private IniSections ise = new IniSections();

        public List<string> Sections = new List<string>();

        private void FillSection(string sectionName, StringBuilder contents)
        {
            if (!string.IsNullOrEmpty(sectionName))
            {
                ise[sectionName] = contents.ToString();
                contents.Clear();
            }
        }

        public Ini(string inFile)
        {
            if (File.Exists(inFile))
            {
                ini = File.ReadAllText(inFile);

                string[] t = ini.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                string section = "";

                int blanks = 0;
                StringBuilder sBody = new StringBuilder("");

                foreach (string u in t)
                {
                    if (u.isEmpty() || u.StartsWith("*") || u.StartsWith("/*")
                        || u.StartsWith(";") || u.StartsWith("#")
                        || u.StartsWith("//")
                        || u.StartsWith("--")
                        )
                    {
                        if (!u.StartsWith("---- "))
                            blanks++;
                        continue;
                    }

                    string s = u.Trim();
                    if (s.StartsWith("[") && s.EndsWith("]"))
                    {
                        FillSection(section, sBody);
                        blanks = 0;
                        section = s.Substring(1, s.Length - 2).Trim();
                        Sections.Add(section);
                    }
                    else if (s.StartsWith("---- "))
                    {
                        FillSection(section, sBody);
                        blanks = 0;
                        section = s.Substring(5, s.Length - 5).Trim();
                        Sections.Add(section);
                    }
                    else
                    {
                        sBody.AppendLine(u);
                        if (blanks < 1)
                        {
                            if (u.IndexOf('=') > 0)
                            {
                                string[] v = u.Split('=');
                                string k = "";
                                if (v.Length > 0)
                                {
                                    k = v[0].Trim();
                                    if (!section.isEmpty()) { k = section + "." + k; }
                                }
                                if (v.Length > 1)
                                {
                                    if (!kv.ContainsKey(k))
                                        kv.Add(k, v[1].Trim());
                                    else
                                        kv[k] = v[1].Trim();
                                }
                            }
                        }
                        blanks = 0;
                    }
                }

                FillSection(section, sBody);
            }
        }

        public string this[string index]
        {
            get
            {
                if (kv.Keys.Contains(index))
                    return kv[index];
                else
                    return null;
            }

            set
            {
                if (!kv.Keys.Contains(index))
                    kv.Add(index, value);
                else
                    kv[index] = value;
            }

        }

        public IniSections Section
        {
            get
            {
                return ise;
            }
        }
    }
}