using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IPackageFeatureService : ICrudService<PackageFeatureDto, PackageFeatureDto, long, PackageFeatureFilterPagedResultRequestDto, CreatePackageFeatureDto, UpdatePackageFeatureDto>
{
}
