namespace LedgerCore
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	public class LRCustomMovementFunctionDO
	{
		#region Private data members
		private LRQuantityDO quantityDO;
		private double tolerance;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Custom Movement Function data object class.
		/// </summary>
		public LRCustomMovementFunctionDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the volume DO data member.
		/// </summary>
		public LRQuantityDO Quantity
		{
			get { return this.quantityDO; }
		}

		/// <summary>
		/// This property returns the tolerance member.
		/// </summary>
		public double Tolerance
		{
			get { return this.tolerance; }
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will execute the custom function to perform custom Movement for
		/// an individual alias.
		/// </summary>
		/// <param name="functionName"></param>
		/// <param name="parameterXML"></param>
		/// <param name="siteGuid"></param>
		/// <param name="ledgerConnection"></param>
		public void ExecuteCustomFunction(string functionName, string parameterXML, Guid siteGuid, LedgerConnection ledgerConnection)
		{
			if (!string.IsNullOrEmpty(functionName) && (!string.IsNullOrEmpty(parameterXML)))
			{
				const string SQL = "EXEC @FunctionName @ParameterXML, @StartSiteIndex";

				using (SqlCommand command = new SqlCommand(SQL))
				{
					command.Parameters.Add("@FunctionName", SqlDbType.NVarChar, 100);
					command.Parameters.Add("@ParameterXML", SqlDbType.NVarChar, -1);
					command.Parameters.Add("@StartSiteIndex", SqlDbType.UniqueIdentifier);

					command.Parameters["@FunctionName"].Value = functionName;
					command.Parameters["@ParameterXML"].Value = parameterXML;
					command.Parameters["@StartSiteIndex"].Value = siteGuid;

					command.CommandTimeout = 0;

					DataSet dataSet = ledgerConnection.GetDataSet(command);

					// Load the retrieve data set.
					this.Load(dataSet);
				}
			}
		}
		#endregion

		#region Load methods
		/// <summary>
		/// This method will load the object based on one row.
		/// </summary>
		/// <param name="row">A single row of data</param>
		private void Load(DataRow row)
		{
			if (null == row)
			{
				return;
			}

			double gross = row.IsNull("Gross") ? 0.0 : (double)row["Gross"];
			double net = row.IsNull("Net") ? 0.0 : (double)row["Net"];
			double mass = row.IsNull("Mass") ? 0.0 : (double)row["Mass"];
			this.tolerance = row.IsNull("Tolerance") ? 0.0 : (double)row["Tolerance"];

			this.quantityDO = new LRQuantityDO(gross, net, mass);
		}

		/// <summary>
		/// This method will load the object based on a data set.
		/// </summary>
		/// <param name="dataSet"></param>
		private void Load(DataSet dataSet)
		{
			if (dataSet == null)
			{
				return;
			}

			this.Init();

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			this.Load(table.Rows[0]);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initial the Custom Math Function DO to its initial state.
		/// </summary>
		private void Init()
		{
			this.quantityDO = null;
		}
		#endregion
	}
}