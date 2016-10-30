using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterApp.Models;

namespace MasterApp.Controllers
{
    public class MasterPointController : Controller
    {
        //
        // GET: /MasterPoint/

        public ActionResult MasterPoint()
        {
            IEnumerable<MasterPointModel> listMasterPoint = MasterPointRepository.Instance.GetListMasterPoint();
            //ViewData["listMasterPoint"] = listMasterPoint;
            return View(listMasterPoint);
        }

        public ActionResult Refresh()
        {
            IEnumerable<MasterPointModel> listMasterPoint = MasterPointRepository.Instance.GetListMasterPoint();
            //ViewData["listMasterPoint"] = listMasterPoint;
            return PartialView("_MasterPointLiveTile", listMasterPoint);
        }

        public ActionResult MasterPointDetail(MasterPointModel model)
        {
            return JavaScript(string.Format("window.location='{0}';", Url.Action("MasterPointDetail", "MasterPointDetail", model)));
            //return RedirectToAction("MasterPointDetail", "MasterPointDetail", new { ID = ID });
        }

    }
}
