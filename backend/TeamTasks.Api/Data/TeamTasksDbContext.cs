using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TeamTasks.Api.Models;

namespace TeamTasks.Api.Data;

public sealed class TeamTasksDbContext : DbContext
{
    public TeamTasksDbContext(DbContextOptions<TeamTasksDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var statusConverter = new EnumToStringConverter<TeamTasks.Api.Models.TaskStatus>();

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.Property(x => x.Name)
                .HasColumnType("varchar(100)")
                .IsRequired();

            entity.Property(x => x.Email)
                .HasColumnType("varchar(100)")
                .IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.HasMany(x => x.Tasks)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.Property(x => x.Title)
                .HasColumnType("varchar(150)")
                .IsRequired();

            entity.Property(x => x.Description)
                .HasColumnType("varchar(max)");

            entity.Property(x => x.UserId).IsRequired();

            entity.Property(x => x.Status)
                .HasColumnType("varchar(20)")
                .HasConversion(statusConverter)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            entity.Property(x => x.AdditionalData)
                .HasColumnType("nvarchar(max)");

            entity.HasIndex(x => new { x.UserId, x.Status })
                .HasDatabaseName("IX_Tasks_UserId_Status");

            entity.HasCheckConstraint(
                "CK_Tasks_Status_Allowed",
                "[Status] IN ('Pending','InProgress','Done')"
            );

            entity.HasCheckConstraint(
                "CK_Tasks_AdditionalData_JSON",
                "[AdditionalData] IS NULL OR ISJSON([AdditionalData]) = 1"
            );
        });
    }
}

