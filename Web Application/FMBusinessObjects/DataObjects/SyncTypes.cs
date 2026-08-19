// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncTypes.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System.Xml.Serialization;

    using FMBusinessObjects.ServiceRequests;

    public static class SyncConstants
    {
        public const string DEFAULT_PROFILE_COMPLETE = "{Complete}";
    }

    public enum TABLENAMEFORMAT
    {
        TABLENAME = 0,
        TABLENAME_SCHEMA = 1,
        TABLENAME_SCHEMA_DATABASE = 2,
        FULLY_QUALIFIED = 3
    }

    /// <summary>
    /// Enumeration that identifies the results of converting a <see cref="SecuritySyncLoginRequest"/> to a <see cref="SecurityLoginRequest"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum LOGINCONVERSIONRESULT
    {
        /// <summary>
        /// Conversion was successful.
        /// </summary>
        OK = 0,

        /// <summary>
        /// Missing Client Certificate.  Only applies if the server only accepts client certificate authentication.
        /// </summary>
        CLIENTCERTMISSING = 1,

        /// <summary>
        /// Missing Credentials.  Only applies if the server only accepts user credentials.
        /// </summary>
        LOGINMISSING = 2
    }

    /// <summary>
    /// Enumeration that identifies the different synchronization request types such as Manual and Periodic.
    /// <para>
    /// 
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncRequestType")]
    [XmlRoot(Namespace = "urn:FMSyncRequestType")]
    public enum SYNCREQUESTTYPE
    {
        MANUAL = 0,
        PERIODIC = 1,
        SCHEDULED = 2,
        RESYNC = 3,
        INIT = 4
    }

    /// <summary>
    /// Enumeration that identifies which method of synchronization the Synchronization Process will use when exchanging information
    /// with the Enterprise Server.  Offline or Online
    /// </summary>
    /// <remarks>
    /// <para>
    /// During offline synchronization, one or more file(s) are generated with the client extract information.  These file(s) are 
    /// provided to the Enterprise Server through an out of band data exchange mechanism.  The Enterprise Server will extract data for 
    /// the client node that provided the data and place the output into one or more file(s) to be returned to the client for 
    /// import into the local repository.
    /// </para>
    /// <para>
    /// During online synchronization, the entire synchronization process is performed in multiple stages but completed within the context
    /// of a large synchronization context.
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncTransferType")]
    [XmlRoot(Namespace = "urn:FMSyncTransferType")]
    public enum SYNCTRANSFERTYPE
    {
        /// <summary>
        /// Synchronize data with the Enterprise Server via the Enterprise WCF Services using any available network connection.
        /// </summary>
        ONLINE = 0,
        /// <summary>
        /// Generate offline synchronization output file(s).
        /// </summary>
        OFFLINE = 1
    }

    /// <summary>
    /// Enumeration that identifies the different steps the SyncController iterates through. 
    /// <para>
    /// Our SyncController processes Inserts/Updates independently of Deletes.  However; the Sync Framework is
    /// designed to always apply Inserts/Updates/Deletes so we disable/enable Inserts/Updates or Deletes depending on
    /// which step we're currently focusing on.
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncControllerStep")]
    [XmlRoot(Namespace = "urn:FMSyncControllerStep")]
    public enum SYNCCONTROLLERSTEP
    {
        PROCESS_ALL = 0,
        PROCESS_INSERT_UPDATE = 1,
        PROCESS_INSERT_UPDATE_CONFLICT = 2,
        PROCESS_DELETE = 3,
        PROCESS_DELETE_CONFLICT = 4
    }

    /// <summary>
    /// Enumeration that identifies the context of sync anchors used by the synchronization engine.  This is a custom implementation.
    /// <para>
    /// Used to designate the context of a specific anchor.  Inserts, Updates or Deletes
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncAnchorType")]
    [XmlRoot(Namespace = "urn:FMSyncAnchorType")]
    public enum SYNCANCHORTYPE
    {
        INSERTS = 0,
        UPDATES = 1,
        DELETES = 2
    }

    /// <summary>
    /// Enumeration that identifies synchronization sessions type.  Incremental, Schema Update or By Date Range
    /// <para>
    /// Regardless of the synchronization session type, SITE/SITEGROUP filtering is always applied at the Server.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Batching can be used for any of the synchronization session types list.  <see cref="IsBatching"/> 
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncSessionType")]
    [XmlRoot(Namespace = "urn:FMSyncSessionType")]
    public enum SYNCSESSION
    {
        /// <summary>
        /// Obtain all changes since the last synchronization session.
        /// </summary>
        /// <remarks>
        /// Synchronization direction is defined by the default SyncAgent configuration.
        /// </remarks>
        DEFAULT = 0,
        /// <summary>
        /// Based on the local schema update history, obtain all records since the server applied the corresponding schema update.
        /// </summary>
        /// <remarks>
        /// This forces a Download only (semi-merged) session.  Uploading records could push NULL/DEFAULT
        /// values to the server for columns that previously did not exist on the client.
        /// A bidrectional synchronization could also prevent records impacted by schema changes 
        /// from being returned or sent due to the bidirectional filtering logic.
        /// </remarks>
        SCHEMA_UPDATE = 1,
        /// <summary>
        /// Obtain a refresh of all records that changed within the date range specified.
        /// </summary>
        /// <remarks>
        /// This forces a Download only session.
        /// </remarks>
        DATE_RANGE = 2
    }

    /// <summary>
    /// Enumeration that identifies the various synchronization directions for the tables.  DownloadOnly, UploadOnly, 
    /// BiDirectional, Snapshot.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncDirectionType")]
    [XmlRoot(Namespace = "urn:FMSyncDirectionType")]
    public enum SYNCDIRECTION
    {
        /// <summary>
        /// During the first synchronization, the client typically downloads an initial data set from the server. 
        /// On subsequent synchronizations, the client downloads changes from the server. 
        /// </summary>
        DOWNLOADONLY = 0,
        /// <summary>
        /// During synchronization, the client uploads changes to the server but does not receive any changes back.
        /// </summary>
        UPLOADONLY = 1,
        /// <summary>
        /// During the first synchronization, the client typically downloads an initial data set from the server. 
        /// On subsequent synchronizations, the client uploads changes to the server and then downloads changes from the server. 
        /// </summary>
        /// <remarks>
        /// This forces a Download only session.
        /// </remarks>
        BIDIRECTIONAL = 2,
        /// <summary>
        /// The client downloads a set of data from the server. The data is completely refreshed during each synchronization. 
        /// </summary>
        SNAPSHOT = 3
    }

    /// <summary>
    /// Enumeration that identifies the various synchronization stages/steps for the synchronization process.
    /// </summary>
    /// <remarks>
    /// The initial implementation of the FuelsManager Synchronization Framework will not support the following stages:
    /// ReadingSchema, CreatingSchema, CreatingMetadata and DeletingMetadata.  These are performed during deployment/application updates.
    /// <para>The stages/steps are temporary placeholders to mirror the Synchronization Framework stages.</para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncStageType")]
    [XmlRoot(Namespace = "urn:FMSyncStageType")]
    public enum SYNCSTAGE
    {
        /// <summary>
        /// Synchronization engine is ready for a synchronization request.
        /// </summary>
        READY = 0,
        /// <summary>
        /// Preparing new synchronization session.  Performing pre-session analysis / optimizations.
        /// </summary>
        INITIALIZING = 1,
        /// <summary>
        /// Reading schema information at the server. (Not Implemented)
        /// </summary>
        READING_SCHEMA = 2,
        /// <summary>
        /// Creating tables at the client. (Not Implemented)
        /// </summary>
        CREATING_SCHEMA = 3,
        /// <summary>
        /// Reading metadata tables at the client.
        /// </summary>
        READING_METADATA = 4,
        /// <summary>
        /// Creating metadata tables at the client. (Not Implemented)
        /// </summary>
        CREATING_METADATA = 5,
        /// <summary>
        /// Removing metadata for one or more tables at the client. (Not Implemented)
        /// </summary>
        DELETING_METADATA = 6,
        /// <summary>
        /// Updating metadata tables at the client.
        /// </summary>
        WRITING_METADATA = 7,
        /// <summary>
        /// Sending changes to the server.
        /// </summary>
        UPLOADING_CHANGES = 8,
        /// <summary>
        /// Receiving changes from the server.
        /// </summary>
        DOWNLOADING_CHANGES = 9,
        /// <summary>
        /// Applying inserts to the client or server store.
        /// </summary>
        APPLYING_INSERTS = 10,
        /// <summary>
        /// Applying updates to the client or server store.
        /// </summary>
        APPLYING_UPDATES = 11,
        /// <summary>
        /// Applying deletes to the client or server store.
        /// </summary>
        APPLYING_DELETES = 12,
        /// <summary>
        /// Selecting inserts from the client or server store.
        /// </summary>
        GETTING_INSERTS = 13,
        /// <summary>
        /// Selecting updates from the client or server store.
        /// </summary>
        GETTING_UPDATES = 14,
        /// <summary>
        /// Selecting deletes from the client or server store.
        /// </summary>
        GETTING_DELETES = 15,
        /// <summary>
        /// Synchronization session completed.
        /// </summary>
        COMPLETED = 16,
        /// <summary>
        /// Synchronization session completed with errors.
        /// </summary>
        COMPLETED_WITH_ERRORS = 17
    }

    /// <summary>
    /// Enumeration that identifies the various synchronization directions for the tables.  DownloadOnly, UploadOnly, 
    /// BiDirectional, Snapshot.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncConflictType")]
    [XmlRoot(Namespace = "urn:FMSyncConflictType")]
    public enum SYNCCONFLICTTYPE
    {
        /// <summary>
        /// The client synchronization provider can classify all conflicts that it encounters, but the server synchronization 
        /// provider cannot. Therefore, some conflicts are classified as Unknown.
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// The client or server store (typically a database) threw an exception while applying a change.
        /// </summary>
        ERRORS_OCCURRED = 1,
        /// <summary>
        /// The client and the server updated the same row. 
        /// </summary>
        CLIENTUPDATE_SERVERUPDATE = 2,
        /// <summary>
        /// The server deleted a row that the client updated. 
        /// </summary>
        CLIENTUPDATE_SERVERDELETE = 3,
        /// <summary>
        /// The client deleted a row that the server updated. 
        /// </summary>
        CLIENTDELETE_SERVERUPDATE = 4,
        /// <summary>
        /// The client and server both inserted a row that has the same primary key value. This can also be caused by a unique key constraint violation.
        /// </summary>
        CLIENTINSERT_SERVERINSERT = 5,
        /// <summary>
        /// The client and server both contain a row that has different primary key values but contain the same ID value within the site entity assignment scope.
        /// </summary>
        CLIENTSERVER_DUPLICATEID = 6
    }

    /// <summary>
    /// Enumeration that identifies synchronization scope types.  Global, Reference Only, Hosted Only or Both
    /// <para>
    /// This enumeration helps to provide synchronization hints to the main SyncController.
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncScopeType")]
    [XmlRoot(Namespace = "urn:FMSyncScopeType")]
    public enum SYNCSCOPETYPE
    {
        /// <summary>
        /// Scope Type is unknown.
        /// </summary>
        UKNOWN = 0,
        /// <summary>
        /// Global scopes are processed one time during a synchronization session, typically before or after all site specific scopes have been synchronized. 
        /// </summary>
        /// <remarks>
        /// </remarks>
        GLOBAL = 1,
        /// <summary>
        /// Scopes that are identified as applicable to Reference Sites are only performed for each SiteGroup referenced by the remote node, but not managed by the node.
        /// </summary>
        /// <remarks>
        /// </remarks>
        REFERENCE_ONLY = 2,
        /// <summary>
        /// Scopes that are identified as applicable to Hosted Sites are only performed for each Site/SiteGroup being hosted by remote node.
        /// </summary>
        /// <remarks>
        /// </remarks>
        HOSTED_ONLY = 3,
        /// <summary>
        /// Scopes that are identified as applicable to Both are only performed for each Reference SiteGroup AND each Hosted Site/SiteGroup on the remote node.
        /// </summary>
        /// <remarks>
        /// </remarks>
        BOTH = 4
    }

    /// <summary>
    /// Enumeration that identifies the current state of the synchronization service.  Idle, In Progress, etc..
    /// <para>
    /// This enumeration is used to report the current state of the synchronization service platform.  It can be used to represent the state for
    /// online or offline synchronization request types.
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncServiceState")]
    [XmlRoot(Namespace = "urn:FMSyncServiceState")]
    public enum SYNCSERVICESTATE
    {
        /// <summary>
        /// State has not been initialized or is unavailable.
        /// </summary>
        UNAVAILABLE = 0,

        /// <summary>
        /// There are no synchronization requests actively being processed.  The synchronization service is ready to synchronize.
        /// </summary>
        /// <remarks>
        /// </remarks>
        READY = 1,

        /// <summary>
        /// A synchronization service request is currently active and in the process of performing synchronization.
        /// </summary>
        /// <remarks>
        /// </remarks>
        IN_PROGRESS = 2,

        /// <summary>
        /// The enterprise server is not accepting synchronization service requests.  This indicates the enterprise server has been configured to disallowed incoming synchronization.
        /// </summary>
        /// <remarks>
        /// </remarks>
        ENTERPRISE_NOT_ACCEPTING = 3,

        /// <summary>
        /// The enterprise server is not accepting synchronization service requests for the specified Site/SiteGroup.  This indicates the enterprise server has disallowed incoming synchronization for a specific Site/SiteGroup.
        /// </summary>
        /// <remarks>
        /// </remarks>
        ENTERPRISE_NOT_ACCEPTING_SITE = 4,

        /// <summary>
        /// Synchronization has been disabled on the local node.  This indicates the client synchronization setting has been configured to disable synchronization to the enterprise.
        /// </summary>
        /// <remarks>
        /// </remarks>
        DISABLED_LOCALLY = 5,

        /// <summary>
        /// The client synchronization node was denied access to the Enterprise Synchronization service.  Typically associated with Transport Layer Security not being configured correctly.
        /// </summary>
        /// <remarks>
        /// </remarks>
        SERVICE_ACCESS_DENIED = 6,

        /// <summary>
        /// The client synchronization node was denied synchronization access by the Enterprise FuelsManager.  Typically associated with FuelsManager User/Application Security not being configured correctly.
        /// The FuelsManager account that initiated synchronization was not granted synchronization rights at the Enterprise.
        /// </summary>
        /// <remarks>
        /// </remarks>
        FMAUTH_ACCESS_DENIED = 7,
        
        /// <summary>
        /// The Enterprise FuelsManager synchronization configuration is not configured to accept FM Username/Password or FM User Certificate authentication.
        /// </summary>
        /// <remarks>
        /// </remarks>
        ENTERPRISE_FM_AUTHENTICATION_NOT_CONFIGURED = 8,

        /// <summary>
        /// The client synchronization node was not able to successfully authenticate to the Enterprise FuelsManager.  Typically associated with FuelsManager User/Application Security issues. 
        /// The FuelsManager account that initiated synchronization has an invalid password or was unrecognized on the Enterprise Server.
        /// </summary>
        /// <remarks>
        /// </remarks>
        FMAUTH_LOGIN_FAILURE = 9
    }

    /// <summary>
    /// Enumeration that identifies how the site was synchronized during a synchronization session.  Root, Reference or Hosted
    /// <para>
    /// This enumeration determines the scope that a Site played during synchronization.
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncSiteType")]
    [XmlRoot(Namespace = "urn:FMSyncSiteType")]
    public enum SYNCSITETYPE
    {
        /// <summary>
        /// The site was specified as the Root site for synchronization.
        /// </summary>
        ROOT = 0,
        /// <summary>
        /// The site is being synchronized as a Reference site.  Only entities used by the Hosted Sites will be synchronized.
        /// </summary>
        REFERENCE = 1,
        /// <summary>
        /// The site is being synchronized as a Hosted site which means that it's a full synchronization scope for the site.
        /// </summary>
        HOSTED = 2
    }

    /// <summary>
    /// Enumeration that identifies the current status of a synchronization session.  New, Started, Completed, Failed, User Stopped, System Stopped, etc.
    /// <para>
    /// The status represents the answer to the question "How is the synchronization process going?" or "How did the synchronization process go?"
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncSessionStatus")]
    [XmlRoot(Namespace = "urn:FMSyncSessionStatus")]
    public enum SYNCSESSIONSTATUS
    {
        /// <summary>
        /// Session has been created but no activity has taken place.
        /// </summary>
        NEW = 0,
        /// <summary>
        /// Synchronization has started.
        /// </summary>
        STARTED = 1,
        /// <summary>
        /// Synchronization has completed successfully without any errors or conflicts.
        /// </summary>
        COMPOK = 2,
        /// <summary>
        /// Synchronization has completed successfully but conflicts were detected.
        /// </summary>
        COMPCON = 3,
        /// <summary>
        /// Synchronization has failed.  The session was terminated.
        /// </summary>
        FAILED = 4,
        /// <summary>
        /// Synchronization was stopped by a user.
        /// </summary>
        USERSTOP = 5,
        /// <summary>
        /// Synchronization was stopped by a non-user system event.
        /// </summary>
        SYSSTOP = 6
    }

    /// <summary>
    /// Enumeration that identifies the current state of a synchronization session.  Initializing, Connecting, Authenticating, Processing Inserts/Updates, Processing Deletes, etc.
    /// <para>
    /// The state represents the answer to the question "What is the synchronization process doing?" or "Where is the synchronization process at?"
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncSessionState")]
    [XmlRoot(Namespace = "urn:FMSyncSessionState")]
    public enum SYNCSESSIONSTATE
    {
        /// <summary>
        /// Session is currently being initialized
        /// </summary>
        INIT = 0,

        /// <summary>
        /// Establishing connection to remote server.
        /// </summary>
        CONN = 1,

        /// <summary>
        /// Performing service level authentication (transport layer).
        /// </summary>
        SYSAUTH = 2,

        /// <summary>
        /// Performing FuelsManager authentication.
        /// </summary>
        FMAUTH = 3,

        /// <summary>
        /// Pending Synchronization.
        /// </summary>
        QUEUED = 4,

        /// <summary>
        /// Synchronizing insert / update changes.
        /// </summary>
        PROCESSINSUPD = 5,

        /// <summary>
        /// Synchronizing delete changes.
        /// </summary>
        PROCESSDEL = 6,

        /// <summary>
        /// Downloading Change Batch from Server.
        /// </summary>
        DOWNLOADBATCHFILE = 7,

        /// <summary>
        /// Uploading change Batch to Server.
        /// </summary>
        UPLOADBATCHFILE = 8,

        /// <summary>
        /// Selecting Client Changes.
        /// </summary>
        GETCLIENTCHANGES = 9,

        /// <summary>
        /// Applying Server Changes on Client.
        /// </summary>
        APPLYCHANGESTOCLIENT = 10,

        /// <summary>
        /// Selecting Server Changes.
        /// </summary>
        GETSERVERCHANGES = 11,

        /// <summary>
        /// Applying Client Changes on Server.
        /// </summary>
        APPLYCHANGESTOSERVER = 12,

        /// <summary>
        /// Identifying synchronization conflicts.
        /// </summary>
        SYNCED = 13,

        /// <summary>
        /// Identifying synchronization conflicts.
        /// </summary>
        CONFLICTS = 14,

        /// <summary>
        /// Performing post synchronization logic.
        /// </summary>
        POSTSYNC = 15,

        /// <summary>
        /// Disconnecting from remote server.
        /// </summary>
        DISCONN = 16,

        /// <summary>
        /// Closing synchronization session.
        /// </summary>
        CLOSE = 17,

        /// <summary>
        /// Synchronization session ended
        /// </summary>
        END = 18
    }

    /// <summary>
    /// Enumeration that identifies the various synchronization conflict resolution status options for a record conflict.  Pending, User Resolved, etc. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMSyncConflictResolutionStatus")]
    [XmlRoot(Namespace = "urn:FMSyncConflictResolutionStatus")]
    public enum SYNCCONFLICTRESOLUTIONSTATUS
    {
        /// <summary>
        /// Conflict pending manual user intervention or subsequent synchronization attempt to automatically resolve.
        /// </summary>
        PENDING = 0,

        /// <summary>
        /// Conflict marked as resolved by user.  Waiting for subsequent synchronization attempt to determine if the conflict can be cleared.
        /// </summary>
        RESOLVED = 1,

        /// <summary>
        /// Conflict has been resolved.
        /// </summary>
        CLEARED = 2,

        /// <summary>
        /// System will automatically retry synchronization of this record during the next session.
        /// </summary>
        AUTORETRY = 3
    }


	/// <summary>
	/// Enumeration that identifies the phases of single pass scope 
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
	[XmlType(Namespace = "urn:FMSyncSinglePassScope")]
	[XmlRoot(Namespace = "urn:FMSyncSinglePassScope")]
	public enum SYNCSINGLEPASSPHASE
	{
		/// <summary>
		/// Extraction based upon Root Table
		/// </summary>
		SYNCROOT = 0,

		/// <summary>
		/// Extraction after Root Table
		/// </summary>
		POSTROOT = 1,

		/// <summary>
		/// Extraction complete
		/// </summary>
		COMPLETE = 2

	}



	public class SyncTypes
    {
        public static string GetSyncRequestTypeString(SYNCREQUESTTYPE pRequestType)
        {
            string stringVal = "Not Defined";

            switch (pRequestType)
            {
                case SYNCREQUESTTYPE.MANUAL:
                    stringVal = "Manual";
                    break;
                case SYNCREQUESTTYPE.PERIODIC:
                    stringVal = "Periodic";
                    break;
                case SYNCREQUESTTYPE.SCHEDULED:
                    stringVal = "Scheduled";
                    break;
                case SYNCREQUESTTYPE.RESYNC:
                    stringVal = "Automatic Resync";
                    break;
                case SYNCREQUESTTYPE.INIT:
                    stringVal = "Initialization";
                    break;
                default:
                    break;
            }

            return stringVal;
        }

        public static string GetSyncTransferTypeString(SYNCTRANSFERTYPE pTransferType)
        {
            string stringVal = "Not Defined";

            switch (pTransferType)
            {
                case SYNCTRANSFERTYPE.ONLINE:
                    stringVal = "Online";
                    break;
                case SYNCTRANSFERTYPE.OFFLINE:
                    stringVal = "Offline";
                    break;
                default:
                    break;
            }

            return stringVal;
        }

        public static string GetSyncControllerStepString(SYNCCONTROLLERSTEP pControllerStep)
        {
            string stringVal = "Not Defined";

            switch (pControllerStep)
            {
                case SYNCCONTROLLERSTEP.PROCESS_ALL:
                    stringVal = "Processing All Record Changes";
                    break;
                case SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE:
                    stringVal = "Processing Inserted / Updated Records";
                    break;
                case SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT:
                    stringVal = "Processing Insert/Update Conflicts";
                    break;
                case SYNCCONTROLLERSTEP.PROCESS_DELETE:
                    stringVal = "Processing Deleted Records";
                    break;
                case SYNCCONTROLLERSTEP.PROCESS_DELETE_CONFLICT:
                    stringVal = "Processing Delete Conflicts";
                    break;
                default:
                    break;
            }

            return stringVal;
        }

        public static string GetSyncSessionString(SYNCSESSION pSessionType)
        {
            string stringVal = "Not Defined";

            switch (pSessionType)
            {
                case SYNCSESSION.DEFAULT:
                    stringVal = "Sync Changes";
                    break;
                case SYNCSESSION.SCHEMA_UPDATE:
                    stringVal = "Schema Updated";
                    break;
                case SYNCSESSION.DATE_RANGE:
                    stringVal = "Date Range Synchronization";
                    break;
                default:
                    break;
            }

            return stringVal;
        }

        public static string GetSyncStageString(SYNCSTAGE pStage)
        {
            string stringVal = "Not Defined";

            switch (pStage)
            {
                case SYNCSTAGE.READY:
                    stringVal = "Ready for Synchronization";
                    break;
                case SYNCSTAGE.INITIALIZING:
                    stringVal = "Initializing Synchronization Engine";
                    break;
                case SYNCSTAGE.READING_SCHEMA:
                    stringVal = "Reading Schema";
                    break;
                case SYNCSTAGE.CREATING_SCHEMA:
                    stringVal = "Creating Schema";
                    break;
                case SYNCSTAGE.READING_METADATA:
                    stringVal = "Reading Synchronization Metadata";
                    break;
                case SYNCSTAGE.CREATING_METADATA:
                    stringVal = "Creating Synchronization Metadata";
                    break;
                case SYNCSTAGE.DELETING_METADATA:
                    stringVal = "Deleting Synchronization Metadata";
                    break;
                case SYNCSTAGE.WRITING_METADATA:
                    stringVal = "Writing Synchronization Metadata";
                    break;
                case SYNCSTAGE.UPLOADING_CHANGES:
                    stringVal = "Uploading Changes";
                    break;
                case SYNCSTAGE.DOWNLOADING_CHANGES:
                    stringVal = "Downloading Changes";
                    break;
                case SYNCSTAGE.APPLYING_INSERTS:
                    stringVal = "Applying Record Inserts";
                    break;
                case SYNCSTAGE.APPLYING_UPDATES:
                    stringVal = "Applying Record Updates";
                    break;
                case SYNCSTAGE.APPLYING_DELETES:
                    stringVal = "Applying Record Deletions";
                    break;
                case SYNCSTAGE.GETTING_INSERTS:
                    stringVal = "Retrieving New Records";
                    break;
                case SYNCSTAGE.GETTING_UPDATES:
                    stringVal = "Retrieving Updated Records";
                    break;
                case SYNCSTAGE.GETTING_DELETES:
                    stringVal = "Retrieving Deleted Records";
                    break;
                case SYNCSTAGE.COMPLETED:
                    stringVal = "Completed";
                    break;
                case SYNCSTAGE.COMPLETED_WITH_ERRORS:
                    stringVal = "Completed with Errors";
                    break;
                default:
                    break;
            }

            return stringVal;
        }

        public static string GetSyncConflictTypeString(SYNCCONFLICTTYPE pConflictType)
        {
            string stringVal = "Not Defined";

            switch (pConflictType)
            {
                case SYNCCONFLICTTYPE.UNKNOWN:
                    stringVal = "Unknown";
                    break;
                case SYNCCONFLICTTYPE.ERRORS_OCCURRED:
                    stringVal = "Database error detected";
                    break;
                case SYNCCONFLICTTYPE.CLIENTUPDATE_SERVERUPDATE:
                    stringVal = "Client / Server Update";
                    break;
                case SYNCCONFLICTTYPE.CLIENTUPDATE_SERVERDELETE:
                    stringVal = "Client Update / Server Delete";
                    break;
                case SYNCCONFLICTTYPE.CLIENTDELETE_SERVERUPDATE:
                    stringVal = "Client Delete / Server Update";
                    break;
                case SYNCCONFLICTTYPE.CLIENTINSERT_SERVERINSERT:
                    stringVal = "Client / Server Insert";
                    break;
                case SYNCCONFLICTTYPE.CLIENTSERVER_DUPLICATEID:
                    stringVal = "Duplicate Record ID Detected";
                    break;
                default:
                    break;
            }

            return stringVal;
        }
        public static string GetSyncScopeTypeString(SYNCSCOPETYPE pScopeType)
        {
            string stringVal = "Not Defined";

            switch (pScopeType)
            {
                case SYNCSCOPETYPE.UKNOWN:
                    stringVal = "Unknown";
                    break;
                case SYNCSCOPETYPE.GLOBAL:
                    stringVal = "Global";
                    break;
                case SYNCSCOPETYPE.REFERENCE_ONLY:
                    stringVal = "Reference Sites Only";
                    break;
                case SYNCSCOPETYPE.HOSTED_ONLY:
                    stringVal = "Hosted Sites Only";
                    break;
                case SYNCSCOPETYPE.BOTH:
                    stringVal = "All Sites (Reference and Hosted)";
                    break;
                default:
                    break;
            }

            return stringVal;
        }
        public static string GetSyncServiceStateString(SYNCSERVICESTATE pServiceState)
        {
            string stringVal = "Not Defined";

            switch (pServiceState)
            {
                case SYNCSERVICESTATE.UNAVAILABLE:
                    stringVal = "Unavailable";
                    break;
                case SYNCSERVICESTATE.READY:
                    stringVal = "Ready";
                    break;
                case SYNCSERVICESTATE.IN_PROGRESS:
                    stringVal = "In Progress";
                    break;
                case SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING:
                    stringVal = "Enterprise Not Accepting Requests";
                    break;
                case SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING_SITE:
                    stringVal = "Enterprise Not Accepting Requests for Site";
                    break;
                case SYNCSERVICESTATE.DISABLED_LOCALLY:
                    stringVal = "Synchronization Disabled Locally";
                    break;
                case SYNCSERVICESTATE.SERVICE_ACCESS_DENIED:
                    stringVal = "Access Denied to Enterprise Synchronization Service";
                    break;
                case SYNCSERVICESTATE.FMAUTH_ACCESS_DENIED:
                    stringVal = "FuelsManager Authentication to Enterprise Server Failed";
                    break;
                case SYNCSERVICESTATE.ENTERPRISE_FM_AUTHENTICATION_NOT_CONFIGURED:
                    stringVal = "Enterprise FuelsManager Authentication Not Configured.  Unable to Synchronize";
                    break;
                default:
                    break;
            }

            return stringVal;
        }

        public static string GetSyncSessionStatusString(SYNCSESSIONSTATUS pSessionStatus)
        {
            string stringVal = "Not Defined";

            switch (pSessionStatus)
            {
                case SYNCSESSIONSTATUS.NEW:
                    stringVal = "New";
                    break;
                case SYNCSESSIONSTATUS.STARTED:
                    stringVal = "Started";
                    break;
                case SYNCSESSIONSTATUS.COMPOK:
                    stringVal = "Completed";
                    break;
                case SYNCSESSIONSTATUS.COMPCON:
                    stringVal = "Completed w/ Conflicts";
                    break;
                case SYNCSESSIONSTATUS.FAILED:
                    stringVal = "Failed";
                    break;
                case SYNCSESSIONSTATUS.USERSTOP:
                    stringVal = "Stopped (User)";
                    break;
                case SYNCSESSIONSTATUS.SYSSTOP:
                    stringVal = "Stopped (System)";
                    break;
                default:
                    break;
            }

            return stringVal;
        }
    }
}
