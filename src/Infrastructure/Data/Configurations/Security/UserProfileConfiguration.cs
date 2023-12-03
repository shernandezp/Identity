using Security.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Domain.Constants;

namespace Security.Infrastructure.Data.Configurations.Security;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.UserProfile, schema: SchemaMetadata.Security);

        //Column names
        builder.Property(x => x.UserId).HasColumnName("userid");
        builder.Property(x => x.ProfileId).HasColumnName("profileid");

    }
}
