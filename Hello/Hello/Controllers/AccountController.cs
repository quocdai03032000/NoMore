using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hello.Models;

namespace Hello.Controllers
{
    public class AccountController : Controller
    {
        QuanLyThuVienEntities database = new QuanLyThuVienEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View(database.AccountADMINs.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(AccountADMIN ac)
        {
            try
            {
                database.AccountADMINs.Add(ac);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Tạo mới không thành công");
            }            
        }
        //xem thông tin chi tiết
        public ActionResult Details(int id)
        {
            return View(database.AccountADMINs.Where(a => a.MaAccount == id).FirstOrDefault());
        }

        //Edit

        public ActionResult Edit (int id)
        {
            return View(database.AccountADMINs.Where(s =>s.MaAccount==id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(int id , AccountADMIN ac)
        {
            database.Entry(ac).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("Index");
            //return Content("thành công");
        }

    }
}