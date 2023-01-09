using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Fims.Data.Entities;

using static Fims.Data.ModelConstants.Common;
using static Fims.Data.ModelConstants.TItem;


namespace Fims.Data.Configurations
{
    internal class TItemConfiguration : IEntityTypeConfiguration<TItem>
    {
        public void Configure(EntityTypeBuilder<TItem> tItem)
        {
            tItem
                .Property(ti => ti.Title)
                //FIXME  .HasMaxLength(MaxNameLength) // 50 causes a Truncated error
                .IsRequired();

            tItem
                .HasOne(ti => ti.TSheet)
                .WithMany(ts => ts.TItems)
                .HasForeignKey(ti => ti.TSheetId)
                .OnDelete(DeleteBehavior.Restrict);

            tItem
                .HasIndex(ti => ti.IsDeleted);

            tItem
                .HasQueryFilter(ti => !ti.IsDeleted);
        }
    }
}