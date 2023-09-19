using APIOne.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace APIOne.Data;

public class DataContextEF : DbContext
{
    private readonly IConfiguration _config;
    public DataContextEF(IConfiguration config)
    {
        _config = config;
    }

    // these properties are what you access when you use EF (e.g. EF.Users.ToList())
    public DbSet<User>? Users { get; set; }
    public DbSet<UserJobInfo>? UserJobInfo { get; set; }
    public DbSet<UserSalary>? UserSalary { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseSqlServer(_config.GetConnectionString("Default"),
            options => options.EnableRetryOnFailure());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("TutorialAppSchema");
        
        // if I wanted UserJobInfo property above ot be something else, I could map it like this:
        // modelBuilder.Entity<UserJobInfo>().ToTable("UserJobInfo", "TutorialAppSchema").HasKey("UserId");

        // can also set key this way (or just have a PK in table in db like Users):
        // modelBuilder.Entity<UserJobInfo>().HasKey(u => u.UserId);

        modelBuilder.Entity<UserJobInfo>().HasKey("UserId");
        modelBuilder.Entity<UserSalary>().HasKey("UserId");
    }
}