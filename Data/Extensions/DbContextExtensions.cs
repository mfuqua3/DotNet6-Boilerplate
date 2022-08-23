using Microsoft.EntityFrameworkCore;
using Utility.DataContracts.Interfaces;

namespace Data.Extensions;

public static class DbContextExtensions
{
    public static void ProcessCustomerInterfaces(this DbContext dbContext)
    {
        var autoDetectChangesEnabled = dbContext.ChangeTracker.AutoDetectChangesEnabled;
        try
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                #region Configure ISoftDelete Fields
                if (entry.Entity is ISoftDelete deleted)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            deleted.IsDeleted = false;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged;
                            deleted.IsDeleted = true;
                            deleted.Deleted = DateTime.UtcNow;
                            break;
                    }
                }
                #endregion

                #region Configure ITracked Fields
                if (entry.Entity is ICreated created && entry.State == EntityState.Added)
                {
                    created.Created = DateTime.UtcNow;
                }
                if (entry.Entity is IUpdated updated && entry.State == EntityState.Modified)
                {
                    updated.Updated = DateTime.UtcNow;
                }
                #endregion
            }
        }
        finally
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;
        }
    } 
}