namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Data;

	public class LRLedgerADFQuery : LRLedgerQueryBase
	{
		#region Constructors
		/// <summary>
		/// This is the default for the Ledger Standard Query class.
		/// </summary>
		public LRLedgerADFQuery(double volumeConversionFactor,
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

		#region Override methods

		/// <summary>
		/// This method will return a sorted list of the transaction information
		/// to compute a ledger. This method will sum up all the transactions for
		/// a given day and alias combination.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public override SortedList SumAndGroupData(DataSet dataSet)
		{
			var inventorySummation = new SortedList();
			const int T8Receipt = 8;

			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];
				LRInventoryDailyAliasDO invDailyAlias = null;
				string key = "";
				DataRow row = null;

				if (table.Rows.Count > 0)
				{
					for (int rowIndex = 0; rowIndex < table.Rows.Count; ++rowIndex)
					{
						row = table.Rows[rowIndex];

						string inventoryDate = (row.IsNull("InventoryDate")) ? "" : (string) row["InventoryDate"];
						string aliasName	 = (row.IsNull("AliasName")) ? "" : (string) row["AliasName"];
						Guid transAliasGuid  = (row.IsNull("TransactionAliasGuid")) ? Guid.Empty : (Guid)row["TransactionAliasGuid"];
						string site			 = (row.IsNull("Site")) ? "" : (string) row["Site"];
						string reversalType  = (row.IsNull("ReversalType")) ? "" : (string) row["ReversalType"];
						Int64 transVersion	 = (row.IsNull("TransVersion")) ? 0 : (Int64) row["TransVersion"];
						string stransTypeID  = (row.IsNull("LookupTransTypeIndex")) ? "0" : row["LookupTransTypeIndex"].ToString();
						int transTypeID		 = Convert.ToInt32(stransTypeID);

						double gross	= (row.IsNull("GrossQuantity")) ? 0.0 : (double) row["GrossQuantity"];
						double net		= (row.IsNull("NetQuantity")) ? 0.0 : (double) row["NetQuantity"];
						double mass		= (row.IsNull("MassQuantity")) ? 0.0 : (double) row["MassQuantity"];
						double price	= (row.IsNull("ProductPrice")) ? 0.0 : (double) row["ProductPrice"];
						double number01 = (row.IsNull("Number01")) ? 0.0 : (double) row["Number01"];
						double number02 = (row.IsNull("Number02")) ? 0.0 : (double) row["Number02"];
						double number03 = (row.IsNull("Number03")) ? 0.0 : (double) row["Number03"];
						double number04 = (row.IsNull("Number04")) ? 0.0 : (double) row["Number04"];
						double number05 = (row.IsNull("Number05")) ? 0.0 : (double) row["Number05"];
						double number06 = (row.IsNull("Number06")) ? 0.0 : (double) row["Number06"];
						bool errorFlag	= (row.IsNull("ErrorFlag")) ? false : (bool) row["ErrorFlag"];

						// Must have an inventory date and alias name.
						if (string.IsNullOrEmpty(inventoryDate) || 
							string.IsNullOrEmpty(aliasName) ||
							string.IsNullOrEmpty(site))
						{
							continue;
						}

						// Find the configured transaction alias name if it differs from the
						// transaction record alias name.
						aliasName = base.FindConfiguredAliasName(aliasName, transAliasGuid);

						// This key will be sorted by the SortedList on inventory date and alias name.
						key = inventoryDate + "|" + aliasName;

						if (inventorySummation.Contains(key) == true)
						{
							invDailyAlias = (LRInventoryDailyAliasDO) inventorySummation[key];

							invDailyAlias.InventoryDateStr	= inventoryDate;
							invDailyAlias.AliasName			= aliasName;
							invDailyAlias.Site				= site;
							invDailyAlias.TransTypeID		= transTypeID;
							invDailyAlias.ReversalType		= reversalType;

							if (transVersion > invDailyAlias.MaxTransVersion)
							{
								invDailyAlias.MaxTransVersion = transVersion;
							}

							invDailyAlias.SumGross(gross);
							invDailyAlias.SumNet(net);
							invDailyAlias.SumMass(mass);

							if (transTypeID == T8Receipt)
							{
								invDailyAlias.SumNetPrice(number06, net);
								invDailyAlias.SumGrossPrice(number06, gross);
								invDailyAlias.SumMassPrice(number06, mass);
							}
							else
							{
								invDailyAlias.SumGrossPrice(price, gross);
								invDailyAlias.SumNetPrice(price, net);
								invDailyAlias.SumMassPrice(price, mass);
							}

							invDailyAlias.SumNumberField(number01, 1);
							invDailyAlias.SumNumberField(number02, 2);
							invDailyAlias.SumNumberField(number03, 3);
							invDailyAlias.SumNumberField(number04, 4);
							invDailyAlias.SumNumberField(number05, 5);
							invDailyAlias.SumNumberField(number06, 6);
							invDailyAlias.OrErrorFlag(errorFlag);
						}
						else
						{
							invDailyAlias = new LRInventoryDailyAliasDO(this.volumeConversionFactor,
																		this.massConversionFactor,
																		this.currencyFactor,
																		this.volumeDecimalPlaces,
																		this.massDecimalPlaces,
																		this.currencyDecimalPlaces,
																		this.volumePackageSize,
																		this.massPackageSize,
																		this.loadByWeight)
							                {
								                InventoryDateStr = inventoryDate,
								                AliasName		 = aliasName,
								                Site			 = site,
								                TransTypeID		 = transTypeID
							                };

							if (transVersion > invDailyAlias.MaxTransVersion)
								invDailyAlias.MaxTransVersion = transVersion;

							invDailyAlias.SumGross(gross);
							invDailyAlias.SumNet(net);

							if (transTypeID == T8Receipt)
							{
								invDailyAlias.SumGrossPrice(number06, gross);
								invDailyAlias.SumNetPrice(number06, net);
							}
							else
							{
								invDailyAlias.SumGrossPrice(price, gross);
								invDailyAlias.SumNetPrice(price, net);
							}

							invDailyAlias.SumNumberField(number01, 1);
							invDailyAlias.SumNumberField(number02, 2);
							invDailyAlias.SumNumberField(number03, 3);
							invDailyAlias.SumNumberField(number04, 4);
							invDailyAlias.SumNumberField(number05, 5);
							invDailyAlias.SumNumberField(number06, 6);
							invDailyAlias.OrErrorFlag(errorFlag);

							inventorySummation.Add(key, invDailyAlias);
						}
					}
				}
			}

			return inventorySummation;
		}
		#endregion
	}
}