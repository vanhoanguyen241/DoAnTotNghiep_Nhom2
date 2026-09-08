using System.Web.Mvc;
using System.Web.Routing;

namespace WebsiteThuongMaiDienTu.Filters
{
    // Bộ lọc chỉ cho phép Khách hàng đã đăng nhập truy cập Controller/Action được gắn [KhachHangOnly].
    // Nếu chưa đăng nhập hoặc đăng nhập với vai trò Nhân viên, tự động chuyển hướng về trang Đăng nhập.
    public class KhachHangOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var loaiTaiKhoan = filterContext.HttpContext.Session["LoaiTaiKhoan"] as string;

            if (loaiTaiKhoan != "KhachHang")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "TaiKhoan",
                        action = "DangNhap"
                    }));
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}