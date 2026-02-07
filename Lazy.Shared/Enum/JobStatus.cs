using System.ComponentModel;

namespace Lazy.Shared.Enum
{
    public enum JobStatus
    {
        [Description("运行中")]
        Pending = 0,

        [Description("运行中")]
        Running = 1,

        [Description("已停止")]
        Stopped = 2
    }
}
