using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAn1_QuanLyThuVien.Models;
using System.IO;
using Common;

namespace DoAn1_QuanLyThuVien.Areas.Admin.Controllers
{
    public class MainController : Controller
    {
        QuanLyThuVienEntities database = new QuanLyThuVienEntities();

        #region ĐĂNG NHẬP - ĐĂNG XUẤT - ĐỔI PASS
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
                Session["Admin"] = ad.User;
                return RedirectToAction("DauSach", "Main");
            }
        }

        /*----- Đăng xuất -----*/

        public ActionResult Logout(Account_Admin ad)
        {
            Session["Admin"] = null;
            return RedirectToAction("Index", "Main");

        }

        /*---- Đổi mật khẩu ----*/
        public ActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePass(Account_Admin ad, string _name, string _newPass, string _nhapLaiPass)
        {
            ad = database.Account_Admin.Where(s => s.MaAccount == 1 && s.Password == _name).FirstOrDefault();
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

#endregion

        /******** Main ************/
        public ActionResult Main()
        {
            /*Kiểm tra xem đã đăng nhập chưa ?*/
            if (Session["Admin"] != null)
            {
                return View();
            }
            else
                return RedirectToAction("Index", "Main");

        }

        #region QUẢN LÝ ĐẦU SÁCH
        /* ---- Đầu sách -----*/
        public ActionResult DauSach(string _name)
        {
            var dausach = database.DauSaches.ToList().OrderByDescending(s => s.MaDauSach).ToList();
            if (Session["Admin"] != null)
            {
                if (_name == null)
                {
                    return View(dausach);
                }
                else
                    return View(database.DauSaches.Where(s => s.TenSach.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");

        }

        public ActionResult ThemDauSach()
        {
            DauSach ds = new DauSach();
            return View(ds);
        }
        [HttpPost]
        public ActionResult ThemDauSach(int MaDauSach, DauSach sach,string imageUploader,string _name,string _tg,string _tl,int year,string NXB)
        {

            try
            {
                if (sach.imageUploader != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(sach.imageUploader.FileName);
                    string extension = Path.GetExtension(sach.imageUploader.FileName);
                    fileName = fileName + extension;
                    sach.HinhAnh = "~/Content/imgsach/" + fileName;
                    sach.imageUploader.SaveAs(Path.Combine(Server.MapPath("~/Content/imgsach/"), fileName));
                }
               
                var check = database.DauSaches.Where(a => a.MaDauSach == MaDauSach).SingleOrDefault();

                if (check == null)
                {
                    sach.MaDauSach = MaDauSach;
                    sach.TenSach = _name;
                    sach.TacGia = _tg;
                    sach.TheLoai = _tl;
                    sach.NamXuatBan = year;
                    sach.NhaXuatBan = NXB;
                    sach.SoLuong = 0;
                    
                    database.DauSaches.Add(sach);
                    database.SaveChanges();
                    return RedirectToAction("DauSach", "Main");

                }
                else
                {
                    ViewBag.ThemDauSach = "Trùng mã đầu sách";
                    return View(sach);
                }
            }
            catch
            {
                return View(Content("Dữ Liệu đã tồn tại!!"));
            }

        }


        public ActionResult SuaDauSach(int id)
        {
            return View(database.DauSaches.Where(a => a.MaDauSach == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult SuaDauSach(int id, DauSach a, string imageUploader)
        {        

            a = database.DauSaches.Where(item => item.MaDauSach == id).SingleOrDefault();
            a.HinhAnh = "~/Content/imgsach/" + imageUploader;
            database.Entry(a).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            ViewBag.SuaDauSach_suss = "Sửa thành công !!!";
            return View("SuaDauSach", a);
        }
        [HttpGet]
        public ActionResult XoaDauSach(int id)
        {
            var check = database.Saches.Where(a => a.MaDauSach == id).FirstOrDefault();
            if (check == null)
            {
                var DauSach = database.DauSaches.Where(a => a.MaDauSach == id).SingleOrDefault();
                database.DauSaches.Remove(DauSach);
                database.SaveChanges();
            }
            else
            {
                ViewBag.Error_XoaDauSach = "Không thể xoá vì còn tồn tại sách thuộc đầu sách này";
            }
            return RedirectToAction("DauSach", "Main");

        }
        #endregion


        /*----- Sách -----*/
        public ActionResult Sach(string _name)
        {
            if (Session["Admin"] != null)
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
        /*----- Thêm Sách -----*/
        public ActionResult ThemSach(int id)
        {
            var flag = database.DauSaches.Where(a => a.MaDauSach == id).SingleOrDefault();
            ViewBag.Id_dauSach = id.ToString();
            ViewBag.Ten_dauSach = flag.TenSach;
            return View();
        }
        [HttpPost]
        public ActionResult ThemSach(int id, string sks, string GhiChu)
        {
            var DauSach = database.DauSaches.Where(a => a.MaDauSach == id).SingleOrDefault();
            var checkSoLuong = database.Saches.Where(a => a.MaDauSach == id).Count();
            int soLuongSach = int.Parse(sks);
            if(checkSoLuong==0)
            {
                for(int i=1; i<=soLuongSach;i++)
                {
                    var a = new Sach();
                    a.MaDauSach = id;
                    a.MaTinhTrangSach = 1;
                    a.SoKiemSoat = i;
                    a.GhiChu = GhiChu;
                    database.Saches.Add(a);
                }
                /*----- Cap nhat so luong -----*/
                DauSach.SoLuong += soLuongSach;
                
                database.SaveChanges();
                ViewBag.ThemSach_message_suss = "Thêm thành công!";
                return RedirectToAction("DauSach", "Main");
            }
            else
            {
                for(int i = checkSoLuong+1; i<=checkSoLuong+soLuongSach; i++)
                {
                    var a = new Sach();
                    a.MaDauSach = id;
                    a.MaTinhTrangSach = 1;
                    a.SoKiemSoat = i;
                    database.Saches.Add(a);
                }
                /*----- Cap nhat so luong -----*/
                DauSach.SoLuong += soLuongSach;
                
                database.SaveChanges();
                ViewBag.ThemSach_message_suss = "Thêm thành công!";
                return RedirectToAction("DauSach", "Main");
            }



            #region Chức năng cũ, thêm sách nhập số kiểm soát bằng tay
            //int soKiemSoat = int.Parse(sks);
            //var DauSach = database.DauSaches.Where(a => a.MaDauSach == id).SingleOrDefault();
            //var check = database.Saches.Where(a => a.MaDauSach == id && a.SoKiemSoat == soKiemSoat).SingleOrDefault();
            //if (check == null)
            //{
            //    var a = new Sach();
            //    a.MaDauSach = id;
            //    a.MaTinhTrangSach = 1;
            //    a.SoKiemSoat = soKiemSoat;
            //    /*----- Cap nhat so luong -----*/
            //    DauSach.SoLuong += 1;
            //    database.Saches.Add(a);
            //    database.SaveChanges();
            //    ViewBag.ThemSach_message_suss = "Thêm thành công!";
            //    return RedirectToAction("DauSach", "Main");
            //}
            //else
            //{
            //    ViewBag.ThemSach_message_error = "Trùng số kiểm soát, hãy nhập lại!";
            //    return View("ThemSach");
            //}
            #endregion
        }
        /*----- Xoá sách -----*/
        [HttpGet]
        public ActionResult XoaSach(int id)
        {
            var xoaSach = database.Saches.Where(a => a.id == id).SingleOrDefault();
            var dauSach = database.DauSaches.Where(a => a.MaDauSach == xoaSach.MaDauSach).SingleOrDefault();
            if(xoaSach.MaTinhTrangSach!=2)
            {
                /*----- Cap nhat so luon dau sach ----- */
                dauSach.SoLuong -= 1;
                database.Saches.Remove(xoaSach);
                database.SaveChanges();
                return RedirectToAction("Sach", "Main");
            }
            else
            {
                Session["XoaSachMess"] = "yes";
                return RedirectToAction("Sach", "Main");
            }
            
        }

        /*----- Thẻ TV -----*/
        public ActionResult DsTheThuVien(string _name)
        {
            if (Session["Admin"] != null)
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
        /*----- block và mở block thẻ thư viện -----*/
        [HttpGet]
        public ActionResult BlockTheThuVien(string id)
        {
            var theTV = database.TheThuViens.Where(a => a.MaThe == id).SingleOrDefault();
            if(theTV.MaTinhTrang!=1)
            {
                theTV.MaTinhTrang = 1;
                database.Entry(theTV).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
                ViewBag.Mess_BlockTheThuVien = "Khoá thẻ thành công";
            }else if(theTV.MaTinhTrang == 1)
            {
                theTV.MaTinhTrang = 2;
                database.Entry(theTV).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
                ViewBag.Mess_BlockTheThuVien = "Mở thẻ thành công";
            }            
            return RedirectToAction("DsTheThuVien");
        }
        /*----- Xoá thẻ thư viện -----*/
        [HttpGet]
        public ActionResult XoaTheThuVien(string id)
        {
            var xoaTheTV = database.TheThuViens.Where(a => a.MaThe == id).SingleOrDefault();
            // kiểm tra chỉ được xoá khi tài khoản k mượn sách
            if (xoaTheTV.MaTinhTrang!=3)
            {                
                database.TheThuViens.Remove(xoaTheTV);
                database.SaveChanges();
                return RedirectToAction("DsTheThuVien", "Main");
            }
            else
            {
                Session["XoaTheThuVien"] = "true";
                return RedirectToAction("DsTheThuVien", "Main");
            }

            
        }

        /*----- Reset Password -----*/
        [HttpGet]
        public ActionResult ResetPass(string id)
        {
            var theTV = database.TheThuViens.Where(a => a.MaThe == id).FirstOrDefault();
            theTV.Password = "123";
            database.SaveChanges();
            return RedirectToAction("DsTheThuVien","Main");
        }


        /*----- Danh Sách đăng ký thẻ TV -----*/
        public ActionResult DsDangKyTheTV(string _name)
        {
            if (Session["Admin"] != null)
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
        /*----- Thêm thẻ thư viện từ danh sách đăng ký thẻ -----*/
        [HttpGet]
        public ActionResult ThemTheTV_form_DS(int id)
        {
            //Thêm thẻ thư viện từ bảng đăng ký thẻ sang thẻ thư viện
            var DangKyTheTV = database.DangKyTheTVs.Where(a => a.MaDangKyThe == id).SingleOrDefault();
            string Email = DangKyTheTV.email;
            var TheTV = new TheThuVien();
            TheTV.MaThe = DangKyTheTV.MaThe;
            TheTV.MaTinhTrang = 2;
            TheTV.Password = DangKyTheTV.Password;
            TheTV.HoTen = DangKyTheTV.HoTen;
            TheTV.NgayLam = DangKyTheTV.NgayLam;
            TheTV.NgayHetHan = DateTime.Now.AddYears(4);
            TheTV.email = DangKyTheTV.email;
            database.TheThuViens.Add(TheTV);
            //Đồng thời xoá thẻ tv trong bảng đăng ký thẻ tv
            database.DangKyTheTVs.Remove(DangKyTheTV);
            database.SaveChanges();

            //Gửi mail khi tài khoản được kích hoạt

            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/Admin/template/neworder.html"));
            content = content.Replace("{{CustommerName}}", DangKyTheTV.MaThe);
            content = content.Replace("{{CustommerPass}}", DangKyTheTV.Password);
            new MailHelper().SendMail(Email, "Kích hoạt tài khoản thành công", content);

            //về lại trang
            return RedirectToAction("DsDangKyTheTV", "Main");
        }

        /*----- Xoá thẻ thư viện từ danh sách đăng ký thẻ -----*/
        [HttpGet]
        public ActionResult XoaTheTV_form_DS(int id)
        {
            var DangKyTheTV = database.DangKyTheTVs.Where(a => a.MaDangKyThe == id).SingleOrDefault();
            database.DangKyTheTVs.Remove(DangKyTheTV);
            database.SaveChanges();
            return RedirectToAction("DsDangKyTheTV", "Main");
        }


        #region Quản Lý mượn trả

        public ActionResult QLMuonTraSach(string _name)
        {
            if (Session["Admin"] != null)
            {
                if (_name == null)
                {
                    return View(database.Sach_Dang_Muon.ToList());
                }
                else
                    return View(database.Sach_Dang_Muon.Where(s => s.MaThe.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");

        }
        //Trả sách
        public ActionResult TraSach(int id)
        {
            var del = database.Sach_Dang_Muon.Where(a => a.MaSachMuon == id).SingleOrDefault();
            string mssv = del.MaThe;
            //thêm vào báo cáo
            BaoCaoTraSach baocao = new BaoCaoTraSach();
            baocao.MaSach = del.MaSach;
            baocao.TenSach = del.Sach.DauSach.TenSach;
            baocao.HoTen = del.TheThuVien.HoTen;
            baocao.MSSV = del.MaThe;
            baocao.NgayTra = DateTime.Now;
            baocao.NgayMuon = del.NgayMuon;
            database.BaoCaoTraSaches.Add(baocao);


            //Cập tình trạng sách
            var sach = database.Saches.Where(a => a.id == del.MaSach).FirstOrDefault();
            sach.MaTinhTrangSach = 1;

            database.SaveChanges();
            //xoá sách
            database.Sach_Dang_Muon.Remove(del);
            database.SaveChanges();

            //kiểm tra và cập nhật lại tình trạng thẻ TV
            var check = database.Sach_Dang_Muon.Where(a => a.MaThe == mssv).FirstOrDefault();
            if(check == null)
            {
                var the = database.TheThuViens.Where(a => a.MaThe == mssv).SingleOrDefault();
                the.MaTinhTrang = 2;
                database.SaveChanges();
            }
            return RedirectToAction("QLMuonTraSach", "Main");
        }


        public ActionResult DSDangKy_MuonSach(string _name)
        {
            //if (Session["Admin"] != null)
            //{

            //}
            //else
            //    return RedirectToAction("Index", "Main");
            if (_name == null)
            {
                return View(database.DKyMuonSaches.ToList());
            }
            else
                return View(database.DKyMuonSaches.Where(s => s.MaThe.Contains(_name)).ToList());
        }
        //Xác nhận mượn sách
        // chuyển sách từ bảng đăng ký mượn sách sang bảng sách đang được mượn
        [HttpGet]
        public ActionResult Them_DSDangKy_MuonSach(int id)
        {
            var DK_MuonSach = database.DKyMuonSaches.Where(a => a.MaDangKyMuonSach == id).SingleOrDefault();
            var Sach_muon = new Sach_Dang_Muon();
            Sach_muon.MaThe = DK_MuonSach.MaThe;
            Sach_muon.MaSach = DK_MuonSach.MaSach;
            Sach_muon.NgayMuon = DateTime.Now;
            database.Sach_Dang_Muon.Add(Sach_muon);

            //Them vao báo cáo
            string mssv = DK_MuonSach.MaThe;
            string tenSach = DK_MuonSach.Sach.DauSach.TenSach;
            string hoTen = DK_MuonSach.TheThuVien.HoTen;
            var maSach = DK_MuonSach.MaSach;
            //----
            var baocao = new BaoCaoMuonSach();
            baocao.NgayMuon = DateTime.Now;
            baocao.Mssv = mssv;
            baocao.TenSach = tenSach;
            baocao.HoTen = hoTen;
            baocao.MaSach = maSach;
            database.BaoCaoMuonSaches.Add(baocao);

            database.DKyMuonSaches.Remove(DK_MuonSach);
            database.SaveChanges();
            return RedirectToAction("DSDangKy_MuonSach", "Main");



            //var DK_MuonSach = database.DKyMuonSaches.Where(a => a.MaDangKyMuonSach == id).FirstOrDefault();
            //var Sach_muon = new Sach_Dang_Muon();
            //Sach_muon.MaThe = DK_MuonSach.MaThe;
            //Sach_muon.MaSach = DK_MuonSach.MaSach;
            //Sach_muon.NgayMuon = DateTime.Now;
            //database.Sach_Dang_Muon.Add(Sach_muon);
            //database.SaveChanges();
            //Them vao báo cáo
            //BaoCaoMuonSach baocao = new Models.BaoCaoMuonSach();
            //baocao.NgayMuon = DateTime.Now;
            //baocao.Mssv = DK_MuonSach.MaThe;
            //baocao.TenSach = DK_MuonSach.Sach.DauSach.TenSach;
            //baocao.HoTen = DK_MuonSach.TheThuVien.HoTen;
            //baocao.MaSach = DK_MuonSach.MaSach;
            //database.BaoCaoMuonSaches.Add(baocao);
            //database.SaveChanges();
            //database.DKyMuonSaches.Remove(DK_MuonSach);
            //database.SaveChanges();
            //return RedirectToAction("DSDangKy_MuonSach", "Main");
        }
        // xoá đối tượng ra khỏi bảng đăng ký mượn sách
        [HttpGet]
        public ActionResult Xoa_DSDangKy_MuonSach(int id)
        {   
            var flag = database.DKyMuonSaches.Where(a => a.MaDangKyMuonSach == id).SingleOrDefault();
            var del = database.DKyMuonSaches.Where(a => a.MaDangKyMuonSach == id).SingleOrDefault();
            //Cập tình trạng sách
            var sach = database.Saches.Where(a => a.id == del.MaSach).FirstOrDefault();                      
            sach.MaTinhTrangSach = 1;
            //         
            string mssv = flag.MaThe;
            database.DKyMuonSaches.Remove(del);
            database.SaveChanges();
            // kiểm tra và cập nhật lại tình trạng thẻ

            var check1 = database.DKyMuonSaches.Where(a => a.MaThe == mssv).FirstOrDefault();
            var check2 = database.Sach_Dang_Muon.Where(a => a.MaThe == mssv).FirstOrDefault();
            if (check1 == null && check2 == null)
            {
                var the = database.TheThuViens.Where(a => a.MaThe == mssv).SingleOrDefault();
                the.MaTinhTrang = 2;
            }
            //
            database.SaveChanges();
            return RedirectToAction("DSDangKy_MuonSach", "Main");
        }      
        #endregion

        public ActionResult BaoCaoMuonSach(string _name)
        {
            if (Session["Admin"] != null)
            {
                if (_name == null)
                {
                    return View(database.BaoCaoMuonSaches.ToList());
                }
                else
                    return View(database.BaoCaoMuonSaches.Where(s => s.Mssv.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");
        }
        public ActionResult BaoCaoTraSach(string _name)
        {
            if (Session["Admin"] != null)
            {
                if (_name == null)
                {
                    return View(database.BaoCaoTraSaches.ToList());
                }
                else
                    return View(database.BaoCaoTraSaches.Where(s => s.MSSV.Contains(_name)).ToList());
            }
            else
                return RedirectToAction("Index", "Main");
        }

        public ActionResult BaoCaoTinhTrang()
        {
            var slDauSach = database.DauSaches.Count();
            var slSach = database.Saches.Count();
            var slSachMuon = database.Sach_Dang_Muon.Count();
            var slSachHong = database.Saches.Where(a => a.MaTinhTrangSach == 4).Count();
            var slSachMat = database.Saches.Where(a => a.MaTinhTrangSach == 3).Count();
            ViewBag.slDauSach = slDauSach.ToString();
            ViewBag.slSach = slSach.ToString();
            ViewBag.slSachMuon = slSachMuon.ToString();
            ViewBag.slSachHong = slSachHong.ToString();
            ViewBag.slSachMat = slSachMat.ToString();
            return View();
        }

        public ActionResult ThemTheTV()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemTheTV(string MSSV,string HoTen, string email)
        {
            var check = database.TheThuViens.Where(a => a.MaThe == MSSV).FirstOrDefault();
            if(check==null)
            {
                TheThuVien theTV = new TheThuVien();
                theTV.MaThe = MSSV;
                theTV.HoTen = HoTen;
                theTV.email = email;

                theTV.NgayLam = DateTime.Now;
                theTV.NgayHetHan = DateTime.Now.AddYears(4);
                theTV.MaTinhTrang = 2;
                theTV.Password = "123";
                database.TheThuViens.Add(theTV);
                database.SaveChanges();
                ViewBag.ThemTheTV = "Thêm thành công";
                return View("ThemTheTV");
            }
            else
            {
                ViewBag.ThemTheTV = "Trùng MSSV";
                return View("ThemTheTV");
            }
        }
    }
}