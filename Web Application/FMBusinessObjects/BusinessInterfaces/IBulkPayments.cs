// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkPayments.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IBulkPayments type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The BulkPayments interface.
	/// </summary>
	[ServiceContract]
	public interface IBulkPayments
	{
		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="payment">
		/// The payment.
		/// </param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, BulkPaymentClass payment );

		/// <summary>
		/// The update.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="payment">
		/// The payment.
		/// </param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Update ( SecurityClass security, BulkPaymentClass payment );

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="payment">
		/// The payment.
		/// </param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Remove ( SecurityClass security, BulkPaymentClass payment );

		/// <summary>
		/// The get by id.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentClass GetByID ( SecurityClass inSecurity, Guid id );

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentCollectionClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentCollectionClass Enumerate ( SecurityClass inSecurity );

		/// <summary>
		/// The enumerate by filter.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="inFilter">
		/// The in filter.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentCollectionClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentCollectionClass EnumerateByFilter ( SecurityClass inSecurity, BulkPaymentFilter inFilter );

		/// <summary>
		/// The enumerate invoice mapping.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="inCollection">
		/// The in collection.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentCollectionClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentCollectionClass EnumerateInvoiceMapping ( SecurityClass inSecurity, BulkPaymentCollectionClass inCollection );

		/// <summary>
		/// The enumerate invoice mapping by payment.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="inPayment">
		/// The in payment.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentClass EnumerateInvoiceMappingByPayment ( SecurityClass inSecurity, BulkPaymentClass inPayment );
	}
}
