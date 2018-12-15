using EmployerServicePortal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployerServicePortal.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public JsonResult GetEmployerData()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];

            try
            {

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchCompanYEmployees(EmployerId, userkey, uid);
                dt.TableName = "ExternalUsers";

                dt.Columns.ToString();

                var employeeCount = (from DataRow dr in dt.Rows
                                     select new
                                     {
                                         PIN = dr["P_I_N"].ToString()
                                     }).ToList();
                var totalCount = employeeCount.Count();

                var CFIStatusCount = (from DataRow dr in dt.Rows
                                      group dr by dr["CFI Status"] into g
                                      select new
                                      {
                                          Mid = g.Key,
                                          Count = g.Count()
                                      }).ToList();

                var cfiCount = CFIStatusCount.Count();


                var sexCount = (from DataRow dr in dt.Rows
                                group dr by dr["SEX"] into g
                                select new
                                {
                                    Mid = g.Key,
                                    Count = g.Count()
                                }).ToList();

                var stateCount = (from DataRow dr in dt.Rows
                                  group dr by dr["State of Origin"] into g
                                  select new
                                  {
                                      Mid = g.Key,
                                      Count = g.Count()
                                  }).ToList();

                var religionCount = (from DataRow dr in dt.Rows
                                     group dr by dr["Religion"] into g
                                     select new
                                     {
                                         Mid = g.Key,
                                         Count = g.Count()
                                     }).ToList();

                var maritalStatus = (from DataRow dr in dt.Rows
                                     group dr by dr["Marital Status"] into g
                                     select new
                                     {
                                         Mid = g.Key,
                                         Count = g.Count()
                                     }).ToList();

                var statementOption = (from DataRow dr in dt.Rows
                                       group dr by dr["translateddelivery"] into g
                                       select new
                                       {
                                           Mid = g.Key,
                                           Count = g.Count()
                                       }).ToList();

                return Json(new { employeeCount = totalCount, statement = statementOption, cfi = CFIStatusCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}