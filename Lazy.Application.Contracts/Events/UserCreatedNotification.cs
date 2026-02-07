using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Events;

public class UserCreatedNotification : INotification
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}
