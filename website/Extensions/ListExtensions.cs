using System.Collections.Generic;


namespace LiquidSC.Foundation.RedirectManager.Extensions
{
    public static class ListExtensions
    { 
        public static void AddRangeIfNew<T>(this IList<T> self, IEnumerable<T> items)
        {
            foreach (var item in items)
                if (!self.Contains(item))
                    self.Add(item);
        }
    }
}
