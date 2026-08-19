namespace LedgerCore
{
	public class LRLedgerModQuery : LRLedgerQueryBase
	{
		#region Constructors
		/// <summary>
		/// This is the default for the Ledger Standard Query class.
		/// </summary>
		public LRLedgerModQuery(double volumeConversionFactor,
								int volumeDecimalPlaces,
								double massConversionFactor,
								int massDecimalPlaces,
								double currencyFactor,
								int currencyDecimalPlaces,
								double volumePackageSize,
								double massPackageSize,
								bool loadByWeight,
								LRTransactionAliasListDO transAliasListDo)
			: base(volumeConversionFactor,
					volumeDecimalPlaces,
					massConversionFactor,
					massDecimalPlaces,
					currencyFactor,
					currencyDecimalPlaces,
					volumePackageSize,
					massPackageSize,
					loadByWeight,
					transAliasListDo)
		{
		}
		#endregion
	}
}