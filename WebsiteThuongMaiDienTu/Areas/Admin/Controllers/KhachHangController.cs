using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class KhachHangController : BaseAdminController
    {
        QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/KhachHang
        public ActionResult Index()
        {
            var listkh = db.khachhangs.ToList();
            return View(listkh);
        }

        // GET: Admin/KhachHang/TaoMoi
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }

        // POST: Admin/KhachHang/TaoMoi
        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            khachhang kh = new khachhang();
            kh.makh = int.Parse(collection["txt_makh"]);
            kh.hoten = collection["txt_hoten"];
            kh.diachi = collection["txt_diachi"];
            kh.SDT = collection["txt_sdt"];

            db.khachhangs.Add(kh);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin/KhachHang/Sua/5
        [HttpGet]
        public ActionResult Sua(int id)
        {
            var kh = db.khachhangs.Find(id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }

        // POST: Admin/KhachHang/Sua/5
        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            int id = int.Parse(collection["txt_makh"]);
            var sua_kh = db.khachhangs.Find(id);

            if (sua_kh != null)
            {
                sua_kh.hoten = collection["txt_hoten"];
                sua_kh.diachi = collection["txt_diachi"];
                sua_kh.SDT = collection["txt_sdt"];
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/KhachHang/Xoa/5
        public ActionResult Xoa(int id)
        {
            var xoa_kh = db.khachhangs.Find(id);
            if (xoa_kh != null)
            {
                // Không cho xóa nếu đã có hóa đơn
                bool coHoaDon = db.hoadons.Any(hd => hd.makh == id);
                if (coHoaDon)
                {
                    TempData["ThongBao"] = "Không thể xóa khách hàng \"" + xoa_kh.hoten + "\" vì đã có hóa đơn!";
                    return RedirectToAction("Index");
                }

                db.khachhangs.Remove(xoa_kh);
                db.SaveChanges();
                TempData["ThongBao"] = "Xóa khách hàng thành công!";
            }
            return RedirectToAction("Index");
        }
    }
}