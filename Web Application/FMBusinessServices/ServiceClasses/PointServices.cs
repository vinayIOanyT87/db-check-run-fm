// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointServices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using DataAccessLayer;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]

	public class PointServices : IPointServices
	{
		/// <summary>
		/// Adds the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointService">The point service.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// or
		/// pointService
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointService pointService)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointService == null)
			{
				throw new ArgumentNullException("pointService");
			}

			var consolidatedDA = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
				pointService.SetCreationStamp(security);
				pointService.AutoGenerateInsertProcSQL(cmd, "gsp_PointServiceInsertByPK");
				cmd.Parameters["@PointServiceGuid"].Direction = ParameterDirection.Output;

				consolidatedDA.ExecuteQuery(security, cmd);

				pointService.IdentityGuid = new Guid(cmd.Parameters["@PointServiceGuid"].Value.ToString());
			}


			return pointService.IdentityGuid;
		}


		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointService">The point service.</param>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// or
		/// pointService
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointService pointService)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointService == null)
			{
				throw new ArgumentNullException("pointService");
			}

			var consolidatedDA = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
				pointService.SetModifyStamp(security);
				pointService.AutoGenerateModifyProcSQL(cmd, "gsp_PointServiceUpdateByPK");

				consolidatedDA.ExecuteQuery(security, cmd);
			}
		}


		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointServiceGuid">The point service unique identifier.</param>
		/// <exception cref="System.ArgumentNullException">security</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointServiceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			// Delete pointService
			var consolidatedDA = new ConsolidatedDAClass();
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "DELETE FROM dbo.tblPointService WHERE PointServiceGuid = @PointServiceGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointServiceGuid", pointServiceGuid);
				consolidatedDA.ExecuteQuery(security, cmd);
			}
		}



		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="hostName">Name of the host.</param>
		/// <returns></returns>
		public PointService Get(SecurityClass security, string hostName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(hostName))
			{
				throw new ArgumentNullException("hostName");
			}

			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT * FROM dbo.tblPointService WHERE HostName = @HostName";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@HostName", hostName);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return null;
			}

			var pointService = new PointService();
			pointService.AutoLoad(table.Rows[0]);

			return pointService;
		}

		public List<PointService> Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT * FROM dbo.tblPointService ORDER BY Hostname";
				cmd.CommandType = CommandType.Text;
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointServiceList = new List<PointService>();
			foreach (DataRow row in table.Rows)
			{
				var pointService = new PointService();
				pointService.AutoLoad(row);
				pointServiceList.Add(pointService);
         }

			return pointServiceList;
		}

	}
}