using System;
using System.Data.Entity; // Cần thiết để dùng .Include()
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class TraCuuDonHangController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: /TraCuuDonHang
        public ActionResult Index()
        {
            return View();
        }

        // POST: /TraCuuDonHang/TimKiem
        [HttpPost]
        public ActionResult TimKiem(FormCollection collection)
        {
            int madathang;
            if (int.TryParse(collection["txt_madathang"], out madathang))
            {
                return RedirectToAction("KetQua", new { id = madathang });
            }

            ViewBag.ThongBao = "Mã đơn hàng không hợp lệ! Vui lòng nhập số.";
            return View("Index");
        }

        // GET: /TraCuuDonHang/KetQua/123
        public ActionResult KetQua(int id)
        {
            // Lấy đầy đủ thông tin: Đơn hàng -> Khách hàng -> Chi tiết đơn -> Sản phẩm
            var donHang = db.dathangs
                .Include(d => d.khachhang)
                .Include(d => d.chitietdathangs.Select(ct => ct.sanpham))
                .FirstOrDefault(d => d.madathang == id);

            if (donHang == null)
            {
                ViewBag.ThongBao = "Không tìm thấy đơn hàng với mã \"" + id + "\". Vui lòng kiểm tra lại!";
                return View("Index");
            }

            return View(donHang);
        }
    }
}