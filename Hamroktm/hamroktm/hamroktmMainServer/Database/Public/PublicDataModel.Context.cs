﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace hamroktmMainServer.Database.Public
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class hamroktmContext : DbContext
    {
        public hamroktmContext()
            : base("name=hamroktmContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Ad> Ads { get; set; }
        public virtual DbSet<Ad_Images> Ad_Images { get; set; }
        public virtual DbSet<AdComment> AdComments { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Category_SubCategory> Category_SubCategory { get; set; }
        public virtual DbSet<SearchTag> SearchTags { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Users_Follow> Users_Follow { get; set; }
        public virtual DbSet<AdPoster> AdPosters { get; set; }
    }
}