//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VanguardPro.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_transaction
    {
        public int tr_id { get; set; }
        public Nullable<int> tr_fid { get; set; }
        public string tr_desc { get; set; }
        public string tr_type { get; set; }
        public string tr_paymentMethod { get; set; }
        public System.DateTime tr_date { get; set; }
        public string tr_receipt { get; set; }
        public decimal tr_amount { get; set; }
    
        public virtual tb_floor tb_floor { get; set; }
    }
}
