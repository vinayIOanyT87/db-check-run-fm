namespace FMBusinessServices.ServiceClasses
{
	 using System;
	 using System.Data;
	 using System.Security;
	 using System.Data.SqlClient;


	 using FMBusinessObjects.BusinessInterfaces;
	 using FMBusinessObjects.DataObjects;
	 using FMCore;

	 using DataAccessLayer;
	 using System.Collections.Generic;
	 using System.ServiceModel;

	 /// <summary>
	 /// Summary description for FCEE
	 /// </summary>
	 [SecuritySafeCritical]
	 [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	 public class FCEDevices : FMServiceBase, IFCEDevices
	 {
		  internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid? Add(SecurityClass security, FCEDevice fceDevice)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (fceDevice.FCEDeviceGuid == Guid.Empty)
			{
				fceDevice.FCEDeviceGuid = Guid.NewGuid();
			}

			using (var cmd = new SqlCommand())
			{
				fceDevice.SetCreationStamp(security);
				fceDevice.AutoGenerateInsertProcSQL(cmd, "[dbo].[gsp_FCEDeviceInsertByPK]");
				cmd.Parameters["@FCEDeviceGuid"].Direction = ParameterDirection.InputOutput;

				ConsolidatedDA.ExecuteQuery(security, cmd);

				fceDevice.FCEDeviceGuid = new Guid(cmd.Parameters["@FCEDeviceGuid"].Value.ToString());
			}


			return fceDevice.FCEDeviceGuid;
		}

		public FCEDevice Get(SecurityClass security, Guid FCEDeviceGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DataSet set = null;
			var fceDevice = new FCEDevice();
			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
				sqlCommand.CommandText = "dbo.usp_FCEDeviceGet";
				sqlCommand.Parameters.AddWithValue("@FCEDeviceGuid", FCEDeviceGuid);
				set = ConsolidatedDA.GetDataSet(sqlCommand, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count > 0)
			{
				var row = set.Tables[0].Rows[0];
				fceDevice.AutoLoad(row);
			}

			fceDevice = PopulateScalerConfiguration(fceDevice, security);

			return fceDevice;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, FCEDevice fceDevice)
		{
				if (security == null)
				{
					 throw new ArgumentNullException("security");
				}


				var existingFCEDevice = this.Get(security, fceDevice.FCEDeviceGuid);
				if (existingFCEDevice.IdentityGuid == Guid.Empty)
				{
					 throw new Exception("FCE Device not found for update.");
				}

				using (var cmd = new SqlCommand())
				{
					 fceDevice.SetModifyStamp(security);
					 fceDevice.AutoGenerateModifyProcSQL(cmd, "[dbo].[gsp_FCEDeviceUpdateByPK]");
					 ConsolidatedDA.ExecuteQuery(security, cmd);
				}

		  }


		  public List<FCEDevice> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid)
		  {
				security.ThrowIfNull(nameof(security));
				List<FCEDevice> FCEDeviceList = new List<FCEDevice>();
				using (var sqlCommand = new SqlCommand())
				{
					 sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
					 sqlCommand.CommandText = "dbo.usp_FCEDeviceEnumerate";
					 sqlCommand.Parameters.AddWithValue("@siteGuid", siteGuid);

					 var dataSet = ConsolidatedDA.GetDataSet(sqlCommand, security);

					 if (dataSet != null &&
						 dataSet.Tables[0].Rows.Count > 0)
					 {

						  return PopulateList(dataSet);
					 }
				}
				return FCEDeviceList;
		  }

		  public void Purge(SecurityClass security, Guid FCEDeviceGuid)
		  {

				if (security == null)
				{
					 throw new ArgumentNullException("security");
				}

				DataSet set = null;
				using (var sqlCommand = new SqlCommand())
				{
					 sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
					 sqlCommand.CommandText = "dbo.usp_FCEDeviceDelete";
					 sqlCommand.Parameters.AddWithValue("@FCEDeviceGuid", FCEDeviceGuid);
					 set = ConsolidatedDA.GetDataSet(sqlCommand, security);
				}
		  }

		  // Utility functions
		  protected List<FCEDevice> PopulateList(DataSet set)
		  {
				List<FCEDevice> FCEDevices = new List<FCEDevice>();
				DataTable table = set.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					 var fceDevice = new FCEDevice();
					 fceDevice.AutoLoad(row);
					 FCEDevices.Add(fceDevice);
				}
				return FCEDevices;
		  }

		  public FCEDevice GetbyIMEI(SecurityClass security, string IMEI)
		  {
				security.ThrowIfNull(nameof(security));
				FCEDevice fceDevice = new FCEDevice();
				using (var sqlCommand = new SqlCommand())
				{
					 sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
					 sqlCommand.CommandText = "dbo.usp_FCEDeviceGetbyIMEI";
					 sqlCommand.Parameters.AddWithValue("@IMEI", IMEI);

					 var dataSet = ConsolidatedDA.GetDataSet(sqlCommand, security);

					 if (dataSet != null &&
						 dataSet.Tables[0].Rows.Count > 0)
					 {
						  var row = dataSet.Tables[0].Rows[0];
						  fceDevice.AutoLoad(row);
					 }

					if (dataSet != null &&
						dataSet.Tables[0].Rows.Count > 0)
					{
						var row = dataSet.Tables[0].Rows[0];
						fceDevice.AutoLoad(row);
					}
					
				}

				fceDevice = PopulateScalerConfiguration(fceDevice, security);

				return fceDevice;
		}

	   private FCEDevice PopulateScalerConfiguration (FCEDevice fceDevice, SecurityClass security)
        {
				DataSet set = null;
				using (var sqlCommand = new SqlCommand())
				{
					 sqlCommand.CommandType = System.Data.CommandType.Text;
					 sqlCommand.CommandText = "SELECT * FROM [dbo].[tblFCEEMapping] WHERE [MsgType] = 7 AND [FCEDeviceGuid] = @FCEDeviceGuid ORDER By [Index]";
					 sqlCommand.Parameters.AddWithValue("@FCEDeviceGuid", fceDevice.IdentityGuid);
					 set = ConsolidatedDA.GetDataSet(sqlCommand, security);
				}

				DataTable table = set.Tables[0];
				if (table.Rows.Count > 0)
				{
					 foreach (DataRow row in set.Tables[0].Rows)
					 {
						  if (row["Index"] is int
						  && row["tagSelection"] is int)
						  {
								byte index = (byte)((int)row["Index"] & 0xFF);
								int tagSelection = (int)row["TagSelection"];
								fceDevice.ScalerConfiguration[index / 8] = (byte)(fceDevice.ScalerConfiguration[index / 8] | 1 << index % 8);

								if (tagSelection == (int)TAGSELECTIONTYPE.LevelProduct
								|| tagSelection == (int)TAGSELECTIONTYPE.LevelWater)
								{
									 fceDevice.ScalerType[index / 8] = (byte)(fceDevice.ScalerType[index / 8] | 1 << index % 8);
								}
						  }
					 }
				}
				return fceDevice;

		  }
	}
}