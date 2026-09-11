using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class ChuyenHangController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/ChuyenHang
        // Admin xem toàn bộ phiếu giao hàng
        // Nhân viên chỉ xem các phiếu được gán cho mình
        public ActionResult Index()
        {
            string vaiTro = Session["VaiTro"]?.ToString();
            bool isAdmin = string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase);

            List<chuyenhang> dsPhieu;

            if (isAdmin)
            {
                dsPhieu = db.chuyenhangs
                    .OrderByDescending(ch => ch.machuyenhang)
                    .ToList();
            }
            else
            {
                int? maNV = Session["MaNV"] as int?;

                dsPhieu = maNV.HasValue
                    ? db.chuyenhangs
                        .Where(ch => ch.manv == maNV.Value)
                        .OrderByDescending(ch => ch.machuyenhang)
                        .ToList()
                    : new List<chuyenhang>();
            }

            return View(dsPhieu);
        }

        // GET: Admin/ChuyenHang/ChoLapPhieu
        // Danh sách đơn đã thanh toán + giao tận nơi + chưa có phiếu giao
        public ActionResult ChoLapPhieu()
        {
            // DB mới: hoadon không còn trangthaidon.
            // Hóa đơn chỉ tồn tại sau khi đã duyệt (DuyetDon tạo) hoặc Admin tạo trực tiếp,
            // nên chỉ cần lọc: đã thanh toán + giao tận nơi + chưa có phiếu giao.
            var dsCanLapPhieu = db.hoadons
                .Where(h => h.dathanhtoan == true
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

        // POST: Admin/ChuyenHang/XacNhanGiaoHang
        [HttpPost]
        public ActionResult XacNhanGiaoHang(int id)
        {
            int? maNV = Session["MaNV"] as int?;
            if (!maNV.HasValue)
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
            }

            // Tìm phiếu giao hàng: phải đúng mã phiếu VÀ đúng nhân viên đang đăng nhập
            var ch = db.chuyenhangs.FirstOrDefault(x => x.machuyenhang == id && x.manv == maNV.Value);

            if (ch == null)
            {
                TempData["ThongBao"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền giao đơn này!";
                return RedirectToAction("CuaToi");
            }

            if (ch.daxuli)
            {
                TempData["ThongBao"] = "Đơn hàng này đã được xác nhận giao trước đó!";
                return RedirectToAction("CuaToi");
            }

            // Cập nhật trạng thái
            ch.daxuli = true;
            ch.ngaygiao = DateTime.Today;
            db.SaveChanges();

            TempData["ThongBao"] = "Xác nhận giao hàng thành công!";
            return RedirectToAction("CuaToi");
        }
    }
}