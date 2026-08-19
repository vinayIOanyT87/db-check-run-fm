 #pragma warning disable 1587
/// <summary>
/// File name:	CloseoutSR.cs
/// Purpose:	Closeout Service Request
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		--------------------	----------------------------------
///		2009-02-18		W.Gray					7.4.6.1 - Revised to process CLOSEOUT_ALL_COMPLETE (CSI 1543)
/// </summary>
#pragma warning restore 1587
    namespace FMBusinessObjects.ServiceRequests
{
    using System;
    using System.Runtime.Serialization;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    [Serializable]
    [DataContract]
	public class CloseoutSR : AccountingServiceRequest
	{
		#region Public data members

        public enum CloseoutType
        {
            CALCULATE,
            CREATE,
            CLOSEOUT_ALL_COMPLETE,
            GET_TO_EXPORT,
            SAVE_TO_IMPORT,
            CALCULATE_FOR_IMPORT,
            CLOSEOUT_ALL_PRODUCTS,
            CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP1,
            CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP2,
            CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP3
        };
		#endregion

		#region Protected data members
        [DataMember]
        protected ProductIrdoCollectionClass allProductsIrdoCollection;
		[DataMember]
		protected Guid closeoutInventoryGuid;
		[DataMember]
		protected CloseoutType closeoutType;
		[DataMember]
		protected DateTime inventoryDate;
		[DataMember]
		protected DateTime? fromDate;
		[DataMember]
		protected DateTime? toDate;
		[DataMember]
		protected string managerCode;
		[DataMember]
		protected string managerName;
		[DataMember]
		protected Guid managerCompanyGuid;
		[DataMember]
		protected string productCode;
		[DataMember]
		protected string productName;
		[DataMember]
		protected Guid productGuid;
		[DataMember]
		protected CloseoutDO closeoutRecord;

        [DataMember]
        private bool force;

        [DataMember]
        private bool convertUnits;

        [DataMember]
        private double tolerance;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Closeout Service Request class.
		/// </summary>
		public CloseoutSR()
		{
		    this.force = false;
		    this.convertUnits = true;
		    this.tolerance = 0.0;
		}
		#endregion

		#region Properties

        public ProductIrdoCollectionClass AllProductsIrdoCollection
        {
            get { return this.allProductsIrdoCollection; }
            set {
                this.allProductsIrdoCollection = value; }
        }

		public Guid CloseoutInventoryGuid
		{
			get { return this.closeoutInventoryGuid; }
			set { this.closeoutInventoryGuid = value; }
		}

		public CloseoutType CloseoutCommand
		{
			get { return this.closeoutType; }
			set { this.closeoutType = value; }
		}

		public DateTime InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = TimeConverter.ToDate(value).Date; }
		}

		public DateTime? FromDate
		{
			get { return this.fromDate; }
			set { this.fromDate = value; }
		}

		public DateTime? ToDate
		{
			get { return this.toDate; }
			set { this.toDate = value; }
		}

		public string ManagerCode
		{
			get { return this.managerCode; }
			set { this.managerCode = value; }
		}

		public string ManagerName
		{
			get { return this.managerName; }
			set { this.managerName = value; }
		}

		public Guid ManagerCompanyGuid
		{
			get { return this.managerCompanyGuid; }
			set { this.managerCompanyGuid = value; }
		}

		public string ProductCode
		{
			get { return this.productCode; }
			set { this.productCode = value; }
		}

		public string ProductName
		{
			get { return this.productName; }
			set { this.productName = value; }
		}

		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		public CloseoutDO Closeout
		{
			get { return this.closeoutRecord; }
			set { this.closeoutRecord = value; }
		}

        public bool Force
        {
            get
            {
                return this.force;
            }

            set
            {
                this.force = value;
            }
        }

        public bool ConvertUnits
        {
            get
            {
                return this.convertUnits;
            }

            set
            {
                this.convertUnits = value;
            }
        }

        public double Tolerance
        {
            get
            {
                return this.tolerance;
            }

            set
            {
                this.tolerance = value;
            }
        }
        #endregion
    }
}
