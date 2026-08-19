using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.BusinessObjects.Constants
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    /// <summary>
    /// Describes what type of communications session is being performed
    /// </summary>
    public enum ExternalStationSessionType
    {
        [Display(Name = "Manual")]
        Manual = 0,
        [Display(Name = "Periodic")]
        Periodic = 1,
        [Display(Name = "Scheduled")]
        Scheduled = 2,
        [Display(Name = "Setup")]
        Setup = 3
    }

    /// <summary>
    /// Describes what type of communications session is being performed
    /// </summary>
    public enum ExternalStationSessionState
    {
        [Display(Name = "Initializing")]
        Init = 0,
        [Display(Name = "Connecting")]
        Connecting = 1,
        [Display(Name = "Upload Configuration")]
        UploadingConfig = 2,
        [Display(Name = "Download Transactions")]
        DownloadTrans = 3,
        [Display(Name = "Importing Transactions")]
        ImportTrans = 4,
        [Display(Name = "Closing Session")]
        Close = 5,
        [Display(Name = "Session Ended")]
        End = 6
    }

    /// <summary>
    /// Describes what type of communications session is being performed
    /// </summary>
    public enum ExternalStationSessionStatus
    {
        [Display(Name = "New")]
        New = 0,
        [Display(Name = "Started")]
        Started = 1,
        [Display(Name = "Completed")]
        CompOk = 2,
        [Display(Name = "Completed w/ Errors")]
        CompErr = 3,
        [Display(Name = "Failed")]
        Failed = 4,
        [Display(Name = "Stopped (User)")]
        UserStop = 5,
        [Display(Name = "Stopped (System)")]
        SysStop = 6
    }

    /// <summary>
    /// Enumeration that identifies the current state of the service process.  Idle, In Progress, etc..
    /// <para>
    /// This enumeration is used to report the current state of the external station service process.
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMExternalStationServiceState")]
    [XmlRoot(Namespace = "urn:FMExternalStationServiceState")]
    public enum ExternalStationServiceProcessState
    {
        /// <summary>
        /// State has not been initialized or is unavailable.
        /// </summary>
        Unavailable = 0,

        /// <summary>
        /// There are no external station requests actively being processed.  The external station service is ready for new requests.
        /// </summary>
        /// <remarks>
        /// </remarks>
        Ready = 1,

        /// <summary>
        /// Communications with one or more external service stations are currently active and in the process of downloading information.
        /// </summary>
        /// <remarks>
        /// </remarks>
        InProgress = 2
    }

    /// <summary>
    /// Enumeration that indicates whether an external transaction was successfully imported into the system.
    /// <para>
    /// This enumeration is used to report the current status of the external station transaction.
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMExternalStationTransactionStatus")]
    [XmlRoot(Namespace = "urn:FMExternalStationTransactionStatus")]
    public enum ExternalStationTransactionStatus
    {
        [Display(Name = "None")]
        None = -1,
        [Display(Name = "Completed")]
        Completed = 0,
        [Display(Name = "Failed")]
        Failed = 1
    }

    /// <summary>
    /// Enumeration that indicates the current status of a failed transaction.
    /// <para>
    /// This enumeration is used to report the current status of an external station transaction that originally failed to import.
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [XmlType(Namespace = "urn:FMExternalStationTransactionFailedStatus")]
    [XmlRoot(Namespace = "urn:FMExternalStationTransactionFailedStatus")]
    public enum ExternalStationTransactionFailedStatus
    {
        [Display(Name = "None")]
        None = -1,
        [Display(Name = "Pending")]
        Pending = 0,
        [Display(Name = "Reprocess")]
        Reprocess= 1,
        [Display(Name = "AutoRetry")]
        AutoRetry = 2,
        [Display(Name = "Suppressed")]
        Suppressed = 3,
        [Display(Name = "Processed")]
        Processed = 4
    }
}
