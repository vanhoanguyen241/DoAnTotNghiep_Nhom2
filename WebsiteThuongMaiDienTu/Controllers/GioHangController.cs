using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Controllers
{
    public class GioHangController : Controller
    {
        private QLBanHang_Model db = new QLBanHang_Model();

        // Lấy giỏ hàng từ Session.
        // Giỏ hàng chỉ lưu: masanpham -> soluong
        private Dictionary<string, int> LayGioHang()
        {
            var gioHang = Session["GioHang"] as Dictionary<string, int>;

            if (gioHang == null)
            {
                gioHang = new Dictionary<string, int>();
                Session["GioHang"] = gioHang;
            }

            return gioHang;
        }

        // GET: /GioHang
        public ActionResult Index()
        {
            var gioHang = LayGioHang();

            var maSanPhamTrongGio = gioHang.Keys.ToList();
            var sanPhams = new List<sanpham>();

            if (maSanPhamTrongGio.Any())
            {
                sanPhams = db.sanphams
                    .Where(sp => maSanPhamTrongGio.Contains(sp.masanpham))
                    .ToList();
            }

            // Xoá các mã sản phẩm không còn tồn tại trong database
            var maTonTai = sanPhams.Select(sp => sp.masanpham).ToList();
            var maCanXoa = maSanPhamTrongGio
                .Where(m => !maTonTai.Contains(m))
                .ToList();

            foreach (var ma in maCanXoa)
            {
                gioHang.Remove(ma);
            }

            // Chỉ giữ sản phẩm còn hàng; nếu số lượng lớn hơn tồn kho thì tự giảm về tồn kho
            var danhSachHopLe = new List<sanpham>();

            foreach (var sp in sanPhams)
            {
                if (sp.soluonghienco <= 0)
                {
                    gioHang.Remove(sp.masanpham);
                    continue;
                }

                if (gioHang.ContainsKey(sp.masanpham) && gioHang[sp.masanpham] > sp.soluonghienco)
                {
                    gioHang[sp.masanpham] = sp.soluonghienco;
                }

                danhSachHopLe.Add(sp);
            }

            sanPhams = danhSachHopLe;

            // Tính tổng tiền
            decimal tongTien = 0;

            foreach (var sp in sanPhams)
            {
                if (gioHang.ContainsKey(sp.masanpham))
                {
                    int soLuong = gioHang[sp.masanpham];
                    tongTien += sp.dongia * soLuong;
                }
            }

            ViewBag.GioHang = gioHang;
            ViewBag.TongTien = tongTien;

            return View(sanPhams);
        }

        // GET hoặc POST: /GioHang/Them/SP01
        public ActionResult Them(string id, int? sl)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            var sanpham = db.sanphams.Find(id);

            if (sanpham == null)
            {
                TempData["LoiGioHang"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }

            if (sanpham.soluonghienco <= 0)
            {
                TempData["LoiGioHang"] = "Sản phẩm \"" + sanpham.tensanpham + "\" đã hết hàng.";
                return RedirectToAction("Index");
            }

            var gioHang = LayGioHang();

            int soLuongThem = sl ?? 1;

            if (soLuongThem <= 0)
            {
                soLuongThem = 1;
            }

            if (gioHang.ContainsKey(id))
            {
                int soLuongMoi = gioHang[id] + soLuongThem;

                if (soLuongMoi > sanpham.soluonghienco)
                {
                    soLuongMoi = sanpham.soluonghienco;
                }

                gioHang[id] = soLuongMoi;
            }
            else
            {
                if (soLuongThem > sanpham.soluonghienco)
                {
                    soLuongThem = sanpham.soluonghienco;
                }

                gioHang.Add(id, soLuongThem);
            }

            TempData["ThongBaoGioHang"] = "Đã thêm sản phẩm vào giỏ hàng.";
            return RedirectToAction("Index");
        }

        // POST: /GioHang/CapNhat
        // hanhdong: them | bot | xoa
        public ActionResult CapNhat(string id, string hanhdong)
        {
            var gioHang = LayGioHang();

            if (string.IsNullOrEmpty(id) || !gioHang.ContainsKey(id))
            {
                return RedirectToAction("Index");
            }

            if (hanhdong == "xoa")
            {
                gioHang.Remove(id);
                TempData["ThongBaoGioHang"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
                return RedirectToAction("Index");
            }

            var sanpham = db.sanphams.Find(id);

            if (sanpham == null)
            {
                gioHang.Remove(id);
                return RedirectToAction("Index");
            }

            if (hanhdong == "them")
            {
                if (sanpham.soluonghienco <= 0)
                {
                    TempData["LoiGioHang"] = "Sản phẩm đã hết hàng.";
                }
                else if (gioHang[id] + 1 <= sanpham.soluonghienco)
                {
                    gioHang[id] = gioHang[id] + 1;
                }
                else
                {
                    TempData["LoiGioHang"] = "Không thể tăng thêm vì chỉ còn " + sanpham.soluonghienco + " sản phẩm.";
                }
            }
            else if (hanhdong == "bot")
            {
                gioHang[id] = gioHang[id] - 1;

                if (gioHang[id] <= 0)
                {
                    gioHang.Remove(id);
                }
            }

            return RedirectToAction("Index");
        }
    }
}