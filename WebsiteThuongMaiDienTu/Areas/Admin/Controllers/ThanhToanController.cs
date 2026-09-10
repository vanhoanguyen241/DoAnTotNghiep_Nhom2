using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class ThanhToanController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/ThanhToan
        // Danh sách hóa đơn chưa thanh toán
        public ActionResult Index()
        {
            var dsChoThanhToan = db.hoadons
                .Include(h => h.khachhang)
                .Include(h => h.dathangs)
                .Where(h => h.dathanhtoan == false)
                .OrderByDescending(h => h.ngaydathang)
                .ToList();

            return View(dsChoThanhToan);
        }

        // GET: Admin/ThanhToan/DaThanhToan
        // Danh sách hóa đơn đã thanh toán
        public ActionResult DaThanhToan()
        {
            var dsDaThanhToan = db.hoadons
                .Include(h => h.khachhang)
                .Include(h => h.dathangs)
                .Where(h => h.dathanhtoan == true)
                .OrderByDescending(h => h.ngaydathang)
                .ToList();

            return View(dsDaThanhToan);
        }

        // GET: Admin/ThanhToan/DuyetThanhToan/5
        public ActionResult DuyetThanhToan(int id)
        {
            var hd = db.hoadons.Find(id);

            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("Index");
            }

            if (hd.dathanhtoan)
            {
                TempData["ThongBao"] = "Hóa đơn #" + id + " đã được thanh toán trước đó!";
                return RedirectToAction("DaThanhToan");
            }

            // Xác nhận thanh toán
            hd.dathanhtoan = true;
            db.SaveChanges();

            TempData["ThongBao"] = "Đã xác nhận thanh toán cho hóa đơn #" + id + "!";
            return RedirectToAction("Index");
        }

        // GET: Admin/ThanhToan/HuyThanhToan/5
        public ActionResult HuyThanhToan(int id)
        {
            var hd = db.hoadons.Find(id);

            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("DaThanhToan");
            }

            if (!hd.dathanhtoan)
            {
                TempData["ThongBao"] = "Hóa đơn #" + id + " chưa được xác nhận thanh toán!";
                return RedirectToAction("Index");
            }

            // Nếu đã có phiếu giao hàng thì không cho hủy thanh toán
            bool coPhieuGiao = db.chuyenhangs.Any(ch => ch.mahoadon == id);
            if (coPhieuGiao)
            {
                TempData["ThongBao"] = "Không thể hủy thanh toán vì hóa đơn #" + id + " đã có phiếu giao hàng!";
                return RedirectToAction("DaThanhToan");
            }

            hd.dathanhtoan = false;
            db.SaveChanges();

            TempData["ThongBao"] = "Đã hủy xác nhận thanh toán cho hóa đơn #" + id + "!";
            return RedirectToAction("DaThanhToan");
        }
    }
}