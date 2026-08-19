using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class RegradeLineItemDO : LineItemDO
	{
		#region Attributes
		[DataMember]
		protected string toProductCode;
		[DataMember]
		protected string toProduct;
		[DataMember]
		protected string toProductType;
		[DataMember]
		protected Guid toProductGuid;
		[DataMember]
		protected string toStorageLocation;
		[DataMember]
		protected Guid toStorageLocationTankGuid;
		#endregion Attributes

		#region Properties
		/// <summary>
		/// This property gets and sets the Product Code data member.
		/// </summary>
		public string ToProductCode
		{
			get { return toProductCode; }
			set { toProductCode = value; }
		}

		/// <summary>
		/// This property gets and sets the Product ID data member.
		/// </summary>
		public string ToProduct
		{
			get { return toProduct; }
			set { toProduct = value; }
		}

		/// <summary>
		/// This property gets and sets the Product Type data member.
		/// </summary>
		public string ToProductType
		{
			get { return toProductType; }
			set { toProductType = value; }
		}

		/// <summary>
		/// This property gets and sets the Product Index data member.
		/// </summary>
		public Guid ToProductGuid
		{
			get { return toProductGuid; }
			set { toProductGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the Storage Location data member.
		/// </summary>
		public string ToStorageLocation
		{
			get { return this.toStorageLocation; }
			set { this.toStorageLocation = value; }
		}

		/// <summary>
		/// This property gets and sets the Storage Location Guid data member.
		/// </summary>
		public Guid ToStorageLocationTankGuid
		{
			get { return this.toStorageLocationTankGuid; }
			set { this.toStorageLocationTankGuid = value; }
		}

		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Regrade Line Item data object.
		/// </summary>
		public RegradeLineItemDO()
		{
		}
		#endregion
	}
}
