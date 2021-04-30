using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Service.Support.Utils
{
    public static class SortingAlgorithm
    {
        #region ASCENDING ORDER

        /// <summary>
        /// Check whether values are in ascending order or not
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsInAscendingOrder<T>(this List<T> values)
        {
            return UIAutomation.Framework.Utils.SortingAlgorithm.IsInAscendingOrder(values);
        } 

        #endregion

        #region DESCENDING ORDER

        /// <summary>
        /// Check whether values are in descending order or not
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsInDescendingOrder<T>(this List<T> values)
        {
            return UIAutomation.Framework.Utils.SortingAlgorithm.IsInDescendingOrder(values);
        } 

        #endregion
    }
}
