//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VanguardPro.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class tb_landlord
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_landlord()
        {
            this.tb_floor = new HashSet<tb_floor>();
        }


        [Display(Name = "ID")]
        public int l_id { get; set; }
        [Display(Name = "Name")]
        public string l_name { get; set; }
        [Display(Name = "Phone")]
        public string l_phone { get; set; }
        [Display(Name = "Date")]
        public System.DateTime l_due { get; set; }
        [Display(Name = "Price")]
        public double l_price { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_floor> tb_floor { get; set; }
    }
}
