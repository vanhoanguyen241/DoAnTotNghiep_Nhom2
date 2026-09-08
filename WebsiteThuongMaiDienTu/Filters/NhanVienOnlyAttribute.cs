using System.Web.Mvc;
using System.Web.Routing;

namespace WebsiteThuongMaiDienTu.Filters
{
    // Bộ lọc chỉ cho phép Nhân viên đã đăng nhập truy cập Controller/Action được gắn [NhanVienOnly].
    // Nếu chưa đăng nhập hoặc đăng nhập với vai trò Khách hàng, tự động chuyển hướng về trang Đăng nhập.
    public class NhanVienOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var loaiTaiKhoan = filterContext.HttpContext.Session["LoaiTaiKhoan"] as string;

            if (loaiTaiKhoan != "NhanVien")
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