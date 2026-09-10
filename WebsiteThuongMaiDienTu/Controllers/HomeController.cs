using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class HomeController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        public ActionResult Index()
        {
            // Lấy sản phẩm nổi bật: có quảng cáo và còn hàng
            var sanPhamNoiBat = db.sanphams
                .Where(sp => sp.quangcao == true && sp.soluonghienco > 0)
                .OrderByDescending(sp => sp.masanpham)
                .Take(12)
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