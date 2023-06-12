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

        public ActionResult UpdatePayment(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_rental tb_rental = db.tb_rental.Find(id);
            if (tb_rental == null)
            {
                return HttpNotFound();
            }
            if (tb_rental.re_outstanding == null)
            {
                tb_rental.re_outstanding = 0;
            }
            ViewBag.outstanding = tb_rental.re_outstanding;

            return PartialView("_UpdatePayment", tb_rental);
        }
        [HttpPost]
        public ActionResult UpdatePayment(tb_rental tb_rental, double paymentAmount)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the existing payment details from the database
                var existingPayment = db.tb_rental.Find(tb_rental.re_id);

                if (existingPayment != null)
                {
                    if (existingPayment.re_outstanding == paymentAmount)
                    {
                        existingPayment.re_outstanding = 0;
                        existingPayment.re_paymentStatus = "Paid";
                    }
                    else if (paymentAmount == 0)
                    {
                        existingPayment.re_paymentStatus = "Unpaid";
                    }
                    else
                    {
                        existingPayment.re_outstanding = existingPayment.re_outstanding - paymentAmount;
                        existingPayment.re_paymentStatus = "Partially Paid";
                    }

                    tb_rental.re_paymentStatus = existingPayment.re_paymentStatus;
                    existingPayment.re_payDate = tb_rental.re_payDate;

                    db.SaveChanges();

                    return RedirectToAction("Index", "tb_tenant", new { id = tb_rental.re_id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment not found.";
                }
            }
            return View(tb_rental);
        }


        // GET: tb_tenant/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_rental tb_rental = db.tb_rental.Find(id);
            if (tb_rental == null)
            {
                return HttpNotFound();
            }
            return View(tb_rental);
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
                    tb_rental.re_outstanding = selectedRoom.r_price;
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
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_rental tb_rental = db.tb_rental.Find(id);
            if (tb_rental == null)
            {
                return HttpNotFound();
            }

            bool isActive = IsTenantActive(tb_rental);
            var currentRooms = db.tb_room.Where(r => r.r_id == tb_rental.re_rid).ToList();
            ViewBag.currentFloor = tb_rental.tb_room.tb_floor.f_id;
            var roomList = currentRooms.Select(r => new
            {
                value = r.r_id,
                text = r.r_roomNo
            });
            if (isActive)
            {
                ViewBag.roomList = roomList;
            }
            else
            {
                ViewBag.roomList = null;
            }

            ViewBag.re_paymentStatus = new SelectList(new[]
{
                new { Value = "Paid", Text = "Paid" },
                new { Value = "Unpaid", Text = "Unpaid" },
                new { Value = "Partially Paid", Text = "Partially Paid" }
            }, "Value", "Text");
            ViewBag.re_rid = new SelectList(db.tb_room.Where(r => r.r_availability == "Available"), "r_id", "r_roomNo", null);
            ViewBag.floor = new SelectList(db.tb_floor, "f_id", "f_desc", tb_rental.tb_room.tb_floor.f_id);

            return View(tb_rental);
        }

        // POST: tb_tenant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tb_rental tb_rental, HttpPostedFileBase tenantIC, HttpPostedFileBase tenantContract)
        {
            if (ModelState.IsValid)
            {
                tb_rental _rental = db.tb_rental.Single(x => x.re_id == tb_rental.re_id);
                if (tenantIC != null && tenantIC.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(tenantIC.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    tenantIC.SaveAs(filePath);
                    _rental.tb_tenant.t_uploadic = fileName; // Save the unique file name in the database
                }
                if (tenantContract != null && tenantContract.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(tenantContract.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    tenantContract.SaveAs(filePath);
                    _rental.tb_tenant.t_contract = fileName; // Save the unique file name in the database
                }

                var existingRoom = db.tb_room.FirstOrDefault(r => r.r_id == _rental.re_rid);
                var selectedRoom = db.tb_room.FirstOrDefault(r => r.r_id == tb_rental.re_rid);

                if (selectedRoom.r_id != existingRoom.r_id)
                {
                    existingRoom.r_availability = "Available";
                    selectedRoom.r_availability = "Occupied";
                    tb_rental.re_rentPrice = selectedRoom.r_price;
                }
                else
                {
                    existingRoom.r_availability = "Occupied";
                }

                _rental.tb_tenant.t_name = tb_rental.tb_tenant.t_name;
                _rental.tb_tenant.t_ic = tb_rental.tb_tenant.t_ic;
                _rental.tb_tenant.t_phone = tb_rental.tb_tenant.t_phone;
                _rental.tb_tenant.t_emergcont = tb_rental.tb_tenant.t_emergcont;
                _rental.re_indate = tb_rental.re_indate;
                _rental.re_outdate = tb_rental.re_outdate;
                _rental.re_outsession = tb_rental.re_outsession;
                _rental.re_rentPrice = tb_rental.re_rentPrice;
                _rental.re_deposit = tb_rental.re_deposit;
                _rental.re_rid = tb_rental.re_rid;
                _rental.tb_tenant.t_accessCardNo = tb_rental.tb_tenant.t_accessCardNo;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            tb_rental rental = db.tb_rental.Find(tb_rental.re_id);
            bool isActive = IsTenantActive(rental);
            var currentRooms = db.tb_room.Where(r => r.r_id == rental.re_rid).ToList();
            ViewBag.currentFloor = rental.tb_room.tb_floor.f_id;
            var roomList = currentRooms.Select(r => new
            {
                value = r.r_id,
                text = r.r_roomNo
            });

            if (isActive)
            {
                ViewBag.roomList = roomList;
            }
            else
            {
                ViewBag.roomList = null;
            }

            ViewBag.currentFloor = rental.tb_room.tb_floor.f_id;
            ViewBag.floor = new SelectList(db.tb_floor, "f_id", "f_desc", ViewBag.currentFloor);
            ViewBag.re_rid = new SelectList(db.tb_room, "r_id", "r_roomNo", tb_rental.re_rid);
            return View(tb_rental);
        }

        // GET: tb_tenant/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_rental tb_rental = db.tb_rental.Find(id);
            if (tb_rental == null)
            {
                return HttpNotFound();
            }
            return View(tb_rental);
        }

        // POST: tb_tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_rental tb_rental = db.tb_rental.Find(id);
            tb_tenant tb_tenant = db.tb_tenant.Find(tb_rental.re_tid);
            var selectedRoom = db.tb_room.FirstOrDefault(r => r.r_id == tb_rental.re_rid);
            if (selectedRoom != null)
            {
                selectedRoom.r_availability = "Available";
            }
            if (tb_rental.tb_tenant.t_uploadic != null)
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), tb_rental.tb_tenant.t_uploadic));
            }
            if (tb_rental.tb_tenant.t_contract != null)
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), tb_rental.tb_tenant.t_contract));
            }
            db.tb_rental.Remove(tb_rental);
            db.tb_tenant.Remove(tb_tenant);
            db.SaveChanges();
            return RedirectToAction("Index");
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
