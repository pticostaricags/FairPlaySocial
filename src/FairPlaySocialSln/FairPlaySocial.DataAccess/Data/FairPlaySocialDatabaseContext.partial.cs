using FairPlaySocial.Common.Interfaces;
using FairPlaySocial.Common.Providers;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Models.Post;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FairPlaySocial.DataAccess.Data
{
    public partial class FairPlaySocialDatabaseContext
    {
        private ICurrentUserProvider CurrentUserProvider { get; }

        public FairPlaySocialDatabaseContext(DbContextOptions<FairPlaySocialDatabaseContext> options,
            ICurrentUserProvider currentUserProvider) : base(options)
        {
            this.CurrentUserProvider = currentUserProvider;
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().ToTable(nameof(Post), p => p.IsTemporal(b =>
            {
                b.HasPeriodStart(nameof(PostModel.ValidFrom));
                b.HasPeriodEnd(nameof(PostModel.ValidTo));
                b.UseHistoryTable($"{nameof(Post)}Hitory");
            }));
        }

        public override int SaveChanges()
        {
            ValidateAndSetDefaultsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ValidateAndSetDefaultsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            await ValidateAndSetDefaultsAsync();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await ValidateAndSetDefaultsAsync();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task ValidateAndSetDefaultsAsync()
        {
            //Check https://www.bricelam.net/2016/12/13/validation-in-efcore.html
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            string ipAddresses = String.Empty;
            string assemblyFullName = String.Empty;
            string rowCretionUser = String.Empty;
            if (entities.Any(p => p is IOriginatorInfo))
            {
                ipAddresses = String.Join(",", await IpAddressProvider.GetCurrentHostIPv4AddressesAsync());
                assemblyFullName = System.Reflection.Assembly.GetEntryAssembly()!.FullName!;
                rowCretionUser = this.CurrentUserProvider.GetUsername();
            }
            foreach (var entity in entities)
            {
                if (entity is IOriginatorInfo)
                {
                    IOriginatorInfo entityWithOriginator = (entity as IOriginatorInfo)!;
                    if (String.IsNullOrWhiteSpace(entityWithOriginator.SourceApplication))
                    {
                        entityWithOriginator.SourceApplication = assemblyFullName;
                    }
                    if (String.IsNullOrWhiteSpace(entityWithOriginator.OriginatorIpaddress))
                    {
                        entityWithOriginator.OriginatorIpaddress = ipAddresses;
                    }
                    if (entityWithOriginator.RowCreationDateTime == DateTimeOffset.MinValue)
                    {
                        entityWithOriginator.RowCreationDateTime = DateTimeOffset.UtcNow;
                    }
                    if (String.IsNullOrWhiteSpace(entityWithOriginator.RowCreationUser))
                    {
                        entityWithOriginator.RowCreationUser = rowCretionUser;
                    }
                }
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }
        }
    }
}
