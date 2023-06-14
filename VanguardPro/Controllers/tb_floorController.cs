using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VanguardPro.Models;

namespace VanguardPro.Controllers
{
    public class tb_floorController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_floor
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var tb_floor = db.tb_floor.Include(t => t.tb_landlord).Include(t => t.tb_user);
            return View(tb_floor.ToList());
        }

        // GET: tb_floor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_floor tb_floor = db.tb_floor.Find(id);
            if (tb_floor == null)
            {
                return HttpNotFound();
            }
            return View(tb_floor);
        }

        // GET: tb_floor/Create
        public ActionResult Create()
        {
            ViewBag.f_lid = new SelectList(db.tb_landlord, "l_id", "l_name");
            ViewBag.f_uid = new SelectList(db.tb_user, "u_id", "u_username");
            return View();
        }

        // POST: tb_floor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "f_id,f_lid,f_desc,f_building,f_wifipwd,f_modemIP,f_cctvqr,f_layout,f_uid")] tb_floor tb_floor, HttpPostedFileBase layout, HttpPostedFileBase qr)
        {
            if (ModelState.IsValid)
            {
                if (layout != null && layout.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(layout.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    layout.SaveAs(filePath);
                    tb_floor.f_layout = fileName; // Save the unique file name in the database
                }
                if (qr != null && qr.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(qr.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    qr.SaveAs(filePath);
                    tb_floor.f_cctvqr = fileName; // Save the unique file name in the database
                }
                db.tb_floor.Add(tb_floor);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.f_lid = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.f_lid);
            ViewBag.f_uid = new SelectList(db.tb_user, "u_id", "u_username", tb_floor.f_uid);
            return View(tb_floor);
        }

        public ActionResult GetFile(string floorLayoutFileName)
        {
            string filePath = Server.MapPath("~/Content/admin/vendor/images/" + floorLayoutFileName);
            if (System.IO.File.Exists(filePath))
            {
                return File(filePath, "image/png"); // Adjust the content type according to the actual file type
            }
            else
            {
                return HttpNotFound();
            }
        }

        // GET: tb_floor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_floor tb_floor = db.tb_floor.Find(id);
            if (tb_floor == null)
            {
                return HttpNotFound();
            }
            ViewBag.f_lid = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.f_lid);
            ViewBag.f_uid = new SelectList(db.tb_user, "u_id", "u_username", tb_floor.f_uid);
            return View(tb_floor);
        }

        // POST: tb_floor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "f_id,f_lid,f_building,f_wifipwd,f_modemIP,f_cctvqr,f_layout,f_uid")] tb_floor tb_floor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_floor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.f_lid = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.f_lid);
            ViewBag.f_uid = new SelectList(db.tb_user, "u_id", "u_username", tb_floor.f_uid);
            return View(tb_floor);
        }

        // GET: tb_floor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_floor tb_floor = db.tb_floor.Find(id);
            if (tb_floor == null)
            {
                return HttpNotFound();
            }
            return View(tb_floor);
        }

        // POST: tb_floor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_floor tb_floor = db.tb_floor.Find(id);
            db.tb_floor.Remove(tb_floor);
            db.SaveChanges();
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
