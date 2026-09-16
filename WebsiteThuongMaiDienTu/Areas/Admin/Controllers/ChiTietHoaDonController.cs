using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class ChiTietHoaDonController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/ChiTietHoaDon/Index/5
        public ActionResult Index(int id)
        {
            var hd = db.hoadons.Include("khachhang").FirstOrDefault(h => h.mahoadon == id);
            if (hd == null)
            {
                return HttpNotFound();
            }
            ViewBag.HoaDon = hd;
            var dsct = db.chitiethoadons
                .Where(x => x.mahoadon == id)
                .ToList();
            return View(dsct);
        }
    }
}