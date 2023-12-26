using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Domain.Constants;
using Security.Infrastructure.Entities;

namespace Security.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.User, schema: SchemaMetadata.Security);

        //Column names
        builder.Property(x => x.UserId).HasColumnName("id");
        builder.Property(x => x.Username).HasColumnName("username");
        builder.Property(x => x.Password).HasColumnName("password");
        builder.Property(x => x.Email).HasColumnName("email");
        builder.Property(x => x.Verified).HasColumnName("verified");
        builder.Property(x => x.Active).HasColumnName("active");

        builder.Property(t => t.Username)
            .HasMaxLength(ColumnMetadata.DefaultUserNameLength)
            .IsRequired();

        builder.Property(t => t.Password)
            .HasMaxLength(ColumnMetadata.DefaultPasswordLength)
            .IsRequired();

        builder.Property(t => t.Email)
            .HasMaxLength(ColumnMetadata.DefaultEmailLength)
            .IsRequired();
    }
}
