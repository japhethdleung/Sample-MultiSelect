using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_MultiSelect.Data.DbContexts
{
    public class appDbContext : DbContext
    {
        public appDbContext()
            : base("appDbContext")
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasMany(i => i.Players)
                .WithMany(i => i.Teams)
                .Map(i =>
                {
                    i.MapLeftKey("TeamId");
                    i.MapRightKey("PlayerId");
                    i.ToTable("Team_Player");
                });
        }
    }
}
