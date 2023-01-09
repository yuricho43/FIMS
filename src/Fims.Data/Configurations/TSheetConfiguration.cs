using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Fims.Data.Entities;


namespace Fims.Data.Configurations
{
    internal class TSheetConfiguration : IEntityTypeConfiguration<TSheet>
    {
        public void Configure(EntityTypeBuilder<TSheet> tSheet)
        {
            tSheet
                .HasOne(ts => ts.User)
                .WithMany(u => u.TSheets)
                .HasForeignKey(ts => ts.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            tSheet
                .HasIndex(ts => ts.IsDeleted);

            tSheet
                .HasQueryFilter(ts => !ts.IsDeleted);
        }
   }
}
