using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        public ActionResult Index()
        {
            // 1. Thống kê nhanh (Giữ lại để làm thẻ Stats)
            ViewBag.TongSanPham = db.sanphams.Count();
            ViewBag.DonChoDuyet = db.dathangs.Count(d => d.trangthai == 0);
            ViewBag.TongKhachHang = db.khachhangs.Count();
            ViewBag.TongHoaDon = db.hoadons.Count();

            // 2. Dữ liệu cho các Tab (Lấy 10 bản ghi mới nhất)
            ViewBag.DSSanPham = db.sanphams
                .OrderByDescending(x => x.masanpham)
                .Take(10)
                .ToList();

            ViewBag.DSKhachHang = db.khachhangs
                .OrderByDescending(x => x.makh)
                .Take(10)
                .ToList();

            ViewBag.DSDonHang = db.dathangs
                .Include(d => d.khachhang)
                .Include(d => d.chitietdathangs)
                .OrderByDescending(d => d.ngaydathang)
                .Take(10)
                .ToList();

            ViewBag.DSHoaDon = db.hoadons
                .Include(h => h.khachhang)
                .OrderByDescending(h => h.ngaydathang)
                .Take(10)
                .ToList();

            return View();
        }
    }
}