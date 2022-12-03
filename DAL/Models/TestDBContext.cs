using Microsoft.Extensions.Options;

namespace DAL.Models
{
    public partial class TestDBContext : DbContext
    {
        public TestDBContext()
        {
        }

        public TestDBContext(DbContextOptions<TestDBContext> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Article> Articles { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Articlenumber);

                entity.Property(e => e.Articlenumber)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId)
                    .HasMaxLength(10)
                    .HasColumnName("UserID")
                    .IsFixedLength();

                entity.Property(e => e.Birthday)
                    .HasMaxLength(10)
                    .HasColumnName("birthday")
                    .IsFixedLength();

                entity.Property(e => e.City)
                    .HasMaxLength(10)
                    .HasColumnName("city")
                    .IsFixedLength();

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .HasColumnName("code")
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .HasMaxLength(10)
                    .HasColumnName("email")
                    .IsFixedLength();

                entity.Property(e => e.Forename)
                    .HasMaxLength(10)
                    .HasColumnName("forename")
                    .IsFixedLength();

                entity.Property(e => e.HouseNumber)
                    .HasMaxLength(10)
                    .HasColumnName("house_number")
                    .IsFixedLength();

                entity.Property(e => e.Street)
                    .HasMaxLength(10)
                    .HasColumnName("street")
                    .IsFixedLength();

                entity.Property(e => e.Surename)
                    .HasMaxLength(10)
                    .HasColumnName("surename")
                    .IsFixedLength();

                entity.Property(e => e.Telephone)
                    .HasMaxLength(10)
                    .HasColumnName("telephone")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Ordernumber);

                entity.Property(e => e.Ordernumber)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Article)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Orderdate)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UserId)
                    .HasMaxLength(10)
                    .HasColumnName("UserID")
                    .IsFixedLength();

                entity.HasOne(d => d.ArticleNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.Article)
                    .HasConstraintName("FK_Orders_Articles");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Orders_Customers");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
