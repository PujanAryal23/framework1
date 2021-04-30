using System.Collections.Generic;

namespace UIAutomation.Framework.Utils
{
    public static class SortingAlgorithm
    {
        /// <summary>
        /// Check whether values are in ascending order or not
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsInAscendingOrder<T>(this List<T> values)
        {
            for (int i = 0; i < values.Count - 1; i++)
            {
                if (Comparer<T>.Default.Compare(values[i],values[i + 1]) > 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check whether values are in descending order or not
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsInDescendingOrder<T>(this List<T> values)
        {
            for (int i = 0; i < values.Count - 1; i++)
            {
                if (Comparer<T>.Default.Compare(values[i], values[i + 1]) < 0)
                    return false;
            }
            return true;
        }
    }
}
