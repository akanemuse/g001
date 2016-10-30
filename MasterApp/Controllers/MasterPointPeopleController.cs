using MasterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterApp.Controllers
{
    public class MasterPointPeopleController : Controller
    {
        //
        // GET: /MasterPointPeople/

        public ActionResult MasterPointPeople(MasterPointModel model)
        {
            MasterPointModel updatedModel = MasterPointRepository.Instance.UpdateMasterPoint(model);
            ViewData["MasterPoint"] = updatedModel;
            return View();
        }

        public ActionResult Refresh(MasterPointModel model)
        {
            MasterPointModel updatedModel = MasterPointRepository.Instance.UpdateMasterPoint(model);
            ViewData["MasterPoint"] = updatedModel;
            return PartialView("_MasterPointPeopleGrid");
        }

    }
}
