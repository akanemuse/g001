using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MasterApp.Models
{
    [Serializable]
    public class MasterPointModel
    {
        public string ID { get; set; }
        public string AreaName { get; set; }
        public string LocationName { get; set; }
        public string InDeviceList { get; set; }
        public string OutDeviceList { get; set; }
        public string Effect { get; set; }
        public string GroupBy { get; set; }
        public int InsideCount { get; set; }
        public List<GroupModel> GroupModelListByDiv { get; set; }
        public List<GroupModel> GroupModelListByJab { get; set; }
        public List<PeopleModel> PeopleList { get; set; }
        //new for assembly point
        public string AssemblyDevice { get; set; }
        public int PeopleInAssemblyPoint { get; set; }
        public int PeopleStillInside { get; set; }
        //for new req
        public string CurrentGroup { get; set; }
        //20150416
        public string LastIn { get; set; }
        public string LastOut { get; set; }
    }

    public class LastInLastOut
    {
        public String LastIn { get; set; }
        public String LastOut { get; set; }
    }
}