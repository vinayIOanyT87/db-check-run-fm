// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionOrigin.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for TransactionOrigin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Enumeration of values for TransactionOrigin.
	/// </summary>
	public enum TransactionOrigin
	{
		/// <summary>
		/// None, or unknown origin
		/// </summary>
		None = 0,

		/// <summary>
		/// Transaction originated in accounting. (1)
		/// </summary>
		Accounting,

		/// <summary>
		/// Transaction originated in terminal automation service. (2)
		/// </summary>
		TerminalAutomationService,

		/// <summary>
		/// Transaction originated in dispatch. (3)
		/// </summary>
		Dispatch,

		/// <summary>
		/// Transaction originated in the ADC Upload interface. (4)
		/// </summary>
		ADCUploadInterface,

		/// <summary>
		/// Transaction originated in the Enterprise Upload Transaction. (5)
		/// </summary>
		EnterpriseUploadTransaction,

		/// <summary>
		/// Transaction originated in the TransactionImportProcessorV6. (6)
		/// </summary>
		TransactionImportProcessorV6,

		/// <summary>
		/// Transaction originated in the TransactionImportProcessorV6 and has been updated through TransactionDetail. (7)
		/// </summary>
		TransactionImportProcessorV6Update,

		/// <summary>
		/// Transaction originated from Service Request Messaging, 
		/// for example a message sent by Delta to SRM which resulted in a FuelsManager transaction record
		/// being created with flight information (8)
		/// </summary>
		ServiceRequestMessaging,

	    /// <summary>
        /// Transactions originally from Dispatch, but completed by ADC (9)
        /// </summary>
        FlightlineADCFromDispatch,

        /// <summary>
        /// Transactions manually created by ADC, but want to behave like Dispatch transactions (10)
        /// </summary>
        FlightlineADCForDispatch,

        /// <summary>
        /// Transactions created manually by ADC, but not visible to Dispatch (11)
        /// </summary>
        FlightlineADCStandard,

		/// <summary>
		/// Transactions created at the base level site. (12)
		/// </summary>
		BaseLevelTransaction,

		/// <summary>
		/// Transactions created at the enterprise level site. (13)
		/// </summary>
		EnterpriseLevelTransaction,

		/// <summary>
		/// Transactions created on the handheld and uploaded to a base level site. (14)
		/// </summary>
		AdcUploadedAtBaseLevel,

		/// <summary>
		/// Transactions created on the handheld and uploaded to the enterprise level. (15)
		/// </summary>
		AdcUploadedAtEnterpriseLevel,

		/// <summary>
		/// (16)
		/// </summary>
		DispatchEnterprise
    }

    public static class TransactionOriginExtensions
    {
        public static bool IsPassthrough(this TransactionOrigin value)
        {
			return (value == TransactionOrigin.EnterpriseUploadTransaction
                    || value == TransactionOrigin.TransactionImportProcessorV6
                    || value == TransactionOrigin.ADCUploadInterface
                    || value == TransactionOrigin.FlightlineADCForDispatch
                    || value == TransactionOrigin.FlightlineADCFromDispatch
                    || value == TransactionOrigin.FlightlineADCStandard);
        }

        public static bool IsVisibleToDispatch(this TransactionOrigin value)
        {
            return (value == TransactionOrigin.Dispatch
                    || value == TransactionOrigin.FlightlineADCForDispatch
                    || value == TransactionOrigin.FlightlineADCFromDispatch
					|| value == TransactionOrigin.DispatchEnterprise);
        }

        public static bool IsFlightlineOrigin(this TransactionOrigin value)
        {
            return (value == TransactionOrigin.FlightlineADCForDispatch
                    || value == TransactionOrigin.FlightlineADCFromDispatch
                    || value == TransactionOrigin.FlightlineADCStandard);
        }

        /// <summary>
        /// Provides comma-separated list of TransactionOrigin values that are consdered to
        /// be "visible" to dispatch. Useful for creating "IN" clauses in SQL statements.
        /// </summary>
        /// <returns></returns>
        public static string GetDispatchOriginList()
        {
            return string.Format(
                "{0}, {1}, {2},{3}",
                (int)TransactionOrigin.Dispatch,
                (int)TransactionOrigin.FlightlineADCForDispatch,
                (int)TransactionOrigin.FlightlineADCFromDispatch,
				(int)TransactionOrigin.DispatchEnterprise);
        }
    }

}
