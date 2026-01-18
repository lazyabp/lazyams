using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

/// <summary>
/// 统一返回结果
/// </summary>
public class BaseResultDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = default;

    public BaseResultDto()
    {
    }

    /// <summary>
    /// BaseResultDto<T>
    /// </summary>
    /// <param name="data"></param>
    public BaseResultDto(T data)
    {
        Success = true;
        Message = "Successfully";
    }

    /// <summary>
    /// BaseResultDto<T>
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    public BaseResultDto(bool success, string message, T data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}

public class ResultResponseDto : BaseResultDto<object>
{
}
