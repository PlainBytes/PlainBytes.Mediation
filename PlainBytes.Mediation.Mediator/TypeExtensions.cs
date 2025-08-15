using System;
using System.Collections.Concurrent;

namespace PlainBytes.Mediation.Mediator
{
    /// <summary>
    /// Defines extensions for the <see cref="Type"/> type.
    /// </summary>
    internal static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, string> FormattedNameCache = new();

        /// <summary>
        /// Returns the name of the type, if generic than with the generic types.
        /// </summary>
        /// <typeparam name="T">Source type.</typeparam>
        public static string GetFormattedName<T>() => GetFormattedName(typeof(T));

        /// <summary>
        /// Returns the name of the type, if generic than with the generic types.
        /// </summary>
        /// <param name="type">Source type.</param>
        public static string GetFormattedName(this Type type)
        {
            return FormattedNameCache.GetOrAdd(type, static t =>
            {
                if (t.IsGenericType)
                {
                    var arguments = t.GetGenericArguments();
                    var trimmed = $"`{arguments.Length}".ToArray();
                    return $"{t.Name.TrimEnd(trimmed)}<{string.Join(',', arguments.Select(x => x.GetFormattedName()))}>";
                }
                return t.Name;
            });
        }
    }
}