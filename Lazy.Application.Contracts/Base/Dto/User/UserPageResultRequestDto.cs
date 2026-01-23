using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Base.Dto.User
{
    public class UserPageResultRequestDto : FilterPagedResultRequestDto
    {
        public string Email { get; set; }
        public bool? IsAdministrator { get; set; }
        public Access? Access { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateBegin { get; set; }
        public DateTime? CreateEnd { get; set; }
    }
}
