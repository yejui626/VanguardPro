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
    public class tb_investorsController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_investors
        public ActionResult Index()
        {
            var tb_investors = db.tb_investors.Include(t => t.tb_profit).Include(t => t.tb_user);
            return View(tb_investors.ToList());
        }

        // GET: tb_investors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investors tb_investors = db.tb_investors.Find(id);
            if (tb_investors == null)
            {
                return HttpNotFound();
            }
            return View(tb_investors);
        }

        // GET: tb_investors/Create
        public ActionResult Create()
        {
            ViewBag.i_pid = new SelectList(db.tb_profit, "p_id", "p_id");
            ViewBag.i_id = new SelectList(db.tb_user, "u_id", "u_username");
            return View();
        }

        // POST: tb_investors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "i_id,i_uid,i_pid,i_lot,i_lotperc")] tb_investors tb_investors)
        {
            if (ModelState.IsValid)
            {
                db.tb_investors.Add(tb_investors);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.i_pid = new SelectList(db.tb_profit, "p_id", "p_id", tb_investors.i_pid);
            ViewBag.i_id = new SelectList(db.tb_user, "u_id", "u_username", tb_investors.i_id);
            return View(tb_investors);
        }

        // GET: tb_investors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investors tb_investors = db.tb_investors.Find(id);
            if (tb_investors == null)
            {
                return HttpNotFound();
            }
            ViewBag.i_pid = new SelectList(db.tb_profit, "p_id", "p_id", tb_investors.i_pid);
            ViewBag.i_id = new SelectList(db.tb_user, "u_id", "u_username", tb_investors.i_id);
            return View(tb_investors);
        }

        // POST: tb_investors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "i_id,i_uid,i_pid,i_lot,i_lotperc")] tb_investors tb_investors)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_investors).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.i_pid = new SelectList(db.tb_profit, "p_id", "p_id", tb_investors.i_pid);
            ViewBag.i_id = new SelectList(db.tb_user, "u_id", "u_username", tb_investors.i_id);
            return View(tb_investors);
        }

        // GET: tb_investors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investors tb_investors = db.tb_investors.Find(id);
            if (tb_investors == null)
            {
                return HttpNotFound();
            }
            return View(tb_investors);
        }

        // POST: tb_investors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_investors tb_investors = db.tb_investors.Find(id);
            db.tb_investors.Remove(tb_investors);
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
