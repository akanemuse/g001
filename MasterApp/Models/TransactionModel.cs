using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterApp.Models
{
    public class TransactionModel
    {
        public string SeqNo { get; set; }
        public string EL5KNo { get; set; }
        public string DevType { get; set; }
        public string DevId { get; set; }
        public string TrDate { get; set; }
        public string TrTime { get; set; }
        public string TrData { get; set; }
        public string TrCode { get; set; }
        public string Extra { get; set; }
        public string TrUser { get; set; }
        public string StaffNumber { get; set; }
        public string Jabatan { get; set; }
        public string Divisi { get; set; }
        public string Name { get; set; }
        //new for assembly point
        public string DataAssembly { get; set; }

    }
}