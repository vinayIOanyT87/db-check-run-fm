/// <summary>
/// File name:	FinanceSR.cs
/// Purpose:	The purpose of the finance service request class
///				is to retrieve the average unit and standing offer price.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				   
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///   Date:			By:						Reason:
///   ----------	--------------------	----------------------------------
///   2009-03-03  Richard Panachida    Updated to contain a delivery location and supplier
///                                    for calculating the standing offer. Defect 1696.
///		
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class FinanceSR : AccountingServiceRequest
	{
		#region Public data members
		public enum SUB_REQUEST { AVERAGE_UNIT_PRICE, STANDING_OFFER_PRICE, NONE };
		#endregion

		#region Private data members
		[DataMember]
		private Guid supplierCompanyGuid;
		[DataMember]
		private Guid productGuid;
		[DataMember]
		private Guid siteGuid;
		[DataMember]
		private string deliveryLocation;
		[DataMember]
		private TransactionTypes transType;
		[DataMember]
		private DateTimeOffset startDate;
		[DataMember]
		private SUB_REQUEST subRequest;
		[DataMember]
		private double? quantity;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Finance service request class.
		/// </summary>
		public FinanceSR()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the supplier guid;
		/// </summary>
		public Guid SupplierCompanyGuid
		{
			get { return this.supplierCompanyGuid; }
			set { this.supplierCompanyGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the product guid;
		/// </summary>
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the site guid;
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the start date;
		/// </summary>
		public DateTimeOffset StartDate
		{
			get { return this.startDate; }
			set { this.startDate = value; }
		}

		/// <summary>
		/// This property sets and gets the sub request (average unit price or
		/// price list (aka standing offer) price).
		/// </summary>
		public FinanceSR.SUB_REQUEST SubRequest
		{
			get { return this.subRequest; }
			set { this.subRequest = value; }
		}

		/// <summary>
		/// This property sets and gets the transaction type.
		/// </summary>
		public TransactionTypes TransactionType
		{
			get { return this.transType; }
			set { this.transType = value; }
		}

		/// <summary>
		/// This property sets and gets the delivery location.
		/// </summary>
		public string DeliveryLocation
		{
			get { return this.deliveryLocation; }
			set { this.deliveryLocation = value; }
		}

		/// <summary>
		/// This property sets and gets the price list (aka standing offer) quantity. Can be null.
		/// </summary>
		public double? Quantity
		{
			get { return this.quantity; }
			set { this.quantity = value; }
		}

		#endregion

		#region Private methods
		/// <summary>
		/// This method iniitalizes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.supplierCompanyGuid = Guid.Empty;
			this.productGuid = Guid.Empty;
			this.siteGuid = Guid.Empty;
			this.deliveryLocation = "";
			this.startDate = DateTimeOffset.Now;
			this.subRequest = FinanceSR.SUB_REQUEST.NONE;
			this.transType = TransactionTypes.T_Maximum;
			this.quantity = null;
		}
		#endregion
	}
}
