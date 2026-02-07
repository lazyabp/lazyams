using Lazy.Application.Contracts.Events;
using MediatR;

namespace Lazy.Application.EventHandlers;

public class SendWelcomeEmailHandler : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        // 模拟发送邮件
        Console.WriteLine($"发送欢迎邮件给 {notification.UserName}");
        await Task.Delay(1000);
    }
}
