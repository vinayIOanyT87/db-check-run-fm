/// <summary>
/// File name:	GetAssociatedParentTxProcessor.cs
/// Purpose:	The purpose of this class is to process the request to retrieve
///            data for the associated parent transactions.
///            
/// Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
/// Author(s):	Richard Panachida
/// Version:	1.0.0  Current version
///	
/// Modification History:
/// Date:		   By:					   Reason:
/// ----------	   -----------------	   ---------------------------------------------------
/// 2010-03-02	   W.Gray					WI 12061 - Correction to join tblTransaction and tblTransactionLineItem on TransIndex
/// 
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;

namespace FMBusinessServices.ServiceClasses
{
	public class GetAssociatedParentTxProcessorClass : IGetAssociatedParentTxProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		private const int InProgressTransactionStatus = 1;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the get transaction processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		/// <param name="accountingServiceImpl"></param>
		public GetAssociatedParentTxProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Methods
		/// <summary>
		/// This method is the entry point for the Get Associated Parent Transaction Processor.
		/// It will return a data object.
		/// </summary>
		/// <param name="accountingSR"></param>
		/// <returns></returns>
		public AssociatedParentTxListDO Process ( GetAssociatedParentTxSR inGetAssociatedParentTxSR )
		{
			GetAssociatedParentTxSR getAssociatedParentTxSR = inGetAssociatedParentTxSR;
			AssociatedParentTxListDO associatedParentTxListDO = null;
			DataSet dataSet = null;

			switch (getAssociatedParentTxSR.SubTypeRequest)
			{
				case GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX:
					dataSet = this.GetDocumentNumbers ( getAssociatedParentTxSR );
					associatedParentTxListDO = this.LoadDocumentNumbers ( dataSet );
					break;
				case GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_LINE:
					dataSet = this.GetCLINs ( getAssociatedParentTxSR );
					associatedParentTxListDO = this.LoadCLINs ( dataSet );
					break;
				case GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_LINE_PER_DOC:
					dataSet = this.GetCLINsBasedOnDocumentNumber ( getAssociatedParentTxSR );
					associatedParentTxListDO = this.LoadCLINs ( dataSet );
					break;
				case GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_TRANSPORT_LINE:
					dataSet = this.GetTransportOrderNumbers ( getAssociatedParentTxSR );
					associatedParentTxListDO = this.LoadTransportOrderNumbers ( dataSet );
					break;
				case GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_TRANSPORT_LINE_PER_DOC:
					dataSet = this.GetTransportOrderNumberBasedOnDocumentNumber ( getAssociatedParentTxSR );
					associatedParentTxListDO = this.LoadTransportOrderNumbers ( dataSet );
					break;
				case GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_TX_BASED_CONTRACT:
					dataSet = this.GetTransIDUsingContractNumber ( getAssociatedParentTxSR );
					associatedParentTxListDO = this.LoadTransIDUsingContractNumber ( dataSet );
					break;
			}

			return associatedParentTxListDO;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will retrieve a TransID that matches the document number (a.k.a. contract number). 
		/// There should only be one TransID.
		/// </summary>
		/// <param name="getAssociatedParentTxSR"></param>
		/// <returns>DataSet</returns>
		private DataSet GetTransIDUsingContractNumber ( GetAssociatedParentTxSR getAssociatedParentTxSR )
		{
			string documentNumber = getAssociatedParentTxSR.AssociatedDocNumber;

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT TransID " +
							 "FROM tblTransactions " +
							 "WHERE DocumentNumber = @DocNumber AND (DeleteFlag = 0 OR DeleteFlag IS NULL) ";
				
				cmd.Parameters.Add("@DocNumber", SqlDbType.NVarChar, 30);

				cmd.Parameters["@DocNumber"].Value = documentNumber;

				dataSet = this.consolidatedDA.GetDataSet(cmd, getAssociatedParentTxSR.Security);
			}

			return dataSet;
		}

		/// <summary>
		/// This method will load the TransID retrieved base on the document number.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns>AssociatedParentTxListDO</returns>
		private AssociatedParentTxListDO LoadTransIDUsingContractNumber ( DataSet dataSet )
		{
			AssociatedParentTxListDO associatedParentTxListDO = new AssociatedParentTxListDO ( );

			if (( dataSet != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable table = dataSet.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					AssociatedParentTxDO associatedParentTxDO = new AssociatedParentTxDO ( );
					associatedParentTxDO.TransID = DataObject.getValue<string>(row["TransID"], "");

					associatedParentTxListDO.Add ( associatedParentTxDO );
				}
			}

			return associatedParentTxListDO;
		}

		/// <summary>
		/// This method will retrieve a collection of document numbers based on all the
		/// parent transactions that are associated to the current transaction.
		/// </summary>
		/// <param name="getAssociatedParentTxSR"></param>
		/// <returns></returns>
		private DataSet GetDocumentNumbers ( GetAssociatedParentTxSR getAssociatedParentTxSR )
		{
			Guid transactionAliasGuid = getAssociatedParentTxSR.TransactionAliasGuid;
			Guid siteGuid = getAssociatedParentTxSR.CurrentSiteGuid;

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT TransID, DocumentNumber, LookupTransTypeIndex, Flag01, Flag02, Flag03, Flag04, Flag05, Flag06 " +
					 "FROM tblTransactions " +
					 "WHERE ISNULL(DeleteFlag, 0) = 0 AND AliasName IN " +
					 "   (SELECT AliasName " +
					 "    FROM tblTransactionAliases " +
					 "    WHERE TransactionAliasGuid IN " +
                     "    (SELECT ParentTransactionAliasGuid FROM map.tblAssociatedTransactionAliases WHERE ChildTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @CurrentTransactionAliasGuid, @CurrentSiteGuid))) " +
					 "    AND (SiteGuid IN (SELECT ParentSiteGuid FROM map.tblSiteToSite " +
					 "                       WHERE ChildSiteGuid = @CurrentSiteGuid AND ParentSiteGuid <> @CurrentSiteGuid) " +
					 "         OR SiteGuid = @CurrentSiteGuid) " +
					 "    AND LookupTransactionStatusIndex = @LookupTransactionStatusIndex ";

				cmd.Parameters.Add("@CurrentTransactionAliasGuid", SqlDbType.UniqueIdentifier).Value = transactionAliasGuid;
				cmd.Parameters.Add("@CurrentSiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
				cmd.Parameters.Add("@LookupTransactionStatusIndex", SqlDbType.Int).Value = GetAssociatedParentTxProcessorClass.InProgressTransactionStatus;
		
				dataSet = this.consolidatedDA.GetDataSet(cmd, getAssociatedParentTxSR.Security);
			}

			return dataSet;
		}

		/// <summary>
		/// This method will load the results of the query into a collection
		/// of data objects.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		private AssociatedParentTxListDO LoadDocumentNumbers ( DataSet dataSet )
		{
			AssociatedParentTxListDO associatedParentTxListDO = new AssociatedParentTxListDO ( );

			if (( dataSet != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable table = dataSet.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					AssociatedParentTxDO associatedParentTxDO = new AssociatedParentTxDO ( );
					associatedParentTxDO.TransTypeID = DataObject.getValue<TransactionTypes>(row["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
					associatedParentTxDO.DocumentNumber	= DataObject.getValue<string>(row["DocumentNumber"], "");
					associatedParentTxDO.TransID			= DataObject.getValue<string>(row["TransID"], "");
					associatedParentTxDO.Flag01			= DataObject.getValue<bool>(row["Flag01"], false);
					associatedParentTxDO.Flag02			= DataObject.getValue<bool>(row["Flag02"], false);
					associatedParentTxDO.Flag03			= DataObject.getValue<bool>(row["Flag03"], false);
					associatedParentTxDO.Flag04			= DataObject.getValue<bool>(row["Flag04"], false);
					associatedParentTxDO.Flag05			= DataObject.getValue<bool>(row["Flag05"], false);
					associatedParentTxDO.Flag06			= DataObject.getValue<bool>(row["Flag06"], false);

					associatedParentTxListDO.Add ( associatedParentTxDO );
				}
			}

			return associatedParentTxListDO;
		}

		/// <summary>
		/// This method will retrieve a collection of CLIN numbers based on a
		/// transaction ID.
		/// </summary>
		/// <param name="getAssociatedParentTxSR"></param>
		/// <returns></returns>
		private DataSet GetCLINs ( GetAssociatedParentTxSR getAssociatedParentTxSR )
		{
			string transID = getAssociatedParentTxSR.TransID;

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT l.CLIN, t.TransID " +
					 "FROM tblTransactions t LEFT OUTER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid " +
					 "WHERE TransID = @TransID ";

				cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64);
				cmd.Parameters["@TransID"].Value = transID;
			
				dataSet = this.consolidatedDA.GetDataSet(cmd, getAssociatedParentTxSR.Security);
			}


			return dataSet;
		}

		/// <summary>
		/// This method will load the results of the query into a collection
		/// of data objects.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		private AssociatedParentTxListDO LoadCLINs ( DataSet dataSet )
		{
			AssociatedParentTxListDO associatedParentTxListDO = new AssociatedParentTxListDO ( );

			if (( dataSet != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable table = dataSet.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					AssociatedParentTxDO associatedParentTxDO = new AssociatedParentTxDO ( );
					associatedParentTxDO.CLIN = DataObject.getValue<string>(row["CLIN"], "");
					associatedParentTxDO.TransID = DataObject.getValue<string>(row["TransID"], "");

					associatedParentTxListDO.Add ( associatedParentTxDO );
				}
			}

			return associatedParentTxListDO;
		}

		/// <summary>
		/// This method will retrieve a list of CLINs based on the associated parent transaction
		/// and the document number.
		/// </summary>
		/// <param name="getAssociatedParentTxSR"></param>
		/// <returns></returns>
		private DataSet GetCLINsBasedOnDocumentNumber ( GetAssociatedParentTxSR getAssociatedParentTxSR )
		{
			string aliasName			= getAssociatedParentTxSR.AliasName;
			Guid transactionAliasGuid = getAssociatedParentTxSR.TransactionAliasGuid;
			Guid siteGuid				= getAssociatedParentTxSR.CurrentSiteGuid;
			string associatedDocNumber	= getAssociatedParentTxSR.AssociatedDocNumber;

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT l.CLIN, t.TransID " +
						 "FROM tblTransactions t LEFT OUTER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid " +
						 "WHERE ISNULL(t.DeleteFlag,0)=0 AND AliasName = " +
						 "   (SELECT AliasName " +
						 "    FROM tblTransactionAliases " +
						 "    WHERE TransactionAliasGuid = " +
                         "      (SELECT ParentTransactionAliasGuid FROM map.tblAssociatedTransactionAliases WHERE ChildTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @CurrentTransactionAliasGuid, @CurrentSiteGuid))) " +
						 "    AND (SiteGuid IN (SELECT ParentSiteGuid FROM map.tblSiteToSite " +
						 "                       WHERE ChildSiteGuid = @CurrentSiteGuid AND ParentSiteGuid <> @CurrentSiteGuid) " +
						 "         OR SiteGuid = @CurrentSiteGuid) " +
						 "    AND t.DocumentNumber = @AssociatedDocumentNumber ";

				ArrayList parameters = new ArrayList();
				cmd.Parameters.Add("@CurrentTransactionAliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CurrentTransactionAliasGuid"].Value = transactionAliasGuid;

				cmd.Parameters.Add("@CurrentSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CurrentSiteGuid"].Value = siteGuid;

				cmd.Parameters.Add("@AssociatedDocumentNumber", SqlDbType.NVarChar, 50);
				cmd.Parameters["@AssociatedDocumentNumber"].Value = associatedDocNumber;

				dataSet = this.consolidatedDA.GetDataSet(cmd, getAssociatedParentTxSR.Security);
			}

			return dataSet;
		}

		/// <summary>
		/// This method will retrieve a list of Transport Document Numbers based on the associated parent transaction
		/// and the document number.
		/// </summary>
		/// <param name="getAssociatedParentTxSR"></param>
		/// <returns></returns>
		private DataSet GetTransportOrderNumberBasedOnDocumentNumber ( GetAssociatedParentTxSR getAssociatedParentTxSR )
		{
			string aliasName			= getAssociatedParentTxSR.AliasName;
			Guid transactionAliasGuid = getAssociatedParentTxSR.TransactionAliasGuid;
			Guid siteGuid				= getAssociatedParentTxSR.CurrentSiteGuid;
			string associatedDocNumber	= getAssociatedParentTxSR.AssociatedDocNumber;

			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT l.TransportOrderNumber, t.TransID " +
						 "FROM tblTransactions t LEFT OUTER JOIN tblTransactionTransportLineItems l ON t.TransactionGuid = l.TransactionGuid " +
						 "WHERE ISNULL(t.DeleteFlag,0)=0 AND AliasName = " +
						 "   (SELECT AliasName " +
						 "    FROM tblTransactionAliases " +
						 "    WHERE TransactionAliasGuid = " +
                         "      (SELECT ParentTransactionAliasGuid FROM map.tblAssociatedTransactionAliases WHERE ChildTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @CurrentTransactionAliasGuid, @CurrentSiteGuid))) " +
						 "    AND (SiteGuid IN (SELECT ParentSiteGuid FROM map.tblSiteToSite " +
						 "                       WHERE ChildSiteGuid = @CurrentSiteGuid AND ParentSiteGuid <> @CurrentSiteGuid) " +
						 "         OR SiteGuid = @CurrentSiteGuid) " +
						 "    AND t.DocumentNumber = @AssociatedDocumentNumber ";

				cmd.Parameters.Add("@CurrentTransactionAliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CurrentTransactionAliasGuid"].Value = transactionAliasGuid;

				cmd.Parameters.Add("@CurrentSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CurrentSiteGuid"].Value = siteGuid;

				cmd.Parameters.Add("@AssociatedDocumentNumber", SqlDbType.NVarChar, 50);
				cmd.Parameters["@AssociatedDocumentNumber"].Value = associatedDocNumber;
		
				dataSet = this.consolidatedDA.GetDataSet(cmd, getAssociatedParentTxSR.Security);
			}

			return dataSet;
		}

		/// <summary>
		/// This method will retrieve a collection of Transport Order Numbers based on a
		/// transaction ID.
		/// </summary>
		/// <param name="getAssociatedParentTxSR"></param>
		/// <returns></returns>
		private DataSet GetTransportOrderNumbers ( GetAssociatedParentTxSR getAssociatedParentTxSR )
		{
			string transID = getAssociatedParentTxSR.TransID;
		
			DataSet dataSet = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT l.TransportOrderNumber, t.TransID " +
					 "FROM tblTransactions t LEFT OUTER JOIN tblTransactionTransportLineItems l ON t.TransactionGuid = l.TransactionGuid " +
					 "WHERE TransID = @TransID ";

	
				cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64);
				cmd.Parameters["@TransID"].Value = transID;

				dataSet = this.consolidatedDA.GetDataSet(cmd, getAssociatedParentTxSR.Security);
			}

			return dataSet;
		}


		/// <summary>
		/// This method will load the results of the query into a collection
		/// of data objects.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		private AssociatedParentTxListDO LoadTransportOrderNumbers ( DataSet dataSet )
		{
			AssociatedParentTxListDO associatedParentTxListDO = new AssociatedParentTxListDO ( );

			if (( dataSet != null ) && ( dataSet.Tables.Count > 0 ))
			{
				DataTable table = dataSet.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					AssociatedParentTxDO associatedParentTxDO = new AssociatedParentTxDO ( );
					associatedParentTxDO.TransportOrderNumber = DataObject.getValue<string>(row["TransportOrderNumber"], "");
					associatedParentTxDO.TransID = DataObject.getValue<string>(row["TransID"], "");

					associatedParentTxListDO.Add ( associatedParentTxDO );
				}
			}

			return associatedParentTxListDO;
		}
		#endregion
	}
}