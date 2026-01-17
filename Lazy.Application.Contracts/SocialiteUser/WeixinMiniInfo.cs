using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.SocialiteUser;

public class WeixinMiniInfo
{
    public string Code { get; set; }

    public string UserInfo { get; set; }

    public string IV { get; set; }

    public string EncryptedData { get; set; }
}
