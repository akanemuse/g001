using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MasterApp.Common
{
    public static class StrTo
    {
        public const string INQ_DATE = "dd/MM/yyyy";
        public const string XLS_DATE = "dd-MM-yyyy";
        public const string TO_MINUTE = "dd MMM yyyy HH:mm";
        public const string TO_DATUM = "dd MMM yyyy";
        public const string SAP_DATUM = "dd.MM.yyyy";
        public const string SAP_IN_DATE = "yyyyMMdd";
        public const string ISO_DATUM = "yyyy.MM.dd";
        public const string SQL_DATUM = "yyyy-MM-dd";
        public const string dateTimeNoTime = "dd MMM yyyy";
        public const string dateTimeWithTime = "dd MMM yyyy HH:mm";
        public const int NULL_YEAR = 1753;

        public static DateTime NULL_DATE
        {
            get
            {
                return new DateTime(NULL_YEAR, 1, 1, 0, 0, 0);
            }
        }

        public static string str(this object o)
        {
            return (o!=null) ? Convert.ToString(o) : null;
        }
        public static string str(this int o)
        {
            return o.ToString();
        }

        public static string DATUM(this DateTime datum)
        {
            return fmt(datum, TO_DATUM);
        }
        public static string MINUTE(this DateTime datum)
        {
            return fmt(datum, TO_MINUTE);
        }

        public static string DATUM(this DateTime? datum)
        {
            return (datum!=null) ? fmt(datum, TO_DATUM): "";
        }
        public static string MINUTE(this DateTime? datum)
        {
            return fmt(datum, TO_MINUTE);
        }
        public static string fmt(this DateTime datum, string dateformat)
        {
            return datum.ToString(dateformat);
        }
        public static string fmt(this DateTime? datum, string dateformat = XLS_DATE)
        {
            return (datum != null) ? 
                ((DateTime)datum).ToString(dateformat) 
                : "";
        }
        public static string SAPdecfmt(this decimal num, bool fractions = false)
        {
            decimal tnum = (!fractions) ? Math.Truncate(num) : num;
            return tnum.ToString(fractions ? "0.00" : "0");
        }

        public static string SAPdecfmt(this Decimal? num, bool fractions = false)
        {
            return (num != null) ? SAPdecfmt(num ?? 0, fractions) : "";     
        }

        public static string SAPnum(this decimal num, bool fractions=false)
        {
            return num.ToString(fractions? "0.00": "0");
        }
        
        public static string SAPnum(this Decimal? num, bool fractions=false)
        {
            return (num != null) ? (num??0).ToString(fractions? "0.00": "0") : "";            
        }
        public static string ISOdate(this DateTime? datum)
        {
            if (datum != null)
                return fmt(datum, ISO_DATUM);
            else
                return "";
        }
        public static string ISOdate(this DateTime datum)
        {
            return fmt(datum, ISO_DATUM);
        }
        public static string SAPdate(this DateTime? datum)
        {
            return fmt(datum, SAP_DATUM);
        }
        public static string SAPdate(this DateTime datum)
        {
            return fmt(datum, SAP_DATUM);
        }
        public static string SAPInDate(this DateTime? datum)
        {
            return fmt(datum, SAP_IN_DATE);
        }
        public static string SAPInDate(this DateTime datum)
        {
            return fmt(datum, SAP_IN_DATE);
        }

        public static DateTime dateSAP(this string d)
        {
            return Date(d, SAP_DATUM, null);
        }

        public static DateTime? dateSAPnul(this string s)
        {
            DateTime D = new DateTime();
            if (!DateOk(s, SAP_DATUM, ref D))
                return null;
            else
                return D;
        }


        public static DateTime? dateSQL(this string s)
        {
            DateTime D = new DateTime();
            if (!DateOk(s, SQL_DATUM, ref D))
                return null;
            else
                return D;
        }

        public static string fmt(this decimal? n, int dec = 0)
        {
            return string.Format("{0:N" + dec.ToString() + "}", n ?? 0);
        }
        public static string fmt(this decimal n, int dec = 0)
        {
            return string.Format("{0:N" + dec.ToString() + "}", n);            
        }
        public static string date(this DateTime d)
        {
            return d.ToString(dateTimeNoTime);
        }
        public static string date(this DateTime? d)
        {
            return (d!=null) ? ((DateTime) d).ToString(dateTimeNoTime) :"";
        }

        public static string minute(this DateTime d)
        {
            return d.ToString(dateTimeWithTime);
        }
        public static string minute(this DateTime? d)
        {
            return (d != null) ? ((DateTime)d).ToString(dateTimeWithTime) : "";
        }
        
        public static bool isEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static int Int(this object o, int v=0)
        {
            return Int(Convert.ToString(o), v);
        }

        public static int Int(this string s, int v = 0)
        {
            int R = 0;
            
            if (s.isEmpty() || !Int32.TryParse(s.Trim(), out R)) R = v;
            return R;
        }
        public static long Long(this string s, long v = 0)
        {
            long R = 0;
            if (!Int64.TryParse(s, out R)) R = v;
            return R;
        }

        public static bool Bool(this string s)
        {
            return (Int(s, 0) > 0);
        }

        public static bool Bool(this int n)
        {
            return n != 0;
        }

        public static long Long(this object o, long v = 0)
        {
            return Long(Convert.ToString(o), v);
        }
        public static double Num(this object o, double v = 0.0d)
        {
            double d = 0;
            if (!Double.TryParse(Convert.ToString(o), out d)) d = v;
            return d;
        }
        public static decimal Dec(this object o, decimal v = 0m)
        {
            decimal x = 0;
            
            if (o==null || !decimal.TryParse(o.ToString(), out x))
            {
                x = v;
            }
            return x;
        }
        public static decimal Dec(this string s, decimal v=0)
        {
            decimal R = 0;
            if (s.isEmpty())
            {
                return v;
            }
            CultureInfo cu = CultureInfo.CurrentCulture; 
            NumberStyles nu = NumberStyles.Number;

            /// find last pos of '.' and ',' whichever comes first 
            int ldot = s.LastIndexOf('.');
            int lcom = s.LastIndexOf(',');
            if (ldot> lcom ) 
            {
                /// last dot is decimal point instead 
                if (lcom > 1)
                    nu = nu | NumberStyles.AllowThousands;

                cu = CultureInfo.CreateSpecificCulture("en-US");
            }
            if (!Decimal.TryParse(s, nu, cu, out R)) R = v;
            return R;
        }

        public static bool DateOk(this string s, string fmt, ref DateTime d)
        {
            bool r = DateTime.TryParseExact(s, fmt, CultureInfo.InvariantCulture, 
                DateTimeStyles.AssumeLocal, out d);
            return r;
        }

        public static DateTime Date(this string s, string fmt, DateTime? v=null)
        {
            DateTime r = new DateTime();
            if (!DateTime.TryParseExact(s, fmt, CultureInfo.InvariantCulture, 
                DateTimeStyles.AssumeLocal, out r))
                r = v ?? new DateTime(NULL_YEAR, 1, 1);
            return r;
        }

        public static DateTime Date(this string a, DateTime? def = null)
        {
            return Date(a, INQ_DATE, def);
        }

        public static DateTime Date(this string a)
        {
            return Date(a, XLS_DATE, new DateTime(NULL_YEAR, 1, 1, 0, 0, 0));            
        }

        public static DateTime DateFrom(this string a, DateTime? def = null)
        {
            DateTime r = Date(a, def);
            DateTime ret = new DateTime(r.Year, r.Month, r.Day, 0, 0, 0);
            return ret;
        }

        public static DateTime DateTo(this string a, DateTime? def = null)
        {
            DateTime r = Date(a, def);

            DateTime ret = new DateTime(r.Year, r.Month, r.Day, 23, 59, 59);
            return ret;
        }

        public static DateTime? DateNull(this string a, string fmt = SAP_IN_DATE)
        {
            DateTime? r = null;
            DateTime t = new DateTime();
            if (DateTime.TryParseExact(a, fmt, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out t))
                r = t;
            return r;
        }


        public static decimal? SAPval(this string s)
        {
            if (s.isEmpty())
                return null;
            decimal? d = null;
            s = s.Trim();
            //int mul; 
            if (s.Substring(s.Length - 1, 1).Equals("-")) { 
                //mul = -1;
                s = s.Substring(0, s.Length - 1).Trim();
            }
            decimal v = 0;
            if (decimal.TryParse(s, out v))
            {
                d = v;
            }
            return d;
        }

        public static string mul(this string s, int n)
        {
            StringBuilder b = new StringBuilder("");
            for (int i = 0; i < n; i++)
            {
                b.Append(s);
            }
            return b.ToString();
        }


        public static string box(this bool b, string n = "")
        {
            return "[" + (b ? "v" : " ") + "]"
                    + (!n.isEmpty() ? " " + n : "");
        }

        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static string RemoveQuotes(string s)
        {
            if (!s.isEmpty())
            {
                return s.Replace("'", "").Replace("\"", "").Replace("`", "");
            }
            else
            {
                return null;
            }
        }

        public static string EscapeQuotes(string s)
        {
            if (!s.isEmpty())
            {
                return s.Replace("'", "''");
            }
            else
            {
                return null;
            }
        }

        public static decimal Frac(this decimal d)
        {
            return d - decimal.Truncate(d);
        }

        public static decimal Frac(this decimal? d) 
        {
            decimal x = d ?? 0;

            return x - decimal.Truncate(x);
        }
        
    }
}
