using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanguardPro.Models;
using System.Data;

namespace VanguardPro.Controllers
{
    public class transactionSummaryController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();

        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var floorList = db.tb_floor.ToList();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "-- All --" },
                new SelectListItem { Value = null, Text = "General" }
            };

            selectList.AddRange(floorList.Select(f => new SelectListItem { Value = f.f_id.ToString(), Text = f.f_desc }));
            ViewBag.trs_floor = new SelectList(selectList, "Value", "Text");

            var paymentList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "-- All --" },
                new SelectListItem { Value = "Bank", Text = "Bank" },
                new SelectListItem { Value = "Cash", Text = "Cash" }
            };
            ViewBag.trs_paymentMethod = new SelectList(paymentList, "Value", "Text");



            var typeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "-- All --" },
                new SelectListItem { Value = "Inflow", Text = "Inflow" },
                new SelectListItem { Value = "Outflow", Text = "Outflow" }
            };
            ViewBag.trs_type = new SelectList(typeList, "Value", "Text");



            var query = from tr in db.tb_transaction
                        join fl in db.tb_floor on tr.tr_fid equals fl.f_id into floorGroup
                        from fl in floorGroup.DefaultIfEmpty()
                        select tr;
            var queryList = query.ToList();
            var tempSummary = new List<transactionSummary>();
            foreach (var tr in queryList)
            {
                var sum = new transactionSummary
                {
                    Year = tr.tr_date.Year,
                    Month = tr.tr_date.Month,
                    Day = tr.tr_date.Day,
                    PaymentMethod = tr.tr_paymentMethod,
                    Desc = tr.tr_desc,
                    Inflow = tr.tr_type == "Inflow" ? (decimal)tr.tr_amount : 0,
                    Outflow = tr.tr_type == "Outflow" ? (decimal)tr.tr_amount : 0,
                    Floor = tr.tr_fid != null ? tr.tb_floor.f_desc : "General"
                };
                tempSummary.Add(sum);
            }

            var groupedQuery = tempSummary.OrderBy(x => x.Year)
                  .ThenBy(x => x.Month)
                  .ThenBy(x => x.Day)
                  .ToList();

            foreach (var item in groupedQuery)
            {
                var monthRecords = groupedQuery.Where(x => x.Year == item.Year && x.Month == item.Month).ToList();
                item.TotalIn = monthRecords.Sum(x => x.Inflow);
                item.TotalOut = monthRecords.Sum(x => x.Outflow);
                item.Difference = item.TotalIn - item.TotalOut;
            }

            var result = new List<transactionSummary>();
            int currentYear = 0;
            int currentMonth = 0;
            int rowSpanYear = 0;
            int rowSpanMonth = 0;

            foreach (var item in groupedQuery)
            {
                if (item.Year != currentYear)
                {
                    currentYear = item.Year;
                    rowSpanYear = groupedQuery.Count(x => x.Year == currentYear);
                }

                if (item.Month != currentMonth)
                {
                    currentMonth = item.Month;
                    rowSpanMonth = groupedQuery.Count(x => x.Year == currentYear && x.Month == currentMonth);
                }

                item.RowSpanYear = rowSpanYear;
                item.RowSpanMonth = rowSpanMonth;

                result.Add(item);
            }
            return View(result);
        }


        public ActionResult summaryFilter(string startDate, string endDate, int? floor, string payment, string type)
        {
            //select and add to model exSummary
            var query = from tr in db.tb_transaction
                        join fl in db.tb_floor on tr.tr_fid equals fl.f_id into floorGroup
                        from fl in floorGroup.DefaultIfEmpty()
                        select tr;
            var queryList = query.ToList();
            // apply filter
            if (floor != 0)
            {
                queryList = queryList.Where(item => item.tr_fid == floor).ToList();
            }

            if (payment != "0")
            {
                queryList = queryList.Where(item => item.tr_paymentMethod == payment).ToList();
            }

            if (type != "0")
            {
                queryList = queryList.Where(item => item.tr_type == type).ToList();
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime startDateTime = DateTime.Parse(startDate);
                DateTime endDateTime = DateTime.Parse(endDate);
                queryList = queryList.Where(item => item.tr_date >= startDateTime && item.tr_date <= endDateTime).ToList();
            }
            var tempSummary = new List<transactionSummary>();
            foreach (var tr in queryList)
            {
                var sum = new transactionSummary
                {
                    Year = tr.tr_date.Year,
                    Month = tr.tr_date.Month,
                    Day = tr.tr_date.Day,
                    PaymentMethod = tr.tr_paymentMethod,
                    Desc = tr.tr_desc,
                    Inflow = tr.tr_type == "Inflow" ? (decimal)tr.tr_amount : 0,
                    Outflow = tr.tr_type == "Outflow" ? (decimal)tr.tr_amount : 0,
                    Floor = tr.tr_fid != null ? tr.tb_floor.f_desc : "General",
                    FloorID = tr.tr_fid
                };
                tempSummary.Add(sum);
            }

            List<transactionSummary> groupedQuery = tempSummary.OrderBy(x => x.Year)
                  .ThenBy(x => x.Month)
                  .ThenBy(x => x.Day)
                  .ToList();

            foreach (var item in groupedQuery)
            {
                var monthRecords = groupedQuery.Where(x => x.Year == item.Year && x.Month == item.Month).ToList();
                item.TotalIn = monthRecords.Sum(x => x.Inflow);
                item.TotalOut = monthRecords.Sum(x => x.Outflow);
                item.Difference = item.TotalIn - item.TotalOut;
            }

            var result = new List<transactionSummary>();
            int currentYear = 0;
            int currentMonth = 0;
            int rowSpanYear = 0;
            int rowSpanMonth = 0;

            foreach (var item in groupedQuery)
            {
                if (item.Year != currentYear)
                {
                    currentYear = item.Year;
                    rowSpanYear = groupedQuery.Count(x => x.Year == currentYear);
                }

                if (item.Month != currentMonth)
                {
                    currentMonth = item.Month;
                    rowSpanMonth = groupedQuery.Count(x => x.Year == currentYear && x.Month == currentMonth);
                }

                item.RowSpanYear = rowSpanYear;
                item.RowSpanMonth = rowSpanMonth;

                result.Add(item);
            }
            return PartialView("_summaryTable", result);
        }



    }
}
