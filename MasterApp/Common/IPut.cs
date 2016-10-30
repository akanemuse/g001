using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterApp.Common
{
    public interface IPut : IDisposable
    {
        int Put(string _what,
            string _user = "",
            string _where = "",
            int ID = 0,
            string _func = "",
            string _remarks = null,
            int _tag = 0,
            string _id = "",
            string _sts = null);

    }
}