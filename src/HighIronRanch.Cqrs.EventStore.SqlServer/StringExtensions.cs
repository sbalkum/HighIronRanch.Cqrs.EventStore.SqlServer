using System;
using System.Collections.Generic;
using System.Linq;

namespace HighIronRanch.Cqrs.EventStore.SqlServer
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> strings, string separator)
        {
            var result = strings.Aggregate(String.Empty, (current, s) => current + (s + separator));
            result = result.TrimEnd(separator.ToCharArray());
            return result;
        }
    }
}