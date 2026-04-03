using CarDelership.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDelership.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<StatusSupply> StatusSupplies { get; set; }
        public DbSet<Supply> Supplies { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Cars> Cars { get; set; }
        public DbSet<StatusSupply> StatusSuppliers { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<OrderStatuses> OrderStatuses { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<DeliveryMethods> DeliveryMethods { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Manufacturers> Manufacturers { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }
        public DbSet<EngineType> EngineTypes { get; set; }
        public DbSet<Transmission> Transmissions { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CarComments> CarComments { get; set; }
        public DbSet<CarTags> CarTags { get; set; }
    }
}
