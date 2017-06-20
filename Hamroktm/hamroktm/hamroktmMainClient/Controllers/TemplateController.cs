using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hamroktmMainClient.Controllers
{
    public class TemplateController : Controller
    {
        //// GET: Template
        //public ActionResult Index()
        //{
        //    return View("Template/Template1");
        //}

        public ActionResult Template1()
        {
            return View("Template/Template1");
        }
    }
}