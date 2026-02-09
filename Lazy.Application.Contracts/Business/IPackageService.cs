using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IPackageService : ICrudService<PackageDto, PackageDto, long, PackageFilterPagedResultRequestDto, CreatePackageDto, UpdatePackageDto>
{
}
