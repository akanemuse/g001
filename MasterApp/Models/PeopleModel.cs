using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterApp.Models
{
    public class PeopleModel
    {
        public string Seq { get; set; }
        public string TrData { get; set; }
        public string Name { get; set; }
        public string Jabatan { get; set; }
        public string Divisi { get; set; }
        public bool isAtAssemblyPoint { get; set; }
        ///new 
        ///
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }


    }

    public class GroupModel
    {
        public string Group { get; set; }
        public int GroupCount { get; set; }
    }

    public class PeopleHistory
    {
        public string TrData { get; set; }
        public string Name { get; set; }
        public string Divisi { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }

    }
}