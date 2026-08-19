//******************************************************************************
//	FILE NAME:		AviationProductClassification.cs
//	PURPOSE:			Control for assigning products as "Fuel" or "Non-fuel"
//
//						This is part of the main dialog - specifically, the 
//						Aviation Product Classification Grid on the Product Configuration
//						tab of the main tab control
//						i.e., MainForm->Tab Control->Product Configuration Page->AviationProducts grid
//
//	COMMENTS:
//		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Varec, Inc.
//
//	AUTHOR(S):	Chris Knight
//	VERSION:		1.0.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:				Reason:
//		---------	-------------- -------------------------------------------
//		04-May-2007	C. Knight		1.0.0.0	- Initial Creation
//
//*******************************************************************************       

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DispatchPrototype;

namespace ASCReporter
{
	public partial class MainForm : FMBaseForm
	{
		#region Event Handlers
		#endregion

		#region Public Members
		/// <summary>
		/// Function to be called upon selecting tab
		/// </summary>
		/// <remarks>
		/// SetActive should be called to initialize data on this tab when this
		/// tab is selected
        /// 
        /// This function assumes that the user logged into the same site that he is 
        /// running the report for - As Dispatch is a single-site application, this is
        /// a safe assumption.
		/// </remarks>
		public void AviationProductClassification_Init()
		{
			// Precondition ASC_AVIATIONPRODUCT_CLASS table
			// This ensures that every aviation product in the tblProducts table has an entry in the
			// ASC_AVIATIONPRODUCT_CLASS table before we proceed.
			SqlCommand preconditionTable = this.connection.CreateCommand();
			preconditionTable.CommandText = "INSERT INTO #ASC_AVIATIONPRODUCT_CLASS "
													+ "(Grade, Class) "
													+ "SELECT P.ProductID, 'Non-fuel' "
													+ "From ConsolidatedDB.dbo.tblProducts P "
													+ "WHERE P.UserData2 = 'Yes' AND "
                                                    + "(P.ProductIndex IN (SELECT [Index] FROM dbo.tblEntityToSiteMap WHERE TypeID = 'Products' AND SiteIndex=@SiteIndex))";

			preconditionTable.Parameters.AddWithValue("@SiteGuid", this.siteGuid);
			preconditionTable.ExecuteNonQuery();

			// Set up the command for populating the grid
			this.aviationClassCommand = this.connection.CreateCommand();
			this.aviationClassCommand.CommandText = "SELECT ndx, Grade, Class FROM #ASC_AVIATIONPRODUCT_CLASS";

			//Adapter setup
			this.aviationClassAdapter = new SqlDataAdapter(this.aviationClassCommand);
			this.aviationClassAdapter.UpdateCommand = new SqlCommand("UPDATE #ASC_AVIATIONPRODUCT_CLASS SET Class = @Class "
																						+ "WHERE ndx = @ndx",
																						this.connection);
			this.aviationClassAdapter.UpdateCommand.Parameters.Add("@Class", SqlDbType.NVarChar, 10, "Class");
			this.aviationClassAdapter.UpdateCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");
			this.aviationClassAdapter.UpdateCommand.Parameters["@ndx"].SourceVersion = DataRowVersion.Original;

			// Create and fill dataset
			this.aviationClassDataSet = new DataSet();
			this.aviationClassAdapter.Fill(this.aviationClassDataSet);
			DataColumn[] primaryKey = new DataColumn[1];
			primaryKey[0] = this.aviationClassDataSet.Tables[0].Columns["ndx"];
			this.aviationClassDataSet.Tables[0].PrimaryKey = primaryKey;

			// Populate grid
			this.aviationProductConfig.AutoGenerateColumns = false;
			this.aviationProductConfig.DataSource = this.aviationClassDataSet.Tables[0];
			this.aviationProduct.DataPropertyName = "Grade";
			this.aviationProductClass.DataPropertyName = "Class";
			this.aviationProductConfig.Refresh();
		}

		private void AviationProductClassification_SetActive()
		{
			this.aviationClassAdapter.Fill(this.aviationClassDataSet);
			this.aviationProductConfig.Refresh();
		}

		private void AviationProductClassification_KillActive()
		{
			this.aviationClassAdapter.Update(this.aviationClassDataSet);
		}

		#endregion

		#region Private variables
		SqlCommand			aviationClassCommand;
		DataSet				aviationClassDataSet;
		SqlDataAdapter		aviationClassAdapter;
		#endregion
	}
}