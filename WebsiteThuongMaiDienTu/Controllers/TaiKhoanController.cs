using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class TaiKhoanController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: TaiKhoan/DangNhap
        public ActionResult DangNhap()
        {
            return View();
        }

        // POST: TaiKhoan/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(taikhoan model)
        {
            if (string.IsNullOrEmpty(model.tendangnhap) || string.IsNullOrEmpty(model.matkhau))
            {
                ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập và mật khẩu");
                return View(model);
            }

            var taikhoan = db.taikhoans
                .FirstOrDefault(t => t.tendangnhap == model.tendangnhap
                                  && t.matkhau == model.matkhau);

            if (taikhoan == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            // Lưu thông tin đăng nhập vào Session
            Session["TenDangNhap"] = taikhoan.tendangnhap;

            if (taikhoan.manv != null)
            {
                Session["LoaiTaiKhoan"] = "NhanVien";
                Session["MaNV"] = taikhoan.manv;
            }
            else if (taikhoan.makh != null)
            {
                Session["LoaiTaiKhoan"] = "KhachHang";
                Session["MaKH"] = taikhoan.makh;
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: TaiKhoan/DangKy
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: TaiKhoan/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(string tendangnhap, string matkhau, string xacnhanmatkhau,
                                    string hoten, string diachi, string sodienthoai, string email)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(tendangnhap) || string.IsNullOrEmpty(matkhau) ||
                string.IsNullOrEmpty(hoten) || string.IsNullOrEmpty(diachi) || string.IsNullOrEmpty(sodienthoai))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin bắt buộc";
                return View();
            }

            // Kiểm tra mật khẩu
            if (matkhau.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự";
                return View();
            }

            if (matkhau != xacnhanmatkhau)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp";
                return View();
            }

            // Kiểm tra tên đăng nhập đã tồn tại
            var taikhoanTonTai = db.taikhoans.FirstOrDefault(t => t.tendangnhap == tendangnhap);
            if (taikhoanTonTai != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại";
                return View();
            }

            try
            {
                // Tạo khách hàng mới
                var khachhangMoi = new khachhang
                {
                    hoten = hoten,
                    diachi = diachi,
                    SDT = sodienthoai
                };

                db.khachhangs.Add(khachhangMoi);
                db.SaveChanges();

                // Tạo tài khoản mới
                var taikhoanMoi = new taikhoan
                {
                    tendangnhap = tendangnhap,
                    matkhau = matkhau,
                    makh = khachhangMoi.makh
                };

                db.taikhoans.Add(taikhoanMoi);
                db.SaveChanges();

                ViewBag.Success = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        // GET: TaiKhoan/DangXuat
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
