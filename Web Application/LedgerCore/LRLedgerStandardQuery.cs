namespace LedgerCore
{
	using System.Collections;
	 
	public class LRLedgerStandardQuery : LRLedgerQueryBase
	{
		#region Constructors
		/// <summary>
		/// This is the default for the Ledger Standard Query class.
		/// </summary>
		public LRLedgerStandardQuery(double volumeConversionFactor,
									int volumeDecimalPlaces,
									double massConversionFactor,
									int massDecimalPlaces,
									double currencyFactor,
									int currencyDecimalPlaces,
									double volumePackageSize,
									double massPackageSize,
									bool loadByWeight,
									LRTransactionAliasListDO transAliasListDo)
			: base(	volumeConversionFactor, 
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