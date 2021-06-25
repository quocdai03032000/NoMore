using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn1_QuanLyThuVien.Models
{
    public class Cart
    {
        QuanLyThuVienEntities database = new QuanLyThuVienEntities();
        public class CartItem
        {
            public Sach sach { get; set; }
            public string MSSV { get; set; }
            public string HinhAnh { get; set; }
        }
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem>Items
        {
            get { return items; }
        }
        public void Add_Sach_Cart(Sach _sach,string hinhAnh)
        {
            var check = Items.FirstOrDefault(s => s.sach.id == _sach.id);
            if (check == null)
                items.Add(new CartItem
                {
                    sach = _sach,
                    HinhAnh = hinhAnh
                });
        }
        public void Remove_CartItem(int id)
        {
            items.RemoveAll(s => s.sach.id == id);
        }
        public void RemoveAllItem()
        {
            items.Clear();
        }
        public void Thue_Sach()
        {
            DKyMuonSach muonSach = new DKyMuonSach();
            foreach(var a in items )
            {
                muonSach.MaSach = a.sach.id;
                muonSach.MaThe = a.MSSV;
                database.DKyMuonSaches.Add(muonSach);
            }
            database.SaveChanges();
        }
    }
    
}