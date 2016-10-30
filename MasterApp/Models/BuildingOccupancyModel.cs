using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MasterApp.Models
{
    [Serializable]
    public class BuildingOccupancyModel
    {
        public string ID { get; set; }
        public string AreaName { get; set; }
        public string LocationName { get; set; }
        public string InDeviceList { get; set; }
        public string OutDeviceList { get; set; }
        public int InsideCount { get; set; }
        public List<PeopleModel> PeopleList { get; set; }
        public int EmployeeCount { get; set; }
        public int GuestCount { get; set; }
       
    }
}