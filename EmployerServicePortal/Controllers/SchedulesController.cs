using EmployerServicePortal.Models;
using EmployerServicePortal.Utility;
using ExcelDataReader;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployerServicePortal.Controllers
{
    public class SchedulesController : Controller
    {


        public void WriteExcelWithNPOI(DataTable dt, String extension)
        {

            IWorkbook workbook;

            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (extension == "xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                throw new Exception("This format is not supported");
            }

            ISheet sheet1 = workbook.CreateSheet("Sheet 1");

            sheet1.SetColumnWidth(0, 70);
            sheet1.SetColumnWidth(1, 350);
            sheet1.SetColumnWidth(2, 250);
            sheet1.SetColumnWidth(3, 150);
            sheet1.SetColumnWidth(4, 150);
            sheet1.SetColumnWidth(5, 150);
            sheet1.SetColumnWidth(6, 150);
            sheet1.SetColumnWidth(7, 150);
            sheet1.SetColumnWidth(8, 150);
            sheet1.SetColumnWidth(9, 150);

            IFont boldFont = workbook.CreateFont();
            boldFont.Boldweight = (short)FontBoldWeight.Bold;
            boldFont.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
            ICellStyle boldStyle = workbook.CreateCellStyle();
            boldStyle.SetFont(boldFont);
            //boldStyle.BorderBottom = CellBorderType.MEDIUM;
            boldStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            boldStyle.FillPattern = FillPattern.SolidForeground;


            ICellStyle boldStyle1 = workbook.CreateCellStyle();
            boldStyle1.SetFont(boldFont);
            //boldStyle.BorderBottom = CellBorderType.MEDIUM;
            boldStyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
            boldStyle1.FillPattern = FillPattern.SolidForeground;

            //make a header row
            IRow row1 = sheet1.CreateRow(0);
            //row1.Height(50);

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                cell.CellStyle = boldStyle1;
            }

            //loops through data
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                    if(dt.Rows[i][columnName].ToString() == "true")
                    {
                        cell.CellStyle = boldStyle;
                        //cell.CellStyle = IndexedColors.BrightGreen.Index;
                    }
                    if (dt.Rows[i][columnName].ToString() == "Error Count")
                    {
                        cell.CellStyle = boldStyle;
                        //cell.CellStyle = IndexedColors.BrightGreen.Index;
                    }
                }
            }

            sheet1.AutoSizeColumn(0);
            sheet1.AutoSizeColumn(1);
            sheet1.AutoSizeColumn(2);
            sheet1.AutoSizeColumn(3);
            sheet1.AutoSizeColumn(4);
            sheet1.AutoSizeColumn(5);
            sheet1.AutoSizeColumn(6);
            sheet1.AutoSizeColumn(7);
            sheet1.AutoSizeColumn(8);
            sheet1.AutoSizeColumn(9);
            sheet1.AutoSizeColumn(10);

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                if (extension == "xlsx") //xlsx file format
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AppendCookie(new HttpCookie("fileDownloadToken", "ScheduleRes"));
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ContactNPOI.xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                }
                else if (extension == "xls")  //xls file format
                {
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AppendCookie(new HttpCookie("fileDownloadToken", "ScheduleRes"));
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ContactNPOI.xls"));
                    Response.BinaryWrite(exportData.GetBuffer());
                }
                Response.End();
            }
        }

        [Authorize]
        public JsonResult ValidateAESSchedules()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string EMPLOYER_ID = (string)Session["EMPLOYER_ID"];
            string LoginUser = (string)Session["LoginSAPID"];

            var file = Request.Files["UploadedImage"];

            DataTable dt = new DataTable();
            if (file != null && file.ContentLength > 0)
            {
                if (file.FileName.EndsWith(".csv"))
                {
                    StreamReader streamCsv = new StreamReader(file.InputStream);
                    string csvDataLine = ""; int CurrentLine = 0;
                    string[] ScheduleData = null;
                    //string listCreated = "";

                    dt.Columns.Add("S/N", typeof(string));
                    dt.Columns.Add("Client Name", typeof(string));
                    dt.Columns.Add("RSAPIN", typeof(string));
                    dt.Columns.Add("Employee Mandatory", typeof(string));
                    dt.Columns.Add("Employer Mandatory", typeof(string));
                    dt.Columns.Add("Employee VC", typeof(string));
                    dt.Columns.Add("Employer VC", typeof(string));
                    dt.Columns.Add("Total", typeof(string));
                    dt.Columns.Add("Error Status", typeof(string));
                    dt.Columns.Add("Comment", typeof(string));

                    csvDataLine = streamCsv.ReadLine();
                    Employer.Employer employer = new Employer.Employer();
                    try
                    {
                        while ((csvDataLine = streamCsv.ReadLine()) != null)
                        {
                            if (CurrentLine != 1)
                            {
                                ScheduleData = csvDataLine.Split(',');
                                var validationStatus = employer.ValidateSchedule(ScheduleData[1], ScheduleData[2], ScheduleData[3], ScheduleData[4], ScheduleData[5], ScheduleData[6], ScheduleData[7], EMPLOYER_ID, userkey, uid);
                                var statusMessage = validationStatus.Split('~');
                                if (statusMessage[9] == "true")
                                {
                                    return Json(new { data = "Schedules Is faulty " }, JsonRequestBehavior.AllowGet);

                                }
                                //dt.Rows.Add(ScheduleData[0], ScheduleData[1], ScheduleData[2], ScheduleData[3], ScheduleData[4], ScheduleData[5], ScheduleData[6], ScheduleData[7], statusMessage[9], statusMessage[10]);
                            }
                        }
                        CurrentLine += 1;
                        //ViewBag.dat = dt;
                        //WriteExcelWithNPOI(dt, "xlsx");

                        return Json(new { data = "Schedule Validation successful" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        LogError logerror = new LogError();
                        logerror.ErrorLog("", LoginUser, "", "Schedules/Index", "Schedules", "Index", "ValidateSchedule Error", ex.Message.ToString(), 0);
                        return Json(new { data = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { data = "Invalid File Type (Only accepts .csv filetype" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { data = "Invalid File Type (Only accepts .csv filetype" }, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: Schedules
        [Authorize]
        public ActionResult Index()
        {
            string EPCOSS = ConfigurationManager.AppSettings["EPCOSS"];
            ViewBag.EPCOSS = EPCOSS;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(Users users, HttpPostedFileBase upload)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string EPCOSS = ConfigurationManager.AppSettings["EPCOSS"];
            ViewBag.EPCOSS = EPCOSS;
            string uid = ConfigurationManager.AppSettings["uid"];
            string EMPLOYER_ID = (string)Session["EMPLOYER_ID"];
            string LoginUser = (string)Session["LoginSAPID"];
            //Session.Remove("StatusMsg");

            DataTable dt = new DataTable();
            if (upload != null && upload.ContentLength > 0)
            {
                if (upload.FileName.EndsWith(".csv"))
                {
                    StreamReader streamCsv = new StreamReader(upload.InputStream);
                    string csvDataLine = ""; int CurrentLine = 0; int statusLine = 0;
                    string[] ScheduleData = null;
                    //string listCreated = "";
                    
                    dt.Columns.Add("S/N", typeof(string));
                    dt.Columns.Add("Client Name", typeof(string));
                    dt.Columns.Add("RSAPIN", typeof(string));
                    dt.Columns.Add("Employee Mandatory", typeof(string));
                    dt.Columns.Add("Employer Mandatory", typeof(string));
                    dt.Columns.Add("Employee VC", typeof(string));
                    dt.Columns.Add("Employer VC", typeof(string));
                    dt.Columns.Add("Total", typeof(string));
                    dt.Columns.Add("Error Status", typeof(string));
                    dt.Columns.Add("Comment", typeof(string));

                    csvDataLine = streamCsv.ReadLine();
                    Employer.Employer employer = new Employer.Employer();
                    try
                    {
                        while ((csvDataLine = streamCsv.ReadLine()) != null)
                        {
                            if (CurrentLine != 1)
                            {
                                ScheduleData = csvDataLine.Split(',');
                                var validationStatus = employer.ValidateSchedule(ScheduleData[1], ScheduleData[2], ScheduleData[3], ScheduleData[4], ScheduleData[5], ScheduleData[6], ScheduleData[7], EMPLOYER_ID, userkey, uid);
                                var statusMessage = validationStatus.Split('~');
                                if (statusMessage[9]  == "true")
                                {
                                    statusLine += 1;
                                }
                                dt.Rows.Add(ScheduleData[0], ScheduleData[1], ScheduleData[2], ScheduleData[3], ScheduleData[4], ScheduleData[5], ScheduleData[6], ScheduleData[7], statusMessage[9], statusMessage[10]);
                            }
                        }
                        dt.Rows.Add();
                        dt.Rows.Add("Error Count", statusLine);
                        CurrentLine += 1;
                        
                        Session["StatusMsg"] = statusLine;
                        
                        WriteExcelWithNPOI(dt, "xlsx");
                        return RedirectToAction("Index");
                        //return View();
                    }
                    catch (Exception ex)
                    {
                        LogError logerror = new LogError();
                        logerror.ErrorLog("", LoginUser, "", "Schedules/Index", "Schedules", "Index", "ValidateSchedule Error", ex.Message.ToString(), 0);
                        TempData["error"] = ex.Message.ToString();
                        return View();
                    }
                }
                else
                {
                    TempData["error"] = "Invalid File Type (Only accepts .csv filetype)";
                    return View();
                }
            }
            else{
                TempData["error"] = "Invalid File Type (Only accepts .csv filetype)";
                return View();
            }
        }



        [Authorize]
        public FileResult ScheduleValidationSample(string ImageName)
        {
            string contentType = string.Empty;
            var sampleFile = Server.MapPath("~/SampleDocs/sampleSchedule.csv");
            if (sampleFile.Contains(".csv"))
            {
                contentType = "application/csv";
            }
            return File(sampleFile, contentType, sampleFile);
        }
    }
}