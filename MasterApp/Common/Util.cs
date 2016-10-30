using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace MasterApp.Common
{
    public static class Util
    {
        // Check whether the input is in number format - Neir Kate
        // Usage Example: if (IsDecimal(txtGridSpentAmount.Text))
        public static bool IsDecimal(String strVal)
        {
            decimal result;
            return Decimal.TryParse((strVal ?? "").Trim(), out result);
        }

        public static Decimal TryStrToDecimalDef(object a, decimal value = 0)
        {
            decimal R = 0;
            if (!Decimal.TryParse(Convert.ToString(a), out R)) R = value;
            return R;
        }
        readonly static string DirtyFileNamePattern = "[\\+/\\\\\\#%&*{}/:<>?|\"-]";

        public static string SanitizeFilename(string insane)
        {
            //string pattern = "[\\+/\\\\\\~#%&*{}/:<>?|\"-]";
            string replacement = "_";

            Regex regEx = new Regex(DirtyFileNamePattern);
            return Regex.Replace(regEx.Replace(insane, replacement), @"\s+", " ");
        }

        public static bool isDirty(string s)
        {
            return Regex.IsMatch(s, DirtyFileNamePattern);    
        }
        public static string CleanFilename(string s)
        {

            
            string t = SanitizeFilename(s);
            
            if (t.Length > 75)
            {
                string ext = Path.GetExtension(t);
                t = t.Substring(0, 75 - ext.Length - 1) + ext;
            }
            return t;
        }

        public static string SourceLocation(this StackTrace s)
        {
            if (s == null) return null;
            StackFrame F = s.GetFrame(1);
            if (F == null) return null;
            MethodBase M = F.GetMethod();
            if (M == null) return null;

            int lin = F.GetFileLineNumber();
            int col = F.GetFileColumnNumber();
            
            return string.Format("{0}.{1}" 
                                  + ((lin>0) 
                                      ? " @({2}, {3})"
                                      : "")
                                  , M.ReflectedType.Name, M.Name
                                  , lin, col);
        }

        public static int Write(this IPut i, Exception ex, string _user = "")
        {
            return i.Put(ex.Message + "\r\n" + ex.StackTrace, _user, ex.TargetSite.ReflectedType + "." + ex.TargetSite.Name);
        }


        public static string Base52(long l)
        {
            const byte BASE = 52;
            const string a = "etaoinshrdlucmfwypvbgkjqxzETAOINSHRDLUCMFWYPVBGKJQXZ";
            StringBuilder x = new StringBuilder("");
            byte n;
            do
            {
                n = (byte)(l % BASE);
                x.Insert(0, a.Substring(n, 1));
                l = (l - n) / BASE;
            }
            while (l > 0);
            return x.ToString();
        }


        public static long UnSexQuat(string s)
        {
            string A = "0123456789etaoinshrdlucmfwypvbgkjqxzETAOINSHRDLUCMFWYPVBGKJQXZ-_";
            const byte BASE = 64;
            long l = 0;
            int j = 1;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                string b = s.Substring(i, 1);

                int n = A.IndexOf(b);
                if (n >= 0)
                {
                    Console.WriteLine("{3} n= {0,2} j= {1,10} n*j= {2,15}", n, j, n * j, b);
                    l = l + ((n) * j);
                }

                j = j * BASE;
            }
            return l;
        }


        public static string SexQuat(long l)
        {
            const byte BASE = 64;
            const string A = "0123456789etaoinshrdlucmfwypvbgkjqxzETAOINSHRDLUCMFWYPVBGKJQXZ-_#";
            StringBuilder x = new StringBuilder("");
            byte n;
            do
            {
                n = (byte)(l % BASE);
                if (n < BASE)
                    x.Insert(0, A.Substring(n, 1));
                l = (l - n) / BASE;
            }
            while (l > 0);
            return x.ToString();
        }

        public static bool ForceDirectories(string d)
        {
            bool r = false;
            try
            {
                if (!Directory.Exists(d))
                {
                    Directory.CreateDirectory(d);
                }
                r = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return r;
        }


        public static string tick(string s)
        {
            return DateTime.Now.ToString("HH:mm:ss") + "\t" + s;
        }

        public delegate string Num2Str(decimal d, string CurrencyCd);

        public static string DecToStr(decimal d, string CurrencyCd)
        {
            return d.ToString("0.00");
        }

        public static string Unique(string n)
        {
            string dir = Path.GetDirectoryName(n)
                , fn = Path.GetFileNameWithoutExtension(n)
                , ext = Path.GetExtension(n)
                , fx = "";
            int i = 0;
            do
            {
                fx = Path.Combine(dir,
                        fn
                        + Base52((int)Math.Floor(DateTime.Now.TimeOfDay.TotalMilliseconds) + (i++))
                        + ext);
            } while (File.Exists(fx));
            return (fx);
        }

        public static string ReMoveFile(string n)
        {
            string newname = n;
            if (!File.Exists(n)) return n;

            try
            {
                newname = Unique(n);
                File.Move(n, newname);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RemoveFile ({0})", n);
                Debug.WriteLine(ex);
                newname = null;
            }
            return newname;
        }

     
    }
}