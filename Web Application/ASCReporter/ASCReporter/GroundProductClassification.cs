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
		#region Private member functions
		private void GroundProductClassification_Init()
		{
			// Precondition ASC_GROUNDPRODUCT_CLASS table
			// This ensures that every aviation product in the tblProducts table has an entry in the
			// ASC_GROUNDPRODUCT_CLASS table before we proceed.
			SqlCommand preconditionTable = this.connection.CreateCommand();
			preconditionTable.CommandText = "INSERT INTO #ASC_GROUNDPRODUCT_CLASS ";
			preconditionTable.CommandText += "(Grade, Class) ";
			preconditionTable.CommandText += "SELECT P.ProductID, 'Non-fuel' ";
			preconditionTable.CommandText += "From ConsolidatedDB.dbo.tblProducts P ";
            preconditionTable.CommandText += "WHERE P.UserData2 = 'No' AND ";
            preconditionTable.CommandText += "(P.ProductGuid IN (SELECT ProductGuid FROM map.tblEntityProductToSite WHERE SiteIndex=@SiteIndex))";
			preconditionTable.Parameters.AddWithValue("@SiteGuid", this.siteGuid);
			preconditionTable.ExecuteNonQuery();

			// Set up the command for populating the grid
			this.groundClassCommand = this.connection.CreateCommand();
			this.groundClassCommand.CommandText = "SELECT ndx, Grade, Class FROM #ASC_GROUNDPRODUCT_CLASS";

			//Adapter setup
			this.groundClassAdapter = new SqlDataAdapter(this.groundClassCommand);
			this.groundClassAdapter.UpdateCommand = new SqlCommand("UPDATE #ASC_GROUNDPRODUCT_CLASS "
																					+ "SET Class = @Class "
																					+ "WHERE ndx = @ndx",
																					this.connection);
			this.groundClassAdapter.UpdateCommand.Parameters.Add("@Class", SqlDbType.NVarChar, 20, "Class");
			this.groundClassAdapter.UpdateCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");
			this.groundClassAdapter.UpdateCommand.Parameters["@ndx"].SourceVersion = DataRowVersion.Original;

			// Create and fill dataset
			this.groundClassDataSet = new DataSet();
			this.groundClassAdapter.Fill(this.groundClassDataSet);
			DataColumn[] primaryKey = new DataColumn[1];
			primaryKey[0] = this.groundClassDataSet.Tables[0].Columns["ndx"];
			this.groundClassDataSet.Tables[0].PrimaryKey = primaryKey;

			// Populate grid
			this.groundProductConfig.AutoGenerateColumns = false;
			this.groundProductConfig.DataSource = this.groundClassDataSet.Tables[0];
			this.groundProduct.DataPropertyName = "Grade";
			this.groundProductClass.DataPropertyName = "Class";
			this.groundProductConfig.Refresh();
		}

		private void GroundProductClassification_SetActive()
		{
			this.groundClassAdapter.Fill(this.groundClassDataSet);
			this.groundProductConfig.Refresh();
		}

		private void GroundProductClassification_KillActive()
		{
			this.groundClassAdapter.Update(this.groundClassDataSet);
		}
		#endregion

		#region Private variables
		SqlCommand			groundClassCommand;
		DataSet				groundClassDataSet;
		SqlDataAdapter		groundClassAdapter;
		#endregion
	}
}