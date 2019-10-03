using KurirServer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            //modelBuilder.Entity<User>().HasMany<UserRole>("ActiveUserRoleID");

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
            modelBuilder.Entity<User>().HasData(
                new User() {
                    FirstName = "Default",
                    LastName = "eko",
                    Phone = "023771642",
                    Mail = "eko@eko.rs",
                    Pass = "eko",
                    RegistrationDate = DateTime.Now,
                    UserID = 1



                },
                new User()
                {
                    FirstName = "Max",
                    LastName = "Fast",
                    Phone = "023771642",
                    Mail = "max@gmail.com",
                    Pass = "lol",
                    RegistrationDate = DateTime.Now,
                    UserID = 2,
                    Procenat = 60



                },
                new User()
                {
                    FirstName = "Matija",
                    LastName = "Levai",
                    Phone = "023771642",
                    Mail = "Matija@gmail.com",
                    Pass = "lol",
                    RegistrationDate = DateTime.Now,
                    UserID = 3,
                    Procenat = 70



                });
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserID = 3, RoleID = 1, UserRoleID = 1 },
                new UserRole { UserID = 3, RoleID = 2, UserRoleID = 2 },
                new UserRole { UserID = 3, RoleID = 3, UserRoleID = 3 },
                new UserRole { UserID = 3, RoleID = 4, UserRoleID = 4 },
                new UserRole { UserID = 3, RoleID = 5, UserRoleID = 5 },
                new UserRole { UserID = 1, RoleID = 3, UserRoleID = 6 },
                new UserRole { UserID = 2, RoleID = 3, UserRoleID = 7 },
                new UserRole { UserID = 2, RoleID = 4, UserRoleID = 8 },
                new UserRole { UserID = 2, RoleID = 5, UserRoleID = 9 }

                );
            modelBuilder.Entity<Location>().HasData(
                new Location { LocationID = 1,
                    Latitude = 45.256872,
                    Longitude = 19.849679,
                    Altitude = 0,
                    DToffSet = new DateTimeOffset(1,1,1,12,0,0,new TimeSpan(2,0,0))
                } );
           modelBuilder.Entity<Delivery>().HasData(
               new Delivery()
               {
                   UserID = 1,
                   DispatcherID = 3,
                   CourierID = 2,
                   DeliveryTypeID = 1,
                   DeliveryID = 1,
                   NameStart = "Nikola",
                   NameEnd = "marko",
                   StartAddress = "Kosovska 1/2",
                   EndAddress = "Temerinska 12/2",
                   PhoneOfStart = "0612889085",
                   PhoneOfEnd = "0623339992",
                   ZoneEnd = 1,
                   ZoneStart = 1,
                   DeliveryPrice = 160,
                   CreateTime = new DateTime(2019, 9, 10, 12, 14, 0),
                   StartTime = new DateTime(2019, 9, 10, 12, 24, 0),
                   EndTime = DateTime.Now,
                   PaymentTypeID = 1,
                   StartLocationID = 1,
                   EndLocationID = 1
               },
               new Delivery()
               {
                   UserID = 1,
                   DispatcherID = 3,
                   CourierID = 2,
                   DeliveryTypeID = 1,
                   DeliveryID = 2,
                   NameStart = "Nina",
                   NameEnd = "marija",
                   StartAddress = "Temerinska 1/2",
                   EndAddress = "Kosovska 12/2",
                   PhoneOfStart = "0612889085",
                   PhoneOfEnd = "0623339992",
                   ZoneEnd = 1,
                   ZoneStart = 1,
                   DeliveryPrice = 160,
                   CreateTime = new DateTime(2019, 9, 10, 12, 14, 0),
                   StartTime = new DateTime(2019, 9, 10, 12, 24, 0),
                   EndTime = DateTime.Now,
                   PaymentTypeID = 2,
                   StartLocationID = 1,
                   EndLocationID = 1
               }
        );
        }
        }
}
