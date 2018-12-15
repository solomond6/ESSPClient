using DataTables.Mvc;
using EmployerServicePortal.Models;
using EmployerServicePortal.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;


namespace EmployerServicePortal.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public JsonResult GetCompanyEmployee([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String EmployerId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];

            try
            {
                Employer.Employer employer = new Employer.Employer();
                DataTable dt = employer.FetchCompanYEmployees(EmployerId, userkey, uid);
                dt.TableName = "CompanyEmployees";
                Crypto.Service1 _crypt = new Crypto.Service1();

                
                List<CompanyEmployee> companyEmployee = new List<CompanyEmployee>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<CompanyEmployee> employeeCount = (from DataRow dr in dt.Rows
                                                       select new CompanyEmployee()
                                                       {
                                                           PIN = dr["P_I_N"].ToString()
                                                       }).ToList();

                companyEmployee = (from DataRow dr in dt.Rows
                                   select new CompanyEmployee()
                                   {
                                       dPIN = _crypt.Encryptfx(dr["P_I_N"].ToString(), _access_key),
                                       PIN = dr["P_I_N"].ToString(),
                                       FirstName = dr["First Name"].ToString(),
                                       LastName = dr["Last Name"].ToString(),
                                       MiddleName = dr["Middle Names"].ToString(),
                                       PhoneNo = dr["Mobile 1"].ToString(),
                                       DateOfBirth = dr["Date Of Birth"].ToString(),
                                       DateOfEmployment = dr["Date Of Employment"].ToString(),
                                       Email = dr["E-mail"].ToString(),
                                   }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = employeeCount.Count();
                var filteredCount = companyEmployee.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.ToLower().Trim();

                    employeeCount = (from DataRow dr in dt.Rows
                                     where dr["P_I_N"].ToString().ToLower().Contains(value) || dr["First Name"].ToString().ToLower().Contains(value)
                                        || dr["Last Name"].ToString().ToLower().Contains(value) || dr["Mobile 1"].ToString().ToLower().Contains(value)
                                        || dr["E-mail"].ToString().ToLower().Contains(value)
                                     select new CompanyEmployee()
                                     {
                                         PIN = dr["P_I_N"].ToString()
                                     }).ToList();

                    companyEmployee = (from DataRow dr in dt.Rows
                                       where dr["P_I_N"].ToString().ToLower().Contains(value) || dr["First Name"].ToString().ToLower().Contains(value)
                                        || dr["Last Name"].ToString().ToLower().Contains(value) || dr["Mobile 1"].ToString().ToLower().Contains(value)
                                        || dr["E-mail"].ToString().ToLower().Contains(value)
                                       select new CompanyEmployee()
                                       {
                                           dPIN = _crypt.Encryptfx(dr["P_I_N"].ToString(), _access_key),
                                           PIN = dr["P_I_N"].ToString(),
                                           FirstName = dr["First Name"].ToString(),
                                           LastName = dr["Last Name"].ToString(),
                                           MiddleName = dr["Middle Names"].ToString(),
                                           PhoneNo = dr["Mobile 1"].ToString(),
                                           DateOfBirth = dr["Date Of Birth"].ToString(),
                                           DateOfEmployment = dr["Date Of Employment"].ToString(),
                                           Email = dr["E-mail"].ToString(),
                                       }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = employeeCount.Count();
                    filteredCount = companyEmployee.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = companyEmployee.Select(emList => new
                {
                    //dPIN = emList.dPIN,
                    PIN = emList.PIN,
                    FirstName = emList.FirstName,
                    LastName = emList.LastName,
                    MiddleName = emList.MiddleName,
                    PhoneNo = emList.PhoneNo,
                    DateOfBirth = emList.DateOfBirth,
                    DateOfEmployment = emList.DateOfEmployment,
                    Email = emList.Email
                }).OrderBy(orderByString == string.Empty ? "PIN asc" : orderByString).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Employee/GetCompanyEmployee", "Employee", "GetCompanyEmployee", "GetCompanyEmployee Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }

        [Authorize]

        public ActionResult Details()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];
            string _access_key = ConfigurationManager.AppSettings["Salt"];
            string Pin = this.Request.QueryString[0];

            try
            {
                Util.Util utility = new Util.Util();
                Crypto.Service1 _crypt = new Crypto.Service1();
                string dPin = _crypt.Decryptfx(Pin, _access_key);

                DataTable dt = utility.Fetchdata(dPin, "", userkey, uid);
                dt.TableName = "CompanyEmployees";

                var EmployeeDetails = (from DataRow dr in dt.Rows
                                       select new CompanyEmployee()
                                       {
                                           Title = dr["Title"].ToString(),
                                           FirstName = dr["First Name"].ToString(),
                                           LastName = dr["Last Name"].ToString(),
                                           MiddleName = dr["Middle Names"].ToString(),
                                           PhoneNo = dr["Mobile 1"].ToString(),
                                           DateOfBirth = dr["Date Of Birth"].ToString(),
                                           DateOfEmployment = dr["Date Of Employment"].ToString(),
                                           Email = dr["E-mail"].ToString(),
                                           StateOfOrigin = dr["State of Origin"].ToString(),
                                           LGA = dr["Local Government Authority"].ToString(),
                                           Address = dr["Address"].ToString(),
                                           Address2 = dr["Address 2"].ToString(),
                                           City = dr["City"].ToString(),
                                           Religion = dr["Religion"].ToString(),
                                           Nationality = dr["Nationality"].ToString(),
                                           Qualification = dr["Qualification"].ToString(),
                                           State = dr["State of Posting"].ToString(),
                                       }).ToList();

                ViewBag.Details = EmployeeDetails;
                return View();
            }
            catch (Exception ex)
            {
                LogError logerror = new LogError();
                logerror.ErrorLog("", LoginUser, "", "Employee/Details", "Employee", "Details", "Fetchdata Error", ex.Message.ToString(), 0);
                return RedirectToAction("Index");
            }
            
        }

    }
}
