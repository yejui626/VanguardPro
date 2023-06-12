using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanguardPro.Models;

namespace VanguardPro.Controllers
{
    public class HomeController : Controller
    {
        private db_vanguardproEntities db = new db_vanguardproEntities();
        public ActionResult Index()
        {
            int totalFloors = db.tb_floor.Count();
            int totalRooms = db.tb_room.Count();
            int totalTenants = db.tb_tenant.Count();

            ViewBag.totalTenants = totalTenants;
            ViewBag.TotalRooms = totalRooms;
            ViewBag.TotalFloors = totalFloors;

            // Retrieve the list of vacant rooms from the database
            var vacantRooms = db.tb_room.Include("tb_floor").ToList();
            ViewBag.VacantRooms = vacantRooms;

            DateTime today = DateTime.Today;
            var landlordsDueToday = db.tb_landlord.Where(l => l.l_due == today).ToList();
            ViewBag.landlordsDueToday = landlordsDueToday;

            var tb_tenant = db.tb_rental.Include("tb_room").Include("tb_tenant").ToList();

            // Calculate cleaner attendance per floor for the current month
            var attendanceCounts = new Dictionary<string, int>();

            // Get the current month and year
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var query = from tr in db.tb_transaction
                        join fl in db.tb_floor on tr.tr_fid equals fl.f_id into floorGroup
                        from fl in floorGroup.DefaultIfEmpty()
                        select tr;
            var queryList = query.ToList();
            var tempSummary = new List<HomeDashboard>();
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

                var homeDashboard = new HomeDashboard
                {
                    TransactionSummary = sum
                };

                tempSummary.Add(homeDashboard);
            }

            var groupedQuery = tempSummary.OrderBy(x => x.TransactionSummary.Year)
                  .ThenBy(x => x.TransactionSummary.Month)
                  .ThenBy(x => x.TransactionSummary.Day)
                  .ToList();

            foreach (var item in groupedQuery)
            {
                var monthRecords = groupedQuery.Where(x => x.TransactionSummary.Year == item.TransactionSummary.Year && x.TransactionSummary.Month == item.TransactionSummary.Month).ToList();
                item.TransactionSummary.TotalIn = monthRecords.Sum(x => x.TransactionSummary.Inflow);
                item.TransactionSummary.TotalOut = monthRecords.Sum(x => x.TransactionSummary.Outflow);
                item.TransactionSummary.Difference = item.TransactionSummary.TotalIn - item.TransactionSummary.TotalOut;
            }

            var result = new List<HomeDashboard>();
            int rowSpanYear = 0;
            int rowSpanMonth = 0;

            foreach (var item in groupedQuery)
            {
                if (item.TransactionSummary.Year != currentYear)
                {
                    currentYear = item.TransactionSummary.Year;
                    rowSpanYear = groupedQuery.Count(x => x.TransactionSummary.Year == currentYear);
                }

                if (item.TransactionSummary.Month != currentMonth)
                {
                    currentMonth = item.TransactionSummary.Month;
                    rowSpanMonth = groupedQuery.Count(x => x.TransactionSummary.Year == currentYear && x.TransactionSummary.Month == currentMonth);
                }

                item.TransactionSummary.RowSpanYear = rowSpanYear;
                item.TransactionSummary.RowSpanMonth = rowSpanMonth;

                result.Add(item);
            }
            UpdateRoomStatus();
            return View(result);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void UpdateRoomStatus()
        {
            var tenantsToUpdate = db.tb_rental.Include("tb_room").Include("tb_tenant").ToList();
            var currentDate = DateTime.Now;

            foreach (var tenant in tenantsToUpdate)
            {
                if (tenant.re_outdate == null && tenant.re_outsession == null)
                {
                    tenant.tb_room.r_availability = "Occupied";
                }
                else
                {
                    if (currentDate.Date > tenant.re_outdate.Value.Date)
                    {
                        tenant.tb_room.r_availability = "Available";
                    }
                    else if (currentDate.Date == tenant.re_outdate.Value.Date)
                    {
                        if (tenant.re_outsession == "Morning" && currentDate.TimeOfDay > TimeSpan.Parse("12:00:00"))
                            tenant.tb_room.r_availability = "Available";
                        else if (tenant.re_outsession == "Afternoon" && currentDate.TimeOfDay > TimeSpan.Parse("18:00:00"))
                            tenant.tb_room.r_availability = "Available";
                        else
                        {
                            tenant.tb_room.r_availability = "Occupied";
                        }
                    }
                    else
                    {
                        tenant.tb_room.r_availability = "Occupied";

                    }
                }

                db.SaveChanges();
            }
        }
    }
}