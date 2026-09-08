using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class LoaiSanPhamController : BaseAdminController
    {
        QLBanHang_Model db = new QLBanHang_Model();

        // GET: Admin/LoaiSanPham
        public ActionResult Index()
        {
            var ds_lsp = db.loaisanphams.ToList();
            return View(ds_lsp);
        }

        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            loaisanpham lsp = new loaisanpham();

            lsp.maloaisanpham = collection["txt_ma"];
            lsp.tenloaisanpham = collection["txt_ten"];

            db.loaisanphams.Add(lsp);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Xoa(string id)
        {
            var xoa_lsp = db.loaisanphams.Find(id);

            if (xoa_lsp != null)
            {
                bool conSanPham = db.sanphams
                    .Any(sp => sp.maloaisanpham == id);

                if (!conSanPham)
                {
                    db.loaisanphams.Remove(xoa_lsp);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sua(string id)
        {
            var lsp = db.loaisanphams.Find(id);

            if (lsp == null)
            {
                return HttpNotFound();
            }

            return View(lsp);
        }

        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            var sua_lsp = db.loaisanphams
                .Find(collection["txt_ma"]);

            if (sua_lsp != null)
            {
                sua_lsp.tenloaisanpham = collection["txt_ten"];

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
