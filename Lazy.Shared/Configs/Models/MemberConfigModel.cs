using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Configs;

public class MemberConfigModel
{
    public bool EnableRegistration { get; set; } = true;
    public bool ValidateEmail { get; set; } = false;
    public bool ValidatePhoneNumber { get; set; } = false;
}
