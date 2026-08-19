/// <summary> =================================================================
///
///	FILE NAME:	FuelTicket.cs
///
///	PURPOSE:		Declaration of the FuelTicket class
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
using ConsolidatedBLL;
using ConsolidatedDAL;
using ConsolidatedDataObjects;
using FM7Accounting;
using FMCommon;
using WebTicketingDataObjects;

namespace WebTicketingBLL
{
	/// <summary>
	/// Summary description for FuelTicketsClass.
	/// </summary>
	[Transaction(TransactionOption.Supported, Isolation=TransactionIsolationLevel.ReadCommitted)]
	public class FuelTicketsClass :	ServicedComponent,
												IDependency
	{
		protected ConsolidatedDAClass ConsolidatedDA;

		public FuelTicketsClass()
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


		private void Validate(FuelTicketClass FuelTicket)
		{
		}

		protected void UpdateLineItems(SecurityClass security,FuelTicketClass FuelTicket)
		{
			FuelTicketLineItemsClass FuelTicketLineItems=new FuelTicketLineItemsClass();
			FuelTicketLineItemCollectionClass	ExistingItemCollection=FuelTicketLineItems.EnumerateByTicketIndex(security,FuelTicket.Index);

			if(FuelTicket.FuelTicketLineItemCollection != null)
			{
				foreach(FuelTicketLineItemClass Item in FuelTicket.FuelTicketLineItemCollection)
				{
					Item.FuelTicketIndex=FuelTicket.Index;

					int Index=0;
					foreach(FuelTicketLineItemClass ExistingItem in ExistingItemCollection)
					{
						if(Item.Index == 0)
						{
							if(ExistingItem.ItemNumber == Item.ItemNumber)
							{
								Item.Index=ExistingItem.Index;
								FuelTicketLineItems.Modify(security,Item);
								break;
							}
						}
								
						else if(ExistingItem.Index == Item.Index)
						{
							FuelTicketLineItems.Modify(security,Item);
							break;
						}
						Index++;
					}

					if(Index == ExistingItemCollection.Count)
						FuelTicketLineItems.Add(security,Item);
					else
						ExistingItemCollection.Remove(Index);
				}
			}

			foreach(FuelTicketLineItemClass ExistingItem in ExistingItemCollection)
				FuelTicketLineItems.Purge(security,ExistingItem.Index);

			FuelTicketLineItems.Dispose();
		}

		[AutoComplete]
		public int Add(SecurityClass security, FuelTicketClass FuelTicket)
		{
			if(!ContextUtil.IsInTransaction)
			{
				ServiceConfig sc=new ServiceConfig();
				sc.Transaction=TransactionOption.Required;
				ServiceDomain.Enter(sc);
				FuelTicketsClass FuelTickets=new FuelTicketsClass();
				try
				{
					return FuelTickets.Add(security,FuelTicket);
				}
				finally
				{
					FuelTickets.Dispose();
					ServiceDomain.Leave();
				}
			}

			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(FuelTicket == null)
				throw new ArgumentNullException("FuelTicket"); 

			Validate(FuelTicket);

			if(!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicket.SiteIndex=security.SiteIndex;
			FuelTicket.UserIndex=security.UserIndex;
			FuelTicket.SequenceNumber=GetNextSequenceNumber(security);
			FuelTicket.CreatedDate=DateTime.UtcNow;
			FuelTicket.CreatedBy=security.UserID;
			FuelTicket.UpdatedDate=FuelTicket.CreatedDate;
			FuelTicket.UpdatedBy=security.UserID;
			FuelTicket.Index=(int) ConsolidatedDA.ExecuteQuery(security, FuelTicket.InsertSQL_, ConsolidatedDAClass.Uniquifier).Tables[0].Rows[0][0];

			UpdateLineItems(security,FuelTicket);

			return FuelTicket.Index;
		}

		[AutoComplete]
		public void Modify(SecurityClass security,FuelTicketClass FuelTicket)
		{
			if(!ContextUtil.IsInTransaction)
			{
				ServiceConfig sc=new ServiceConfig();
				sc.Transaction=TransactionOption.Required;
				ServiceDomain.Enter(sc);
				FuelTicketsClass FuelTickets=new FuelTicketsClass();
				try
				{
					FuelTickets.Modify(security,FuelTicket);
					return;
				}
				finally
				{
					FuelTickets.Dispose();
					ServiceDomain.Leave();
				}
			}

			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(FuelTicket == null)
				throw new ArgumentNullException("FuelTicket"); 

			Validate(FuelTicket);

			if(!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			int Index=GetIndex(security,FuelTicket.SequenceNumber);
			if(Index != 0
			&& Index != FuelTicket.Index)
				throw(new Exception("FuelTicket Exists"));


			FuelTicket.UpdatedDate=DateTime.UtcNow;
			FuelTicket.UpdatedBy=security.UserID;
			ConsolidatedDA.ExecuteQuery(security, FuelTicket.UpdateSQL);

			UpdateLineItems(security,FuelTicket);
		}

		[AutoComplete]
		public FuelTicketClass Get(SecurityClass security,int Index)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketClass	FuelTicket=new FuelTicketClass();
			FuelTicket.Index=Index;
			FuelTicket.Load(ConsolidatedDA.GetDataSet(FuelTicket.SelectSQL,security));

			FuelTicketLineItemsClass FuelTicketLineItems=new FuelTicketLineItemsClass();
			FuelTicket.FuelTicketLineItemCollection=FuelTicketLineItems.EnumerateByTicketIndex(security,Index);
			FuelTicketLineItems.Dispose();

			return FuelTicket;
		}

		[AutoComplete]
		public int GetIndex(SecurityClass security,int SequenceNumber)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketClass	FuelTicket=new FuelTicketClass();
			FuelTicket.SiteIndex=security.SiteIndex;
			FuelTicket.UserIndex=security.UserIndex;
			FuelTicket.SequenceNumber=SequenceNumber;
			FuelTicket.Load(ConsolidatedDA.GetDataSet(FuelTicket.SelectByUserAndSequenceNumberSQL,security));

			return FuelTicket.Index;
		}

		[AutoComplete]
		public int GetNextSequenceNumber(	SecurityClass	Security)
		{
			if(Security == null)
				throw new ArgumentNullException("Security"); 

			FuelTicketClass FuelTicket=new FuelTicketClass();
			FuelTicket.SiteIndex=Security.SiteIndex;
			FuelTicket.UserIndex=Security.UserIndex;
			DataSet	Set=ConsolidatedDA.GetDataSet(FuelTicket.SequenceNumberSQL,Security);
			DataTable Table=Set.Tables[0];
			int SequenceNumber=0;
			if(Table.Rows.Count != 0)
				SequenceNumber=(int) Table.Rows[0][0];
			SequenceNumber++;
			return SequenceNumber;	
		}


		[AutoComplete]
		public void Purge(SecurityClass security,int Index)
		{
			if(!ContextUtil.IsInTransaction)
			{
				ServiceConfig sc=new ServiceConfig();
				sc.Transaction=TransactionOption.Required;
				ServiceDomain.Enter(sc);
				FuelTicketsClass FuelTickets=new FuelTicketsClass();
				try
				{
					FuelTickets.Purge(security,Index);
					return;
				}
				finally
				{
					FuelTickets.Dispose();
					ServiceDomain.Leave();
				}
			}

			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketClass	FuelTicket=new FuelTicketClass();
			FuelTicket.Index=Index;

			UpdateLineItems(security,FuelTicket);

			ConsolidatedDA.ExecuteQuery(security, FuelTicket.PurgeSQL);
		}

		[AutoComplete]
		public FuelTicketCollectionClass Enumerate(SecurityClass security)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
			&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketClass FuelTicket=new FuelTicketClass();
			FuelTicket.SiteIndex=security.SiteIndex;
			FuelTicket.UserIndex=security.UserIndex;
			DataSet	Set=ConsolidatedDA.GetDataSet(FuelTicket.EnumerateSQL,security);
			FuelTicketCollectionClass	FuelTicketCollection=new FuelTicketCollectionClass();

			DataTable Table=Set.Tables[0];
			while(Table.Rows.Count != 0)
			{
				FuelTicket=new FuelTicketClass();
				FuelTicket.Load(Set);
				FuelTicketCollection.Add(FuelTicket);
				Table.Rows.RemoveAt(0);
			}

			return FuelTicketCollection;
		}			

		[AutoComplete]
		public void Send(SecurityClass security)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
				throw(new Exception("Access Denied"));

			FuelTicketCollectionClass	FuelTicketCollection=Enumerate(security);
			FuelTicketLineItemsClass FuelTicketLineItems=new FuelTicketLineItemsClass();
			AccountingServiceImpl AccountingService=new AccountingServiceImpl();

			TransactionAliasesClass TransactionAliases=new TransactionAliasesClass();
			int IssueIndex=TransactionAliases.GetIndex(security,"Issue");
			int DefuelIndex=TransactionAliases.GetIndex(security,"Defuel");

			try
			{

				foreach(FuelTicketClass FuelTicket in FuelTicketCollection)
				{
					SaveTransactionsSR ServiceRequest=new SaveTransactionsSR();
					ServiceRequest.Security=security;

					TransactionDO Transaction=new TransactionDO();
					Transaction.Alias										= FuelTicket.Type;
					
					short sign = 1;
					if(FuelTicket.Type == "Issue")
					{
						Transaction.AliasIndex							= new VInteger(IssueIndex);
						Transaction.TransTypeID							= ConsolidatedDataObjects.TransactionTypes.T5_PrimaryDisbursement;
						sign = -1;
					}
					else if(FuelTicket.Type == "Defuel")
					{
						Transaction.AliasIndex							= new VInteger(DefuelIndex);
						Transaction.TransTypeID							= ConsolidatedDataObjects.TransactionTypes.T3_PrimaryDefuel;
						sign = 1;
					}
					Transaction.TransactionDateTime					= new VDateTime(FuelTicket.Date);
					Transaction.InventoryDate							= FuelTicket.Date;
					Transaction.Site										= security.SiteID;
					Transaction.SiteIndex								= new VInteger(security.SiteIndex);
					Transaction.ManagerID								= FuelTicket.ManagerID;
					Transaction.ManagerIndex							= (FuelTicket.ManagerIndex == 0) ? null : new VInteger(FuelTicket.ManagerIndex);
					Transaction.OwnerID									= FuelTicket.OwnerID;
					Transaction.OwnerIndex								= (FuelTicket.OwnerIndex == 0) ? null : new VInteger(FuelTicket.OwnerIndex);
					Transaction.CarrierID								= FuelTicket.VendorID;
					Transaction.CarrierIndex							= (FuelTicket.VendorIndex == 0) ? null : new VInteger(FuelTicket.VendorIndex);
					Transaction.ShipToID									= FuelTicket.ShipToID;
					Transaction.ShipToIndex								= (FuelTicket.ShipToIndex == 0) ? null : new VInteger(FuelTicket.ShipToIndex);
					Transaction.CloseoutDateTime						= null;
					Transaction.RouteInfo.RoutingID					= FuelTicket.FlightNumber;
					Transaction.RouteInfo.NextStation				= FuelTicket.Destination;
					Transaction.PaymentInfo.CreditCardNumber		= FuelTicket.CreditCardNumber;
					Transaction.PaymentInfo.CreditCardExpiration	= new VDateTime(FuelTicket.CreditCardExpiration);

					WeightReadingDO WeightReadings=new WeightReadingDO();
					WeightReadings.BeginQuantity						= new VDouble(FuelTicket.ArrivalGaugeQuantity);
					WeightReadings.RequestedQuantity					= new VDouble(FuelTicket.RequiredGaugeQuantity);
					WeightReadings.FinalQuantity						= new VDouble(FuelTicket.FinalGaugeQuantity);

					Transaction.WeightReadings.Add(WeightReadings);

					FuelTicket.FuelTicketLineItemCollection=FuelTicketLineItems.EnumerateByTicketIndex(security,FuelTicket.Index);

					foreach(FuelTicketLineItemClass FuelTicketLineItem in FuelTicket.FuelTicketLineItemCollection)
					{
						LineItemDO LineItem=new LineItemDO();

						LineItem.DocumentNumber							= FuelTicketLineItem.DocumentNumber;
						LineItem.Pit										= FuelTicketLineItem.PitID;
						LineItem.SourceEQ.RegistrationID				= FuelTicketLineItem.EquipmentID;
						LineItem.SourceEQ.EquipmentIndex				= (FuelTicketLineItem.EquipmentIndex == 0) ? null : new VInteger(FuelTicketLineItem.EquipmentIndex);
						LineItem.OperatorID								= FuelTicketLineItem.PersonID;
						LineItem.OperatorIndex							= (FuelTicketLineItem.PersonIndex == 0) ? null : new VInteger(FuelTicketLineItem.PersonIndex);
						LineItem.Product									= FuelTicketLineItem.ProductID;
						LineItem.ProductIndex							= (FuelTicketLineItem.ProductIndex == 0) ? null : new VInteger(FuelTicketLineItem.ProductIndex);
						LineItem.MeterReading.MeterStart				= new VDouble(FuelTicketLineItem.MeterStart);
						LineItem.MeterReading.MeterStop				= new VDouble(FuelTicketLineItem.MeterEnd);
						LineItem.Volume.Gross							= (sign * FuelTicketLineItem.GrossQuantity);
						LineItem.Volume.Net								= (sign * FuelTicketLineItem.GrossQuantity);
						LineItem.MeterReading.StartDateTime			= new VDateTime(FuelTicketLineItem.StartTime);
						LineItem.MeterReading.StopDateTime			= new VDateTime(FuelTicketLineItem.StopTime);
						LineItem.DestinationEQ.RegistrationID		= FuelTicket.TailNumber;
						LineItem.DestinationEQ.EquipmentType		= FuelTicket.AircraftType;
						LineItem.LoadingLocationID						= FuelTicket.Gate;
						LineItem.LoadingLocationIndex					= (FuelTicket.GateIndex == 0) ? null : new VInteger(FuelTicket.GateIndex);
						if(FuelTicket.FTZ)
							LineItem.Customs								= "FTZ";

						Transaction.LineItems.Add(LineItem);
					}

					ServiceRequest.Transactions.Add(Transaction);

					AccountingService.processRequest(ServiceRequest);

					Purge(security,FuelTicket.Index);
				}
			}
			catch (SaveTransactionsException e)
			{
				FuelTicketLineItems.Dispose();
				if(e.Results.Count >= 1
				&& typeof(TransactionValidationResult).IsInstanceOfType(e.Results[0])
				&& ((TransactionValidationResult) e.Results[0]).ErrorList.Count >= 1)
					throw new Exception(((TransactionValidationResult) e.Results[0]).ErrorList[0]);

				else
					throw new Exception("Unknown SaveTransactionException");
			}
			FuelTicketLineItems.Dispose();
		}			



		[AutoComplete]
		void IDependency.Insert(SecurityClass security,ConsolidatedDataObjectClass Object)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(Object == null)
				throw new ArgumentNullException("Object"); 

		}

		[AutoComplete]
		void IDependency.Update(SecurityClass security,ConsolidatedDataObjectClass Object)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(Object == null)
				throw new ArgumentNullException("Object"); 

		}

		[AutoComplete]
		void IDependency.Purge(SecurityClass security,ConsolidatedDataObjectClass Object)
		{
			if(security == null)
				throw new ArgumentNullException("Security"); 

			if(Object == null)
				throw new ArgumentNullException("Object"); 

			// Purge Tickets
			if(typeof(ConsolidatedDataObjects.UserClass).IsInstanceOfType(Object))
			{
				UserClass	User=(UserClass) Object;
				FuelTicketCollectionClass	FuelTicketCollection=Enumerate(security);
				foreach(FuelTicketClass FuelTicket in FuelTicketCollection)
					Purge(security,FuelTicket.Index);
			}
		}			
	}
}
