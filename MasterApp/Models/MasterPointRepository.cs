using MasterApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;
using System.Web.Hosting;
using System.Xml.Linq;

namespace MasterApp.Models
{
    public class MasterPointRepository
    {
        private MasterPointRepository() { }
        private static MasterPointRepository instance = null;
        public static MasterPointRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MasterPointRepository();
                }
                return instance;
            }
        }
        
        public List<MasterPointModel> GetListMasterPoint()
        {
            string path = HostingEnvironment.MapPath(@"~/SQL/config.xml");

            List<MasterPointModel> listMasterPoint = new List<MasterPointModel>();

            XElement xelement = XElement.Load(path);
            IEnumerable<XElement> MasterPointModels = xelement.Elements();

            foreach (var item in MasterPointModels)
            {
                MasterPointModel model = new MasterPointModel();
                model.ID = item.Element("ID").Value;
                model.AreaName = item.Element("AreaName").Value;
                model.LocationName = item.Element("LocationName").Value;
                model.InDeviceList = item.Element("InDeviceList").Value;
                model.OutDeviceList = item.Element("OutDeviceList").Value;
                MasterPointModel mdl = GetInformation(model);
                model.InsideCount = mdl.PeopleList.Count();
                model.GroupModelListByDiv = mdl.GroupModelListByDiv;
                model.GroupModelListByJab = mdl.GroupModelListByJab;
                model.PeopleList = mdl.PeopleList;
                model.LastIn = mdl.LastIn;
                model.LastOut = mdl.LastOut;

                model.Effect = item.Element("Effect").Value;
                listMasterPoint.Add(model);
            }

            return listMasterPoint;
        }

        public MasterPointModel UpdateMasterPoint(MasterPointModel model)
        {
            MasterPointModel updatedModel = new MasterPointModel();
            updatedModel = new MasterPointModel();
            updatedModel.ID = model.ID;
            updatedModel.AreaName = model.AreaName;
            updatedModel.LocationName = model.LocationName;
            updatedModel.InDeviceList = model.InDeviceList;
            updatedModel.OutDeviceList = model.OutDeviceList;

            MasterPointModel mdl = GetInformation(model);
            updatedModel.InsideCount = mdl.PeopleList.Count();
            updatedModel.GroupModelListByDiv = mdl.GroupModelListByDiv;
            updatedModel.GroupModelListByJab = mdl.GroupModelListByJab;
            updatedModel.PeopleList = mdl.PeopleList;
            updatedModel.LastIn = mdl.LastIn;
            updatedModel.LastOut = mdl.LastOut;

            updatedModel.Effect = model.Effect;
            updatedModel.GroupBy = model.GroupBy;
            updatedModel.CurrentGroup = model.CurrentGroup;

            return updatedModel;
        }

        public MasterPointModel GetInformation(MasterPointModel model)
        {
            //new for last in last out
            IEnumerable<LastInLastOut> LastInLastOut = Connection.Instance.Fetch<LastInLastOut>("GetLastInLastOut", new { InDevice = model.InDeviceList, OutDevice = model.OutDeviceList });

            //List<TransactionModel> listTransactionModel = new List<TransactionModel>();
            IEnumerable<TransactionModel> TransactionData = Connection.Instance.Fetch<TransactionModel>("GetTransactionData", new { InDevice = model.InDeviceList, OutDevice = model.OutDeviceList});
            
            String LastTransactionData = "";
            List<string> listInside = new List<string>();
            bool alreadyOut = false;

            foreach (TransactionModel item in TransactionData)
            {
                if (!item.TrData.Equals(LastTransactionData))
                {
                    alreadyOut = true;
                    LastTransactionData = item.TrData;
                    if (item.TrCode.Equals("0"))
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
                else
                {
                    if (item.TrCode.Equals("9"))
                    {
                        listInside.Remove(item.TrData);
                        alreadyOut = true;
                    }
                    else if (alreadyOut)
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
            }

            List<PeopleModel> listPeople = new List<PeopleModel>();
            foreach (var item in TransactionData.OrderBy(y=>y.TrData).ThenByDescending(m=>m.TrDate).ThenByDescending(k=>k.TrTime))
            {
                if (listInside.Contains(item.TrData))
                {
                    PeopleModel people = new PeopleModel();
                    people.Seq = item.SeqNo;
                    people.TrData = item.TrData;
                    people.Name = item.Name;
                    people.Divisi = item.Divisi.Trim();
                    people.Jabatan = item.Jabatan.Trim();
                    people.TimeIn = item.TrTime;
                    if (!listPeople.Any(p => p.TrData == item.TrData))
                    {
                        listPeople.Add(people);
                    }
                }
            };

            var groupedPeopleByDivisi = 
                from b in listPeople.AsEnumerable()
                group b by b.Divisi into g
                select new
                {
                    Group = g.Key,
                    GroupCount = g.Count()
                };

            List<GroupModel> groupByDiv = new List<GroupModel>();
            foreach (var item in groupedPeopleByDivisi)
            {
                GroupModel grpMdl = new GroupModel();
                grpMdl.Group = item.Group;
                grpMdl.GroupCount = item.GroupCount;
                groupByDiv.Add(grpMdl);
            }

            var groupedPeopleByJabatan = 
                from b in listPeople.AsEnumerable()
                group b by b.Jabatan into g
                select new
                {
                    Group = g.Key,
                    GroupCount = g.Count()
                };

            List<GroupModel> groupByJab = new List<GroupModel>();
            foreach (var item in groupedPeopleByJabatan)
            {
                GroupModel grpMdl = new GroupModel();
                grpMdl.Group = item.Group;
                grpMdl.GroupCount = item.GroupCount;
                groupByJab.Add(grpMdl);
            }

            MasterPointModel ret = new MasterPointModel();
            ret.GroupModelListByDiv = groupByDiv;
            ret.GroupModelListByJab = groupByJab;
            ret.PeopleList = listPeople.OrderByDescending(a => a.Seq).ToList();

            if (LastInLastOut != null && LastInLastOut.Count() > 0)
            {
                LastInLastOut lilo = LastInLastOut.FirstOrDefault();
                ret.LastIn = lilo.LastIn;
                ret.LastOut = lilo.LastOut;
            }

            return ret;
        }

        /// <summary>
        /// Used for assembly point
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MasterPointModel GetAssemblyInformation(MasterPointModel model)
        {
            IEnumerable<TransactionModel> TransactionData = Connection.Instance.Fetch<TransactionModel>("GetAssemblyPointData", new { InDevice = model.InDeviceList, OutDevice = model.OutDeviceList, AssemblyDevice = model.AssemblyDevice });

            String LastTransactionData = "";
            List<string> listInside = new List<string>();
            bool alreadyOut = false;

            foreach (TransactionModel item in TransactionData)
            {
                if (!item.TrData.Equals(LastTransactionData))
                {
                    alreadyOut = true;
                    LastTransactionData = item.TrData;
                    if (item.TrCode.Equals("0"))
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
                else
                {
                    if (item.TrCode.Equals("9"))
                    {
                        listInside.Remove(item.TrData);
                        alreadyOut = true;
                    }
                    else if (alreadyOut)
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
            }

            List<PeopleModel> listPeople = new List<PeopleModel>();
            foreach (var item in TransactionData.OrderBy(y => y.TrData).ThenByDescending(m => m.TrDate).ThenByDescending(k => k.TrTime))
            {
                if (listInside.Contains(item.TrData))
                {
                    PeopleModel people = new PeopleModel();
                    people.Seq = item.SeqNo;
                    people.TrData = item.TrData;
                    people.Name = item.Name;
                    people.Divisi = item.Divisi.Trim();
                    people.Jabatan = item.Jabatan.Trim();
                    people.isAtAssemblyPoint = !string.IsNullOrEmpty(item.DataAssembly);

                    if (!listPeople.Any(p => p.TrData == item.TrData))
                    {
                        listPeople.Add(people);
                    }
                }
            };


            MasterPointModel ret = new MasterPointModel();
            ret.PeopleList = listPeople.OrderByDescending(a => a.Seq).ToList();
            ret.PeopleInAssemblyPoint = listPeople.Where(a => a.isAtAssemblyPoint == true).Count();
            ret.PeopleStillInside = ret.PeopleList.Where(a => a.isAtAssemblyPoint == false).Count();

            return ret;
        }

        public List<MasterPointModel> GetListMasterPointAssembly()
        {
            string path = HostingEnvironment.MapPath(@"~/SQL/configAssembly.xml");

            List<MasterPointModel> listMasterPoint = new List<MasterPointModel>();

            XElement xelement = XElement.Load(path);
            IEnumerable<XElement> MasterPointModels = xelement.Elements();

            foreach (var item in MasterPointModels)
            {
                MasterPointModel model = new MasterPointModel();
                model.ID = item.Element("ID").Value;
                model.AreaName = item.Element("AreaName").Value;
                model.LocationName = item.Element("LocationName").Value;
                model.InDeviceList = item.Element("InDeviceList").Value;
                model.OutDeviceList = item.Element("OutDeviceList").Value;
                model.AssemblyDevice = item.Element("AssemblyDevice").Value;

                MasterPointModel mdl = GetAssemblyInformation(model);
                model.InsideCount = mdl.PeopleList.Count();
                model.PeopleList = mdl.PeopleList;
                model.PeopleInAssemblyPoint = mdl.PeopleInAssemblyPoint;
                model.PeopleStillInside = mdl.PeopleStillInside;

                model.Effect = item.Element("Effect").Value;
                listMasterPoint.Add(model);
            }

            return listMasterPoint;
        }

        public MasterPointModel UpdateMasterPointAssembly(MasterPointModel model)
        {
            MasterPointModel updatedModel = new MasterPointModel();
            updatedModel = new MasterPointModel();
            updatedModel.ID = model.ID;
            updatedModel.AreaName = model.AreaName;
            updatedModel.LocationName = model.LocationName;
            updatedModel.InDeviceList = model.InDeviceList;
            updatedModel.OutDeviceList = model.OutDeviceList;
            updatedModel.AssemblyDevice = model.AssemblyDevice;

            MasterPointModel mdl = GetAssemblyInformation(model);
            updatedModel.InsideCount = mdl.PeopleList.Count();
            updatedModel.PeopleList = mdl.PeopleList;

            return updatedModel;
        }

        ////20150416 building occupancy
        public List<BuildingOccupancyModel> GetListOccupancy()
        {
            string path = HostingEnvironment.MapPath(@"~/SQL/config.xml");

            List<BuildingOccupancyModel> listMasterPoint = new List<BuildingOccupancyModel>();

            XElement xelement = XElement.Load(path);
            IEnumerable<XElement> MasterPointModels = xelement.Elements();

            foreach (var item in MasterPointModels)
            {
                BuildingOccupancyModel model = new BuildingOccupancyModel();
                model.ID = item.Element("ID").Value;
                model.AreaName = item.Element("AreaName").Value;
                model.LocationName = item.Element("LocationName").Value;
                model.InDeviceList = item.Element("InDeviceList").Value;
                model.OutDeviceList = item.Element("OutDeviceList").Value;
                BuildingOccupancyModel mdl = GetInformationBuildingOccupancy(model);
                model.InsideCount = mdl.PeopleList.Count();
                model.EmployeeCount = mdl.EmployeeCount;
                model.GuestCount = mdl.GuestCount;
                model.PeopleList = mdl.PeopleList;

                listMasterPoint.Add(model);
            }

            return listMasterPoint;
        }

        public BuildingOccupancyModel GetInformationBuildingOccupancy(BuildingOccupancyModel model)
        {
            IEnumerable<TransactionModel> TransactionData = Connection.Instance.Fetch<TransactionModel>("GetTransactionData", new { InDevice = model.InDeviceList, OutDevice = model.OutDeviceList});

            String LastTransactionData = "";
            List<string> listInside = new List<string>();
            bool alreadyOut = false;

            foreach (TransactionModel item in TransactionData)
            {
                if (!item.TrData.Equals(LastTransactionData))
                {
                    alreadyOut = true;
                    LastTransactionData = item.TrData;
                    if (item.TrCode.Equals("0"))
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
                else
                {
                    if (item.TrCode.Equals("9"))
                    {
                        listInside.Remove(item.TrData);
                        alreadyOut = true;
                    }
                    else if (alreadyOut)
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
            }

            List<PeopleModel> listPeople = new List<PeopleModel>();
            foreach (var item in TransactionData.OrderBy(y => y.TrData).ThenByDescending(m => m.TrDate).ThenByDescending(k => k.TrTime))
            {
                if (listInside.Contains(item.TrData))
                {
                    PeopleModel people = new PeopleModel();
                    people.Seq = item.SeqNo;
                    people.TrData = item.TrData;
                    people.Name = item.Name;
                    people.Divisi = item.Divisi.Trim();
                    people.Jabatan = item.Jabatan.Trim();
                    people.isAtAssemblyPoint = !string.IsNullOrEmpty(item.DataAssembly);
                    if (!listPeople.Any(p => p.TrData == item.TrData))
                    {
                        listPeople.Add(people);
                    }
                }
            };

            BuildingOccupancyModel ret = new BuildingOccupancyModel();
            //ret.EmployeeCount = listPeople.Where(a => a.Divisi != "No Division").Count();
            //ret.GuestCount = listPeople.Where(a => a.Divisi == "No Division").Count();
            ret.EmployeeCount = listPeople.Where(a => a.Name != "GUEST").Count();
            ret.GuestCount = listPeople.Where(a => a.Name == "GUEST").Count();
            ret.PeopleList = listPeople.OrderByDescending(a => a.Seq).ToList();
            return ret;
        }

        /////// people history 
        public IEnumerable<PeopleHistory> GetPeopleHistory(MasterPointModel model)
        {
            /////get people inside
            IEnumerable<TransactionModel> TransactionData = Connection.Instance.Fetch<TransactionModel>("GetTransactionData", new { InDevice = model.InDeviceList, OutDevice = model.OutDeviceList });

            String LastTransactionData = "";
            List<string> listInside = new List<string>();
            bool alreadyOut = false;

            foreach (TransactionModel item in TransactionData)
            {
                if (!item.TrData.Equals(LastTransactionData))
                {
                    alreadyOut = true;
                    LastTransactionData = item.TrData;
                    if (item.TrCode.Equals("0"))
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
                else
                {
                    if (item.TrCode.Equals("9"))
                    {
                        listInside.Remove(item.TrData);
                        alreadyOut = true;
                    }
                    else if (alreadyOut)
                    {
                        listInside.Add(item.TrData);
                        alreadyOut = false;
                    }
                }
            }

            List<PeopleModel> listPeople = new List<PeopleModel>();
            foreach (var item in TransactionData.OrderBy(y => y.TrData).ThenByDescending(m => m.TrDate).ThenByDescending(k => k.TrTime))
            {
                if (listInside.Contains(item.TrData))
                {
                    PeopleModel people = new PeopleModel();
                    people.Seq = item.SeqNo;
                    people.TrData = item.TrData;
                    people.Name = item.Name;
                    people.Divisi = item.Divisi.Trim();
                    people.Jabatan = item.Jabatan.Trim();
                    people.TimeIn = item.TrTime;
                    if (!listPeople.Any(p => p.TrData == item.TrData))
                    {
                        listPeople.Add(people);
                    }
                }
            };

            List<PeopleHistory> peopleInside = new List<PeopleHistory>();
            foreach (var item in listPeople)
            {
                PeopleHistory people = new PeopleHistory();
                people.TrData = item.TrData;
                people.Divisi = item.Divisi;
                people.Name = item.Name;
                people.TimeIn = item.TimeIn;
                people.TimeOut = item.TimeOut;
                peopleInside.Add(people);
            }

            IEnumerable<PeopleHistory> PeopleHistory = Connection.Instance.Fetch<PeopleHistory>("GetPeopleHistory", new { InDevice = model.InDeviceList, OutDevice = model.OutDeviceList });
            
            PeopleHistory = PeopleHistory.Except(peopleInside, new PersonComparer());

            IEnumerable<PeopleHistory> totalPeople = peopleInside.OrderByDescending(m => m.TimeIn).Union(PeopleHistory, new PersonComparer());
            //List<PeopleHistory> totalPeople = (List<PeopleHistory>)PeopleHistory.Union(peopleInside, new PersonComparer());

            return totalPeople;
        }

    }

    public class PersonComparer : IEqualityComparer<PeopleHistory>
    {
        public bool Equals(PeopleHistory p1, PeopleHistory p2)
        {
            return p1.TrData == p2.TrData;
        }

        public int GetHashCode(PeopleHistory p)
        {
            return p.TrData.GetHashCode();
        }
    }
}