using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class AutoJobLogService : CrudService<AutoJobLog, AutoJobLogDto, AutoJobLogDto, long, FilterPagedResultRequestDto, CreateAutoJobLogDto, UpdateAutoJobLogDto>,
    IAutoJobLogService, ITransientDependency
{
    public AutoJobLogService(LazyDBContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public async Task Clear()
    {
        var logs = await LazyDBContext.Database.ExecuteSqlRawAsync("truncate table AutoJobLog");
    }
}
