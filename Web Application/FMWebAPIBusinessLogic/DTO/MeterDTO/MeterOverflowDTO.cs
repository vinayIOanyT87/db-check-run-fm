using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.DTO.MeterDTO
{
    public class MeterOverflowDTO
    {
        public bool MeterOverflowed { get; set; }
        public double Difference { get; set; }
        public int NumberOfDigitsInMeter { get; set; }
    }
}
