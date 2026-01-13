using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazy.Application.Contracts.Dto;

public class RolePermissionInput
{
    public long Id { get; set; }

    public List<long> MenuIds { get; set; }
}
