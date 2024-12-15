using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr;

public partial class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetails> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("AppDbContext"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Carts_pkey");

            entity.HasIndex(e => e.ProductId, "fki_Carts_ProductId_fkey");

            entity.HasIndex(e => e.UserId, "fki_Carts_UserId_fkey");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Carts_ProductId_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Carts_UserId_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Products_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(1024);
            entity.Property(e => e.Title).HasMaxLength(128);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Transactions_pkey");

            entity.HasOne(e => e.User).WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("Transactions_UserId_fkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Login).HasMaxLength(128);
            entity.Property(e => e.PasswordHash).HasMaxLength(128);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Orders_pkey");

            entity.HasOne(e => e.User).WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("Orders_UserId_fkey");

            entity.HasMany(e => e.OrderDetails).WithOne();

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<OrderDetails>(entity =>
        {
            entity.ToTable("OrderDetails");

            entity.HasKey(e => e.Id).HasName("OrderDetails_pkey");

            entity.HasOne(e => e.Product).WithMany()
                .HasForeignKey(e => e.ProductId)
                .HasConstraintName("OrderDetails_ProductId_fkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
