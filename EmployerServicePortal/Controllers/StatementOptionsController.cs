using DataTables.Mvc;
using EmployerServicePortal.Models;
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
    public class StatementOptionsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(Batch batch)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string CompanyName = (string)Session["CompanyName"];

            try
            {
                Employer.Employer employer = new Employer.Employer();

                if (batch.statementOption == "1")
                {
                    if (batch.ContactName2 == null)
                    {
                        batch.ContactName2 = "";
                    }

                    if (batch.ContactAddress1 == null)
                    {
                        batch.ContactAddress1 = "";
                    }

                    if (batch.ContactPhone2 == null)
                    {
                        batch.ContactPhone2 = "";
                    }

                    var batchStatus = employer.MapDeliverybyEmployer(CompanyName, EmployerId, batch.ContactName, batch.ContactName2, batch.ContactPhone, batch.ContactPhone2, batch.ContactAddress, batch.ContactAddress1,  userkey, uid);
                    var statusDetails = batchStatus.Split('~');

                    if (statusDetails[0] != "01")
                    {
                        TempData["bMsg"] = statusDetails[1];
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["bError"] = statusDetails[1];
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    var pins = string.Join(",", batch.id);
                    var batchStatus = employer.MapDeliverybyBatch(CompanyName, EmployerId, batch.AddressId, batch.BatchId, pins, userkey, uid);
                    var statusDetails = batchStatus.Split('~');
                    //employer.();

                    if (statusDetails[0] != "01")
                    {
                        TempData["bMsg"] = statusDetails[1];
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["bError"] = statusDetails[1];
                        return RedirectToAction("Index");
                    }
                }
                
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "StatementOptions/Batch", "StatementOptions", "Index", "MapDeliverybyBatch Error", ex.Message.ToString(), 0);
                TempData["bError"] = ex.Message.ToString();
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult BatchList()
        {
            return View();
        }


        [Authorize]
        public JsonResult GetBatchList3([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String batchId, String addressId)
        {

            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string CompanyName = (string)Session["CompanyName"];

            string WebUserID = (string)Session["WebUserID"];
            string EMPLOYER_ID = (string)Session["EMPLOYER_ID"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchStatementEmployerBatch(CompanyName, EMPLOYER_ID, userkey, uid);
                dt.TableName = "StatementEmployerBatch";
                dt.Columns.ToString();

                List<BatchList> batches = new List<BatchList>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<BatchList> batchCount = (from DataRow dr in dt.Rows
                                               select new BatchList()
                                               {
                                                   ID = dr["Category ID"].ToString(),
                                               }).ToList();

                batches = (from DataRow dr in dt.Rows
                            select new BatchList()
                            {
                                ID = dr["Category ID"].ToString(),
                                AddressId = dr["Address ID"].ToString(),
                                BatchName = dr["StmtBatchName"].ToString(),
                                RecipientName = dr["Receipient Name"].ToString(),
                                Phone = dr["Phone Number"].ToString(),
                                RecipientName2 = dr["Recepient 2"].ToString(),
                                Phone2 = dr["Phone 2"].ToString(),
                                Address = dr["Statement Address 2"].ToString(),
                                State = dr["State"].ToString(),
                                LGA = dr["LGA"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = batchCount.Count();
                var filteredCount = batches.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    batchCount = (from DataRow dr in dt.Rows
                                    where dr["StmtBatchName"].ToString().ToLower().Contains(value)
                                    select new BatchList()
                                    {
                                        ID = dr["Category ID"].ToString(),
                                    }).ToList();

                    batches = (from DataRow dr in dt.Rows
                                where dr["StmtBatchName"].ToString().ToLower().Contains(value)
                                select new BatchList()
                                {
                                    ID = dr["Category ID"].ToString(),
                                    AddressId = dr["Address ID"].ToString(),
                                    BatchName = dr["StmtBatchName"].ToString(),
                                    RecipientName = dr["Receipient Name"].ToString(),
                                    Phone = dr["Phone Number"].ToString(),
                                    RecipientName2 = dr["Recepient 2"].ToString(),
                                    Phone2 = dr["Phone 2"].ToString(),
                                    Address = dr["Statement Address 2"].ToString(),
                                    State = dr["State"].ToString(),
                                    LGA = dr["LGA"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = batchCount.Count();
                    filteredCount = batches.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = batches.Select(emList => new
                {
                    ID = emList.ID,
                    AddressId = emList.AddressId,
                    BatchName = emList.BatchName,
                    RecipientName = emList.RecipientName,
                    Phone = emList.Phone,
                    RecipientName2 = emList.RecipientName2,
                    Phone2 = emList.Phone2,
                    Address = emList.Address,
                    State = emList.State,
                    LGA = emList.LGA,
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "StatementOptions/GetBatchList3", "StatementOptions", "GetBatchList3", "FetchStatementEmployerBatch Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetBatchDetails([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String batchId, String addressId)
        {

            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string CompanyName = (string)Session["CompanyName"];
            string WebUserID = (string)Session["WebUserID"];
            string EMPLOYER_ID = (string)Session["EMPLOYER_ID"];

            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {
                Employer.Employer employer = new Employer.Employer();

                string dBatchId = CryptoEngine.DecryptString(batchId, _access_key);
                string dAddressId = CryptoEngine.DecryptString(addressId, _access_key);

                DataTable dt = employer.FetchPINsBatch(CompanyName, EMPLOYER_ID, dBatchId, dAddressId, userkey, uid);
                dt.TableName = "PinsBatch";
                dt.Columns.ToString();

                List<CompanyEmployee> pins = new List<CompanyEmployee>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<CompanyEmployee> pinCount = (from DataRow dr in dt.Rows
                                              select new CompanyEmployee()
                                              {
                                                  PIN = dr["P_I_N"].ToString(),
                                              }).ToList();

                pins = (from DataRow dr in dt.Rows
                           select new CompanyEmployee()
                           {
                               PIN = dr["P_I_N"].ToString(),
                               FirstName = dr["First Name"].ToString(),
                               LastName = dr["Last Name"].ToString(),
                               MiddleName = dr["Middle Names"].ToString()
                           }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = pinCount.Count();
                var filteredCount = pins.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    pinCount = (from DataRow dr in dt.Rows
                                where dr["P_I_N"].ToString().ToLower().Contains(value) || dr["First Name"].ToString().ToLower().Contains(value) || dr["Last Name"].ToString().ToLower().Contains(value) || dr["Middle Names"].ToString().ToLower().Contains(value)
                                select new CompanyEmployee()
                                  {
                                      PIN = dr["Category ID"].ToString(),
                                  }).ToList();

                    pins = (from DataRow dr in dt.Rows
                            where dr["P_I_N"].ToString().ToLower().Contains(value) || dr["First Name"].ToString().ToLower().Contains(value) || dr["Last Name"].ToString().ToLower().Contains(value) || dr["Middle Names"].ToString().ToLower().Contains(value)
                            select new CompanyEmployee()
                               {
                                   PIN = dr["P_I_N"].ToString(),
                                   FirstName = dr["First Name"].ToString(),
                                   LastName = dr["Last Name"].ToString(),
                                   MiddleName = dr["Middle Names"].ToString()
                               }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = pinCount.Count();
                    filteredCount = pins.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = pins.Select(emList => new
                {
                    PIN = emList.PIN,
                    FirstName = emList.FirstName,
                    LastName = emList.LastName,
                    MiddleName = emList.MiddleName
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "StatementOptions/GetBatchDetails", "StatementOptions", "GetBatchDetails", "FetchPINsBatch Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult BatchDetails(string batch, string addid)
        {
            ViewBag.Batch = batch.Replace(' ', '+');
            ViewBag.Addid = addid.Replace(' ', '+');
            return View();
        }


        [Authorize]
        public JsonResult GetBatchLists2()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string CompanyName = (string)Session["CompanyName"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchStatementEmployerBatch(CompanyName, EmployerId, userkey, uid);
                dt.TableName = "EmployerBatch";
                dt.Columns.ToString();

                var batches = (from DataRow dr in dt.Rows
                               select new
                               {
                                   ID = dr["Category ID"].ToString(),
                                   BatchName = dr["timestamp"].ToString(),
                                   //BatchName = "BatchName",
                               }).ToList();

                return Json(new { data = batches }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "StatementOptions/GetBatchLists", "StatementOptions", "GetBatchLists", "FetchInternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Batch(Batch batch)
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string CompanyName = (string)Session["CompanyName"];

            try
            {
                Employer.Employer employer = new Employer.Employer();

                var batchStatus = employer.CreateStatementEmployerBatch(batch.BatchName, CompanyName, EmployerId, batch.ContactAddress, batch.ContactAddress1, batch.ContactLGA, batch.ContactState, batch.ContactName, batch.ContactPhone, batch.ContactName2, batch.ContactPhone2, userkey, uid);
                var statusDetails = batchStatus.Split('~');

                if (statusDetails[0] != "01")
                {
                    TempData["bMsg"] = statusDetails[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["bError"] = statusDetails[1];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "StatementOptions/Batch", "StatementOptions", "Batch", "FetchInternalUsers Error", ex.Message.ToString(), 0);
                TempData["bError"] = ex.Message.ToString();
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public JsonResult GetBatchLists()
        {
            string LoginUser = (string)Session["LoginSAPID"];
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string EmployerId = (string)Session["EMPLOYER_ID"];
            string CompanyName = (string)Session["CompanyName"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {
                Employer.Employer employer = new Employer.Employer();

                DataTable dt = employer.FetchStatementEmployerBatch(CompanyName, EmployerId, userkey, uid);
                dt.TableName = "EmployerBatch";
                dt.Columns.ToString();

                var batches = (from DataRow dr in dt.Rows
                             select new 
                             {
                                 ID = CryptoEngine.EncryptString(dr["Category ID"].ToString(), _access_key),
                                 BatchName = dr["StmtBatchName"].ToString(),
                                 AddressId = CryptoEngine.EncryptString(dr["Address ID"].ToString(), _access_key),
                             }).ToList();

                return Json(new { data = batches }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "StatementOptions/GetBatchLists", "StatementOptions", "GetBatchLists", "FetchInternalUsers Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}