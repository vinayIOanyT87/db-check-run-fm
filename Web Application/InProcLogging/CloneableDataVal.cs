using System;
using System.Collections.Generic;
using System.Text;

namespace InProcLogging
{
    public class CloneableDataVal<T> : ICloneable
    {
        private T dataVal;

        public T DataVal
        {
            get { return dataVal; }
            set { dataVal = value; }
        }

        public CloneableDataVal(T aDataVal)
        {
            dataVal = aDataVal;
        }

        public CloneableDataVal()
        {
        }

        public object Clone()
        {
            CloneableDataVal<T> ret = new CloneableDataVal<T>(dataVal);
            return ret;
        }

        public override string ToString()
        {
            return DataVal.ToString();
        }

    }
}
