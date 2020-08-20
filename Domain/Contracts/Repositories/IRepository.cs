using System;
using System.Collections.Generic;
using System.Text;

namespace SpotSync.Domain.Contracts
{
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        T Get(T item);
        bool Exists(T item);
    }
}
