using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class ThanhToanController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();
        // GET: Admin/ThanhToan/DuyetThanhToan/5
        public ActionResult DuyetThanhToan(int id)
        {
            var hd = db.hoadons.Find(id);
            if (hd == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hóa đơn!";
                return RedirectToAction("Index", "HoaDon");
            }
            if (hd.dathanhtoan)
            {
                TempData["ThongBao"] = "Hóa đơn " + id + " đã được thanh toán trước đó!";
                return RedirectToAction("Index", "ChiTietHoaDon", new { id = id });
            }

            hd.dathanhtoan = true;
            db.SaveChanges();
            TempData["ThongBao"] = "Đã xác nhận thanh toán cho hóa đơn " + id + "!";

            // Quay lại trang Chi tiết hóa đơn vừa duyệt
            return RedirectToAction("Index", "ChiTietHoaDon", new { id = id });
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
                TempData["ThongBao"] = "Hóa đơn " + id + " chưa được xác nhận thanh toán!";
                return RedirectToAction("Index");
            }

            // Nếu đã có phiếu giao hàng thì không cho hủy thanh toán
            bool coPhieuGiao = db.chuyenhangs.Any(ch => ch.mahoadon == id);
            if (coPhieuGiao)
            {
                TempData["ThongBao"] = "Không thể hủy thanh toán vì hóa đơn " + id + " đã có phiếu giao hàng!";
                return RedirectToAction("DaThanhToan");
            }

            hd.dathanhtoan = false;
            db.SaveChanges();

            TempData["ThongBao"] = "Đã hủy xác nhận thanh toán cho hóa đơn " + id + "!";
            return RedirectToAction("DaThanhToan");
        }
    }
}