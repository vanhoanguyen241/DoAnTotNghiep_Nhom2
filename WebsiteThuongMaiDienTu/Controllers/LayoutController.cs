using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    // Controller phụ trách các thành phần dùng chung trong Layout (ví dụ: thanh danh mục sản phẩm).
    // Action trong Controller này chỉ được gọi từ View bằng @Html.Action(), không có URL truy cập trực tiếp.
    public class LayoutController : Controller
    {
        [ChildActionOnly]
        public ActionResult DanhMucNav()
        {
            using (var db = new QLBanHang_Model())
            {
                var danhMuc = db.loaisanphams
                    .OrderBy(l => l.tenloaisanpham)
                    .Take(8)
                    .ToList();

                return PartialView("_DanhMucNav", danhMuc);
            }
        }
    }
}