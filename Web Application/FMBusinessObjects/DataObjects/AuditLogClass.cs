// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditLogClass.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///	  A collection object for groups of AuditLogClass objects.
    /// </summary>
    [Serializable]
    [DataContract]
    public class AuditLogCollectionClass
    {
        [DataMember]
        private int fullRecordCount;

        [DataMember]
        private List<AuditLogClass> auditLogList;

        public int FullRecordCount
        {
            get { return this.fullRecordCount; }
            set { this.fullRecordCount = value; }
        }

        public List<AuditLogClass> AuditLogList
        {
            get { return this.auditLogList; }
            set { this.auditLogList = value; }
        }

        public AuditLogCollectionClass()
        {
            this.fullRecordCount = 0;
            this.auditLogList = new List<AuditLogClass>();
        }
    }

    /// <summary>
    ///	  Data object describing an Audit Log entry.
    /// </summary>
    [DataContract]
    [Serializable]
    [XMLObject(NodeName = "AuditLog")]
    public class AuditLogClass : BaseDataObject, IEquatable<AuditLogClass>
    {        

        #region Fields

        [DataMember]
        private string sourceNode = string.Empty;

        [DataMember]
        private string auditContext = string.Empty;

        [DataMember]
        private string sessionId = string.Empty;

        [DataMember]
        private string actionId = string.Empty;

        [DataMember]
        private string typeId = string.Empty;

        [DataMember]
        private string parentTypeId = string.Empty;

        [DataMember]
        private string propertyId = string.Empty;

        [DataMember]
        private string oldValue = string.Empty;

        [DataMember]
        private string newValue = string.Empty;

        [DataMember]
        protected DateTimeOffset? auditedDate;


        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogClass"/> class.
        /// </summary>
        public AuditLogClass()
        {
            this.Initialize();
        }

        #endregion

        #region Public Properties



        public string SourceNode
        {
            get { return this.sourceNode; }
            set { this.sourceNode = value; }
        }

        public string AuditContext
        {
            get { return this.auditContext; }
            set { this.auditContext = value; }
        }

        public string SessionId
        {
            get { return this.sessionId; }
            set { this.sessionId = value; }
        }

        public string ActionId
        {
            get { return this.actionId; }
            set { this.actionId = value; }
        }

        public string TypeId
        {
            get { return this.typeId; }
            set { this.typeId = value; }
        }

        public string ParentTypeId
        {
            get { return this.parentTypeId; }
            set { this.parentTypeId = value; }
        }

        public string PropertyId
        {
            get { return this.propertyId; }
            set { this.propertyId = value; }
        }

        public string OldValue
        {
            get { return this.oldValue; }
            set { this.oldValue = value; }
        }

        public string NewValue
        {
            get { return this.newValue; }
            set { this.newValue = value; }
        }

        public DateTimeOffset? AuditedDate
        {
            get { return this.auditedDate; }
            set { this.auditedDate = value; }
        }


        public bool Equals(AuditLogClass other)
        {
            if (other == null)
            {
                return false;
            }

            return other.IdentityGuid == this.IdentityGuid;
        }
        
        
        public override void Reset()
        {
            base.Reset();
            this.Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        ///	  Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.SiteID = string.Empty;
        }
        
        #endregion
    }
}
