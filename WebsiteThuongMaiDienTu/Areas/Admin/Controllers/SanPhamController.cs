using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class SanPhamController : BaseAdminController
    {
        QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/SanPham
        public ActionResult Index()
        {
            var ds_sp = db.sanphams.ToList();
            return View(ds_sp);
        }

        // GET: Admin/SanPham/TaoMoi
        [HttpGet]
        public ActionResult TaoMoi()
        {
            NapDanhSachChonLua();
            return View();
        }

        // POST: Admin/SanPham/TaoMoi
        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            // Lấy dữ liệu từ View đúng theo tên name trong form của bạn
            string masanpham = collection["txt_masp"];
            string tensanpham = collection["txt_tensp"];
            string maloaisanpham = collection["txt_lsp"];
            string madonvisanxuat = collection["txt_dvsx"];
            string mota = collection["txt_mota"];

            // Validate dữ liệu bắt buộc
            if (string.IsNullOrEmpty(masanpham) || string.IsNullOrEmpty(tensanpham)
                || string.IsNullOrEmpty(maloaisanpham) || string.IsNullOrEmpty(madonvisanxuat))
            {
                ViewBag.ThongBao = "Vui lòng điền đầy đủ Mã SP, Tên SP, Loại SP và Nhà sản xuất!";
                NapDanhSachChonLua(maloaisanpham, madonvisanxuat);
                return View();
            }

            // Mã sản phẩm không được trùng
            if (db.sanphams.Find(masanpham) != null)
            {
                ViewBag.ThongBao = "Mã sản phẩm \"" + masanpham + "\" đã tồn tại!";
                NapDanhSachChonLua(maloaisanpham, madonvisanxuat);
                return View();
            }

            decimal dongia = decimal.Parse(collection["txt_dongia"]);
            int thoigianbaohanh = int.Parse(collection["txt_tgbaohanh"]);
            int soluonghienco = int.Parse(collection["txt_soluong"]);

            if (dongia <= 0)
            {
                ViewBag.ThongBao = "Đơn giá phải lớn hơn 0!";
                NapDanhSachChonLua(maloaisanpham, madonvisanxuat);
                return View();
            }

            if (soluonghienco < 0 || thoigianbaohanh < 0)
            {
                ViewBag.ThongBao = "Số lượng và thời gian bảo hành không được âm!";
                NapDanhSachChonLua(maloaisanpham, madonvisanxuat);
                return View();
            }

            // Gán vào model đúng tên trường trong sanpham.cs
            sanpham sp = new sanpham();
            sp.masanpham = masanpham;
            sp.tensanpham = tensanpham;
            sp.maloaisanpham = maloaisanpham;
            sp.madonvisanxuat = madonvisanxuat;
            sp.mota = mota;
            sp.thoigianbaohanh = thoigianbaohanh;
            sp.dongia = dongia;
            sp.soluonghienco = soluonghienco;
            sp.quangcao = collection["chk_quangcao"] != null;

            // Xử lý nhập tên file ảnh (thay cho upload file)
            string tenFileHinh = collection["txt_hinh"]?.Trim();
            if (string.IsNullOrWhiteSpace(tenFileHinh))
            {
                sp.hinh = "~/Content/images/sanpham/default.png";
            }
            else
            {
                sp.hinh = "~/Content/images/sanpham/" + tenFileHinh;
            }

            db.sanphams.Add(sp);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm sản phẩm \"" + sp.tensanpham + "\" thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/SanPham/Sua/SP01
        [HttpGet]
        public ActionResult Sua(string id)
        {
            var sp = db.sanphams.Find(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            NapDanhSachChonLua(sp.maloaisanpham, sp.madonvisanxuat);
            return View(sp);
        }

        // POST: Admin/SanPham/Sua
        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            string masanpham = collection["txt_masp"];
            var sua_sp = db.sanphams.Find(masanpham);
            if (sua_sp == null)
            {
                return HttpNotFound();
            }

            string tensanpham = collection["txt_tensp"];
            if (string.IsNullOrEmpty(tensanpham))
            {
                ViewBag.ThongBao = "Tên sản phẩm không được để trống!";
                NapDanhSachChonLua(collection["txt_lsp"], collection["txt_dvsx"]);
                return View(sua_sp);
            }

            decimal dongia = decimal.Parse(collection["txt_dongia"]);
            int thoigianbaohanh = int.Parse(collection["txt_tgbaohanh"]);
            int soluonghienco = int.Parse(collection["txt_soluong"]);

            if (dongia <= 0)
            {
                ViewBag.ThongBao = "Đơn giá phải lớn hơn 0!";
                NapDanhSachChonLua(collection["txt_lsp"], collection["txt_dvsx"]);
                return View(sua_sp);
            }

            // Cập nhật đúng tên trường
            sua_sp.tensanpham = tensanpham;
            sua_sp.maloaisanpham = collection["txt_lsp"];
            sua_sp.madonvisanxuat = collection["txt_dvsx"];
            sua_sp.mota = collection["txt_mota"];
            sua_sp.thoigianbaohanh = thoigianbaohanh;
            sua_sp.dongia = dongia;
            sua_sp.soluonghienco = soluonghienco;
            sua_sp.quangcao = collection["chk_quangcao"] != null;

            // Cập nhật ảnh nếu người dùng có nhập tên file mới
            string tenFileHinhMoi = collection["txt_hinh"]?.Trim();
            if (!string.IsNullOrWhiteSpace(tenFileHinhMoi))
            {
                sua_sp.hinh = "~/Content/images/sanpham/" + tenFileHinhMoi;
            }

            db.SaveChanges();

            TempData["ThongBao"] = "Cập nhật sản phẩm \"" + sua_sp.tensanpham + "\" thành công!";
            return RedirectToAction("Index");
        }

        // GET: Admin/SanPham/Xoa/SP01
        public ActionResult Xoa(string id)
        {
            var xoa_sp = db.sanphams.Find(id);
            if (xoa_sp != null)
            {
                try
                {
                    db.sanphams.Remove(xoa_sp);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Xóa sản phẩm \"" + xoa_sp.tensanpham + "\" thành công!";
                }
                catch (DbUpdateException)
                {
                    // Sản phẩm đã nằm trong chitiethoadon -> FK chặn xóa
                    TempData["ThongBao"] = "Sản phẩm \"" + xoa_sp.tensanpham + "\" đã có trong lịch sử bán hàng, không thể xóa!";
                }
            }
            return RedirectToAction("Index");
        }

        // ---------- Hàm dùng chung ----------
        // Nạp dropdown Loại sản phẩm + Nhà sản xuất (Đúng chuẩn View của bạn)
        private void NapDanhSachChonLua(string maloaiDaChon = null, string maDvsxDaChon = null)
        {
            var loai_sp = db.loaisanphams.ToList();
            ViewBag.maloaisanpham = new SelectList(loai_sp, "maloaisanpham", "tenloaisanpham", maloaiDaChon);

            var donvi_sx = db.donvisanxuats.ToList();
            ViewBag.donvisanxuat = new SelectList(donvi_sx, "madonvisanxuat", "tendonvisanxuat", maDvsxDaChon);
        }
    }
}