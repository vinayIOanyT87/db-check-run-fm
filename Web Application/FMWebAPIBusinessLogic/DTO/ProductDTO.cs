using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.DTO
{
    public class ProductDTO
    {
        public string ID { get; set; }
        public int VolumeDecimalPlaces { get; set; }
        public int TemperatureDecimalPlaces { get; set; }
        public int DensityDecimalPlaces { get; set; }
    }
}
