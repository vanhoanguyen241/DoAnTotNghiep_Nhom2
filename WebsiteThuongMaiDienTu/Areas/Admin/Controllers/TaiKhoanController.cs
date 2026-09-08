using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class TaiKhoanController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();
        // GET: TaiKhoan/DangXuat
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        }
    }
}
