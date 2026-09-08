using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class SanPhamCongKhaiController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: SanPham/DanhSach
        public ActionResult DanhSach(string maloai, string tukhoa, int? page)
        {
            // Số sản phẩm trên mỗi trang
            int pageSize = 12;
            int pageNumber = page ?? 1;

            // Bắt đầu với query cơ bản
            var sanphams = db.sanphams.AsQueryable();

            // Lọc theo loại sản phẩm nếu có
            if (!string.IsNullOrEmpty(maloai))
            {
                sanphams = sanphams.Where(sp => sp.maloaisanpham == maloai);
                ViewBag.MaLoai = maloai;

                // Lấy tên loại sản phẩm
                var loai = db.loaisanphams.FirstOrDefault(l => l.maloaisanpham == maloai);
                ViewBag.TenLoai = loai != null ? loai.tenloaisanpham : "";
            }

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(tukhoa))
            {
                sanphams = sanphams.Where(sp =>
                    sp.tensanpham.Contains(tukhoa) ||
                    sp.mota.Contains(tukhoa));
                ViewBag.TuKhoa = tukhoa;
            }

            // Sắp xếp theo mã sản phẩm mới nhất
            sanphams = sanphams.OrderByDescending(sp => sp.masanpham);

            // Đếm tổng số sản phẩm
            int tongSoSanPham = sanphams.Count();
            ViewBag.TongSoSanPham = tongSoSanPham;

            // Tính tổng số trang
            int tongSoTrang = (int)Math.Ceiling((double)tongSoSanPham / pageSize);
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.TrangHienTai = pageNumber;
            ViewBag.PageSize = pageSize;

            // Phân trang thủ công
            var sanPhamsPhanTrang = sanphams
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy danh sách loại sản phẩm để hiển thị filter
            ViewBag.DanhSachLoai = db.loaisanphams.OrderBy(l => l.tenloaisanpham).ToList();

            return View(sanPhamsPhanTrang);
        }

        // GET: SanPham/ChiTiet/5
        public ActionResult ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var sanpham = db.sanphams
                .Include("loaisanpham")
                .Include("donvisanxuat")
                .FirstOrDefault(sp => sp.masanpham == id);

            if (sanpham == null)
            {
                return HttpNotFound();
            }

            // Lấy sản phẩm liên quan (cùng loại, khác mã)
            var sanPhamLienQuan = db.sanphams
                .Where(sp => sp.maloaisanpham == sanpham.maloaisanpham
                          && sp.masanpham != id
                          && sp.soluonghienco > 0)
                .OrderByDescending(sp => sp.quangcao)
                .Take(4)
                .ToList();

            ViewBag.SanPhamLienQuan = sanPhamLienQuan;

            return View(sanpham);
        }

        // GET: SanPham/TheoLoai/5
        public ActionResult TheoLoai(string id, int? page)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("DanhSach");
            }

            // Sử dụng action DanhSach với tham số maloai
            return RedirectToAction("DanhSach", new { maloai = id, page = page });
        }

        // GET: SanPham/TimKiem
        public ActionResult TimKiem(string tukhoa, string maloai, int? page)
        {
            if (string.IsNullOrEmpty(tukhoa) && string.IsNullOrEmpty(maloai))
            {
                return RedirectToAction("DanhSach");
            }

            // Sử dụng action DanhSach với tham số tìm kiếm
            return RedirectToAction("DanhSach", new { tukhoa = tukhoa, maloai = maloai, page = page });
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
