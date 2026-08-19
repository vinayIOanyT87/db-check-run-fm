using System.Collections.Generic;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.DTO.TransactionDTO
{
    public class TransactionDetailsDTO
    {
        public IEnumerable<TransactionAliasFieldClassWithColumn> TransactionFields { get; set; }

        public bool AutoDocumentNumber { get; set; }

        public IEnumerable<FieldWithAssociatedList> FieldsWithLists { get; set; }
        public int VolumeDecimalPrecision { get; set; }
        public int TemperatureDecimalPlaces { get; set; }
        public int DensityDecimalPlaces { get; set; }
        public IEnumerable<ProductDTO> AllProducts { get; set; }
        public TransactionTypes TransactionAliasType { get; set; }
    }
}
