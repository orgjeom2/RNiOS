using System.Collections.Generic;

namespace CarHunters.Core.Common.Extensions
{
    public static class BinarySearchExtensions
    {
        public static int BinarySearch<T>(this IList<T> items, T value, IComparer<T> comparer)
        {
            return BinarySearch(items, 0, items.Count, value, comparer);
        }

        static int BinarySearch<T>(IList<T> items, int startIndex, int length, T value, IComparer<T> comparer)
        {
            int start = startIndex;
            int end = startIndex + length - 1;
            while (start <= end)
            {
                int mid = start + ((end - start) >> 1);
                int result = comparer.Compare(items[mid], value);
                if (result == 0)
                {
                    return ~mid;
                }
                if (result < 0)
                {
                    start = mid + 1;
                }
                else
                {
                    end = mid - 1;
                }
            }
            return ~start;
        }
    }
}
