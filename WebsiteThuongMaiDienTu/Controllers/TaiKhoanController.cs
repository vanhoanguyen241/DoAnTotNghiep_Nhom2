using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

                    var nv = db.nhanviens.Find(tk_login.manv);
                    if (nv != null)
                    {
                        Session["VaiTro"] = nv.vaitro;
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
            var Tendangnhap = (collection["txt_tendangnhap"] ?? "").Trim();
            var Matkhau = (collection["txt_matkhau"] ?? "").Trim();
            var Xacnhanmk = (collection["txt_xacnhanmk"] ?? "").Trim();
            var Hoten = (collection["txt_hoten"] ?? "").Trim();
            var Diachi = (collection["txt_diachi"] ?? "").Trim();
            var Sodienthoai = (collection["txt_sdt"] ?? "").Trim();

            // Kiểm tra dữ liệu bắt buộc
            if (string.IsNullOrEmpty(Tendangnhap)
                || string.IsNullOrEmpty(Matkhau)
                || string.IsNullOrEmpty(Xacnhanmk)
                || string.IsNullOrEmpty(Hoten)
                || string.IsNullOrEmpty(Diachi)
                || string.IsNullOrEmpty(Sodienthoai))
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

            // ======================================================
            // ADR-009: Kiểm tra SDT đã tồn tại trong khachhang chưa
            // ======================================================
            int maKhMoi;

            var khachCu = (from kh in db.khachhangs
                           where kh.SDT == Sodienthoai
                           select kh).FirstOrDefault();

            if (khachCu != null)
            {
                // SDT đã tồn tại -> dùng lại khách hàng cũ
                maKhMoi = khachCu.makh;
            }
            else
            {
                // SDT chưa tồn tại -> tạo khách hàng mới
                if (db.khachhangs.Any())
                    maKhMoi = db.khachhangs.Max(k => k.makh) + 1;
                else
                    maKhMoi = 1;

                khachhang kh_moi = new khachhang();
                kh_moi.makh = maKhMoi;
                kh_moi.hoten = Hoten;
                kh_moi.diachi = Diachi;
                kh_moi.SDT = Sodienthoai;

                db.khachhangs.Add(kh_moi);
                db.SaveChanges();
            }

            // Tạo tài khoản khách hàng
            taikhoan tk_moi = new taikhoan();
            tk_moi.tendangnhap = Tendangnhap;
            tk_moi.matkhau = Matkhau;
            tk_moi.manv = null;
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

        // GET: TaiKhoan/DoiMatKhau
        public ActionResult DoiMatKhau()
        {
            // Chưa đăng nhập thì không cho vào
            if (Session["TenDangNhap"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            // Hiển thị thông báo thành công (truyền từ POST qua redirect)
            if (TempData["ThongBaoThanhCong"] != null)
            {
                ViewBag.ThongBaoThanhCong = TempData["ThongBaoThanhCong"];
            }
            return View();
        }

        // POST: TaiKhoan/DoiMatKhau
        [HttpPost]
        public ActionResult DoiMatKhau(FormCollection collection)
        {
            if (Session["TenDangNhap"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var matKhauCu = (collection["txt_matkhaucu"] ?? "").Trim();
            var matKhauMoi = (collection["txt_matkhaumoi"] ?? "").Trim();
            var xacNhanMk = (collection["txt_xacnhanmk"] ?? "").Trim();

            // 1. Kiểm tra điền đầy đủ
            if (string.IsNullOrEmpty(matKhauCu)
                || string.IsNullOrEmpty(matKhauMoi)
                || string.IsNullOrEmpty(xacNhanMk))
            {
                ViewBag.ThongBao = "Vui lòng điền đầy đủ các ô mật khẩu!";
                return View();
            }

            // 2. Lấy tài khoản đang đăng nhập
            var tendangnhap = Session["TenDangNhap"].ToString();
            var tk = (from t in db.taikhoans
                      where t.tendangnhap == tendangnhap
                      select t).FirstOrDefault();

            if (tk == null)
            {
                Session.Clear();
                return RedirectToAction("DangNhap");
            }

            // 3. Kiểm tra mật khẩu cũ
            if (tk.matkhau != matKhauCu)
            {
                ViewBag.ThongBao = "Mật khẩu cũ không đúng!";
                return View();
            }

            // 4. Kiểm tra độ dài mật khẩu mới
            if (matKhauMoi.Length < 6)
            {
                ViewBag.ThongBao = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return View();
            }

            // 5. Mật khẩu mới phải khác mật khẩu cũ
            if (matKhauMoi == matKhauCu)
            {
                ViewBag.ThongBao = "Mật khẩu mới phải khác mật khẩu cũ!";
                return View();
            }

            // 6. Kiểm tra xác nhận mật khẩu
            if (matKhauMoi != xacNhanMk)
            {
                ViewBag.ThongBao = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // 7. Cập nhật mật khẩu mới vào database
            tk.matkhau = matKhauMoi;
            db.SaveChanges();

            TempData["ThongBaoThanhCong"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại bằng mật khẩu mới.";
            return RedirectToAction("DangXuat", "TaiKhoan", new { area = "" });
        }
        public ActionResult ThongTin()
        {
            if (Session["MaKH"] == null) return RedirectToAction("DangNhap");
            int makh = (int)Session["MaKH"];
            var kh = db.khachhangs.Find(makh);
            if (kh == null) return HttpNotFound();

            if (TempData["ThongBao"] != null) ViewBag.ThongBao = TempData["ThongBao"];
            return View(kh);
        }

        [HttpPost]
        public ActionResult ThongTin(FormCollection collection)
        {
            if (Session["MaKH"] == null) return RedirectToAction("DangNhap");
            int makh = (int)Session["MaKH"];
            var kh = db.khachhangs.Find(makh);
            if (kh != null)
            {
                kh.hoten = collection["txt_hoten"];
                kh.diachi = collection["txt_diachi"];
                kh.SDT = collection["txt_sdt"];
                db.SaveChanges();
                TempData["ThongBao"] = "Cập nhật thông tin thành công!";
            }
            return RedirectToAction("ThongTin");
        }

        public ActionResult LichSuDonHang()
        {
            if (Session["MaKH"] == null) return RedirectToAction("DangNhap");
            int makh = (int)Session["MaKH"];
            var dsDonHang = db.dathangs
                .Where(d => d.makh == makh)
                .OrderByDescending(d => d.ngaydathang)
                .ToList();
            return View(dsDonHang);
        }
    }
}