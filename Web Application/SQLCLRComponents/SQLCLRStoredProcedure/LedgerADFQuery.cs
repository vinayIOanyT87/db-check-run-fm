/// <summary>
///   File name:	LedgerADFQuery.cs
///   Purpose:	   The purpose of this class is to return ledger vertical data queries and results
///               for the ADF project.
///				
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///	yyyy-mm-dd		developer's name 		reason for the change
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

public class LedgerADFQuery:LedgerQueryBase
{
	#region Constructors
	/// <summary>
	/// This is the default for the Ledger Standard Query class.
	/// </summary>
	public LedgerADFQuery(	double volumeConversionFactor,
									int volumeDecimalPlaces,
									double massConversionFactor,
									int massDecimalPlaces,
									double currencyFactor,
									int currencyDecimalPlaces,
									double volumePackageSize,
									double massPackageSize,
									bool loadByWeight)
		: base(volumeConversionFactor,volumeDecimalPlaces,
					massConversionFactor,massDecimalPlaces,
					currencyFactor,currencyDecimalPlaces,
					volumePackageSize,massPackageSize,loadByWeight)
	{
	}
	#endregion

	#region Override methods

	/// <summary>
	/// This method will return a sorted list of the transaction information
	/// to compute a ledger. This method will sum up all the transactions for
	/// a given day and alias combination.
	/// </summary>
	/// <param name="dataSets"></param>
	/// <returns></returns>
	public override SortedList SumAndGroupData(DataSet dataSet)
	{
		SortedList inventorySummation = new SortedList();


		int T8_Receipt = 8;

		if((dataSet != null) && (dataSet.Tables.Count > 0))
		{
			DataTable table = dataSet.Tables[0];
			InventoryDailyAliasDO invDailyAlias = null;
			string key = "";
			DataRow row = null;

			if(table.Rows.Count > 0)
			{
				for(int rowIndex = 0;rowIndex < table.Rows.Count;++rowIndex)
				{
					row = table.Rows[rowIndex];

					string inventoryDate = (row.IsNull("InventoryDate")) ? ""  : (string)row["InventoryDate"];
					string aliasName     = (row.IsNull("AliasName"))     ? ""  : (string)row["AliasName"];
					string site          = (row.IsNull("Site"))          ? ""  : (string)row["Site"];
					string reversalType  = (row.IsNull("ReversalType"))  ? ""  : (string)row["ReversalType"];
					Int64 transVersion	= (row.IsNull("TransVersion"))  ? 0 : (Int64)row["TransVersion"];
					string stransTypeID  = (row.IsNull("TransTypeID"))   ? "0" : row["TransTypeID"].ToString();
					int transTypeID      = Convert.ToInt32(stransTypeID);

					double gross    = (row.IsNull("GrossQuantity")) ? 0.0   : (double)row["GrossQuantity"];
					double net      = (row.IsNull("NetQuantity"))   ? 0.0   : (double)row["NetQuantity"];
					double mass		 = (row.IsNull("MassQuantity"))  ? 0.0   : (double)row["MassQuantity"];
					double price    = (row.IsNull("ProductPrice"))  ? 0.0   : (double)row["ProductPrice"];
					double number01 = (row.IsNull("Number01"))      ? 0.0   : (double)row["Number01"];
					double number02 = (row.IsNull("Number02"))      ? 0.0   : (double)row["Number02"];
					double number03 = (row.IsNull("Number03"))      ? 0.0   : (double)row["Number03"];
					double number04 = (row.IsNull("Number04"))      ? 0.0   : (double)row["Number04"];
					double number05 = (row.IsNull("Number05"))      ? 0.0   : (double)row["Number05"];
					double number06 = (row.IsNull("Number06"))      ? 0.0   : (double)row["Number06"];
					bool errorFlag  = (row.IsNull("ErrorFlag"))     ? false : (bool)row["ErrorFlag"];

					// Must have an inventory date and alias name.
					if((inventoryDate == "") || (aliasName == ""))
					{
						continue;
					}

					// This key will be sorted by the SortedList on inventory date and alias name.
					key = inventoryDate + "|" + aliasName;

					if(inventorySummation.Contains(key) == true)
					{
						invDailyAlias = (InventoryDailyAliasDO)inventorySummation[key];

						invDailyAlias.InventoryDateStr = inventoryDate;
						invDailyAlias.AliasName        = aliasName;
						invDailyAlias.Site             = site;
						invDailyAlias.TransTypeID      = transTypeID;
						invDailyAlias.ReversalType     = reversalType;

						if(transVersion > invDailyAlias.MaxTransVersion)
							invDailyAlias.MaxTransVersion=transVersion;

						invDailyAlias.SumGross(gross);
						invDailyAlias.SumNet(net);
						invDailyAlias.SumMass(mass);

						if(transTypeID == T8_Receipt)
						{
							invDailyAlias.SumNetPrice(number06,net);
							invDailyAlias.SumGrossPrice(number06,gross);
							invDailyAlias.SumMassPrice(number06,mass);
						}
						else
						{
							invDailyAlias.SumGrossPrice(price,gross);
							invDailyAlias.SumNetPrice(price,net);
							invDailyAlias.SumMassPrice(price,mass);
						}

						invDailyAlias.SumNumberField(number01,1);
						invDailyAlias.SumNumberField(number02,2);
						invDailyAlias.SumNumberField(number03,3);
						invDailyAlias.SumNumberField(number04,4);
						invDailyAlias.SumNumberField(number05,5);
						invDailyAlias.SumNumberField(number06,6);
						invDailyAlias.OrErrorFlag(errorFlag);
					}
					else
					{
						invDailyAlias = new InventoryDailyAliasDO(this.volumeConversionFactor,
															this.massConversionFactor,
																				this.currencyFactor,
																				this.volumeDecimalPlaces,
																				this.massDecimalPlaces,
																				this.currencyDecimalPlaces,
																				this.volumePackageSize,
																				this.massPackageSize,
																				this.loadByWeight);

						invDailyAlias.InventoryDateStr = inventoryDate;
						invDailyAlias.AliasName        = aliasName;
						invDailyAlias.Site             = site;
						invDailyAlias.TransTypeID      = transTypeID;

						if(transVersion > invDailyAlias.MaxTransVersion)
							invDailyAlias.MaxTransVersion=transVersion;

						invDailyAlias.SumGross(gross);
						invDailyAlias.SumNet(net);

						if(transTypeID == T8_Receipt)
						{
							invDailyAlias.SumGrossPrice(number06,gross);
							invDailyAlias.SumNetPrice(number06,net);
						}
						else
						{
							invDailyAlias.SumGrossPrice(price,gross);
							invDailyAlias.SumNetPrice(price,net);
						}

						invDailyAlias.SumNumberField(number01,1);
						invDailyAlias.SumNumberField(number02,2);
						invDailyAlias.SumNumberField(number03,3);
						invDailyAlias.SumNumberField(number04,4);
						invDailyAlias.SumNumberField(number05,5);
						invDailyAlias.SumNumberField(number06,6);
						invDailyAlias.OrErrorFlag(errorFlag);

						inventorySummation.Add(key,invDailyAlias);
					}
				}
			}
		}

		return inventorySummation;
	}
	#endregion
}
