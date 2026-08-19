// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveTransmitTranListSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SaveTransmitTranListSR type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
    using System;
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Defines the SaveTransmitTranListSR type.
	/// </summary>
    [Serializable]
    [DataContract]
	public class SaveTransmitTranListSR : AccountingServiceRequest
	{
		#region Attributes
		/// <summary>
		/// Internal storage of the transmit transactions data object.
		/// </summary>
		[DataMember]
		private TransmitTranListDO dataobject;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="SaveTransmitTranListSR"/> class. 
		/// This is the default constructor for the save transmit transaction list 
		/// service request class.
		/// </summary>
		public SaveTransmitTranListSR()
		{
			this.dataobject = null;
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the transmit transactions object.
		/// </summary>
		/// <value>
		/// The transmit transactions object.
		/// </value>
		public TransmitTranListDO Transactions
		{
			get { return this.dataobject; }
			set { this.dataobject = value; }
		}
		#endregion
	}
}
