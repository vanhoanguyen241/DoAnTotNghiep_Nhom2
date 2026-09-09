using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class ChiTietHoaDonController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/ChiTietHoaDon/Index/5
        public ActionResult Index(int id)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                return HttpNotFound();
            }

            ViewBag.HoaDon = hd;

            var dsct = db.chitiethoadons
                .Where(x => x.mahoadon == id)
                .ToList();

            return View(dsct);
        }

        // GET: Admin/ChiTietHoaDon/TaoMoi/5
        [HttpGet]
        public ActionResult TaoMoi(int id)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                return HttpNotFound();
            }

            ViewBag.Mahoadon = id;
            NapDanhSachSanPham();

            return View();
        }

        // POST: Admin/ChiTietHoaDon/TaoMoi
        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            int mahoadon;
            if (!int.TryParse(collection["txt_mahoadon"], out mahoadon))
            {
                return RedirectToAction("Index", "HoaDon");
            }

            var hd = db.hoadons.Find(mahoadon);
            if (hd == null)
            {
                return HttpNotFound();
            }

            string masanpham = collection["txt_masp"];
            if (string.IsNullOrEmpty(masanpham))
            {
                ViewBag.ThongBao = "Vui lòng chọn sản phẩm!";
                ViewBag.Mahoadon = mahoadon;
                NapDanhSachSanPham();
                return View();
            }

            int soluong;
            if (!int.TryParse(collection["txt_soluong"], out soluong) || soluong <= 0)
            {
                ViewBag.ThongBao = "Số lượng phải lớn hơn 0!";
                ViewBag.Mahoadon = mahoadon;
                NapDanhSachSanPham(masanpham);
                return View();
            }

            var sp = db.sanphams.Find(masanpham);
            if (sp == null)
            {
                ViewBag.ThongBao = "Sản phẩm không tồn tại!";
                ViewBag.Mahoadon = mahoadon;
                NapDanhSachSanPham();
                return View();
            }

            // Kiểm tra trùng chi tiết hóa đơn
            bool trung = db.chitiethoadons
                .Any(x => x.mahoadon == mahoadon && x.masanpham == masanpham);

            if (trung)
            {
                ViewBag.ThongBao = "Sản phẩm này đã có trong hóa đơn!";
                ViewBag.Mahoadon = mahoadon;
                NapDanhSachSanPham(masanpham);
                return View();
            }

            chitiethoadon ct = new chitiethoadon();
            ct.mahoadon = mahoadon;
            ct.masanpham = masanpham;
            ct.soluong = soluong;
            ct.dongia = sp.dongia;
            ct.thanhtien = ct.dongia * soluong;

            db.chitiethoadons.Add(ct);

            // Cập nhật tổng tiền hóa đơn
            hd.tongtien += ct.thanhtien;

            db.SaveChanges();

            TempData["ThongBao"] = "Thêm chi tiết hóa đơn thành công!";
            return RedirectToAction("Index", new { id = mahoadon });
        }

        // GET: Admin/ChiTietHoaDon/Sua?mahoadon=5&masanpham=SP01
        [HttpGet]
        public ActionResult Sua(int mahoadon, string masanpham)
        {
            var ct = db.chitiethoadons
                .FirstOrDefault(x => x.mahoadon == mahoadon && x.masanpham == masanpham);

            if (ct == null)
            {
                return HttpNotFound();
            }

            return View(ct);
        }

        // POST: Admin/ChiTietHoaDon/Sua
        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            int mahoadon;
            if (!int.TryParse(collection["txt_mahoadon"], out mahoadon))
            {
                return RedirectToAction("Index", "HoaDon");
            }

            string masanpham = collection["txt_masp"];

            var ct = db.chitiethoadons
                .FirstOrDefault(x => x.mahoadon == mahoadon && x.masanpham == masanpham);

            if (ct == null)
            {
                return HttpNotFound();
            }

            int soluong;
            if (!int.TryParse(collection["txt_soluong"], out soluong) || soluong <= 0)
            {
                ViewBag.ThongBao = "Số lượng phải lớn hơn 0!";
                return View(ct);
            }

            decimal dongia;
            if (!decimal.TryParse(collection["txt_dongia"], out dongia) || dongia <= 0)
            {
                ViewBag.ThongBao = "Đơn giá không hợp lệ!";
                return View(ct);
            }

            decimal thanhtiencu = ct.thanhtien;

            ct.soluong = soluong;
            ct.dongia = dongia;
            ct.thanhtien = dongia * soluong;

            // Cập nhật lại tổng tiền hóa đơn
            var hd = db.hoadons.Find(mahoadon);
            if (hd != null)
            {
                hd.tongtien = hd.tongtien - thanhtiencu + ct.thanhtien;
            }

            db.SaveChanges();

            TempData["ThongBao"] = "Cập nhật chi tiết hóa đơn thành công!";
            return RedirectToAction("Index", new { id = mahoadon });
        }

        // GET: Admin/ChiTietHoaDon/Xoa?mahoadon=5&masanpham=SP01
        public ActionResult Xoa(int mahoadon, string masanpham)
        {
            var ct = db.chitiethoadons
                .FirstOrDefault(x => x.mahoadon == mahoadon && x.masanpham == masanpham);

            if (ct != null)
            {
                var hd = db.hoadons.Find(mahoadon);

                if (hd != null)
                {
                    hd.tongtien -= ct.thanhtien;
                }

                db.chitiethoadons.Remove(ct);
                db.SaveChanges();

                TempData["ThongBao"] = "Xóa chi tiết hóa đơn thành công!";
            }

            return RedirectToAction("Index", new { id = mahoadon });
        }

        // Nạp dropdown sản phẩm
        private void NapDanhSachSanPham(string maSpDaChon = null)
        {
            var dssp = db.sanphams
                .OrderBy(x => x.tensanpham)
                .ToList();

            ViewBag.masanpham = new SelectList(dssp, "masanpham", "tensanpham", maSpDaChon);
        }
    }
}