using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Entity;

/// <summary>
/// 数据传输对象
/// </summary>
public class TData
{
    /// <summary>
    /// 操作结果，Tag为1代表成功，0代表失败，其他的验证返回结果，可根据需要设置
    /// </summary>
    public int Tag { get; set; }

    /// <summary>
    /// 提示信息或异常信息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 扩展Message
    /// </summary>
    public string Description { get; set; }
}
