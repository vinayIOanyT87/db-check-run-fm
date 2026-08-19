/// <summary>
///   File name:	LedgerQueryBase.cs
///   Purpose:	   The purpose of this class is to return ledger vertical data queries and results.
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
///	2010-05-28		W.Gray 					Revised to improve performance (WI 14681)
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

public class LedgerQueryBase
{
	#region Protected data members
	protected const int TRANSACTION_STATUS_SUSPENCE = 15;
	protected double volumeConversionFactor;
	protected int volumeDecimalPlaces;
	protected double massConversionFactor;
	protected int massDecimalPlaces;
	protected double currencyFactor;
	protected int currencyDecimalPlaces;
	protected double volumePackageSize;
	protected double massPackageSize;
	protected bool loadByWeight;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default for the Ledger Query Base class.
	/// </summary>
	public LedgerQueryBase(double VolumeConversionFactor, int VolumeDecimalPlaces,
							double MassConversionFactor, int MassDecimalPlaces,
							double CurrencyFactor, int CurrencyDecimalPlaces,
							double VolumePackageSize, double MassPackageSize, bool LoadByWeight)
	{
		this.volumeConversionFactor = VolumeConversionFactor;
		this.volumeDecimalPlaces = VolumeDecimalPlaces;
		this.massConversionFactor = MassConversionFactor;
		this.massDecimalPlaces = MassDecimalPlaces;
		this.currencyFactor = CurrencyFactor;
		this.currencyDecimalPlaces = CurrencyDecimalPlaces;
		this.volumePackageSize = VolumePackageSize;
		this.massPackageSize = MassPackageSize;
		this.loadByWeight = LoadByWeight;
	}
	#endregion

	#region Public methods
	/// <summary>
	/// This method returns an SQL string containing the SQL used 
	/// to retrieve the transactional data for computing the ledger.
	/// </summary>
	/// <returns></returns>
	public virtual string CreateLineItemSqlStatement(int managerIndex, int ownerIndex, int tankIndex)
	{
		string select = "SELECT CONVERT(Char(10), t.InventoryDate, 111) as InventoryDate, " +
							 "t.AliasName, " +
							 "l.GrossQuantity AS GrossQuantity, " +
							 "l.ProductPrice, " +
							 "l.NetQuantity AS NetQuantity, " +
							 "l.MassQuantity AS MassQuantity, " +
							 "t.Site, " +
							 "t.TransTypeID, " +
							 "l.Number01, " +
							 "l.Number02, " +
							 "l.Number03, " +
							 "l.Number04, " +
							 "l.Number05, " +
							 "l.Number06, " +
							 "t.ErrorFlag, " +
							 "t.ReversalType, " +
							 "t.TransVersion ";
		string from = "FROM tblTransactionLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransIndex = t.TransIndex ";
		string where = "WHERE t.SiteIndex = @SiteIndex " +
							"AND (t.InventoryDate BETWEEN @BeginDate AND @EndDate) " +
							"AND t.DeleteFlag = cast(0 as bit) " +
							"AND (l.ProductIndex = @ProductIndex " +
							"OR l.ProductIndex IN (SELECT ProductIndex FROM tblProducts	WHERE TrackingProductIndex = @ProductIndex AND ProductIndex IN (SELECT [Index] FROM tblEntityToSiteMap WHERE SiteIndex = @SelectedSiteIndex))) " +
							"AND ISNULL(l.Quality, 1) = 1 " +
							"AND t.TransactionStatus <> " + TRANSACTION_STATUS_SUSPENCE.ToString() + " " +
							"AND t.AliasIndex IN (SELECT B.AliasID FROM (SELECT AliasID FROM [dbo].[udf_AliasList](@LoginSiteGuid, @SelectedSiteGuid)) B) " +
							"AND EXISTS (SELECT A.CompanyIndex FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesIndex](@LoginSiteGuid, @SelectedSiteGuid, @UserGuid)) A  " +
							"WHERE A.CompanyIndex IN (0, t.ShipToIndex, t.SupplierIndex, t.ShipperIndex, t.OwnerIndex, " +
							"t.ManagerIndex, t.CarrierIndex, t.BillToIndex)) ";

		if (ownerIndex > 0)
		{
			where = where + "AND (t.OwnerIndex = @OwnerIndex OR t.OwnerIndex IS NULL)";
		}

		if (managerIndex > 0)
		{
			where = where + "AND t.ManagerIndex = @ManagerIndex ";
		}

		if (tankIndex > 0)
		{
			where = where + "AND l.StorageLocationIndex = @TankIndex ";
		}

		return (select + from + where);
	}

	/// <summary>
	/// This method returns an SQL string containing the SQL used 
	/// to retrieve the transactional sub-line item data for computing the ledger.
	/// </summary>
	/// <returns></returns>
	public virtual string CreateSubLineItemSqlStatement(int managerIndex, int ownerIndex, int tankIndex)
	{
		string select = "SELECT CONVERT(Char(10), t.InventoryDate, 111) as InventoryDate, " +
							 "t.AliasName, " +
							 "l.GrossQuantity AS GrossQuantity, " +
							 "CAST(0.0 AS FLOAT) AS ProductPrice, " +
							 "l.NetQuantity AS NetQuantity, " +
							 "l.MassQuantity AS MassQuantity, " +
							 "t.Site, " +
							 "t.TransTypeID, " +
							 "CAST(0.0 AS FLOAT) AS Number01, " +
							 "CAST(0.0 AS FLOAT) AS Number02, " +
							 "CAST(0.0 AS FLOAT) AS Number03, " +
							 "CAST(0.0 AS FLOAT) AS Number04, " +
							 "CAST(0.0 AS FLOAT) AS Number05, " +
							 "CAST(0.0 AS FLOAT) AS Number06, " +
							 "t.ErrorFlag, " +
							 "t.ReversalType," +
							 "t.TransVersion ";
		string from = "FROM tblTransactionSubLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransIndex = t.TransIndex ";
		string where = "WHERE t.SiteIndex = @SiteIndex " +
							"AND (t.InventoryDate BETWEEN @BeginDate AND @EndDate) " +
							"AND t.DeleteFlag = cast(0 as bit) " +
							"AND (l.ProductIndex = @ProductIndex " +
							"OR l.ProductIndex IN (SELECT ProductIndex FROM tblProducts	WHERE TrackingProductIndex = @ProductIndex AND ProductIndex IN (SELECT [Index] FROM tblEntityToSiteMap WHERE SiteIndex = @SelectedSiteIndex))) " +
							"AND ISNULL(l.Quality, 1) = 1 " +
							"AND t.TransactionStatus <> " + TRANSACTION_STATUS_SUSPENCE.ToString() + " " +
							"AND t.AliasIndex IN (SELECT B.AliasID FROM (SELECT AliasID FROM [dbo].[udf_AliasList](@LoginSiteGuid, @SelectedSiteGuid)) B) " +
							"AND EXISTS (SELECT A.CompanyIndex FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesIndex](@LoginSiteGuid, @SelectedSiteGuid, @UserGuid)) A  " +
							"WHERE A.CompanyIndex IN (0, t.ShipToIndex, t.SupplierIndex, t.ShipperIndex, t.OwnerIndex, " +
							"t.ManagerIndex, t.CarrierIndex, t.BillToIndex)) ";

		if (ownerIndex > 0)
		{
			where = where + "AND (t.OwnerIndex = @OwnerIndex OR t.OwnerIndex IS NULL)";
		}

		if (managerIndex > 0)
		{
			where = where + "AND t.ManagerIndex = @ManagerIndex ";
		}

		if (tankIndex > 0)
		{
			where = where + "AND l.StorageLocationIndex = @TankIndex ";
		}

		return (select + from + where);
	}

	/// <summary>
	/// This method will return a sorted list of the transaction information
	/// to compute a ledger. This method will sum up all the transactions for
	/// a given day and alias combination.
	/// </summary>
	/// <param name="dataSet"></param>
	/// <returns></returns>
	public virtual SortedList SumAndGroupData(DataSet dataSet)
	{
		SortedList inventorySummation = new SortedList();


		if ((dataSet != null) && (dataSet.Tables.Count > 0))
		{
			DataTable table = dataSet.Tables[0];
			InventoryDailyAliasDO invDailyAlias = null;
			string key = "";
			DataRow row = null;

			if (table.Rows.Count > 0)
			{
				for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
				{
					row = table.Rows[rowIndex];

					string inventoryDate = (row.IsNull("InventoryDate")) ? "" : (string)row["InventoryDate"];
					string aliasName = (row.IsNull("AliasName")) ? "" : (string)row["AliasName"];
					string site = (row.IsNull("Site")) ? "" : (string)row["Site"];
					string reversalType = (row.IsNull("ReversalType")) ? "" : (string)row["ReversalType"];
					Int64 transVersion = (row.IsNull("TransVersion")) ? 0 : (Int64)row["TransVersion"];
					string stransTypeID = (row.IsNull("TransTypeID")) ? "0" : row["TransTypeID"].ToString();
					int transTypeID = Convert.ToInt32(stransTypeID);

					double gross = (row.IsNull("GrossQuantity")) ? 0.0 : (double)row["GrossQuantity"];
					double net = (row.IsNull("NetQuantity")) ? 0.0 : (double)row["NetQuantity"];
					double mass = (row.IsNull("MassQuantity")) ? 0.0 : (double)row["MassQuantity"];
					double price = (row.IsNull("ProductPrice")) ? 0.0 : (double)row["ProductPrice"];
					double number01 = (row.IsNull("Number01")) ? 0.0 : (double)row["Number01"];
					double number02 = (row.IsNull("Number02")) ? 0.0 : (double)row["Number02"];
					double number03 = (row.IsNull("Number03")) ? 0.0 : (double)row["Number03"];
					double number04 = (row.IsNull("Number04")) ? 0.0 : (double)row["Number04"];
					double number05 = (row.IsNull("Number05")) ? 0.0 : (double)row["Number05"];
					double number06 = (row.IsNull("Number06")) ? 0.0 : (double)row["Number06"];
					bool errorFlag = (row.IsNull("ErrorFlag")) ? false : (bool)row["ErrorFlag"];

					// Must have an inventory date and alias name.
					if ((inventoryDate == "") || (aliasName == ""))
					{
						continue;
					}

					// This key will be sorted by the SortedList on inventory date and alias name.
					key = inventoryDate + "|" + aliasName;

					if (inventorySummation.Contains(key) == true)
					{
						invDailyAlias = (InventoryDailyAliasDO)inventorySummation[key];

						invDailyAlias.InventoryDateStr = inventoryDate;
						invDailyAlias.AliasName = aliasName;
						invDailyAlias.Site = site;
						invDailyAlias.TransTypeID = transTypeID;

						if (transVersion > invDailyAlias.MaxTransVersion)
							invDailyAlias.MaxTransVersion = transVersion;

						invDailyAlias.SumGross(gross);
						invDailyAlias.SumNet(net);
						invDailyAlias.SumMass(mass);
						invDailyAlias.SumGrossPrice(price, gross);
						invDailyAlias.SumNetPrice(price, net);
						invDailyAlias.SumMassPrice(price, mass);
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
						invDailyAlias.AliasName = aliasName;
						invDailyAlias.Site = site;
						invDailyAlias.TransTypeID = transTypeID;

						if (transVersion > invDailyAlias.MaxTransVersion)
							invDailyAlias.MaxTransVersion = transVersion;

						invDailyAlias.SumGross(gross);
						invDailyAlias.SumNet(net);
						invDailyAlias.SumMass(mass);
						invDailyAlias.SumGrossPrice(price, gross);
						invDailyAlias.SumNetPrice(price, net);
						invDailyAlias.SumMassPrice(price, mass);
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
