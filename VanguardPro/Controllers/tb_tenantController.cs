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
    public class tb_tenantController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_tenant
        public ActionResult Index(string Status)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var tb_rental = db.tb_rental.Include(r => r.tb_tenant).Include(r => r.tb_room).ToList();

            if (!string.IsNullOrEmpty(Status) && Status != "Select Status")
            {
                if (Status == "Active")
                {
                    tb_rental = tb_rental.Where(r => IsTenantActive(r)).ToList();
                }
                else if (Status == "Inactive")
                {
                    tb_rental = tb_rental.Where(r => !IsTenantActive(r)).ToList();
                }
            }
            else
            {
                tb_rental = db.tb_rental.ToList();
            }

            return View(tb_rental);
        }

        private bool IsTenantActive(tb_rental tb_rental)
        {
            DateTime currentDate = DateTime.Now.Date;

            if (tb_rental.re_outdate != null && tb_rental.re_outsession != null)
            {
                if (currentDate > tb_rental.re_outdate.Value.Date)
                {
                    return false;
                }
                else if (currentDate == tb_rental.re_outdate.Value.Date)
                {
                    if (tb_rental.re_outsession == "Morning" && DateTime.Now.TimeOfDay > TimeSpan.Parse("12:00:00"))
                    {
                        return false;
                    }
                    else if (tb_rental.re_outsession == "Afternoon" && DateTime.Now.TimeOfDay > TimeSpan.Parse("18:00:00"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
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
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.re_paymentStatus = new SelectList(new[]
            {
                new { Value = "Paid", Text = "Paid" },
                new { Value = "Unpaid", Text = "Unpaid" },
                new { Value = "Partially Paid", Text = "Partially Paid" }
            }, "Value", "Text");
            ViewBag.re_rid = new SelectList(db.tb_room.Where(r => r.r_availability == "Available"), "r_id", "r_roomNo", null);
            var floorList = db.tb_floor.ToList();
            var selectList = new List<SelectListItem>
            {
            };
            selectList.AddRange(floorList.Select(f => new SelectListItem { Value = f.f_id.ToString(), Text = f.f_desc }));
            ViewBag.tr_fid = new SelectList(selectList, "Value", "Text");

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
        public ActionResult Create(tb_rental tb_rental, HttpPostedFileBase tenantIC, HttpPostedFileBase tenantContract)
        {
            if (ModelState.IsValid)
            {
                if (tenantIC != null && tenantIC.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(tenantIC.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    tenantIC.SaveAs(filePath);
                    tb_rental.tb_tenant.t_uploadic = fileName; // Save the unique file name in the database
                }
                if (tenantContract != null && tenantContract.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(tenantContract.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    tenantContract.SaveAs(filePath);
                    tb_rental.tb_tenant.t_contract = fileName; // Save the unique file name in the database
                }

                var selectedRoom = db.tb_room.FirstOrDefault(r => r.r_id == tb_rental.re_rid);
                if (selectedRoom != null)
                {
                    selectedRoom.r_availability = "Occupied";
                    tb_rental.re_rentPrice = selectedRoom.r_price;
                }
                tb_rental.re_paymentStatus = "Unpaid";
                db.tb_rental.Add(tb_rental);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tb_rental);
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
        public ActionResult Edit([Bind(Include = "t_id,t_name,t_ic,t_uploadic,t_contract,t_phone,t_emergcont,t_siriNo,t_accessCardNo")] tb_tenant tb_tenant, HttpPostedFileBase tenantIC, HttpPostedFileBase tenantContract)
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
                if (tenantContract != null && tenantContract.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(tenantContract.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    tenantContract.SaveAs(filePath);
                    tb_tenant.t_contract = fileName; // Save the unique file name in the database
                }
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
