using System;
using System.Collections.Generic;
using System.Text;

namespace InProcLogging
{
    public class CircularBuffer<T>
    {
        private T[] mBuff = null;
        private int startIndex = 0;
        private int endIndex = 0;
        private bool isEmpty = true;

        public T this[int index]
        {
            get
            {
                if (mBuff == null)
                {
                    return default(T);
                }
                if (index >= mBuff.Length)
                {
                    throw new Exception("Index Out Of Range");
                }
                return mBuff[index];
            }
        }

        public int Length
        {
            get
            {
                if (mBuff == null)
                {
                    return 0;
                }
                return mBuff.Length;
            }
            set
            {
                if (value < 1)
                {
                    mBuff = null;
                    startIndex = 0;
                    endIndex = 0;
                    isEmpty = true;
                    return;
                }
                if (mBuff == null)
                {
                    mBuff = new T[value];
                    startIndex = 0;
                    endIndex = 0;
                    isEmpty = true;
                    return;
                }
                if (mBuff.Length == value)
                {
                    return;
                }
                T[] temp = new T[value];
                int i = 0;
                if (IsEmpty == false)
                {
                    while (IsEmpty == false && i < value)
                    {
                        temp[i] = RemoveFromBack();
                        i++;
                    }
                    mBuff = temp;
                    startIndex = i - 1;
                    endIndex = 0;
                    incrStart();
                }
                else
                {
                    mBuff = temp;
                }
                return;
            }
        }

        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; }
        }

        public bool IsFull
        {
            get
            {
                if (isEmpty == true)
                {
                    return false;
                }
                int tempStartIndex = startIndex;
                tempStartIndex++;
                if (tempStartIndex >= mBuff.Length)
                {
                    tempStartIndex = 0;
                }
                if (tempStartIndex == endIndex)
                {
                    return true;
                }
                return false;
            }
        }

        private void incrStart()
        {
            startIndex++;
            if (startIndex >= mBuff.Length)
            {
                startIndex = 0;
            }
            if (startIndex == endIndex)
            {
                endIndex++;
                if (endIndex >= mBuff.Length)
                {
                    endIndex = 0;
                }
            }
            isEmpty = false;
        }
        private void incrEnd()
        {
            if (isEmpty == false)
            {
                endIndex++;
                if (endIndex >= mBuff.Length)
                {
                    endIndex = 0;
                }
                if (startIndex == endIndex)
                {
                    isEmpty = true;
                }
            }

        }
        public CircularBuffer(int aDepth)
        {
            mBuff = new T[aDepth + 1];
        }
        public void Add(T aElement)
        {
            mBuff[startIndex] = aElement;
            incrStart();
        }
        public T RemoveFromBack()
        {
            if (isEmpty)
            {
                throw new Exception("Circular Queue Empty");
            }
            T ret = mBuff[endIndex];
            incrEnd();
            return ret;
        }
    }
}
