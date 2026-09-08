using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Filters;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    // Module 2 — Quản lý khách hàng. Chỉ Nhân viên mới được truy cập toàn bộ Controller này.
    [NhanVienOnly]
    public class KhachHangController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: KhachHang
        public ActionResult Index(string tukhoa, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var khachhangs = db.khachhangs.AsQueryable();

            if (!string.IsNullOrEmpty(tukhoa))
            {
                khachhangs = khachhangs.Where(kh =>
                    kh.hoten.Contains(tukhoa) ||
                    (kh.SDT != null && kh.SDT.Contains(tukhoa)));
                ViewBag.TuKhoa = tukhoa;
            }

            khachhangs = khachhangs.OrderByDescending(kh => kh.makh);

            int tongSo = khachhangs.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSo / pageSize);

            ViewBag.TongSo = tongSo;
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.TrangHienTai = pageNumber;

            var danhSach = khachhangs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(danhSach);
        }

        // GET: KhachHang/TaoMoi
        public ActionResult TaoMoi()
        {
            return View();
        }

        // POST: KhachHang/TaoMoi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoMoi(khachhang model)
        {
            ValidateSDT(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            db.khachhangs.Add(model);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm khách hàng thành công.";
            return RedirectToAction("Index");
        }

        // GET: KhachHang/Sua/5
        public ActionResult Sua(int id)
        {
            var khachhang = db.khachhangs.Find(id);
            if (khachhang == null)
            {
                return HttpNotFound();
            }

            return View(khachhang);
        }

        // POST: KhachHang/Sua/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(int id, khachhang model)
        {
            if (id != model.makh)
            {
                return HttpNotFound();
            }

            ValidateSDT(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var khachhang = db.khachhangs.Find(id);
            if (khachhang == null)
            {
                return HttpNotFound();
            }

            khachhang.hoten = model.hoten;
            khachhang.diachi = model.diachi;
            khachhang.SDT = model.SDT;

            db.SaveChanges();

            TempData["ThongBao"] = "Cập nhật khách hàng thành công.";
            return RedirectToAction("Index");
        }

        // POST: KhachHang/Xoa/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Xoa(int id)
        {
            var khachhang = db.khachhangs.Find(id);
            if (khachhang == null)
            {
                return HttpNotFound();
            }

            // Quy tắc: không xóa khách đã có hóa đơn.
            bool coHoaDon = db.hoadons.Any(hd => hd.makh == id);
            if (coHoaDon)
            {
                TempData["Loi"] = "Không thể xóa khách hàng \"" + khachhang.hoten + "\" vì đã có hóa đơn trong hệ thống.";
                return RedirectToAction("Index");
            }

            try
            {
                // Khách có tài khoản thì xóa tài khoản liên quan trước, sau đó mới xóa khách.
                var taikhoansLienQuan = db.taikhoans.Where(tk => tk.makh == id).ToList();
                foreach (var tk in taikhoansLienQuan)
                {
                    db.taikhoans.Remove(tk);
                }

                db.khachhangs.Remove(khachhang);
                db.SaveChanges();

                TempData["ThongBao"] = "Xóa khách hàng thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["Loi"] = "Không thể xóa khách hàng này vì dữ liệu đang được tham chiếu ở nơi khác.";
            }

            return RedirectToAction("Index");
        }

        // Validate số điện thoại: nếu có nhập thì bắt buộc đúng 10-11 chữ số.
        // hoten và diachi đã được validate tự động qua [Required] trên model khachhang.
        private void ValidateSDT(khachhang model)
        {
            if (!string.IsNullOrWhiteSpace(model.SDT) && !Regex.IsMatch(model.SDT, @"^[0-9]{10,11}$"))
            {
                ModelState.AddModelError("SDT", "Số điện thoại phải gồm 10-11 chữ số.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}