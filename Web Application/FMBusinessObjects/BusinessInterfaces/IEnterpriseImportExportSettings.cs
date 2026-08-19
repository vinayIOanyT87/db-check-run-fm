// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnterpriseImportExportSettings.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IEnterpriseImportExportSettings
	{
		#region Public Methods and Operators

		[OperationContract]
		DataTable SelectAll(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Update(SecurityClass security, string settingKey, string settingValue);

		#endregion
	}
}