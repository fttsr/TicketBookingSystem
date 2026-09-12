using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishUserRegisteredAsync(Guid userId, string email);
    }
}
