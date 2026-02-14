using Microsoft.EntityFrameworkCore;
using TestDB.Server.Models;

namespace TestDB.Server.Data;

/// <summary>
/// Database context for TestDB
/// </summary>
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Requirement> Requirements { get; set; }
    public DbSet<TestCase> TestCases { get; set; }
    public DbSet<RequirementTestMapping> RequirementTestMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username").IsRequired();
            entity.Property(e => e.Password).HasColumnName("password").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Requirement entity
        modelBuilder.Entity<Requirement>(entity =>
        {
            entity.ToTable("requirements");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Priority)
                .HasColumnName("priority")
                .HasConversion<string>();
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure TestCase entity
        modelBuilder.Entity<TestCase>(entity =>
        {
            entity.ToTable("test_cases");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Steps).HasColumnName("steps");
            entity.Property(e => e.ExpectedResult).HasColumnName("expected_result");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure RequirementTestMapping entity
        modelBuilder.Entity<RequirementTestMapping>(entity =>
        {
            entity.ToTable("requirement_test_mapping");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RequirementId).HasColumnName("requirement_id");
            entity.Property(e => e.TestCaseId).HasColumnName("test_case_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.HasOne(e => e.Requirement)
                .WithMany()
                .HasForeignKey(e => e.RequirementId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.TestCase)
                .WithMany()
                .HasForeignKey(e => e.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.RequirementId, e.TestCaseId }).IsUnique();
        });
    }
}
