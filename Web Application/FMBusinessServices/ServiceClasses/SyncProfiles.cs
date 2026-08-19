// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncProfiles.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncProfiles
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Security;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;
    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// Summary description for SyncProfiles
    /// </summary>
    [SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SyncProfiles : ISyncProfiles
    {
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        public SyncProfiles()
        {
        }

        #region Public Methods

        /// <summary>
        /// Operation to add a new SyncProfile master record version.
        /// </summary>
        /// <param name="pSecurity"></param>
        /// <param name="pSyncProfile"></param>
        /// <returns></returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass pSecurity, SyncProfileDO pSyncProfile)
        {
            if (pSecurity == null)
                throw new ArgumentNullException("pSecurity");

            if (pSyncProfile == null)
                throw new ArgumentNullException("pSyncProfile");

            if (!pSecurity.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw (new Exception("Insufficient Rights"));
            }

            Validate(pSecurity, pSyncProfile);

            pSyncProfile.CreatedDate = DateTimeOffset.Now;
            pSyncProfile.CreatedBy = pSecurity.UserID;
            pSyncProfile.UpdatedDate = pSyncProfile.CreatedDate;
            pSyncProfile.UpdatedBy = pSecurity.UserID;

            using (var dbi = new SyncProfileDBI(pSecurity.UserID))
            {
                dbi.Save(pSecurity, pSyncProfile);
            }

            return (pSyncProfile.IdentityGuid);
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass pSecurity, SyncProfileDO pSyncProfile)
        {
            if (pSecurity == null)
                throw new ArgumentNullException("pSecurity");

            if (pSyncProfile == null)
                throw new ArgumentNullException("pSyncProfile");

            if (!pSecurity.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
                throw (new Exception("Insufficient Rights"));

            Validate(pSecurity, pSyncProfile);

            pSyncProfile.UpdatedDate = DateTimeOffset.Now;
            pSyncProfile.UpdatedBy = pSecurity.UserID;

            using (var dbi = new SyncProfileDBI(pSecurity.UserID))
            {
                dbi.Save(pSecurity, pSyncProfile);
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass pSecurity, Guid pSyncProfileGuid)
        {
            if (pSecurity == null)
                throw new ArgumentNullException("pSecurity");

            if (!pSecurity.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS))
                throw (new Exception("Insufficient Rights"));

            SyncProfileDO pSyncProfile = null;

            using (var dbi = new SyncProfileDBI(pSecurity.UserID))
            {
                pSyncProfile = dbi.Get(pSecurity, pSyncProfileGuid, null);

                if (null == pSyncProfile)
                    throw (new Exception("SyncProfile Not Found"));
                else
                {
                    if (pSyncProfile.IdentityGuid == Guid.Empty)
                        throw (new Exception("SyncProfile Not Found"));
                }

                dbi.Delete(pSecurity, pSyncProfile, true);
            }
        }

        public SyncProfileDO Get(SecurityClass pSecurity, Guid pSyncProfileGuid)
        {
            SyncProfileDO syncProfile = null;

            using (var dbi = new SyncProfileDBI(pSecurity.UserID))
            {
                syncProfile = dbi.Get(pSecurity, pSyncProfileGuid, null);
            }

            return (syncProfile);
        }

        public SyncProfileDO GetById(SecurityClass pSecurity, string pID)
        {
            if (pSecurity == null)
                throw new ArgumentNullException("pSecurity");

            if (!pSecurity.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
                throw (new Exception("Insufficient Rights"));

            SyncProfileDO syncProfile = null;

            using (var dbi = new SyncProfileDBI(pSecurity.UserID))
            {
                syncProfile = dbi.Get(pSecurity, null, pID);
            }

            return (syncProfile);
        }


        public Guid GetIdentityGuid(SecurityClass pSecurity, string pID)
        {
            Guid result = Guid.Empty;
            SyncProfileDO syncProfile = GetById(pSecurity, pID);

            if (syncProfile != null)
                result = syncProfile.IdentityGuid;

            return (result);
        }

        public SyncProfileCollection Enumerate(SecurityClass security)
        {
            return EnumerateExt(security);
        }

        [SecurityCritical]
        public SyncProfileCollection EnumerateExt(SecurityClass pSecurity, int pLimit = 0)
        {
            if (pSecurity == null)
                throw new ArgumentNullException("pSecurity");

            if (!pSecurity.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                throw (new Exception("Insufficient Rights"));
            }

            var syncProfiles = new SyncProfileCollection();

            using (var dbi = new SyncProfileDBI(pSecurity.UserID))
            {
                syncProfiles.AddRange(dbi.GetList(pSecurity));
            }

            return (syncProfiles);
        }

        private string ConvertToString(System.Byte[] rowVersion)
        {
            string result = string.Empty;

            foreach (byte b in rowVersion)
            {
                result += b.ToString("X");
            }

            return result;
        }
        #endregion Public Methods

        #region Validation

        private void Validate(SecurityClass pSecurity, SyncProfileDO pSyncProfile)
        {
            if (string.IsNullOrEmpty(pSyncProfile.ID))
                throw new Exception("ID Required");

            if (pSyncProfile.ID == "{Complete}")
                throw new Exception("ID is reserved key word " + pSyncProfile.ID);

            if (pSyncProfile.FriendlyName.Length > 100)
                throw new Exception("Name Exceeded max length (100)");

            Guid pSyncProfileGuid = GetIdentityGuid(pSecurity, pSyncProfile.ID);
            if (pSyncProfileGuid != Guid.Empty && pSyncProfileGuid != pSyncProfile.IdentityGuid)
                throw (new Exception("SyncProfile Exists"));
        }

        #endregion Validation
    }
}
