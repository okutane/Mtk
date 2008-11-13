using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Common.Utilities
{
    public static class UArray
    {
        public static T[] Prepend<T>(this T[] array, T item)
        {
            T[] result = new T[array.Length + 1];
            result[0] = item;
            Array.Copy(array, 0, result, 1, array.Length);
            return result;
        }
    }
}
