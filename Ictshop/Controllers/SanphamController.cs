using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Controllers
{
    public class SanphamController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        // GET: Sanpham
        public ActionResult Sanphammoi()
        {
            var Sanphammoi = db.Sanphams.OrderByDescending(sp => sp.Masp).Take(4).ToList();
            return PartialView(Sanphammoi);
        }
        public ActionResult dtiphonepartial()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 2).Take(4).ToList();
            return PartialView(ip);
        }
        public ActionResult dtsamsungpartial()
        {
            var ss = db.Sanphams.Where(n => n.Mahang == 1).Take(4).ToList();
            return PartialView(ss);
        }
        public ActionResult dtxiaomipartial()
        {
            var mi = db.Sanphams.Where(n => n.Mahang == 3).Take(4).ToList();
            return PartialView(mi);
        }



        public ActionResult Search(string name)
        {
            var results = db.Sanphams.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                results = results.Where(s => s.Tensp.Contains(name));
            }

            // chỉ định tên View khi trả về kết quả
            return View("TatCaSanPham", results.ToList());
        }
        // GET: Sanpham
        public ActionResult TatCaSanPham()
        {
            var tatCaSanPham = db.Sanphams.ToList(); // Lấy ra danh sách tất cả sản phẩm từ cơ sở dữ liệu
            return View(tatCaSanPham); // Trả về danh sách sản phẩm cho view
        }



        //public ActionResult dttheohang()
        //{
        //    var mi = db.Sanphams.Where(n => n.Mahang == 5).Take(4).ToList();
        //    return PartialView(mi);
        //}
        public ActionResult xemchitiet(int Masp = 0)
        {
            var chitiet = db.Sanphams.SingleOrDefault(n => n.Masp == Masp);
            if (chitiet == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitiet);
        }

    }

}

