namespace Server.Contexts;

using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Models.Base;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Session> Sessions => Set<Session>();

    public DbSet<TempCode> TempCodes => Set<TempCode>();

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        UpdateTimestamps();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();

        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        var modifiedEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(entity => entity.State == EntityState.Modified);

        foreach (var entity in modifiedEntities)
        {
            entity.Entity.UpdatedAt = utcNow;
        }
    }
}
