using KurirServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KurirServer
{
    public class KurirDbContext:DbContext
    {
        private readonly IConfiguration configuration;

        public KurirDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }
        public DbSet<User> Users { get; set; }  
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<DeliveryType> DeliveryTypes { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Kurir"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentType>().HasData(
                new PaymentType()
                { PaymentTypeID = 1,
                    PaymentTypeName = "Kes" },
                 new PaymentType()
                 {
                     PaymentTypeID = 2,
                     PaymentTypeName = "Preko racuna"
                 },
                  new PaymentType()
                  {
                      PaymentTypeID = 3,
                      PaymentTypeName = "Kupon"
                  });
            modelBuilder.Entity<DeliveryType>().HasData(
               new DeliveryType()
               {
                   DeliveryTypeID = 1,
                   DeliveryTypeName = "Regular"
               },
               new DeliveryType()
               {
                   DeliveryTypeID = 2,
                   DeliveryTypeName = "Return shipping"
               },
               new DeliveryType()
               {
                   DeliveryTypeID = 3,
                   DeliveryTypeName = "Shoping"
               },
               new DeliveryType()
               {
                   DeliveryTypeID = 4,
                   DeliveryTypeName = "Post service"
               }
               );
            modelBuilder.Entity<Role>().HasData(
               new Role()
               {
                   RoleID = 1,
                   RoleName = "SuperAdmin"
               },
                new Role()
                {
                    RoleID = 2,
                    RoleName = "Admin"
                },
                new Role()
                {
                    RoleID = 3,
                    RoleName = "User"
                },
                new Role()
                {
                    RoleID = 4,
                    RoleName = "Courier"
                },
                new Role()
                {
                    RoleID = 5,
                    RoleName = "Dispatcher"
                });
        }
        }
}
