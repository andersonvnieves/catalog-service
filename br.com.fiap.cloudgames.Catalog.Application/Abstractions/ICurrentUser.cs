using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.Abstractions
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        string Name { get; }
        string Email { get; }
    }
}
