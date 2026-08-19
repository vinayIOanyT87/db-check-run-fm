// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISites.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Interface definition for the sites service class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Interface definition for the sites service class
	/// </summary>
	[ServiceContract]
	public interface ISites
	{
		/// <summary>
		/// Adds the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="site">The site.</param>
		/// <param name="userID">The user ID.</param>
		/// <param name="password">The password.</param>
		/// <returns>The identity GUID of the newly created site.</returns>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, SiteClass site, string userID, string password );

		[OperationContract]
		bool CheckCurrentPassword ( UserClass user, string passwordText );

		[OperationContract]
		SiteClass GetByMemberAndProcessVariables(SecurityClass security, Guid identityGuid, bool getMemberSites, bool getSchedulesAndProcessVariables);

		[OperationContract]
		SiteClass Get ( SecurityClass security, Guid identityGuid, bool getMemberSites, bool getSchedulesAndProcessVariables, bool bGetAssociatedAliases );

		/// <summary>
		/// This method will get the site class without member sites,
		/// schedules, process variables, and associated aliases.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The site's identity GUID.</param>
		/// <returns>Returns the site class object.</returns>
		[OperationContract]
		SiteClass GetBasic(SecurityClass security, Guid identityGuid);

		[OperationContract]
		SiteClass GetUsingGuid(SecurityClass security, Guid identityGuid);

		[OperationContract]
		SiteClass GetByID ( SecurityClass security, string siteID, bool skipReset );

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string siteID);

		[OperationContract]
		string GetReportDirectory(SecurityClass security, string reportPath);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		string GetNextDocumentNumber ( SecurityClass security, DOCUMENT_TYPE type, Guid siteGuid );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<string> GetNextDocumentNumbers(SecurityClass security, List<DOCUMENT_TYPE> documentTypes, Guid siteGuid, int numberDesired);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		string GetNextInvoiceNumber ( SecurityClass security );

		[OperationContract]
		SecurityClass GetSecurity ( string token );

		[OperationContract]
		SecurityClass GetSecurityWithoutSessionRenewal(string token);

		[OperationContract]
		string GetIDNoRefresh ( SecurityClass security, Guid siteGuid );

		[OperationContract]
		int GetSiteCount(SecurityClass security);

		[OperationContract]
		int GetSiteCountByServiceLogin(SecurityClass security, string serviceLogin);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void LogoutToken ( string token );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, DATA_TYPE type, SiteClass site, bool updateDocumentNumbers );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifySecurity ( string token, SecurityClass security );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		SecurityLoginResponse Login2(SecurityLoginRequest sr);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		string Login(out bool changePassword, out int daysUntilExpiration, out SecurityClass security, SecurityLoginRequest sr);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Logout ( SecurityClass security );

		[OperationContract]
		void RefreshTransactionSecurityRightsCache ( ref SecurityClass security );

		/// <summary>
		/// Enumerates the sites in the system and returns them as a collection.
		/// </summary>
		/// <param name="security">A valid FuelsManager SecurityClass object.</param>
		/// <returns>SiteCollectionClass containing a list of SiteClass objects</returns>
		[OperationContract]
		SiteCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		SiteCollectionClass EnumerateByServiceLogin ( SecurityClass security, string serviceLogin );

		[OperationContract]
		SiteCollectionClass EnumerateSitesInfo ( SecurityClass security );

		[OperationContract]
		SiteSelectList EnumerateForSiteSelect( SecurityClass security, Guid parentSiteGuid );

		[OperationContract]
		SiteCollectionClass EnumerateBySiteGroup ( SecurityClass security, bool siteGroup );

		[OperationContract]
		SiteCollectionClass EnumerateByParentSite(SecurityClass security, Guid siteGuid);

		[OperationContract]
		SiteCollectionClass EnumerateByParentSiteCurrentUserAssigned(SecurityClass security, Guid siteGuid);

		[OperationContract]
		SiteCollectionClass EnumerateSitesByAssignedUser(SecurityClass security, Guid siteGuid, Guid userGuid);

		[OperationContract]
		SiteCollectionClass EnumerateByChildSite(SecurityClass security, Guid siteGuid);

		[OperationContract]
		SiteCollectionClass EnumerateByCandidateChildrenSites(SecurityClass security, Guid siteGuid);

		[OperationContract]
		SiteCollectionClass EnumerateByChildSiteForUser(SecurityClass security, Guid siteGuid);

		[OperationContract]
		SiteSyncList EnumerateSiteSynchronizationListBySiteSQL(SecurityClass security, Guid siteGuid);

		/// <summary>
		/// The enumerate limit site member by parent site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SiteCollectionClass"/>.
		/// </returns>
		[OperationContract]
		SiteCollectionClass EnumerateLimitSiteMemberByParentSite(SecurityClass security, Guid siteGuid);
        
	    [OperationContract]
	    SiteCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid);

	    /// <summary>
	    /// The enumerate report directories.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <returns>
	    /// The <see>
	    ///         <cref>List</cref>
	    ///     </see>.
	    /// </returns>
	    [OperationContract]
		List<SiteClass> EnumerateReportDirectories(SecurityClass security);

	    /// <summary>
	    /// The enumerate index ID group flag.
	    /// </summary>
	    /// <param name="security">
	    /// The security.
	    /// </param>
	    /// <returns>
	    /// The <see>
	    ///         <cref>List</cref>
	    ///     </see>.
	    /// </returns>
	    [OperationContract]
		List<SiteClass> EnumerateIndexIdGroupFlag(SecurityClass security);

        /// <summary>
        /// Enumerate sites based on email information.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Returns a list of sites that contains email information</returns>
        [OperationContract]
        List<SiteClass> EnumerateMailInfo(SecurityClass security);

        [OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid CreateDefaultSingleSite ( SecurityClass security, SiteClass site );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid CreateDefaultSingleSiteByLoginID(SecurityClass security, SiteClass site, string databaseLogOnId);

		/// <summary>
		/// Purges the specified site.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The identity GUID of the site to delete.</param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid identityGuid);

		/// <summary>
		/// Determines whether the site identified by the identityGuid is a Site Group.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="identityGuid">The identity GUID of the site to check.</param>
		/// <returns>
		///	<c>true</c> if the site identified by the identityGuid is a Site Group; otherwise, <c>false</c>.
		/// </returns>
		[OperationContract]
		bool IsSiteGroup(SecurityClass security, Guid identityGuid);

		[OperationContract]
		void InitializeNextDocumentInvoiceNumbers(SecurityClass security, string siteId);

		/// <summary>
		/// Gets the maximum site row version.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		[OperationContract]
		Int64? GetMaxSiteRowVersion(SecurityClass security);

	    [OperationContract]
	    bool ApplyGlobalRecordVersionUpdates(SecurityClass security);

		/// <summary>
		/// Gets the maximum site row version.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		[OperationContract]
		Dictionary<Guid, string> EnumerateTimeZonesForSiteGuidList(SecurityClass security, List<Guid> siteGuidList);

		/// <summary>
		/// Determines if Alarm and Silense are enabled based upon IsEnterprise and Site.Enterprise
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>isAcknowledxgeAndSilenceEnabled</returns>
		[OperationContract]
		bool IsAcknowledgeAndSilenceEnabled(SecurityClass security);

		/// <summary>
		/// retrieves ID for Movement Point of the form "Movement Site.ID Site.MovementNumber"
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>MovementID</returns>
		[OperationContract]
		string GetMovementID(SecurityClass security);
	}
}
