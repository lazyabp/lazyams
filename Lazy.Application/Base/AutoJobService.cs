using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class AutoJobService : CrudService<AutoJob, AutoJobDto, AutoJobDto, long, FilterPagedResultRequestDto, CreateAutoJobDto, UpdateAutoJobDto>,
    IAutoJobService, ITransientDependency
{
    public AutoJobService(LazyDBContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }
}
