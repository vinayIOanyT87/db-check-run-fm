// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataDictionariesClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDataDictionariesClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for access data dictionary translations
	/// </summary>
	[ServiceContract]
	public interface IDataDictionariesClass
	{
		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="dataDictionary">The data dictionary.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, DataDictionaryClass dataDictionary);

		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A collection of data dictionary keys.</returns>
		[OperationContract]
		DataDictionaryCollectionClass Enumerate(SecurityClass security);

		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>The cached collection of data dictionary keys.</returns>
		[OperationContract]
		DataDictionaryCollectionClass EnumerateCached(Guid siteGuid);

		/// <summary>
		/// Enumerates the by site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A collection of data dictionary keys.</returns>
		[OperationContract]
		DataDictionaryCollectionClass EnumerateBySite(SecurityClass security);

        /// <summary>
        /// Enumerates the by site.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns>A collection of data dictionary keys.</returns>
        [OperationContract]
        List<DataDictionaryClass> EnumerateBySite2(SecurityClass security);

        /// <summary>
        /// Enumerates from updated date time.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="updatedDateTime">The updated date time.</param>
        /// <returns>A collection of data dictionary keys.</returns>
        [OperationContract]
		DataDictionaryCollectionClass EnumerateFromUpdatedDateTime(SecurityClass security, DateTimeOffset updatedDateTime);

		/// <summary>
		/// Returns the properly translated text for the specified key at the specified site or the default translation for the key, if no translation exists.
		/// </summary>
		/// <param name="siteGuid">
		/// The site to use for translation context.
		/// </param>
		/// <param name="key">
		/// The key to translate.
		/// </param>
		/// <returns>
		/// Properly translated text. 
		/// </returns>
		[OperationContract]
		string Get(Guid siteGuid, string key);

		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="dataDictionary">The data dictionary.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, DataDictionaryClass dataDictionary);

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="key">The key.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, string key);

        /// <summary>
        /// This interface will start the process of importing the data dictionary items. Some items will be
        /// added, modified, or deleted.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="addList">The add list.</param>
        /// <param name="modList">The modify list.</param>
        /// <param name="delList">The delete list.</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void ImportData(SecurityClass security, List<DataDictionaryClass> addList, List<DataDictionaryClass> modList, List<DataDictionaryClass> delList);

        /// <summary>
        /// Resets the specified data dictionary cache item.
        /// </summary>
        /// <param name="siteGuid">Guid identifying the site for which the data dictionary cache should be reset.</param>
        [OperationContract]
		void ResetDataDictionaryCache(Guid siteGuid);

		/// <summary>
		/// Populates the values for keys in the provided keyTable.
		/// </summary>
		/// <param name="siteGuid">The site for which to draw translations.</param>
		/// <param name="keyTable">A table containing Keys and Values fields for translation.</param>
		/// <returns>The data table with translated values.</returns>
		[OperationContract]
		Dictionary<string,string> TranslateKeyPairTable(Guid siteGuid, Dictionary<string,string> keyTable);

		#endregion
	}
}