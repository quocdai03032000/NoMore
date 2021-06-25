using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn1_QuanLyThuVien.Models;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using PagedList;
using PagedList.Mvc;

namespace DoAn1_QuanLyThuVien.Areas.User.Controllers
{
    public class MainUserController : Controller
    {
        QuanLyThuVienEntities database = new QuanLyThuVienEntities();
        public ActionResult Index(string text, int? page)
        {
            var dausach = database.DauSaches.ToList().OrderByDescending(s => s.MaDauSach).ToList();
            int pageSize = 16;
            int pageNum = (page ?? 1);
            var dausach_search = database.DauSaches.Where(s => s.TenSach.Contains(text)).ToList().OrderByDescending(s => s.MaDauSach).ToList();
            
            if (dausach_search.Count==0)
            {
                dausach_search = database.DauSaches.Where(s => s.TheLoai.Contains(text)).ToList();
            }            
            if (text != null)
            {
                return View(dausach_search.ToPagedList(pageNum, pageSize));
            }                
            else
            {
                return View(dausach.ToPagedList(pageNum, pageSize));
            }
        }        

        /*----- LOGIN -----*/
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string tk,string pass)
        {
            //kiểm tra tài khoản có trong bảng thẻ tv và có bị block hay không?
            var check = database.TheThuViens.Where(a => a.MaThe == tk && a.Password == pass && a.MaTinhTrang==2 || a.MaThe == tk && a.Password == pass && a.MaTinhTrang == 3).FirstOrDefault();
            //kiểm tra tài khoản đã được kích hoạt chưa ?
            var checkActive = database.DangKyTheTVs.Where(a => a.MaThe == tk).FirstOrDefault();
            //kiểm tra tài khoản có bị block hay k ?
            var checkBlock = database.TheThuViens.Where(a => a.MaThe == tk && a.Password == pass && a.MaTinhTrang == 1).FirstOrDefault();
            if (check!=null)
            {
                Session["User"] = check.HoTen;
                Session["MSSV"] = check.MaThe;
                return RedirectToAction("Index", "MainUser");               
            }
            else if(checkBlock!=null)
            {
                ViewBag.MessLogin = "Tài khoản của bạn đã bị BLOCK";
                return View("Login");
            }
            else if(checkActive!=null)
            {
                ViewBag.MessLogin = "Tài khoản chưa được kích hoạt";
                return View("Login");
            }
            else
            {
                ViewBag.MessLogin = "Sai tài khoản hoặc mật khẩu!";
                return View("Login");
            }
        }
        /*----- Logout -----*/
        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "MainUser");

        }

        #region Thẻ thư viện
        /*----- Tạo thẻ thư viện -----*/
        public ActionResult Register()
        {
            DangKyTheTV theTV = new DangKyTheTV();
            return View(theTV);
        }       

        [HttpPost]
        public ActionResult Register(DangKyTheTV theTV,string ConfirmPass, string email)
        {
            var CheckTheTV = database.DangKyTheTVs.Where(a => a.MaThe == theTV.MaThe).SingleOrDefault();
            var check = database.TheThuViens.Where(a => a.MaThe == theTV.MaThe).SingleOrDefault();       
            
            if (theTV.Password == ConfirmPass)
            {
                if (CheckTheTV == null && check == null)
                {
                    theTV.NgayLam = DateTime.Now;
                    database.DangKyTheTVs.Add(theTV);
                    database.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Đã tồn tại MSSV này!";
                    return View("Register");
                }                
            }
            else
            {
                ViewBag.Error = "Mật khẩu không trùng khớp";
                return View("Register");
            }
        }
        #endregion

        [HttpGet]
        public ActionResult ChiTietSach(int id)
        {
            var dauSach = database.DauSaches.Where(a => a.MaDauSach == id).FirstOrDefault();
            var soLuong = database.Saches.Where(a => a.MaDauSach == id && a.MaTinhTrangSach == 1).Count();
            ViewBag.TenDauSach = dauSach.TenSach;
            ViewBag.NamXB = dauSach.NamXuatBan;
            ViewBag.SoLuong = soLuong.ToString();
            ViewBag.TheLoai = dauSach.TheLoai;
            ViewBag.TacGia = dauSach.TacGia;
            ViewBag.hinhAnh = dauSach.HinhAnh;
            ViewBag.NXB = dauSach.NhaXuatBan;
            return View(database.Saches.Where(a=>a.MaDauSach==id&& a.MaTinhTrangSach==1).ToList());
        }

        /*----------------- Giỏ hàng --------------------*/
        #region Giỏ hàng
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public ActionResult CartNull()
        {
            return View();
        }

        public ActionResult Cart()
        {
            //if (Session["Cart"] == null)
            //    return RedirectToAction("Cart", "MainUser");

            if (Session["User"] != null)
            {                
                Cart _cart = Session["Cart"] as Cart;
                if(_cart == null)
                {
                    return RedirectToAction("CartNull", "MainUser");
                }
                else
                {
                    ViewBag.TenUse = Session["User"];
                    ViewBag.MSSV = Session["MSSV"];
                    return View(_cart);
                }
                
            }
            else
            {
                return RedirectToAction("Login", "MainUser");
            }
        }

        public ActionResult AddToCart(int id)
        {
            if (Session["User"] != null)
            {
               
                var sach = database.Saches.SingleOrDefault(s => s.id == id);
                string hinhAnh = sach.DauSach.HinhAnh;

                if (sach != null)
                {
                    GetCart().Add_Sach_Cart(sach, hinhAnh);                    
                    return RedirectToAction("Index", "MainUser");
                }
                else
                {                   
                    return RedirectToAction("Index");
                }                
            }
            else
                return RedirectToAction("Login", "MainUser");            
        }
                                                                                                                                                              
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);
            return RedirectToAction("Cart", "MainUser");
            
        }
        #endregion
        /*-------------- Thuê sách  ------------*/
        public ActionResult ThueSach(TheThuVien tv)
        {
            //Phần mượn sách cũ -  đã oke
            #region cũ
            //Cart cart = Session["Cart"] as Cart;
            //string maThe = Session["MSSV"].ToString();
            //int soLuong = cart.Items.Count();
            //string mssv = Session["MSSV"].ToString();
            //tv = database.TheThuViens.Where(a => a.MaThe == mssv).SingleOrDefault();
            //// Kiểm tra số lượng - chỉ được mượn tối đa 3 cuốn
            //if(soLuong <=3)
            //{
            //    if (tv.MaTinhTrang == 2)
            //    {
            //        foreach (var item in cart.Items)
            //        {
            //            var BookStatus = database.Saches.Where(a => a.id == item.sach.id).SingleOrDefault();
            //            // cập nhật tình trạng sách
            //            BookStatus.MaTinhTrangSach = 2;
            //            DKyMuonSach ThueSach = new DKyMuonSach();                        
            //            ThueSach.MaSach = item.sach.id;
            //            ThueSach.MaThe = maThe;
            //            database.DKyMuonSaches.Add(ThueSach);
            //        }
            //        // cập nhật tài khoản sang trạng thái đang mượn
            //        tv.MaTinhTrang = 3;
            //        database.SaveChanges();
            //        Session["MessThueSach"] = "suss";
            //        return RedirectToAction("Cart", "MainUser");
            //    }
            //    else
            //    {
            //        Session["MessThueSach"] = "fail";
            //        return RedirectToAction("Cart", "MainUser");
            //    }
            //}
            //else
            //{
            //    Session["FailAddCart"] = "fail";
            //    return RedirectToAction("Cart", "MainUser");
            //}

            #endregion
            /*-----------------------------------*/

            //Phần mượn sách mở rộng (Được  mượn tối đa 5 cuốn, mượn rồi được mượn tiếp, miển là <= 5 cuốn)
            #region new

            Cart cart = Session["Cart"] as Cart;
            string maThe = Session["MSSV"].ToString();
            int soLuong = cart.Items.Count();
            string mssv = Session["MSSV"].ToString();
            tv = database.TheThuViens.Where(a => a.MaThe == mssv).SingleOrDefault();
            int total = database.DKyMuonSaches.Where(a => a.MaThe == mssv).Count() + database.Sach_Dang_Muon.Where(a =>a.MaThe==mssv).Count();
            Session["MuonToiDa"] = soLuong+total;
            if (soLuong+total <= 5)
            {
                foreach (var item in cart.Items)
                {
                    var BookStatus = database.Saches.Where(a => a.id == item.sach.id).SingleOrDefault();
                    // cập nhật tình trạng sách
                    BookStatus.MaTinhTrangSach = 2;
                    DKyMuonSach ThueSach = new DKyMuonSach();
                    ThueSach.MaSach = item.sach.id;
                    ThueSach.MaThe = maThe;
                    database.DKyMuonSaches.Add(ThueSach);                   
                }

                cart.RemoveAllItem();
                // cập nhật tài khoản sang trạng thái đang mượn
                tv.MaTinhTrang = 3;
                database.SaveChanges();
                Session["MessThueSach"] = "suss";               
                return RedirectToAction("Cart", "MainUser");
            }
            else
            {
                Session["FailAddCart"] = "fail";
                return RedirectToAction("Cart", "MainUser");
            }           

            #endregion

        }

        /*-------------- Đổi mật khẩu -------------*/
        public ActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePass(TheThuVien ad, string _name, string _newPass, string _nhapLaiPass)
        {
            string mssv = Session["MSSV"].ToString();
            ad = database.TheThuViens.Where(s => s.MaThe == mssv && s.Password == _name).FirstOrDefault();
            //var check = database.Account_Admin.Where(s=>s.Password == _name && s.MaAccount==1).FirstOrDefault();            
            if (ad != null)
            {
                if (_newPass == _nhapLaiPass)
                {
                    ad.Password = _newPass;
                    database.Entry(ad).State = System.Data.Entity.EntityState.Modified;
                    database.SaveChanges();
                    ViewBag.Error_ChangePass = "Đổi mật khẩu thành công";
                }
                else
                {
                    ViewBag.Error_ChangePass = "Mật khẩu không trùng khớp";
                }
                return View("ChangePass", ad);
            }
            else
            {
                ViewBag.Error_ChangePass = "Mật khẩu cũ không đúng!";
                return View("ChangePass", ad);
            }

        }

        public ActionResult XemThongTinChiTiet()
        {
            if (Session["User"] != null)
            {
                string mssv = Session["MSSV"].ToString();
                var sv = database.TheThuViens.Where(a => a.MaThe == mssv).SingleOrDefault();
                ViewBag.ctmssv = mssv;
                ViewBag.ctHoTen = sv.HoTen;
                ViewBag.ctTinhTrang = sv.TinhTrang_TheTV.TenTinhTrang;
                ViewBag.ctNgayLamThe = sv.NgayLam;
                ViewBag.ctNgayHetHan = sv.NgayHetHan;
                //------------------------------------
                int SlSach = database.Sach_Dang_Muon.Where(a => a.MaThe == mssv).Count();
                var SachMuon = database.Sach_Dang_Muon.Where(a => a.MaThe == mssv).ToList();
                var NgayThang = database.Sach_Dang_Muon.Where(a => a.MaThe == mssv).FirstOrDefault();
                if (SlSach > 0)
                {
                    ViewBag.ctSoLuong = SlSach;
                    foreach (var item in SachMuon)
                    {
                        ViewBag.SachDangMuon += item.Sach.DauSach.TenSach + ", ";
                    }
                    ViewBag.ctNgayMuon = NgayThang.NgayMuon.ToString();
                    DateTime NgayTra = (DateTime)NgayThang.NgayMuon;
                    NgayTra = NgayTra.AddDays(15);
                    ViewBag.ctNgayTra = NgayTra;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "MainUser");
            }
            
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}