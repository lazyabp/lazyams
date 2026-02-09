using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class PackageFeatureService :
    CrudService<PackageFeature, PackageFeatureDto, PackageFeatureDto, long, PackageFeatureFilterPagedResultRequestDto, CreatePackageFeatureDto, UpdatePackageFeatureDto>,
    IPackageFeatureService, ITransientDependency
{
    public PackageFeatureService(LazyDBContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    protected override IQueryable<PackageFeature> CreateFilteredQuery(PackageFeatureFilterPagedResultRequestDto input)
    {
        var query = GetQueryable();

        if (input.PackageId.HasValue)
            query = query.Where(x => x.PackageId == input.PackageId.Value);

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(x => x.Description == input.Filter);

        return query;
    }
}
