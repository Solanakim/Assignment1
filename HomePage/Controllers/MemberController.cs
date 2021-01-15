using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HomePage.Models;
using System.Data.Entity;

namespace HomePage.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        HomePageEntities db = new HomePageEntities();       

        [HttpGet]
        public ActionResult Entry()
        {
            Members member = new Members();
        
            return View(member);
        }

        [HttpPost]
        
        public ActionResult Entry(Members member)
        {
            member.EntryDate = DateTime.Now;
            try
            {
                db.Members.Add(member);
                db.SaveChanges();
                ViewBag.Result = "OK";
            }
            catch (Exception ex)
            {
                ViewBag.Result = "FAIL";
            }

            return RedirectToAction("Index","Home");
            //return View(member);
        }

        [HttpGet]

        public JsonResult IDCheck(string memberid)
        {
            string result = string.Empty;
            Members member = db.Members.Find(memberid);
            if(member == null)
            {
                result = "OK";
            }
            else
            {
                result = "FAIL";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult List()
        {
            List<Members> list = db.Members.OrderByDescending(o => o.EntryDate).ToList();
  
            return View(list);
        }

        [HttpGet]
        public ActionResult Edit(string memberid)
        {
            Members member = db.Members.Where(c => c.MemberID == memberid).FirstOrDefault();

            return View(member);
        }
        [HttpPost]
        public ActionResult Edit(Members member)
        {
            Members dbMember = db.Members.Find(member.MemberID);

            if (dbMember != null)
            {
                try
                {
                    dbMember.MemberName = member.MemberName;
                    dbMember.MemberPWD = member.MemberPWD;
                    dbMember.Email = member.Email;
                    dbMember.Telephone = member.Telephone;
                    db.Entry(dbMember).State = System.Data.EntityState.Modified;
                    db.SaveChanges();

                    ViewBag.Result = "OK";


                }
                catch (Exception ex)
                {
                    ViewBag.Result = "FAIL";

                }
               
            }
            else
            {
                ViewBag.Result = "FAIL";

            }

            return View(dbMember);
        }

        [HttpGet]
        public JsonResult Delete(string memberid)
        {
            Members dbmember = db.Members.Find(memberid);
            db.Members.Remove(dbmember);
            db.SaveChanges();
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

    }
}