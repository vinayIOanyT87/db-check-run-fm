// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISessions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface for persisting SessionClass instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// This interface abstracts the data object for persisting SessionClass instances
	/// </summary>
	[ServiceContract]
	public interface ISessions
	{
		/// <summary>
		/// This method adds ISession instance
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="session">A SessionClass instance</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, SessionClass session);

		/// <summary>
		/// This method modifies an existing ISession instance
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="session">A SessionClass instance</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, SessionClass session);

		/// <summary>
		/// This method purges an existing ISession instance
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="sessionGuid">A SessionClass instance</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid sessionGuid);

		/// <summary>
		/// Delete any sessions that have expired based on the timeout value
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeExpired(SecurityClass security);

		/// <summary>
		/// This method returns a collection of SessionClass instances from users currently logged into FuelsManager
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <returns>A SessionClassCollection instance</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SessionClassCollection GetDistinctUserSessions(SecurityClass security);

		/// <summary>
		/// This method will clean up the user sessions.
		/// </summary>
		/// <param name="security"></param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void CleanupExpiredUserSessions(SecurityClass security);

		/// <summary>
		/// Pings the session to keep it alive.
		/// </summary>
		/// <param name="security">The security object.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PingSession(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SessionClassCollection GetExpiredUserSessions(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SessionClassCollection EnumerateUserSessions(SecurityClass security);
		
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SessionClassCollection EnumerateUserSessionsWithOrder(SecurityClass security, string orderBy);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		DataSet GetUserSessionsList(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		DataSet GetUserSessionsListWithOrder(SecurityClass security, string orderBy);

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      DataSet GetActiveOperateScreensList(SecurityClass security);

      [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SessionClass GetSessionInfo(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		bool IsSessionValid(string token);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		int GetCountActiveOperateScreens(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void MarkScreenUsingOperate(SecurityClass security, string windoName, bool usingOperate);

		/// <summary>
		/// Clears the OperateActive flag on sessions that are determined to be stale.
		/// </summary>
		/// <param name="security">The FM security object for this call.</param>
		/// <param name="staleSessionTimeout">age of the session in seconds to determine that it's usage of operate is stale</param>
		/// <remarks>
		/// While a user is in Operate, the session entry is expected to be updated every second by the alarm status pings.
		/// If the session hasn't been updated in some amount of time (the stale session timeout), we can assume that the
		/// session is abandoned and the flag can be cleared.
		/// </remarks>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeactivateStaleOperateScreens(SecurityClass security, int staleSessionTimeout);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
      void SaveSessionOperateStatistics(SecurityClass security, OperateStatistics statistics);
    }
}
