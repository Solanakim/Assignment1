using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Data.Entity.Validation;

using HomePage.Models;
using System.Data.Entity;

namespace HomePage.Controllers
{
    public class BoardController : Controller
    {
        HomePageEntities db = new HomePageEntities();    //데이터베이스 사용하기 
        // GET: Board
        
        [HttpGet]
        public ActionResult Create()
        {
            Articles article = new Articles();
            return View(article);
        }
        [HttpPost]
        public ActionResult Create(Articles article)       
        {
            Articles dbarticle = new Articles();
            try
            {
                dbarticle.Title = article.Title;
                dbarticle.Contents = article.Contents;
                dbarticle.Category = article.Category;
                dbarticle.RegistDate = DateTime.Now;
                dbarticle.ModifyDate = DateTime.Now;
                dbarticle.ViewCnt = 0;
                dbarticle.IPAddress = Request.ServerVariables["REMOTE_ADDR"].ToString();
                dbarticle.RegistMemberID = "admin";
                dbarticle.ModifyMemberID = "admin";

                db.Articles.Add(dbarticle);
                db.SaveChanges();

                if(Request.Files.Count > 0)
                {
                    var attachFile = Request.Files[0];

                    if(attachFile !=null && attachFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(attachFile.FileName);                      
                        var path = Path.Combine(Server.MapPath("~/Upload/"), fileName);
                        attachFile.SaveAs(path);

                        ArticleFiles file = new ArticleFiles();
                        file.ArticleIDX = dbarticle.ArticleIDX;
                        file.FilePath = "/Upload/";
                        file.FileName = fileName;
                        file.FileFormat = Path.GetExtension(attachFile.FileName);
                        file.FileSize = attachFile.ContentLength;
                        file.UploadDate = DateTime.Now;
                        db.ArticleFiles.Add(file);
                        db.SaveChanges();
                    }
                }
                ViewBag.Result = "OK";
            }
            catch(Exception ex)
            {
                ViewBag.Result = "FAIL";
                Console.WriteLine($"Exception: '{ex}'");

            }

            return View(article);           
        }

        [HttpPost]

        public ActionResult ArticleEdit(ArticleViewModel vm)
        {
            ArticleViewModel dbVM = new ArticleViewModel();
            try
            {
                Articles dbArticle = db.Articles.Find(vm.Article.ArticleIDX);
                dbArticle.Title = vm.Article.Title;
                dbArticle.Category = vm.Article.Category;
                dbArticle.Contents = vm.Article.Contents;
                dbArticle.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                dbArticle.ModifyDate = DateTime.Now;
                dbArticle.ModifyMemberID = "admin";

                db.Entry(dbArticle).State = System.Data.EntityState.Modified;
                db.SaveChanges();

                if(Request.Files.Count > 0)
                {
                    var attachFile = Request.Files[0];

                    if(attachFile !=null && attachFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(attachFile.FileName);    //파일 이름 추출
                        var path = Path.Combine(Server.MapPath("~/Upload"), fileName);   //파일경로 생성
                        attachFile.SaveAs(path);  //생성한 파일 경로대로 실제파일 저장 

                        ArticleFiles file = new ArticleFiles();

                        file.ArticleIDX = vm.Article.ArticleIDX;
                        file.FilePath = "/Upload/";
                        file.FileName = fileName;
                        file.FileFormat = Path.GetExtension(attachFile.FileName);
                        file.FileSize = attachFile.ContentLength;
                        file.UploadDate = DateTime.Now;


                        db.ArticleFiles.Add(file);
                        db.SaveChanges();

                    }

                }
                Articles article = db.Articles.Where(c => c.ArticleIDX == vm.Article.ArticleIDX).FirstOrDefault();
                List<ArticleFiles> files = db.ArticleFiles.Where(c => c.ArticleIDX == vm.Article.ArticleIDX).OrderBy(o => o.UploadDate).ToList();

                dbVM.Article = article;
                dbVM.Files = files;
                ViewBag.Result = "OK";
            }
            catch(DbEntityValidationException dbEx)
            {
                //Console.WriteLine($"Exception: '{ex}'");

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                dbVM = vm;
                ViewBag.Result = "FAIL";

            }
            return View(dbVM);     
        }


        [HttpGet]

        public ActionResult ArticleList()
        {
            List<Articles> articleList = db.Articles.OrderByDescending(o => o.ModifyDate).ToList();
            return View(articleList);
        }

        [HttpGet]

        public ActionResult ArticleDetail(int articleid)
        {
            ArticleViewModel vm = new ArticleViewModel();

            Articles article = db.Articles.Where(c => c.ArticleIDX == articleid).FirstOrDefault();
            article.ViewCnt += 1;
            db.SaveChanges();            
            
            
            article = db.Articles.Where(c => c.ArticleIDX == articleid).FirstOrDefault();
            List<ArticleFiles> files = db.ArticleFiles.Where(c => c.ArticleIDX == articleid).OrderBy(o => o.UploadDate).ToList();

            vm.Article = article;
            vm.Files = files;

            return View(vm);
        }

        [HttpGet]
        public ActionResult ArticleEdit(int articleid)
        {
            ArticleViewModel vm = new ArticleViewModel();

            Articles article = db.Articles.Where(c => c.ArticleIDX == articleid).FirstOrDefault();
            List<ArticleFiles> files = db.ArticleFiles.Where(c => c.ArticleIDX == articleid).OrderBy(o => o.UploadDate).ToList();

            vm.Article = article;
            vm.Files = files;

            return View(vm);
        }

        [HttpGet]

        public JsonResult ArticleDelete(int articleid)
        {
            Articles article = db.Articles.Find(articleid);
            ArticleFiles files = db.ArticleFiles.Find(articleid);

            db.ArticleFiles.Remove(files);
            db.Articles.Remove(article);
            db.SaveChanges();
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public JsonResult FileDelete(int fileIDX)
        {
            ArticleFiles file = db.ArticleFiles.Where(c=> c.FileIDX == fileIDX).FirstOrDefault();
            int articleIDX = Convert.ToInt32(file.ArticleIDX);

            System.IO.File.Delete(Server.MapPath(file.FilePath + file.FileName));

            db.ArticleFiles.Remove(file);
            db.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}