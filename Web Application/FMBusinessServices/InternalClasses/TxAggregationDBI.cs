// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TxAggregationDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Text;

	using FMBusinessObjects.DataObjects;

	public class TxAggregationDBI : BaseDBI
	{
		#region Constructors and Destructors

		public TxAggregationDBI(string user, DateTimeOffset saveTime)
			: base(user, saveTime)
		{
		}

		#endregion

		#region Public Methods and Operators

		public TxAggregationDO Aggregate(
			SecurityClass security, Guid siteGuid, short parentTransTypeID, ArrayList atxLineItemGuids)
		{
			var result = new TxAggregationDO();

			this.selectCmd.Parameters[0].Value = siteGuid;
			this.selectCmd.Parameters[1].Value = parentTransTypeID;

			var lineIds = new StringBuilder();

			for (int i = 0; i < atxLineItemGuids.Count; ++i)
			{
				lineIds.Append(atxLineItemGuids[i] + ",");

				// because the maximum nvarchar handle-able is 4000 characters, we have to send batches of 4000 characters
				if (lineIds.Length > 3800 || // 3800 characters to be safe
				    i == atxLineItemGuids.Count - 1)
				{
					// reached the end before reaching 3800 characters
					lineIds.Remove(lineIds.Length - 1, 1);

					// execute the query
					this.selectCmd.Parameters[2].Value = lineIds.ToString();

					DataSet set = this.ConsolidatedDA.GetDataSet(this.selectCmd, security);

					if (set.Tables.Count > 0)
					{
						using (DataTableReader reader = set.CreateDataReader(set.Tables[0]))
						{
							if (reader.HasRows)
							{
								reader.Read();

								result.Quantity += reader.IsDBNull(0) ? 0.0 : reader.GetDouble(0);
								result.Excise += reader.IsDBNull(1) ? 0.0 : reader.GetDouble(1);
								result.Gst += reader.IsDBNull(2) ? 0.0 : reader.GetDouble(2);
								result.Margin += reader.IsDBNull(3) ? 0.0 : reader.GetDouble(3);
								result.OnCost += reader.IsDBNull(4) ? 0.0 : reader.GetDouble(4);
								result.TotalValue += reader.IsDBNull(5) ? 0.0 : reader.GetDouble(5);
								result.TotalPriceWithTax += reader.IsDBNull(6) ? 0.0 : reader.GetDouble(6);
								result.TotalForeignPrice += reader.IsDBNull(7) ? 0.0 : reader.GetDouble(7);
								result.Number01 += reader.IsDBNull(8) ? 0.0 : reader.GetDouble(8);
								result.Number02 += reader.IsDBNull(9) ? 0.0 : reader.GetDouble(9);
								result.Number03 += reader.IsDBNull(10) ? 0.0 : reader.GetDouble(10);
								result.Number04 += reader.IsDBNull(11) ? 0.0 : reader.GetDouble(11);
								result.Number05 += reader.IsDBNull(12) ? 0.0 : reader.GetDouble(12);
								result.Number06 += reader.IsDBNull(13) ? 0.0 : reader.GetDouble(13);
							}
						}

						// clear lineIDs for possibly the next iteration
						lineIds = new StringBuilder();
					}
				}
			}

			return result;
		}

		#endregion

		#region Methods

		protected override void PrepareDeleteRemainingStatement()
		{
		}

		protected override void PrepareDeleteStatement()
		{
		}

		protected override void PrepareInsertStatement()
		{
		}

		protected override void PrepareSelectStatement()
		{
			this.selectCmd.CommandText = "fm_ADF_CustomAggregationAssociatedTxValues";
			this.selectCmd.CommandType = CommandType.StoredProcedure;
			this.selectCmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			this.selectCmd.Parameters.Add("@ParentTransTypeID", SqlDbType.SmallInt);
			this.selectCmd.Parameters.Add("@AtxLineItemGuid", SqlDbType.UniqueIdentifier);
		}

		protected override void PrepareUpdateStatement()
		{
		}

		#endregion
	}
}