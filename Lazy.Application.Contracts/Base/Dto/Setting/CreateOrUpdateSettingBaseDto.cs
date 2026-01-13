using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class CreateOrUpdateSettingBaseDto : BaseEntityDto
{
    public string Key { get; set; }

    public string Value { get; set; }
}
