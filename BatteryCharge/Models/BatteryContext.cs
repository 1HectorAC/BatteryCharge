using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatteryCharge.Models;

public partial class BatteryContext : DbContext
{
    public BatteryContext()
    {
    }

    public BatteryContext(DbContextOptions<BatteryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BatteryTrackerUser> BatteryTrackerUsers { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatteryTrackerUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.BatteryTrackerUser");

            entity.ToTable("BatteryTrackerUser");

            entity.Property(e => e.AspNetUserId).HasMaxLength(450);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Device");

            entity.ToTable("Device");

            entity.Property(e => e.DateBought).HasColumnType("date");
            entity.Property(e => e.LastRechargeDate).HasColumnType("date");
            entity.Property(e => e.Name).HasMaxLength(64);

            entity.HasOne(d => d.Owner).WithMany(p => p.Devices)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.Device");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
