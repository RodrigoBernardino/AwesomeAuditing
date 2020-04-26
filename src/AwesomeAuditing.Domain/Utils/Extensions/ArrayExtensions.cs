using System;

namespace AwesomeAuditing.Domain.Utils.Extensions
{
    internal static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] arrCopy = new T[length];
            Array.Copy(data, index, arrCopy, 0, length);

            return arrCopy;
        }
    }
}
