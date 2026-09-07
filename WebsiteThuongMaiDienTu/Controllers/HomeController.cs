using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class HomeController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm nổi bật (quangcao = 1)
            var sanPhamNoiBat = db.sanphams
                .Where(sp => sp.quangcao == true && sp.soluonghienco > 0)
                .OrderByDescending(sp => sp.masanpham)
                .Take(12)
                .ToList();

            return View(sanPhamNoiBat);
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