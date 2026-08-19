// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextUtil.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Class for checking transaction status
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System.Transactions;

	/// <summary>
	/// Class for checking transaction status
	/// </summary>
	internal class ContextUtil
	{
		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether this instance is in transaction.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is in transaction; otherwise, <c>false</c>.
		/// </value>
		public static bool IsInTransaction
		{
			get
			{
				bool transactionStatus = false;

				if (Transaction.Current != null)
				{
					var transactionInformation = Transaction.Current.TransactionInformation;

					if (transactionInformation.Status == TransactionStatus.Active)
					{
						transactionStatus = true;
					}
				}

				return transactionStatus;
			}
		}

		#endregion
	}
}