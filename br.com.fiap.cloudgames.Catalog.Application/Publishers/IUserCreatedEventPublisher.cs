using br.com.fiap.cloudgames.Catalog.Application.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Publishers
{
    public interface IUserCreatedEventPublisher
    {
        Task PublishAsync(UserCreatedEvent message);
    }
}
