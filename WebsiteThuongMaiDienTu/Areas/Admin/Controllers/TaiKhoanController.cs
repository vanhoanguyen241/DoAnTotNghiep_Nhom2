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

        // GET: Admin/TaiKhoan/DangXuat
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap", "TaiKhoan", new { area = "" });
        }

        // GET: Admin/TaiKhoan
        public ActionResult Index()
        {
            // Lấy tất cả tài khoản (cả Nhân viên và Khách hàng)
            var dstk = db.taikhoans.OrderBy(t => t.tendangnhap).ToList();
            return View(dstk);
        }

        [HttpGet]
        public ActionResult TaoMoi()
        {
            NapDanhSachChon();
            return View();
        }

        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            string tendangnhap = collection["txt_tendangnhap"]?.Trim();
            string matkhau = collection["txt_matkhau"]?.Trim();
            string loai = collection["txt_loai"]; // "NhanVien" hoặc "KhachHang"

            if (string.IsNullOrEmpty(tendangnhap) || string.IsNullOrEmpty(matkhau))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                NapDanhSachChon();
                return View();
            }

            if (matkhau.Length < 6)
            {
                ViewBag.ThongBao = "Mật khẩu phải có ít nhất 6 ký tự!";
                NapDanhSachChon();
                return View();
            }

            if (db.taikhoans.Any(t => t.tendangnhap == tendangnhap))
            {
                ViewBag.ThongBao = "Tên đăng nhập đã tồn tại!";
                NapDanhSachChon();
                return View();
            }

            taikhoan tk = new taikhoan();
            tk.tendangnhap = tendangnhap;
            tk.matkhau = matkhau;

            if (loai == "NhanVien")
            {
                int manv;
                if (!int.TryParse(collection["txt_manv"], out manv))
                {
                    ViewBag.ThongBao = "Vui lòng chọn nhân viên!";
                    NapDanhSachChon();
                    return View();
                }
                tk.manv = manv;
                tk.makh = null;
            }
            else if (loai == "KhachHang")
            {
                int makh;
                if (!int.TryParse(collection["txt_makh"], out makh))
                {
                    ViewBag.ThongBao = "Vui lòng chọn khách hàng!";
                    NapDanhSachChon();
                    return View();
                }
                tk.makh = makh;
                tk.manv = null;
            }
            else
            {
                ViewBag.ThongBao = "Vui lòng chọn loại tài khoản!";
                NapDanhSachChon();
                return View();
            }

            db.taikhoans.Add(tk);
            db.SaveChanges();

            TempData["ThongBao"] = "Tạo tài khoản thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sua(string id)
        {
            var tk = db.taikhoans.Find(id);
            if (tk == null) return HttpNotFound();
            return View(tk);
        }

        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            string tendangnhap = collection["txt_tendangnhap"];
            var tk = db.taikhoans.Find(tendangnhap);
            if (tk == null) return HttpNotFound();

            string matkhaumoi = collection["txt_matkhaumoi"]?.Trim();

            if (!string.IsNullOrEmpty(matkhaumoi))
            {
                if (matkhaumoi.Length < 6)
                {
                    ViewBag.ThongBao = "Mật khẩu phải có ít nhất 6 ký tự!";
                    return View(tk);
                }
                tk.matkhau = matkhaumoi;
                db.SaveChanges();
                TempData["ThongBao"] = "Đổi mật khẩu thành công!";
            }
            else
            {
                TempData["ThongBao"] = "Không có thay đổi nào được thực hiện!";
            }

            return RedirectToAction("Index");
        }

        public ActionResult Xoa(string id)
        {
            var tk = db.taikhoans.Find(id);
            if (tk != null)
            {
                db.taikhoans.Remove(tk);
                db.SaveChanges();
                TempData["ThongBao"] = "Xóa tài khoản thành công!";
            }
            return RedirectToAction("Index");
        }

        // Nạp dropdown cho cả Nhân viên và Khách hàng chưa có tài khoản
        private void NapDanhSachChon()
        {
            var nvChuaCoTK = db.nhanviens
                .Where(n => !db.taikhoans.Any(t => t.manv == n.manv))
                .OrderBy(n => n.hoten)
                .ToList();
            ViewBag.manv = new SelectList(nvChuaCoTK, "manv", "hoten");

            var khChuaCoTK = db.khachhangs
                .Where(k => !db.taikhoans.Any(t => t.makh == k.makh))
                .OrderBy(k => k.hoten)
                .ToList();
            ViewBag.makh = new SelectList(khChuaCoTK, "makh", "hoten");
        }
    }
}