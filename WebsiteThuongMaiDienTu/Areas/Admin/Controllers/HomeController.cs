using System.Web.Mvc;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        public ActionResult Index() => View();
    }
}