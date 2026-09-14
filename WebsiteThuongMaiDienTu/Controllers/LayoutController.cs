// WebsiteThuongMaiDienTu\Controllers\LayoutController.cs
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class LayoutController : Controller
    {
        [ChildActionOnly]
        public ActionResult DanhMucNav()
        {
            using (var db = new QLBanHang_Model())
            {
                var danhMuc = db.loaisanphams
                    .OrderBy(l => l.tenloaisanpham)
                    .ToList();
                return PartialView("_DanhMucNav", danhMuc);
            }
        }

        [ChildActionOnly]
        public ActionResult DanhMucFooter()
        {
            using (var db = new QLBanHang_Model())
            {
                var danhMuc = db.loaisanphams
                    .OrderBy(l => l.tenloaisanpham)
                    .ToList();
                return PartialView("_DanhMucFooter", danhMuc);
            }
        }
    }
}