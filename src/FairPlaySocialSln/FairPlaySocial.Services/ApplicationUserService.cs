using FairPlaySocial.Common.CustomAttributes;
using FairPlaySocial.DataAccess.Models;
using FairPlaySocial.Common.CustomExceptions;
using FairPlaySocial.Models.FilteringSorting;
using FairPlaySocial.Services.Utils;

namespace FairPlaySocial.Services
{
    [ServiceOfEntity(entityName: nameof(ApplicationUser), primaryKeyType: typeof(long))]
    public partial class ApplicationUserService
    {
        public IQueryable<ApplicationUser> 
            GetFilteredApplicationUser(FilteringSortingModel filteringSortingModel)
        {
            if (filteringSortingModel.Filtering is null)
                throw new CustomValidationException("filter must not be empty");
            var query = this._fairplaysocialDatabaseContext.ApplicationUser.
                Where(
                propertyName: filteringSortingModel.Filtering!.PropertyName!,
                comparison: filteringSortingModel.Filtering!.ComparisonOperator!.Value,
                value: filteringSortingModel.Filtering!.PropertyComparisonValue!);
            if (filteringSortingModel.Sorting != null)
                return query.OrderBy(filteringSortingModel.Sorting);
            return query;
        }
    }
}