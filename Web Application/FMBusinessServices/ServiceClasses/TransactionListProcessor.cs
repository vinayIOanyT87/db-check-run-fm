/// <summary>
/// File name:	TransactionListProcessor.cs
/// Purpose:	To decipher the request to retrieve the transaction list
///				data object.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		2006-08-08	Richard Panachida		Changed stored procedure prefix from "sp_"
///													to "fm_".
///		
///		2009-06-22	I.Orndorff				- Modified "process()" to pass 
///													  "transactionListSR.ShowDeletedTransactions". 
///													  This addresses Task 4128.
///		
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.UtilityObjects;

	public class TransactionListProcessorClass : ITransactionListProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		private TransactionListSR transactionListSR;
		private DataSet combineDataSet;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction list processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public TransactionListProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.combineDataSet = null;
		}
		#endregion

		#region Public Methods
		public TransactionListDO Process(TransactionListSR sr)
		{
			this.combineDataSet = null;
			this.transactionListSR = sr;

			// Get an empty dataset if the alias name list is empty.
			if (this.transactionListSR.AliasNames.Count <= 0)
			{
				string emptyAliasName = "";
				using (SqlCommand cmd = new SqlCommand())
				{
					this.GetNewSQL(cmd, emptyAliasName);
					this.combineDataSet = this.consolidatedDA.GetDataSet(cmd, transactionListSR.Security);
				}
			}
			else
			{
				// Get data for each of the transaction aliases in the list.
				// Combine the data set results into one data set to be returned.
				foreach (string aliasName in this.transactionListSR.AliasNames)
				{
					using (SqlCommand cmd = new SqlCommand())
					{
						this.GetNewSQL(cmd, aliasName);
						DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, transactionListSR.Security);

						this.CombineDataSets(dataSet);
					}
				}
			}

			TransactionListDO transactionListDO = new TransactionListDO();
			transactionListDO.TransactionDataSet = this.combineDataSet;
			this.TranslateDateColumns( sr.Security, transactionListDO.TransactionDataSet );

			return transactionListDO;
		}

		private void TranslateDateColumns(SecurityClass security, DataSet dataSet)
		{
			// Translate date columns to owner site time zone
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				HardwareKeyClass hwKey = new HardwareKeyClass();
				var table = dataSet.Tables[0];

				// Build list of DateTimeOffset fields
				var dateColumns = new List<int>();
				foreach (DataColumn column in table.Columns)
				{
					if (column.DataType.Name.Equals("DateTimeOffset", StringComparison.InvariantCultureIgnoreCase)
						&& (!column.ColumnName.Equals("InventoryDate", StringComparison.InvariantCultureIgnoreCase)
								|| (hwKey.IsDescKey() && column.ColumnName.Equals("InventoryDate", StringComparison.InvariantCultureIgnoreCase)
										&& this.transactionListSR.DateType == BsmeLedgerDateType.DateProcessTypes.ByCreateDate)))
					{

						if (hwKey.IsDescKey())
						{
							if (column.ColumnName.Equals("Date03") || column.ColumnName.Equals("Date04"))
							{
								//exclude these columns from datetime conversion to localsite
								continue;
							}

							if (column.ColumnName.Equals("Date01") &&
								(this.transactionListSR.AliasNames.Contains("Receive")
								|| this.transactionListSR.AliasNames.Contains("Receive - Transfer")
								|| this.transactionListSR.AliasNames.Contains("Receive - Contract")
								|| this.transactionListSR.AliasNames.Contains("Shipment")
								|| this.transactionListSR.AliasNames.Contains("Shipment - Transfer")
								|| this.transactionListSR.AliasNames.Contains("Shipment - Contract")))
							{
								continue;
							}
						}
						dateColumns.Add(column.Ordinal);
					}
				}

				if (dateColumns.Count > 0)
				{
					var sites = new SitesClass();
					var site = sites.Get(security, security.SiteGuid, false, false, false); //get once since it is the site being displayed on the screen that matters
					var timeZoneInfo = site.GetTimeZoneInfo();

					foreach (DataRow row in table.Rows)
					{
						foreach (int columnOrdinal in dateColumns)
						{
							DataColumn column = table.Columns[columnOrdinal];

							if (row.IsNull(column.ColumnName) == false)
							{
								row[column.ColumnName] = TimeConverter.ToSiteTime(timeZoneInfo, (DateTimeOffset) row[column.ColumnName]);
							}
						}
					}
				}
			}
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// This method will return the SQL based on the alias name.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="aliasName"></param>
		/// <returns></returns>
		private void GetNewSQL(SqlCommand cmd, string aliasName)
		{
			cmd.CommandText = "usp_TransactionAndLineItemList";
			cmd.CommandType = CommandType.StoredProcedure;

			var hardwareKey = new HardwareKeyClass();
			
			if (hardwareKey.IsDescKey() ||
				hardwareKey.IsDescEnterpriseKey() ||
				hardwareKey.IsDescProfessionalKey())
			{
				bool isBaseDb = !hardwareKey.IsDescEnterpriseKey();
				cmd.CommandText = "usp_BsmeTransactionAndLineItemList";
				cmd.Parameters.AddWithValue("@DateType", (int)this.transactionListSR.DateType);
				cmd.Parameters.AddWithValue("@IsBaseDb", isBaseDb);
			}

			cmd.Parameters.AddWithValue("@AliasName", aliasName);
			cmd.Parameters.AddWithValue("@NominationKey", this.transactionListSR.NominationKey);
			cmd.Parameters.AddWithValue("@BeginDate", this.transactionListSR.TransactionDate.Date);
			cmd.Parameters.AddWithValue("@EndDate", this.transactionListSR.TransactionDate.AddDays(1).Date);
			cmd.Parameters.AddWithValue("@ManagerID", this.transactionListSR.Manager);
			cmd.Parameters.AddWithValue("@OwnerID", this.transactionListSR.Owner);
			cmd.Parameters.AddWithValue("@Product", this.transactionListSR.Product);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", this.transactionListSR.Security.LoginSiteGuid);
			cmd.Parameters.AddWithValue("@SiteGuid", this.transactionListSR.Security.SiteGuid);

			if (this.transactionListSR.Security.UserGuid == Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@UserGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@UserGuid", this.transactionListSR.Security.UserGuid);
			}

			cmd.Parameters.AddWithValue("@ShowDeletedTrx", Convert.ToInt16(this.transactionListSR.ShowDeletedTransactions));
		}

		/// <summary>
		/// This method will combine each alias' data sets into one data set.
		/// </summary>
		/// <param name="inDataSet"></param>
		private void CombineDataSets(DataSet inDataSet)
		{
			if (inDataSet != null)
			{
				DataTable table = inDataSet.Tables[0];

				if (table != null)
				{
					if (this.combineDataSet == null)
					{
						this.combineDataSet = inDataSet;
					}
					else
					{
						DataTable combineTable = this.combineDataSet.Tables[0];

						foreach (DataRow row in table.Rows)
						{
							DataRow combineRow = combineTable.NewRow();

							foreach (DataColumn column in table.Columns)
							{
								if (combineTable.Columns.Contains(column.ColumnName) == true)
								{
									if ((column.DataType == typeof(int))
										|| (column.DataType == typeof(Int32)))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = (int)row[column];
										}
									}
									else if (column.DataType == typeof(Int16))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = Convert.ToInt32(row[column]);
										}
									}
									else if (column.DataType == typeof(Int64))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = Convert.ToInt64(row[column]);
										}
									}
									else if (column.DataType == typeof(float))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = (float)row[column];
										}
									}
									else if (column.DataType == typeof(double))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = (double)row[column];
										}
									}
									else if (column.DataType == typeof(string))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = (string)row[column];
										}
									}
									else if (column.DataType == typeof(bool))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = (bool)row[column];
										}
									}
									else if (column.DataType == typeof(DateTime))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = (DateTime)row[column];
										}
									}
									else if (column.DataType == typeof(DateTimeOffset))
									{
										if (row.IsNull(column) == false)
										{
											combineRow[column.ColumnName] = (DateTimeOffset)row[column];
										}
									}
									else
									{
										string whatType = column.DataType.ToString();
									}
								}
							}

							combineTable.Rows.Add(combineRow);
						}
					}
				}
			}
		}

		private DataObject delete()
		{
			return null;
		}

		private void modify()
		{
		}

		private DataObject refresh()
		{
			return null;
		}

		private void close()
		{
		}

		private DataObject nextPage()
		{
			return null;
		}
		#endregion
	}
}