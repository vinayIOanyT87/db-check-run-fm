/// <summary>
/// File name:	JournalDO.cs
/// Purpose:	To contain the data journal report data.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>
/// 
using System;

namespace ReportingServices
{
	public class JournalDO : DataObjectBase
	{
		#region Attributes
		private const int EMPTY_STRING = 0;
		//private string   tempJouralTable;
		private string   sessionID;
		private string   inventoryDate;
		private double   bookInventory;
		private double   beginInventory;
		private double   adjustment;
		private double   twentyFourHour;
		private double   rotation;
		private double   defuel;
		private double   issue;
		private double   bulkIssue;
		private double   transfer;
		private double   physicalInventory;
		private double   commercialRequest;
		private double   commercialReceipt;
		private double   loadRack;
		private double   loadRackReceipt;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the journal data object.
		/// </summary>
		public JournalDO()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the session ID.
		/// </summary>
		public string SessionID
		{
			get { return this.sessionID; }
			set { this.sessionID = value; }
		}

		/// <summary>
		/// This property will get and set the inventory transaction value.
		/// </summary>
		public string InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}

		/// <summary>
		/// This property will get and set the beginning inventory transaction value.
		/// </summary>
		public double BookInventory
		{
			get { return this.bookInventory; }
			set { this.bookInventory = value; }
		}

		/// <summary>
		/// This property will get and set the beginning inventory transaction value.
		/// </summary>
		public double BeginInventory
		{
			get { return this.beginInventory; }
			set { this.beginInventory = value; }
		}

		/// <summary>
		/// This property will get and set the 24 hour transaction value.
		/// </summary>
		public double TwentyFourHour
		{
			get { return this.twentyFourHour; }
			set { this.twentyFourHour = value; }
		}

		/// <summary>
		/// This property will get and set the rotation transaction value.
		/// </summary>
		public double Rotation
		{
			get { return this.rotation; }
			set { this.rotation = value; }
		}

		/// <summary>
		/// This property will get and set the defuel transaction value.
		/// </summary>
		public double Defuel
		{
			get { return this.defuel; }
			set { this.defuel = value; }
		}

		/// <summary>
		/// This property will get and set the issue transaction value.
		/// </summary>
		public double Issue
		{
			get { return this.issue; }
			set { this.issue = value; }
		}

		/// <summary>
		/// This property will get and set the bulk issue transaction value.
		/// </summary>
		public double BulkIssue
		{
			get { return this.bulkIssue; }
			set { this.bulkIssue = value; }
		}

		/// <summary>
		/// This property will get and set the tranfer transaction value.
		/// </summary>
		public double Transfer
		{
			get { return this.transfer; }
			set { this.transfer = value; }
		}

		/// <summary>
		/// This property will get and set the physical inventory transaction value.
		/// </summary>
		public double PhysicalInventory
		{
			get { return this.physicalInventory; }
			set { this.physicalInventory = value;}
		}

		/// <summary>
		/// This property will get and set the commercial receipt transaction value.
		/// </summary>
		public double CommercialReceipt
		{
			get { return this.commercialReceipt; }
			set { this.commercialReceipt = value; }
		}

		/// <summary>
		/// This property will get and set the commercial request transaction value.
		/// </summary>
		public double CommercialRequest
		{
			get { return this.commercialRequest; }
			set { this.commercialRequest = value; }
		}

		/// <summary>
		/// This property will get and set the adjustment transaction value.
		/// </summary>
		public double Adjustment
		{
			get { return this.adjustment; }
			set { this.adjustment = value; }
		}

		/// <summary>
		/// This property will get and set the load rack transaction value.
		/// </summary>
		public double LoadRack
		{
			get { return this.loadRack; }
			set { this.loadRack = value; }
		}

		/// <summary>
		/// This property will get and set the load rack receipt transaction value.
		/// </summary>
		public double LoadRackReceipt
		{
			get { return this.loadRackReceipt; }
			set { this.loadRackReceipt = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the journal DO to its initial state.
		/// </summary>
		private void Initialize()
		{
			//this.tempJouralTable   = "tblTempJournalReport";
			this.sessionID         = "";
			this.bookInventory     = 0;
			this.beginInventory    = 0;
			this.adjustment        = 0;
			this.twentyFourHour    = 0;
			this.rotation          = 0;
			this.defuel            = 0;
			this.issue             = 0;
			this.bulkIssue         = 0;
			this.transfer          = 0;
			this.physicalInventory = 0;
			this.commercialRequest = 0;
			this.commercialReceipt = 0;
			this.loadRack          = 0;
			this.loadRackReceipt   = 0;
		}
		#endregion
	}
}
