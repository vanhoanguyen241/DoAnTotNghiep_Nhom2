using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class HoaDonController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/HoaDon
        public ActionResult Index()
        {
            var dshd = db.hoadons
                .OrderByDescending(h => h.ngaydathang)
                .ToList();

            return View(dshd);
        }

        // GET: Admin/HoaDon/TaoMoi
        [HttpGet]
        public ActionResult TaoMoi()
        {
            NapDanhSachKhachHang();
            return View();
        }

        // POST: Admin/HoaDon/TaoMoi
        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            int makh;
            if (!int.TryParse(collection["txt_makh"], out makh))
            {
                ViewBag.ThongBao = "Vui lòng chọn khách hàng!";
                NapDanhSachKhachHang();
                return View();
            }

            DateTime ngaydathang = DateTime.Today;
            DateTime tempNgayDat;
            if (DateTime.TryParse(collection["txt_ngaydathang"], out tempNgayDat))
            {
                ngaydathang = tempNgayDat;
            }

            DateTime ngaygiaohang = ngaydathang;
            DateTime tempNgayGiao;
            if (DateTime.TryParse(collection["txt_ngaygiaohang"], out tempNgayGiao))
            {
                ngaygiaohang = tempNgayGiao;
            }

            if (ngaygiaohang < ngaydathang)
            {
                ViewBag.ThongBao = "Ngày giao hàng không được nhỏ hơn ngày đặt hàng!";
                NapDanhSachKhachHang(makh);
                return View();
            }

            // Lấy mã hóa đơn lớn nhất hiện có
            int maHDLonNhat = db.hoadons
                .Select(h => h.mahoadon)
                .DefaultIfEmpty(0)
                .Max();

            hoadon hd = new hoadon();
            hd.mahoadon = maHDLonNhat + 1;
            hd.makh = makh;
            hd.nguoilap = Session["MaNV"] as int?;
            hd.ngaydathang = ngaydathang;
            hd.ngaygiaohang = ngaygiaohang;
            hd.tongtien = 0;
            hd.dathanhtoan = collection["chk_dathanhtoan"] != null;
            hd.giaotannoi = collection["chk_giaotannoi"] != null;

            // Admin tạo hóa đơn trực tiếp thì có thể xem như đã duyệt
            hd.trangthaidon = 1;

            hd.ghichu = collection["txt_ghichu"];

            db.hoadons.Add(hd);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm hóa đơn mới thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/HoaDon/Sua/5
        [HttpGet]
        public ActionResult Sua(int id)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                return HttpNotFound();
            }

            NapDanhSachKhachHang(hd.makh);
            return View(hd);
        }

        // POST: Admin/HoaDon/Sua
        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            int id;
            if (!int.TryParse(collection["txt_mahoadon"], out id))
            {
                return RedirectToAction("Index");
            }

            var sua_hd = db.hoadons.Find(id);
            if (sua_hd == null)
            {
                return HttpNotFound();
            }

            int makh;
            if (!int.TryParse(collection["txt_makh"], out makh))
            {
                ViewBag.ThongBao = "Vui lòng chọn khách hàng!";
                NapDanhSachKhachHang(sua_hd.makh);
                return View(sua_hd);
            }

            DateTime ngaydathang = sua_hd.ngaydathang;
            DateTime tempNgayDat;
            if (DateTime.TryParse(collection["txt_ngaydathang"], out tempNgayDat))
            {
                ngaydathang = tempNgayDat;
            }

            DateTime ngaygiaohang = sua_hd.ngaygiaohang;
            DateTime tempNgayGiao;
            if (DateTime.TryParse(collection["txt_ngaygiaohang"], out tempNgayGiao))
            {
                ngaygiaohang = tempNgayGiao;
            }

            if (ngaygiaohang < ngaydathang)
            {
                ViewBag.ThongBao = "Ngày giao hàng không được nhỏ hơn ngày đặt hàng!";
                NapDanhSachKhachHang(makh);
                return View(sua_hd);
            }

            sua_hd.makh = makh;
            sua_hd.ngaydathang = ngaydathang;
            sua_hd.ngaygiaohang = ngaygiaohang;
            sua_hd.dathanhtoan = collection["chk_dathanhtoan"] != null;
            sua_hd.giaotannoi = collection["chk_giaotannoi"] != null;
            sua_hd.ghichu = collection["txt_ghichu"];

            db.SaveChanges();

            TempData["ThongBao"] = "Cập nhật hóa đơn thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/HoaDon/Xoa/5
        public ActionResult Xoa(int id)
        {
            var xoa_hd = db.hoadons.Find(id);
            if (xoa_hd != null)
            {
                // Không cho xóa hóa đơn đã có phiếu chuyển hàng
                bool coPhieuChuyen = db.chuyenhangs.Any(ch => ch.mahoadon == id);

                if (coPhieuChuyen)
                {
                    TempData["ThongBao"] = "Không thể xóa hóa đơn đã có phiếu chuyển hàng!";
                }
                else
                {
                    // Xóa toàn bộ chi tiết hóa đơn trước
                    var chiTiet = db.chitiethoadons
                        .Where(ct => ct.mahoadon == id)
                        .ToList();

                    if (chiTiet.Any())
                    {
                        db.chitiethoadons.RemoveRange(chiTiet);
                    }

                    db.hoadons.Remove(xoa_hd);
                    db.SaveChanges();

                    TempData["ThongBao"] = "Xóa hóa đơn thành công!";
                }
            }

            return RedirectToAction("Index");
        }

        // Nạp dropdown khách hàng
        private void NapDanhSachKhachHang(int? maKhachDaChon = null)
        {
            var dskh = db.khachhangs
                .OrderBy(k => k.hoten)
                .ToList();

            ViewBag.makh = new SelectList(dskh, "makh", "hoten", maKhachDaChon);
        }
    }
}