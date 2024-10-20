using Microsoft.EntityFrameworkCore;
using TestTask.Models.Patient;

namespace TestTask.Persistence;

public class ApplicationDbContext: DbContext
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<GivenName> GivenNames => Set<GivenName>();

    public DbSet<PatientName> PatientNames => Set<PatientName>();

    public ApplicationDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var sqlUser = Environment.GetEnvironmentVariable("SQL_USER");
        var sqlPass = Environment.GetEnvironmentVariable("SQL_PASSWORD");
        var sqlServer = Environment.GetEnvironmentVariable("SQL_SERVER");
        optionsBuilder.UseSqlServer($"Server={sqlServer};Initial Catalog=hospital;User ID={sqlUser};Password={sqlPass};");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.Name)
            .WithOne(n => n.Patient);
        
        modelBuilder.Entity<PatientName>()
            .HasOne(n => n.Patient)
            .WithOne(p => p.Name);
        
        modelBuilder.Entity<PatientName>()
            .HasMany(n => n.Given)
            .WithOne(g => g.PatientName)
            .HasForeignKey(n => n.PatientNameId);
        
        modelBuilder.Entity<PatientName>()
            .Property(n => n.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<Patient>()
            .Property(n => n.Id)
            .ValueGeneratedNever();
        
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.BirthDate);
        
        modelBuilder.Entity<GivenName>()
            .Property(n => n.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<GivenName>()
            .HasOne(g => g.PatientName)
            .WithMany(n => n.Given)
            .HasForeignKey(g => g.PatientNameId);
        
    }
}