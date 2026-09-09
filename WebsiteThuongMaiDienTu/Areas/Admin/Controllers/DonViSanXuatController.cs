using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuongMaiDienTu.Models;

namespace WebsiteThuongMaiDienTu.Areas.Admin.Controllers
{
    public class DonViSanXuatController : BaseAdminController
    {
        private QLBanHang_Model db = new QLBanHang_Model();
        // GET: Admin/DonViSanXuat
        public ActionResult Index()
        {
            var ds_dvsx = db.donvisanxuats.ToList();
            return View(ds_dvsx);
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TaoMoi(FormCollection collection)
        {
            donvisanxuat dvsx = new donvisanxuat();

            dvsx.madonvisanxuat = collection["txt_ma"];
            dvsx.tendonvisanxuat = collection["txt_ten"];

            db.donvisanxuats.Add(dvsx);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Xoa(string id)
        {
            var xoa_dvsx = db.donvisanxuats.Find(id);

            if (xoa_dvsx != null)
            {
                bool conSanPham = db.sanphams
                    .Any(sp => sp.madonvisanxuat == id);

                if (!conSanPham)
                {
                    db.donvisanxuats.Remove(xoa_dvsx);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sua(string id)
        {
            var dvsx = db.donvisanxuats.Find(id);

            if (dvsx == null)
            {
                return HttpNotFound();
            }

            return View(dvsx);
        }
        [HttpPost]
        public ActionResult Sua(FormCollection collection)
        {
            var sua_dvsx = db.donvisanxuats
                .Find(collection["txt_ma"]);

            if (sua_dvsx != null)
            {
                sua_dvsx.tendonvisanxuat = collection["txt_ten"];

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}