using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class MemberSettingModel
{
    public bool EnableRegistration { get; set; } = true;
    public bool ValidateEmail { get; set; } = false;
    public bool ValidatePhoneNumber { get; set; } = false;
}
