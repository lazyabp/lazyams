using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class UserInfoDto : BaseEntityDto
{
    public string UserName { get; set; }

    public string NickName { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public int? Age { get; set; }

    public bool IsAdministrator { get; set; }

    public Access Access { get; set; }

    public Gender Gender { get; set; }

    public string Avatar { get; set; }

    public string Address { get; set; }

    public bool IsActive { get; set; }
}
