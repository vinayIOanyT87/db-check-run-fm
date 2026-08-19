using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class BSMEVerificationsClass : IBSMEVerifications
	{
		private ConsolidatedDAClass consolidatedDA;

		public BSMEVerificationsClass ()
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}

		public string FindMatchShipment ( string shipmentNumber, SecurityClass security )
		{
			if (string.IsNullOrEmpty ( shipmentNumber ) == true)
			{
				throw new Exception ( "Shipment number is invalid." );
			}

			string docNumber = "";

			using (SqlCommand cmd = new SqlCommand())
			{
				string select		= "SELECT u.UserData16 ";
				string from			= "FROM tblTransactionUserData u LEFT OUTER JOIN tblTransactions t ON u.TransIndex = t.TransIndex ";
				string where		= "WHERE u.UserData13 = @ShipmentNumber " +
								  "AND t.AliasName = 'SHIPMENT - TRANSFER' AND t.DeleteFlag = 0";

				cmd.CommandText = select + from + where;

				SqlParameter parm = new SqlParameter( "@ShipmentNumber", SqlDbType.NVarChar, 100 );
				parm.Value = shipmentNumber;
				cmd.Parameters.Add( parm );

				try
				{
					DataSet dataSet = this.consolidatedDA.GetDataSet( cmd, security );

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						DataTable table = dataSet.Tables[0];

						if (table.Rows.Count > 0)
						{
							DataRow row = table.Rows[0];
							string docNum = DataObject.getValue<string>(row["UserData16"], "");

							if (string.IsNullOrEmpty( docNum ) == false)
							{
								docNumber = docNum;
							}
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine( ex.Message );
					throw new Exception( "Error searching matching shipment number. " + ex.Message );
				}

				return docNumber;
			}
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public bool CheckOrUpdateSuspenseStatus( string strNumber, string docNumber, SecurityClass security )
		{
			if (( string.IsNullOrEmpty ( strNumber ) == true ) || ( string.IsNullOrEmpty ( docNumber ) == true ))
			{
				throw new Exception ( "Number or Document Number is invalid." );
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				string update	= "Update tblTransactions SET t.TransactionStatus = @TransStatus, u.UserData16 = @DocNumber ";
				string from		= "FROM tblTransactionUserData u LEFT OUTER JOIN tblTransactions t ON u.TransIndex = t.TransIndex ";
				string where	= "WHERE u.UserData13 = @StrNumber AND t.AliasName like 'RECEIVE%' AND t.DeleteFlag = 0 " +
							  "AND t.TransactionStatus = @TransStatus2 ";

				cmd.CommandText = update + from + where;

				SqlParameter parm = new SqlParameter( "@TransStatus", SqlDbType.Int );
				parm.Value = (int)TransactionStatus.Completed;
				cmd.Parameters.Add( parm );

				parm = new SqlParameter( "@DocNumber", SqlDbType.NVarChar, 100 );
				parm.Value = docNumber;
				cmd.Parameters.Add( parm );

				parm = new SqlParameter( "@StrNumber", SqlDbType.NVarChar, 100 );
				parm.Value = strNumber;
				cmd.Parameters.Add( parm );

				parm = new SqlParameter( "@TransStatus2", SqlDbType.Int );
				parm.Value = (int)TransactionStatus.Suspended;
				cmd.Parameters.Add( parm );

				try
				{
					DataSet dataSet = this.consolidatedDA.GetDataSet( cmd, security );

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						DataTable table = dataSet.Tables[0];
						if (table.Rows.Count > 0)
						{
							return true;
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine( ex.Message );
					throw new Exception( "Error updating status. " + ex.Message );
				}
			}

			return false;
		}

		public string GetNextDocumentSeqNumber ( string partialDocNumber, string aliasName, SecurityClass security )
		{
			if (( string.IsNullOrEmpty ( partialDocNumber ) == true ) || ( string.IsNullOrEmpty ( aliasName ) == true ))
			{
				throw new Exception ( "Alias name or Partial Document Number is invalid." );
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				string seqNumber	= "01";
				string select		= "SELECT TOP(1) u.UserData16 ";
				string from			= "FROM tblTransactionUserData u LEFT OUTER JOIN tblTransactions t ON u.TransIndex = t.TransIndex ";
				string where		= "WHERE u.UserData16 LIKE (@PartialDocNumber) " +
								  "AND t.AliasName = @AliasName";
				string orderBy		= "ORDER BY 1 DESC ";

				cmd.CommandText = select + from + where + orderBy;

				SqlParameter parm = new SqlParameter( "@PartialDocNumber", SqlDbType.NVarChar, 100 );
				parm.Value = partialDocNumber + "%";
				cmd.Parameters.Add( parm );

				parm = new SqlParameter( "@AliasName", SqlDbType.NVarChar, 100 );
				parm.Value = aliasName;
				cmd.Parameters.Add( parm );

				try
				{
					DataSet dataSet = this.consolidatedDA.GetDataSet( cmd, security );

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						DataTable table = dataSet.Tables[0];

						if (table.Rows.Count > 0)
						{
							DataRow row = table.Rows[0];
							string docNumber = DataObject.getValue<string>(row["UserData16"], "");

							if ((docNumber != null) && (docNumber.Length == 14))
							{
								seqNumber = docNumber.Substring( 12, 2 );

								int seqNum = Convert.ToInt32( seqNumber );
								seqNum++;

								if (seqNum > 99)
								{
									seqNum = 1;
								}

								if (seqNum < 10)
								{
									seqNumber = "0" + seqNum.ToString();
								}
								else
								{
									seqNumber = seqNum.ToString();
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception( "Error getting next document sequence number. " + ex.Message );
				}

				return seqNumber;
			}
		}

		public bool IsShippingTransactionDuplicate ( TransactionDO transDO, SecurityClass security )
		{
			string shippingTransID = transDO.UserData[TransactionDO.USER_DATA_KEY_15] as string;

			if (string.IsNullOrEmpty ( shippingTransID ) == true)
			{
				return true;
			}

			bool isDuplicate = true;
			
			string sql = "SELECT COUNT(u.userdata15) AS IDCount FROM tblTransactionUserData u WHERE u.UserData15 = @TransID";

			using (SqlCommand cmd = new SqlCommand( sql ))
			{
				cmd.Parameters.Add( "@TransID", SqlDbType.NVarChar, 64 );
				cmd.Parameters["@TransID"].Value = shippingTransID;

				DataSet dataSet = this.consolidatedDA.GetDataSet( cmd, security );

				if (dataSet != null)
				{
					DataTable table = dataSet.Tables[0];

					if (table != null)
					{
						DataRow row = table.Rows[0];

						if (row != null)
						{
							int count = DataObject.getValue<int>(row["IDCount"], -1);
							if (count <= 0)
							{
								isDuplicate = false;
							}
						}
					}
				}

				return isDuplicate;
			}
		}

		public DataTable LoadSuspenseData ( SecurityClass security )
		{
			DataTable dataTable = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				string select	= "SELECT u.UserData16, u.UserData13, t.TransID, t.Site, t.ShipperID, l.Product, l.GrossQuantity, " +
							  "t.TransDateTime, u.UserData9 ";
				string from		= "FROM tblTransactions t LEFT JOIN tblTransactionUserData u " +
							  "ON t.TransIndex = u.TransIndex JOIN tblTransactionLineItems l ON u.TransIndex = l.TransIndex ";
				string where	= "Where t.TransID like 'MFCS%' AND t.AliasName like 'RECEIVE%' AND t.DeleteFlag = 0 AND " +
							  "t.TransactionStatus = @TransStatus AND t.Site = @SiteID ";

				cmd.CommandText = select + from + where;

				SqlParameter parm = new SqlParameter( "@TransStatus", SqlDbType.Int );
				parm.Value = (int)TransactionStatus.Suspended;
				cmd.Parameters.Add( parm );

				parm = new SqlParameter( "@SiteID", SqlDbType.Int );
				parm.Value = security.SiteID;
				cmd.Parameters.Add( parm );

				try
				{
					DataSet ds;
					ds = this.consolidatedDA.GetDataSet( cmd, security );

					if ((ds != null) && (ds.Tables.Count > 0))
					{
						dataTable = ds.Tables[0];
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine( ex.Message );
					throw new Exception( "Error retrieving load suspense data. " + ex.Message );
				}

				return dataTable;
			}
		}

		public string GetShipmentDocumentNumber ( string shipment, SecurityClass security )
		{
			string documentNumber = "";

			if (string.IsNullOrEmpty ( shipment ) == true)
			{
				return documentNumber;
			}

			try
			{
				using (SqlCommand cmd	= new SqlCommand())
				{
					string select	= "SELECT u.UserData16  ";
					string from		= "FROM tblTransactionUserData u LEFT OUTER JOIN tblTransactions t ON u.TransIndex = t.TransIndex ";
					string where	= "WHERE u.UserData13 = @Shipment " +
								  "AND t.AliasName = 'SHIPMENT - TRANSFER' AND t.DeleteFlag = 0";

					cmd.CommandText = select + from + where;

					SqlParameter parm = new SqlParameter( "@Shipment", SqlDbType.NVarChar, 100 );
					parm.Value = shipment;
					cmd.Parameters.Add( parm );

					DataSet dataSet = this.consolidatedDA.GetDataSet( cmd, security );

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						DataTable table = dataSet.Tables[0];
						if (table.Rows.Count > 0)
						{
							DataRow row = table.Rows[0];
							documentNumber = DataObject.getValue<string>(row["UserData16"], "");
						}
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine ( ex.Message );
				throw new Exception ( "Error retrieving shipment number. " + ex.Message );
			}

			return documentNumber;
		}

		public bool DuplicateDocNumber ( string docNumber, string aliasName, SecurityClass security )
		{
			bool duplicate = false;

			if (( string.IsNullOrEmpty ( docNumber ) == true ) || ( string.IsNullOrEmpty ( aliasName ) == true ))
			{
				return duplicate;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				string select	= "SELECT u.UserData16 ";
				string from		= "FROM tblTransactionUserData u LEFT OUTER JOIN tblTransactions t ON u.TransIndex = t.TransIndex ";
				string where	= "WHERE u.UserData16 = @DocNumber  AND t.AliasName = @AliasName ";

				cmd.CommandText = select + from + where;

				SqlParameter parm = new SqlParameter( "@DocNumber", SqlDbType.NVarChar, 100 );
				parm.Value = docNumber;
				cmd.Parameters.Add( parm );

				parm = new SqlParameter( "@AliasName", SqlDbType.NVarChar, 100 );
				parm.Value = aliasName;
				cmd.Parameters.Add( parm );

				try
				{
					DataSet dataSet = this.consolidatedDA.GetDataSet( cmd, security );

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						DataTable table = dataSet.Tables[0];

						if (table.Rows.Count > 0)
						{
							DataRow row = table.Rows[0];
							string docNum = DataObject.getValue<string>(row["UserData16"], "");

							if (string.IsNullOrEmpty( docNumber ) == false)
							{
								duplicate = true;
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception( "Error determining duplication document number. " + ex.Message );
				}

				return duplicate;
			}
		}
	}
}