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
    public class tb_roomController : Controller
    {

        private db_vanguardproEntities db = new db_vanguardproEntities();

        public ActionResult GetRoomsByFloor(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            var emptyRooms = db.tb_room.Where(r => r.r_fid == id && r.r_availability == "Available").ToList();
            var roomList = emptyRooms.Select(r => new
            {
                value = r.r_id,
                text = r.r_roomNo
            });

            return Json(roomList, JsonRequestBehavior.AllowGet);
        }
        // GET: tb_room
        public ActionResult Index()
        {
            var tb_room = db.tb_room.Include(t => t.tb_floor);
            return View(tb_room.ToList());
        }

        // GET: tb_room/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_room tb_room = db.tb_room.Find(id);
            if (tb_room == null)
            {
                return HttpNotFound();
            }
            return View(tb_room);
        }

        // GET: tb_room/Create
        public ActionResult Create()
        {
            ViewBag.r_fid = new SelectList(db.tb_floor, "f_id", "f_desc");
            return View();
        }

        // POST: tb_room/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "r_id,r_fid,r_price,r_availability,r_roomNo")] tb_room tb_room)
        {
            if (ModelState.IsValid)
            {
                db.tb_room.Add(tb_room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.r_fid = new SelectList(db.tb_floor, "f_id", "f_desc", tb_room.r_fid);
            return View(tb_room);
        }

        // GET: tb_room/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_room tb_room = db.tb_room.Find(id);
            if (tb_room == null)
            {
                return HttpNotFound();
            }
            ViewBag.r_fid = new SelectList(db.tb_floor, "f_id", "f_desc", tb_room.r_fid);
            return View(tb_room);
        }

        // POST: tb_room/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "r_id,r_fid,r_price,r_availability,r_roomNo")] tb_room tb_room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.r_fid = new SelectList(db.tb_floor, "f_id", "f_desc", tb_room.r_fid);
            return View(tb_room);
        }

        // GET: tb_room/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_room tb_room = db.tb_room.Find(id);
            if (tb_room == null)
            {
                return HttpNotFound();
            }
            return View(tb_room);
        }

        // POST: tb_room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_room tb_room = db.tb_room.Find(id);
            db.tb_room.Remove(tb_room);
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
