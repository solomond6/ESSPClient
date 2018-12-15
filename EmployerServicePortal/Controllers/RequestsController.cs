using DataTables.Mvc;
using EmployerServicePortal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using EmployerServicePortal.Models;

namespace EmployerServicePortal.Controllers
{
    public class RequestsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public JsonResult GetRequests([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            string WebUserID = (string)Session["WebUserID"];
            string EMPLOYER_ID = (string)Session["EMPLOYER_ID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchIncidents("6", EMPLOYER_ID, "N/A", userkey, uid);
                dt.TableName = "Requests";
                dt.Columns.ToString();

                List<Requests> requests = new List<Requests>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                //DateTime dateTime10 = Convert.ToDateTime(loginStatus[0].LastLogin);
                //Session["LastLogin"] = dateTime10.ToString("dd-MMM-yyyy hh:mm");

                List<Requests> requestCount = (from DataRow dr in dt.Rows
                                                select new Requests()
                                                {
                                                    RequestID = dr["RequestID"].ToString()
                                                }).ToList();

                requests = (from DataRow dr in dt.Rows
                            orderby dr["Datecreated"] descending
                            select new Requests()
                            {
                                ID = dr["RequestID"].ToString(),
                                RequestID = dr["RequestID"].ToString(),
                                Category = dr["Cat"].ToString(),
                                Comment = dr["Comment"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CurrentStatus = dr["CurrentStatus"].ToString(),
                                CurrentAssignedToID = dr["CurrentAssignedToID"].ToString(),
                                CurrentAssignedRoleID = dr["CurrentAssignedRoleID"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requests.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                     where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                        || dr["Datecreated"].ToString().ToLower().Contains(value) || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                    select new Requests()
                                     {
                                         RequestID = dr["RequestID"].ToString()
                                     }).ToList();

                    requests = (from DataRow dr in dt.Rows
                                orderby dr["Datecreated"] descending
                                where dr["RequestID"].ToString().ToLower().Contains(value) || dr["Cat"].ToString().ToLower().Contains(value)
                                 || Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm").ToLower().Contains(value)
                                 || dr["CurrentStatus"].ToString().ToLower().Contains(value)
                                select new Requests()
                                {
                                    RequestID = dr["RequestID"].ToString(),
                                    Category = dr["Cat"].ToString(),
                                    Comment = dr["Comment"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CurrentStatus = dr["CurrentStatus"].ToString(),
                                    CurrentAssignedToID = dr["CurrentAssignedToID"].ToString(),
                                    CurrentAssignedRoleID = dr["CurrentAssignedRoleID"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requests.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = requests.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    Category = emList.Category,
                    Comment = emList.Comment,
                    CurrentStatus = emList.CurrentStatus,
                    Datecreated = emList.Datecreated,
                    CurrentAssignedToID = emList.CurrentAssignedToID,
                    CurrentAssignedRoleID = emList.CurrentAssignedRoleID
                }).OrderBy(orderByString == string.Empty ? "ID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetRequestsHistory([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String RequestId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                string dRequestId = CryptoEngine.DecryptString(RequestId, _access_key);
                string request_ID = dRequestId.Replace(' ', '+');

                DataTable dt = employer.FetchAssignmentHistory(dRequestId, userkey, uid);
                dt.TableName = "RequestsHistory";
                dt.Columns.ToString();

                List<RequestsHistory> requestsHistory = new List<RequestsHistory>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<RequestsHistory> requestCount = (from DataRow dr in dt.Rows
                                                      select new RequestsHistory()
                                                      {
                                                          RequestID = dr["RequestID"].ToString()
                                                      }).ToList();

                requestsHistory = (from DataRow dr in dt.Rows
                                   orderby dr["AssignDate"] descending
                                   select new RequestsHistory()
                                   {
                                       RequestID = dr["RequestID"].ToString(),
                                       HistoryID = dr["HistoryID"].ToString(),
                                       Comment = dr["Comment"].ToString(),
                                       Assignee = dr["Assignee"].ToString(),
                                       Assignor = dr["Assignor"].ToString(),
                                       AssignDate = Convert.ToDateTime(dr["AssignDate"]).ToString("dd-MMM-yyyy hh:mm"),
                                       AssignStatus = dr["AssignStatus"].ToString()
                                   }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = requestCount.Count();
                var filteredCount = requestsHistory.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();

                    requestCount = (from DataRow dr in dt.Rows
                                    where dr["RequestID"].ToString().Contains(value) || dr["Assignee"].ToString().Contains(value)
                                       || dr["Assignor"].ToString().Contains(value) || Convert.ToDateTime(dr["AssignDate"]).ToString("dd-MMM-yyyy hh:mm").Contains(value)
                                       || dr["Assignee"].ToString().Contains(value) || dr["AssignStatus"].ToString().Contains(value)
                                    select new RequestsHistory()
                                    {
                                        RequestID = dr["RequestID"].ToString()
                                    }).ToList();

                    requestsHistory = (from DataRow dr in dt.Rows
                                       orderby dr["AssignDate"] descending
                                       where dr["RequestID"].ToString().Contains(value) || dr["Assignee"].ToString().Contains(value)
                                               || dr["Assignor"].ToString().Contains(value) || Convert.ToDateTime(dr["AssignDate"]).ToString("dd-MMM-yyyy hh:mm").Contains(value)
                                               || dr["Assignee"].ToString().Contains(value) || dr["AssignStatus"].ToString().Contains(value)
                                       select new RequestsHistory()
                                       {
                                           RequestID = dr["RequestID"].ToString(),
                                           HistoryID = dr["HistoryID"].ToString(),
                                           Comment = dr["Comment"].ToString(),
                                           Assignee = dr["Assignee"].ToString(),
                                           Assignor = dr["Assignor"].ToString(),
                                           AssignDate = Convert.ToDateTime(dr["AssignDate"]).ToString("dd-MMM-yyyy hh:mm"),
                                           AssignStatus = dr["AssignStatus"].ToString()
                                       }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = requestCount.Count();
                    filteredCount = requestsHistory.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = requestsHistory.Select(emList => new
                {
                    RequestID = emList.RequestID,
                    HistoryID = emList.HistoryID,
                    Comment = emList.Comment,
                    Assignee = emList.Assignee,
                    Assignor = emList.Assignor,
                    AssignDate = emList.AssignDate,
                    AssignStatus = emList.AssignStatus
                }).OrderBy(orderByString == string.Empty ? "ID asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetInternalUsers()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {

                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchInternalUsers(userkey, uid);
                dt.TableName = "InternalUsers";
                dt.Columns.ToString();

                var users = (from DataRow dr in dt.Rows
                             select new Models.InternalUsers()
                             {
                                 ID = dr["ID"].ToString(),
                                 FULLNAME = dr["FULLNAME"].ToString(),
                                 ROLE_ID = dr["ROLE_ID"].ToString()
                             }).ToList();

                return Json(new { data = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/GetInternalUsers", "Requests", "GetInternalUsers", "FetchInternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult Details(string RequestId)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            ViewBag.RequestId = RequestId;
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult Create(string requestCategory, string Comment, string convertedFile, string convertedFile1, string convertedFile2)
        {
            string UploadStatus = "";
            string UploadStatus1 = "";
            string UploadStatus2 = "";
            //convertedFile = "";
            //convertedFile1 = "";
            //convertedFile2 = "";

            string ipaddress = Request.UserHostAddress;
            string Agent = Request.UserAgent;
            string BrowserUsed = Request.Browser.Browser;
            string SessionID = HttpContext.Session.SessionID;

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["ROLE_ID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                var requestStatus = employer.CreateRequest(WebUserID, RoleID, requestCategory, "Initiated", Comment, userkey, uid);
                var requestDetails = requestStatus.Split('~');

                if (requestDetails[0] != "01")
                {
                    var file = Request.Files;

                    if (file[0].ContentLength > 0)
                    {
                        string _FileName = file[0].FileName;
                        string Docext = Path.GetExtension(file[0].FileName);
                        string Doc = convertedFile;
                        UploadStatus = employer.LogRequestDoc(requestDetails[1], "", Doc, Docext, _FileName, "Y", userkey, uid);
                        var uploadDetails = UploadStatus.Split('~');
                    }
                    if (file[1].ContentLength > 0)
                    {
                        string _FileName1 = file[1].FileName;
                        string Docext1 = Path.GetExtension(file[1].FileName);
                        string Doc1 = convertedFile1;
                        UploadStatus1 = employer.LogRequestDoc(requestDetails[1], "", Doc1, Docext1, _FileName1, "Y", userkey, uid);
                        //var uploadDetails1 = UploadStatus1.Split('~');
                    }
                    if (file[2].ContentLength > 0)
                    {
                        string _FileName2 = file[2].FileName;
                        string Docext2 = Path.GetExtension(file[2].FileName);
                        string Doc2 = convertedFile2;
                        UploadStatus2 = employer.LogRequestDoc(requestDetails[1], "", Doc2, Docext2, _FileName2, "Y", userkey, uid);
                        //uploadDetails2 = UploadStatus2.Split('~');
                    }

                    if (UploadStatus == "00~Your documents have been logged successfully" || UploadStatus1 == "00~Your documents have been logged successfully" || UploadStatus2 == "00~Your documents have been logged successfully")
                    {
                        employer.NotifyIncident(requestDetails[1], userkey, uid);
                        string status = "00~Your documents have been logged successfully";
                        var statusDetails = status.Split('~');
                        TempData["msg"] = "Your request and "+statusDetails[1];
                        //employer
                        return RedirectToAction("Index");
                    }
                    else
                    {

                    }
                    

                    TempData["msg"] = requestDetails[2];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = requestDetails[2];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/Create", "Requests", "Create", "CreateRequest Error", ex.InnerException.Message.ToString(), 0);
                TempData["error"] = "An error processing complaints";
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateRequest(string requestId, string statusUpdate, string internalUser, string externalUser, string custodianUser, string Comment, string convertedFile, string convertedFile1, string convertedFile2)
        {
            string UploadStatus = "";
            string UploadStatus1 = "";
            string UploadStatus2 = "";
            string assignedToId = "";
            string roleId = "";
            //convertedFile = "";
            //convertedFile1 = "";
            //convertedFile2 = "";

            string ipaddress = Request.UserHostAddress;
            string Agent = Request.UserAgent;
            string BrowserUsed = Request.Browser.Browser;
            string SessionID = HttpContext.Session.SessionID;

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            string LoginUser = (string)Session["LoginSAPID"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string WebUserID = (string)Session["WebUserID"];
            string RoleID = (string)Session["ROLE_ID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();

                if (internalUser != "")
                {
                    assignedToId = internalUser;
                    roleId = "2";
                }
                else if (externalUser != "")
                {
                    assignedToId = externalUser;
                    roleId = "3";
                }
                else if (custodianUser != "")
                {
                    assignedToId = externalUser;
                    roleId = "4";
                }
                string requestStatus;
                if (statusUpdate == "Assigned")
                {
                    requestStatus = employer.AssignRequest(requestId, Comment, WebUserID, "2", roleId, assignedToId, statusUpdate, userkey, uid);
                }
                else
                {
                    requestStatus = employer.AssignRequest(requestId, Comment, WebUserID, "2", roleId, WebUserID, statusUpdate, userkey, uid);
                }
                //var requestStatus = employer.CreateRequest(WebUserID, "3", requestCategory, "Initiated", Comment, userkey, uid);
                var requestDetails = requestStatus.Split('~');

                if (requestDetails[0] != "01")
                {
                    var file = Request.Files;

                    if (file[0].ContentLength > 0)
                    {
                        string _FileName = file[0].FileName;
                        string Docext = Path.GetExtension(file[0].FileName);
                        string Doc = convertedFile;
                        UploadStatus = employer.LogRequestDoc(requestId, requestDetails[1], Doc, Docext, _FileName, "Y", userkey, uid);
                        var uploadDetails = UploadStatus.Split('~');
                    }
                    if (file[1].ContentLength > 0)
                    {
                        string _FileName1 = file[1].FileName;
                        string Docext1 = Path.GetExtension(file[1].FileName);
                        string Doc1 = convertedFile1;
                        UploadStatus1 = employer.LogRequestDoc(requestId, requestDetails[1], Doc1, Docext1, _FileName1, "Y", userkey, uid);
                        //var uploadDetails1 = UploadStatus1.Split('~');
                    }
                    if (file[2].ContentLength > 0)
                    {
                        string _FileName2 = file[2].FileName;
                        string Docext2 = Path.GetExtension(file[2].FileName);
                        string Doc2 = convertedFile2;
                        UploadStatus2 = employer.LogRequestDoc(requestId, requestDetails[1], Doc2, Docext2, _FileName2, "Y", userkey, uid);
                        //uploadDetails2 = UploadStatus2.Split('~');
                    }

                    if (UploadStatus == "00~Your documents have been logged successfully" || UploadStatus1 == "00~Your documents have been logged successfully" || UploadStatus2 == "00~Your documents have been logged successfully")
                    {
                        employer.NotifyIncident(requestDetails[1], userkey, uid);
                        string status = "00~Your documents have been logged successfully";
                        var statusDetails = status.Split('~');
                        TempData["error"] = "Your request and " + statusDetails[1];
                        //employer
                        return RedirectToAction("Index");
                    }
                    else
                    {

                    }


                    TempData["msg"] = requestDetails[2];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = requestDetails[2];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Requests/UpdateRequest", "Requests", "UpdateRequest", "AssignRequest Error", ex.InnerException.Message.ToString(), 0);
                TempData["error"] = "An error processing complaints";
                return View();
            }
        }


        [Authorize]
        public JsonResult GetRequestCategory()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];
                string ROLE_ID = (string)Session["ROLE_ID"];

                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchRequestCategories(ROLE_ID, userkey, uid);
                dt.TableName = "Fetchrequestcategory";
                dt.Columns.ToString();
                var requestCategory = (from DataRow dr in dt.Rows
                                       select new
                                       {
                                           ID = dr["ID"].ToString(),
                                           Descr = dr["Descr"].ToString(),
                                           Mail = dr["Mail"].ToString(),
                                           Dynamictext = dr["dynamictext"].ToString(),
                                           Dynamicurl = dr["dynamicurls"].ToString(),
                                           IsAES = dr["IsAES"].ToString()
                                       }).ToList();

                return Json(new { data = requestCategory }, JsonRequestBehavior.AllowGet);
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