using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class ChuyenHangController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/ChuyenHang
        // Danh sách toàn bộ phiếu giao hàng
        public ActionResult Index()
        {
            var dsPhieu = db.chuyenhangs
                .OrderByDescending(ch => ch.machuyenhang)
                .ToList();
            return View(dsPhieu);
        }

        // GET: Admin/ChuyenHang/ChoLapPhieu
        // Danh sách đơn đã thanh toán + giao tận nơi + chưa có phiếu giao
        public ActionResult ChoLapPhieu()
        {
            var dsCanLapPhieu = db.hoadons
                .Where(h => h.trangthaidon == 1
                         && h.dathanhtoan == true
                         && h.giaotannoi == true
                         && !h.chuyenhangs.Any())
                .OrderByDescending(h => h.ngaydathang)
                .ToList();
            return View(dsCanLapPhieu);
        }

        // GET: Admin/ChuyenHang/LapPhieu/5
        // Tạo phiếu giao cho một hóa đơn
        public ActionResult LapPhieu(int id)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("ChoLapPhieu");
            }

            // Kiểm tra điều kiện nghiệp vụ trước khi tạo phiếu
            if (hd.trangthaidon != 1)
            {
                TempData["ThongBao"] = "Chỉ lập phiếu giao cho đơn hàng đã duyệt!";
                return RedirectToAction("ChoLapPhieu");
            }
            if (!hd.dathanhtoan)
            {
                TempData["ThongBao"] = "Đơn hàng " + id + " chưa thanh toán, không thể lập phiếu giao!";
                return RedirectToAction("ChoLapPhieu");
            }
            if (!hd.giaotannoi)
            {
                TempData["ThongBao"] = "Đơn hàng " + id + " nhận tại cửa hàng, không cần phiếu giao!";
                return RedirectToAction("ChoLapPhieu");
            }
            if (db.chuyenhangs.Any(chhang => chhang.mahoadon == id))
            {
                TempData["ThongBao"] = "Đơn hàng " + id + " đã có phiếu giao hàng!";
                return RedirectToAction("ChoLapPhieu");
            }

            // Tạo phiếu giao mới: chưa gán nhân viên, chưa giao
            chuyenhang ch = new chuyenhang();
            ch.mahoadon = id;
            ch.manv = null;
            ch.ngaygiao = null;
            ch.daxuli = false;

            db.chuyenhangs.Add(ch);
            db.SaveChanges();

            TempData["ThongBao"] = "Đã tạo phiếu giao hàng cho hóa đơn " + id + "!";
            return RedirectToAction("Index");
        }

        // GET: Admin/ChuyenHang/GanNhanVien/5
        [HttpGet]
        public ActionResult GanNhanVien(int id)
        {
            var ch = db.chuyenhangs.Find(id);
            if (ch == null)
            {
                return HttpNotFound();
            }

            // Không cho gán lại khi phiếu đã giao xong
            if (ch.daxuli)
            {
                TempData["ThongBao"] = "Phiếu giao " + id + " đã hoàn thành, không thể thay đổi nhân viên!";
                return RedirectToAction("Index");
            }

            NapDanhSachNhanVien(ch.manv);
            return View(ch);
        }

        // POST: Admin/ChuyenHang/GanNhanVien
        [HttpPost]
        public ActionResult GanNhanVien(FormCollection collection)
        {
            int id;
            if (!int.TryParse(collection["txt_machuyenhang"], out id))
            {
                return RedirectToAction("Index");
            }

            var ch = db.chuyenhangs.Find(id);
            if (ch == null)
            {
                return HttpNotFound();
            }
            if (ch.daxuli)
            {
                TempData["ThongBao"] = "Phiếu giao " + id + " đã hoàn thành, không thể thay đổi nhân viên!";
                return RedirectToAction("Index");
            }

            int manv;
            if (!int.TryParse(collection["txt_manv"], out manv))
            {
                ViewBag.ThongBao = "Vui lòng chọn nhân viên giao hàng!";
                NapDanhSachNhanVien(ch.manv);
                return View(ch);
            }

            var nv = db.nhanviens.Find(manv);
            if (nv == null)
            {
                ViewBag.ThongBao = "Nhân viên không tồn tại!";
                NapDanhSachNhanVien(ch.manv);
                return View(ch);
            }

            ch.manv = manv;
            db.SaveChanges();

            TempData["ThongBao"] = "Đã gán nhân viên \"" + nv.hoten + "\" cho phiếu giao " + id + "!";
            return RedirectToAction("Index");
        }

        // GET: Admin/ChuyenHang/XacNhanDaGiao/5
        public ActionResult XacNhanDaGiao(int id)
        {
            var ch = db.chuyenhangs.Find(id);
            if (ch == null)
            {
                TempData["ThongBao"] = "Không tìm thấy phiếu giao hàng!";
                return RedirectToAction("Index");
            }

            if (ch.daxuli)
            {
                TempData["ThongBao"] = "Phiếu giao " + id + " đã được xác nhận giao xong trước đó!";
                return RedirectToAction("Index");
            }

            // Bắt buộc phải có nhân viên giao trước khi xác nhận hoàn thành
            if (ch.manv == null)
            {
                TempData["ThongBao"] = "Vui lòng gán nhân viên giao hàng trước khi xác nhận đã giao!";
                return RedirectToAction("Index");
            }

            ch.daxuli = true;
            ch.ngaygiao = DateTime.Today;
            db.SaveChanges();

            TempData["ThongBao"] = "Đã xác nhận giao hàng thành công cho phiếu giao " + id + "!";
            return RedirectToAction("Index");
        }

        // Nạp dropdown nhân viên giao hàng
        private void NapDanhSachNhanVien(int? maNvDaChon = null)
        {
            var dsnv = db.nhanviens
                .OrderBy(n => n.hoten)
                .ToList();
            ViewBag.manv = new SelectList(dsnv, "manv", "hoten", maNvDaChon);
        }
    }
}