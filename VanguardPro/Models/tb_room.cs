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
    
    public partial class tb_room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_room()
        {
            this.tb_rental = new HashSet<tb_rental>();
        }
    
        public int r_id { get; set; }
        public int r_fid { get; set; }
        public double r_price { get; set; }
        public string r_availability { get; set; }
        public string r_roomNo { get; set; }
    
        public virtual tb_floor tb_floor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_rental> tb_rental { get; set; }
    }
}
