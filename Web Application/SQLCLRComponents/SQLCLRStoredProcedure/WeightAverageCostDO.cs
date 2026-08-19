/// <summary>
///   File name:	WeightAverageCostDO.cs
///   Purpose:	   The purpose of this class is to contain the WAC data along with the
///               SQL to retrieve and load the data.
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
public class WeightAverageCostDO
{
   #region Private data members
   private int      wacIndex;
   private int      productIndex;
   private int      siteIndex;
   private double   wacValue;
   private bool     isManualOverride;
   private string   source;
   private string   notes;
   private string   createdBy;
   private string   updatedBy;
   private DateTimeOffset createdDate;
   private DateTimeOffset updatedDate;
   #endregion

   #region Constructors
   /// <summary>
   /// This is the default constructor for the Weight Average Cost data object class.
   /// </summary>
   public WeightAverageCostDO()
   {
      this.Init();
   }
   #endregion

   #region Properties
   /// <summary>
   /// This property returns and sets the WAC Index.
   /// </summary>
   public int WacIndex 
   { 
      get { return this.wacIndex; }
      set { this.wacIndex = value; }
   }

   /// <summary>
   /// This property returns and sets the Site Index.
   /// </summary>
   public int SiteIndex 
   {
      get { return this.siteIndex; }
      set { this.siteIndex = value; }
   }

   /// <summary>
   /// This property returns and sets the Product Index.
   /// </summary>
   public int ProductIndex 
   {
      get { return this.productIndex; }
      set { this.productIndex = value; }
   }

   /// <summary>
   /// This property returns and sets the WAC value.
   /// </summary>
   public double WacValue
   {
      get { return this.wacValue; }
      set { this.wacValue = value; }
   }

   /// <summary>
   /// This property returns true if it is a manual override.
   /// </summary>
   public bool IsManualOverride
   {
      get { return this.isManualOverride; }
      set { this.isManualOverride = value; }
   }

   /// <summary>
   /// This property returns and sets the source of the WAC change.
   /// </summary>
   public string Source
   {
      get { return this.source; }
      set { this.source = value; }
   }

   /// <summary>
   /// This property returns and sets the reason for the WAC change.
   /// </summary>
   public string Notes
   {
      get { return this.notes; }
      set { this.notes = value; }
   }

   /// <summary>
   /// This property returns and sets the Created By value.
   /// </summary>
   public string CreatedBy
   {
      get { return this.createdBy; }
      set { this.createdBy = value; }
   }

   /// <summary>
   /// This property returns and sets the Updated By value.
   /// </summary>
   public string UpdatedBy
   {
      get { return this.updatedBy; }
      set { this.updatedBy = value; }
   }

   /// <summary>
   /// This property returns and sets the Created Date value.
   /// </summary>
   public DateTimeOffset CreatedDate
   {
      get { return this.createdDate; }
      set { this.createdDate = value; }
   }

   /// <summary>
   /// This property returns and sets the Updated Date value.
   /// </summary>
   public DateTimeOffset UpdatedDate
   {
      get { return this.updatedDate; }
      set { this.updatedDate = value; }
   }
   #endregion

   #region Private Methods
   /// <summary>
   /// This method will initial the WAC DO to its initial state.
   /// </summary>
   private void Init()
   {
      this.WacIndex         = 0;
      this.SiteIndex        = 0;
      this.ProductIndex     = 0;
      this.WacValue         = 0;
      this.IsManualOverride = true;
      this.Source           = "Error: empty";
      this.Notes            = "";
      this.CreatedBy        = "SYSTEM";
      this.CreatedDate      = DateTimeOffset.Now;
      this.UpdatedBy        = this.CreatedBy;
      this.UpdatedDate      = this.CreatedDate;
   }
   #endregion

   #region Load methods
   /// <summary>
   /// This method will load the object based on one row.
   /// </summary>
   /// <param name="row"></param>
   public void Load(DataRow row)
   {
      if (null == row)
      {
          return;
      }

      this.WacIndex         = row.IsNull("WacIndex")         ? 0     : (int)    row["WacIndex"];
      this.SiteIndex        = row.IsNull("SiteIndex")        ? 0     : (int)    row["SiteIndex"];
      this.ProductIndex     = row.IsNull("ProductIndex")     ? 0     : (int)    row["ProductIndex"];
      this.WacValue         = row.IsNull("WacValue")         ? 0.0   : (double) row["WacValue"];
      this.IsManualOverride = row.IsNull("IsManualOverride") ? false : (bool)   row["IsManualOverride"];
      this.Source           = row.IsNull("Source")           ? ""    : (string) row["Source"];
      this.Notes            = row.IsNull("Notes")            ? ""    : (string) row["Notes"];
      this.CreatedBy        = row.IsNull("CreatedBy")        ? ""    : (string) row["CreatedBy"];
      this.UpdatedBy        = row.IsNull("UpdatedBy")        ? ""    : (string) row["UpdatedBy"];

      if (row.IsNull("CreatedDate") == false)
      {
         this.CreatedDate = (DateTimeOffset) row["CreatedDate"];
      }

      if (row.IsNull("UpdatedDate") == false)
      {
         this.UpdatedDate = (DateTimeOffset) row["UpdatedDate"];
      }
   }

   /// <summary>
   /// This method will load the object based on a data set.
   /// </summary>
   /// <param name="dataSet"></param>
   public void Load(DataSet dataSet)
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

   #region SQL Methods
   /// <summary>
   /// This method will create a WAC query to get the most recent WAC for a given
   /// product, site, and date.
   /// </summary>
   /// <returns></returns>
   public string EnumerateSQLBySiteDateProduct()
   {
      string sql = "SELECT TOP (1) * FROM tblWeightedAverageCosts " +
                   "WHERE siteIndex = @SiteIndex AND productIndex = @ProductIndex " +
                   "AND InventoryDate <= @StartDate " +
                   "ORDER BY InventoryDate DESC, CreatedDate DESC ";

      return sql;
   }

   /// <summary>
   /// This method will retrieve the most recent WAC for the site, product, and 
   /// date combination.
   /// </summary>
   /// <param name="siteIndex"></param>
   /// <param name="productIndex"></param>
   /// <param name="startDate"></param>
   public void PerformWACQuery(SqlConnection connection, int siteIndex, int productIndex, DateTime startDate)
   {
      DataSet dataSet    = new DataSet();

      // Retrieve the most recent WAC SQL
      string sql = this.EnumerateSQLBySiteDateProduct();

      SqlCommand command = new SqlCommand(sql, connection);

      command.Parameters.Add("@SiteIndex",    System.Data.SqlDbType.Int);
      command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);
      command.Parameters.Add("@StartDate",    System.Data.SqlDbType.DateTime);

      command.Parameters["@SiteIndex"].Value    = siteIndex;
      command.Parameters["@ProductIndex"].Value = productIndex;
      command.Parameters["@StartDate"].Value    = startDate;

      command.Prepare();

      SqlDataAdapter adapter = new SqlDataAdapter(command);
      adapter.Fill(dataSet);

      // Load the retrieve data set.
      this.Load(dataSet);
   }
   #endregion
}

