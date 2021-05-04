using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn1_QuanLyThuVien.Models;
namespace DoAn1_QuanLyThuVien.Areas.Admin.Controllers
{
    public class MainController : Controller
    {
        QuanLyThuVienEntities database = new QuanLyThuVienEntities();      
       
        /******** Login ************/
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Account_Admin ad)
        {
            var check = database.Account_Admin.Where(s => s.User.Equals(ad.User) && s.Password.Equals(ad.Password)).FirstOrDefault();
            if (check == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View("Index", ad);
            }
            else
            {                
                Session["User"] = ad.User;
                return RedirectToAction("Main", "Main");
            }
        }

        /*----- Đăng xuất -----*/

        public ActionResult Logout(Account_Admin ad)
        {            
            Session.Abandon();
            return RedirectToAction("Index", "Main");
          
        }

        /*---- Đổi mật khẩu ----*/
        public ActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePass(Account_Admin ad, string _name, string _newPass)
        {
            ad = database.Account_Admin.Where(s => s.MaAccount == 1 && s.Password == _name).FirstOrDefault();
            //var check = database.Account_Admin.Where(s=>s.Password == _name && s.MaAccount==1).FirstOrDefault();            
            if (ad != null)
            {

                //database.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                ad.MaAccount = 1;
                ad.User = "admin";
                ad.Password = _newPass;
                database.SaveChanges();
                return RedirectToAction("Main", "Main");
            }
            else
            {
                ViewBag.Error_ChangePass = "Mật khẩu cũ không đúng!";
                return View("ChangePass", ad);
            }

        }


        /******** Main ************/
        public ActionResult Main()
        {
            /*Kiểm tra xem đã đăng nhập chưa ?*/
            if (Session["User"] != null)
            {
                return View();
            }
            else
                return RedirectToAction("Index", "Main");
                     
        }

        /* ---- Đầu sách -----*/
        public ActionResult DauSach(string _name)
        {
            if (Session["User"] != null)
            {
                if (_name == null)
                {
                    return View(database.DauSaches.ToList());
                }
                else
                    return View(database.DauSaches.Where(s => s.TenSach.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");
            
        }

        public ActionResult ThemDauSach()
        {
            return View();
        }
        public ActionResult SuaDauSach(int id)
        {
            return View(database.DauSaches.Where(a=>a.MaDauSach==id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult SuaDauSach(int id, DauSach a)
        {
            database.Entry(a).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            ViewBag.SuaDauSach_suss = "Sửa thành công !!!";
            return View("SuaDauSach", a);
        }

        /*----- Sách -----*/
        public ActionResult Sach(string _name)
        {
            if (Session["User"] != null)
            {
                if (_name == null)
                {
                    return View(database.Saches.ToList());
                }
                else
                    return View(database.Saches.Where(s => s.DauSach.TenSach.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");
        }

        public ActionResult ThemSach(int id)
        {
            ViewBag.Id_dauSach = id.ToString();
            return View(/*database.DauSaches.Where(a => a.MaDauSach == id).FirstOrDefault()*/);
        }
        [HttpPost]
        public ActionResult ThemSach(int id,string sks)
        {
            var a = new Sach();
            a.MaDauSach = id;
            a.MaTinhTrangSach = 1;
            a.SoKiemSoat = int.Parse(sks);
            database.Saches.Add(a);
            database.SaveChanges();
            return RedirectToAction("DauSach", "Main");
        }

        /*----- Thẻ TV -----*/
        public ActionResult DsTheThuVien(string _name)
        {
            if (Session["User"] != null)
            {
                if (_name == null)
                {
                    return View(database.TheThuViens.ToList());
                }
                else
                    return View(database.TheThuViens.Where(s => s.MaThe.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");
            
        }
        /*----- Đăng ký thẻ TV -----*/
        public ActionResult DsDangKyTheTV(string _name)
        {
            if (Session["User"] != null)
            {
                if (_name == null)
                {
                    return View(database.DangKyTheTVs.ToList());
                }
                else
                    return View(database.DangKyTheTVs.Where(s => s.MaThe.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");
            
        }

       


      
    }
}