using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VanguardPro.Models;

namespace VanguardPro.Controllers
{
    public class tb_tenantController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_tenant
        public ActionResult Index()
        {
            return View(db.tb_tenant.ToList());
        }

        // GET: tb_tenant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            if (tb_tenant == null)
            {
                return HttpNotFound();
            }
            return View(tb_tenant);
        }

        // GET: tb_tenant/Create
        public ActionResult Create()
        {
            return View();
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

        // POST: tb_tenant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "t_id,t_name,t_ic,t_uploadic,t_contract,t_phone,t_emergcont,t_siriNo,t_paymentStatus")] tb_tenant tb_tenant, HttpPostedFileBase tenantIC)
        {
            if (ModelState.IsValid)
            {
                if (tenantIC != null && tenantIC.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(tenantIC.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    tenantIC.SaveAs(filePath);
                    tb_tenant.t_uploadic = fileName; // Save the unique file name in the database
                }

                db.tb_tenant.Add(tb_tenant);
                db.SaveChanges();

                /*
                var room = db.rooms.FirstOrDefault(r => r.room_id == tenant.room_id);
                if (room != null)
                {
                    room.room_status = "Booked";
                    db.SaveChanges();
                }
                */
                return RedirectToAction("Index");
            }

            return View(tb_tenant);
        }

        // GET: tb_tenant/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            if (tb_tenant == null)
            {
                return HttpNotFound();
            }
            return View(tb_tenant);
        }

        // POST: tb_tenant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "t_id,t_name,t_ic,t_uploadic,t_contract,t_phone,t_emergcont,t_siriNo,t_paymentStatus")] tb_tenant tb_tenant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_tenant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_tenant);
        }

        // GET: tb_tenant/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            if (tb_tenant == null)
            {
                return HttpNotFound();
            }
            return View(tb_tenant);
        }

        // POST: tb_tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            db.tb_tenant.Remove(tb_tenant);
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
