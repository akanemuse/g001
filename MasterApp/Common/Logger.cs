using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Diagnostics;

using MasterApp.Common;
using Dapper;

namespace MasterApp.Common
{
    public class Logger : IPut
    {

        public Logger()
        {

        }

        private int pid = 0;
        public int PID
        {
            get
            {
                return pid;
            }
        }

        public int Put(
            string _what,
            string _user = "",
            string _where = "",
            int ID = 0,
            string _func = "",
            string _remarks = null,
            int _tag = 0, 
            string _id = "",
            string _sts = null)
        {
            if (ID < 1 && pid > 0)
                ID = pid;
            else if (ID > 0)
                pid = ID;

            if (string.IsNullOrEmpty(_where))
            {
                _where = (new StackTrace(true)).SourceLocation();
            }
            
            DynamicParameters dyn = new DynamicParameters();
            dyn.Add("@what", _what);
            dyn.Add("@user", _user);
            dyn.Add("@where", _where);
            dyn.Add("@pid", ID, DbType.Int32, ParameterDirection.InputOutput);
            dyn.Add("@func", _func);
            dyn.Add("@rem", _remarks);
            dyn.Add("@tag", _tag);
            dyn.Add("@id", _id);
            dyn.Add("@sts", _sts);
            try
            {
                Connection.Instance.DB.Execute("sp_PutLog", dyn, null, null, CommandType.StoredProcedure);
                ID = dyn.Get<int>("@pid");
                pid = ID;
            }
            catch (Exception ex)
            {
                // fallback when error happends
                TextLogger t = new TextLogger();
                t.Write(ex, _user);
            }
            return ID;
        }

       
        public void Dispose()
        {
          
        }

       
    }
}