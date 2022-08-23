using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Seeds;

public abstract class DataSeeder<T>: IEntityTypeConfiguration<T> where T:class
{
    protected abstract IEnumerable<T> SeedData { get; }

    public void Configure(EntityTypeBuilder<T> builder)
        => builder.HasData(SeedData);
}