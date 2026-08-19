// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkPaymentInvoiceMappings.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IBulkPaymentInvoiceMappings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The BulkPaymentInvoiceMappings interface.
	/// </summary>
	[ServiceContract]
	public interface IBulkPaymentInvoiceMappings
	{
		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="invoiceMapping">
		/// The invoice mapping.
		/// </param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, BulkPaymentInvoiceMappingClass invoiceMapping );

		/// <summary>
		/// The update.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="invoiceMapping">
		/// The invoice mapping.
		/// </param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Update ( SecurityClass security, BulkPaymentInvoiceMappingClass invoiceMapping );

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="invoiceMapping">
		/// The invoice mapping.
		/// </param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Remove ( SecurityClass security, BulkPaymentInvoiceMappingClass invoiceMapping );

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentInvoiceMappingCollectionClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentInvoiceMappingCollectionClass Enumerate ( SecurityClass security );

		/// <summary>
		/// The enumerate by invoice trans ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transId">
		/// The trans ID.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentInvoiceMappingClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentInvoiceMappingClass EnumerateByInvoiceTransID ( SecurityClass security, string transId );

		/// <summary>
		/// The enumerate by bulk payment ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentInvoiceMappingCollectionClass"/>.
		/// </returns>
		[OperationContract]
		BulkPaymentInvoiceMappingCollectionClass EnumerateByBulkPaymentID ( SecurityClass security, Guid id );
	}
}
