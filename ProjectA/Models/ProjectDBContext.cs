using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace ProjectA.Models
{
    public class ProjectDBContext : DbContext
    {
        static ProjectDBContext() 
        {
            Database.SetInitializer<ProjectDBContext>(null);
        }

        public DbSet<Register> register { get; set; }
        public DbSet<ProductDB> Products { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<shopInfo> shopInfo { get; set; }
        public DbSet<Images> image { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Note> Notes { get; set; }

    }
}