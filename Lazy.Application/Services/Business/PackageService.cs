using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class PackageService :
    CrudService<Package, PackageDto, PackageDto, long, PackageFilterPagedResultRequestDto, CreatePackageDto, UpdatePackageDto>,
    IPackageService, ITransientDependency
{
    public PackageService(LazyDBContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    protected override IQueryable<Package> CreateFilteredQuery(PackageFilterPagedResultRequestDto input)
    {
        var query = GetQueryable();

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(x => x.Name == input.Filter || x.Description == input.Filter);

        return query;
    }

    public async Task<bool> ActiveAsync(long id, ActiveDto input)
    {
        var entity = await GetEntityByIdAsync(id);
        if (entity == null)
            throw new EntityNotFoundException($"Invalid package {id}");

        entity.IsActive = input.IsActive;
        SetUpdatedAudit(entity);

        await LazyDBContext.SaveChangesAsync();

        return true;
    }
}
