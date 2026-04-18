using System;
using System.Collections.Generic;

namespace CustomsSim.Core
{
    /// <summary>Minimal typed event bus. Handlers are invoked synchronously.</summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> Handlers = new();

        public static void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            var type = typeof(T);
            Handlers.TryGetValue(type, out var existing);
            Handlers[type] = existing == null ? handler : Delegate.Combine(existing, handler);
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var type = typeof(T);
            if (!Handlers.TryGetValue(type, out var existing)) return;
            var updated = Delegate.Remove(existing, handler);
            if (updated == null) Handlers.Remove(type);
            else Handlers[type] = updated;
        }

        public static void Raise<T>(T evt)
        {
            if (!Handlers.TryGetValue(typeof(T), out var d)) return;
            ((Action<T>)d)?.Invoke(evt);
        }

        public static void Clear() => Handlers.Clear();
    }
}
