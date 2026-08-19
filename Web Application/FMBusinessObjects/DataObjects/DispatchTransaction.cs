// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchTransaction.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchTransaction data type
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.Runtime.Serialization;

	/// <summary>
	/// Dispatch transaction object
	/// </summary>
	[DataContract]
	[Serializable]
	public class DispatchTransaction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchTransaction"/> class.
		/// </summary>
		/// <param name="dataRow">
		/// The data row.
		/// </param>
		/// <param name="timePattern">
		/// The time pattern.
		/// </param>
		public DispatchTransaction(DataRow dataRow, string timePattern)
		{
			this.TimePattern = timePattern;

			this.AliasName = dataRow["AliasName"].ToString();
			this.BillToID = dataRow["BillToID"].ToString();
			this.CardNumber = dataRow["CardNumber"].ToString();
			this.Color = string.Empty;
			this.ContactSurname = dataRow["ContactSurname"].ToString();
			this.AircraftID = dataRow["AircraftID"].ToString();
			this.Flag01 = DataObject.getOptionalBool(dataRow["Flag01"]);
			this.Flag02 = DataObject.getOptionalBool(dataRow["Flag02"]);
			this.Flag03 = DataObject.getOptionalBool(dataRow["Flag03"]);
			this.Flag04 = DataObject.getOptionalBool(dataRow["Flag04"]);
			this.Flag05 = DataObject.getOptionalBool(dataRow["Flag05"]);
			this.Flag06 = DataObject.getOptionalBool(dataRow["Flag06"]);
			this.FuelCardID = dataRow["FuelCardID"].ToString();
			this.FuelTime = DataObject.getOptionalInt(dataRow["FuelTime"]);
			this.Grade = dataRow["ProductID"].ToString();
			this.GrossQuantity = DataObject.getOptionalDouble(dataRow["GrossQuantity"]);
			this.IdentityGuid = dataRow["TransId"].ToString();
			this.LineItemGuid = dataRow["TransactionLineItemGuid"].ToString();
			this.Model = dataRow["Model"].ToString();
			this.NetQuantity = DataObject.getOptionalDouble(dataRow["NetQuantity"]);
			this.Notes = dataRow["Notes"].ToString();
			this.Number01 = DataObject.getOptionalDouble(dataRow["Number01"]);
			this.Number02 = DataObject.getOptionalDouble(dataRow["Number02"]);
			this.Number03 = DataObject.getOptionalDouble(dataRow["Number03"]);
			this.Number04 = DataObject.getOptionalDouble(dataRow["Number04"]);
			this.Number05 = DataObject.getOptionalDouble(dataRow["Number05"]);
			this.Number06 = DataObject.getOptionalDouble(dataRow["Number06"]);
			this.OperatorID = dataRow["OperatorName"].ToString();
			this.OperatorPersonnelGuid = DataObject.getValue(dataRow["OperatorPersonnelGuid"], Guid.Empty);
			//this.OperatorName = dataRow["OperatorName"].ToString();
			this.ResponseTime = DataObject.getOptionalInt(dataRow["ResponseTime"]);
			this.ShipToID = dataRow["ShipToID"].ToString();
			this.VehicleID = dataRow["VehicleID"].ToString();
			this.Status = dataRow["LookupTransactionStatusIndex"].ToString();
			this.TransId = dataRow["TransId"].ToString();
			this.TransType = dataRow["LookupTransTypeIndex"].ToString();
			this.UserData1 = dataRow["UserData1"].ToString();
			this.UserData2 = dataRow["UserData2"].ToString();
			this.UserData3 = dataRow["UserData3"].ToString();
			this.UserData4 = dataRow["UserData4"].ToString();
			this.UserData5 = dataRow["UserData5"].ToString();
			this.UserData6 = dataRow["UserData6"].ToString();
			this.UserData7 = dataRow["UserData7"].ToString();
			this.UserData8 = dataRow["UserData8"].ToString();
			this.UserData9 = dataRow["UserData9"].ToString();
			this.UserData10 = dataRow["UserData10"].ToString();
			this.UserData11 = dataRow["UserData11"].ToString();
			this.UserData12 = dataRow["UserData12"].ToString();
			this.UserData13 = dataRow["UserData13"].ToString();
			this.UserData14 = dataRow["UserData14"].ToString();
			this.UserData15 = dataRow["UserData15"].ToString();
			this.UserData16 = dataRow["UserData16"].ToString();
			this.UserData17 = dataRow["UserData17"].ToString();
			this.UserData18 = dataRow["UserData18"].ToString();
			this.UserData19 = dataRow["UserData19"].ToString();
			this.UserData20 = dataRow["UserData20"].ToString();
			this.UserData21 = dataRow["UserData21"].ToString();
			this.UserData22 = dataRow["UserData22"].ToString();
			this.UserData23 = dataRow["UserData23"].ToString();
			this.UserData24 = dataRow["UserData24"].ToString();
			this.XREF = dataRow["XREF"].ToString();
			this.FuelAdditiveFlag = DataObject.getOptionalBool(dataRow["FuelAdditiveFlag"]);
			this.SubmittedToAccounting = DataObject.getOptionalBool(dataRow["SubmittedToAccounting"]);
			this.IssuePoint = dataRow["IssuePoint"].ToString();
			this.IssuePointNumber = dataRow["IssuePointNumber"].ToString();
			this.RadioNumber = dataRow["RadioNumber"].ToString();
			this.ControlLogUrl = string.Empty;
			this.Variance = DataObject.getOptionalDouble(dataRow["Variance"]);
			this.Location = DataObject.getString(dataRow["Location"]);
			this.TransactionGuid = dataRow["TransactionGuid"].ToString();
			this.OnHandQuantity = DataObject.getOptionalDouble(dataRow["OnHandQuantity"]);
			this.Site = dataRow["Site"].ToString();

			DateTimeOffset? transactionDate = DataObject.getOptionalDateTimeOffset(dataRow["TransDateTime"]);
			this.TransactionDate = transactionDate == null ? string.Empty : transactionDate.Value.LocalDateTime.ToString(CultureInfo.InvariantCulture);

			this.RequestedTime = this.GetTimeString(DataObject.getOptionalDateTimeOffset(dataRow["RequestedDateTime"]));
			this.DispatchedTime = this.GetTimeString(DataObject.getOptionalDateTimeOffset(dataRow["DispatchedDateTime"]));
			this.FST = this.GetTimeString(DataObject.getOptionalDateTimeOffset(dataRow["FST"]));
			this.TimeEnd = this.GetTimeString(DataObject.getOptionalDateTimeOffset(dataRow["TimeEnd"]));
			this.TimeIn = this.GetTimeString(DataObject.getOptionalDateTimeOffset(dataRow["TimeIn"]));
			this.TimeOut = this.GetTimeString(DataObject.getOptionalDateTimeOffset(dataRow["TimeOut"]));

			this.Cancelled = this.Status == "7";
		}

		/// <summary>
		/// Gets or sets the time pattern.
		/// </summary>
		/// <value>
		/// The time pattern.
		/// </value>
		protected string TimePattern { get; set; }

		/// <summary>
		/// Gets the time string.
		/// </summary>
		/// <param name="dateTimeOffset">The date time offset.</param>
		/// <returns>Time value</returns>
		private string GetTimeString(DateTimeOffset? dateTimeOffset)
		{
			if (dateTimeOffset != null)
			{
				return dateTimeOffset.Value.ToString(this.TimePattern);
			}

			return string.Empty;
		}

		#region Public Properties
		/// <summary>
		/// Gets or sets TransactionGuid.
		/// </summary>
		[DataMember]
		public string TransactionGuid { get; set; }

		/// <summary>
		/// Gets or sets the line item GUID.
		/// </summary>
		[DataMember]
		public string LineItemGuid { get; set; }

		/// <summary>
		/// Gets or sets AliasName.
		/// </summary>
		[DataMember]
		public string ControlLogUrl { get; set; }

		/// <summary>
		/// Gets or sets AliasName.
		/// </summary>
		[DataMember]
		public string AliasName { get; set; }

		/// <summary>
		/// Gets or sets BillToID.
		/// </summary>
		[DataMember]
		public string BillToID { get; set; }

		/// <summary>
		/// Gets or sets CardNumber.
		/// </summary>
		[DataMember]
		public string CardNumber { get; set; }

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		[DataMember]
		public string Color { get; set; }

		/// <summary>
		/// Gets or sets ContactSurname.
		/// </summary>
		[DataMember]
		public string ContactSurname { get; set; }

		/// <summary>
		/// Gets or sets AircraftID.
		/// </summary>
		[DataMember]
		public string AircraftID { get; set; }

		/// <summary>
		/// Gets or sets DispatchedDateTime.
		/// </summary>
		public DateTimeOffset? DispatchedDateTime { get; set; }

		/// <summary>
		/// Gets or sets the dispatched time.
		/// </summary>
		/// <value>
		/// The dispatched time.
		/// </value>
		[DataMember]
		public string DispatchedTime { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the transaction status is Cancelled.
		/// </summary>
		[DataMember]
		public bool Cancelled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether Flag01 is set.
		/// </summary>
		[DataMember]
		public bool? Flag01 { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether Flag02 is set.
		/// </summary>
		[DataMember]
		public bool? Flag02 { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether Flag03 is set.
		/// </summary>
		[DataMember]
		public bool? Flag03 { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether Flag04 is set.
		/// </summary>
		[DataMember]
		public bool? Flag04 { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether Flag05 is set.
		/// </summary>
		[DataMember]
		public bool? Flag05 { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether Flag06 is set.
		/// </summary>
		[DataMember]
		public bool? Flag06 { get; set; }

		/// <summary>
		/// Gets or sets FuelCardID.
		/// </summary>
		[DataMember]
		public string FuelCardID { get; set; }

		/// <summary>
		/// Gets or sets FuelTime.
		/// </summary>
		[DataMember]
		public int? FuelTime { get; set; }

		/// <summary>
		/// Gets or sets FST.
		/// </summary>
		[DataMember]
		public string FST { get; set; }

		/// <summary>
		/// Gets or sets Grade.
		/// </summary>
		[DataMember]
		public string Grade { get; set; }

		/// <summary>
		/// Gets or sets Gross Quantity.
		/// </summary>
		[DataMember]
		public double? GrossQuantity { get; set; }

		/// <summary>
		/// Gets or sets TransId.
		/// </summary>
		[DataMember]
		public string IdentityGuid { get; set; }

		/// <summary>
		/// Gets or sets Model.
		/// </summary>
		[DataMember]
		public string Model { get; set; }

		/// <summary>
		/// Gets or sets Net Quantity.
		/// </summary>
		[DataMember]
		public double? NetQuantity { get; set; }

		/// <summary>
		/// Gets or sets On Hand Quantity.
		/// </summary>
		[DataMember]
		public double? OnHandQuantity { get; set; }

		/// <summary>
		/// Gets or sets Site.
		/// </summary>
		[DataMember]
		public string Site { get; set; }

		/// <summary>
		/// Gets or sets Notes.
		/// </summary>
		[DataMember]
		public string Notes { get; set; }

		/// <summary>
		/// Gets or sets Number01.
		/// </summary>
		[DataMember]
		public double? Number01 { get; set; }

		/// <summary>
		/// Gets or sets Number01.
		/// </summary>
		[DataMember]
		public double? Number02 { get; set; }

		/// <summary>
		/// Gets or sets Number03.
		/// </summary>
		[DataMember]
		public double? Number03 { get; set; }

		/// <summary>
		/// Gets or sets Number04.
		/// </summary>
		[DataMember]
		public double? Number04 { get; set; }

		/// <summary>
		/// Gets or sets Number05.
		/// </summary>
		[DataMember]
		public double? Number05 { get; set; }

		/// <summary>
		/// Gets or sets Number06.
		/// </summary>
		[DataMember]
		public double? Number06 { get; set; }

		/// <summary>
		/// Gets or sets OperatorID.
		/// </summary>
		[DataMember]
		public string OperatorID { get; set; }

		/// <summary>
		/// Gets or sets Operator Name.
		/// </summary>
		[DataMember]
		public string OperatorName { get; set; }

		/// <summary>
		/// Gets or sets the operator personnel GUID.
		/// </summary>
		/// <value>
		/// The operator personnel GUID.
		/// </value>
		[DataMember]
		public Guid OperatorPersonnelGuid { get; set; }

		/// <summary>
		/// Gets or sets the requested time.
		/// </summary>
		[DataMember]
		public string RequestedTime { get; set; }

		/// <summary>
		/// Gets or sets ResponseTime.
		/// </summary>
		[DataMember]
		public int? ResponseTime { get; set; }

		/// <summary>
		/// Gets or sets ShipToID.
		/// </summary>
		[DataMember]
		public string ShipToID { get; set; }

		/// <summary>
		/// Gets or sets VehicleID.
		/// </summary>
		[DataMember]
		public string VehicleID { get; set; }

		/// <summary>
		/// Gets or sets the transaction status.
		/// </summary>
		[DataMember]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets TimeEnd.
		/// </summary>
		[DataMember]
		public string TimeEnd { get; set; }

		/// <summary>
		/// Gets or sets TransId.
		/// </summary>
		[DataMember]
		public string TransId { get; set; }

		/// <summary>
		/// Gets or sets TimeIn.
		/// </summary>
		[DataMember]
		public string TimeIn { get; set; }

		/// <summary>
		/// Gets or sets TimeOut.
		/// </summary>
		[DataMember]
		public string TimeOut { get; set; }

		/// <summary>
		/// Gets or sets transaction date.
		/// </summary>
		[DataMember]
		public string TransactionDate { get; set; }

		/// <summary>
		/// Gets or sets TransType.
		/// </summary>
		[DataMember]
		public string TransType { get; set; }

		/// <summary>
		/// Gets or sets UserData1.
		/// </summary>
		[DataMember]
		public string UserData1 { get; set; }

		/// <summary>
		/// Gets or sets UserData2.
		/// </summary>
		[DataMember]
		public string UserData2 { get; set; }

		/// <summary>
		/// Gets or sets UserData3.
		/// </summary>
		[DataMember]
		public string UserData3 { get; set; }

		/// <summary>
		/// Gets or sets UserData4.
		/// </summary>
		[DataMember]
		public string UserData4 { get; set; }

		/// <summary>
		/// Gets or sets UserData5.
		/// </summary>
		[DataMember]
		public string UserData5 { get; set; }

		/// <summary>
		/// Gets or sets UserData6.
		/// </summary>
		[DataMember]
		public string UserData6 { get; set; }

		/// <summary>
		/// Gets or sets UserData7.
		/// </summary>
		[DataMember]
		public string UserData7 { get; set; }

		/// <summary>
		/// Gets or sets UserData8.
		/// </summary>
		[DataMember]
		public string UserData8 { get; set; }

		/// <summary>
		/// Gets or sets UserData9.
		/// </summary>
		[DataMember]
		public string UserData9 { get; set; }

		/// <summary>
		/// Gets or sets UserData10.
		/// </summary>
		[DataMember]
		public string UserData10 { get; set; }

		/// <summary>
		/// Gets or sets UserData11.
		/// </summary>
		[DataMember]
		public string UserData11 { get; set; }

		/// <summary>
		/// Gets or sets UserData12
		/// </summary>
		[DataMember]
		public string UserData12 { get; set; }

		/// <summary>
		/// Gets or sets UserData13
		/// </summary>
		[DataMember]
		public string UserData13 { get; set; }

		/// <summary>
		/// Gets or sets UserData14.
		/// </summary>
		[DataMember]
		public string UserData14 { get; set; }

		/// <summary>
		/// Gets or sets UserData15.
		/// </summary>
		[DataMember]
		public string UserData15 { get; set; }

		/// <summary>
		/// Gets or sets UserData16.
		/// </summary>
		[DataMember]
		public string UserData16 { get; set; }

		/// <summary>
		/// Gets or sets UserData17.
		/// </summary>
		[DataMember]
		public string UserData17 { get; set; }

		/// <summary>
		/// Gets or sets UserData18.
		/// </summary>
		[DataMember]
		public string UserData18 { get; set; }

		/// <summary>
		/// Gets or sets UserData19.
		/// </summary>
		[DataMember]
		public string UserData19 { get; set; }

		/// <summary>
		/// Gets or sets UserData20.
		/// </summary>
		[DataMember]
		public string UserData20 { get; set; }

		/// <summary>
		/// Gets or sets UserData21.
		/// </summary>
		[DataMember]
		public string UserData21 { get; set; }

		/// <summary>
		/// Gets or sets UserData22.
		/// </summary>
		[DataMember]
		public string UserData22 { get; set; }

		/// <summary>
		/// Gets or sets UserData23.
		/// </summary>
		[DataMember]
		public string UserData23 { get; set; }

		/// <summary>
		/// Gets or sets UserData24.
		/// </summary>
		[DataMember]
		public string UserData24 { get; set; }

		/// <summary>
		/// Gets or sets XREF.
		/// </summary>
		[DataMember]
		public string XREF { get; set; }

		/// <summary>
		/// Gets or sets FuelAdditiveFlag flag
		/// </summary>
		[DataMember]
		public bool? FuelAdditiveFlag { get; set; }

		/// <summary>
		/// Gets or sets SubmittedToAccounting flag
		/// </summary>
		[DataMember]
		public bool? SubmittedToAccounting { get; set; }

		/// <summary>
		/// Gets or sets IssuePoint
		/// </summary>
		[DataMember]
		public string IssuePoint { get; set; }

		/// <summary>
		/// Gets or sets IssuePointNumber
		/// </summary>
		[DataMember]
		public string IssuePointNumber { get; set; }

		/// <summary>
		/// Gets or sets RadioNumber
		/// </summary>
		[DataMember]
		public string RadioNumber { get; set; }

		/// <summary>
		/// Gets or sets the selection back color.
		/// </summary>
		[DataMember]
		public string SelectionBackColor { get; set; }

		/// <summary>
		/// Gets or sets the fore color.
		/// </summary>
		[DataMember]
		public string ForeColor { get; set; }

		/// <summary>
		/// Gets or sets the selection fore color.
		/// </summary>
		[DataMember]
		public string SelectionForeColor { get; set; }

		/// <summary>
		/// Gets or sets the Variance.
		/// </summary>
		[DataMember]
		public double? Variance { get; set; }

		/// <summary>
		/// Gets or sets the Location
		/// </summary>
		[DataMember]
		public string Location { get; set; }

		#endregion
	}
}
