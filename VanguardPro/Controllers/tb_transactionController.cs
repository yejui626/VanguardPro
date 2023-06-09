﻿using System;
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
    public class tb_transactionController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        // GET: tb_transaction
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var tb_transaction = db.tb_transaction.Include(t => t.tb_floor);
            return View(tb_transaction.ToList());
        }

        // GET: tb_transaction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_transaction tb_transaction = db.tb_transaction.Find(id);
            if (tb_transaction == null)
            {
                return HttpNotFound();
            }
            return View(tb_transaction);
        }

        // GET: tb_transaction/Create
        public ActionResult Create(HttpPostedFileBase file)
        {
            //add null to select list
            var floorList = db.tb_floor.ToList();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = null, Text = "General" }
            };
            selectList.AddRange(floorList.Select(f => new SelectListItem { Value = f.f_id.ToString(), Text = f.f_desc }));
            ViewBag.tr_fid = new SelectList(selectList, "Value", "Text");
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Images"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
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

        // POST: tb_transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tr_id,tr_fid,tr_desc,tr_type,tr_paymentMethod,tr_date,tr_receipt,tr_amount")] tb_transaction tb_transaction, HttpPostedFileBase receipt)
        {
            if (ModelState.IsValid)
            {
                if (receipt != null && receipt.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(receipt.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    receipt.SaveAs(filePath);
                    tb_transaction.tr_receipt = fileName; // Save the unique file name in the database
                }

                db.tb_transaction.Add(tb_transaction);
                db.SaveChanges();
                CalculateCurrentMonthProfit();
                return RedirectToAction("Index");
            }

            ViewBag.tr_fid = new SelectList(db.tb_floor, "f_id", "f_desc", tb_transaction.tr_fid);
            return View(tb_transaction);
        }

        // GET: tb_transaction/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_transaction tb_transaction = db.tb_transaction.Find(id);
            if (tb_transaction == null)
            {
                return HttpNotFound();
            }
            //add null to select list
            var floorList = db.tb_floor.ToList();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = null, Text = "General" }
            };
            selectList.AddRange(floorList.Select(f => new SelectListItem { Value = f.f_id.ToString(), Text = f.f_desc }));
            ViewBag.tr_fid = new SelectList(selectList, "Value", "Text");
            return View(tb_transaction);
        }

        // POST: tb_transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "tr_id,tr_fid,tr_desc,tr_type,tr_paymentMethod,tr_date,tr_receipt,tr_amount")] tb_transaction tb_transaction, HttpPostedFileBase receipt)
        {
            if (ModelState.IsValid)
            {
                if (receipt != null && receipt.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(receipt.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/admin/vendor/images"), fileName);
                    receipt.SaveAs(filePath);
                    tb_transaction.tr_receipt = fileName; // Save the unique file name in the database
                }
                db.Entry(tb_transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.tr_fid = new SelectList(db.tb_floor, "f_id", "f_desc", tb_transaction.tr_fid);
            return View(tb_transaction);
        }

        // GET: tb_transaction/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_transaction tb_transaction = db.tb_transaction.Find(id);
            if (tb_transaction == null)
            {
                return HttpNotFound();
            }
            return View(tb_transaction);
        }

        // POST: tb_transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_transaction tb_transaction = db.tb_transaction.Find(id);
            db.tb_transaction.Remove(tb_transaction);
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

        public void CalculateCurrentMonthProfit()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Retrieve the investors from the tb_investor table
            var investors = db.tb_investors.ToList();

            var financeData = db.tb_transaction
                .Where(tr => tr.tr_date.Month == currentMonth && tr.tr_date.Year == currentYear && tr.tr_fid != null)
                .GroupBy(tr => new { tr.tr_fid})
                .Select(g => new
                {
                    Profit = g.Sum(tr => tr.tr_amount * (tr.tr_type == "Inflow" ? 1 : -1))
                })
                .ToList();

            // Calculate the total profit for the current month
            var profit = financeData.Sum(t => t.Profit);
            var partner = 3;
            var lot = db.tb_investors.Select(x => x.i_lot).Distinct().Count();
            profit = (profit * 6 / 10) / (partner + lot);
            var p_date = DateTime.Now;
            int i = 0;

            foreach (var investor in investors)
            {
                var sharing = profit * (decimal)(investor.i_lotperc / 100);

                // Check if a profit entry already exists for the current month and investor
                var existingProfit = db.tb_profit.FirstOrDefault(p => p.p_investorid == investor.i_id && p.p_date.Month == currentMonth && p.p_date.Year == currentYear);

                if (existingProfit != null)
                {
                    // Update the existing profit entry
                    existingProfit.p_profit = (double)sharing;
                    db.Entry(existingProfit).State = EntityState.Modified;

                    var user = db.tb_user.Find(investor.i_uid);
                }
                else
                {
                    // Create a new profit entry
                    var profitEntry = new tb_profit
                    {
                        p_investorid = investor.i_id,
                        p_date = DateTime.Now,
                        p_profit = (double)sharing
                    };

                    // Add the profit entry to the table
                    db.tb_profit.Add(profitEntry);
                }
            }

            // Save the changes to the database
            db.SaveChanges();
        }
        //END OF CalculateCurrentMonthProfit



    }
}
