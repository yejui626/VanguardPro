﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db_vanguardproEntities : DbContext
    {
        public db_vanguardproEntities()
            : base("name=db_vanguardproEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tb_attendance> tb_attendance { get; set; }
        public virtual DbSet<tb_floor> tb_floor { get; set; }
        public virtual DbSet<tb_inventory> tb_inventory { get; set; }
        public virtual DbSet<tb_investors> tb_investors { get; set; }
        public virtual DbSet<tb_landlord> tb_landlord { get; set; }
        public virtual DbSet<tb_profit> tb_profit { get; set; }
        public virtual DbSet<tb_reminder> tb_reminder { get; set; }
        public virtual DbSet<tb_rental> tb_rental { get; set; }
        public virtual DbSet<tb_room> tb_room { get; set; }
        public virtual DbSet<tb_tenant> tb_tenant { get; set; }
        public virtual DbSet<tb_transaction> tb_transaction { get; set; }
        public virtual DbSet<tb_user> tb_user { get; set; }
    }
}