using System;
using System.Collections.Generic;

namespace CustomsSim.Core
{
    /// <summary>Tiny service locator. Prefer injection where practical.</summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<T>(T service) where T : class
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            Services[typeof(T)] = service;
        }

        public static void Unregister<T>() where T : class
        {
            Services.Remove(typeof(T));
        }

        public static T Resolve<T>() where T : class
        {
            Services.TryGetValue(typeof(T), out var service);
            return service as T;
        }

        public static bool TryResolve<T>(out T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var raw) && raw is T typed)
            {
                service = typed;
                return true;
            }
            service = null;
            return false;
        }

        public static void Clear() => Services.Clear();
    }
}
