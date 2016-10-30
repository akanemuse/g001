using MasterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterApp.Controllers
{
    public class MasterPointDetailController : Controller
    {
        //
        // GET: /MasterPointDetail/

        public ActionResult MasterPointDetail(MasterPointModel model)
        {
            MasterPointModel updatedModel = MasterPointRepository.Instance.UpdateMasterPoint(model);
            ViewData["MasterPoint"] = updatedModel;
            return View();
        }

        public ActionResult Refresh(MasterPointModel model)
        {
            MasterPointModel updatedModel = MasterPointRepository.Instance.UpdateMasterPoint(model);
            ViewData["MasterPoint"] = updatedModel;
            //return PartialView("_MasterPointDetailLiveTile");
            return PartialView("_MasterPointDetailGrid");
        }

        public ActionResult MasterPointPeople(MasterPointModel model)
        {
            return JavaScript(string.Format("window.location='{0}';", Url.Action("MasterPointPeople", "MasterPointPeople", model)));
            //return RedirectToAction("MasterPointDetail", "MasterPointDetail", new { ID = ID });
        }

        public ActionResult PeopleHistory(MasterPointModel model)
        {
            return JavaScript(string.Format("window.location='{0}';", Url.Action("PeopleHistory", "PeopleHistory", model)));
            //return RedirectToAction("MasterPointDetail", "MasterPointDetail", new { ID = ID });
        }
    }
}
