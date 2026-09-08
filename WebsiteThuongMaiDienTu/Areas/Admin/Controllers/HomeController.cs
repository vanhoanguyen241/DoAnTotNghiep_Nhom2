using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Filters;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        public ActionResult Index() => View();
    }
}