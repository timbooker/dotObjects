using System;
using System.Collections.Generic;
using dotObjects.Core.Configuration;

namespace dotObjects.Core.Persistence
{
    /// <summary>
    /// The dotObjects Context factory class.
    /// </summary>
    public static class ContextFactory
    {
        [ThreadStatic]
        private static Dictionary<string, Context> contexts;

        private static Dictionary<string, Context> Contexts
        {
            get { return contexts ?? (contexts = new Dictionary<string, Context>()); }
        }

        /// <summary>
        /// Gets the current Context instance.
        /// </summary>
        /// <returns><see cref="Context"/></returns>
        public static Context GetContext(Type type)
        {
            var name = GetContextName(type);
            CoreSection config = CoreSection.Current;
            if (!Contexts.ContainsKey(name))
                Contexts.Add(name, config.Contexts.Find(name).GetContext());
            if (Contexts[name].IsReleased)
                Contexts[name] = config.Contexts.Find(name).GetContext();
            return Contexts[name];
        }

        public static Context GetContext(Type type, bool fromCache)
        {
            if (fromCache)
                return GetContext(type);
            return CoreSection.Current.Contexts.Find(GetContextName(type)).GetContext();
        }

        private static string GetContextName(Type type)
        {
            ContextAttribute[] attrs = (ContextAttribute[]) type.GetCustomAttributes(typeof (ContextAttribute), false);
            if (attrs.Length > 0)
                return attrs[0].Name;
            return CoreSection.Current.Contexts.Default;
        }
    }
}