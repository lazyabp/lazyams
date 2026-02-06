using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazy.Application.Contracts.Dto;

public class UserWithRoleIdsDto : UserInfoDto
{
    public List<long> RoleIds { get; set; } = new List<long>();
}


public class UserLoginDto
{
    public UserInfoDto User { get; set; }
    public List<long> RoleIds { get; set; } = new List<long>();
    public List<string> Permissions { get; set; }
    public List<MenuDto> Menus { get; set; }
}
