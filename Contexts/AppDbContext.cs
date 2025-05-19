namespace StreamierGraphQLServer.Contexts;

using Microsoft.EntityFrameworkCore;
using StreamierGraphQLServer.Models;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// The primary database context for the application, managing interactions with the database.
/// Handles entity tracking, change detection, and automatic timestamp updates for entities.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Users table in the database.
    /// Represents all registered users of the application.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        UpdateTimestamps();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Saves all changes made in this context to the database synchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();

        return base.SaveChanges();
    }

    /// <summary>
    /// Saves all changes made in this context to the database synchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the timestamps for all modified entities that inherit from <see cref="BaseEntity"/>.
    /// Sets the UpdatedAt field to the current UTC time for all modified entities.
    /// </summary>
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
