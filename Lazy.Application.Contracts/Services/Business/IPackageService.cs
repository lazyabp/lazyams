using Lazy.Model.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IPackageService : ICrudService<PackageDto, PackageDto, long, PackageFilterPagedResultRequestDto, CreatePackageDto, UpdatePackageDto>
{
    Task<bool> ActiveAsync(long id, ActiveDto input);
}
