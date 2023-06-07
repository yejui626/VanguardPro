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
            transactionSubSummary SubSummary = new transactionSubSummary();
            transactionSummary Summary = new transactionSummary();
            var query = from tr in db.tb_transaction
                        select new transactionSubSummary
                        {
                            Year = tr.tr_date.Year,
                            Month = tr.tr_date.Month,
                            Day = tr.tr_date.Day,
                            PaymentMethod = tr.tr_paymentMethod,
                            Desc = tr.tr_desc,
                            Inflow = tr.tr_type == "Inflow" ? (decimal)tr.tr_amount : 0,
                            Outflow = tr.tr_type == "Outflow" ? (decimal)tr.tr_amount : 0,
                        };

            var resultList = query.GroupBy(x => new { x.Year, x.Month })
                              .Select(g => new transactionSummary
                              {
                                  Year = g.Key.Year,
                                  Month = g.Key.Month,
                                  Day = 0,
                                  Inflow = 0,
                                  Outflow = 0,
                                  PaymentMethod = null,
                                  Desc = null,
                                  TotalIn = g.Sum(x => x.Inflow),
                                  TotalOut = g.Sum(x => x.Outflow),
                                  Difference = g.Sum(x => x.Inflow) - g.Sum(x => x.Outflow)
                              })
                              .OrderBy(x => x.Year)
                              .ThenBy(x => x.Month)
                              .ThenBy(x => x.Day)
                              .ToList();
            return View(resultList);
        }
    }
}
