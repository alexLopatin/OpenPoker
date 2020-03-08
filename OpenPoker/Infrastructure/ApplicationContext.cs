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

            modelBuilder.Entity<MatchUsers>()
                .HasKey(mu => new { mu.MatchId, mu.UserId });
            modelBuilder.Entity<MatchUsers>()
                    .HasOne(mu => mu.Match)
                    .WithMany(m => m.Users)
                    .HasForeignKey(mu => mu.MatchId);
            modelBuilder.Entity<MatchUsers>()
                    .HasOne(mu => mu.User)
                    .WithMany(u => u.matches)
                    .HasForeignKey(mu => mu.UserId);
        }
    }
}
