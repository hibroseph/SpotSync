using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpotSync.Domain.Events
{
    public interface IHandles<T> where T : IDomainEvent
    {
        Task HandleAsync(T args);
    }
}
