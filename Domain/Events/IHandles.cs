using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Events
{
    public interface IHandles<T> where T : IDomainEvent
    {
        void Handle(T args);
    }
}
