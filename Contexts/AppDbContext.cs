namespace StreamierGraphQLServer.Contexts;

using Microsoft.EntityFrameworkCore;
using StreamierGraphQLServer.Models;
using StreamierGraphQLServer.Models.Base;

/// <summary>
/// The primary database context for the application, managing interactions with the database.
/// Handles entity tracking, change detection, and automatic timestamp updates for entities.
/// </summary>
/// <param name="options">The configuration options for this context, including connection string and provider settings.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Users table in the database.
    /// Represents all registered users of the application.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets or sets the Sessions table in the database.
    /// Represents active user sessions within the application.
    /// </summary>
    public DbSet<Session> Sessions => Set<Session>();

    /// <summary>
    /// Gets or sets the TempCodes table in the database.
    /// Stores temporary authentication or verification codes with expiration.
    /// </summary>
    public DbSet<TempCode> TempCodes => Set<TempCode>();

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether AcceptAllChanges should be called
    /// after the changes were successfully sent to the database.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
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
    /// Saves all changes made in this context to the database synchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        UpdateTimestamps();

        return base.SaveChanges();
    }

    /// <summary>
    /// Saves all changes made in this context to the database synchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether AcceptAllChanges should be called
    /// after the changes were successfully sent to the database.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// Automatically updates timestamps for modified entities before saving.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
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
