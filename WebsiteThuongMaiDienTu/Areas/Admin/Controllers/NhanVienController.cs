using System;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class NhanVienController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/NhanVien
        public ActionResult Index()
        {
            var dsnv = db.nhanviens.OrderBy(n => n.hoten).ToList();
            return View(dsnv);
        }

        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            string hoten = collection["txt_hoten"]?.Trim();
            string dienthoai = collection["txt_dienthoai"]?.Trim();
            string vaitro = collection["txt_vaitro"]?.Trim();

            if (string.IsNullOrEmpty(hoten))
            {
                ViewBag.ThongBao = "Vui lòng nhập họ tên nhân viên!";
                return View();
            }

            if (string.IsNullOrEmpty(vaitro) || (vaitro != "Admin" && vaitro != "NhanVien"))
            {
                ViewBag.ThongBao = "Vai trò không hợp lệ!";
                return View();
            }

            nhanvien nv = new nhanvien();
            nv.hoten = hoten;
            nv.dienthoai = dienthoai;
            nv.vaitro = vaitro;

            db.nhanviens.Add(nv);
            db.SaveChanges();

            TempData["ThongBao"] = "Thêm nhân viên thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sua(int id)
        {
            var nv = db.nhanviens.Find(id);
            if (nv == null) return HttpNotFound();
            return View(nv);
        }

        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            int id;
            if (!int.TryParse(collection["txt_manv"], out id)) return RedirectToAction("Index");

            var nv = db.nhanviens.Find(id);
            if (nv == null) return HttpNotFound();

            string hoten = collection["txt_hoten"]?.Trim();
            string dienthoai = collection["txt_dienthoai"]?.Trim();
            string vaitro = collection["txt_vaitro"]?.Trim();

            if (string.IsNullOrEmpty(hoten))
            {
                ViewBag.ThongBao = "Vui lòng nhập họ tên nhân viên!";
                return View(nv);
            }

            if (string.IsNullOrEmpty(vaitro) || (vaitro != "Admin" && vaitro != "NhanVien"))
            {
                ViewBag.ThongBao = "Vai trò không hợp lệ!";
                return View(nv);
            }

            nv.hoten = hoten;
            nv.dienthoai = dienthoai;
            nv.vaitro = vaitro;

            db.SaveChanges();
            TempData["ThongBao"] = "Cập nhật nhân viên thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult Xoa(int id)
        {
            var nv = db.nhanviens.Find(id);
            if (nv != null)
            {
                // Chặn xóa nếu đã có dữ liệu liên quan
                bool coHoaDon = db.hoadons.Any(h => h.nguoilap == id);
                bool coPhieuGiao = db.chuyenhangs.Any(c => c.manv == id);
                bool coTaiKhoan = db.taikhoans.Any(t => t.manv == id);

                if (coHoaDon || coPhieuGiao || coTaiKhoan)
                {
                    TempData["ThongBao"] = "Không thể xóa nhân viên \"" + nv.hoten + "\" vì đã có dữ liệu liên quan (hóa đơn, phiếu giao, tài khoản)!";
                }
                else
                {
                    db.nhanviens.Remove(nv);
                    db.SaveChanges();
                    TempData["ThongBao"] = "Xóa nhân viên thành công!";
                }
            }
            return RedirectToAction("Index");
        }
    }
}