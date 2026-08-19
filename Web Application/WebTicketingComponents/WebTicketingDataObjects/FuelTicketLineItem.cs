/******************************************************************************

	FILE NAME:		FuelTicketLineItem.cs


	PURPOSE:			FuelTicketLineItemClass


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
	/// Summary description for FuelTicketLineItemCollectionClass.
	/// </summary>
	[Serializable()]
	public class FuelTicketLineItemCollectionClass : CollectionBase
	{

		public void Add(FuelTicketLineItemClass FuelTicketLineItem)
		{
			List.Add(FuelTicketLineItem);
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

		public FuelTicketLineItemClass Item(int Index)
		{
			return (FuelTicketLineItemClass) List[Index];
		}
	}


	/// <summary>
	/// Summary description for FuelTicketLineItemClass.
	/// </summary>
	[Serializable()]
	public class FuelTicketLineItemClass : ConsolidatedDataObjectClass
	{
		protected int											_FuelTicketIndex;
		protected byte											_ItemNumber;
		protected string										_DocumentNumber;
		protected string										_PitID;
		protected string										_EquipmentID;
		protected int											_EquipmentIndex;
		protected string										_PersonID;
		protected int											_PersonIndex;
		protected string										_ProductID;
		protected int											_ProductIndex;
		protected double										_MeterStart;
		protected double										_MeterEnd;
		protected double										_GrossQuantity;
		protected DateTime									_StartTime;
		protected DateTime									_StopTime;

		public int FuelTicketIndex{get{return _FuelTicketIndex;}set{_FuelTicketIndex=value;}}
		public byte ItemNumber{get{return _ItemNumber;}set{_ItemNumber=value;}}
		public string DocumentNumber{get{return _DocumentNumber;}set{SetString("DocumentNumber",50,value,ref _DocumentNumber);}}
		public string PitID{get{return _PitID;}set{SetString("PitID",30,value,ref _PitID);}}
		public string EquipmentID{get{return _EquipmentID;}set{SetString("EquipmentID",30,value,ref _EquipmentID);}}
		public int EquipmentIndex{get{return _EquipmentIndex;}set{_EquipmentIndex=value;}}
		public string PersonID{get{return _PersonID;}set{SetString("PersonID",50,value,ref _PersonID);}}
		public int PersonIndex{get{return _PersonIndex;}set{_PersonIndex=value;}}
		public string ProductID{get{return _ProductID;}set{SetString("ProductID",30,value,ref _ProductID);}}
		public int ProductIndex{get{return _ProductIndex;}set{_ProductIndex=value;}}
		public double MeterStart{get{return _MeterStart;}set{_MeterStart=value;}}
		public double MeterEnd{get{return _MeterEnd;}set{_MeterEnd=value;}}
		public double GrossQuantity{get{return _GrossQuantity;}set{_GrossQuantity=value;}}
		public DateTime StartTime{get{return _StartTime;}set{_StartTime=value;}}
		public DateTime StopTime{get{return _StopTime;}set{_StopTime=value;}}


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
				return "Fuel Ticket Line Items";
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

		public FuelTicketLineItemClass()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			_FuelTicketIndex=0;
			_ItemNumber=0;
			_DocumentNumber="";
			_PitID="";
			_EquipmentID="";
			_EquipmentIndex=0;
			_PersonID="";
			_PersonIndex=0;
			_ProductID="";
			_ProductIndex=0;
			_MeterStart=0;
			_MeterEnd=0;
			_GrossQuantity=0;
			_StartTime=DateTime.UtcNow;
			_StopTime=DateTime.UtcNow;
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
			_FuelTicketIndex=(int) Row[1];
			_ItemNumber=(byte) Row[2];
			_DocumentNumber=(string) Row[3];
			_PitID=(string) Row[4];
			_EquipmentID=(string) Row[5];
			_EquipmentIndex=(int) Row[6];
			_PersonID=(string) Row[7];
			_PersonIndex=(int) Row[8];
			_ProductID=(string) Row[9];
			_ProductIndex=(int) Row[10];
			_MeterStart=(double) Row[11];
			_MeterEnd=(double) Row[12];
			_GrossQuantity=(double) Row[13];
			_StartTime=(DateTime) Row[14];
			_StopTime=(DateTime) Row[15];
			_CreatedDate=(DateTime) Row[16];
			_CreatedBy=(string) Row[17];
			_UpdatedDate=(DateTime) Row[18];
			_UpdatedBy=(string) Row[19];
		}

		public string InsertSQL_
		{
			get
			{
				string SQL;

				SQL=	"INSERT INTO tblFuelTicketLineItems "+
						"(FuelTicketIndex,"+
						"ItemNumber,"+
						"DocumentNumber,"+
						"PitID,"+
						"EquipmentID,"+
						"EquipmentIndex,"+
						"PersonID,"+
						"PersonIndex,"+
						"ProductID,"+
						"ProductIndex,"+
						"MeterStart,"+
						"MeterEnd,"+
						"GrossQuantity,"+
						"StartDateTime,"+
						"StopDateTime,"+
						"CreatedDate,"+
						"CreatedBy,"+
						"UpdatedDate,"+
						"UpdatedBy"+
						") VALUES ("+
						_FuelTicketIndex.ToString()+","+
						_ItemNumber.ToString()+","+
						"N'"+_DocumentNumber+"',"+
						"N'"+_PitID+"',"+
						"N'"+_EquipmentID+"',"+
						_EquipmentIndex.ToString()+","+
						"N'"+_PersonID+"',"+
						_PersonIndex.ToString()+","+
						"N'"+_ProductID+"',"+
						_ProductIndex.ToString()+","+
						_MeterStart.ToString()+","+
						_MeterEnd.ToString()+","+
						_GrossQuantity.ToString()+","+
						_StartTime.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						_StopTime.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						_CreatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						"N'"+_CreatedBy+"',"+
						_UpdatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						"N'"+_UpdatedBy+"'"+
						") Select [Index]"+
						" FROM tblFuelTicketLineItems"+
						" WHERE FuelTicketIndex = '"+_FuelTicketIndex.ToString()+"'"+
						" AND ItemNumber = '"+_ItemNumber.ToString()+"'";

				return SQL;
			}
		}

		public string UpdateSQL
		{
			get
			{
				string SQL;

				SQL=	"UPDATE tblFuelTicketLineItems "+
						"SET ItemNumber = "+_ItemNumber.ToString()+","+
						"DocumentNumber = N'"+_DocumentNumber+"',"+
						"PitID = N'"+_PitID+"',"+
						"EquipmentID = N'"+_EquipmentID+"',"+
						"EquipmentIndex = "+_EquipmentIndex.ToString()+","+
						"PersonID = N'"+_PersonID+"',"+
						"PersonIndex = "+_PersonIndex.ToString()+","+
						"ProductID = N'"+_ProductID+"',"+
						"ProductIndex = "+_ProductIndex.ToString()+","+
						"MeterStart = "+_MeterStart+","+
						"MeterEnd = "+_MeterEnd+","+
						"GrossQuantity = "+_GrossQuantity.ToString()+","+
						"StartDateTime = "+_StartTime.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						"StopDateTime = "+_StopTime.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						"UpdatedDate = "+_UpdatedDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
						"UpdatedBy = N'"+_UpdatedBy+"'"+
						"WHERE [Index] = '"+_Index.ToString()+"'";

				return SQL;
			}
		}

		public string PurgeSQL
		{
			get
			{
				string SQL;

				SQL="Delete FROM tblFuelTicketLineItems WHERE [Index] = '"+_Index.ToString()+"'";

				return SQL;
			}
		}

		public string SelectSQL
		{
			get
			{
				string SQL;

				SQL=	"SELECT tblFuelTicketLineItems.* FROM tblFuelTicketLineItems WHERE [Index] = '"+_Index.ToString()+"'";

				return SQL;
			}
		}

		public string SelectByTicketIndexAndItemNumberSQL
		{
			get
			{
				string SQL;

				SQL=	"Select tblFuelTicketLineItems.*"+
						" FROM tblFuelTicketLineItems"+
						" WHERE FuelTicketIndex = '"+_FuelTicketIndex.ToString()+"'"+
						" AND ItemNumber = '"+_ItemNumber.ToString()+"'";

				return SQL;
			}
		}

		public string EnumerateByTicketIndexSQL
		{
			get
			{
				string SQL;

				SQL=	"SELECT tblFuelTicketLineItems.* FROM tblFuelTicketLineItems"+
						" WHERE FuelTicketIndex = '"+_FuelTicketIndex.ToString()+"'"+
						" ORDER BY ItemNumber";

				return SQL;
			}
		}
	}
}
