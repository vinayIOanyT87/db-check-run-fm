using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	public class CorrectionFactorClass
    {
        protected const int MAX_INDEX = 5;
        public double[] CorrectionFactorData = new double[MAX_INDEX];
        public double this[int Index]
        {
            get
            {
                if (Index > -1 && Index < MAX_INDEX)
                    return CorrectionFactorData[Index];
                else
                    throw new InvalidOperationException( "CorrectionFactorClass.set_Item Index Out of Range" );
            }
            set
            {
                if (Index > -1 && Index < MAX_INDEX)
                    CorrectionFactorData[Index] = value;
                else
                    throw new InvalidOperationException( "CorrectionFactorClass.set_Item Index Out of Range" );
            }
        }
    }
}
