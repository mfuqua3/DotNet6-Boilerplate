// using Compassus.Data.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Compassus.Data.EntityConfigurations;
//
// public class ExampleEntityConfiguration : IEntityTypeConfiguration<ExampleEntity>
// {
//     public void Configure(EntityTypeBuilder<ExampleEntity> builder)
//     {
//         builder.HasKey(x => x.Key); <== Specify Primary Key that isn't named "Id"
//         builder.HasIndex(x => x.ActiveDirectoryId).IsUnique(); <== Create Unique Index
//         builder.HasData(new []{new ExampleEntity { Id = 1 }}); <== Seed Data
//     }
// }

//This file is an example of a configuration for a database table.
//Adding files to this directory that implement the IEntityTypeConfiguration<T> interface will automatically
//be picked up by the EntityFramework model builder, and can be used to setting up more complex table relationships 
//prior to creating your migration.