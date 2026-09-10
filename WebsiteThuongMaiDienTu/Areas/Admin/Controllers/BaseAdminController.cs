using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public abstract class BaseAdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Check đã đăng nhập và là nhân viên chưa
            if (Session["LoaiTaiKhoan"]?.ToString() != "NhanVien")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "TaiKhoan", action = "DangNhap", area = "" }));
                return;
            }

            // 2. Check quyền sâu
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string vaiTro = Session["VaiTro"]?.ToString();

            if (vaiTro != "Admin")
            {
                // Danh sách các Controller chỉ Admin mới được vào
                List<string> adminOnlyControllers = new List<string>
                {
                    "LoaiSanPham",
                    "DonViSanXuat",
                    "HoaDon",
                    "ChiTietHoaDon",
                    "ThanhToan"
                };

                if (adminOnlyControllers.Contains(controllerName))
                {
                    // Chuyển về trang chủ Admin
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Index", area = "Admin" }));
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}