using Microsoft.EntityFrameworkCore;
using VinylStore.Data.Models;
using static System.Net.Mime.MediaTypeNames;
using System;
using Microsoft.Identity.Client.AppConfig;


namespace VinylStore.Data
{
    public class WebDbContext : DbContext
    {

        public const string CONNECTION_STRING = "Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog = VinylStore; Integrated Security = True;";
        public DbSet<VinylData> Vinyl { get; set; }
        


        public WebDbContext() { }

        public WebDbContext(DbContextOptions<WebDbContext> contextOptions)
            : base(contextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
            // base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<BookData>()
            //    .HasMany(x => x.ClientsWhoTakeIt)
            //    .WithMany(x => x.BooksWhichUserTakes);


        }
    }
}
