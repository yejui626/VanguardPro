using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VanguardPro.Models
{
    public class transactionSummary
    {
        public System.DateTime tr_date { get; set; }
        public string tr_type { get; set; }
        public string tr_paymentMethod { get; set; }
        public string tr_desc { get; set; }
        public decimal tr_amount { get; set; }
        public decimal Total { get; set; }
    }

    public class transactionSubSummary
    {
        public string tr_desc { get; set; }
        public decimal tr_amount { get; set; }
        public string tr_type { get; set; }

        public string tr_paymentMethod { get; set; }

        public System.DateTime tr_date { get; set; }

    }
}