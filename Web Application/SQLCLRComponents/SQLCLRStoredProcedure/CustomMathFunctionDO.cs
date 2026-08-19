/// <summary>
///   File name:	CustomMathFunctionDO.cs
///   Purpose:	   The purpose of this class is to call a special SQL function to
///               perform custom calculations.
///				
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///   yyyy-mm-dd     Developer's name     The reason for the modification
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
public class CustomMathFunctionDO
{
	#region Private data members
	private QuantityDO quantityDO;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor for the Custom Math Function data object class.
	/// </summary>
	public CustomMathFunctionDO()
	{
		this.Init();
	}
	#endregion

	#region Properties
	/// <summary>
	/// This property returns the quantity DO data member.
	/// </summary>
	public QuantityDO Quantity
	{
		get { return this.quantityDO; }
	}
	#endregion

	#region SQL Methods
	/// <summary>
	/// This method will execute the custom function to perform custom math for
	/// aggregate aliases.
	/// </summary>
	/// <param name="functionName"></param>
	/// <param name="parameterXML"></param>
	public void ExecuteCustomFunction(SqlConnection connection, string functionName, string parameterXML)
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
		double package = 0;
		double grossPrice = row.IsNull("GrossPrice") ? 0.0 : (double)row["GrossPrice"];
		double netPrice = row.IsNull("NetPrice") ? 0.0 : (double)row["NetPrice"];
		double massPrice = row.IsNull("MassPrice") ? 0.0 : (double)row["MassPrice"];
		double number1 = row.IsNull("Number01") ? 0.0 : (double)row["Number01"];
		double number2 = row.IsNull("Number02") ? 0.0 : (double)row["Number02"];
		double number3 = row.IsNull("Number03") ? 0.0 : (double)row["Number03"];
		double number4 = row.IsNull("Number04") ? 0.0 : (double)row["Number04"];
		double number5 = row.IsNull("Number05") ? 0.0 : (double)row["Number05"];
		double number6 = row.IsNull("Number06") ? 0.0 : (double)row["Number06"];

		this.quantityDO = new QuantityDO(gross, net, mass, package, grossPrice, netPrice, massPrice, number1, number2, number3, number4, number5, number6);
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
