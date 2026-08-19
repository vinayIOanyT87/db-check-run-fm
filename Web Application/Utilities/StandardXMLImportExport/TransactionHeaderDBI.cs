using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for TransactionHeaderDBI.
	/// </summary>
	public class TransactionHeaderDBI : BaseDBI
	{
		#region Attributes
		protected int transactionVersion;

		#endregion Attributes

		public TransactionHeaderDBI(System.Data.SqlClient.SqlConnection conn, string user) : base(conn, user)
		{
		}

		public void Save(SuperTransactionDO trans)
		{
			//If transaction exists, delete and insert, preserving original CreatedBy and CreatedDateTime.
			createdBy = user;
			updatedBy = user;
			createdDateTime = now;
			updatedDateTime = now;
			if(TransactionExists(trans) == true)
			{
				Update(trans);
			}
			else
			{
				Insert(trans);
			}
		}

		protected void Update(SuperTransactionDO trans)
		{
			Delete(trans);
			Insert(trans);
		}

		#region Overrides
		override protected void PrepareInsertStatement()
		{
			insertCmd = new System.Data.SqlClient.SqlCommand();
			insertCmd.Connection = conn;

			insertCmd.CommandText = "INSERT INTO tblTransactions VALUES (" +
				"@TransID, " +
				"@AliasName, " +
				"@TransTypeID, " +
				"@Site, " +
				"@TransReferenceID, " +
				"@InventoryDate, " +
				"@ShipToID, " +
				"@SupplierID, " +
				"@CloseoutDate, " +
				"@CreatedDate, " +
				"@CreatedBy, " +
				"@RequestedDeliveryDate, " +
				"@UpdatedDate, " +
				"@UpdatedBy, " +
				"@TransDateTime, " +
				"@TransVersion, " +
				"@Location, " +
				"@SCACCode, " +
				"@CardNumber, " +
				"@ShipmentNumber, " +
				"@ShipperID, " +
				"@OwnerID, " +
				"@ManagerID, " +
				"@CarrierID, " +
				"@ConjoinTransID, " +
				"@ReversedTransID, " +
				"@LinkedTicketNumber, " +
				"@ReversalType, " +
				"@PONumber, " +
				"@TimeIn, " +
				"@TimeOut, " +
				"@TimeEnd, " +
				"@RoutingID, " +
				"@TicketSource, " +
				"@LoadID, " +
				"@TransactionStatus, " +
				"@BillToID, " +
				"@DriverIdentificationNumber, " +
				"@CreditAmount, " +
				"@CardExpiration, " +
				"@CardName, " +
				"@CardType, " +
				"@CashAmount, " +
				"@RouteOriginationDate, " +
				"@InternationalRouteIndicator, " +
				"@PreviousRoutingID, " +
				"@FinalStation, " +
				"@PreviousStation, " +
				"@NextStation, " +
				"@OriginStation, " +
				"@ShippingDocumentNumber, " +
				"@DocumentNumber, " +
				"@STD, " +
				"@ETD, " +
				"@STA, " +
				"@ETA, " +
				"@SFT, " +
				"@FST, " +
				"@EstimatedFuelingDuration, " +
				"@DeleteFlag" +
				")";

			insertCmd.Parameters.Add("@TransID", System.Data.SqlDbType.NVarChar, 64);
			insertCmd.Parameters.Add("@AliasName", System.Data.SqlDbType.NVarChar, 32);
			insertCmd.Parameters.Add("@TransTypeID", System.Data.SqlDbType.NVarChar, 25);
			insertCmd.Parameters.Add("@Site", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@TransReferenceID", System.Data.SqlDbType.NVarChar, 64);
			insertCmd.Parameters.Add("@InventoryDate", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@ShipToID", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@SupplierID", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@CloseoutDate", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@CreatedBy", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@RequestedDeliveryDate", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@UpdatedDate", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@UpdatedBy", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@TransDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@TransVersion", System.Data.SqlDbType.Int);
			insertCmd.Parameters.Add("@Location", System.Data.SqlDbType.NVarChar, 65);
			insertCmd.Parameters.Add("@SCACCode", System.Data.SqlDbType.NVarChar, 4);
			insertCmd.Parameters.Add("@CardNumber", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@ShipmentNumber", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@ShipperID", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@OwnerID", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@ManagerID", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@CarrierID", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@ConjoinTransID", System.Data.SqlDbType.NVarChar, 64);
			insertCmd.Parameters.Add("@ReversedTransID", System.Data.SqlDbType.NVarChar, 64);
			insertCmd.Parameters.Add("@LinkedTicketNumber", System.Data.SqlDbType.NVarChar, 64);
			insertCmd.Parameters.Add("@ReversalType", System.Data.SqlDbType.Char, 1);
			insertCmd.Parameters.Add("@PONumber", System.Data.SqlDbType.NVarChar, 14);
			insertCmd.Parameters.Add("@TimeIn", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@TimeOut", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@TimeEnd", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@RoutingID", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@TicketSource", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@LoadID", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@TransactionStatus", System.Data.SqlDbType.Int);
			insertCmd.Parameters.Add("@BillToID", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@DriverIdentificationNumber", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@CreditAmount", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@CardExpiration", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@CardName", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@CardType", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@CashAmount", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@RouteOriginationDate", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@InternationalRouteIndicator", System.Data.SqlDbType.Bit);
			insertCmd.Parameters.Add("@PreviousRoutingID", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@FinalStation", System.Data.SqlDbType.NVarChar, 4);
			insertCmd.Parameters.Add("@PreviousStation", System.Data.SqlDbType.NVarChar, 4);
			insertCmd.Parameters.Add("@NextStation", System.Data.SqlDbType.NVarChar, 4);
			insertCmd.Parameters.Add("@OriginStation", System.Data.SqlDbType.NVarChar, 4);
			insertCmd.Parameters.Add("@ShippingDocumentNumber", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@DocumentNumber", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@STD", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@ETD", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@STA", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@ETA", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@SFT", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@FST", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@EstimatedFuelingDuration", System.Data.SqlDbType.Int);
			insertCmd.Parameters.Add("@DeleteFlag", System.Data.SqlDbType.Bit);

			insertCmd.Prepare();
		}
		
		override protected void PrepareSelectStatement()
		{
			selectCmd = new System.Data.SqlClient.SqlCommand();
			selectCmd.Connection = conn;
			selectCmd.CommandText = "SELECT CreatedBy, CreatedDate FROM tblTransactions WHERE TransID = @TransID";
			selectCmd.Parameters.Add("@TransID", System.Data.SqlDbType.NVarChar, 64, "TransID");
			selectCmd.Prepare();
		}

		override protected void PrepareDeleteStatement()
		{
			deleteCmd = new System.Data.SqlClient.SqlCommand();
			deleteCmd.Connection = conn;
			deleteCmd.CommandText = "DELETE FROM tblTransactions WHERE TransID = @TransID";
			deleteCmd.Parameters.Add("@TransID", System.Data.SqlDbType.NVarChar, 64, "TransID");
			deleteCmd.Prepare();
		}
		#endregion Overrides

		protected void Insert(SuperTransactionDO trans)
		{
			int i = 0;
			insertCmd.Parameters[i++].Value = trans.TransID;
			insertCmd.Parameters[i++].Value = trans.Alias;
			insertCmd.Parameters[i++].Value = trans.TransTypeID;
			insertCmd.Parameters[i++].Value = trans.Site;
			insertCmd.Parameters[i++].Value = trans.TransRefID;
			insertCmd.Parameters[i++].Value = trans.InventoryDate;
			insertCmd.Parameters[i++].Value = trans.ShipTo;
			insertCmd.Parameters[i++].Value = trans.Supplier;
			insertCmd.Parameters[i++].Value = trans.CloseoutDateTime;
			insertCmd.Parameters[i++].Value = createdDateTime;
			insertCmd.Parameters[i++].Value = createdBy;
			insertCmd.Parameters[i++].Value = (trans.RequestedDeliveryDate == null) ? null : (object) trans.RequestedDeliveryDate.Value;
			insertCmd.Parameters[i++].Value = updatedDateTime;
			insertCmd.Parameters[i++].Value = updatedBy;
			insertCmd.Parameters[i++].Value = trans.TransactionDateTime;
			insertCmd.Parameters[i++].Value = GetSequenceValue("TransactionVersion");
			insertCmd.Parameters[i++].Value = trans.Location;
			insertCmd.Parameters[i++].Value = trans.SCACCode;
			insertCmd.Parameters[i++].Value = trans.PaymentInfo.CreditCardNumber;
			insertCmd.Parameters[i++].Value = trans.ShipmentNumber;
			insertCmd.Parameters[i++].Value = trans.Shipper;
			insertCmd.Parameters[i++].Value = trans.Owner;
			insertCmd.Parameters[i++].Value = trans.Manager;
			insertCmd.Parameters[i++].Value = trans.Carrier;
			insertCmd.Parameters[i++].Value = trans.ConjoinedTransID;
			insertCmd.Parameters[i++].Value = trans.ReversedTransID;
			insertCmd.Parameters[i++].Value = trans.LinkedDocumentNumber;
			insertCmd.Parameters[i++].Value = trans.ReversalType;
			insertCmd.Parameters[i++].Value = trans.PONumber;
			insertCmd.Parameters[i++].Value = trans.TimeIn;
			insertCmd.Parameters[i++].Value = trans.TimeOut;
			insertCmd.Parameters[i++].Value = trans.TimeEnd;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.RoutingID;
			insertCmd.Parameters[i++].Value = trans.TicketSource;
			insertCmd.Parameters[i++].Value = trans.LoadID;
			insertCmd.Parameters[i++].Value = trans.TransactionStatus;
			insertCmd.Parameters[i++].Value = trans.BillTo;
			insertCmd.Parameters[i++].Value = trans.DriverIDNumber;
			insertCmd.Parameters[i++].Value = trans.PaymentInfo.CreditCardAmount;
			insertCmd.Parameters[i++].Value = trans.PaymentInfo.CreditCardExpiration;
			insertCmd.Parameters[i++].Value = trans.PaymentInfo.CreditCardName;
			insertCmd.Parameters[i++].Value = trans.PaymentInfo.CreditCardType;
			insertCmd.Parameters[i++].Value = trans.PaymentInfo.CashAmount;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.RouteOriginationDate;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.InternationalRouteIndicator;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.PreviousRoutingID;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.FinalStation;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.PreviousStation;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.NextStation;
			insertCmd.Parameters[i++].Value = trans.RouteInfo.OriginStation;
			insertCmd.Parameters[i++].Value = trans.ShippingDocumentNumber;
			insertCmd.Parameters[i++].Value = trans.DocumentNumber;
			insertCmd.Parameters[i++].Value = trans.RouteSchedule.STD;
			insertCmd.Parameters[i++].Value = trans.RouteSchedule.ETD;
			insertCmd.Parameters[i++].Value = trans.RouteSchedule.STA;
			insertCmd.Parameters[i++].Value = trans.RouteSchedule.ETA;
			insertCmd.Parameters[i++].Value = trans.RouteSchedule.SFT;
			insertCmd.Parameters[i++].Value = trans.RouteSchedule.FST;
			insertCmd.Parameters[i++].Value = (trans.EstimatedFuelingDuration == null) ? null : (object) trans.EstimatedFuelingDuration.Value;
			insertCmd.Parameters[i++].Value = trans.DeleteFlag;

			foreach(System.Data.SqlClient.SqlParameter param in insertCmd.Parameters)
			{
				if(param.Value == null)
				{
					param.Value = System.DBNull.Value;
				}
			}

			int result = insertCmd.ExecuteNonQuery();		
		}

		public void Delete(SuperTransactionDO trans)
		{
			deleteCmd.Parameters[0].Value = trans.TransID;
			int result = deleteCmd.ExecuteNonQuery();
			System.Diagnostics.Debug.Assert(result == 1,
				"ImportProcessor.DeleteTransaction() deleted " + result + " transactions.");
		}

		protected bool TransactionExists(SuperTransactionDO trans)
		{
			bool exists = false;
			selectCmd.Parameters[0].Value = trans.TransID;
			System.Data.SqlClient.SqlDataReader reader = selectCmd.ExecuteReader();
			if(reader.HasRows)
			{
				reader.Read();
				if(! reader.IsDBNull(0))
				{
					createdBy = reader.GetString(0);
				}
				if(! reader.IsDBNull(1))
				{
					createdDateTime = reader.GetDateTime(1);
				}
				exists = true;
			}
			reader.Close();
			return exists;
		}


	}
}
