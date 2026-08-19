// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeightedAverageCostDBI.cs" company="Varec, Inc.">
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

	using FMBusinessObjects.DataObjects;

	// updates WAC via transaction updates only, for user overrides see ConsolidatedBLL.WeightedAverageCostsClass
	public class WeightedAverageCostDBI : BaseDBI
	{
		#region Constants and Fields

		protected static object singleton = new Object();

		protected SecurityClass security = null;

		protected WeightedAverageCostDO wacDO;

		#endregion

		#region Constructors and Destructors

		public WeightedAverageCostDBI(SecurityClass inSecurity, string user, DateTimeOffset saveTime)
			: base(user, saveTime)
		{
			this.security = inSecurity;
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// </summary>
		/// <param name="a_trans">
		/// </param>
		/// <param name="a_lineItem">
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <remarks>
		/// will save WAC dispite the type of transaction, filtering done by caller
		/// </remarks>
		public Guid Save(TransactionDO a_trans, LineItemDO a_lineItem)
		{
			// initialise WAC member using transaction and line item values
			this.wacDO.WeightedAverageCostGuid = Guid.Empty;
			this.wacDO.SiteGuid = a_trans.SiteGuid;
			this.wacDO.ProductGuid = a_lineItem.ProductGuid;
			this.wacDO.WacValue = a_lineItem.Tax4.Value;
			this.wacDO.CreatedDate = a_trans.TransactionDateTime.Value;
			this.wacDO.CreatedBy = base.user;
			this.wacDO.UpdatedDate = this.wacDO.CreatedDate;
			this.wacDO.UpdatedBy = this.wacDO.CreatedBy;
			this.wacDO.Source = a_trans.TransID;
			this.wacDO.IsManualOverride = false;
			this.wacDO.Notes = " ";
			this.wacDO.InventoryDate = a_trans.InventoryDate;

			this.Save(this.wacDO);

			// always create as new wac object
			this.wacDO.WeightedAverageCostGuid = this.Insert();

			return this.wacDO.WeightedAverageCostGuid;
		}

		public Guid Save(WeightedAverageCostDO a_wac)
		{
			this.wacDO = a_wac;

			this.wacDO.WeightedAverageCostGuid = Guid.Empty; // reset to Guid.Empty always

			this.wacDO.WeightedAverageCostGuid = this.Insert();
			a_wac.WeightedAverageCostGuid = this.wacDO.WeightedAverageCostGuid;

			return this.wacDO.SiteGuid;
		}

		#endregion

		#region Methods

		protected Guid Insert()
		{
			int i = 0;
			this.insertCmd.Parameters[i].Value = this.wacDO.SiteGuid;
			this.insertCmd.Parameters[++i].Value = this.wacDO.ProductGuid;
			this.insertCmd.Parameters[++i].Value = this.wacDO.WacValue;
			this.insertCmd.Parameters[++i].Value = this.wacDO.IsManualOverride;
			this.insertCmd.Parameters[++i].Value = this.wacDO.Source;
			this.insertCmd.Parameters[++i].Value = this.wacDO.Notes;
			this.insertCmd.Parameters[++i].Value = this.wacDO.CreatedBy;
			this.insertCmd.Parameters[++i].Value = this.wacDO.CreatedDate;
			this.insertCmd.Parameters[++i].Value = this.wacDO.UpdatedBy;
			this.insertCmd.Parameters[++i].Value = this.wacDO.UpdatedDate;
			this.insertCmd.Parameters[++i].Value = this.wacDO.InventoryDate;
			this.insertCmd.Parameters[++i].Value = Guid.NewGuid();

			lock (singleton)
			{
				this.ConsolidatedDA.ExecuteQuery(this.security, this.insertCmd);
			}

			return this.wacDO.WeightedAverageCostGuid;
		}

		protected override void PrepareDeleteRemainingStatement()
		{
			// not needed
		}

		protected override void PrepareDeleteStatement()
		{
			// not needed
		}

		protected override void PrepareInsertStatement()
		{
			this.insertCmd.CommandText = "INSERT INTO tblWeightedAverageCosts " + "(SiteGuid, " + "ProductGuid, " + "WacValue, "
			                             + "IsManualOverride, " + "Source, " + "Notes, " + "CreatedBy, " + "CreatedDate, "
			                             + "UpdatedBy, " + "UpdatedDate, " + "InventoryDate," + "WeightedAverageCostGuid"
			                             + ") VALUES ( " + "@SiteGuid, " + "@ProductGuid, " + "@WacValue, "
			                             + "@IsManualOverride, " + "@Source, " + "@Notes, " + "@CreatedBy, " + "@CreatedDate, "
			                             + "@UpdatedBy, " + "@UpdatedDate, " + "@InventoryDate," + "@WeightedAverageCostGuid"
			                             + ")";

			this.insertCmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			this.insertCmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			this.insertCmd.Parameters.Add("@WacValue", SqlDbType.Float);
			this.insertCmd.Parameters.Add("@IsManualOverride", SqlDbType.Bit);
			this.insertCmd.Parameters.Add("@Source", SqlDbType.NVarChar, 64);
			this.insertCmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 1024);
			this.insertCmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			this.insertCmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			this.insertCmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			this.insertCmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			this.insertCmd.Parameters.Add("@InventoryDate", SqlDbType.DateTimeOffset);
			this.insertCmd.Parameters.Add("@WeightedAverageCostGuid", SqlDbType.UniqueIdentifier);
		}

		protected override void PrepareSelectStatement()
		{
			// selects ONE wac value

			// (actually this is unused too)
			this.selectCmd.CommandText = "SELECT * FROM tblWeightedAverageCosts "
			                             + "WHERE SiteGuid = @SiteGuid AND ProductGuid = @ProductGuid AND CreatedDate = @CreatedDate";

			this.selectCmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			this.selectCmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			this.selectCmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
		}

		protected override void PrepareUpdateStatement()
		{
			// not needed
		}

		#endregion
	}
}