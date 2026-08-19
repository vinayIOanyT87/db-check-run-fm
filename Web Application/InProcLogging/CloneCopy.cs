using System;
using System.Collections.Generic;
using System.Text;

namespace InProcLogging
{
    public class CloneCopy
    {
        public static T[] CloneArray<T>(T[] array) where T : System.ICloneable
        {
            T[] ret = null;
            if (array != null)
            {
                ret = new T[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    ret[i] = (T)array[i].Clone();
                }
            }
            return ret;
        }

        public static T[] CopyArray<T>(T[] array)
        {
            T[] ret = null;
            if (array != null)
            {
                ret = new T[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    ret[i] = (T)array[i];
                }
            }
            return ret;
        }
    }
}
