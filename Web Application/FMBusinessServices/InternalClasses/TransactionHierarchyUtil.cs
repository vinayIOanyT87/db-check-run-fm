// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionHierarchyUtil.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	///     Summary description for TransactionHierarchyUtil.
	/// </summary>
	public class TransactionHierarchyUtil
	{
		#region Constants and Fields

		private readonly ConsolidatedDAClass consolidatedDA;

		private readonly SecurityClass security;

		private double totalReceived;

		private double totalValueReceived;

		#endregion

		#region Constructors and Destructors

		public TransactionHierarchyUtil(SecurityClass inSecurity)
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.security = inSecurity;
		}

		#endregion

		#region Public Properties

		public double TotalReceived
		{
			get
			{
				return this.totalReceived;
			}
		}

		public double TotalValueReceived
		{
			get
			{
				return this.totalValueReceived;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Calculates the total quantity received and total value received
		/// </summary>
		/// <param name="dt">
		/// The data in the DataTable represents the children
		///     of the top level of the hierarchy.  The values calculated will be
		///     compared to the parent line item (supply order)
		/// </param>
		/// <param name="accountingSite">
		/// The accounting Site.
		/// </param>
		public void CalculateValues(DataTable dt, AccountingSite accountingSite)
		{
			// Determine the total amount received for children
			this.totalReceived = 0;
			this.totalValueReceived = 0;
			double received;

			foreach (DataRow dr in dt.Rows)
			{
				// If the child is a completed receipt and good quality
				// the gross quantity should be added to the totalReceivedForChildren
				if (Convert.ToInt32(dr["LookupTransTypeIndex"]) == (int)TransactionTypes.T8_Receipt)
				{
					// Took this out since this routine is used for exceedence check, and Receipt quantity is
					// excluded.
					/*if (Convert.ToInt32(dr["TransactionStatus"]) == (int)TransactionStatus.Completed &&
						Convert.ToInt32(dr["Quality"]) == (int)TransactionQuality.Usable)
					{
						received = 
							accountingSite.convertFromSI((double)dr["GrossQuantity"],
							AccountingSite.ConversionUnits.VOLUME);

						if (dr["ProductPrice"] != DBNull.Value)
							totalValueReceived += ((double)dr["ProductPrice"] * received);

						totalReceived += received;
					}*/
				}
				else
				{
					// Since the child line item is not a receipt check to see if
					// it is a completed line item.  If not, use the gross quantity.
					// Because the items should roll up only get the items at the
					// level just below the parent.
					if (Convert.ToInt32(dr["LookupQualityIndex"]) == (int)TransactionQuality.Usable && Convert.ToInt16(dr["Tier"]) == 0)
					{
						received = accountingSite.ConvertFromSi((double)dr["GrossQuantity"], AccountingSite.ConversionUnits.VOLUME);

						if (dr["ProductPrice"] != DBNull.Value)
						{
							this.totalValueReceived += (double)dr["ProductPrice"] * received;
						}

						this.totalReceived += received;
					}
				}
			}
		}

		public DataSet GetHierarchy(Guid transactionLineItemGuid)
		{
			var unionDS = new DataSet();

			// Get the child line items associated with the passed line item
			using (var cmd = new SqlCommand("usp_GetChildLineItems"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new SqlParameter("@parentTransactionLineItemGuid", SqlDbType.UniqueIdentifier));
				cmd.Parameters[0].Value = transactionLineItemGuid;

				DataSet dataSet1 = this.consolidatedDA.GetDataSet(cmd, this.security);

				if ((dataSet1 != null) && (dataSet1.Tables != null) && (dataSet1.Tables.Count > 0))
				{
					DataTable dataTable = dataSet1.Tables[0];
					dataTable.TableName = "Children";
					dataSet1.Tables.Remove(dataTable);
					unionDS.Tables.Add(dataTable);
				}

				// Now get the parents
				cmd.Parameters.Clear();
				cmd.CommandText = "usp_GetParentLineItems";
				cmd.Parameters.Add(new SqlParameter("@childTransactionLineItemGuid", SqlDbType.UniqueIdentifier));
				cmd.Parameters[0].Value = transactionLineItemGuid;

				DataSet dataSet2 = this.consolidatedDA.GetDataSet(cmd, this.security);

				if ((dataSet2 != null) && (dataSet2.Tables != null) && (dataSet2.Tables.Count > 0))
				{
					DataTable dataTable = dataSet2.Tables[0];
					dataTable.TableName = "Parents";
					dataSet2.Tables.Remove(dataTable);
					unionDS.Tables.Add(dataTable);
				}

				return unionDS;
			}
		}

		public void UpdateAggregatedParents(Guid transactionLineItemGuid, bool deleteFlag)
		{
			// Create a connection
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_GetParentLineItems";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@childTransactionLineItemGuid", SqlDbType.UniqueIdentifier));
				cmd.Parameters[0].Value = transactionLineItemGuid;

				DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, this.security);

				if ((dataSet != null) && (dataSet.Tables.Count > 0))
				{
					DataTable dataTable = dataSet.Tables[0];

					if (dataTable.Rows != null)
					{
						foreach (DataRow dr in dataTable.Rows)
						{
							if (dr.IsNull("AggregateChildren") ? false : (bool)dr["AggregateChildren"])
							{
								// Get all the children for the line item
								using (var cmd2 = new SqlCommand())
								{
									cmd2.CommandText = "usp_GetChildLineItems";
									cmd2.CommandType = CommandType.StoredProcedure;
									cmd2.Parameters.Add(new SqlParameter("@parentTransactionLineItemGuid", SqlDbType.UniqueIdentifier));
									cmd2.Parameters[0].Value = (Guid)dr["ParentTransactionLineItemGuid"];

									DataSet childrenDS = this.consolidatedDA.GetDataSet(cmd2, this.security);

									if ((childrenDS == null) || (childrenDS.Tables.Count <= 0)
									    || (childrenDS.Tables[0].Rows.Count == 0))
									{
										continue;
									}

									DataTable children = childrenDS.Tables[0];
									children.TableName = "Children";

									// Sum the quantity of the children immediately under
									double childrenQty = 0;
									double childrenExcise = 0;
									double childrenGST = 0;
									double childrenMarkup = 0;

									// Aggregate if not deleting line item transaction.
									foreach (DataRow child in children.Rows)
									{
										if (Convert.ToInt16(child["Tier"]) == 0)
										{
											var childTransactionLineItemGuid = (Guid)child["ChildTransactionLineItemGuid"];

											if (!(deleteFlag && transactionLineItemGuid == childTransactionLineItemGuid))
											{
												childrenQty += child.IsNull("GrossQuantity") ? 0 : (double)child["GrossQuantity"];
												childrenExcise += child.IsNull("Tax1") ? 0 : (double)child["Tax1"];
												childrenGST += child.IsNull("Tax2") ? 0 : (double)child["Tax2"];
												childrenMarkup += child.IsNull("Tax3") ? 0 : (double)child["Tax3"];
											}
										}
									}

									// If the value for gross quantity or tax values have changed, update
									// the parent line item. (Update will be performed only if the
									// line item passed into this method belongs to a Demand or Receipt type
									// transaction)
									double parentQty = dr.IsNull("GrossQuantity") ? 0 : (double)dr["GrossQuantity"];
									double parentExcise = dr.IsNull("Tax1") ? 0 : (double)dr["Tax1"];
									double parentGST = dr.IsNull("Tax2") ? 0 : (double)dr["Tax2"];
									double parentMarkup = dr.IsNull("Tax3") ? 0 : (double)dr["Tax3"];

									if (childrenQty != parentQty || childrenExcise != parentExcise || childrenGST != parentGST
									    || childrenMarkup != parentMarkup)
									{
										using (var cmd3 = new SqlCommand())
										{
											cmd3.CommandType = CommandType.Text;
											cmd3.CommandText = @"UPDATE tblTransactionLineItems SET 
												  GrossQuantity = @gross, Tax1 = @Excise, Tax2 = @GST, Tax3 = @Markup 
												  WHERE TransactionLineItemGuid = @parentTransactionLineItemGuid AND 
												  EXISTS(SELECT * FROM tblTransactions t INNER JOIN tblTransactionLinks l ON 
												  t.TransID = l.LinkedTransID WHERE (LookupTransTypeIndex = 9 OR LookupTransTypeIndex = 8 OR LookupTransTypeIndex = 5) AND
												  l.LinkedTransactionLineItemGuid=@TransactionLineItemGuid AND l.TransactionLineItemGuid=@parentTransactionLineItemGuid )";

											cmd3.Parameters.Clear();
											cmd3.Parameters.Add(new SqlParameter("@gross", SqlDbType.Float));
											cmd3.Parameters.Add(new SqlParameter("@Excise", SqlDbType.Float));
											cmd3.Parameters.Add(new SqlParameter("@GST", SqlDbType.Float));
											cmd3.Parameters.Add(new SqlParameter("@Markup", SqlDbType.Float));
											cmd3.Parameters.Add(new SqlParameter("@parentTransactionLineItemGuid", SqlDbType.UniqueIdentifier));
											cmd3.Parameters.Add(new SqlParameter("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier));

											cmd3.Parameters["@gross"].Value = Math.Abs(childrenQty);
											cmd3.Parameters["@Excise"].Value = childrenExcise;
											cmd3.Parameters["@GST"].Value = childrenGST;
											cmd3.Parameters["@Markup"].Value = childrenMarkup;
											cmd3.Parameters["@parentTransactionLineItemGuid"].Value = (Guid)dr["ParentTransactionLineItemGuid"];
											cmd3.Parameters["@TransactionLineItemGuid"].Value = transactionLineItemGuid;

											this.consolidatedDA.ExecuteQuery(this.security, cmd3);
										}
									}
								}
							}
							else
							{
								break;
							}
						}
					}
				}
			}
		}

		#endregion
	}
}