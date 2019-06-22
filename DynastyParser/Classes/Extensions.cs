using System;
using System.Collections.Generic;

namespace DynastyParser.Classes
{
    public static class Extensions
    {
        public static T[] SkipLast<T>(this T[] array, int skip)
        {
            List<T> outList = new List<T>();
            for (int i = 0; i < array.Length - skip; i++)
            {
                outList.Add(array[i]);
            }
            return outList.ToArray();
        }

        public static List<T> SkipLast<T>(this List<T> list, int skip)
        {
            List<T> outList = new List<T>();
            for (int i = 0; i < list.Count - skip; i++)
            {
                outList.Add(list[i]);
            }
            return outList;
        }

        public static string Join(this string[] array, string separator)
        {
            return string.Join(separator, array);
        }

        public static string Join(this List<string> array, string separator)
        {
            return string.Join(separator, array);
        }
    }
}
