using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VanguardPro.Models;

namespace VanguardPro.Controllers
{
    public class tb_inventoryController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_inventory
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var tb_inventory = db.tb_inventory.Include(t => t.tb_floor);
            return View(tb_inventory.ToList());
        }

        // GET: tb_inventory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            if (tb_inventory == null)
            {
                return HttpNotFound();
            }
            return View(tb_inventory);
        }

        // GET: tb_inventory/Create
        public ActionResult Create()
        {
            ViewBag.ivtry_fid = new SelectList(db.tb_floor, "f_id", "f_building");
            return View();
        }

        // POST: tb_inventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "invtry_id, ivtry_fid,ivtry_item,ivtry_count")] tb_inventory tb_inventory, List<string> IvtryItems, List<int> IvtryCounts)
        {
            if (ModelState.IsValid)
            {
                using (var db = new db_vanguardproEntities())
                {
                    db.tb_inventory.Add(tb_inventory);
                    db.SaveChanges();
                    if (IvtryItems != null && IvtryCounts != null)
                    {
                        for (int i = 0; i < IvtryItems.Count; i++)
                        {
                            var additionalItem = new tb_inventory
                            {
                                ivtry_id = tb_inventory.ivtry_id, // Set the ivtry_id of the main item here
                                ivtry_item = IvtryItems[i],
                                ivtry_count = IvtryCounts[i]
                            };
                            db.tb_inventory.Add(additionalItem);
                            db.SaveChanges();
                        }
                        
                    }
                    return RedirectToAction("Index");
                }
            }
            ViewBag.ivtry_fid = new SelectList(db.tb_floor, "f_id", "f_building", tb_inventory.ivtry_fid);
            return View(tb_inventory);
        }

















        // GET: tb_inventory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            if (tb_inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ivtry_fid = new SelectList(db.tb_floor, "f_id", "f_building", tb_inventory.ivtry_fid);
            return View(tb_inventory);
        }

        // POST: tb_inventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ivtry_id,ivtry_fid,ivtry_item,ivtry_count")] tb_inventory tb_inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ivtry_fid = new SelectList(db.tb_floor, "f_id", "f_building", tb_inventory.ivtry_fid);
            return View(tb_inventory);
        }

        // GET: tb_inventory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            if (tb_inventory == null)
            {
                return HttpNotFound();
            }
            return View(tb_inventory);
        }

        // POST: tb_inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            db.tb_inventory.Remove(tb_inventory);
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
