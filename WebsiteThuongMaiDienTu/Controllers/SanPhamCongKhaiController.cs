using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class SanPhamCongKhaiController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: /SanPhamCongKhai/DanhSach
        public ActionResult DanhSach(string maloai, string tukhoa, int? page)
        {
            int pageSize = 12;
            int pageNumber = page ?? 1;

            var query = db.sanphams.AsQueryable();

            // Lọc theo loại sản phẩm
            if (!string.IsNullOrEmpty(maloai))
            {
                query = query.Where(sp => sp.maloaisanpham == maloai);

                var loai = db.loaisanphams.FirstOrDefault(l => l.maloaisanpham == maloai);

                ViewBag.MaLoai = maloai;
                ViewBag.TenLoai = loai != null ? loai.tenloaisanpham : "";
            }

            // Tìm kiếm theo tên hoặc mô tả
            if (!string.IsNullOrEmpty(tukhoa))
            {
                query = query.Where(sp =>
                    sp.tensanpham.Contains(tukhoa) ||
                    sp.mota.Contains(tukhoa));

                ViewBag.TuKhoa = tukhoa;
            }

            // Sắp xếp mới nhất theo mã sản phẩm
            query = query.OrderByDescending(sp => sp.masanpham);

            // Tính phân trang đơn giản
            int tongSoSanPham = query.Count();
            int tongSoTrang = (int)Math.Ceiling((double)tongSoSanPham / pageSize);

            ViewBag.TongSoSanPham = tongSoSanPham;
            ViewBag.TongSoTrang = tongSoTrang;
            ViewBag.TrangHienTai = pageNumber;

            // Lấy danh sách loại để render bộ lọc
            ViewBag.DanhSachLoai = db.loaisanphams
                .OrderBy(l => l.tenloaisanpham)
                .ToList();

            var danhSach = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(danhSach);
        }

        // GET: /SanPhamCongKhai/ChiTiet/SP01
        public ActionResult ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var sanpham = db.sanphams.Find(id);

            if (sanpham == null)
            {
                return HttpNotFound();
            }

            var loai = db.loaisanphams
                .FirstOrDefault(l => l.maloaisanpham == sanpham.maloaisanpham);

            var nsx = db.donvisanxuats
                .FirstOrDefault(d => d.madonvisanxuat == sanpham.madonvisanxuat);

            ViewBag.TenLoai = loai != null ? loai.tenloaisanpham : "";
            ViewBag.TenNSX = nsx != null ? nsx.tendonvisanxuat : "";

            return View(sanpham);
        }
    }
}