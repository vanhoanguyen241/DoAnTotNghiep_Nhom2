using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Filters;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    [NhanVienOnly]
    public class HomeController : Controller
    {
        public ActionResult Index() => View();
    }
}