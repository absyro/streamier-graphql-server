namespace Server.Contexts;

using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Models.User> Users { get; set; }

    public DbSet<Models.Session> Sessions { get; set; }

    public DbSet<Models.TempCode> TempCodes { get; set; }

    public override int SaveChanges()
    {
        var modifiedEntities = ChangeTracker
            .Entries()
            .Where(entity => entity.State == EntityState.Modified);

        foreach (var entity in modifiedEntities)
        {
            if (entity.Entity is Models.Base.BaseEntity baseEntity)
            {
                baseEntity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
    }
}
