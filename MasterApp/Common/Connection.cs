using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.Data.SqlClient;
using Dapper;

namespace MasterApp.Common
{
    /// <summary>
    ///  holds everyting that needs to be a singleton request span lifetime
    /// </summary>
    public class Connection : IDisposable
    {
        private static readonly string MyKey = "Singlet";
        private static IDictionary singKonTeks = new Dictionary<string, object>(); 
       public static Connection Instance
        {
            get
            {
                var me = kon[MyKey] as Connection;
                if (me == null)
                {
                    me = new Connection();
                    kon[MyKey] = me;
                }
                return me;
            }
        }

        public static IDictionary kon 
        {
            get 
            {
                return singKonTeks;                
            }
        }

        public void Dispose()
        {

            if (_db != null && _db.State == ConnectionState.Open) {
                
                try
                {
                    _db.Close();
                }
                catch (Exception ex)
                {
                    
                }
            }
            for (int i = kon.Keys.Count; i > 0; i--)
            {
                IDisposable o = kon[i] as IDisposable;
                if (o != null && o != this)
                {
                    try
                    {
                        o.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error when disposing " + i + "\r\n" + e.Trace());
                    }
                    kon[i] = null;
                }
            }
        }

        private IDbConnection _db = null;
        public IDbConnection DB
        {
            get
            {
                if (_db == null)
                {
                    _db = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);
                    if ((_db.State == ConnectionState.Closed) || (_db.State == ConnectionState.Broken))
                    {
                        
                        _db.Open();
                    }
                }
                return _db;
            }
        }

        public QStore SQL
        {
            get
            {
                var q = Getter<QStore>();
                if (q == null)
                {
                    GetRef<QStore>(ref q);
                    Setter(q);
                }
                return q;
            }
        }

        public T GetRef<T>(ref T x, params object[] o)
        {
            if (x == null)
            {
                x = (T)Activator.CreateInstance(typeof(T), o);
            }
            return x;
        }

        protected IDbTransaction DbTx = null;
        protected bool DbBufferd = true;
        protected int? DbTimeOut = null;

        public void QuSet(IDbTransaction t, bool buf, int? timeout = null)
        {
            DbTx = t;
            DbBufferd = buf;
            DbTimeOut = timeout;
        }

        public IEnumerable<T> Qu<T>(string sqlID, params object[] o)
        {
            string sql = this.SQL[sqlID];
            if (string.IsNullOrEmpty(sql))
                throw new Exception(string.Format("sql[\"{0}\"] is empty or not found", sqlID));
            return DB.Query<T>(string.Format(this.SQL[sqlID], o), null, DbTx, DbBufferd, DbTimeOut);
        }

        public IEnumerable<T> Fetch<T>(
                  string sqlID
                , dynamic param = null
                , IDbTransaction transaction = null
                , bool buffered = true
                , int? commandTimeout = null
                , CommandType? commandType = null)
        {
            string sql = this.SQL[sqlID];
            if (string.IsNullOrEmpty(sql))
                throw new Exception(string.Format("sql[\"{0}\"] is empty or not found", sqlID));
            return DB.Query<T>(sql, param as object, transaction, buffered, commandTimeout, commandType);
        }

        public int Exec(
                  string spName
                , dynamic param = null
                , IDbTransaction transaction = null
                , int? commandTimeout = null
                , CommandType? commandType = CommandType.StoredProcedure)
        {
            return DB.Execute(spName, param as object, transaction, commandTimeout, commandType);
        }

        public int Do(
                string SqlID
                , dynamic param = null
                , IDbTransaction transaction = null
                , int? commandTimeout = null
                , CommandType? commandType = CommandType.Text)
        {
            string sql = this.SQL[SqlID];
            if (string.IsNullOrEmpty(sql))
                throw new Exception(string.Format("sql[\"{0}\"] is empty or not found", SqlID));

            return DB.Execute(sql, param as object, transaction, commandTimeout, commandType);
        }

        private T Getter<T>()
        {
            object a = null;
            string X = MyKey + "/" + typeof(T).Name;
            a = kon[X];

            return (T)a;
        }

        private void Setter<T>(T value)
        {
            string X = "Singlet/" + typeof(T).Name;
            kon[X] = value;            
        }

        public static readonly string Defaults = "=|Put=HrDataServices.Logic.TextLogger|Say=HrDataServices.Logic.TextLogger|";

        public static string Default(string name)
        {
            string v = null;
            string k = "|" + name + "=";
            int i = Defaults.IndexOf(k);
            int l = i + k.Length;
            int j = (i > 0) ? Defaults.IndexOf('|', l) : 0;
            if (i < j && j > 0 && (j - l) > 0)
            {
                v = Defaults.Substring(l, j - (l));
            }
            return v;
        }

        public static T Get<T>(params object[] args)
        {
            string n = typeof(T).Name;
            n = n.Substring(1, n.Length - 1);
            string x = ConfigurationManager.AppSettings[n];
            if (string.IsNullOrEmpty(x))
            {
                x = Default(n);
                if (x.isEmpty())
                    throw new Exception("AppSetting key not found: \"" + n + "\"");
            }
            Type t = Type.GetType(x);
            if (t != null)
            {
                if (args == null || args.Length < 1)
                {
                    return (T)Activator.CreateInstance(t);
                }
                else
                {
                    return (T)Activator.CreateInstance(t, args);
                }
            }
            else
                throw new Exception("Class not found: " + x);
        }

        public object this[string Index]
        {
            get
            {
                return kon[Index];
            }

            set
            {
                kon[Index] = value;
            }
        }

        private T Prop<T>()
        {
            T s = Getter<T>();
            if (s == null)
            {
                s = Get<T>();
                Setter(s);
            }
            return s;
        }


        public ISay Text
        {
            get
            {
                return Prop<ISay>();
            }
        }

        public IPut Log
        {
            get
            {
                return Prop<IPut>(); 
            }
        }
       
    }
}