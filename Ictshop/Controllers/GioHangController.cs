using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Controllers
{
    public class GioHangController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        //Lấy giỏ hàng của người dùng hiện tại
        public List<GioHang> LayGioHang()
        {
            List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang == null)
            {
                gioHang = new List<GioHang>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }

        //Thêm sản phẩm vào giỏ hàng
        public ActionResult ThemGioHang(int iMasp, string strURL)
        {
            if (Session["use"] == null)
            {
                TempData["CantAddToCartWithoutLogin"] = "Hãy đăng nhập trước khi thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Dangnhap", "User");
            }

            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMasp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            List<GioHang> gioHang = LayGioHang();
            GioHang sanpham = gioHang.Find(n => n.iMasp == iMasp);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMasp);
                gioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }

        //Cập nhật giỏ hàng
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            List<GioHang> gioHang = LayGioHang();
            GioHang sanpham = gioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        //Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            List<GioHang> gioHang = LayGioHang();
            GioHang sanpham = gioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            if (sanpham != null)
            {
                gioHang.RemoveAll(n => n.iMasp == iMaSP);
            }

            if (gioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }

        //Hiển thị giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<GioHang> gioHang = LayGioHang();
            return View(gioHang);
        }

        //Tính tổng số lượng sản phẩm trong giỏ hàng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang != null)
            {
                iTongSoLuong = gioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        //Tính tổng tiền của giỏ hàng
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> gioHang = Session["GioHang"] as List<GioHang>;
            if (gioHang != null)
            {
                dTongTien = gioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }

        //Hiển thị phần giỏ hàng (partial view)
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        //Hiển thị trang chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> gioHang = LayGioHang();
            return View(gioHang);
        }

        // Chức năng đặt hàng
        [HttpPost]
        public ActionResult DatHang(FormCollection donhangForm)
        {
            if (Session["use"] == null || Session["use"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }

            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            string diachinhanhang = donhangForm["Diachinhanhang"].ToString();
            string thanhtoan = donhangForm["MaTT"].ToString();
            int ptthanhtoan = Int32.Parse(thanhtoan);

            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["use"];
            List<GioHang> gh = LayGioHang();
            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;
            ddh.Thanhtoan = ptthanhtoan;
            ddh.Diachinhanhang = diachinhanhang;
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }
            ddh.Tongtien = tongtien;
            db.Donhangs.Add(ddh);
            db.SaveChanges();

            foreach (var item in gh)
            {
                Chitietdonhang ctDH = new Chitietdonhang();
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                ctDH.Madon = ddh.Madon;
                ctDH.Masp = item.iMasp;
                ctDH.Soluong = item.iSoLuong;
                ctDH.Dongia = (decimal)item.dDonGia;
                ctDH.Thanhtien = (decimal)thanhtien;
                ctDH.Phuongthucthanhtoan = 1;
                db.Chitietdonhangs.Add(ctDH);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Donhangs");
        }

        // Hiển thị trang thanh toán đơn hàng
        public ActionResult ThanhToanDonHang()
        {
            ViewBag.MaTT = new SelectList(new[]
            {
                new { MaTT = 1, TenPT="Thanh toán tiền mặt" },
                new { MaTT = 2, TenPT="Thanh toán chuyển khoản" },
            }, "MaTT", "TenPT", 1);

            ViewBag.MaNguoiDung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Hoten");

            if (Session["use"] == null || Session["use"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }

            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["use"];
            List<GioHang> gh = LayGioHang();
            decimal tongtien = 0;

            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }

            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;

            ViewBag.tongtien = tongtien;

            Session["GioHang"] = null;

            return View(ddh);
        }
    }
}
