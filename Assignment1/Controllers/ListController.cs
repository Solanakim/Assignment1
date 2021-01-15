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


namespace HomePage.Controllers
{
    public class ListController : Controller
    {
        // GET: List
        [HttpGet]
        public ActionResult Transactions()
        {
           return View();
        }

        [HttpPost]      
        public JsonResult GetFileContent(string extension)
        {
            if (Request.Files.Count > 0)
            {
                List<Statement> statements = new List<Statement>();
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];
                    string fileName = file.FileName;

                    // create the uploads folder if it doesn't exist
                    Directory.CreateDirectory(Server.MapPath("~/uploads/"));
                    string path = Path.Combine(Server.MapPath("~/uploads/"), fileName);

                    // save the file
                    file.SaveAs(path);

                    if(extension == ".json")
                    {
                        
                        string filePath = path;
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            statements = JsonConvert.DeserializeObject<List<Statement>>(sr.ReadToEnd());
                        }
                        Console.WriteLine("{0}",Json(statements));
                        return Json(statements, JsonRequestBehavior.AllowGet);
                      
                    }
                    else if(extension == ".xls" || extension == ".xlsx")
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
                                    });
                                }
                            }
                        }

                        return Json(statements, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                catch (Exception e)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("FAIL", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAsJson(List<Json> statement)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
            string jsondata = JsonConvert.SerializeObject(statement, Formatting.Indented);
            try
            {
                Directory.CreateDirectory(Server.MapPath("~/JSON/"));
                string path = Server.MapPath("~/JSON/");
                System.IO.File.WriteAllText(path + date + ".json", jsondata);
            }
            catch (System.IO.IOException e)
            {
                return Json("FAIL", JsonRequestBehavior.AllowGet);
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult JsonList()
        {
            List<JsonFile> jsonFiles = new List<JsonFile>();

            string[] files = System.IO.Directory.GetFiles(Server.MapPath("~/JSON/"), "*.JSON");

            foreach (string s in files)
            {
                // Create the FileInfo object only when needed to ensure
                // the information is as current as possible.
                System.IO.FileInfo fi = null;
                try
                {
                    fi = new System.IO.FileInfo(s);
                    jsonFiles.Add(new JsonFile
                    {
                        FilePath = fi.FullName,
                        FileName = fi.Name,
                        FileSize = fi.Length.ToString(),                       
                    });
                    
                }
                catch (System.IO.FileNotFoundException e)
                {
                    // To inform the user and continue is
                    // sufficient for this demonstration.
                    // Your application may require different behavior.
                    Console.WriteLine(e.Message);
                    continue;
                }
                Console.WriteLine("{0} : {1}", fi.Name, fi.Directory);
            }


            return View(jsonFiles);
        }

        [HttpPost]
        public ActionResult JsonList(Statement statement)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Json(string fileName)
        {
            List<Statement> statements = new List<Statement>();
            string filePath = Server.MapPath("~/JSON/") + fileName;
            using (StreamReader sr = new StreamReader(filePath))
            {
                statements = JsonConvert.DeserializeObject<List<Statement>>(sr.ReadToEnd());
            }

            return View(statements);
        }
    }
}