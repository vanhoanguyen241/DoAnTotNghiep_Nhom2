using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class DuyetDonController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/DuyetDon
        public ActionResult Index()
        {
            var dsChoDuyet = db.hoadons
                .Where(h => h.trangthaidon == 0)
                .OrderByDescending(h => h.ngaydathang)
                .ToList();
            return View(dsChoDuyet);
        }

        // GET: Admin/DuyetDon/ChiTiet/5
        public ActionResult ChiTiet(int id)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("Index");
            }
            if (hd.trangthaidon != 0)
            {
                TempData["ThongBao"] = "Đơn hàng này đã được xử lý!";
                return RedirectToAction("Index");
            }

            ViewBag.HoaDon = hd;
            ViewBag.KhachHang = hd.khachhang;

            var dsChiTiet = db.chitiethoadons
                .Where(ct => ct.mahoadon == id)
                .ToList();

            // Nạp tồn kho hiện tại cho từng sản phẩm (để view hiển thị)
            var tonKho = new Dictionary<string, int>();
            foreach (var ct in dsChiTiet)
            {
                var sp = db.sanphams.Find(ct.masanpham);
                tonKho[ct.masanpham] = sp != null ? sp.soluonghienco : 0;
            }
            ViewBag.TonKho = tonKho;

            return View(dsChiTiet);
        }

        // POST: Admin/DuyetDon/XuLyDuyet
        [HttpPost]
        public ActionResult XuLyDuyet(FormCollection collection)
        {
            int id;
            if (!int.TryParse(collection["mahoadon"], out id))
                return RedirectToAction("Index");

            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("Index");
            }
            if (hd.trangthaidon != 0)
            {
                TempData["ThongBao"] = "Đơn hàng này đã được xử lý!";
                return RedirectToAction("Index");
            }

            var dsChiTiet = db.chitiethoadons
                .Where(ct => ct.mahoadon == id)
                .ToList();

            // Lưu số lượng gốc (trước khi duyệt) để so sánh
            var soLuongGoc = new Dictionary<string, int>();
            foreach (var ct in dsChiTiet)
                soLuongGoc[ct.masanpham] = ct.soluong;

            // Đọc số lượng duyệt từ form
            var soLuongDuyet = new Dictionary<string, int>();
            bool allZero = true;
            bool coThayDoi = false;
            var ghiChuThayDoi = new List<string>();

            foreach (var ct in dsChiTiet)
            {
                string key = "sl_" + ct.masanpham;
                int slDuyet;
                if (!int.TryParse(collection[key], out slDuyet))
                    slDuyet = 0;

                if (slDuyet < 0)
                {
                    TempData["ThongBao"] = "Số lượng duyệt không được âm!";
                    return RedirectToAction("ChiTiet", new { id = id });
                }

                var sp = db.sanphams.Find(ct.masanpham);
                if (sp == null)
                {
                    TempData["ThongBao"] = "Sản phẩm \"" + ct.masanpham + "\" không tồn tại!";
                    return RedirectToAction("ChiTiet", new { id = id });
                }
                if (slDuyet > sp.soluonghienco)
                {
                    TempData["ThongBao"] = "Sản phẩm \"" + sp.tensanpham
                        + "\" chỉ còn " + sp.soluonghienco + " trong kho!";
                    return RedirectToAction("ChiTiet", new { id = id });
                }

                soLuongDuyet[ct.masanpham] = slDuyet;
                if (slDuyet > 0) allZero = false;

                if (slDuyet != soLuongGoc[ct.masanpham])
                {
                    coThayDoi = true;
                    if (slDuyet == 0)
                        ghiChuThayDoi.Add("SP \"" + sp.tensanpham + "\" bị loại");
                    else
                        ghiChuThayDoi.Add("SP \"" + sp.tensanpham + "\": "
                            + soLuongGoc[ct.masanpham] + " → " + slDuyet);
                }
            }

            // Nếu TẤT CẢ dòng = 0 → tự động từ chối
            if (allZero)
            {
                hd.trangthaidon = 2;
                hd.nguoilap = Session["MaNV"] as int?;
                string lyDo = " | Tự động từ chối: Không có sản phẩm nào được duyệt.";
                hd.ghichu = (hd.ghichu ?? "") + lyDo;
                if (hd.ghichu.Length > 1000) hd.ghichu = hd.ghichu.Substring(0, 1000);
                db.SaveChanges();
                TempData["ThongBao"] = "Đơn hàng đã được tự động từ chối (không có sản phẩm nào được duyệt)!";
                return RedirectToAction("Index");
            }

            // Xử lý từng dòng: xóa dòng = 0, cập nhật dòng > 0, trừ tồn kho
            decimal tongTienMoi = 0;
            foreach (var ct in dsChiTiet)
            {
                int slDuyet = soLuongDuyet[ct.masanpham];
                if (slDuyet == 0)
                {
                    db.chitiethoadons.Remove(ct);
                }
                else
                {
                    ct.soluong = slDuyet;                 // ghi đè trực tiếp
                    ct.thanhtien = ct.dongia * slDuyet;
                    tongTienMoi += ct.thanhtien;

                    var sp = db.sanphams.Find(ct.masanpham);
                    sp.soluonghienco -= slDuyet;         // Trừ tồn kho khi duyệt
                }
            }

            // Cập nhật hóa đơn
            hd.trangthaidon = 1;
            hd.nguoilap = Session["MaNV"] as int?;
            hd.tongtien = tongTienMoi;

            if (coThayDoi)
            {
                string ghiChu = " | Duyệt một phần: " + string.Join("; ", ghiChuThayDoi);
                hd.ghichu = (hd.ghichu ?? "") + ghiChu;
                if (hd.ghichu.Length > 1000) hd.ghichu = hd.ghichu.Substring(0, 1000);
            }

            db.SaveChanges();
            TempData["ThongBao"] = "Duyệt đơn hàng thành công!";
            return RedirectToAction("Index");
        }

        // POST: Admin/DuyetDon/TuChoi
        [HttpPost]
        public ActionResult TuChoi(int id, string lydo)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("Index");
            }
            if (hd.trangthaidon != 0)
            {
                TempData["ThongBao"] = "Đơn hàng này đã được xử lý!";
                return RedirectToAction("Index");
            }

            hd.trangthaidon = 2;
            hd.nguoilap = Session["MaNV"] as int?;
            string lyDoFmt = " | Từ chối: "
                + (string.IsNullOrWhiteSpace(lydo) ? "Không nêu lý do" : lydo.Trim());
            hd.ghichu = (hd.ghichu ?? "") + lyDoFmt;
            if (hd.ghichu.Length > 1000) hd.ghichu = hd.ghichu.Substring(0, 1000);

            db.SaveChanges();
            TempData["ThongBao"] = "Đã từ chối đơn hàng!";
            return RedirectToAction("Index");
        }

        // GET: Admin/DuyetDon/DanhSachTuChoi
        public ActionResult DanhSachTuChoi()
        {
            var dsTuChoi = db.hoadons
                .Where(h => h.trangthaidon == 2)
                .OrderByDescending(h => h.ngaydathang)
                .ToList();
            return View(dsTuChoi);
        }

        // GET: Admin/DuyetDon/XoaDonTuChoi/5
        public ActionResult XoaDonTuChoi(int id)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("DanhSachTuChoi");
            }
            if (hd.trangthaidon != 2)
            {
                TempData["ThongBao"] = "Chỉ xóa được đơn đã bị từ chối!";
                return RedirectToAction("DanhSachTuChoi");
            }
            if (db.chuyenhangs.Any(ch => ch.mahoadon == id))
            {
                TempData["ThongBao"] = "Không thể xóa đơn đã có phiếu chuyển hàng!";
                return RedirectToAction("DanhSachTuChoi");
            }

            var chiTiet = db.chitiethoadons.Where(ct => ct.mahoadon == id).ToList();
            if (chiTiet.Any()) db.chitiethoadons.RemoveRange(chiTiet);
            db.hoadons.Remove(hd);
            db.SaveChanges();

            TempData["ThongBao"] = "Đã xóa đơn hàng bị từ chối!";
            return RedirectToAction("DanhSachTuChoi");
        }
    }
}