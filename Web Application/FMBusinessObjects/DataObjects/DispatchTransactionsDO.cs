// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchTransactionsDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchTransactionsDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Diagnostics;
	using System.Runtime.Serialization;

	/// <summary>
	/// Result data object for dispatch transaction enumerations.
	/// </summary>
	[DataContract]
   [Serializable]
	[DebuggerDisplay("DispatchTransacitonDO Row Count = {Transactions.Tables[0].Rows.Count}")]
	public class DispatchTransactionsDO : DataObject
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets Transactions dataset.
		/// </summary>
		[DataMember]
		public DataSet Transactions { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Mandatory but unused override.
		/// </summary>
		/// <returns>
		/// Nothing at all.
		/// </returns>
		/// <exception cref="NotImplementedException">
		/// Always throws this exeption
		/// </exception>
		public override string getDeleteCommand()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Mandatory but unused override.
		/// </summary>
		/// <returns>
		/// Nothing at all.
		/// </returns>
		/// <exception cref="NotImplementedException">
		/// Always throws this exeption
		/// </exception>
		public override string getInsertCommand()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Mandatory but unused override.
		/// </summary>
		/// <returns>
		/// Nothing at all.
		/// </returns>
		/// <exception cref="NotImplementedException">
		/// Always throws this exeption
		/// </exception>
		public override string getSelectCommand()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Mandatory but unused override.
		/// </summary>
		/// <returns>
		/// Nothing at all.
		/// </returns>
		/// <exception cref="NotImplementedException">
		/// Always throws this exeption.
		/// </exception>
		public override string getUpdateCommand()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}