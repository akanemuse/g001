using MasterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterApp.Controllers
{
    public class PeopleHistoryController : Controller
    {
        //
        // GET: /PeopleHistory/

        public ActionResult PeopleHistory(MasterPointModel model)
        {
            IEnumerable<PeopleHistory> PeopleHistory = MasterPointRepository.Instance.GetPeopleHistory(model);
            ViewData["PeopleHistory"] = PeopleHistory;
            return View(PeopleHistory);
        }

    }
}
