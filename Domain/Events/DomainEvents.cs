using SpotSync.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Threading.Tasks;

/*
 This was implemented using Udi Dahan Domain Events blog post:
 https://udidahan.com/2009/06/14/domain-events-salvation/
 */
namespace SpotSync.Domain.Events
{
    public static class DomainEvents
    {
        private static List<Delegate> _actions;
        private static IServiceProvider _container;

        public static void Configure(IServiceProvider provider)
        {
            _container = provider;
        }

        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (_actions == null)
            {
                _actions = new List<Delegate>();
            }

            _actions.Add(callback);
        }

        public static void ClearCallbacks()
        {
            _actions = null;
        }

        public async static Task RaiseAsync<T>(T args) where T : IDomainEvent
        {
            if (_container != null)
            {
                IHandles<T> service = ((IHandles<T>)_container.GetService(typeof(IHandles<T>)));
                if (service != null)
                {
                    await service.HandleAsync(args);
                }
            }

            if (_actions != null)
            {
                foreach (var action in _actions)
                {
                    if (action is Action<T>)
                    {
                        ((Action<T>)action)(args);
                    }
                }
            }
        }
    }
}
