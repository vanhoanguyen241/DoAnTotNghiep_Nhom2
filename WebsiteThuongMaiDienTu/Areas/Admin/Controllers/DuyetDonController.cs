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
            var dsChoDuyet = db.dathangs
                .Where(d => d.trangthai == 0)
                .OrderByDescending(d => d.ngaydathang)
                .ToList();
            return View(dsChoDuyet);
        }

        // GET: Admin/DuyetDon/ChiTiet/5
        public ActionResult ChiTiet(int id)
        {
            var dh = db.dathangs.Find(id);
            if (dh == null)
            {
                TempData["ThongBao"] = "Không tìm thấy đơn đặt hàng!";
                return RedirectToAction("Index");
            }
            if (dh.trangthai != 0)
            {
                TempData["ThongBao"] = "Đơn hàng này đã được xử lý!";
                return RedirectToAction("Index");
            }

            ViewBag.DatHang = dh;
            ViewBag.KhachHang = dh.khachhang;

            var dsChiTiet = db.chitietdathangs
                .Where(ct => ct.madathang == id)
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
            if (!int.TryParse(collection["madathang"], out id))
                return RedirectToAction("Index");

            var dh = db.dathangs.Find(id);
            if (dh == null)
            {
                TempData["ThongBao"] = "Không tìm thấy đơn đặt hàng!";
                return RedirectToAction("Index");
            }
            if (dh.trangthai != 0)
            {
                TempData["ThongBao"] = "Đơn hàng này đã được xử lý!";
                return RedirectToAction("Index");
            }

            var dsChiTiet = db.chitietdathangs
                .Where(ct => ct.madathang == id)
                .ToList();

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

                if (slDuyet != ct.soluongdat)
                {
                    coThayDoi = true;
                    if (slDuyet == 0)
                        ghiChuThayDoi.Add("SP \"" + sp.tensanpham + "\" bị loại");
                    else
                        ghiChuThayDoi.Add("SP \"" + sp.tensanpham + "\": "
                            + ct.soluongdat + " → " + slDuyet);
                }
            }

            // Nếu TẤT CẢ dòng = 0 → tự động từ chối
            if (allZero)
            {
                dh.trangthai = 2;
                string lyDo = " | Tự động từ chối: Không có sản phẩm nào được duyệt.";
                dh.ghichu = (dh.ghichu ?? "") + lyDo;
                if (dh.ghichu.Length > 1000) dh.ghichu = dh.ghichu.Substring(0, 1000);
                db.SaveChanges();

                TempData["ThongBao"] = "Đơn hàng đã được tự động từ chối (không có sản phẩm nào được duyệt)!";
                return RedirectToAction("Index");
            }

            // Tạo hóa đơn mới từ các dòng được duyệt
            // TĂNG THỦ CÔNG mahoadon
            int maHoaDonMoi;
            if (db.hoadons.Any())
                maHoaDonMoi = db.hoadons.Max(h => h.mahoadon) + 1;
            else
                maHoaDonMoi = 1;

            hoadon hd = new hoadon();
            hd.mahoadon = maHoaDonMoi;
            hd.makh = dh.makh;
            hd.nguoilap = Session["MaNV"] as int?;
            hd.ngaydathang = dh.ngaydathang;
            hd.ngaygiaohang = dh.ngaygiaohang;
            hd.dathanhtoan = false;
            hd.giaotannoi = dh.giaotannoi;
            hd.ghichu = dh.ghichu;

            decimal tongTienMoi = 0;

            foreach (var ct in dsChiTiet)
            {
                int slDuyet = soLuongDuyet[ct.masanpham];
                ct.soluongduyet = slDuyet;

                if (slDuyet > 0)
                {
                    chitiethoadon cthd = new chitiethoadon();
                    cthd.mahoadon = maHoaDonMoi;
                    cthd.masanpham = ct.masanpham;
                    cthd.soluong = slDuyet;
                    cthd.dongia = ct.dongia;
                    cthd.thanhtien = ct.dongia * slDuyet;
                    hd.chitiethoadons.Add(cthd);
                    tongTienMoi += cthd.thanhtien;

                    var sp = db.sanphams.Find(ct.masanpham);
                    sp.soluonghienco -= slDuyet;   // Trừ tồn kho khi duyệt
                }
            }

            hd.tongtien = tongTienMoi;

            if (coThayDoi)
            {
                string ghiChu = " | Duyệt một phần: " + string.Join("; ", ghiChuThayDoi);
                hd.ghichu = (hd.ghichu ?? "") + ghiChu;
                if (hd.ghichu.Length > 1000) hd.ghichu = hd.ghichu.Substring(0, 1000);
            }

            db.hoadons.Add(hd);
            db.SaveChanges();

            // Liên kết phiếu đặt hàng với hóa đơn vừa tạo
            dh.trangthai = 1;
            dh.mahoadon = hd.mahoadon;
            db.SaveChanges();

            TempData["ThongBao"] = "Duyệt đơn hàng thành công! Đã tạo hóa đơn #" + hd.mahoadon;
            return RedirectToAction("Index");
        }

        // POST: Admin/DuyetDon/TuChoi
        [HttpPost]
        public ActionResult TuChoi(int id, string lydo)
        {
            var dh = db.dathangs.Find(id);
            if (dh == null)
            {
                TempData["ThongBao"] = "Không tìm thấy đơn đặt hàng!";
                return RedirectToAction("Index");
            }
            if (dh.trangthai != 0)
            {
                TempData["ThongBao"] = "Đơn hàng này đã được xử lý!";
                return RedirectToAction("Index");
            }

            dh.trangthai = 2;
            string lyDoFmt = " | Từ chối: "
                + (string.IsNullOrWhiteSpace(lydo) ? "Không nêu lý do" : lydo.Trim());
            dh.ghichu = (dh.ghichu ?? "") + lyDoFmt;
            if (dh.ghichu.Length > 1000) dh.ghichu = dh.ghichu.Substring(0, 1000);
            db.SaveChanges();

            TempData["ThongBao"] = "Đã từ chối đơn hàng!";
            return RedirectToAction("Index");
        }

        // GET: Admin/DuyetDon/DanhSachTuChoi
        public ActionResult DanhSachTuChoi()
        {
            var dsTuChoi = db.dathangs
                .Where(d => d.trangthai == 2)
                .OrderByDescending(d => d.ngaydathang)
                .ToList();
            return View(dsTuChoi);
        }

        // GET: Admin/DuyetDon/XoaDonTuChoi/5
        public ActionResult XoaDonTuChoi(int id)
        {
            var dh = db.dathangs.Find(id);
            if (dh == null)
            {
                TempData["ThongBao"] = "Không tìm thấy đơn đặt hàng!";
                return RedirectToAction("DanhSachTuChoi");
            }
            if (dh.trangthai != 2)
            {
                TempData["ThongBao"] = "Chỉ xóa được đơn đã bị từ chối!";
                return RedirectToAction("DanhSachTuChoi");
            }

            // Vì đơn từ chối chưa bao giờ tạo hóa đơn nên không cần check chuyenhang
            var chiTiet = db.chitietdathangs.Where(ct => ct.madathang == id).ToList();
            if (chiTiet.Any()) db.chitietdathangs.RemoveRange(chiTiet);
            db.dathangs.Remove(dh);
            db.SaveChanges();

            TempData["ThongBao"] = "Đã xóa đơn hàng bị từ chối!";
            return RedirectToAction("DanhSachTuChoi");
        }
    }
}