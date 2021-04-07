using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebNangCao___ThucHanh.Models;

namespace WebNangCao___ThucHanh.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        QuanLyThuVienEntities database = new QuanLyThuVienEntities();
        // GET: Admin/Login
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(Account_Admin ad)
        {
            var check = database.Account_Admin.Where(s => s.User.Equals(ad.User) && s.Password.Equals(ad.Password)).FirstOrDefault();
            if(check==null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View("Index",ad);
            }
            else
            {
                Session["User"] = ad.User;
                return RedirectToAction("Index", "Main");
            }
            
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }


    }
}