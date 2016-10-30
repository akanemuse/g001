using MasterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterApp.Controllers
{
    public class OccupancyPeopleController : Controller
    {
        //
        // GET: /OccupancyPeople/

        public ActionResult Index(MasterPointModel model)
        {
            MasterPointModel updatedModel = MasterPointRepository.Instance.UpdateMasterPointAssembly(model);
            ViewData["MasterPointAssembly"] = updatedModel;
            return View();
        }

        public ActionResult Refresh(MasterPointModel model)
        {
            MasterPointModel updatedModel = MasterPointRepository.Instance.UpdateMasterPointAssembly(model);
            ViewData["MasterPointAssembly"] = updatedModel;
            return PartialView("_MasterPointPeopleAssemblyGrid");
        }

    }
}
