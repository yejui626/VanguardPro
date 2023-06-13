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
    public class tb_attendanceController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_attendance
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var tb_attendance = db.tb_attendance.Include(t => t.tb_floor);
            return View(tb_attendance.ToList());
        }

        // GET: tb_attendance/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            if (tb_attendance == null)
            {
                return HttpNotFound();
            }
            return View(tb_attendance);
        }

        // GET: tb_attendance/Create
        public ActionResult Create()
        {
            ViewBag.atd_fid = new SelectList(db.tb_floor, "f_id", "f_building");
            return View();
        }

        // POST: tb_attendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "atd_id,atd_fid,atd_check")] tb_attendance tb_attendance)
        {
            if (ModelState.IsValid)
            {
                db.tb_attendance.Add(tb_attendance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.atd_fid = new SelectList(db.tb_floor, "f_id", "f_building", tb_attendance.atd_fid);
            return View(tb_attendance);
        }

        // GET: tb_attendance/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            if (tb_attendance == null)
            {
                return HttpNotFound();
            }
            ViewBag.atd_fid = new SelectList(db.tb_floor, "f_id", "f_building", tb_attendance.atd_fid);
            return View(tb_attendance);
        }

        // POST: tb_attendance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "atd_id,atd_fid,atd_check")] tb_attendance tb_attendance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_attendance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.atd_fid = new SelectList(db.tb_floor, "f_id", "f_building", tb_attendance.atd_fid);
            return View(tb_attendance);
        }

        // GET: tb_attendance/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            if (tb_attendance == null)
            {
                return HttpNotFound();
            }
            return View(tb_attendance);
        }

        // POST: tb_attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            db.tb_attendance.Remove(tb_attendance);
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
