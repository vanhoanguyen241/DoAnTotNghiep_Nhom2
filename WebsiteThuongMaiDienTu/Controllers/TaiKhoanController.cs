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
        QLBanHang_Model db = new QLBanHang_Model();

        // GET: TaiKhoan/DangNhap
        public ActionResult DangNhap()
        {
            return View();
        }

        // POST: TaiKhoan/DangNhap
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var Tendangnhap = collection["txt_tendangnhap"];
            var Matkhau = collection["txt_matkhau"];

            if (string.IsNullOrEmpty(Tendangnhap) || string.IsNullOrEmpty(Matkhau))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                return View();
            }

            var tk_login = (from s in db.taikhoans
                            where s.tendangnhap == Tendangnhap && s.matkhau == Matkhau
                            select s).FirstOrDefault();

            if (tk_login != null)
            {
                Session["TenDangNhap"] = tk_login.tendangnhap;

                if (tk_login.manv != null)
                {
                    Session["LoaiTaiKhoan"] = "NhanVien";
                    Session["MaNV"] = tk_login.manv;

                    // Lấy vai trò từ bảng nhanvien
                    var nv = db.nhanviens.Find(tk_login.manv);
                    if (nv != null)
                    {
                        Session["VaiTro"] = nv.vaitro; // Lưu 'Admin' hoặc 'NhanVien'
                    }
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
            else if (tk_login.makh != null)
                {
                    Session["LoaiTaiKhoan"] = "KhachHang";
                    Session["MaKH"] = tk_login.makh;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: TaiKhoan/DangKy
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: TaiKhoan/DangKy
        [HttpPost]
        public ActionResult DangKy(FormCollection collection)
        {
            var Tendangnhap = collection["txt_tendangnhap"];
            var Matkhau = collection["txt_matkhau"];
            var Xacnhanmk = collection["txt_xacnhanmk"];
            var Hoten = collection["txt_hoten"];
            var Diachi = collection["txt_diachi"];
            var Sodienthoai = collection["txt_sdt"];

            // Kiểm tra dữ liệu bắt buộc
            if (string.IsNullOrEmpty(Tendangnhap) || string.IsNullOrEmpty(Matkhau) ||
                string.IsNullOrEmpty(Xacnhanmk) || string.IsNullOrEmpty(Hoten) ||
                string.IsNullOrEmpty(Diachi) || string.IsNullOrEmpty(Sodienthoai))
            {
                ViewBag.ThongBao = "Vui lòng điền đầy đủ thông tin bắt buộc!";
                return View();
            }

            if (Matkhau.Length < 6)
            {
                ViewBag.ThongBao = "Mật khẩu phải có ít nhất 6 ký tự!";
                return View();
            }

            if (Matkhau != Xacnhanmk)
            {
                ViewBag.ThongBao = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // Kiểm tra tên đăng nhập đã tồn tại chưa
            var tk_tontai = (from t in db.taikhoans
                             where t.tendangnhap == Tendangnhap
                             select t).FirstOrDefault();

            if (tk_tontai != null)
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            // Lấy mã khách hàng lớn nhất hiện có trong bảng khachhang
            var maKhLonNhat = (from kh in db.khachhangs
                               select kh.makh).DefaultIfEmpty(0).Max();

            int maKhMoi = maKhLonNhat + 1;

            // Tạo khách hàng mới
            khachhang kh_moi = new khachhang();
            kh_moi.makh = maKhMoi;
            kh_moi.hoten = Hoten;
            kh_moi.diachi = Diachi;
            kh_moi.SDT = Sodienthoai;

            db.khachhangs.Add(kh_moi);
            db.SaveChanges();

            // Tạo tài khoản mới
            taikhoan tk_moi = new taikhoan();
            tk_moi.tendangnhap = Tendangnhap;
            tk_moi.matkhau = Matkhau;
            tk_moi.makh = maKhMoi;

            db.taikhoans.Add(tk_moi);
            db.SaveChanges();

            ViewBag.ThongBaoThanhCong = "Đăng ký thành công! Vui lòng đăng nhập.";
            return View();
        }

        // GET: TaiKhoan/DangXuat
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        }
    }
}