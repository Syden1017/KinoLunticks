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

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<BankAccount> BankAccounts { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Row> Rows { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SelectedSeat> SelectedSeats { get; set; }

    public virtual DbSet<Showing> Showings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAB30-01\\SQLEXPRESS; Database=KinoLuntics; User=ИСП-42; Password=1234567890; Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.ActorId).HasName("PK_Actors_ActorID");

            entity.Property(e => e.ActorId).HasColumnName("ActorID");
            entity.Property(e => e.ActorLastName).HasMaxLength(50);
            entity.Property(e => e.ActorName).HasMaxLength(50);

            entity.HasMany(d => d.MovieCodes).WithMany(p => p.Actors)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieActor",
                    r => r.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieCode")
                        .HasConstraintName("FK_MovieActors_Movies"),
                    l => l.HasOne<Actor>().WithMany()
                        .HasForeignKey("ActorId")
                        .HasConstraintName("FK_MovieActors_Actors"),
                    j =>
                    {
                        j.HasKey("ActorId", "MovieCode");
                        j.ToTable("MovieActors");
                        j.IndexerProperty<int>("ActorId").HasColumnName("ActorID");
                    });
        });

        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.AccountNumber).HasName("PK_BankAccount_AccountNumber");

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

            entity.HasMany(d => d.Users).WithMany(p => p.AccountNumbers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserBankAccount",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserBankAccounts_UserID"),
                    l => l.HasOne<BankAccount>().WithMany()
                        .HasForeignKey("AccountNumber")
                        .HasConstraintName("FK_UserBankAccounts_AccountNumber"),
                    j =>
                    {
                        j.HasKey("AccountNumber", "UserId");
                        j.ToTable("UserBankAccounts");
                        j.IndexerProperty<string>("AccountNumber")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            .IsFixedLength();
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                    });
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK_Genres_GenreID");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.GenreName).HasMaxLength(25);

            entity.HasMany(d => d.MovieCodes).WithMany(p => p.Genres)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieCode")
                        .HasConstraintName("FK_MovieGenres_Movies"),
                    l => l.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK_MovieGenres_Genres"),
                    j =>
                    {
                        j.HasKey("GenreId", "MovieCode");
                        j.ToTable("MovieGenres");
                        j.IndexerProperty<int>("GenreId").HasColumnName("GenreID");
                    });
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.HallId).HasName("PK_Halls_HallID");

            entity.Property(e => e.HallId).HasColumnName("HallID");
            entity.Property(e => e.HallNumber).HasMaxLength(10);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieCode).HasName("PK_Movies_MovieCode");

            entity.Property(e => e.AgeRestriction)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.MovieDuration).HasMaxLength(10);
            entity.Property(e => e.MovieName).HasMaxLength(50);
            entity.Property(e => e.ProducerName).HasMaxLength(75);
            entity.Property(e => e.TicketPrice).HasColumnType("decimal(8, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderNumber);

            entity.Property(e => e.Amount).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.SelectedRowNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SelectedRow)
                .HasConstraintName("FK_Orders_Rows");

            entity.HasOne(d => d.ShowingNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Showing)
                .HasConstraintName("FK_Orders_Showings");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Role_RoleID");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(25);
        });

        modelBuilder.Entity<Row>(entity =>
        {
            entity.HasKey(e => e.RowId).HasName("PK_Rows_RowID");

            entity.Property(e => e.HallId).HasColumnName("HallID");
            entity.Property(e => e.RowNumber).HasMaxLength(10);

            entity.HasOne(d => d.Hall).WithMany(p => p.Rows)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rows_Halls");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK_Seats_SeatID");

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.SeatNumber).HasMaxLength(10);

            entity.HasOne(d => d.Row).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RowId)
                .HasConstraintName("FK_Seats_Rows");
        });

        modelBuilder.Entity<SelectedSeat>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.SeatId).HasColumnName("SeatID");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_SelectedSeats_Orders");

            entity.HasOne(d => d.Seat).WithMany()
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SelectedSeats_Seats");
        });

        modelBuilder.Entity<Showing>(entity =>
        {
            entity.HasKey(e => e.ShowingId).HasName("PK_Showings_ShowingID");

            entity.Property(e => e.ShowingId).HasColumnName("ShowingID");

            entity.HasOne(d => d.Hall).WithMany(p => p.Showings)
                .HasForeignKey(d => d.HallId)
                .HasConstraintName("FK_Showings_Halls");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showings)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK_Showings_Movies");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users_UserID");

            entity.HasIndex(e => e.EmailAddress, "UQ_Users_EmailAddress").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Login).HasMaxLength(25);
            entity.Property(e => e.UserLastName).HasMaxLength(25);
            entity.Property(e => e.UserName).HasMaxLength(25);
            entity.Property(e => e.UserRole).HasDefaultValue(2);

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRole)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
