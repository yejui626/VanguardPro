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

            foreach (var floor in db.tb_floor)
            {
                // Count the attendance for the current month and floor
                int attendanceCount = db.tb_attendance.Count(c => c.atd_check.HasValue
                    && c.atd_check.Value.Month == currentMonth
                    && c.atd_check.Value.Year == currentYear
                    && c.tb_floor.f_id == floor.f_id);
                attendanceCounts[floor.f_desc] = attendanceCount;
            }

            ViewBag.AttendanceCounts = attendanceCounts;

            //select and add to model exSummary
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


            UpdateRoomStatus();
            return View(tb_tenant);
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