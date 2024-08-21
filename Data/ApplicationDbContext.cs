using marketControlSpamers.Models;
using Microsoft.EntityFrameworkCore;

namespace marketControlSpamers.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<FederalEntity> FederalEntity { get; set; }
        public DbSet<Township> Township { get; set; }
        public DbSet<Neighborhood> Neighborhood { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<FederalEntity>().ToTable("FederalEntity");
            modelBuilder.Entity<Township>().ToTable("Township");
            modelBuilder.Entity<Neighborhood>().ToTable("Neighborhood");

            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.HasKey(e => e.IdUsers);
            //});
            modelBuilder.Entity<User>().HasOne(u => u.Role).WithMany().HasForeignKey(u => u.idRoles);
            modelBuilder.Entity<Township>().HasOne(t => t.FederalEntity).WithMany().HasForeignKey(t => t.idFederalEntity);
            modelBuilder.Entity<Neighborhood>().HasOne(n => n.Township).WithMany().HasForeignKey(n => n.idTownship);

            //modelBuilder.Entity<Role>(entity =>
            //{
            //    entity.HasKey(e => e.idRoles);
            //});
        }
    }
}
