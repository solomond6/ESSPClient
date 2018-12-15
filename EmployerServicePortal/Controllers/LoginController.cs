using EmployerServicePortal.Models;
using EmployerServicePortal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EmployerServicePortal.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Users users, string ReturnUrl)
        {
            string LoginType;

            string ipaddress = Request.UserHostAddress;
            string Agent = Request.UserAgent;
            string BrowserUsed = Request.Browser.Browser;
            string SessionID = HttpContext.Session.SessionID;

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            try
            {
                List<LoginStatus> loginStatus;

                Employer.Employer employer = new Employer.Employer();

                if((users.Username.Trim().Length >= 5) && (users.Username.Trim().Contains("@") == true) && (users.Username.Trim().Contains(".") == true))
                {
                    LoginType = "2";
                    DataTable dt = employer.ExternalLogin(users.Username, LoginType, users.Password, BrowserUsed, SessionID, Agent, ipaddress, userkey, uid);
                    dt.TableName = "ReturnedVal";

                    var response = dt.Columns.Count.ToString();

                    if (response == "2")
                    {
                        var loginError = (from DataRow dr in dt.Rows
                                          select new LoginStatus()
                                          {
                                              ErrorMessage = dr["ErrorMessage"].ToString(),
                                              LoginStat = dr["LoginStat"].ToString(),
                                          }).ToList();

                        TempData["error"] = loginError[0].ErrorMessage;
                        return View();
                    }

                    loginStatus = (from DataRow dr in dt.Rows
                                    select new LoginStatus()
                                    {
                                        ErrorMessage = dr["ErrorMessage"].ToString(),
                                        LoginStat = dr["LoginStat"].ToString(),
                                        WebUserID = dr["WebUserID"].ToString(),
                                        EMPLOYER_ID = dr["EMPLOYER_ID"].ToString(),
                                        EnforceChange = dr["EnforceChange"].ToString(),
                                        CUSTODIAN_ID = dr["CUSTODIAN_ID"].ToString(),
                                        LastLogin = dr["LastLogin"].ToString(),
                                        email = dr["email"].ToString(),
                                        BrowserUsed = dr["BrowserUsed"].ToString(),
                                        ROLE_ID = dr["ROLE_ID"].ToString(),
                                        FULLNAME = dr["FULLNAME"].ToString(),
                                    }).ToList();

                    if (loginStatus[0].EnforceChange == "Y")
                    {
                        ViewBag.Email = loginStatus[0].email;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        Session["ROLE_ID"] = loginStatus[0].ROLE_ID;
                        return RedirectToAction("ChangePassword");
                    }
                    else if (loginStatus[0].LoginStat == "False")
                    {
                        TempData["error"] = loginStatus[0].ErrorMessage;
                        return View();
                    }
                    else if(loginStatus[0].ROLE_ID == "3")
                    {
                        FormsAuthentication.SetAuthCookie(loginStatus[0].FULLNAME, false);
                        Session["LoginSAPID"] = users.Username;
                        DateTime dateTime10 = Convert.ToDateTime(loginStatus[0].LastLogin);
                        Session["LastLogin"] = dateTime10.ToString("dd-MMM-yyyy hh:mm");
                        Session["EMPLOYER_ID"] = loginStatus[0].EMPLOYER_ID;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        Session["ROLE_ID"] = loginStatus[0].ROLE_ID;

                        DataTable dts = employer.FetchCompanYEmployees(loginStatus[0].EMPLOYER_ID, userkey, uid);
                        dts.TableName = "CompanyEmployees";
                        var companyEmployee = (from DataRow dr in dts.Rows
                                               select new
                                               {
                                                   Coyname = dr["Coyname"].ToString()
                                               }).ToList();

                        Session["CompanyName"] = companyEmployee[0].Coyname;

                        DataTable dtz = employer.FetchEmployerContact(loginStatus[0].EMPLOYER_ID);
                        dtz.TableName = "EmpoyerContact";
                        dtz.Columns.ToString();

                        var EmpoyerContact = (from DataRow dr in dtz.Rows
                                              select new
                                              {
                                                  Email = dr["Email"].ToString(),
                                                  Address = dr["Address"].ToString(),
                                                  MobilePhone = dr["Mobile Phone"].ToString(),
                                                  Name = dr["Name"].ToString()
                                              }).ToList();

                        Session["EmpoyerContactName"] = EmpoyerContact[0].Name;
                        Session["EmpoyerContactEmail"] = EmpoyerContact[0].Email;
                        Session["EmpoyerContactAddress"] = EmpoyerContact[0].Address;
                        Session["EmpoyerContactMobilePhone"] = EmpoyerContact[0].MobilePhone;

                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else if (loginStatus[0].ROLE_ID == "4")
                    {
                        FormsAuthentication.SetAuthCookie(loginStatus[0].FULLNAME, false);
                        Session["LoginSAPID"] = users.Username;
                        DateTime dateTime10 = Convert.ToDateTime(loginStatus[0].LastLogin);
                        Session["LastLogin"] = dateTime10.ToString("dd-MMM-yyyy hh:mm");
                        Session["CUSTODIAN_ID"] = loginStatus[0].CUSTODIAN_ID;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        Session["ROLE_ID"] = loginStatus[0].ROLE_ID;

                        DataTable dts = employer.FetchCompanYEmployees(loginStatus[0].EMPLOYER_ID, userkey, uid);
                        dts.TableName = "CompanyEmployees";
                        var companyEmployee = (from DataRow dr in dts.Rows
                                               select new
                                               {
                                                   Coyname = dr["Coyname"].ToString()
                                               }).ToList();
                        Session["CompanyName"] = companyEmployee[0].Coyname;

                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Employee");
                        }
                    }
                    else if (loginStatus[0].ROLE_ID == "7")
                    {
                        FormsAuthentication.SetAuthCookie(loginStatus[0].FULLNAME, false);
                        Session["LoginSAPID"] = users.Username;
                        DateTime dateTime10 = Convert.ToDateTime(loginStatus[0].LastLogin);
                        Session["LastLogin"] = dateTime10.ToString("dd-MMM-yyyy hh:mm");
                        Session["EMPLOYER_ID"] = loginStatus[0].EMPLOYER_ID;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        Session["ROLE_ID"] = loginStatus[0].ROLE_ID;

                        DataTable dts = employer.FetchCompanYEmployees(loginStatus[0].EMPLOYER_ID, userkey, uid);
                        dts.TableName = "CompanyEmployees";
                        var companyEmployee = (from DataRow dr in dts.Rows
                                               select new 
                                               {
                                                   Coyname = dr["Coyname"].ToString()
                                               }).ToList();
                        Session["CompanyName"] = companyEmployee[0].Coyname;

                        DataTable dtz = employer.FetchEmployerContact(loginStatus[0].EMPLOYER_ID);
                        dtz.TableName = "EmpoyerContact";
                        dtz.Columns.ToString();

                        var EmpoyerContact = (from DataRow dr in dtz.Rows
                                               select new
                                               {
                                                   Email = dr["Email"].ToString(),
                                                   Address = dr["Address"].ToString(),
                                                   MobilePhone = dr["Mobile Phone"].ToString(),
                                                   Name = dr["Name"].ToString()
                                               }).ToList();

                        Session["EmpoyerContactName"] = EmpoyerContact[0].Name;
                        Session["EmpoyerContactEmail"] = EmpoyerContact[0].Email;
                        Session["EmpoyerContactAddress"] = EmpoyerContact[0].Address;
                        Session["EmpoyerContactMobilePhone"] = EmpoyerContact[0].MobilePhone;

                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else if((users.Username.Trim().Length >= 10) && ((users.Username.Trim().All(char.IsDigit)) == true))
                {
                    LoginType = "1";
                    DataTable dt = employer.ExternalLogin(users.Username, LoginType, users.Password, BrowserUsed, SessionID, Agent, ipaddress, userkey, uid);
                    dt.TableName = "ReturnedVal";
                    var response = dt.Columns.Count.ToString();

                    if (response == "2")
                    {
                        var loginError = (from DataRow dr in dt.Rows
                                          select new LoginStatus()
                                          {
                                              ErrorMessage = dr["ErrorMessage"].ToString(),
                                              LoginStat = dr["LoginStat"].ToString(),
                                          }).ToList();

                        TempData["error"] = loginError[0].ErrorMessage;
                        return View();
                    }

                    loginStatus = (from DataRow dr in dt.Rows
                                   select new LoginStatus()
                                   {
                                       ErrorMessage = dr["ErrorMessage"].ToString(),
                                       LoginStat = dr["LoginStat"].ToString(),
                                       WebUserID = dr["WebUserID"].ToString(),
                                       EMPLOYER_ID = dr["EMPLOYER_ID"].ToString(),
                                       EnforceChange = dr["EnforceChange"].ToString(),
                                       CUSTODIAN_ID = dr["CUSTODIAN_ID"].ToString(),
                                       LastLogin = dr["LastLogin"].ToString(),
                                       email = dr["email"].ToString(),
                                       BrowserUsed = dr["BrowserUsed"].ToString(),
                                       ROLE_ID = dr["ROLE_ID"].ToString(),
                                       FULLNAME = dr["FULLNAME"].ToString()
                                   }).ToList();

                    if(loginStatus[0].EnforceChange == "Y")
                    {
                        ViewBag.Email = loginStatus[0].email;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        return RedirectToAction("ChangePassword");
                    }
                    else if (loginStatus[0].LoginStat == "False")
                    {
                        ViewBag.Email = loginStatus[0].email;
                        return View();
                    }
                    else if (loginStatus[0].LoginStat == "False")
                    {
                        TempData["error"] = loginStatus[0].ErrorMessage;
                        return View();
                    }
                    else if (loginStatus[0].ROLE_ID == "3")
                    {
                        FormsAuthentication.SetAuthCookie(loginStatus[0].FULLNAME, false);
                        Session["LoginSAPID"] = users.Username;
                        DateTime dateTime10 = Convert.ToDateTime(loginStatus[0].LastLogin);
                        Session["LastLogin"] = dateTime10.ToString("dd-MMM-yyyy hh:mm");
                        Session["EMPLOYER_ID"] = loginStatus[0].EMPLOYER_ID;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        Session["ROLE_ID"] = loginStatus[0].ROLE_ID;

                        DataTable dts = employer.FetchCompanYEmployees(loginStatus[0].EMPLOYER_ID, userkey, uid);
                        dts.TableName = "CompanyEmployees";
                        var companyEmployee = (from DataRow dr in dts.Rows
                                               select new
                                               {
                                                   Coyname = dr["Coyname"].ToString()
                                               }).ToList();
                        Session["CompanyName"] = companyEmployee[0].Coyname;

                        DataTable dtz = employer.FetchEmployerContact(loginStatus[0].EMPLOYER_ID);
                        dtz.TableName = "EmpoyerContact";
                        dtz.Columns.ToString();

                        var EmpoyerContact = (from DataRow dr in dtz.Rows
                                              select new
                                              {
                                                  Email = dr["Email"].ToString(),
                                                  Address = dr["Address"].ToString(),
                                                  MobilePhone = dr["Mobile Phone"].ToString(),
                                                  Name = dr["Name"].ToString()
                                              }).ToList();

                        Session["EmpoyerContactName"] = EmpoyerContact[0].Name;
                        Session["EmpoyerContactEmail"] = EmpoyerContact[0].Email;
                        Session["EmpoyerContactAddress"] = EmpoyerContact[0].Address;
                        Session["EmpoyerContactMobilePhone"] = EmpoyerContact[0].MobilePhone;

                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else if (loginStatus[0].ROLE_ID == "4")
                    {
                        FormsAuthentication.SetAuthCookie(loginStatus[0].FULLNAME, false);
                        Session["LoginSAPID"] = users.Username;
                        DateTime dateTime10 = Convert.ToDateTime(loginStatus[0].LastLogin);
                        Session["LastLogin"] = dateTime10.ToString("dd-MMM-yyyy hh:mm");
                        Session["CUSTODIAN_ID"] = loginStatus[0].CUSTODIAN_ID;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        Session["ROLE_ID"] = loginStatus[0].ROLE_ID;

                        DataTable dts = employer.FetchCompanYEmployees(loginStatus[0].EMPLOYER_ID, userkey, uid);
                        dts.TableName = "CompanyEmployees";
                        var companyEmployee = (from DataRow dr in dts.Rows
                                               select new
                                               {
                                                   Coyname = dr["Coyname"].ToString()
                                               }).ToList();
                        Session["CompanyName"] = companyEmployee[0].Coyname;

                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Employee");
                        }
                    }
                    else if (loginStatus[0].ROLE_ID == "7")
                    {
                        FormsAuthentication.SetAuthCookie(loginStatus[0].FULLNAME, false);
                        Session["LoginSAPID"] = users.Username;
                        DateTime dateTime10 = Convert.ToDateTime(loginStatus[0].LastLogin);
                        Session["LastLogin"] = dateTime10.ToString("dd-MMM-yyyy hh:mm");
                        Session["EMPLOYER_ID"] = loginStatus[0].EMPLOYER_ID;
                        Session["WebUserID"] = loginStatus[0].WebUserID;
                        Session["ROLE_ID"] = loginStatus[0].ROLE_ID;

                        DataTable dts = employer.FetchCompanYEmployees(loginStatus[0].EMPLOYER_ID, userkey, uid);
                        dts.TableName = "CompanyEmployees";
                        var companyEmployee = (from DataRow dr in dts.Rows
                                               select new
                                               {
                                                   Coyname = dr["Coyname"].ToString()
                                               }).ToList();
                        Session["CompanyName"] = companyEmployee[0].Coyname;

                        DataTable dtz = employer.FetchEmployerContact(loginStatus[0].EMPLOYER_ID);
                        dtz.TableName = "EmpoyerContact";
                        dtz.Columns.ToString();

                        var EmpoyerContact = (from DataRow dr in dtz.Rows
                                              select new
                                              {
                                                  Email = dr["Email"].ToString(),
                                                  Address = dr["Address"].ToString(),
                                                  MobilePhone = dr["Mobile Phone"].ToString(),
                                                  Name = dr["Name"].ToString()
                                              }).ToList();

                        Session["EmpoyerContactName"] = EmpoyerContact[0].Name;
                        Session["EmpoyerContactEmail"] = EmpoyerContact[0].Email;
                        Session["EmpoyerContactAddress"] = EmpoyerContact[0].Address;
                        Session["EmpoyerContactMobilePhone"] = EmpoyerContact[0].MobilePhone;

                        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/") && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog(users.Username, "", "", "Login/Index", "Login", "Index", "ExternalLogin Error", ex.Message.ToString(), 0); 
                return View();
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Username)
        {
            string LoginType;
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                string ipaddress = Request.UserHostAddress;
                string Agent = Request.UserAgent;
                string BrowserUsed = Request.Browser.Browser;

                Employer.Employer employer = new Employer.Employer();

                if ((Username.Trim().Length >= 5) && (Username.Trim().Contains("@") == true) && (Username.Trim().Contains(".") == true))
                {
                    LoginType = "2";
                    var LoginVal = Username;
                    DataTable dt = employer.ResetPassword(Username, LoginType, BrowserUsed, uid, userkey, uid);
                    dt.TableName = "ReturnedVal";

                    var response = dt.Columns.Count.ToString();

                    if (response == "2")
                    {
                        var erroStatus = (from DataRow dr in dt.Rows
                                            select new ForgotPassword()
                                            {
                                                Message = dr["Message"].ToString(),
                                                LoginStat = dr["LoginStat"].ToString(),
                                            }).ToList();
                        TempData["error"] = erroStatus[0].Message;
                        return View();
                    }

                    var forgotStatus = (from DataRow dr in dt.Rows
                                                         select new ForgotPassword()
                                                         {
                                                             Message = dr["Message"].ToString(),
                                                             LoginStat = dr["LoginStat"].ToString(),
                                                             RSAPIN = dr["RSAPIN"].ToString()
                                                         }).ToList();

                    TempData["sMsg"] = forgotStatus[0].Message;
                    return RedirectToAction("Index");

                }
                else if ((Username.Trim().Length >= 10) && ((Username.Trim().All(char.IsDigit)) == true))
                {
                    LoginType = "1";
                    var LoginVal = Username;
                    DataTable dt = employer.ResetPassword(Username, LoginType, BrowserUsed, uid, userkey, uid);
                    dt.TableName = "ReturnedVal";

                    var response = dt.Columns.Count.ToString();

                    if (response == "2")
                    {
                        var erroStatus = (from DataRow dr in dt.Rows
                                          select new ForgotPassword()
                                          {
                                              Message = dr["Message"].ToString(),
                                              LoginStat = dr["LoginStat"].ToString(),
                                          }).ToList();
                        TempData["error"] = erroStatus[0].Message;
                        return View();
                    }

                    var forgotStatus = (from DataRow dr in dt.Rows
                                        select new ForgotPassword()
                                        {
                                            Message = dr["Message"].ToString(),
                                            LoginStat = dr["LoginStat"].ToString(),
                                            RSAPIN = dr["RSAPIN"].ToString()
                                        }).ToList();

                    TempData["sMsg"] = forgotStatus[0].Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Email/Password not found";
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog(Username, "", "", "Login/ForgotPassword", "Login", "ForgotPassword", "ResetPassword Error", ex.Message.ToString(), 0);
                return View();
            }
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string WebUserID, string oldpassword, string newpassword)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            string ipaddress = Request.UserHostAddress;
            string Agent = Request.UserAgent;
            string BrowserUsed = Request.Browser.Browser;

            try
            {
                Employer.Employer employer = new Employer.Employer();
                var changeStatus = employer.ChangePassword(WebUserID, BrowserUsed, WebUserID, newpassword, uid, userkey, uid);

                var changeDetails = changeStatus.Split('~');

                if (changeDetails[0] != "4")
                {
                    TempData["error"] = changeDetails[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = changeDetails[1];
                    return RedirectToAction("Index");
                }
            }catch(Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog(WebUserID, "", "", "Login/ChangePassword", "Login", "ChangePassword", "ChangePassword Error", ex.Message.ToString(), 0);
                return View();
            }
            
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [Authorize]
        public JsonResult GetMenuItems()
        {

            try
            {
                var menuItems = Session["menuView"];
                return Json(new { data = menuItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Employer/GetCompanyEmployee", "Employer", "GetCompanyEmployee", "GetCompanyEmployee Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}