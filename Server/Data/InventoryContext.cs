using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using DotNetGalutinis.Server.Models.Inventory;

namespace DotNetGalutinis.Server.Data
{
    public partial class InventoryContext : DbContext
    {
        public InventoryContext()
        {
        }

        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.OnModelBuilding(builder);
        }

        public DbSet<DotNetGalutinis.Server.Models.Inventory.User> Users { get; set; }

        public DbSet<DotNetGalutinis.Server.Models.Inventory.Item> Items { get; set; }

        public DbSet<DotNetGalutinis.Server.Models.Inventory.Reservation> Reservations { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}