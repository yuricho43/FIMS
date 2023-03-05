using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Fims.Data.Contracts;
using Fims.Data.Entities;


namespace Fims.Data
{
    public class FimsDbContext : IdentityDbContext<FimsUser, FimsRole, string>
    {
        public FimsDbContext(DbContextOptions<FimsDbContext> options)
            : base(options)
        {
        }

        public DbSet<TSheet> TSheets { get; set; }
        public DbSet<TItem> TItems { get; set; }

        public override int SaveChanges()
        {
            this.ApplyAuditInfoRules();
            this.ApplyDeletableEntityRules();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            this.ApplyDeletableEntityRules();

            int writtenEntriesCount = 0;
            try
            {
                writtenEntriesCount = await base.SaveChangesAsync(cancellationToken);
                if (writtenEntriesCount > 0)
                {
                    // is saved
                }
                else
                {
                    // is not saved
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //_logger.LogError(e, "couldn't SaveChangesAsync");
            }

            return writtenEntriesCount;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        private void ApplyAuditInfoRules()
            => this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added ||
                     e.State == EntityState.Modified))
                .ToList()
                .ForEach(entry =>
                {
                    var entity = (IAuditInfo)entry.Entity;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    else
                    {
                        entity.ModifiedOn = DateTime.UtcNow;
                    }
                });

        private void ApplyDeletableEntityRules()
            => this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IDeletableEntity &&
                    e.State == EntityState.Deleted)
                .ToList()
                .ForEach(entry =>
                {
                    var entity = (IDeletableEntity)entry.Entity;

                    entity.IsDeleted = true;
                    entity.DeletedOn = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                });
    }
}
