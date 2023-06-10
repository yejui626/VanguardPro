using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VanguardPro.Models
{
    public class transactionSummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public decimal Inflow { get; set; }
        public decimal Outflow { get; set; }
        public string PaymentMethod { get; set; }
        public string Desc { get; set; }
        public string Floor { get; set; }
        public int? FloorID { get; set; }
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }
        public decimal Difference { get; set; }
        public int RowSpanYear { get; set; }
        public int RowSpanMonth { get; set; }
    }
}