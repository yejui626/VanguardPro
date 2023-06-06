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
                            tr_date = tr.tr_date,
                            tr_type = tr.tr_type,
                            tr_paymentMethod = tr.tr_paymentMethod,
                            tr_desc = tr.tr_desc,
                        };

            var resultList = query.GroupBy(x => new { x.tr_date })
                              .Select(g => new transactionSummary
                              {
                                  tr_date = g.Key.tr_date,
                                  tr_type = null,
                                  tr_paymentMethod = null,
                                  tr_desc = null,
                                  Total = g.Sum(x => x.tr_amount)
                              })
                              .OrderBy(x => x.tr_date)
                              .ToList();




            return View(resultList);
        }
    }
}
