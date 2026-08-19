/// <summary>
/// File name:	ExcelImport.cs
/// Purpose:	Read and parse the ground fuel transactions.
///				
/// Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
/// Author(s):	Richard R. Panachida
/// Version:	1.0.0  Current version
///	
/// Modification History:
///	Date:			By:					Reason:
///	----------  ----------------  ------------------------------------------------------
///	2008-10-02	Bill Dimovski		Modified (and renamed) orginal TFMSParser import from
///											Excel Worksheets (CSI 385).
///	2008-12-17  Bill Dimovski       Modified to improve error handling if invalid Excel
///	                                spreadsheets/worksheet's have been uploaded.
/// </summary>
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using System.Collections;
using System.Data.OleDb;

using FMBusinessObjects.DataObjects;

namespace ADFWebApp
{
   public class ExcelImport
   {
      #region Private data members
      private ArrayList TFMSList;
      #endregion

      #region Constructor
      /// <summary>
      /// This is the default constructor for the ExcelImport class.
      /// </summary>
      public ExcelImport()
      {
      }
      #endregion

      #region Properties
      /// <summary>
      /// This property will return the collection of TFMS data objects.
      /// </summary>
      public ArrayList TFMSCollection
      {
         get { return this.TFMSList; }
      }
      #endregion

      #region Public methods
      /// <summary>
      /// This method will read from an Excel Worksheet (Ground Fuel Transactions).
      /// </summary>
      /// <param name="file"></param>
      /// <param name="worksheetName"></param>
      public void ReadWorksheet(string file, string worksheetName)
      {
         bool errorFlag      = false;
         string errMsg       = "";
         DataTable dataTable = new DataTable();

         try
         {
            string connectionString      = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                           "Data Source=" + file + ";" +
                                           "Extended Properties=Excel 8.0;";
            OleDbConnection connection   = new OleDbConnection(connectionString);
            OleDbCommand dbCommand       = new OleDbCommand("SELECT * FROM [" + worksheetName + "$]", connection);
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter();
            dataAdapter.SelectCommand    = dbCommand;
            dataAdapter.Fill(dataTable);
         }
         catch (Exception ex)
         {
            errMsg += ex.Message;
            errorFlag = true;
         }

         TFMSDO tfmsDO;
         this.TFMSList = new ArrayList();
         int recordCount = 0;

         // If there is a phantom row (1 row with empty values), then just return.
         if (dataTable.Rows.Count > 0)
         {
            if (this.HasPhantomRow(dataTable.Rows.Count, dataTable) == true)
            {
               return;
            }
         }

         for (int nextRow = 0; nextRow < dataTable.Rows.Count; nextRow++)
         {
            recordCount++;

            if (dataTable.Columns.Count != 20)
            {
               throw new Exception("Record #" + recordCount.ToString() +
                                   " has invalid length of " +
                                   dataTable.Columns.Count.ToString() + " columns.\n");
            }

            try
            {
               tfmsDO                           = new TFMSDO();
               tfmsDO.PurchaseNumber            = dataTable.Rows[nextRow]["Direct Fuel Purchase Number"].ToString().Replace("\"", "");
               tfmsDO.Location                  = dataTable.Rows[nextRow]["Location"].ToString().Replace("\"", "");
               tfmsDO.DateTime                  = this.ConvertToDate(dataTable.Rows[nextRow]["Transaction Date"].ToString().Replace("\"", ""), 3, recordCount);
               tfmsDO.Customer                  = dataTable.Rows[nextRow]["Customer"].ToString().Replace("\"", "");
               tfmsDO.Supplier                  = dataTable.Rows[nextRow]["Supplier"].ToString().Replace("\"", "");
               tfmsDO.Quantity                  = this.ConvertToDouble(dataTable.Rows[nextRow]["Quantity"].ToString(), 6, recordCount);
               tfmsDO.UOM                       = dataTable.Rows[nextRow]["UOM"].ToString().Replace("\"", "");
               tfmsDO.UOMQuantity               = this.ConvertToDouble(dataTable.Rows[nextRow]["Quantity in UOM Chosen"].ToString(), 8, recordCount);
               tfmsDO.Product                   = dataTable.Rows[nextRow]["Fuel Type"].ToString().Replace("\"", "");
               tfmsDO.DefenseAssetID            = dataTable.Rows[nextRow]["Defence Asset ID"].ToString().Replace("\"", "");
               tfmsDO.Country                   = dataTable.Rows[nextRow]["Country"].ToString().Replace("\"", "");
               tfmsDO.FuelPriceAUD              = this.ConvertToDouble(dataTable.Rows[nextRow]["Fuel Price (AUD)"].ToString(), 12, recordCount);
               tfmsDO.TotalPriceAUD             = this.ConvertToDouble(dataTable.Rows[nextRow]["Total Price (AUD)"].ToString(), 13, recordCount);
               tfmsDO.GST                       = this.ConvertToDouble(dataTable.Rows[nextRow]["GST"].ToString(), 14, recordCount);
               tfmsDO.Excise                    = this.ConvertToDouble(dataTable.Rows[nextRow]["Excise"].ToString(), 15, recordCount);
               tfmsDO.ForeignCurrencyUnit       = dataTable.Rows[nextRow]["Foreign Currency"].ToString().Replace("\"", "");
               tfmsDO.ForeignCurrencyPrice      = this.ConvertToDouble(dataTable.Rows[nextRow]["Invoice Foreign Currency Price"].ToString(), 17, recordCount);
               tfmsDO.TotalForeignCurrencyPrice = this.ConvertToDouble(dataTable.Rows[nextRow]["Total Foreign Price"].ToString(), 18, recordCount);
               tfmsDO.FuelCardNumber            = dataTable.Rows[nextRow]["Fuel Card Number"].ToString().Replace("\"", "").Trim();
               tfmsDO.Notes                     = dataTable.Rows[nextRow]["Notes"].ToString().Replace("\"", "");
               this.TFMSList.Add(tfmsDO);
            }
            catch (Exception ex)
            {
               errMsg += ex.Message;
               errorFlag = true;
            }
            if (errorFlag == true)
            {
               throw new Exception(errMsg);
            }
         }
      }
      #endregion

      #region Private methods
      /// <summary>
      /// This method will detect if there is a phantom row in the spreadsheet. If all the 
      /// fields on the first row are null or have a blank string, then the row is a phantom
      /// row. This method will return true if the first row exists and all the fields are
      /// empty.  Otherwise, it returns false.
      /// </summary>
      /// <param name="rowCount"></param>
      /// <param name="dataTable"></param>
      /// <returns></returns>
      private bool HasPhantomRow(int rowCount, DataTable dataTable)
      {
         bool phantomRow = false;

         if (rowCount == 1)
         {
            string PurchaseNumber            = dataTable.Rows[0]["Direct Fuel Purchase Number"].ToString().Replace("\"", "");
            string Location                  = dataTable.Rows[0]["Location"].ToString().Replace("\"", "");
            string DateTime                  = dataTable.Rows[0]["Transaction Date"].ToString().Replace("\"", "");
            string Customer                  = dataTable.Rows[0]["Customer"].ToString().Replace("\"", "");
            string Supplier                  = dataTable.Rows[0]["Supplier"].ToString().Replace("\"", "");
            string Quantity                  = dataTable.Rows[0]["Quantity"].ToString();
            string UOM                       = dataTable.Rows[0]["UOM"].ToString().Replace("\"", "");
            string UOMQuantity               = dataTable.Rows[0]["Quantity in UOM Chosen"].ToString();
            string Product                   = dataTable.Rows[0]["Fuel Type"].ToString().Replace("\"", "");
            string DefenseAssetID            = dataTable.Rows[0]["Defence Asset ID"].ToString().Replace("\"", "");
            string Country                   = dataTable.Rows[0]["Country"].ToString().Replace("\"", "");
            string FuelPriceAUD              = dataTable.Rows[0]["Fuel Price (AUD)"].ToString();
            string TotalPriceAUD             = dataTable.Rows[0]["Total Price (AUD)"].ToString();
            string GST                       = dataTable.Rows[0]["GST"].ToString();
            string Excise                    = dataTable.Rows[0]["Excise"].ToString();
            string ForeignCurrencyUnit       = dataTable.Rows[0]["Foreign Currency"].ToString().Replace("\"", "");
            string ForeignCurrencyPrice      = dataTable.Rows[0]["Invoice Foreign Currency Price"].ToString();
            string TotalForeignCurrencyPrice = dataTable.Rows[0]["Total Foreign Price"].ToString();
            string Notes                     = dataTable.Rows[0]["Notes"].ToString().Replace("\"", "");

            if ((string.IsNullOrEmpty(PurchaseNumber) == true)
               && (string.IsNullOrEmpty(Location) == true)
               && (string.IsNullOrEmpty(DateTime) == true)
               && (string.IsNullOrEmpty(Customer) == true)
               && (string.IsNullOrEmpty(Supplier) == true)
               && (string.IsNullOrEmpty(Quantity) == true)
               && (string.IsNullOrEmpty(UOM) == true)
               && (string.IsNullOrEmpty(UOMQuantity) == true)
               && (string.IsNullOrEmpty(Product) == true)
               && (string.IsNullOrEmpty(Country) == true)
               && (string.IsNullOrEmpty(FuelPriceAUD) == true)
               && (string.IsNullOrEmpty(TotalPriceAUD) == true)
               && (string.IsNullOrEmpty(GST) == true)
               && (string.IsNullOrEmpty(Excise) == true)
               && (string.IsNullOrEmpty(ForeignCurrencyUnit) == true)
               && (string.IsNullOrEmpty(ForeignCurrencyPrice) == true)
               && (string.IsNullOrEmpty(TotalForeignCurrencyPrice) == true)
               && (string.IsNullOrEmpty(Notes) == true))
            {
               phantomRow = true;
            }
         }

         return phantomRow;
      }

      /// <summary>
      /// This method will convert the date/time string into a datetime object. It will throw
      /// an exception if invalid.
      /// </summary>
      /// <param name="date"></param>
      /// <param name="fieldNumber"></param>
      /// <param name="recordCount"></param>
      /// <returns></returns>
      private DateTime? ConvertToDate(string date, int fieldNumber, int recordCount)
      {
         string msg = "";
         DateTime? newDateTime = null;

         if (string.IsNullOrEmpty(date) == false)
         {
            try
            {
               newDateTime = DateTime.Parse(date);
               return newDateTime;
            }
            catch (Exception)
            {
               msg = "Field #" + fieldNumber.ToString() + " in record #" + recordCount.ToString() +
                    " invalid date.\n";
               throw new Exception(msg);
            }
         }
         else
         {
            msg = "Date is required in Field #" + fieldNumber.ToString() + " in record #" + recordCount.ToString() + " \n";
            throw new Exception(msg);
         }
      }

      /// <summary>
      /// This method will convert a string to a VDouble. It will return null if the
      /// string is empty. It will throw an exception if the field is not numeric.
      /// </summary>
      /// <param name="field"></param>
      /// <param name="fieldNumber"></param>
      /// <param name="recordCount"></param>
      /// <returns></returns>
      private double? ConvertToDouble(string field, int fieldNumber, int recordCount)
      {
         if (string.IsNullOrEmpty(field) == true)
         {
            return null;
         }

         try
         {
            return Convert.ToDouble(field);
         }
         catch (System.Exception)
         {
            string msg = "Field #" + fieldNumber.ToString() + " in record #" + recordCount.ToString() +
                        "'" + field + "' is not numeric.\n";
            throw new System.Exception(msg);
         }
      }

      /// <summary>
      /// This method will convert a string to an integer. It will return null if the
      /// string is empty. It will throw an exception if the field is not numeric.
      /// </summary>
      /// <param name="field"></param>
      /// <param name="fieldNumber"></param>
      /// <param name="recordCount"></param>
      /// <returns></returns>
      private int? ConvertToInteger(string field, int fieldNumber, int recordCount)
      {
         if (string.IsNullOrEmpty(field) == true)
         {
            return null;
         }

         try
         {
            return Convert.ToInt32(field);
         }
         catch (System.Exception)
         {
            string msg = "Field #" + fieldNumber.ToString() + " in record #" + recordCount.ToString() +
                         "'" + field + "' is not numeric.\n";
            throw new System.Exception(msg);
         }
      }
      #endregion
   }
}
