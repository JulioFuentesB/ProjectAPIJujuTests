using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public partial class JujuTestContext : DbContext
    {
        public JujuTestContext()
        {
        }

        public JujuTestContext(DbContextOptions<JujuTestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Post> Post { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.Body).HasMaxLength(500);

                entity.Property(e => e.Category).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(500);
            });
        }
    }
}
