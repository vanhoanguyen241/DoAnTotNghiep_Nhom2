using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class HomeController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        public ActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = page ?? 1;

            var query = db.sanphams
                .Where(sp => sp.quangcao == true && sp.soluonghienco > 0)
                .OrderByDescending(sp => sp.masanpham);

            int tongSoSanPham = query.Count();
            ViewBag.TongSoTrang = (int)Math.Ceiling((double)tongSoSanPham / pageSize);
            ViewBag.TrangHienTai = pageNumber;

            var sanPhamNoiBat = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(sanPhamNoiBat);
        }

        public ActionResult GioiThieu()
        {
            return View();
        }

        public ActionResult TuyenDung()
        {
            return View();
        }

        public ActionResult LienHe()
        {
            return View();
        }
    }
}