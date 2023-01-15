using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Fims.Data.Entities;
using static Fims.Data.ModelConstants.Common;


namespace Fims.Data.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<FimsUser>
    {
        public void Configure(EntityTypeBuilder<FimsUser> user)
        {
            user
                .Property(u => u.HangulName)
                .HasMaxLength(MaxNameLength)
                .IsRequired();

            user
                .Property(u => u.EnglishName)
                .HasMaxLength(MaxNameLength);
                //optional  .IsRequired();

            user
                .HasIndex(u => u.IsDeleted);

            user
                .HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
