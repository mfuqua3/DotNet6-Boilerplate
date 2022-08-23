using System.Reflection;
using Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Data.Components;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplySoftDeleteQueryFilters();
        modelBuilder.RestrictForeignKeyDelete();
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        this.ProcessCustomerInterfaces();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}