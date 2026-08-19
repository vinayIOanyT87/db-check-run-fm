/// <summary> =================================================================
///
///	FILE NAME:	FuelTicketLineItems.cs
///
///	PURPOSE:		Declaration of the FuelTicketLineItemsClass
///
///		Copyright (C) 1999-2009	      Varec, Inc.          All Rights Reserved
///										      Norcross, GA, USA
///
///		This file shall not be copied or reproduced in any form without the
///		express written consent of Varec.
///
///		DATE				BY								VERSION		REASON
///		==========		============				========		============================
///		2009-01-01		W. Gray						7.5.0.13		Initial Creation.
///
/// </summary> ================================================================

using System;
using System.Data;
using System.EnterpriseServices;
using ConsolidatedDAL;
using FMCommon;
using WebTicketingDataObjects;


namespace WebTicketingBLL
{
	/// <summary>
	/// Summary description for FuelTicketLineItemsClass.
	/// </summary>
	[Transaction(TransactionOption.Supported, Isolation=TransactionIsolationLevel.ReadCommitted)]
	public class FuelTicketLineItemsClass :	ServicedComponent
	{
		protected ConsolidatedDAClass ConsolidatedDA;

		public FuelTicketLineItemsClass()
		{
		}

		protected override void Activate()
		{
			ConsolidatedDA=new ConsolidatedDAClass();
			if(ConsolidatedDA == null)
				throw(new Exception("new ConsolidatedDAClass"));
		}
 
		protected override bool CanBePooled()
		{
			return false;
		}

		protected override void Deactivate()
		{
			ConsolidatedDA.Dispose();
			ConsolidatedDA=null;
		}


		private void Validate(FuelTicketLineItemClass FuelTicketLineItem)
		{
		}

		[AutoComplete]
		public int Add(SecurityClass security, FuelTicketLineItemClass FuelTicketLineItem)
		{
			if(!ContextUtil.IsInTransaction)
			{
				ServiceConfig sc=new ServiceConfig();
				sc.Transaction=TransactionOption.Required;
				ServiceDomain.Enter(sc);
				FuelTicketLineItemsClass FuelTicketLineItems=new FuelTicketLineItemsClass();
				try
				{
					return FuelTicketLineItems.Add(security,FuelTicketLineItem);
				}
				finally
				{
					FuelTicketLineItems.Dispose();
					ServiceDomain.Leave();
				}
			}

			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(FuelTicketLineItem == null)
				throw new ArgumentNullException("FuelTicketLineItem"); 

			Validate(FuelTicketLineItem);

			if(!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			if(GetIndex(security,FuelTicketLineItem.FuelTicketIndex,FuelTicketLineItem.ItemNumber) != 0)
				throw(new Exception("Fuel Ticket Line Item Exists"));

			FuelTicketLineItem.CreatedDate=DateTime.UtcNow;
			FuelTicketLineItem.CreatedBy=security.UserID;
			FuelTicketLineItem.UpdatedDate=FuelTicketLineItem.CreatedDate;
			FuelTicketLineItem.UpdatedBy=security.UserID;
			FuelTicketLineItem.Index=(int) ConsolidatedDA.ExecuteQuery(security, FuelTicketLineItem.InsertSQL_, ConsolidatedDAClass.Uniquifier).Tables[0].Rows[0][0];

			return FuelTicketLineItem.Index;
		}

		[AutoComplete]
		public void Modify(SecurityClass security,FuelTicketLineItemClass FuelTicketLineItem)
		{
			if(!ContextUtil.IsInTransaction)
			{
				ServiceConfig sc=new ServiceConfig();
				sc.Transaction=TransactionOption.Required;
				ServiceDomain.Enter(sc);
				FuelTicketLineItemsClass FuelTicketLineItems=new FuelTicketLineItemsClass();
				try
				{
					FuelTicketLineItems.Modify(security,FuelTicketLineItem);
					return;
				}
				finally
				{
					FuelTicketLineItems.Dispose();
					ServiceDomain.Leave();
				}
			}

			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(FuelTicketLineItem == null)
				throw new ArgumentNullException("FuelTicketLineItem"); 

			Validate(FuelTicketLineItem);

			if(!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			int Index=GetIndex(security,FuelTicketLineItem.FuelTicketIndex,FuelTicketLineItem.ItemNumber);
			if(Index != 0
			&& Index != FuelTicketLineItem.Index)
				throw(new Exception("FuelTicketLineItem Exists"));


			FuelTicketLineItem.UpdatedDate=DateTime.UtcNow;
			FuelTicketLineItem.UpdatedBy=security.UserID;
			ConsolidatedDA.ExecuteQuery(security, FuelTicketLineItem.UpdateSQL);
		}

		[AutoComplete]
		public FuelTicketLineItemClass Get(SecurityClass security,int Index)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketLineItemClass	FuelTicketLineItem=new FuelTicketLineItemClass();
			FuelTicketLineItem.Index=Index;
			FuelTicketLineItem.Load(ConsolidatedDA.GetDataSet(FuelTicketLineItem.SelectSQL,security));

			return FuelTicketLineItem;
		}


		[AutoComplete]
		public int GetIndex(SecurityClass security,int TicketIndex,byte ItemNumber)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketLineItemClass	FuelTicketLineItem=new FuelTicketLineItemClass();
			FuelTicketLineItem.FuelTicketIndex=TicketIndex;
			FuelTicketLineItem.ItemNumber=ItemNumber;
			FuelTicketLineItem.Load(ConsolidatedDA.GetDataSet(FuelTicketLineItem.SelectByTicketIndexAndItemNumberSQL,security));

			return FuelTicketLineItem.Index;
		}



		[AutoComplete]
		public void Purge(SecurityClass security,int Index)
		{
			if(!ContextUtil.IsInTransaction)
			{
				ServiceConfig sc=new ServiceConfig();
				sc.Transaction=TransactionOption.Required;
				ServiceDomain.Enter(sc);
				FuelTicketLineItemsClass FuelTicketLineItems=new FuelTicketLineItemsClass();
				try
				{
					FuelTicketLineItems.Purge(security,Index);
					return;
				}
				finally
				{
					FuelTicketLineItems.Dispose();
					ServiceDomain.Leave();
				}
			}

			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketLineItemClass	FuelTicketLineItem=new FuelTicketLineItemClass();
			FuelTicketLineItem.Index=Index;

			ConsolidatedDA.ExecuteQuery(security, FuelTicketLineItem.PurgeSQL);
		}


		[AutoComplete]
		public FuelTicketLineItemCollectionClass EnumerateByTicketIndex(SecurityClass security,int TicketIndex)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketLineItemClass FuelTicketLineItem=new FuelTicketLineItemClass();
			FuelTicketLineItem.FuelTicketIndex=TicketIndex;
			DataSet	Set=ConsolidatedDA.GetDataSet(FuelTicketLineItem.EnumerateByTicketIndexSQL,security);
			FuelTicketLineItemCollectionClass	FuelTicketLineItemCollection=new FuelTicketLineItemCollectionClass();

			DataTable Table=Set.Tables[0];
			while(Table.Rows.Count != 0)
			{
				FuelTicketLineItem=new FuelTicketLineItemClass();
				FuelTicketLineItem.Load(Set);
				FuelTicketLineItemCollection.Add(FuelTicketLineItem);
				Table.Rows.RemoveAt(0);
			}

			return FuelTicketLineItemCollection;
		}			
	}
}
