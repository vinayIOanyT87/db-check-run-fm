/// <summary>
///   File name:	CustomMovementFunctionDO.cs
///   Purpose:	   The purpose of this class is to call a special SQL function to
///               perform custom movement calculations.
///				
///   Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA,
///				   2010.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Varec, Inc.
///				
///	Author(s):	Ivan Orndorff
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///   2001-08-27     I.Orndorff				- Initial Revision.
///
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Data.SqlClient;

[System.Serializable]
public class CustomMovementFunctionDO
{
	#region Private data members
	private QuantityDO quantityDO;
	private double tolerance;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor for the Custom Movement Function data object class.
	/// </summary>
	public CustomMovementFunctionDO()
	{
		this.Init();
	}
	#endregion

	#region Properties
	/// <summary>
	/// This property returns the volume DO data member.
	/// </summary>
	public QuantityDO Quantity
	{
		get { return this.quantityDO; }
	}

	/// <summary>
	/// This property returns the tolerance member.
	/// </summary>
	public double Tolerance
	{
		get { return tolerance; }
	}
	#endregion

	#region SQL Methods
	/// <summary>
	/// This method will execute the custom function to perform custom Movement for
	/// an individual alias.
	/// </summary>
	/// <param name="functionName"></param>
	/// <param name="parameterXML"></param>
	public void ExecuteCustomFunction(string functionName, string parameterXML, SqlConnection connection)
	{
		DataSet dataSet = new DataSet();

		if ((functionName != null) && (functionName.Length > 0) && (parameterXML != null) && (parameterXML.Length > 0))
		{
			string sql = "EXEC @FunctionName @ParameterXML";

			SqlCommand command = new SqlCommand(sql, connection);

			command.Parameters.Add("@FunctionName", System.Data.SqlDbType.NVarChar, 100);
			command.Parameters.Add("@ParameterXML", System.Data.SqlDbType.NVarChar, 4000);

			command.Parameters["@FunctionName"].Value = functionName;
			command.Parameters["@ParameterXML"].Value = parameterXML;

			command.Prepare();

			SqlDataAdapter adapter = new SqlDataAdapter(command);
			adapter.Fill(dataSet);

			// Load the retrieve data set.
			this.Load(dataSet);
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

		this.quantityDO = new QuantityDO(gross, net, mass);
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
