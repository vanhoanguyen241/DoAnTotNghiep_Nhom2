using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public abstract class BaseAdminController : Controller
    {
        // Danh sách action mà nhân viên được phép truy cập
        private static readonly Dictionary<string, string[]> NhanVienAllowedActions =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "Home", new[] { "Index" } },
                { "TaiKhoan", new[] { "DangXuat" } },

                // Nhân viên được xem, thêm, sửa khách hàng; không xoá
                { "KhachHang", new[] { "Index", "TaoMoi", "Sua" } },

                // Nhân viên chỉ xem sản phẩm
                { "SanPham", new[] { "Index" } },

                // Nhân viên chỉ xem danh mục
                { "LoaiSanPham", new[] { "Index" } },
                { "DonViSanXuat", new[] { "Index" } },

                // Nhân viên chỉ xem hoá đơn và chi tiết hoá đơn
                { "HoaDon", new[] { "Index" } },
                { "ChiTietHoaDon", new[] { "Index" } },

                // Nhân viên được xem và duyệt thanh toán, không được huỷ thanh toán
                { "ThanhToan", new[] { "Index", "DaThanhToan", "DuyetThanhToan" } },

                // Nhân viên được xem, duyệt, từ chối đơn; không được xoá đơn từ chối
                {
                    "DuyetDon",
                    new[]
                    {
                        "Index",
                        "ChiTiet",
                        "XuLyDuyet",
                        "TuChoi",
                        "DanhSachTuChoi"
                    }
                },

                // Nhân viên xem giao hàng/đơn của mình và xác nhận giao hàng
                { "ChuyenHang", new[] { "Index", "XacNhanGiaoHang" } }
            };

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Phải đăng nhập là nhân viên mới được vào khu Admin
            if (Session["LoaiTaiKhoan"]?.ToString() != "NhanVien")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "TaiKhoan",
                        action = "DangNhap",
                        area = ""
                    }));
                return;
            }

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            string vaiTro = Session["VaiTro"]?.ToString();

            bool isAdmin = string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase);

            // Biến này dùng ngoài View để ẩn/hiện nút Thêm/Sửa/Xoá
            ViewBag.IsAdmin = isAdmin;

            // 2. Nếu không phải Admin thì kiểm tra whitelist
            if (!isAdmin)
            {
                bool isAllowed =
                    NhanVienAllowedActions.TryGetValue(controllerName, out string[] allowedActions)
                    && allowedActions.Contains(actionName, StringComparer.OrdinalIgnoreCase);

                if (!isAllowed)
                {
                    TempData["ThongBao"] = "Bạn không có quyền thực hiện chức năng này!";

                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "Index",
                            area = "Admin"
                        }));
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}