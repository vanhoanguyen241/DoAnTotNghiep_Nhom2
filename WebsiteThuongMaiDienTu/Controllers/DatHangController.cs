using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class DatHangController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: /DatHang
        public ActionResult Index()
        {
            var gioHang = Session["GioHang"] as Dictionary<string, int>;

            if (gioHang == null || !gioHang.Any())
            {
                TempData["LoiGioHang"] = "Giỏ hàng trống. Vui lòng chọn sản phẩm trước khi đặt hàng.";
                return RedirectToAction("Index", "GioHang");
            }

            var sanPhams = LaySanPhamTrongGio(gioHang);

            if (!sanPhams.Any())
            {
                Session.Remove("GioHang");
                TempData["LoiGioHang"] = "Giỏ hàng không hợp lệ. Vui lòng chọn lại sản phẩm.";
                return RedirectToAction("Index", "GioHang");
            }

            // Kiểm tra nhanh tồn kho trước khi vào trang đặt hàng
            foreach (var sp in sanPhams)
            {
                int soLuong = gioHang.ContainsKey(sp.masanpham) ? gioHang[sp.masanpham] : 0;

                if (sp.soluonghienco <= 0)
                {
                    TempData["LoiGioHang"] = "Sản phẩm \"" + sp.tensanpham + "\" hiện đã hết hàng.";
                    return RedirectToAction("Index", "GioHang");
                }

                if (soLuong > sp.soluonghienco)
                {
                    TempData["LoiGioHang"] = "Sản phẩm \"" + sp.tensanpham + "\" chỉ còn " + sp.soluonghienco + " sản phẩm.";
                    return RedirectToAction("Index", "GioHang");
                }
            }

            ChuanBiViewDatHang(gioHang, sanPhams);

            return View(sanPhams);
        }

        // POST: /DatHang/XacNhanDatHang
        [HttpPost]
        public ActionResult XacNhanDatHang(FormCollection collection)
        {
            var gioHang = Session["GioHang"] as Dictionary<string, int>;

            if (gioHang == null || !gioHang.Any())
            {
                TempData["LoiGioHang"] = "Giỏ hàng trống. Vui lòng chọn sản phẩm trước khi đặt hàng.";
                return RedirectToAction("Index", "GioHang");
            }

            var sanPhams = LaySanPhamTrongGio(gioHang);

            if (!sanPhams.Any())
            {
                Session.Remove("GioHang");
                TempData["LoiGioHang"] = "Giỏ hàng không hợp lệ. Vui lòng chọn lại sản phẩm.";
                return RedirectToAction("Index", "GioHang");
            }

            // Kiểm tra tồn kho lần cuối trước khi tạo đơn
            foreach (var sp in sanPhams)
            {
                int soLuong = gioHang.ContainsKey(sp.masanpham) ? gioHang[sp.masanpham] : 0;

                if (sp.soluonghienco <= 0)
                {
                    TempData["LoiGioHang"] = "Sản phẩm \"" + sp.tensanpham + "\" hiện đã hết hàng.";
                    return RedirectToAction("Index", "GioHang");
                }

                if (soLuong > sp.soluonghienco)
                {
                    TempData["LoiGioHang"] = "Sản phẩm \"" + sp.tensanpham + "\" chỉ còn " + sp.soluonghienco + " sản phẩm.";
                    return RedirectToAction("Index", "GioHang");
                }
            }

            ChuanBiViewDatHang(gioHang, sanPhams);

            // Xác định khách hàng
            int makh;
            khachhang khachHienTai = null;

            int? maKHTuSession = Session["MaKH"] as int?;

            if (Session["LoaiTaiKhoan"] != null
                && Session["LoaiTaiKhoan"].ToString() == "KhachHang"
                && maKHTuSession.HasValue)
            {
                khachHienTai = db.khachhangs.Find(maKHTuSession.Value);

                if (khachHienTai != null)
                {
                    makh = khachHienTai.makh;
                }
                else
                {
                    makh = 0;
                }
            }
            else
            {
                string hoten = (collection["txt_hoten"] ?? "").Trim();
                string diachi = (collection["txt_diachi"] ?? "").Trim();
                string sdt = (collection["txt_sdt"] ?? "").Trim();

                if (string.IsNullOrEmpty(hoten)
                    || string.IsNullOrEmpty(diachi)
                    || string.IsNullOrEmpty(sdt))
                {
                    ViewBag.ThongBao = "Vui lòng nhập đầy đủ họ tên, địa chỉ và số điện thoại!";
                    return View("Index", sanPhams);
                }

                if (sdt.Length < 10 || sdt.Length > 11 || sdt.Any(c => !char.IsDigit(c)))
                {
                    ViewBag.ThongBao = "Số điện thoại phải từ 10 đến 11 chữ số!";
                    return View("Index", sanPhams);
                }

                // Nếu SDT đã tồn tại thì dùng lại khách hàng cũ
                var khachCu = db.khachhangs.FirstOrDefault(k => k.SDT == sdt);

                if (khachCu != null)
                {
                    makh = khachCu.makh;
                    khachHienTai = khachCu;
                }
                else
                {
                    int maKhLonNhat = db.khachhangs
                        .Select(k => k.makh)
                        .DefaultIfEmpty(0)
                        .Max();

                    khachhang kh_moi = new khachhang();
                    kh_moi.makh = maKhLonNhat + 1;
                    kh_moi.hoten = hoten;
                    kh_moi.diachi = diachi;
                    kh_moi.SDT = sdt;

                    db.khachhangs.Add(kh_moi);
                    db.SaveChanges();

                    makh = kh_moi.makh;
                    khachHienTai = kh_moi;
                }
            }

            if (makh <= 0)
            {
                ViewBag.ThongBao = "Không xác định được khách hàng. Vui lòng thử lại!";
                return View("Index", sanPhams);
            }

            // Ngày giao mong muốn
            DateTime ngayDatHang = DateTime.Today;
            DateTime ngayGiaoHang = ngayDatHang;
            DateTime tempNgayGiao;

            if (DateTime.TryParse(collection["txt_ngaygiaohang"], out tempNgayGiao))
            {
                ngayGiaoHang = tempNgayGiao.Date;
            }

            if (ngayGiaoHang < ngayDatHang)
            {
                ViewBag.ThongBao = "Ngày giao hàng không được nhỏ hơn ngày đặt hàng!";
                return View("Index", sanPhams);
            }

            bool giaotannoi = collection["chk_giaotannoi"] != null;
            string ghiChu = (collection["txt_ghichu"] ?? "").Trim();

            // Nếu giao tận nơi thì nối địa chỉ giao vào ghi chú
            if (giaotannoi)
            {
                string diaChiGiao = (collection["txt_diachigiao"] ?? "").Trim();

                if (string.IsNullOrWhiteSpace(diaChiGiao) && khachHienTai != null)
                {
                    diaChiGiao = khachHienTai.diachi;
                }

                if (!string.IsNullOrWhiteSpace(diaChiGiao))
                {
                    if (!string.IsNullOrWhiteSpace(ghiChu))
                    {
                        ghiChu = ghiChu + " | Giao tận nơi: " + diaChiGiao;
                    }
                    else
                    {
                        ghiChu = "Giao tận nơi: " + diaChiGiao;
                    }
                }
            }

            if (!string.IsNullOrEmpty(ghiChu) && ghiChu.Length > 1000)
            {
                ghiChu = ghiChu.Substring(0, 1000);
            }

            // Tính tổng tiền
            decimal tongTien = 0;

            foreach (var sp in sanPhams)
            {
                if (gioHang.ContainsKey(sp.masanpham))
                {
                    tongTien += sp.dongia * gioHang[sp.masanpham];
                }
            }

            // Tạo hóa đơn
            int maHoaDonLonNhat = db.hoadons
                .Select(h => h.mahoadon)
                .DefaultIfEmpty(0)
                .Max();

            hoadon hd = new hoadon();
            hd.mahoadon = maHoaDonLonNhat + 1;
            hd.makh = makh;
            hd.nguoilap = null;
            hd.ngaydathang = ngayDatHang;
            hd.ngaygiaohang = ngayGiaoHang;
            hd.tongtien = tongTien;
            hd.dathanhtoan = false;
            hd.giaotannoi = giaotannoi;
            hd.trangthaidon = 0;
            hd.ghichu = ghiChu;

            db.hoadons.Add(hd);

            // Tạo chi tiết hóa đơn
            foreach (var sp in sanPhams)
            {
                int soLuong = gioHang.ContainsKey(sp.masanpham) ? gioHang[sp.masanpham] : 0;

                chitiethoadon ct = new chitiethoadon();
                ct.mahoadon = hd.mahoadon;
                ct.masanpham = sp.masanpham;
                ct.soluong = soLuong;
                ct.dongia = sp.dongia;
                ct.thanhtien = sp.dongia * soLuong;

                db.chitiethoadons.Add(ct);
            }

            db.SaveChanges();

            // Xóa giỏ hàng sau khi đặt thành công
            Session.Remove("GioHang");

            TempData["ThongBao"] = "Đặt hàng thành công!";

            return RedirectToAction("ThanhCong", new { id = hd.mahoadon });
        }

        // GET: /DatHang/ThanhCong/123
        public ActionResult ThanhCong(int id)
        {
            var hoaDon = db.hoadons.Find(id);

            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            return View(hoaDon);
        }

        // ---------- Hàm dùng chung ----------

        private List<sanpham> LaySanPhamTrongGio(Dictionary<string, int> gioHang)
        {
            var maSanPhamTrongGio = gioHang.Keys.ToList();

            var sanPhams = db.sanphams
                .Where(sp => maSanPhamTrongGio.Contains(sp.masanpham))
                .ToList();

            return sanPhams;
        }

        private decimal TinhTongTien(List<sanpham> sanPhams, Dictionary<string, int> gioHang)
        {
            decimal tongTien = 0;

            foreach (var sp in sanPhams)
            {
                if (gioHang.ContainsKey(sp.masanpham))
                {
                    tongTien += sp.dongia * gioHang[sp.masanpham];
                }
            }

            return tongTien;
        }

        private void ChuanBiViewDatHang(Dictionary<string, int> gioHang, List<sanpham> sanPhams)
        {
            ViewBag.GioHang = gioHang;
            ViewBag.TongTien = TinhTongTien(sanPhams, gioHang);

            int? maKHTuSession = Session["MaKH"] as int?;

            if (Session["LoaiTaiKhoan"] != null
                && Session["LoaiTaiKhoan"].ToString() == "KhachHang"
                && maKHTuSession.HasValue)
            {
                var khachHang = db.khachhangs.Find(maKHTuSession.Value);

                if (khachHang != null)
                {
                    ViewBag.KhachHang = khachHang;
                }
            }
        }
    }
}