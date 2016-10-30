using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;

namespace MasterApp.Common
{
    public class TextLogger : IPut, ISay
    {
        private int pid = 0;
        private string _dir = "";
        private string lastWhere = "";
        private readonly string dateFmt = "HH:mm:ss";
        private string filler = "        ";
        private DateTime lastNow = new DateTime(1900, 12, 31);
        public readonly string logExt = ".txt";
        public TextLogger()
        {
            _dir = GetPath();

            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }
        }

        public int Put(string what,
            string user = "",
            string site = "",
            int ID = 0, // ignored 
            string function = "", // ignored 
            string remarks = null, // ignored 
            int tag = 0, // ignored 
            string messageId = "", // ignored 
            string status = null // ignored 
            )
        {
            if (string.IsNullOrEmpty(site))
                site = (new StackTrace(true)).SourceLocation();

            string lin = "";
            string filename = user + logExt;
            if (string.Compare(lastWhere, site) != 0)
            {
                lastWhere = site;
                lin = filler + " [" + site + "]" + Environment.NewLine;
            }
            else
                site = "";

            string ts = "        ";
            if ( Math.Floor( (DateTime.Now - lastNow).TotalSeconds) > 0)
            {
                ts = DateTime.Now.ToString(dateFmt);
                lastNow = DateTime.Now;
            }
            else
            {
                ts = filler;
            }
            lin = lin + string.Format("{0} {1}\r\n", ts, what);
            string logfile = Path.Combine(_dir, filename);

            try
            {
                File.AppendAllText(logfile, lin);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(lin);
                Debug.WriteLine(ex.Message);
            }
            if (ID >= 0)
            {
                pid = ID;
            }
            else
            {
                pid = 1;
            }
            return pid;
        }

        public static string GetPath()
        {
            DateTime n = DateTime.Now;
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                , AppSetting.Domain, AppSetting.ApplicationID, "log"
                , n.Year.ToString()
                , n.Month.ToString()
                , n.Day.ToString());
        }

        public void Dispose()
        {

        }

        public string _path = GetPath();

        public int Say(string loc, string w, params object[] x)
        {
            string path = GetPath();

            string logfile = Path.Combine(_path, ((string.IsNullOrEmpty(loc)) ? "0" + logExt : Util.SanitizeFilename(loc) + logExt));
            string ts = filler;
            if (Math.Floor( (DateTime.Now - lastNow).TotalSeconds) > 0)
            {
                ts = DateTime.Now.ToString(dateFmt);
                lastNow = DateTime.Now;
            }
            else
            {
                ts = filler;
            }
            try
            {
                if (!Util.ForceDirectories(_path))
                    return -1;
                File.AppendAllText(logfile, Environment.NewLine + ts + " "
                    + ((x != null) ? String.Format(w.Replace("|", Environment.NewLine + "\t"), x) : w));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                
                return -2;
            }
            return 0;
        }
    }
}