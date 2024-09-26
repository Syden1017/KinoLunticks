using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Models;

public partial class KinoLunticsContext : DbContext
{
    public KinoLunticsContext()
    {
    }

    public KinoLunticsContext(DbContextOptions<KinoLunticsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BankAccount> BankAccounts { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Syden1810; Database=KinoLuntics; Integrated Security=true; Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.AccountNumber).HasName("PK_BankAccount_AccountNumber");

            entity.ToTable("BankAccount");

            entity.Property(e => e.AccountNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Cvccode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CVCCode");
            entity.Property(e => e.ExpirationDate)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasMany(d => d.UserLogins).WithMany(p => p.AccountNumbers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserBankAccount",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserLogin")
                        .HasConstraintName("FK_UserBankAccounts_UserLogin"),
                    l => l.HasOne<BankAccount>().WithMany()
                        .HasForeignKey("AccountNumber")
                        .HasConstraintName("FK_UserBankAccounts_AccountNumber"),
                    j =>
                    {
                        j.HasKey("AccountNumber", "UserLogin");
                        j.ToTable("UserBankAccounts");
                        j.IndexerProperty<string>("AccountNumber")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .IsFixedLength();
                        j.IndexerProperty<string>("UserLogin").HasMaxLength(25);
                    });
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieCode).HasName("PK_Movies_MovieCode");

            entity.Property(e => e.MovieCode)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.AgeRestriction)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.MovieDuration).HasMaxLength(10);
            entity.Property(e => e.MovieGenre).HasMaxLength(50);
            entity.Property(e => e.MovieName).HasMaxLength(50);
            entity.Property(e => e.TicketPrice).HasColumnType("decimal(19, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderNumber).HasName("PK_Order_OrderNumber");

            entity.ToTable("Order");

            entity.Property(e => e.OrderNumber)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.Movie)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.Seats)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.User).HasMaxLength(25);

            entity.HasOne(d => d.MovieNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Movie)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Movies");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Role_RoleID");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(25);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PK_Users_Login");

            entity.HasIndex(e => e.EmailAddress, "UQ_Users_EmailAddress").IsUnique();

            entity.Property(e => e.Login).HasMaxLength(25);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.UserLastName).HasMaxLength(25);
            entity.Property(e => e.UserName).HasMaxLength(25);
            entity.Property(e => e.UserRole)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRole)
                .HasConstraintName("FK_Users_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
