using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class LoaiSanPhamController : Controller
    {
            private QLBanHang_Model db = new QLBanHang_Model();
        // GET: Admin/LoaiSanPham
        public ActionResult Index()
            {
                var ds = db.loaisanphams.ToList();
                return View(ds);
            }

            // GET: LoaiSanPham/TaoMoi
            public ActionResult TaoMoi()
            {
                return View();
            }

            // POST: LoaiSanPham/TaoMoi
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult TaoMoi(loaisanpham model)
            {
                if (string.IsNullOrWhiteSpace(model.maloaisanpham))
                {
                    ModelState.AddModelError(
                        "maloaisanpham",
                        "Mã loại sản phẩm là bắt buộc."
                    );
                }
                else if (db.loaisanphams.Any(x => x.maloaisanpham == model.maloaisanpham))
                {
                    ModelState.AddModelError(
                        "maloaisanpham",
                        "Mã loại sản phẩm đã tồn tại."
                    );
                }

                if (string.IsNullOrWhiteSpace(model.tenloaisanpham))
                {
                    ModelState.AddModelError(
                        "tenloaisanpham",
                        "Tên loại sản phẩm là bắt buộc."
                    );
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                try
                {
                    db.loaisanphams.Add(model);
                    db.SaveChanges();

                    TempData["ThongBaoThanhCong"] =
                        "Thêm loại sản phẩm thành công.";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(
                        "",
                        "Có lỗi xảy ra: " + ex.Message
                    );

                    return View(model);
                }
            }

            // GET: LoaiSanPham/Sua
            public ActionResult Sua(string id)
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index");
                }

                var loai = db.loaisanphams.Find(id);

                if (loai == null)
                {
                    return HttpNotFound();
                }

                return View(loai);
            }

            // POST: LoaiSanPham/Sua
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Sua(string id, loaisanpham model)
            {
                var loai = db.loaisanphams.Find(id);

                if (loai == null)
                {
                    return HttpNotFound();
                }

                if (string.IsNullOrWhiteSpace(model.tenloaisanpham))
                {
                    ModelState.AddModelError(
                        "tenloaisanpham",
                        "Tên loại sản phẩm là bắt buộc."
                    );
                }

                if (!ModelState.IsValid)
                {
                    model.maloaisanpham = id;
                    return View(model);
                }

                loai.tenloaisanpham = model.tenloaisanpham;

                try
                {
                    db.SaveChanges();

                    TempData["ThongBaoThanhCong"] =
                        "Cập nhật loại sản phẩm thành công.";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(
                        "",
                        "Có lỗi xảy ra: " + ex.Message
                    );

                    return View(loai);
                }
            }

            // POST: LoaiSanPham/Xoa
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Xoa(string id)
            {
                var loai = db.loaisanphams.Find(id);

                if (loai == null)
                {
                    return RedirectToAction("Index");
                }

                bool conSanPham = db.sanphams
                    .Any(sp => sp.maloaisanpham == id);

                if (conSanPham)
                {
                    TempData["ThongBaoLoi"] =
                        "Còn sản phẩm thuộc loại này, không thể xóa.";

                    return RedirectToAction("Index");
                }

                try
                {
                    db.loaisanphams.Remove(loai);
                    db.SaveChanges();

                    TempData["ThongBaoThanhCong"] =
                        "Xóa loại sản phẩm thành công.";
                }
                catch (DbUpdateException)
                {
                    TempData["ThongBaoLoi"] =
                        "Còn sản phẩm thuộc loại này, không thể xóa.";
                }

                return RedirectToAction("Index");
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