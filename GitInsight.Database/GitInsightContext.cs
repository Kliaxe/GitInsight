using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GitInsight.Database
{
    public sealed class GitInsightContext : DbContext
    {
        public DbSet<GitRepo> Repositories => Set<GitRepo>();
        public DbSet<UserDateCount> UserDateCounts => Set<UserDateCount>();
        public DbSet<Fork> Forks => Set<Fork>();

        public GitInsightContext(DbContextOptions<GitInsightContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fork>()
                .HasOne(e => e.Parent)
                .WithMany(e => e.Children);
        }
    }
}
