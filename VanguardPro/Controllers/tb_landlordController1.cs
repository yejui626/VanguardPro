using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VanguardPro.Models;

namespace VanguardPro.Controllers
{
    public class tb_landlordController1 : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_landlord
        public ActionResult Index()
        {
            return View(db.tb_landlord.ToList());
        }

        // GET: tb_landlord/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            if (tb_landlord == null)
            {
                return HttpNotFound();
            }
            return View(tb_landlord);
        }

        // GET: tb_landlord/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tb_landlord/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "l_id,l_name,l_phone,l_due")] tb_landlord tb_landlord)
        {
            if (ModelState.IsValid)
            {
                db.tb_landlord.Add(tb_landlord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_landlord);
        }

        // GET: tb_landlord/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            if (tb_landlord == null)
            {
                return HttpNotFound();
            }
            return View(tb_landlord);
        }

        // POST: tb_landlord/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "l_id,l_name,l_phone,l_due")] tb_landlord tb_landlord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_landlord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_landlord);
        }

        // GET: tb_landlord/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            if (tb_landlord == null)
            {
                return HttpNotFound();
            }
            return View(tb_landlord);
        }

        // POST: tb_landlord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            db.tb_landlord.Remove(tb_landlord);
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
