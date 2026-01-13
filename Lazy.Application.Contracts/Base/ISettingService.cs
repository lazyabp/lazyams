using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface ISettingService : ICrudService<SettingDto, SettingDto, long, FilterPagedResultRequestDto, CreateSettingDto, UpdateSettingDto>
{
    Task<SettingDto> GetByKeyAsync(string key);

    Task<T> GetModelByKeyAsync<T>(string key);
}
