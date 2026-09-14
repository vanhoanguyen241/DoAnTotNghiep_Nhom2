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