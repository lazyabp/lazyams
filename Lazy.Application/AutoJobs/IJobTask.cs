using Lazy.Shared.Entity;

namespace Lazy.Application.AutoJobs;

public interface IJobTask
{
    Task<TData> Start();
}
