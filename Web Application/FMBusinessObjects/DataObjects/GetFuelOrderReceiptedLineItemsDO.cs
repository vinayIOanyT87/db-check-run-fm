// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFuelOrderReceiptedLineItemsDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GetFuelOrderReceiptedLineItemsDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// The get fuel order receipted line items data object.
	/// </summary>
	[DataContract]
	[Serializable]
	public class GetFuelOrderReceiptedLineItemsDO : DataObject
	{
		#region Attributes
		/// <summary>
		/// The GUID list.
		/// </summary>
		[DataMember]
		private List<Guid> guidList;
		#endregion // Attributes

		#region Construction
		/// <summary>
		/// Initializes a new instance of the <see cref="GetFuelOrderReceiptedLineItemsDO"/> class.
		/// </summary>
		public GetFuelOrderReceiptedLineItemsDO()
		{
			this.guidList = new List<Guid>();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the GUID list.
		/// </summary>
		public List<Guid> GuidList
		{
			get { return this.guidList; }
			set { this.guidList = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// The add line item GUID.
		/// </summary>
		/// <param name="newGuid">
		/// The new GUID.
		/// </param>
		public void AddLineItemGuid(Guid newGuid)
		{
			this.guidList.Add(newGuid);
		}

		/// <summary>
		/// The get result.
		/// </summary>
		/// <returns>
		/// The <see cref="List"/>.
		/// </returns>
		public List<Guid> GetResult()
		{
			return this.guidList;
		}
		#endregion

		#region Abstracts
		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion
	}
}
