using System;

namespace InProcLogging
{
    public class ArrayCompare
    {
        public static bool ArraysEqual<T>(T[] array1, T[] array2) where T : IComparable
        {
            if (array1.Length == array2.Length)
            {
                for (int i = 0; i < array1.Length; ++i)
                {
                    if (array1[i].CompareTo(array2[i]) != 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
