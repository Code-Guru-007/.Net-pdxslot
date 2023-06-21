using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PdxSlots.API.Models;

namespace PdxSlots.API.Data
{
    public partial class PdxSlotsContext : DbContext
    {
        public PdxSlotsContext()
        {
        }

        public PdxSlotsContext(DbContextOptions<PdxSlotsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameFeature> GameFeatures { get; set; }
        public virtual DbSet<GameMath> GameMaths { get; set; }
        public virtual DbSet<IgcuserGaf> IgcuserGafs { get; set; }
        public virtual DbSet<Open> Opens { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<PeriodicJob> PeriodicJobs { get; set; }
        public virtual DbSet<PeriodicJobEmail> PeriodicJobEmails { get; set; }
        public virtual DbSet<PeriodicJobZippedFile> PeriodicJobZippedFiles { get; set; }
        public virtual DbSet<Round> Rounds { get; set; }
        public virtual DbSet<RoundStatus> RoundStatuses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ZipFileUpload> ZipFileUploads { get; set; }
        public virtual DbSet<ZippedFile> ZippedFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=ConnectionStrings:DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.Browser)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.OperatingSystem)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Device_User");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.DeviceId)
                    .HasConstraintName("FK_Event_Device");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK_Event_Game");

                entity.HasOne(d => d.GameMath)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.GameMathId)
                    .HasConstraintName("FK_Event_GameMath");

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.OperatorId)
                    .HasConstraintName("FK_Event_Operator");

                entity.HasOne(d => d.PeriodicJob)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.PeriodicJobId)
                    .HasConstraintName("FK_Event_PeriodicJob");

                entity.HasOne(d => d.Round)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.RoundId)
                    .HasConstraintName("FK_Event_Round");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Event_User");
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Game");

                entity.HasIndex(e => e.ExternalGameId, "UQ_Game")
                    .IsUnique();

                entity.Property(e => e.DesktopFileUrl)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ExternalGameId)
                    .IsRequired()
                    .HasMaxLength(55)
                    .IsUnicode(false);

                entity.Property(e => e.MobileFileUrl)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.DesktopZipFileUpload)
                    .WithMany(p => p.GameDesktopZipFileUploads)
                    .HasForeignKey(d => d.DesktopZipFileUploadId)
                    .HasConstraintName("FK_Game_DesktopZipFileUpload");

                entity.HasOne(d => d.MobileZipFileUpload)
                    .WithMany(p => p.GameMobileZipFileUploads)
                    .HasForeignKey(d => d.MobileZipFileUploadId)
                    .HasConstraintName("FK_Game_MobileZipFileUpload");
            });

            modelBuilder.Entity<GameFeature>(entity =>
            {
                entity.ToTable("GameFeature");

                entity.Property(e => e.Feature)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UserId).HasMaxLength(500);

                entity.Property(e => e.Value).HasColumnType("decimal(16, 4)");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.GameFeatures)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK_GameFeature_Game");

                entity.HasOne(d => d.GameMath)
                    .WithMany(p => p.GameFeatures)
                    .HasForeignKey(d => d.GameMathId)
                    .HasConstraintName("FK_GameFeature_GameMath");

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.GameFeatures)
                    .HasForeignKey(d => d.OperatorId)
                    .HasConstraintName("FK_GameFeature_Operator");
            });

            modelBuilder.Entity<GameMath>(entity =>
            {
                entity.ToTable("GameMath");

                entity.Property(e => e.AvailableDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Bets).HasMaxLength(500);

                entity.Property(e => e.Denominations).HasMaxLength(500);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ExternalGameId)
                    .IsRequired()
                    .HasMaxLength(55)
                    .IsUnicode(false);

                entity.Property(e => e.ExternalOperatorId)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.MathFileUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.MaxBet).HasColumnType("money");

                entity.Property(e => e.MaxLiability).HasColumnType("money");

                entity.Property(e => e.PayTable).HasMaxLength(500);

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.GameMaths)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GameMath_Game");

                entity.HasOne(d => d.MathFileUpload)
                    .WithMany(p => p.GameMaths)
                    .HasForeignKey(d => d.MathFileUploadId)
                    .HasConstraintName("FK_GameMath_MathFileUpload");

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.GameMaths)
                    .HasForeignKey(d => d.OperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GameMath_Operator");
            });

            modelBuilder.Entity<IgcuserGaf>(entity =>
            {
                entity.ToTable("IGCUserGaf");

                entity.HasIndex(e => e.UserId, "UQ_IGCUserGaf")
                    .IsUnique();

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.IgcuserGafs)
                    .HasForeignKey(d => d.OperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IGCUserGaf_Operator");
            });

            modelBuilder.Entity<Open>(entity =>
            {
                entity.ToTable("Open");

                entity.Property(e => e.Browser)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Created)
                    .HasPrecision(0)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExternalGameId)
                    .IsRequired()
                    .HasMaxLength(55)
                    .IsUnicode(false);

                entity.Property(e => e.IpAddress)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OperatingSystem)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OperatorId)
                    .IsRequired()
                    .HasMaxLength(55)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Operator>(entity =>
            {
                entity.ToTable("Operator");

                entity.HasIndex(e => e.ExternalOperatorId, "UQ_Operator")
                    .IsUnique();

                entity.Property(e => e.ExternalOperatorId)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.Property(e => e.VendorId)
                    .IsRequired()
                    .HasMaxLength(55);
            });

            modelBuilder.Entity<PeriodicJob>(entity =>
            {
                entity.ToTable("PeriodicJob");

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Start)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<PeriodicJobEmail>(entity =>
            {
                entity.ToTable("PeriodicJobEmail");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<PeriodicJobZippedFile>(entity =>
            {
                entity.ToTable("PeriodicJobZippedFile");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrentHash).IsRequired();

                entity.Property(e => e.OriginalHash).IsRequired();

                entity.HasOne(d => d.PeriodicJob)
                    .WithMany(p => p.PeriodicJobZippedFiles)
                    .HasForeignKey(d => d.PeriodicJobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PeriodicJobZippedFile_PeriodicJob");

                entity.HasOne(d => d.ZippedFile)
                    .WithMany(p => p.PeriodicJobZippedFiles)
                    .HasForeignKey(d => d.ZippedFileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PeriodicJobZippedFile_ZippedFile");
            });

            modelBuilder.Entity<Round>(entity =>
            {
                entity.ToTable("Round");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Denomination).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExternalGameId)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.Property(e => e.ExternalOperatorId)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.Property(e => e.FundsEnd).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.FundsStart).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.IncentiveWager).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.IncentiveWin).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.NonWager).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PayTable)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.Property(e => e.ProgressiveWin).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ProgressiveWinCont).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.SessionId)
                    .IsRequired()
                    .HasMaxLength(55);

                entity.Property(e => e.Updated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Wager).HasColumnType("decimal(10, 0)");

                entity.Property(e => e.WagerCurrency).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.WalletApprovalId).HasMaxLength(55);

                entity.Property(e => e.WalletRoundId).HasMaxLength(55);

                entity.Property(e => e.Win).HasColumnType("decimal(10, 0)");

                entity.Property(e => e.WinCurrency).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Rounds)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Round_Device");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Rounds)
                    .HasForeignKey(d => d.GameId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Round_Game");

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.Rounds)
                    .HasForeignKey(d => d.OperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Round_Operator");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Rounds)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Round_RoundStatus");
            });

            modelBuilder.Entity<RoundStatus>(entity =>
            {
                entity.ToTable("RoundStatus");

                entity.HasIndex(e => e.Name, "UQ_RoundStatus_Name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.UserIdentityId, "IX_User_UserIdentityId");

                entity.HasIndex(e => e.UserIdentityId, "UC_User_UserIdentity")
                    .IsUnique();

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UserIdentityId)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ZipFileUpload>(entity =>
            {
                entity.ToTable("ZipFileUpload");

                entity.Property(e => e.BlobFileUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ZipFileUploads)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZipFileUpload_User");
            });

            modelBuilder.Entity<ZippedFile>(entity =>
            {
                entity.ToTable("ZippedFile");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FileUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Hash).IsRequired();

                entity.Property(e => e.LocalFilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.ZipFileUpload)
                    .WithMany(p => p.ZippedFiles)
                    .HasForeignKey(d => d.ZipFileUploadId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ZippedFile_ZipFileUpload");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
