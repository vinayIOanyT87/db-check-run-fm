// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationSettings.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface definition for the ConfigurationSettings service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface definition for the ConfigurationSettings service class.
	/// </summary>
	[ServiceContract]
	public interface IConfigurationSettings
	{
		#region Public Methods and Operators

		/// <summary>
		/// Enumerates the FuelsManager configuration settings in the database.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>An object containing a collection of configuration items.</returns>
		[OperationContract]
		ConfigurationSettingDOCollectionClass Enumerate(SecurityClass security);

		/// <summary>
		/// Gets a configuration setting by GUID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="guid">The GUID identifier of the desired configuration setting.</param>
		/// <returns>The configuration setting specified by the guid.</returns>
		[OperationContract]
		ConfigurationSettingDOClass GetByGuid(SecurityClass security, string guid);

		/// <summary>
		/// Gets a configuration setting by the key value.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="key">The key of the configuration setting.</param>
		/// <returns>The configuration setting specified by the key.</returns>
		[OperationContract]
		ConfigurationSettingDOClass GetByKey(SecurityClass security, string key);

		/// <summary>
		/// Gets the configuration setting value by key.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="key">The key to lookup.</param>
		/// <returns>The value of the specified configuration setting.</returns>
		[OperationContract]
		string GetKeyValueByKey(SecurityClass security, string key);

		/// <summary>
		/// Modifies the specified configuration setting.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="key">The key of the configuration setting.</param>
		/// <param name="keyValue">The new value.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, string key, string keyValue);

	    /// <summary>
	    /// Modifies the specified configuration setting and saves the value encrypted.
	    /// </summary>
	    /// <param name="security">The security object.</param>
	    /// <param name="key">The key of the configuration setting.</param>
	    /// <param name="keyValue">The new value.</param>
	    /// <param name="keyType">the type of parameter</param>
	    [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void ModifyWithEncryption(SecurityClass security, string key, string keyValue, string keyType);

        /// <summary>
        /// Updates a specific Configuration Setting called IsEnterprise to match the installed HardwareKey
        /// </summary>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateIsEnterpriseSetting();

        #endregion Public Methods and Operators
    }
}