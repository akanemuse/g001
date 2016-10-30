using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterApp.Common
{
    public interface ISay: IDisposable
    {
        int Say(string loc, string w, params object[] x);
    }
}
