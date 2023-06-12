using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace VanguardPro.Models
{
    public class HomeDashboard
    {
        public virtual tb_rental tb_rental { get; set; }
        public virtual transactionSummary TransactionSummary { get; set; }
    }
}
