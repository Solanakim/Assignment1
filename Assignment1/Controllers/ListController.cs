using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomePage.Models;
using ExcelDataReader;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Web.Script.Serialization;


namespace HomePage.Controllers
{
    public class ListController : Controller
    {
        HomePageEntities db = new HomePageEntities();
        // GET: List
        [HttpGet]
        public ActionResult Transactions()
        {
            return View();
        }

        /// <summary>
        /// 파일을 받아서 거래내역 내용만 추출해서 view로 return(json, excel 파일)
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetFileContent(string extension)
        {
            Transactions transactions = new Transactions();
            List<Options> options = db.Options.Where(c => c.Enabled == 1).OrderBy(o => o.Value).ToList();
            if (Request.Files.Count > 0)
            {
                List<Statement> statements = new List<Statement>();
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];
                    string fileName = file.FileName;
                    string extensions = Path.GetExtension(file.FileName);

                    // create the uploads folder if it doesn't exist
                    Directory.CreateDirectory(Server.MapPath("~/uploads/"));
                    string path = Path.Combine(Server.MapPath("~/uploads/"), fileName);

                    // save the file
                    file.SaveAs(path);

                    if (extension == ".json")
                    {
                        string filePath = path;
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            statements = JsonConvert.DeserializeObject<List<Statement>>(sr.ReadToEnd());
                        }
                    }
                    else if (extension == ".xls" || extension == ".xlsx")
                    {
                        string filePath = path;
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                        using (var stream = System.IO.File.Open(filePath, System.IO.FileMode.Open))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {

                                while (reader.Read()) //Each row of the file
                                {
                                    statements.Add(new Statement
                                    {
                                        TransDate = reader.GetValue(0).ToString(),
                                        ReferenceNumber = reader.GetValue(2).ToString(),
                                        MerchantName = reader.GetValue(5).ToString(),
                                        Amount = reader.GetValue(6).ToString(),
                                        Content = "",
                                        Type = "",
                                        TypeValue = "",
                                    });
                                }
                            }
                        }
                    }
                    transactions.options = options;
                    transactions.statements = statements;

                    return Json(transactions, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("FAIL", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 받아온 파일과 현재 테이블에 있는 데이터를 합친다. 단 중복되는 승인번호의 데이터는 합치지 않는다.
        /// </summary>
        /// <param name="statements_json"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMergeData(string statements_json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Transactions transactions = new Transactions();
            List<Statement> comparison = js.Deserialize<List<Statement>>(statements_json);   //현재 테이블에 있는 데이터(비교본)
            List<Statement> statements = js.Deserialize<List<Statement>>(statements_json);   //return할 데이터
            List<Options> options = db.Options.Where(c => c.Enabled == 1).OrderBy(o => o.Value).ToList();
            bool check = false;
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];
                    string fileName = file.FileName;
                    string extensions = Path.GetExtension(file.FileName);

                    // create the uploads folder if it doesn't exist
                    Directory.CreateDirectory(Server.MapPath("~/uploads/"));
                    string path = Path.Combine(Server.MapPath("~/uploads/"), fileName);

                    // save the file
                    file.SaveAs(path);

                    string filePath = path;
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    using (var stream = System.IO.File.Open(filePath, System.IO.FileMode.Open))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {

                            while (reader.Read()) //Each row of the file
                            {
                                string rfnumber = reader.GetValue(2).ToString().Trim();
                                foreach (Statement statement in comparison)
                                {
                                    if (statement.ReferenceNumber == rfnumber || rfnumber == "승인번호")
                                    {
                                        check = false;
                                        break;
                                    }
                                    else
                                    {
                                        check = true;
                                    }
                                }

                                if (check == true)
                                {
                                    statements.Add(new Statement
                                    {
                                        TransDate = reader.GetValue(0).ToString(),
                                        ReferenceNumber = reader.GetValue(2).ToString(),
                                        MerchantName = reader.GetValue(5).ToString(),
                                        Amount = reader.GetValue(6).ToString(),
                                        Content = "",
                                        Type = "",
                                        TypeValue = "",
                                    });
                                    check = false;
                                }
                            }
                        }
                    }

                    transactions.options = options;
                    transactions.statements = statements;

                    return Json(transactions, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("FAIL", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetOptions()
        {
            List<Options> options = db.Options.Where(c => c.Enabled == 1).OrderBy(o => o.Value).ToList();

            return Json(options, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 테이블 데이터를 받아서 3가지 방식으로 json 파일로 저장한다.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="saveType">1-새로운 json파일 저장, 2-파일 이름 받아서 저장, 3-기존에 있는 파일 이름으로 저장</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveAsJson(string id, List<Json> statement, int saveType, string fileName)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
            string date_path = DateTime.Now.ToString("yyyy-MM");

            statement = statement.OrderBy(x => x.TransDate).ToList();
            string jsondata = JsonConvert.SerializeObject(statement, Formatting.Indented);
            try
            {
                Directory.CreateDirectory(Server.MapPath("~/JSON/" + id + "/" + date_path + "/"));
                string path = Server.MapPath("~/JSON/" + id + "/" + date_path + "/");

                if (saveType == 1)  //새로 저장
                    System.IO.File.WriteAllText(path + date + ".json", jsondata);
                else if (saveType == 2) //다른 이름으로 저장
                    System.IO.File.WriteAllText(path + fileName + ".json", jsondata);
                else if (saveType == 3)//기존 이름으로 덮어쓰기
                {
                    if (System.IO.File.Exists(path + fileName))
                    {
                        // Use a try block to catch IOExceptions, to
                        // handle the case of the file already being
                        // opened by another process.
                        try
                        {
                            System.IO.File.Delete(path + fileName);
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                            return Json("FAIL", JsonRequestBehavior.AllowGet);
                        }
                        System.IO.File.WriteAllText(path + fileName, jsondata);
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                return Json("FAIL", JsonRequestBehavior.AllowGet);
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 저장한 JSON 파일들의 정보를 return 한다. 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult JsonList(string id)
        {
            //List<JsonFile> jsonFiles = new List<JsonFile>();
            //Directory.CreateDirectory(Server.MapPath("~/JSON/" + id + "/"));
            //IOrderedEnumerable<string> files = System.IO.Directory.GetFiles(Server.MapPath("~/JSON/" + id + "/"), "*.JSON", SearchOption.AllDirectories).OrderByDescending(f => new FileInfo(f).CreationTime);

            //foreach (string s in files)
            //{
            //    // Create the FileInfo object only when needed to ensure
            //    // the information is as current as possible.
            //    System.IO.FileInfo fi = null;
            //    try
            //    {
            //        fi = new System.IO.FileInfo(s);
            //        jsonFiles.Add(new JsonFile
            //        {
            //            FilePath = fi.FullName,
            //            FileName = fi.Name,
            //            FileSize = fi.Length.ToString(),
            //        });

            //    }
            //    catch (System.IO.FileNotFoundException e)
            //    {
            //        // To inform the user and continue is
            //        // sufficient for this demonstration.
            //        // Your application may require different behavior.
            //        Console.WriteLine(e.Message);
            //    }
            //    Console.WriteLine("{0} : {1}", fi.Name, fi.Directory);
            //}
            //return View(jsonFiles);
            return View();
        }

        /// <summary>
        /// 파일 생성 날짜로 구분해서 JSON 파일들의 정보를 return 한다
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult GetJsonListByDate(int month, int year,string id)
        //{
        //    List<JsonFile> jsonFiles = new List<JsonFile>();
        //    DateTime startDate = new DateTime(year, month, 1);
        //    DateTime endDate = startDate.AddMonths(1).AddDays(-1);
        //    IOrderedEnumerable<string> files = System.IO.Directory.GetFiles(Server.MapPath("~/JSON/" + id + "/"), "*.JSON").OrderByDescending(f => new FileInfo(f).CreationTime);

        //    foreach (string fi in files)
        //    {
        //        FileInfo fileInfo = new FileInfo(fi);
        //        if (fileInfo.CreationTime.Date >= startDate.Date && fileInfo.CreationTime.Date <= endDate.Date)
        //        { //use directly flInfo.CreationTime and flInfo.Name without create another variable 
        //            jsonFiles.Add(new JsonFile
        //            {
        //                FilePath = fileInfo.FullName,
        //                FileName = fileInfo.Name,
        //                FileSize = fileInfo.Length.ToString(),
        //            });
        //        }
        //    }
        //    return Json(jsonFiles, JsonRequestBehavior.AllowGet);
        //} 


        /// <summary>
        /// 월별 폴더에서 JSON 파일들의 정보를 return 한다
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetJsonListByDate(int month, int year, string id)
        {
            List<JsonFile> jsonFiles = new List<JsonFile>();
            string month_s = month < 10 ? "0" + month : "" + month;
            if (Directory.Exists(Server.MapPath("~/JSON/" + id + "/" + year + "-" + month_s + "/")))
            {
                IOrderedEnumerable<string> files = System.IO.Directory.GetFiles(Server.MapPath("~/JSON/" + id + "/" + year + "-" + month_s + "/"), "*.JSON").OrderByDescending(f => new FileInfo(f).CreationTime);

                foreach (string fi in files)
                {
                    FileInfo fileInfo = new FileInfo(fi);

                    jsonFiles.Add(new JsonFile
                    {
                        FilePath = fileInfo.FullName,
                        FileName = fileInfo.Name,
                        FileSize = fileInfo.Length.ToString(),
                    });

                }
                return Json(jsonFiles, JsonRequestBehavior.AllowGet);
            }        
            return Json(jsonFiles, JsonRequestBehavior.AllowGet);
        }

        /*파일 이름으로 구분해서 가져오기*/
        //[HttpPost]
        //public JsonResult GetJsonListByDate(string month, string year)
        //{

        //    if (System.Convert.ToInt32(month) < 10)
        //    {
        //        month = '0' + month;
        //    }

        //    List<JsonFile> jsonFiles = new List<JsonFile>();
        //    IOrderedEnumerable<string> files = System.IO.Directory.GetFiles(Server.MapPath("~/JSON/"), "*.JSON").OrderByDescending(f => new FileInfo(f).CreationTime);

        //    foreach (string fi in files)
        //    {
        //        FileInfo fileInfo = new FileInfo(fi);
        //        string[] date = fileInfo.Name.Split('-');
        //        if (month == date[1] && year == date[0])
        //        { //use directly flInfo.CreationTime and flInfo.Name without create another variable 
        //            jsonFiles.Add(new JsonFile
        //            {
        //                FilePath = fileInfo.FullName,
        //                FileName = fileInfo.Name,
        //                FileSize = fileInfo.Length.ToString(),
        //            });
        //        }
        //    }
        //    return Json(jsonFiles, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 선택한 JSON 파일의 정보를 return 한다.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Json(string fileName, string id, int month, int year)
        {
            List<Statement> statements = new List<Statement>();
            string month_s = month < 10 ? "0" + month : "" + month;
            if (Directory.Exists(Server.MapPath("~/JSON/" + id + "/" + year + "-" + month_s + "/")))
            {
                string filePath = Server.MapPath("~/JSON/" + id + "/" + year + "-" + month_s + "/") + fileName;
                using (StreamReader sr = new StreamReader(filePath))
                {
                    statements = JsonConvert.DeserializeObject<List<Statement>>(sr.ReadToEnd());
                }

                return View(statements);
            }
            return View(statements);
        }
    }
}