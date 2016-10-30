using MasterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterApp.Controllers
{
    public class BuildingOccupancyController : Controller
    {

        public ActionResult Index()
        {
            IEnumerable<BuildingOccupancyModel> ListMasterPoint = MasterPointRepository.Instance.GetListOccupancy();
            ViewData["ListMasterPoint"] = ListMasterPoint;
            return View(ListMasterPoint);
        }

        public ActionResult Refresh()
        {
            IEnumerable<BuildingOccupancyModel> ListMasterPoint = MasterPointRepository.Instance.GetListOccupancy();
            ViewData["ListMasterPoint"] = ListMasterPoint;
            //return View(ListMasterPoint);
            return PartialView("_BuildingOccupancyGrid", ListMasterPoint);
            //return RedirectToAction("Index", "BuildingOccupancy");
        }

        public ActionResult MasterPointPeople(MasterPointModel model)
        {
            return JavaScript(string.Format("window.location='{0}';", Url.Action("MasterPointPeople", "MasterPointPeople", model)));
            
        }
        public ActionResult PeopleHistory(MasterPointModel model)
        {
            return JavaScript(string.Format("window.location='{0}';", Url.Action("PeopleHistory", "PeopleHistory", model)));
            //return RedirectToAction("MasterPointDetail", "MasterPointDetail", new { ID = ID });
        }
    }
}
