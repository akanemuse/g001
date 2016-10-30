using MasterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterApp.Controllers
{
    public class OccupancyController : Controller
    {
        //
        // GET: /Occupancy/

        public ActionResult Index()
        {
            IEnumerable<MasterPointModel> listMasterPointAssembly = MasterPointRepository.Instance.GetListMasterPointAssembly();
            return View(listMasterPointAssembly);
        }

        public ActionResult Refresh()
        {
            IEnumerable<MasterPointModel> listMasterPointAssembly = MasterPointRepository.Instance.GetListMasterPointAssembly();
            return PartialView("_OccupancyGrid", listMasterPointAssembly);
        }

        public ActionResult MasterPointAssemblyPeople(MasterPointModel model)
        {
            return JavaScript(string.Format("window.location='{0}';", Url.Action("Index", "OccupancyPeople", model)));
        }

    }
}
