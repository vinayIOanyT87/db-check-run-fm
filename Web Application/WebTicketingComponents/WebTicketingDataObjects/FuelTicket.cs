/******************************************************************************

	FILE NAME:		FuelTicket.cs


	PURPOSE:			FuelTicketClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using System;
using System.Collections;
using System.Data;
using ConsolidatedDataObjects;
using FMCommon;

namespace WebTicketingDataObjects
{
	/// <summary>
	/// Summary description for FuelTicketCollectionClass.
	/// </summary>
	[Serializable()]
	public class FuelTicketCollectionClass : CollectionBase
	{

		public void Add(FuelTicketClass FuelTicket)
		{
			List.Add(FuelTicket);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				List.RemoveAt(index); 
			}
		}

		public void Remove(FuelTicketClass FuelTicket)
		{
			int index=0;
			foreach(FuelTicketClass Item in List)
			{
				if(Item.Index == FuelTicket.Index)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public FuelTicketClass Item(int Index)
		{
			return (FuelTicketClass) List[Index];
		}
	}


	/// <summary>
	/// Summary description for FuelTicketClass.
	/// </summary>
	[Serializable()]
	public class FuelTicketClass : ConsolidatedDataObjectClass
	{
		protected int												_UserIndex;
		protected DateTime										_Date;
		protected string											_Type;
		protected string											_ManagerID;
		protected int												_ManagerIndex;
		protected string											_OwnerID;
		protected int												_OwnerIndex;
		protected string											_VendorID;
		protected int												_VendorIndex;
		protected string											_ShipToID;
		protected int												_ShipToIndex;
		protected string											_FlightNumber;
		protected string											_TailNumber;
		protected int												_TailNumberIndex;
		protected string											_AircraftType;
		protected string											_Destination;
		protected int												_DestinationIndex;
		protected bool												_FTZ;
		protected string											_Gate;
		protected int												_GateIndex;
		protected string											_CreditCardNumber;
		protected DateTime										_CreditCardExpiration;
		protected double											_ArrivalGaugeQuantity;
		protected double											_RequiredGaugeQuantity;
		protected double											_FinalGaugeQuantity;
		protected int												_SequenceNumber;
		public FuelTicketLineItemCollectionClass		FuelTicketLineItemCollection;


		public int UserIndex{get{return _UserIndex;}set{_UserIndex=value;}}
		public DateTime Date{get{return _Date;}set{_Date=value;}}
		public string Type{get{return _Type;}set{SetString("Type",30,value,ref _Type);}}
		public string ManagerID{get{return _ManagerID;}set{SetString("ManagerID",30,value,ref _ManagerID);}}
		public int ManagerIndex{get{return _ManagerIndex;}set{_ManagerIndex=value;}}
		public string OwnerID{get{return _OwnerID;}set{SetString("OwnerID",30,value,ref _OwnerID);}}
		public int OwnerIndex{get{return _OwnerIndex;}set{_OwnerIndex=value;}}
		public string VendorID{get{return _VendorID;}set{SetString("VendorID",30,value,ref _VendorID);}}
		public int VendorIndex{get{return _VendorIndex;}set{_VendorIndex=value;}}
		public string ShipToID{get{return _ShipToID;}set{SetString("ShipToID",30,value,ref _ShipToID);}}
		public int ShipToIndex{get{return _ShipToIndex;}set{_ShipToIndex=value;}}
		public string FlightNumber{get{return _FlightNumber;}set{SetString("FlightNumber",20,value,ref _FlightNumber);}}
		public string TailNumber{get{return _TailNumber;}set{SetString("TailNumber",30,value,ref _TailNumber);}}
		public int TailNumberIndex{get{return _TailNumberIndex;}set{_TailNumberIndex=value;}}
		public string AircraftType{get{return _AircraftType;}set{SetString("AircraftType",20,value,ref _AircraftType);}}
		public string Destination{get{return _Destination;}set{SetString("Destination",10,value,ref _Destination);}}
		public int DestinationIndex{get{return _DestinationIndex;}set{_DestinationIndex=value;}}
		public bool FTZ{get{return _FTZ;}set{_FTZ=value;}}
		public string Gate{get{return _Gate;}set{SetString("Gate",10,value,ref _Gate);}}
		public int GateIndex{get{return _GateIndex;}set{_GateIndex=value;}}
		public string CreditCardNumber{get{return _CreditCardNumber;}set{SetString("CreditCardNumber",50,value,ref _CreditCardNumber);}}
		public DateTime CreditCardExpiration{get{return _CreditCardExpiration;}set{_CreditCardExpiration=value;}}
		public double ArrivalGaugeQuantity{get{return _ArrivalGaugeQuantity;}set{_ArrivalGaugeQuantity=value;}}
		public double RequiredGaugeQuantity{get{return _RequiredGaugeQuantity;}set{_RequiredGaugeQuantity=value;}}
		public double FinalGaugeQuantity{get{return _FinalGaugeQuantity;}set{_FinalGaugeQuantity=value;}}
		public int SequenceNumber{get{return _SequenceNumber;}set{_SequenceNumber=value;}}


		public override PropertyMap [] Properties
		{
			get
			{
				PropertyMap [] Properties={};
				return Properties;
			}
		} 

		public override string EntityTypeID
		{
			get
			{
				return "Fuel Tickets";
			}

			set
			{
			}
		}

		
		public override string ParentEntityTypeID
		{
			get
			{
				return "";
			}

			set {}
		}

		public FuelTicketClass()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			_UserIndex=0;
			_Date=DateTime.UtcNow;
			_Type="";
			_ManagerID="";
			_ManagerIndex=0;
			_OwnerID="";
			_OwnerIndex=0;
			_VendorID="";
			_VendorIndex=0;
			_ShipToID="";
			_ShipToIndex=0;
			_FlightNumber="";
			_TailNumber="";
			_TailNumberIndex=0;
			_AircraftType="";
			_Destination="";
			_DestinationIndex=0;
			_FTZ=false;
			_Gate="";
			_GateIndex=0;
			_CreditCardNumber="";
			_CreditCardExpiration=DateTime.Now;
			_ArrivalGaugeQuantity=0.0;
			_RequiredGaugeQuantity=0.0;
			_FinalGaugeQuantity=0.0;
			_SequenceNumber=1;

			FuelTicketLineItemCollection=new FuelTicketLineItemCollectionClass();
		}

		public void Load(DataSet Set)
		{
			if(Set == null)
				throw new ArgumentNullException("Set"); 

			Reset();

			DataTable Table=Set.Tables[0];
			if(Table.Rows.Count == 0)
			{
				SiteIndex=0;
				return;
			}

			DataRow	Row=Table.Rows[0];

			_Index=(int) Row[0];
			_SiteIndex=(int) Row[1];
			_UserIndex=(int) Row[2];
			_Date=(DateTime) Row[3];
			_Type=(string) Row[4];
			_ManagerID=(string) Row[5];
			_ManagerIndex=(int) Row[6];
			_OwnerID=(string) Row[7];
			_OwnerIndex=(int) Row[8];
			_VendorID=(string) Row[9];
			_VendorIndex=(int) Row[10];
			_ShipToID=(string) Row[11];
			_ShipToIndex=(int) Row[12];
			_FlightNumber=(string) Row[13];
			_TailNumber=(string) Row[14];
			_TailNumberIndex=(int) Row[15];
			_AircraftType=(string) Row[16];
			_Destination=(string) Row[17];
			_DestinationIndex=(int) Row[18];
			_FTZ=(bool) Row[19];
			_Gate=(string) Row[20];
			_GateIndex=(int) Row[21];
			_CreditCardNumber=(string) Row[22];
			_CreditCardExpiration=(DateTime) Row[23];
			_ArrivalGaugeQuantity=(double) Row[24];
			_RequiredGaugeQuantity=(double) Row[25];
			_FinalGaugeQuantity=(double) Row[26];
			_SequenceNumber=(int) Row[27];
			CreatedDate=(DateTime) Row[28];
			CreatedBy=(string) Row[29];
			UpdatedDate=(DateTime) Row[30];
			UpdatedBy=(string) Row[31];
		}

		public string InsertSQL_
		{
			get
			{
				string SQL;

				SQL=	"INSERT INTO tblFuelTickets "+
					"(SiteIndex,"+
					"UserIndex,"+
					"[Date],"+
					"Type,"+
					"ManagerID,"+
					"ManagerIndex,"+
					"OwnerID,"+
					"OwnerIndex,"+
					"VendorID,"+
					"VendorIndex,"+
					"ShipToID,"+
					"ShipToIndex,"+
					"FlightNumber,"+
					"TailNumber,"+
					"TailNumberIndex,"+
					"AircraftType,"+
					"Destination,"+
					"DestinationIndex,"+
					"FTZ,"+
					"Gate,"+
					"GateIndex,"+
					"CreditCardNumber,"+
					"CreditCardExpiration,"+
					"ArrivalGaugeQuantity,"+
					"RequiredGaugeQuantity,"+
					"FinalGaugeQuantity,"+
					"SequenceNumber,"+
					"CreatedDate,"+
					"CreatedBy,"+
					"UpdatedDate,"+
					"UpdatedBy"+
					") VALUES ("+
					_SiteIndex.ToString()+","+
					_UserIndex.ToString()+","+
					_Date.ToString("\\{\\d\\ \\'yyyy\\-MM\\-dd\\'\\}")+","+
					"N'"+_Type+"',"+
					"N'"+_ManagerID+"',"+
					_ManagerIndex.ToString()+","+
					"N'"+_OwnerID+"',"+
					_OwnerIndex.ToString()+","+
					"N'"+_VendorID+"',"+
					_VendorIndex.ToString()+","+
					"N'"+_ShipToID+"',"+
					_ShipToIndex.ToString()+","+
					"N'"+_FlightNumber+"',"+
					"N'"+_TailNumber+"',"+
					_TailNumberIndex.ToString()+","+
					"N'"+_AircraftType+"',"+
					"N'"+_Destination+"',"+
					_DestinationIndex.ToString()+","+
					(_FTZ ? "1" : "0")+","+
					"N'"+_Gate+"',"+
					_GateIndex.ToString()+","+
					"N'"+_CreditCardNumber+"',"+
					_CreditCardExpiration.ToString("\\{\\d\\ \\'yyyy\\-MM\\-dd\\'\\}")+","+
					_ArrivalGaugeQuantity.ToString()+","+
					_RequiredGaugeQuantity.ToString()+","+
					_FinalGaugeQuantity.ToString()+","+
					_SequenceNumber.ToString()+","+
					_CreatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
					"N'"+_CreatedBy+"',"+
					_UpdatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
					"N'"+_UpdatedBy+"'"+
					") Select [Index]"+
					" FROM tblFuelTickets"+
					" WHERE UserIndex = "+_UserIndex.ToString()+
					" AND [Date] = "+_Date.ToString("\\{\\d\\ \\'yyyy\\-MM\\-dd\\'\\}")+
					" AND Type = N'"+_Type+"'"+
					" AND FlightNumber = N'"+_FlightNumber+"'"+
					" AND SequenceNumber = "+_SequenceNumber.ToString();

				return SQL;
			}
		}

		public string UpdateSQL
		{
			get
			{
				string SQL;

				SQL=	"UPDATE tblFuelTickets "+
						"SET 	[Date] = "+_Date.ToString("\\{\\d\\ \\'yyyy\\-MM\\-dd\\'\\}")+","+
						"Type = N'"+_Type+"',"+
						"ManagerID = N'"+_ManagerID+"',"+
						"ManagerIndex = "+_ManagerIndex.ToString()+","+
						"OwnerID = N'"+_OwnerID+"',"+
						"OwnerIndex = "+_OwnerIndex.ToString()+","+
						"VendorID = N'"+_VendorID+"',"+
						"VendorIndex = "+_VendorIndex.ToString()+","+
						"ShipToID = N'"+_ShipToID+"',"+
						"ShipToIndex = "+_ShipToIndex.ToString()+","+
						"FlightNumber = N'"+_FlightNumber+"',"+
						"TailNumber = N'"+_TailNumber+"',"+
						"TailNumberIndex = "+_TailNumberIndex.ToString()+","+
						"AircraftType = N'"+_AircraftType+"',"+
						"Destination = N'"+_Destination+"',"+
						"DestinationIndex = "+_DestinationIndex.ToString()+","+
						"FTZ = '"+(_FTZ ? "1" : "0")+"',"+
						"Gate = N'"+_Gate+"',"+
						"GateIndex = "+_GateIndex.ToString()+","+
						"CreditCardNumber = '"+_CreditCardNumber+"',"+
						"CreditCardExpiration = "+_CreditCardExpiration.ToString("\\{\\d\\ \\'yyyy\\-MM\\-dd\\'\\}")+","+
						"ArrivalGaugeQuantity = "+_ArrivalGaugeQuantity.ToString()+","+
						"RequiredGaugeQuantity = "+_RequiredGaugeQuantity.ToString()+","+
						"FinalGaugeQuantity = "+_FinalGaugeQuantity.ToString()+","+
						"UpdatedDate = "+_UpdatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						"UpdatedBy = N'"+_UpdatedBy+"' "+
						"WHERE [Index] = '"+_Index.ToString()+"'";

				return SQL;
			}
		}

		public string PurgeSQL
		{
			get
			{
				string SQL;

				SQL="Delete FROM tblFuelTickets WHERE [Index] = '"+_Index.ToString()+"'";

				return SQL;
			}
		}

		public string SelectSQL
		{
			get
			{
				string SQL;

				SQL=	"SELECT tblFuelTickets.* FROM tblFuelTickets WHERE [Index] = '"+_Index.ToString()+"'";

				return SQL;
			}
		}

		public string SelectByUserAndSequenceNumberSQL
		{
			get
			{
				string SQL=	"Select tblFuelTickets.* "+
								" FROM tblFuelTickets"+
								" WHERE UserIndex = "+_UserIndex.ToString()+
								" AND SequenceNumber = "+_SequenceNumber.ToString()+
								" AND SiteIndex = '"+_SiteIndex.ToString()+"'";

				return SQL;
			}
		}

		public string EnumerateSQL
		{
			get
			{
				string SQL;

				SQL=	"SELECT tblFuelTickets.* FROM tblFuelTickets"+
					" WHERE UserIndex = '"+_UserIndex.ToString()+"'"+
					" AND SiteIndex = '"+_SiteIndex.ToString()+"'"+
					" ORDER BY SequenceNumber";

				return SQL;
			}
		}

		public string SequenceNumberSQL
		{
			get
			{
				string SQL;

				SQL="SELECT TOP 1 SequenceNumber FROM tblFuelTickets"+
					" WHERE UserIndex = "+_UserIndex.ToString()+
					" AND SiteIndex = "+_SiteIndex.ToString()+
					" ORDER BY SequenceNumber DESC";

				return SQL;
			}		
		}

	}
}
