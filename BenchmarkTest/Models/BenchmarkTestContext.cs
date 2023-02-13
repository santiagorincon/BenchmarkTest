using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkTest.Models;

public partial class BenchmarkTestContext : DbContext
{
    public BenchmarkTestContext()
    {
    }

    public BenchmarkTestContext(DbContextOptions<BenchmarkTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameScore> GameScores { get; set; }

    public virtual DbSet<GameUser> GameUsers { get; set; }

    public virtual DbSet<Roll> Rolls { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.WinnerUser).WithMany(p => p.Games)
                .HasForeignKey(d => d.WinnerUserId)
                .HasConstraintName("FK_GamesWinnerUserId");
        });

        modelBuilder.Entity<GameScore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameScor__3214EC074C108602");

            entity.HasOne(d => d.Game).WithMany(p => p.GameScores)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameScore__GameI__34C8D9D1");

            entity.HasOne(d => d.User).WithMany(p => p.GameScores)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GameScore__UserI__35BCFE0A");
        });

        modelBuilder.Entity<GameUser>(entity =>
        {
            entity.HasOne(d => d.Game).WithMany(p => p.GameUsers)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameUsersGameId");

            entity.HasOne(d => d.User).WithMany(p => p.GameUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GameUsersUserId");
        });

        modelBuilder.Entity<Roll>(entity =>
        {
            entity.Property(e => e.IsSpare).HasColumnName("isSpare");
            entity.Property(e => e.IsStrike).HasColumnName("isStrike");

            entity.HasOne(d => d.Game).WithMany(p => p.Rolls)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RollsGameId");

            entity.HasOne(d => d.User).WithMany(p => p.Rolls)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RollsUserId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
