
using BudgetPlanner.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER CONFIGURATION
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();
            });

            // CATEGORY CONFIGURATION
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);

                entity.Property(c => c.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Type)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasIndex(c => new
                {
                    c.UserId,
                    c.CategoryName,
                    c.Type
                }).IsUnique();

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TRANSACTION CONFIGURATION
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);

                entity.Property(t => t.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(t => t.Type)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(t => t.Description)
                    .HasMaxLength(500);

                entity.HasOne(t => t.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Category)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BUDGET CONFIGURATION
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasKey(b => b.BudgetId);

                entity.Property(b => b.MonthlyLimit)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.HasIndex(b => new
                {
                    b.UserId,
                    b.CategoryId,
                    b.Month,
                    b.Year
                }).IsUnique();

                entity.HasOne(b => b.User)
                    .WithMany(u => u.Budgets)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Category)
                    .WithMany(c => c.Budgets)
                    .HasForeignKey(b => b.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
