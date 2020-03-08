using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenPoker.Models;

namespace OpenPoker.Infrastructure
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Match> Matches { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Match>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
        }
    }
}
