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
    public class tb_profitController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_profit
        public ActionResult Index()
        {
            if (Session["UserID"] != null && Session["UserType"] == "2")
            {
                var userId = Convert.ToInt32(Session["UserID"]);
                var tb_investor = db.tb_investors.FirstOrDefault(i => i.i_uid == userId);

                if (tb_investor != null)
                {
                    int i_id = tb_investor.i_id;
                    var currentDate = DateTime.Now;
                    var currentMonth = DateTime.Now.Month;

                    var tb_profit = db.tb_profit
                        .Where(p => p.p_investorid == i_id && !p.p_date.Month.Equals(currentMonth))
                        .ToList();

                    foreach (var profitEntry in tb_profit)
                    {
                        // Retrieve investor name from tb_user
                        var investor = db.tb_user.FirstOrDefault(u => u.u_id == tb_investor.i_uid);
                        if (investor != null)
                            profitEntry.tb_investors.tb_user.u_username = investor.u_username;
                    }

                    tb_profit = tb_profit
                        .OrderByDescending(p => p.p_date.Year) // Order by year
                        .ThenByDescending(p => p.p_date.Month) // Order by month
                        .ToList();

                    return View(tb_profit);
                }
            }

            var profit = db.tb_profit
                .Include(t => t.tb_investors)
                .ToList()
                .Select(p => new tb_profit
                {
                    p_id = p.p_id,
                    p_investorid = p.p_investorid,
                    p_date = p.p_date,
                    p_profit = p.p_profit,
                })
                .OrderByDescending(p => p.p_date.Year) // Order by year
                .ThenByDescending(p => p.p_date.Month) // Order by month
                .ToList();

            return View(profit);
        }

        // GET: tb_profit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_profit tb_profit = db.tb_profit.Find(id);
            if (tb_profit == null)
            {
                return HttpNotFound();
            }
            return View(tb_profit);
        }

        // GET: tb_profit/Create
        public ActionResult Create()
        {
            ViewBag.p_investorid = new SelectList(db.tb_investors, "i_id", "i_id");
            return View();
        }

        // POST: tb_profit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "p_id,p_date,p_profit,p_investorid")] tb_profit tb_profit)
        {
            if (ModelState.IsValid)
            {
                db.tb_profit.Add(tb_profit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.p_investorid = new SelectList(db.tb_investors, "i_id", "i_id", tb_profit.p_investorid);
            return View(tb_profit);
        }

        // GET: tb_profit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_profit tb_profit = db.tb_profit.Find(id);
            if (tb_profit == null)
            {
                return HttpNotFound();
            }
            ViewBag.p_investorid = new SelectList(db.tb_investors, "i_id", "i_id", tb_profit.p_investorid);
            return View(tb_profit);
        }

        // POST: tb_profit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "p_id,p_date,p_profit,p_investorid")] tb_profit tb_profit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_profit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.p_investorid = new SelectList(db.tb_investors, "i_id", "i_id", tb_profit.p_investorid);
            return View(tb_profit);
        }

        // GET: tb_profit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_profit tb_profit = db.tb_profit.Find(id);
            if (tb_profit == null)
            {
                return HttpNotFound();
            }
            return View(tb_profit);
        }

        // POST: tb_profit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_profit tb_profit = db.tb_profit.Find(id);
            db.tb_profit.Remove(tb_profit);
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
