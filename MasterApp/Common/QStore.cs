using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace MasterApp.Common
{
    public class QStore
    {

        public QStore()
        {
            Init();
        }
        
        IniSections data = null;

        public string QueryPath
        {
            get
            {
                string dir = AppSetting.Read("SQLFilesDir", "SQL");
                if (dir.isEmpty()) dir = "SQL";

                if (!dir.Contains(":\\"))
                {
                    if (HttpRuntime.AppDomainAppId != null)
                        dir = Path.Combine(Path.GetDirectoryName(HostingEnvironment.ApplicationPhysicalPath), dir);
                    else
                        dir = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), dir);
                    
                }
                return dir;
            }
        }

        public void Init()
        {
                data = Connection.Instance["IniSQL"] as IniSections;
                if (data == null)
                {
                    data = new IniSections();
                    Connection.Instance["IniSQL"] = data;

                    var ak = ConfigurationManager.AppSettings.AllKeys.Where(x => x.StartsWith("SQLFiles"));
                    string[] f;
                    if (ak.Any())
                    {
                        f = ak.ToArray();
                        for (int i = 0; i < f.Length; i++)
                        {
                            string qx = f[i];
                            if (f[i].Substring(8).Int(0) > 0)
                            {
                                f[i] = ConfigurationManager.AppSettings[f[i]];
                            }
                            else f[i] = "";
                        }
                    }
                    else
                        f = new string[] { "q.sql" };

                    for (int i = 0; i < f.Length; i++)
                    {
                        if (f[i].isEmpty()) continue;

                        string p = Path.Combine(QueryPath, f[i]);
                        Ini ini = new Ini(p);
                        foreach (string s in ini.Sections)
                        {
                            data[s] = ini.Section[s];
                        }
                    }

                }

            
        }


        public void ResetCache()
        {
            Connection.Instance["IniSQL"] = null;
            
            Init();
            
        }
        public string this[string Index]
        {
            get
            {
                /// non value index not acceptable 
                if (Index.isEmpty()) return null;

                if (data == null) 
                    Init();

                // init failed 
                if (data == null) return null;

                if (data != null) 
                {
                    string qq = data[Index];
                    if (!qq.isEmpty())
                        return qq;
                    else 
                    {
                        string t = null;
                        string dir = QueryPath;
                        string filename = Path.Combine(QueryPath, Util.SanitizeFilename(Index));
                        if (Index.Contains("/"))
                        {
                            string v = Index.Replace('/', Path.DirectorySeparatorChar);
                            int i = v.LastIndexOf(Path.DirectorySeparatorChar);
                            string d = v.Substring(0, i);
                            string f = v.Substring(i + 1, v.Length - i - 1);
                            filename = Path.Combine(QueryPath + d, f);
                        }
                        string fn = filename+ ".sql";
                        string ft = filename + ".txt";
                        string sql = fn;
                        if (File.Exists(fn))
                            sql = fn;
                        else if (File.Exists(ft))
                            sql = ft;
                        else
                            sql = "";

                        if (!string.IsNullOrEmpty(sql)) 
                        {
                            t = File.ReadAllText(sql);
                            if (!t.isEmpty())
                            {
                                if (!t.StartsWith("--"))
                                {
                                    t = string.Format("-- {0}\r\n\r\n",  Index) + t;
                                }
                                data[Index] = t;
                            }
                        }
                        return t;
                    }

                }
                else
                    return null;
            }
        }
    }
}