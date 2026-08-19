// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineItemDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LineItemDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Xml.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Varec.CommonComponents.EthanolExcessLibrary;

	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// The line item data object.
	/// </summary>
	[XmlType("LineItem")]
	[XmlInclude(typeof(RegradeLineItemDO)), XmlInclude(typeof(StorageTransferLineItemDO))]
	[DataContract]
	[Serializable]
	[KnownType(typeof(BaseCollections))]
	[KnownType(typeof(ArrayList))]
	[KnownType(typeof(RegradeLineItemDO))]
	[KnownType(typeof(StorageTransferLineItemDO))]
	[KnownType(typeof(AssociatedTxDO))]
	[KnownType(typeof(SubLineItemDO))]
	public class LineItemDO : BaseTransactionLineItemDO
	{
		#region Data members
		[DataMember]
		protected Dictionary<string, string> userData;
		[DataMember]
		private List<AssociatedTxDO> assocInvoiceTx;
		[DataMember]
		private List<AssociatedTxDO> associatedTx;
		[DataMember]
		private bool isNewLineItem;
		#endregion


		/// <summary>
		/// Hash map of database column name to LineItemDO property name.  Primarily used to determine which
		/// fields to clear when a transaction is copied or created from an existing transaction.  Only entries
		/// in which the database column name is different from the LineItemDO property name should be added
		/// to the map.  To specify the property of a nested object, append the property name to the nested
		/// object property name with a dot.  For example the Mass property of the Quantity nested object
		/// is specified as "Quantity.Mass"
		/// </summary>
		private static readonly Dictionary<string, string> DbNameToPropertyMap = new Dictionary<string, string>
				{
					{ "SequenceId", "sequenceId" },
					{ "LookupTransactionStatusIndex", "Status" },
					{ "LookupQualityIndex", "Quality" },
					{ "GrossQuantity", "Quantity.GrossInventoryChange" },
					{ "DeliveredGrossQuantity", "Quantity.DeliveredGrossInventoryChange" },
					{ "NetQuantity", "Quantity.NetInventoryChange" },
					{ "DeliveredNetQuantity", "Quantity.DeliveredNetInventoryChange" },
					{ "MassQuantity", "Quantity.Mass" },
					{ "NetManualValueFlag", "Quantity.NetManualValueFlag" },
					{ "DeliveredNetManualValueFlag", "Quantity.DeliveredNetManualValueFlag" },
					{ "MassManualValueFlag", "Quantity.MassManualValueFlag" },
					{ "GrossManualValueFlag", "Quantity.GrossManualValueFlag" },
					{ "DeliveredGrossManualValueFlag", "Quantity.DeliveredGrossManualValueFlag" },
					{ "VcfManualValueFlag", "Quantity.VcfManualValueFlag" },
					{ "DestinationEquipmentModel", "DestinationEQ.EquipmentModel" },
					{ "DestinationEquipmentType", "DestinationEQ.EquipmentType" },
					{ "DestinationRegistrationID", "DestinationEQ.RegistrationID" },
					{ "DestinationSerialNumber", "DestinationEQ.SerialNumber" },
					{ "DestinationCompanyEquipmentID", "DestinationEQ.CompanyEquipmentID" },
					{ "SourceEquipmentModel", "SourceEQ.EquipmentModel" },
					{ "SourceEquipmentType", "SourceEQ.EquipmentType" },
					{ "SourceRegistrationID", "SourceEQ.RegistrationID" },
					{ "SourceSerialNumber", "SourceEQ.SerialNumber" },
					{ "SourceCompanyEquipmentID", "SourceEQ.CompanyEquipmentID" },
					{ "MeterFactor", "MeterReading.MeterFactor" },
					{ "MeterStart", "MeterReading.MeterStart" },
					{ "MeterStop", "MeterReading.MeterStop" },
					{ "MeterStartDateTime", "MeterReading.StartDateTime" },
					{ "MeterStopDateTime", "MeterReading.StopDateTime" },
					{ "", "" }
				};

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="LineItemDO"/> class.
		/// </summary>
		public LineItemDO()
		{
			this.userData = new Dictionary<string, string>();
			this.assocInvoiceTx = new List<AssociatedTxDO>();
			this.associatedTx = new List<AssociatedTxDO>();
			this.isNewLineItem = false;
			this.IsEthanolBlend = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LineItemDO"/> class.
		/// This is a copy constructor for the Line Item DO class.
		/// NOTE: It is incomplete!
		/// </summary>
		/// <param name="inLineItemDO">
		/// The line item data object.
		/// </param>
		public LineItemDO(BaseTransactionLineItemDO inLineItemDO)
			: base(inLineItemDO)
		{
			this.userData = new Dictionary<string, string>();
			this.assocInvoiceTx = new List<AssociatedTxDO>();
			this.associatedTx = new List<AssociatedTxDO>();

			if (inLineItemDO.GetType() == typeof(LineItemDO))
			{
				var lineItemDO = inLineItemDO as LineItemDO;

				if (lineItemDO != null)
				{
					this.userData = new Dictionary<string, string>(lineItemDO.UserData);
				}
			}

			this.VcfModuleSettings = new VcfModuleSettings();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether is new line item.
		/// </summary>
		public bool IsNewLineItem
		{
			get { return this.isNewLineItem; }
			set { this.isNewLineItem = value; }
		}

		[DataMember]
		[XmlIgnore]
		public Guid TransactionLineItemUserDataGuid { get; set; }

		[DataMember]
		public bool IsEthanolBlend { get; set; }

		[DataMember]
		public VcfModuleSettings VcfModuleSettings { get; set; }


		/// <summary>
		/// Contains user defined data fields for the line item
		/// </summary>
		[XmlIgnore]
		public Dictionary<string, string> UserData
		{
			get { return this.userData; }
			private set { this.userData = value; }
		}

		[QueryWriterField("User Data 1", "tblTransactionLineItemUserData.UserData1")]
		public string UserData1
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_01))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_01];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_01] = value;
			}
		}

		[QueryWriterField("User Data 2", "tblTransactionLineItemUserData.UserData2")]
		public string UserData2
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_02))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_02];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_02] = value;
			}
		}

		[QueryWriterField("User Data 3", "tblTransactionLineItemUserData.UserData3")]
		public string UserData3
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_03))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_03];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_03] = value;
			}
		}

		[QueryWriterField("User Data 4", "tblTransactionLineItemUserData.UserData4")]
		public string UserData4
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_04))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_04];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_04] = value;
			}
		}

		[QueryWriterField("User Data 5", "tblTransactionLineItemUserData.UserData5")]
		public string UserData5
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_05))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_05];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_05] = value;
			}
		}

		[QueryWriterField("User Data 6", "tblTransactionLineItemUserData.UserData6")]
		public string UserData6
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_06))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_06];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_06] = value;
			}
		}

		[QueryWriterField("User Data 7", "tblTransactionLineItemUserData.UserData7")]
		public string UserData7
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_07))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_07];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_07] = value;
			}
		}

		[QueryWriterField("User Data 8", "tblTransactionLineItemUserData.UserData8")]
		public string UserData8
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_08))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_08];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_08] = value;
			}
		}

		[QueryWriterField("User Data 9", "tblTransactionLineItemUserData.UserData9")]
		public string UserData9
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_09))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_09];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_09] = value;
			}
		}

		[QueryWriterField("User Data 10", "tblTransactionLineItemUserData.UserData10")]
		public string UserData10
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_10))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_10];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_10] = value;
			}
		}

		[QueryWriterField("User Data 11", "tblTransactionLineItemUserData.UserData11")]
		public string UserData11
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_11))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_11];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_11] = value;
			}
		}

		[QueryWriterField("User Data 12", "tblTransactionLineItemUserData.UserData12")]
		public string UserData12
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_12))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_12];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_12] = value;
			}
		}

		[QueryWriterField("User Data 13", "tblTransactionLineItemUserData.UserData13")]
		public string UserData13
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_13))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_13];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_13] = value;
			}
		}

		[QueryWriterField("User Data 14", "tblTransactionLineItemUserData.UserData14")]
		public string UserData14
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_14))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_14];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_14] = value;
			}
		}

		[QueryWriterField("User Data 15", "tblTransactionLineItemUserData.UserData15")]
		public string UserData15
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_15))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_15];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_15] = value;
			}
		}

		[QueryWriterField("User Data 16", "tblTransactionLineItemUserData.UserData16")]
		public string UserData16
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_16))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_16];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_16] = value;
			}
		}

		[QueryWriterField("User Data 17", "tblTransactionLineItemUserData.UserData17")]
		public string UserData17
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_17))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_17];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_17] = value;
			}
		}

		[QueryWriterField("User Data 18", "tblTransactionLineItemUserData.UserData18")]
		public string UserData18
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_18))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_18];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_18] = value;
			}
		}

		[QueryWriterField("User Data 19", "tblTransactionLineItemUserData.UserData19")]
		public string UserData19
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_19))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_19];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_19] = value;
			}
		}

		[QueryWriterField("User Data 20", "tblTransactionLineItemUserData.UserData20")]
		public string UserData20
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_20))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_20];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_20] = value;
			}
		}

		[QueryWriterField("User Data 21", "tblTransactionLineItemUserData.UserData21")]
		public string UserData21
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_21))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_21];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_21] = value;
			}
		}

		[QueryWriterField("User Data 22", "tblTransactionLineItemUserData.UserData22")]
		public string UserData22
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_22))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_22];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_22] = value;
			}
		}

		[QueryWriterField("User Data 23", "tblTransactionLineItemUserData.UserData23")]
		public string UserData23
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_23))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_23];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_23] = value;
			}
		}

		[QueryWriterField("User Data 24", "tblTransactionLineItemUserData.UserData24")]
		public string UserData24
		{
			get
			{
				if (this.userData.ContainsKey(USER_DATA_LINE_ITEM_KEY_24))
				{
					return this.userData[USER_DATA_LINE_ITEM_KEY_24];
				}

				return null;
			}
			set
			{
				this.userData[USER_DATA_LINE_ITEM_KEY_24] = value;
			}
		}


		public List<AssociatedTxDO> AssociatedInvoiceTx
		{
			get { return this.assocInvoiceTx; }
			set { this.assocInvoiceTx = value; }
		}

		/// <summary>
		/// Returns a collection of ID's for transactions associated with this
		/// line item.  This property should replace the other associated
		/// transaction properties in the future.
		/// </summary>
		public List<AssociatedTxDO> AssociatedTransactions
		{
			get { return this.associatedTx; }
			set { this.associatedTx = value; }
		}

		[QueryWriterField("Contract Number", "tblTransactionLineItems.ContractNumber")]
		public string ContractNumber
		{
			get { return base.contractNumber; }
			set { base.contractNumber = value; }
		}

		[QueryWriterField("CLIN", "tblTransactionLineItems.CLIN")]
		public string CLIN
		{
			get { return base.clin; }
			set { base.clin = value; }
		}

		/// <summary>
		/// Gets or sets the arm number.
		/// </summary>
		[QueryWriterField("Arm Number", "tblTransactionLineItems.ArmNumber")]
		public int? ArmNumber
		{
			get { return this.armNumber; }
			set { this.armNumber = value; }
		}

		/// <summary>
		/// This method causes the Arm Number property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeArmNumber()
		{
			return this.armNumber.HasValue;
		}

		/// <summary>
		/// Gets or sets the line number.
		/// </summary>
		public int? LineNumber
		{
			get { return this.lineNumber; }
			set { this.lineNumber = value; }
		}

		/// <summary>
		/// This method causes the Line Number property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeLineNumber()
		{
			return this.lineNumber.HasValue;
		}

		[QueryWriterField("Operator ID", "tblTransactionLineItems.OperatorID")]
		public string OperatorID
		{
			get { return base.operatorID; }
			set { base.operatorID = value; }
		}

		[XmlIgnore]
		public Guid OperatorPersonnelGuid
		{
			get { return base.operatorPersonnelGuid; }
			set { base.operatorPersonnelGuid = value; }
		}

		[QueryWriterField("Batch Number", "tblTransactionLineItems.BatchNumber")]
		public string BatchNumber
		{
			get { return base.batchNumber; }
			set { base.batchNumber = value; }
		}

		/// <summary>
		/// Gets or sets the line fill.
		/// </summary>
		[QueryWriterField("Line Fill", "tblTransactionLineItems.LineFill", false)]
		public double? LineFill
		{
			get { return this.lineFill; }
			set { this.lineFill = value; }
		}

		/// <summary>
		/// This method causes the Line Fill property to not be serialized if it
		/// is null. The reason we have to do this is because the XSD validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeLineFill()
		{
			return this.lineFill.HasValue;
		}

		/// <summary>
		/// Gets or sets the bottom volume.
		/// </summary>
		[QueryWriterField("Bottom Volume", "tblTransactionLineItems.BottomVolume", false)]
		public double? BottomVolume
		{
			get { return this.bottomVolume; }
			set { this.bottomVolume = value; }
		}

		/// <summary>
		/// This method causes the Bottom Volume property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeBottomVolume()
		{
			return this.bottomVolume.HasValue;
		}

		/// <summary>
		/// Gets or sets the net capacity.
		/// </summary>
		[QueryWriterField("Net Capacity", "tblTransactionLineItems.NetCapacity", false)]
		public double? NetCapacity
		{
			get { return this.netCapacity; }
			set { this.netCapacity = value; }
		}

		/// <summary>
		/// This method causes the Net Capacity property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNetCapacity()
		{
			return this.netCapacity.HasValue;
		}

		[QueryWriterField("Tank Status", "tblTransactionLineItems.TankStatus")]
		public string TankStatus
		{
			get { return this.tankStatus; }
			set { this.tankStatus = value; }
		}

		[QueryWriterField("Line Source Equip Reg ID", "tblTransactionLineItems.SourceRegistrationID")]
		public string SourceRegistrationID
		{
			get { return this.SourceEQ.RegistrationID; }
		}

		[QueryWriterField("Pit", "tblTransactionLineItems.Pit")]
		public string Pit
		{
			get { return base.pit; }
			set { base.pit = value; }
		}

		[XmlElement("RequestedDateTimeString")]
		public string RequestedDateTimeString
		{
			get
			{
				return this.requestedDateTime == null ? string.Empty : ((DateTimeOffset)this.requestedDateTime).ToString(TimeFormat);
			}

			set
			{
				this.requestedDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("Requested Date", "tblTransactionLineItems.RequestedDateTime")]
		[XmlIgnore]
		public DateTimeOffset? RequestedDateTime
		{
			get { return base.requestedDateTime; }
			set { base.requestedDateTime = value; }
		}

		[XmlElement("DispatchedDateTimeString")]
		public string DispatchedDateTimeString
		{
			get
			{
				return this.dispatchedDateTime == null ? string.Empty : ((DateTimeOffset)this.dispatchedDateTime).ToString(TimeFormat);
			}

			set
			{
				this.dispatchedDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Dispatched Date", "tblTransactionLineItems.DispatchedDateTime")]
		[XmlIgnore]
		public DateTimeOffset? DispatchedDateTime
		{
			get { return base.dispatchedDateTime; }
			set { base.dispatchedDateTime = value; }
		}

		[XmlElement("AcknowledgedDateTimeString")]
		public string AcknowledgedDateTimeString
		{
			get
			{
				return this.acknowledgedDateTime == null ? string.Empty : ((DateTimeOffset)this.acknowledgedDateTime).ToString(TimeFormat);
			}

			set
			{
				this.acknowledgedDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Acknowledged Date", "tblTransactionLineItems.AcknowledgedDateTime")]
		[XmlIgnore]
		public DateTimeOffset? AcknowledgedDateTime
		{
			get { return base.acknowledgedDateTime; }
			set { base.acknowledgedDateTime = value; }
		}

		[XmlElement("OnLocationTimeString")]
		public string OnLocationTimeString
		{
			get
			{
				return this.onLocationTime == null ? string.Empty : ((DateTimeOffset)this.onLocationTime).ToString(TimeFormat);
			}

			set
			{
				this.onLocationTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("On Location Time", "tblTransactionLineItems.OnLocationTime")]
		[XmlIgnore]
		public DateTimeOffset? OnLocationTime
		{
			get { return base.onLocationTime; }
			set { base.onLocationTime = value; }
		}

		[XmlElement("ValidationDateTimeString")]
		public string ValidationDateTimeString
		{
			get
			{
				return this.validationDateTime == null ? string.Empty : ((DateTimeOffset)this.validationDateTime).ToString(TimeFormat);
			}

			set
			{
				this.validationDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Validation Date", "tblTransactionLineItems.ValidationDateTime")]
		[XmlIgnore]
		public DateTimeOffset? ValidationDateTime
		{
			get { return base.validationDateTime; }
			set { base.validationDateTime = value; }
		}

		[XmlElement("CompletionDateTimeString")]
		public string CompletionDateTimeString
		{
			get
			{
				return this.completionDateTime == null ? string.Empty : ((DateTimeOffset)this.completionDateTime).ToString(TimeFormat);
			}

			set
			{
				this.completionDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Completion Date", "tblTransactionLineItems.CompletionDateTime")]
		[XmlIgnore]
		public DateTimeOffset? CompletionDateTime
		{
			get { return base.completionDateTime; }
			set { base.completionDateTime = value; }
		}

		/// <summary>
		/// Gets or sets the receipt variance.
		/// </summary>
		[QueryWriterField("Receipt Variance", "tblTransactionLineItems.ReceiptVariance", false)]
		public double? ReceiptVariance
		{
			get { return this.receiptVariance; }
			set { this.receiptVariance = value; }
		}

		/// <summary>
		/// This method causes the Receipt Variance property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeReceiptVariance()
		{
			return this.receiptVariance.HasValue;
		}

		/// <summary>
		/// Gets or sets the differential pressure.
		/// </summary>
		[QueryWriterField("Differential Pressure", "tblTransactionLineItems.DifferentialPressure", false)]
		public double? DifferentialPressure
		{
			get { return this.differentialPressure; }
			set { this.differentialPressure = value; }
		}

		/// <summary>
		/// This method causes the Differential Pressure property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeDifferentialPressure()
		{
			return this.differentialPressure.HasValue;
		}

		/// <summary>
		/// Gets or sets the load rack variance.
		/// </summary>
		[QueryWriterField("Load Rack Variance", "tblTransactionLineItems.LoadRackVariance", false)]
		public double? LoadRackVariance
		{
			get { return this.loadRackVariance; }
			set { this.loadRackVariance = value; }
		}

		/// <summary>
		/// This method causes the Load Rack Variance property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeLoadRackVariance()
		{
			return this.loadRackVariance.HasValue;
		}

		/// <summary>
		/// Gets or sets the freeze point.
		/// </summary>
		[QueryWriterField("Freeze Point", "tblTransactionLineItems.FreezePoint", false)]
		public double? FreezePoint
		{
			get { return this.freezePoint; }
			set { this.freezePoint = value; }
		}

		/// <summary>
		/// This method causes the Freeze Point property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeFreezePoint()
		{
			return this.freezePoint.HasValue;
		}

		[QueryWriterField("Line Dest Equip Reg ID", "tblTransactionLineItems.DestinationRegistrationID")]
		public string DestinationRegistrationID
		{
			get { return this.DestinationEQ.RegistrationID; }
		}

		[QueryWriterField("Line Dest Equip Serial Number", "tblTransactionLineItems.DestinationSerialNumber")]
		public string DestinationSerialNumber
		{
			get { return this.DestinationEQ.SerialNumber; }
		}

		[QueryWriterField("Line Dest Equip Type", "tblTransactionLineItems.DestinationEquipmentType")]
		public string DestinationEquipmentType
		{
			get { return this.DestinationEQ.EquipmentType; }
		}

		[QueryWriterField("Line Dest Equip Model", "tblTransactionLineItems.DestinationEquipmentModel")]
		public string DestinationEquipmentModel
		{
			get { return this.DestinationEQ.EquipmentModel; }
		}

		public EquipmentDO DestinationEQ
		{
			get { return base.destinationEQ; }
			set { base.destinationEQ = value; }
		}

		[QueryWriterField("Destination Compartment ID", "tblTransactionLineItems.DestinationCompartmentID")]
		public string DestinationCompartmentID
		{
			get { return base.destinationCompartmentID; }
			set { base.destinationCompartmentID = value; }
		}

		[XmlIgnore]
		public Guid DestinationCompartmentEquipmentGuid
		{
			get { return base.destinationCompartmentEquipmentGuid; }
			set { base.destinationCompartmentEquipmentGuid = value; }
		}

		public EquipmentDO SourceEQ
		{
			get { return base.sourceEQ; }
			set { base.sourceEQ = value; }
		}

		[QueryWriterField("Source Compartment ID", "tblTransactionLineItems.SourceCompartmentID")]
		public string SourceCompartmentID
		{
			get { return base.sourceCompartmentID; }
			set { base.sourceCompartmentID = value; }
		}

		[XmlIgnore]
		public Guid SourceCompartmentEquipmentGuid
		{
			get { return base.sourceCompartmentEquipmentGuid; }
			set { base.sourceCompartmentEquipmentGuid = value; }
		}

		public MeterReadingDO MeterReading
		{
			get { return base.meterReading; }
			set { base.meterReading = value; }
		}

		/// <summary>
		/// Gets the meter factor.
		/// </summary>
		[QueryWriterField("Meter Factor", "tblTransactionLineItems.MeterFactor")]
		public double? MeterFactor
		{
			get { return this.MeterReading.MeterFactor; }
		}

		/// <summary>
		/// This method causes the Meter Factor property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeMeterFactor()
		{
			return this.MeterReading.MeterFactor.HasValue;
		}

		/// <summary>
		/// Gets the meter start.
		/// </summary>
		[QueryWriterField("Meter Start", "tblTransactionLineItems.MeterStart")]
		public double? MeterStart
		{
			get { return this.MeterReading.MeterStart; }
		}

		/// <summary>
		/// This method causes the Meter Start property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeMeterStart()
		{
			return this.MeterReading.MeterStart.HasValue;
		}

		/// <summary>
		/// Gets the meter stop.
		/// </summary>
		[QueryWriterField("Meter Stop", "tblTransactionLineItems.MeterStop")]
		public double? MeterStop
		{
			get { return this.MeterReading.MeterStop; }
		}

		/// <summary>
		/// This method causes the Meter Stop property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeMeterStop()
		{
			return this.MeterReading.MeterStop.HasValue;
		}

		[QueryWriterField("Meter Start Date", "tblTransactionLineItems.MeterStartDateTime")]
		[XmlIgnore]
		public DateTimeOffset? MeterStartDateTime
		{
			get { return this.MeterReading.StartDateTime; }
			set { this.MeterReading.StartDateTime = value; }
		}

		[QueryWriterField("Meter Stop Date", "tblTransactionLineItems.MeterStopDateTime")]
		[XmlIgnore]
		public DateTimeOffset? MeterStopDateTime
		{
			get { return this.MeterReading.StopDateTime; }
			set { this.MeterReading.StopDateTime = value; }
		}

		[QueryWriterField("Additive Profile ID", "tblTransactionLineItems.AdditiveProfileID")]
		public string AdditiveProfileID
		{
			get { return base.additiveProfileID; }
			set { base.additiveProfileID = value; }
		}

		[XmlIgnore]
		public Guid AdditiveProfileGuid
		{
			get { return base.additiveProfileGuid; }
			set { base.additiveProfileGuid = value; }
		}

		[QueryWriterField("Storage Location ID", "tblTransactionLineItems.StorageLocationID")]
		public string StorageLocationID
		{
			get { return base.storageLocationID; }
			set { base.storageLocationID = value; }
		}

		[XmlIgnore]
		public Guid StorageLocationTankGuid
		{
			get { return base.storageLocationTankGuid; }
			set { base.storageLocationTankGuid = value; }
		}

		[QueryWriterField("Meter ID", "tblTransactionLineItems.MeterID")]
		public string MeterID
		{
			get { return base.meterID; }
			set { base.meterID = value; }
		}

		[XmlIgnore]
		public Guid MeterGuid
		{
			get { return base.meterGuid; }
			set { base.meterGuid = value; }
		}

		[QueryWriterField("Line Delete Flag", "tblTransactionLineItems.DeleteFlag")]
		public bool DeleteFlag
		{
			get { return base.deleteFlag; }
			set { base.deleteFlag = value; }
		}

		[XmlArray("TransactionSubLineItems")] //rename the node to "TransctionSubLineItems"
		[XmlArrayItem(Type = typeof(SubLineItemDO))]
		public List<SubLineItemDO> SubLineItems
		{
			get { return base.subLineItems; }
			set { base.subLineItems = value; }
		}

		[XmlIgnore]
		public ProductMapClass SplashBlendingMap
		{
			get { return base._SplashBlendingMap; }
			set { base._SplashBlendingMap = value; }
		}

		[QueryWriterField("Loading Location ID", "tblTransactionLineItems.LoadingLocationID")]
		public string LoadingLocationID
		{
			get { return base.loadingLocationID; }
			set { base.loadingLocationID = value; }
		}

		/// <summary>
		/// Gets or sets the loading location station GUID.
		/// </summary>
		[XmlIgnore]
		public Guid LoadingLocationStationGuid
		{
			get { return this.loadingLocationStationGuid; }
			set { this.loadingLocationStationGuid = value; }
		}

		/// <summary>
		/// Gets or sets the improper additization.
		/// </summary>
		[QueryWriterField("Improper Additization", "tblTransactionLineItems.ImproperAdditization")]
		new public bool? ImproperAdditization
		{
			get { return base.ImproperAdditization; }
			set { base.ImproperAdditization = value; }
		}

		/// <summary>
		/// Gets or sets the contaminate prompt.
		/// </summary>
		[QueryWriterField("Contaminate Prompt", "tblTransactionLineItems.ContaminatePrompt")]
		new public bool? ContaminatePrompt
		{
			get { return base.ContaminatePrompt; }
			set { base.ContaminatePrompt = value; }
		}

		[QueryWriterField("Compartments Previously Loaded", "tblTransactionLineItems.CompartmentsPreviouslyLoaded")]
		new public bool? CompartmentsPreviouslyLoaded
		{
			get { return base.CompartmentsPreviouslyLoaded; }
			set { base.CompartmentsPreviouslyLoaded = value; }
		}

		// START   2014-Apr-04 p carpenter added to support expanded FSR fields.
		[QueryWriterField("MeterStartObtainedAutomaticallyFlag", "tblTransactionLineItems.MeterStartObtainedAutomaticallyFlag")]
		public bool? MeterStartObtainedAutomaticallyFlag
		{
			get { return base.meterStartObtainedAutomaticallyFlag; }
			set { base.meterStartObtainedAutomaticallyFlag = value; }
		}

		[QueryWriterField("MeterStopObtainedAutomaticallyFlag", "tblTransactionLineItems.MeterStopObtainedAutomaticallyFlag")]
		public bool? MeterStopObtainedAutomaticallyFlag
		{
			get { return base.meterStopObtainedAutomaticallyFlag; }
			set { base.meterStopObtainedAutomaticallyFlag = value; }
		}

		[QueryWriterField("DualFuelingModeFlag", "tblTransactionLineItems.DualFuelingModeFlag")]
		public bool? DualFuelingModeFlag
		{
			get { return base.dualFuelingModeFlag; }
			set { base.dualFuelingModeFlag = value; }
		}

		[QueryWriterField("FlowRate", "tblTransactionLineItems.FlowRate")]
		public double? FlowRate
		{
			get { return base.flowRate; }
			set { base.flowRate = value; }
		}

		[QueryWriterField("EngineRunTime", "tblTransactionLineItems.EngineRunTime")]
		public double? EngineRunTime
		{
			get { return base.engineRunTime; }
			set { base.engineRunTime = value; }
		}

		[QueryWriterField("FuelCompressionFactor", "tblTransactionLineItems.FuelCompressionFactor")]
		public double? FuelCompressionFactor
		{
			get { return base.fuelCompressionFactor; }
			set { base.fuelCompressionFactor = value; }
		}

		[QueryWriterField("HydrantPressure", "tblTransactionLineItems.HydrantPressure")]
		public double? HydrantPressure
		{
			get { return base.hydrantPressure; }
			set { base.hydrantPressure = value; }
		}

		[QueryWriterField("MobileDeviceID", "tblTransactionLineItems.MobileDeviceID")]
		public string MobileDeviceID
		{
			get { return base.mobileDeviceID; }
			set { base.mobileDeviceID = value; }
		}

		[QueryWriterField("MobileDeviceGuid", "tblTransactionLineItems.MobileDeviceGuid")]
		public Guid? MobileDeviceGuid
		{
			get { return base.mobileDeviceGuid; }
			set { base.mobileDeviceGuid = value; }
		}

		[QueryWriterField("DualFuelingPrimaryFlag", "tblTransactionLineItems.DualFuelingPrimaryFlag")]
		public bool? DualFuelingPrimaryFlag
		{
			get { return base.dualFuelingPrimaryFlag; }
			set { base.dualFuelingPrimaryFlag = value; }
		}

		[QueryWriterField("TemperatureQualityStatus", "tblTransactionLineItems.TemperatureQualityStatus")]
		public string TemperatureQualityStatus
		{
			get { return base.temperatureQualityStatus; }
			set { base.temperatureQualityStatus = value; }
		}

		[QueryWriterField("NetVolumeIndicator", "tblTransactionLineItems.NetVolumeIndicator")]
		public bool? NetVolumeIndicator
		{
			get { return base.netVolumeIndicator; }
			set { base.netVolumeIndicator = value; }
		}



		// END   2014-Apr-04 p carpenter added to support expanded FSR fields.

		#endregion Properties

		/// <summary>
		/// Gets the LineItemDO property name associated with the specified database column name.
		/// </summary>
		/// <param name="databaseColumnName">The database column name of the property</param>
		/// <returns>The property name associated with the specified database column name</returns>
		public static string GetPropertyName(string databaseColumnName)
		{
			string propertyName;
			if (DbNameToPropertyMap.TryGetValue(databaseColumnName, out propertyName))
			{
				return propertyName;
			}

			return databaseColumnName;
		}

		#region Public methods
		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="transTypeId">
		/// The transaction type ID.
		/// </param>
		public void Load(DataRow row, TransactionTypes transTypeId)
		{
			this.TransactionLineItemGuid	= getGuid(row["TransactionLineItemGuid"]);
			this.AcknowledgedDateTime		= getOptionalDateTimeOffset(row["AcknowledgedDateTime"]);
			this.ArmNumber					= getOptionalInt(row["ArmNumber"]);
			this.BatchNumber				= getString(row["BatchNumber"]);
			this.BottomVolume				= getOptionalDouble(row["BottomVolume"]);
			this.CLIN						= getString(row["CLIN"]);
			this.CompletionDateTime			= getOptionalDateTimeOffset(row["CompletionDateTime"]);
			this.ContractNumber				= getString(row["ContractNumber"]);
			this.Customs					= getString(row["Customs"]);
			this.DeleteFlag					= getBool(row["DeleteFlag"]);
			this.Density					= getOptionalDouble(row["Density"]);
			this.DifferentialPressure		= getOptionalDouble(row["DifferentialPressure"]);
			this.DispatchedDateTime			= getOptionalDateTimeOffset(row["DispatchedDateTime"]);
			this.DocumentNumber				= getString(row["DocumentNumber"]);
			this.FreezePoint				= getOptionalDouble(row["FreezePoint"]);
			this.LineFill					= getOptionalDouble(row["LineFill"]);
			this.LineNumber					= getOptionalInt(row["LineNumber"]);
			this.LoadRackVariance			= getOptionalDouble(row["LoadRackVariance"]);
			this.NetCapacity				= getOptionalDouble(row["NetCapacity"]);
			this.OnLocationTime				= getOptionalDateTimeOffset(row["OnLocationTime"]);
			this.OperatorID					= getString(row["OperatorID"]);
			this.OperatorPersonnelGuid		= getGuid(row["OperatorPersonnelGuid"]);
			this.Pit						= getString(row["Pit"]);
			this.PresetAmount				= getOptionalDouble(row["PresetAmount"]);
			this.Product					= getString(row["Product"]);
			this.ProductCode				= getString(row["ProductCode"]);
			this.ProductPrice				= getOptionalDouble(row["ProductPrice"]);
			this.ProductType				= getString(row["ProductType"]);
			this.ProductGuid				= getGuid(row["ProductGuid"]);
			this.ReceiptVariance			= getOptionalDouble(row["ReceiptVariance"]);
			this.RequestedBy				= getString(row["RequestedBy"]);
			this.RequestedDateTime			= getOptionalDateTimeOffset(row["RequestedDateTime"]);
			this.SequenceId					= getOptionalInt(row["SequenceID"]);
			this.Status							= (TransactionStatus)getInt(row["LookupTransactionStatusIndex"]);
			this.TankStatus					= getString(row["TankStatus"]);
			this.Temperature					= getOptionalDouble(row["Temperature"]);
			this.ValidationDateTime			= getOptionalDateTimeOffset(row["ValidationDateTime"]);
			this.VCF								= getOptionalDouble(row["VCF"]);
			this.Pressure						= getOptionalDouble(row["Pressure"]);
			this.AdditiveProfileID			= getString(row["AdditiveProfileID"]);
			this.AdditiveProfileGuid		= getGuid(row["AdditiveProfileGuid"]);
			this.StorageLocationID			= getString(row["StorageLocationID"]);
			this.StorageLocationTankGuid	= getGuid(row["StorageLocationTankGuid"]);
			this.MeterID						= getString(row["MeterID"]);
			this.MeterGuid						= getGuid(row["MeterGuid"]);
			this.FuelCompressionFactor    = getOptionalDouble(row["FuelCompressionFactor"]);
			this.NetVolumeIndicator			= getBool(row["NetVolumeIndicator"]);

			// Order Entry
			this.EngineeringUnitsIndex = (EngineeringUnit)getInt(row["EngineeringUnitsIndex"]);
			this.CustomerProductName = getString(row["CustomerProductName"]);
			this.CustomerProductCode = getString(row["CustomerProductCode"]);
			this.OrderReferenceTransactionLineItemGuid = getGuid(row["OrderReferenceTransactionLineItemGuid"]);

			// Volume
			this.Quantity.GrossInventoryChange = getDouble(row["GrossQuantity"]);
			this.Quantity.DeliveredGrossInventoryChange = getDouble(row["DeliveredGrossQuantity"]);
			this.Quantity.NetInventoryChange = getDouble(row["NetQuantity"]);
			this.Quantity.DeliveredNetInventoryChange = getDouble(row["DeliveredNetQuantity"]);

			if ((transTypeId == TransactionTypes.T17_Order) || (transTypeId == TransactionTypes.T18_SupplyOrder))
			{
				this.Quantity.AffectsInventory = false;
			}
			else
			{
				this.Quantity.AffectsInventory = true;
			}

			// Equipment
			this.DestinationEQ.EquipmentModel			= getString(row["DestinationEquipmentModel"]);
			this.DestinationEQ.EquipmentType			= getString(row["DestinationEquipmentType"]);
			this.DestinationEQ.RegistrationID			= getString(row["DestinationRegistrationID"]);
			this.DestinationEQ.SerialNumber				= getString(row["DestinationSerialNumber"]);
			this.DestinationEQ.CompanyEquipmentID		= getString(row["DestinationCompanyEquipmentID"]);
			this.DestinationEQ.EquipmentGuid			= getValue<Guid>(row["DestinationEquipmentGuid"], Guid.Empty);
			this.DestinationCompartmentID				= getString(row["DestinationCompartmentID"]);
			this.DestinationCompartmentEquipmentGuid	= getGuid(row["DestinationCompartmentEquipmentGuid"]);
			this.SourceEQ.EquipmentModel				= getString(row["SourceEquipmentModel"]);
			this.SourceEQ.EquipmentType					= getString(row["SourceEquipmentType"]);
			this.SourceEQ.RegistrationID				= getString(row["SourceRegistrationID"]);
			this.SourceEQ.SerialNumber					= getString(row["SourceSerialNumber"]);
			this.SourceEQ.CompanyEquipmentID			= getString(row["SourceCompanyEquipmentID"]);
			this.SourceEQ.EquipmentGuid					= getValue<Guid>(row["SourceEquipmentGuid"], Guid.Empty);
			this.SourceCompartmentID					= getString(row["SourceCompartmentID"]);
			this.SourceCompartmentEquipmentGuid			= getGuid(row["SourceCompartmentEquipmentGuid"]);

			// Meter Reading
			this.MeterReading.MeterFactor	= getOptionalDouble(row["MeterFactor"]);
			this.MeterReading.MeterStart	= getOptionalDouble(row["MeterStart"]);
			this.MeterReading.MeterStop		= getOptionalDouble(row["MeterStop"]);
			this.MeterReading.StartDateTime = getOptionalDateTimeOffset(row["MeterStartDateTime"]);
			this.MeterReading.StopDateTime	= getOptionalDateTimeOffset(row["MeterStopDateTime"]);

			// Certificate Of Analysis
			this.COAID		= getString(row["COAID"]);
			this.COANote	= getString(row["COANote"]);
			this.COAWaiver	= getBool(row["COAWaiver"]);

			// Quality
			this.Quality = (TransactionQuality)getInt(row["LookupQualityIndex"]);

			// Tax fields
			this.Tax1 = getOptionalDouble(row["Tax1"]);
			this.Tax2 = getOptionalDouble(row["Tax2"]);
			this.Tax3 = getOptionalDouble(row["Tax3"]);
			this.Tax4 = getOptionalDouble(row["Tax4"]);
			this.Tax5 = getOptionalDouble(row["Tax5"]);

			// Loading Location
			this.LoadingLocationID = getString(row["LoadingLocationID"]);
			this.LoadingLocationStationGuid = getGuid(row["LoadingLocationStationGuid"]);

			// Improper Additization
			this.ImproperAdditization = getBool(row["ImproperAdditization"]);
			this.BrokenBlend = getBool(row["BrokenBlend"]);

			// Contaminate Prompt
         this.ContaminatePrompt = getOptionalBool(row["ContaminatePrompt"]);
         this.CompartmentsPreviouslyLoaded = getOptionalBool(row["CompartmentsPreviouslyLoaded"]);
         this.CompartmentsEmpty = getOptionalBool(row["CompartmentsEmpty"]);

			// vthompson 5-21-2008
			// Generic Flags
			this.Flag01 = getBool(row["Flag01"]);
			this.Flag02 = getBool(row["Flag02"]);
			this.Flag03 = getBool(row["Flag03"]);
			this.Flag04 = getBool(row["Flag04"]);
			this.Flag05 = getBool(row["Flag05"]);
			this.Flag06 = getBool(row["Flag06"]);

			// Generic number fields
			this.Number01 = getOptionalDouble(row["Number01"]);
			this.Number02 = getOptionalDouble(row["Number02"]);
			this.Number03 = getOptionalDouble(row["Number03"]);
			this.Number04 = getOptionalDouble(row["Number04"]);
			this.Number05 = getOptionalDouble(row["Number05"]);
			this.Number06 = getOptionalDouble(row["Number06"]);

			// vthompson 5-21-2008 for ADF
			this.OdometerHours = getOptionalDouble(row["OdometerHours"]);
			this.EndDeliveryDate = getOptionalDateTimeOffset(row["EndDeliveryDate"]);
			this.RequestedDeliveryDate = getOptionalDateTimeOffset(row["RequestedDeliveryDate"]);

			// vthompson 05-22-2008
			this.InvoiceNumber = getString(row["InvoiceNumber"]);
			this.InvoiceLineNumber = getString(row["InvoiceLineNumber"]);
			this.AlternativeGrossVolume = getOptionalDouble(row["AlternativeGrossVolume"]);
			this.AlternativeNetVolume = getOptionalDouble(row["AlternativeNetVolume"]);
			this.AlternativeUnits = getOptionalInt(row["AlternativeUnits"]);
			this.TankLevel = getOptionalDouble(row["TankLevel"]);
			this.TankLevelUnits = getOptionalInt(row["TankLevelUnits"]);

			// vt 07-09-2008
			this.Date01 = getOptionalDateTimeOffset(row["Date01"]);
			this.Date02 = getOptionalDateTimeOffset(row["Date02"]);
			this.Date03 = getOptionalDateTimeOffset(row["Date03"]);
			this.Date04 = getOptionalDateTimeOffset(row["Date04"]);

			// vt 07-10-2008
			this.NonDomesticPrice = getOptionalDouble(row["NonDomesticPrice"]);
			this.CurrencyGuid = getGuid(row["CurrencyGuid"]);
			this.ExchangeRate = getOptionalDouble(row["ExchangeRate"]);
			this.QualityTestNumber = getString(row["QualityTestNumber"]);
			this.Odometer = getOptionalDouble(row["Odometer"]);

			// vthompson 9/22/2008
			this.DeliveryLocation = getString(row["DeliveryLocation"]);

			// wcg 9/16/2009
			this.Variance = getOptionalDouble(row["Variance"]);
			this.PartialFill = getOptionalBool(row["PartialFill"]);
			this.Quantity.Mass = getDouble(row["MassQuantity"]);
			this.Quantity.NetManualValueFlag = getOptionalBool(row["NetManualValueFlag"]);
			this.Quantity.GrossManualValueFlag = getOptionalBool(row["GrossManualValueFlag"]);
			this.Quantity.MassManualValueFlag = getOptionalBool(row["MassManualValueFlag"]);
         this.Quantity.PackageManualValueFlag = getOptionalBool(row["PackageManualValueFlag"]);
         this.Quantity.VcfManualValueFlag = getOptionalBool(row["VcfManualValueFlag"]);
			this.Quantity.DeliveredGrossManualValueFlag = getOptionalBool(row["DeliveredGrossManualValueFlag"]);
			this.Quantity.DeliveredNetManualValueFlag = getOptionalBool(row["DeliveredNetManualValueFlag"]);

			// JS20100114 CCP-042
			this.WacCalculated = false;

			this.CleanLineProduct = getBool(row["CleanLineItem"]);
			this.CleanLineDeductProduct = getBool(row["CleanLineDeductItem"]);
			this.CleanLineDeductQuantity = getOptionalDouble(row["CleanLineDeductQuantity"]);
			this.CleanLinePackQuantity = getOptionalDouble(row["CleanLinePackQuantity"]);


			// Depending on how the line item is retreived, we may have the unit and decimal place fields.
			// Before trying to load them, make sure they exist.
			if (row.Table != null)
			{
				if (row.Table.Columns.Contains("LevelUnitIndex"))
				{
					this.LevelUnits = (EngineeringUnit)getInt(row["LevelUnitIndex"]);
					this.TemperatureUnits = (EngineeringUnit)getInt(row["TemperatureUnitIndex"]);
					this.DensityUnits = (EngineeringUnit)getInt(row["DensityUnitIndex"]);
					this.PressureUnits = (EngineeringUnit)getInt(row["PressureUnitIndex"]);
					this.FlowUnits = (EngineeringUnit)getInt(row["FlowUnitIndex"]);
					this.VolumeUnits = (EngineeringUnit)getInt(row["VolumeUnitIndex"]);
					this.MassUnits = (EngineeringUnit)getInt(row["MassUnitIndex"]);
				}

				if (row.Table.Columns.Contains("LevelDecimalPlaces"))
				{
					this.LevelDecimalPlaces = (byte)getInt(row["LevelDecimalPlaces"]);
					this.TemperatureDecimalPlaces = (byte)getInt(row["TemperatureDecimalPlaces"]);
					this.DensityDecimalPlaces = (byte)getInt(row["DensityDecimalPlaces"]);
					this.PressureDecimalPlaces = (byte)getInt(row["PressureDecimalPlaces"]);
					this.FlowDecimalPlaces = (byte)getInt(row["FlowDecimalPlaces"]);
					this.VolumeDecimalPlaces = (byte)getInt(row["VolumeDecimalPlaces"]);
					this.MassDecimalPlaces = (byte)getInt(row["MassDecimalPlaces"]);
				}

				if (row.Table.Columns.Contains("VolumePackageSize"))
				{
					this.VolumePackageSize = getDouble(row["VolumePackageSize"]);
				}

				if (row.Table.Columns.Contains("MassPackageSize"))
				{
					this.MassPackageSize = getDouble(row["MassPackageSize"]);
				}

				if (!string.IsNullOrEmpty(row["VcfModuleSettings"] as string))
				{
					try
					{
						using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(row["VcfModuleSettings"] as string)))
						{
							DataContractSerializer serializer = new DataContractSerializer(typeof(VcfModuleSettings));
							this.VcfModuleSettings = serializer.ReadObject(memoryStream) as VcfModuleSettings;
						}
					}
					catch
					{
						// Try catch can be removed after next release after FM12 SP3 as it will be fixed on first start after upgrade to SP3
						// All products will be resaved with new serializer on first start after upgrade.
						var serializer = CachingXmlSerializerFactory.Create(typeof(VcfModuleSettings));
						var stringReader = new StringReader(DataObject.getValue<string>(row["VcfModuleSettings"], null));
						this.VcfModuleSettings = (VcfModuleSettings)serializer.Deserialize(stringReader);
					}
				}
			}
		}

		public void UpdateDeliveredQuantities(bool stationEthanolExcess, LoadArmClass loadArm, bool invertQuantity)
		{
            if (loadArm == null
            || !stationEthanolExcess
            || !this.IsEthanolBlend)
            {
                this.Quantity.DeliveredGrossInventoryChange = this.Quantity.GrossInventoryChange;
				this.Quantity.DeliveredNetInventoryChange = this.Quantity.NetInventoryChange;

				foreach (SubLineItemDO subLineItem in this.SubLineItems)
				{
					subLineItem.Quantity.DeliveredGrossInventoryChange = subLineItem.Quantity.GrossInventoryChange;
					subLineItem.Quantity.DeliveredNetInventoryChange = subLineItem.Quantity.NetInventoryChange;
				}

				return;
			}

			var ethanolExcess = new EthanolExcess();

			// Determine if this Senario #1 or #2, as indicated by the Ethanol being a Component or External Component
			var ethanolSubLineItems = this.SubLineItems.Where(x => x.IsEthanol).ToList();
			if (ethanolSubLineItems != null
			&& ethanolSubLineItems.Count == 1)
			{
				var ethanolComponents = loadArm.ComponentCollection.ToList().Where(x => x.AssignedGuid == ethanolSubLineItems[0].ProductGuid).ToList();

				// Scenario #1
				if (ethanolComponents != null &&
				ethanolComponents.Count == 1)
				{
					var subLineItemsAdditive = this.SubLineItems.Where(x => !x.IsEthanol && x.ProductType == ProductClass.ProductTypeID(FMBusinessObjects.DataObjects.ProductType.AdditiveProduct)).ToList();
					foreach (SubLineItemDO subLineItem in subLineItemsAdditive)
					{
						subLineItem.Quantity.DeliveredGrossInventoryChange = subLineItem.Quantity.GrossInventoryChange;
						subLineItem.Quantity.DeliveredNetInventoryChange = subLineItem.Quantity.NetInventoryChange;
					}

					var subLineItemsBob = this.SubLineItems.Where(x => !x.IsEthanol && x.ProductType == ProductClass.ProductTypeID(FMBusinessObjects.DataObjects.ProductType.ComponentProduct)).ToList();

					// total the components
					double grossQuantityBob = 0.0;
					double netQuantityBob = 0.0;
					foreach (var subLineItemBob in subLineItemsBob)
					{
						grossQuantityBob += subLineItemBob.Quantity.Gross;
						netQuantityBob += subLineItemBob.Quantity.Net;
					}

					if (grossQuantityBob == 0.0
					|| ethanolSubLineItems[0].Quantity.Gross == 0.0)
					{
						return;
					}

					this.Quantity.DeliveredGrossInventoryChange = ethanolSubLineItems[0].Quantity.GrossInventoryChange;
					this.Quantity.DeliveredNetInventoryChange = 0.0;
					ethanolSubLineItems[0].Quantity.DeliveredGrossInventoryChange = ethanolSubLineItems[0].Quantity.GrossInventoryChange;
					ethanolSubLineItems[0].Quantity.DeliveredNetInventoryChange = 0.0;

					foreach (var subLineItemBob in subLineItemsBob)
					{
						subLineItemBob.Quantity.DeliveredGrossInventoryChange = subLineItemBob.Quantity.GrossInventoryChange;

						if (subLineItemBob.Quantity.Gross == 0.0)
						{
							continue;
						}

						double ethanolGrossPerComponent = ethanolSubLineItems[0].Quantity.Gross * subLineItemBob.Quantity.Gross / grossQuantityBob;

						double standardVolumeEthanol = 0.0;
						double standardVolumeBob;
						double standardVolumeBge;
						double volumeBge = subLineItemBob.Quantity.Gross + ethanolGrossPerComponent;

						if (volumeBge <= 0.0)
						{
							continue;
						}

						// Weighted average of Temperature of Ethanol and BOB Teperature
						double temperatureBge = ethanolSubLineItems[0].Temperature.Value * ethanolSubLineItems[0].Quantity.Gross / volumeBge
														+ subLineItemBob.Temperature.Value * subLineItemBob.Quantity.Gross / volumeBge;

						ethanolExcess.CalculateBGEV60withVcfModuleSettings(subLineItemBob.Quantity.Gross,
																(subLineItemBob.Temperature.HasValue) ? subLineItemBob.Temperature.Value : 0,
																(subLineItemBob.Density.HasValue) ? subLineItemBob.Density.Value : 0,
																subLineItemBob.VcfModuleSettings.GetCommonComponentVcfModuleSettings(subLineItemBob.PressureUnits),
																ethanolGrossPerComponent,
																(ethanolSubLineItems[0].Temperature.HasValue) ? ethanolSubLineItems[0].Temperature.Value : 0,
																(ethanolSubLineItems[0].Density.HasValue) ? ethanolSubLineItems[0].Density.Value : 0,
																ethanolSubLineItems[0].VcfModuleSettings.GetCommonComponentVcfModuleSettings(subLineItemBob.PressureUnits),
																volumeBge,
																temperatureBge,
																subLineItemBob.VolumeUnits,
																subLineItemBob.TemperatureUnits,
																subLineItemBob.PressureUnits,
																subLineItemBob.DensityUnits,
																false,
																out standardVolumeEthanol,
																out standardVolumeBob,
																out standardVolumeBge,
																(this.Pressure.HasValue) ? this.Pressure.Value : 0,
																(subLineItemBob.Pressure.HasValue) ? subLineItemBob.Pressure.Value : 0,
																(ethanolSubLineItems[0].Pressure.HasValue) ? ethanolSubLineItems[0].Pressure.Value : 0);

						// Use the subLineItemBob.Quantity.Net rather than standardVolumeBob as these may vary slightly due to descrepency between FM and Preset Device.

						// all of the Volume Decimal Places should be the same, yet should they be different which really cannot happen, then it is best to round according to the line/subline decimal places
						standardVolumeEthanol = Math.Round(standardVolumeEthanol, ethanolSubLineItems[0].VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
						standardVolumeBge = Math.Round(standardVolumeBge, this.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);

						double excess = standardVolumeBge - (standardVolumeEthanol + subLineItemBob.Quantity.Net);

						if (invertQuantity)
						{
                            subLineItemBob.Quantity.Gross = -Math.Round(subLineItemBob.Quantity.Gross, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
                            subLineItemBob.Quantity.Net = -Math.Round(subLineItemBob.Quantity.Net, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
                            subLineItemBob.Quantity.DeliveredNetInventoryChange = -subLineItemBob.Quantity.Net;
							ethanolSubLineItems[0].Quantity.DeliveredNetInventoryChange -= standardVolumeEthanol + excess;
							this.Quantity.DeliveredGrossInventoryChange -= subLineItemBob.Quantity.Gross;
							this.Quantity.DeliveredNetInventoryChange -= standardVolumeBge;
						}
						else
						{
                            subLineItemBob.Quantity.Gross = Math.Round(subLineItemBob.Quantity.Gross, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
                            subLineItemBob.Quantity.Net = Math.Round(subLineItemBob.Quantity.Net, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
                            subLineItemBob.Quantity.DeliveredNetInventoryChange = subLineItemBob.Quantity.Net;
							ethanolSubLineItems[0].Quantity.DeliveredNetInventoryChange += standardVolumeEthanol + excess;
							this.Quantity.DeliveredGrossInventoryChange += subLineItemBob.Quantity.Gross;
							this.Quantity.DeliveredNetInventoryChange += standardVolumeBge;
						}
					}
				}
			}
		}

		// Automatic BOL for ethanol expansion scenario 2 - Handles expansion calculation for multiple ethanol sub line items created by the Station Manager
		public void UpdateLoadRackScenario2DeliveredQuantities(bool stationEthanolExcess, LoadArmClass loadArm, bool invertQuantity)
		{
			this.Quantity.DeliveredGrossInventoryChange = this.Quantity.GrossInventoryChange;
			this.Quantity.DeliveredNetInventoryChange = this.Quantity.NetInventoryChange;

			foreach (SubLineItemDO subLineItem in this.SubLineItems)
			{
				subLineItem.Quantity.DeliveredGrossInventoryChange = subLineItem.Quantity.GrossInventoryChange;
				subLineItem.Quantity.DeliveredNetInventoryChange = subLineItem.Quantity.NetInventoryChange;
			}

			if (loadArm == null
				|| !stationEthanolExcess
				|| !this.IsEthanolBlend)
			{
				return;
			}

			string componentProductType = ProductClass.ProductTypeID(FMBusinessObjects.DataObjects.ProductType.ComponentProduct);
			var ethanolSubLineItems = this.SubLineItems.Where(x => x.ProductType == componentProductType && x.IsEthanol).ToList();
			var bobSubLineItems = this.SubLineItems.Where(x => x.ProductType == componentProductType && !x.IsEthanol).ToList();
			var componentSubLineItems = bobSubLineItems.Concat(ethanolSubLineItems).ToList();

			if (componentSubLineItems.Count == 0)
			{
				return;
			}

			var ethanolSubLineItemsByMeter = ethanolSubLineItems
				.Where(x => !string.IsNullOrWhiteSpace(x.MeterID))
				.GroupBy(x => x.MeterID, StringComparer.Ordinal)
				.ToDictionary(x => x.Key, x => x.ToList(), StringComparer.Ordinal);
			var bobSubLineItemsByMeter = bobSubLineItems
				.Where(x => !string.IsNullOrWhiteSpace(x.MeterID))
				.GroupBy(x => x.MeterID, StringComparer.Ordinal)
				.ToDictionary(x => x.Key, x => x.ToList(), StringComparer.Ordinal);

			var ee = new EthanolExcess();
			double inventoryDirection = invertQuantity ? -1.0 : 1.0;

			foreach (KeyValuePair<string, List<SubLineItemDO>> bobMeterGroup in bobSubLineItemsByMeter)
			{
				List<SubLineItemDO> ethanolMeterGroup;
				if (bobMeterGroup.Value.Count != 1
					|| !ethanolSubLineItemsByMeter.TryGetValue(bobMeterGroup.Key, out ethanolMeterGroup)
					|| ethanolMeterGroup.Count != 1)
				{
					continue;
				}

				SubLineItemDO subLineItemBob = bobMeterGroup.Value[0];
				SubLineItemDO subLineItemEthanol = ethanolMeterGroup[0];

				if (double.IsNaN(subLineItemBob.Quantity.Gross)
					|| double.IsNaN(subLineItemBob.Quantity.NetInventoryChange)
					|| double.IsNaN(subLineItemEthanol.Quantity.Gross)
					|| double.IsNaN(subLineItemEthanol.Quantity.NetInventoryChange)
					|| !this.Temperature.HasValue)
				{
					continue;
				}


				double grossBge = subLineItemBob.Quantity.Gross + subLineItemEthanol.Quantity.Gross;
				double standardVolumeEthanol;
				double volumeBob;
				double standardVolumeBob;
				double standardDensityBge;
				double standardVolumeBge;
				double vcfBge;

				ee.CalculateBOBV60withVcfModuleSettings(
					grossBge,
					this.temperature.Value,
					(subLineItemBob.Density.HasValue) ? subLineItemBob.Density.Value : 0,
					subLineItemBob.VcfModuleSettings.GetCommonComponentVcfModuleSettings(subLineItemBob.PressureUnits),
					subLineItemEthanol.Quantity.Gross,
					(subLineItemEthanol.Temperature.HasValue) ? subLineItemEthanol.Temperature.Value : 0,
					(subLineItemEthanol.Density.HasValue) ? subLineItemEthanol.Density.Value : 0,
					subLineItemEthanol.VcfModuleSettings.GetCommonComponentVcfModuleSettings(subLineItemBob.PressureUnits),
					subLineItemBob.VolumeUnits,
					subLineItemBob.TemperatureUnits,
					subLineItemBob.PressureUnits,
					subLineItemBob.DensityUnits,
					false,
					(this.Pressure.HasValue) ? this.Pressure.Value : 0,
					subLineItemEthanol.Pressure.Value,
					out standardVolumeEthanol,
					out volumeBob,
					out standardVolumeBob,
					out standardDensityBge,
					out standardVolumeBge,
					out vcfBge);

				double volumeBobRounded = Math.Round(volumeBob, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
				double standardVolumeBobRounded = Math.Round(standardVolumeBob, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
				double standardVolumeEthanolRounded = Math.Round(standardVolumeEthanol, subLineItemEthanol.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
				double ethanolExcess = standardVolumeBge - (standardVolumeBob + standardVolumeEthanol);
				double deliveredNetEthanol = standardVolumeEthanol + ethanolExcess;
				double deliveredNetEthanolRounded = Math.Round(deliveredNetEthanol, subLineItemEthanol.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);

				subLineItemBob.Quantity.GrossInventoryChange = inventoryDirection * volumeBobRounded;
				subLineItemBob.Quantity.DeliveredGrossInventoryChange = inventoryDirection * volumeBobRounded;
				subLineItemBob.Quantity.NetInventoryChange = inventoryDirection * standardVolumeBobRounded;
				subLineItemBob.Quantity.DeliveredNetInventoryChange = inventoryDirection * standardVolumeBobRounded;
				subLineItemEthanol.Quantity.NetInventoryChange = inventoryDirection * standardVolumeEthanolRounded;
				subLineItemEthanol.Quantity.DeliveredNetInventoryChange = inventoryDirection * deliveredNetEthanolRounded;
			}


			double totalGross = 0.0;
			double totalNet = 0.0;
			double totalDeliveredGross = 0.0;
			double totalDeliveredNet = 0.0;

			foreach (SubLineItemDO componentSubLineItem in componentSubLineItems)
			{

				totalGross += componentSubLineItem.Quantity.GrossInventoryChange;
				totalNet += componentSubLineItem.Quantity.NetInventoryChange;
				totalDeliveredGross += componentSubLineItem.Quantity.DeliveredGrossInventoryChange;
				totalDeliveredNet += componentSubLineItem.Quantity.DeliveredNetInventoryChange;
			}

			this.Quantity.GrossInventoryChange = Math.Round(totalGross, this.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
			this.Quantity.NetInventoryChange = Math.Round(totalNet, this.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
			this.Quantity.DeliveredGrossInventoryChange = Math.Round(totalDeliveredGross, this.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
			this.Quantity.DeliveredNetInventoryChange = Math.Round(totalDeliveredNet, this.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);

			if (totalDeliveredGross != 0.0)
			{
				this.VCF = Math.Round(
					totalDeliveredNet / totalDeliveredGross,
					5,
					MidpointRounding.AwayFromZero);
			}

		}

		public void UpdateDeliveredQuantities(bool stationEthanolExcess, LoadArmClass loadArm, bool invertQuantity, Dictionary<Guid, double> bobBlendPercentages)
		{
            if (loadArm == null
				|| !stationEthanolExcess
				|| !this.IsEthanolBlend)
            {
                this.Quantity.DeliveredGrossInventoryChange = this.Quantity.GrossInventoryChange;
                this.Quantity.DeliveredNetInventoryChange = this.Quantity.NetInventoryChange;

                foreach (SubLineItemDO subLineItem in this.SubLineItems)
                {
                    subLineItem.Quantity.DeliveredGrossInventoryChange = subLineItem.Quantity.GrossInventoryChange;
                    subLineItem.Quantity.DeliveredNetInventoryChange = subLineItem.Quantity.NetInventoryChange;
                }

                return;
            }
            var ethanolSubLineItems = this.SubLineItems.Where(x => x.IsEthanol).ToList();
			if (ethanolSubLineItems.Count != 1
				|| loadArm.ExternalComponentCollection == null)
			{
				return;
			}

			var ethanolExcess = new EthanolExcess();
            var ethanolExternalComponents = loadArm.ExternalComponentCollection.ToList().Where(x => x.AssignedGuid == ethanolSubLineItems[0].ProductGuid).ToList();

            if (ethanolExternalComponents != null &&
            ethanolExternalComponents.Count == 1)
            {
                var subLineItemsBob = this.SubLineItems.Where(x => !x.IsEthanol && x.ProductType == ProductClass.ProductTypeID(FMBusinessObjects.DataObjects.ProductType.ComponentProduct)).ToList();
                if (subLineItemsBob.Count == 0)
                {
                    return;
                }

                SubLineItemDO subLineItemEthanol = ethanolSubLineItems[0];
                if (this.Quantity.Gross <= 0.0
                || subLineItemEthanol.Quantity.Gross <= 0.0
                || this.Quantity.Gross <= subLineItemEthanol.Quantity.Gross
                || double.IsNaN(this.Quantity.Gross)
                || double.IsNaN(subLineItemEthanol.Quantity.Gross)
                || !this.Temperature.HasValue
                || double.IsNaN(this.Temperature.Value)
                || !subLineItemEthanol.Temperature.HasValue
                || double.IsNaN(subLineItemEthanol.Temperature.Value)
                || !subLineItemEthanol.Density.HasValue
                || subLineItemEthanol.Density.Value <= 0.0
                || double.IsNaN(subLineItemEthanol.Density.Value)
                || subLineItemEthanol.VcfModuleSettings == null
                || bobBlendPercentages == null)
                {
                    return;
                }

                // should be un-modified value given from the user. this should be a measured value already available to the user  
                // cannot take the weighted average since BOB quantity is unknown
                double temperatureBge = this.Temperature.HasValue ? this.Temperature.Value : 0;

				double totalGrossVolumeComponents = this.Quantity.Gross - subLineItemEthanol.Quantity.Gross;
				double totalBobBlendPercentage = 0.0;
				foreach (SubLineItemDO subLineItemBob in subLineItemsBob)
				{
					double bobBlendPercentage;
					if (subLineItemBob.VcfModuleSettings == null
						|| !subLineItemBob.Density.HasValue
						|| subLineItemBob.Density.Value <= 0.0
						|| double.IsNaN(subLineItemBob.Density.Value)
						|| !bobBlendPercentages.TryGetValue(subLineItemBob.ProductGuid, out bobBlendPercentage)
						|| bobBlendPercentage <= 0.0
						|| double.IsNaN(bobBlendPercentage))
					{
						return;
					}

					totalBobBlendPercentage += bobBlendPercentage;
                }

				if (totalGrossVolumeComponents <= 0.0
					|| double.IsNaN(totalGrossVolumeComponents)
					|| totalBobBlendPercentage <= 0.0
					|| double.IsNaN(totalBobBlendPercentage))
				{
					return;
				}

				var subLineItemsAdditive = this.SubLineItems.Where(x => !x.IsEthanol && x.ProductType == ProductClass.ProductTypeID(FMBusinessObjects.DataObjects.ProductType.AdditiveProduct)).ToList();
				foreach (SubLineItemDO subLineItem in subLineItemsAdditive)
				{
					subLineItem.Quantity.DeliveredGrossInventoryChange = subLineItem.Quantity.GrossInventoryChange;
					subLineItem.Quantity.DeliveredNetInventoryChange = subLineItem.Quantity.NetInventoryChange;
				}

				this.Quantity.Net = 0.0;
				subLineItemEthanol.Quantity.Net = 0.0;
				this.Quantity.DeliveredGrossInventoryChange = subLineItemEthanol.Quantity.GrossInventoryChange;
				this.Quantity.DeliveredNetInventoryChange = 0.0;
				subLineItemEthanol.Quantity.DeliveredGrossInventoryChange = subLineItemEthanol.Quantity.GrossInventoryChange;
				subLineItemEthanol.Quantity.DeliveredNetInventoryChange = 0.0;
                double totalNetVolumeBob = 0.0;

                foreach (SubLineItemDO subLineItemBob in subLineItemsBob)
				{
					double volumeGrossBobEstimate = totalGrossVolumeComponents * (bobBlendPercentages[subLineItemBob.ProductGuid] / totalBobBlendPercentage);	
					double ethanolGrossPerComponent = subLineItemEthanol.Quantity.Gross * volumeGrossBobEstimate / totalGrossVolumeComponents;

                    double standardVolumeEthanol = 0.0;
                    double volumeBob = 0.0;
                    double standardVolumeBob = 0.0;
                    double standardDensityBge = 0.0;
                    double standardVolumeBge = 0.0;
					double vcfBge = 0.0;

					double weightedBgePerComponent = volumeGrossBobEstimate + ethanolGrossPerComponent;
                    ethanolExcess.CalculateBOBV60withVcfModuleSettings(weightedBgePerComponent,
                                                            temperatureBge,
                                                            (subLineItemBob.Density.HasValue) ? subLineItemBob.Density.Value : 0,
                                                            subLineItemBob.VcfModuleSettings.GetCommonComponentVcfModuleSettings(subLineItemBob.PressureUnits),
                                                            ethanolGrossPerComponent,
                                                            (subLineItemEthanol.Temperature.HasValue) ? subLineItemEthanol.Temperature.Value : 0,
                                                            (subLineItemEthanol.Density.HasValue) ? subLineItemEthanol.Density.Value : 0,
                                                            subLineItemEthanol.VcfModuleSettings.GetCommonComponentVcfModuleSettings(subLineItemBob.PressureUnits),
                                                            subLineItemBob.VolumeUnits,
                                                            subLineItemBob.TemperatureUnits,
                                                            subLineItemBob.PressureUnits,
                                                            subLineItemBob.DensityUnits,
                                                            false,
                                                            (this.Pressure.HasValue) ? this.Pressure.Value : 0,
                                                            (subLineItemEthanol.Pressure.HasValue) ? subLineItemEthanol.Pressure.Value : 0,
                                                            out standardVolumeEthanol,
                                                            out volumeBob,
                                                            out standardVolumeBob,
                                                            out standardDensityBge,
                                                            out standardVolumeBge,
															out vcfBge);

                    volumeBob = Math.Round(volumeBob, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
                    standardVolumeBob = Math.Round(standardVolumeBob, subLineItemBob.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
                    standardVolumeEthanol = Math.Round(standardVolumeEthanol, subLineItemEthanol.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);

                    double excess = standardVolumeBge - (standardVolumeEthanol + standardVolumeBob);
                    double ethanolDeliveredNet = standardVolumeEthanol + excess;

					this.vcf = vcfBge;
                    if (invertQuantity)
                    {
						subLineItemBob.Quantity.GrossInventoryChange = -volumeGrossBobEstimate;
                        subLineItemBob.Quantity.DeliveredGrossInventoryChange = -volumeGrossBobEstimate;
                        subLineItemBob.Quantity.NetInventoryChange = -standardVolumeBob;
                        subLineItemBob.Quantity.DeliveredNetInventoryChange = -standardVolumeBob;
						subLineItemEthanol.Quantity.NetInventoryChange -= standardVolumeEthanol;
                        subLineItemEthanol.Quantity.DeliveredNetInventoryChange -= ethanolDeliveredNet;
						totalNetVolumeBob -= standardVolumeBob;
						this.Quantity.DeliveredGrossInventoryChange -= volumeGrossBobEstimate;
						this.Quantity.DeliveredNetInventoryChange -= standardVolumeBge;
                    }
                    else
                    {
                        subLineItemBob.Quantity.GrossInventoryChange = volumeGrossBobEstimate;
                        subLineItemBob.Quantity.DeliveredGrossInventoryChange = volumeGrossBobEstimate;
                        subLineItemBob.Quantity.NetInventoryChange = standardVolumeBob;
                        subLineItemBob.Quantity.DeliveredNetInventoryChange = standardVolumeBob;
                        subLineItemEthanol.Quantity.NetInventoryChange += standardVolumeEthanol;
                        subLineItemEthanol.Quantity.DeliveredNetInventoryChange += ethanolDeliveredNet;
                        totalNetVolumeBob += standardVolumeBob;
                        this.Quantity.DeliveredGrossInventoryChange += volumeGrossBobEstimate;
						this.Quantity.DeliveredNetInventoryChange += standardVolumeBge;
                    }
                }

				this.Quantity.NetInventoryChange = totalNetVolumeBob + subLineItemEthanol.Quantity.NetInventoryChange;
            }
        }
	}

	#endregion
}

