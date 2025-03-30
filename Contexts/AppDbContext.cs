namespace StreamierServer.Contexts;

using Microsoft.EntityFrameworkCore;
using StreamierServer.Models;
using StreamierServer.Models.Base;

/// <summary>
/// Represents the application database context.
/// </summary>
/// <param name="options">The database context options.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Represents the users table.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Represents the sessions table.
    /// </summary>
    public DbSet<Session> Sessions => Set<Session>();

    /// <summary>
    /// Represents the temp codes table.
    /// </summary>
    public DbSet<TempCode> TempCodes => Set<TempCode>();

    /// <summary>
    /// Represents the save changes method.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        UpdateTimestamps();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Represents the save changes method.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        UpdateTimestamps();

        return base.SaveChanges();
    }

    /// <summary>
    /// Represents the save changes method.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess"></param>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Represents the save changes method.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The number of state entries written to the database.</returns>
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
