using System;
using System.Collections.Generic;

namespace CsIRC.Utils
{
    /// <summary>
    /// Extensions of basic lists.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Returns an item at the given index and then removes that item from the list.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="list">The list to pop the element from.</param>
        /// <param name="index">The zero-based index of the element to pop.</param>
        /// <returns>The item that has been removed from the list.</returns>
        public static T Pop<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
                throw new ArgumentOutOfRangeException("Index is less than 0.-or-index is equal to or greater than System.Collections.Generic.List`1.Count.");

            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
    }
}
