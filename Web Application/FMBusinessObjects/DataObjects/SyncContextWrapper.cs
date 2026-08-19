using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    using Microsoft.Synchronization.Data;

    [Serializable]
    [DataContract]
    [KnownType(typeof(SecurityClass))]
    [KnownType(typeof(SiteClass))]
    [KnownType(typeof(SyncTableToScopeMapColumnCollection))]
    [KnownType(typeof(SyncTableToScopeMapColumnDO))]
    [KnownType(typeof(DBNull))]
	[KnownType(typeof(SYNCSITETYPE))]
    [KnownType(typeof(SYNCANCHORTYPE))]
    [KnownType(typeof(SyncTableMetadataFM))]
    [KnownType(typeof(SyncContext))]
    [KnownType(typeof(SyncConflictFM))]
    public class SyncContextWrapper
    {
        #region Public Properties

        [DataMember]
        public SyncContext SyncContext { get; set; }
        [DataMember]
        public byte[] SyncContextFMBytes { get; set; }
        #endregion Public Properties

        #region Constructors
        public SyncContextWrapper()
        {
        }
        public SyncContextWrapper(SyncContext syncContext, byte[] syncContextFMBytes)
        {
            this.SyncContext = syncContext;
            this.SyncContextFMBytes = (byte[])syncContextFMBytes.Clone();
        }
        #endregion Constructors

        #region Public Methods
        public SyncContextWrapper Clone()
        {
            return (SyncContextWrapper)this.MemberwiseClone();
        }
        #endregion Public Methods
    }
}
