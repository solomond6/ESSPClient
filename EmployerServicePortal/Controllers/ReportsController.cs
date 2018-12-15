using EmployerServicePortal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployerServicePortal.Controllers
{
    public class ReportsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string ReportType, string ReportFormat, string startdate, string enddate)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string EMPLOYER_ID = (string)Session["EMPLOYER_ID"];
            string CompanyName = (string)Session["CompanyName"];

            try
            {
                if (ReportType == "1")
                {
                    if (ReportFormat == "PDF")
                    {
                        Employer.Employer employer = new Employer.Employer();
                        var report = employer.Report_CFI_Status_Report_By_Employer(EMPLOYER_ID, "PDF", userkey, uid, uid);

                        //var report = employer.Report_RSA_Details_by_employer(CompanyName, "PDF", userkey, uid, uid);
                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".pdf";
                            Response.AppendCookie(new HttpCookie("fileDownloadToken", "Report"));
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "application/pdf", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                    }
                    else if (ReportFormat == "CSV")
                    {
                        Employer.Employer employer = new Employer.Employer();
                        var report = employer.Report_CFI_Status_Report_By_Employer(EMPLOYER_ID, "csv", userkey, uid, uid);

                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".csv";
                            Response.AppendCookie(new HttpCookie("fileDownloadToken", "Report"));
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "text/csv", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                        
                    }
                    else
                    {
                        TempData["error"] = "File type not allowed";
                        return View();
                    }
                }
                else if (ReportType == "2")
                {
                    Employer.Employer employer = new Employer.Employer();

                    var report = employer.Report_Pencom_Employer_Code(EMPLOYER_ID, "PDF", userkey, uid, uid);
                    if (report != null)
                    {
                        string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                        string pth = stmt_name + ".pdf";
                        //FileStream stream = System.IO.File.Create(pth, report.Length);
                        //stream.Write(report, 0, report.Length);
                        //stream.Close();
                        return File(report, "application/pdf", pth);
                    }
                    else
                    {
                        TempData["error"] = "Report cannot be generated at the moment. Kindly contact System Admininistrator.";
                        return View();
                    }
                }
                else if (ReportType == "3"){
                    if (ReportFormat == "PDF")
                    {
                        Employer.Employer employer = new Employer.Employer();

                        var report = employer.Report_RSA_Details_by_employer(CompanyName, "PDF", userkey, uid, uid);
                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".pdf";
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "application/pdf", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                    }
                    else if (ReportFormat == "CSV")
                    {
                        Employer.Employer employer = new Employer.Employer();
                        var report = employer.Report_RSA_Details_by_employer(CompanyName, "csv", userkey, uid, uid);

                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".csv";
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "text/csv", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["error"] = "File type not allowed";
                        return View();
                    }
                }
                else if (ReportType == "4")
                {
                    if (ReportFormat == "PDF")
                    {
                        Employer.Employer employer = new Employer.Employer();

                        var report = employer.Report_RSA_Unfunded(CompanyName, startdate, enddate, "PDF", userkey, uid, uid);
                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".pdf";
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "application/pdf", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                    }
                    else if (ReportFormat == "CSV")
                    {
                        Employer.Employer employer = new Employer.Employer();
                        var report = employer.Report_RSA_Unfunded(CompanyName, startdate, enddate, "csv", userkey, uid, uid);

                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".csv";
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "text/csv", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["error"] = "File type not allowed";
                        return View();
                    }
                }
                else if (ReportType == "5")
                {
                    if (ReportFormat == "PDF")
                    {
                        Employer.Employer employer = new Employer.Employer();

                        var report = employer.Report_RSA_Unfunded(CompanyName, startdate, enddate, "PDF", userkey, uid, uid);
                        
                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".pdf";
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "application/pdf", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                    }
                    else if (ReportFormat == "CSV")
                    {
                        Employer.Employer employer = new Employer.Employer();
                        var report = employer.Report_RSA_Unfunded(CompanyName, startdate, enddate, "csv", userkey, uid, uid);

                        if (report != null)
                        {
                            string stmt_name = EMPLOYER_ID.ToString().Trim() + "_" + DateTime.Today.Day.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Year.ToString() + DateTime.Today.Second.ToString() + DateTime.Today.Millisecond.ToString() + DateTime.UtcNow.Ticks + EMPLOYER_ID;
                            string pth = stmt_name + ".csv";
                            //FileStream stream = System.IO.File.Create(pth, report.Length);
                            //stream.Write(report, 0, report.Length);
                            //stream.Close();
                            return File(report, "text/csv", pth);
                        }
                        else
                        {
                            TempData["error"] = "Report not found";
                            return View();
                        }
                    }
                    else
                    {
                        TempData["error"] = "File type not allowed";
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "ReportType not selected";
                    return View();
                }
                
            } catch(Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Reports/Index", "Reports", "Index", "Reports Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                return View();
            }
            
        }
    }
}