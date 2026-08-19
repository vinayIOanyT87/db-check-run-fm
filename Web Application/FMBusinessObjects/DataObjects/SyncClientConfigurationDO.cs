using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Net;

using System.ComponentModel;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	using FMBusinessObjects.ChannelFactories;

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(SyncClientConfigurationDO))]
	public class SyncClientConfigurationCollection : List<SyncClientConfigurationDO>
	{
	}

	[XmlType("SyncClientConfiguration")]
	[DataContract]
	[Serializable]
	public class SyncClientConfigurationDO : BaseDataObject, INotifyPropertyChanged
	{
		#region Data Members
		private bool _EnableChangeTracking = true;
		private bool _Changed = false;

		private Guid _SyncNodeGuid = Guid.Empty;    // This field is given to us for quick reference from ConfigurationSettings 
		// because there can only be one NodeGuid per physical instance.

		private string _RootSiteID = null;
		private string _EnterpriseURL = null;
		private bool _SuspendSynchronizationFlag = false;
		private string _ServerAuthUserName = null;
		private string _ServerAuthPassword = null;
		private string _ServerAuthDomain = null;
		private string _ServerAuthClientCertificate = null;
		private string _FMAuthUserName = null;
		private string _FMAuthPassword = null;
		private string _FMAuthClientCertificate = null;
		private string _MessageSecuritySigningCertificate = null;
		private string _MessageSecurityOfflineEncryptionCertificate = null;
		private string _MessageSecurityOfflineDecryptionCertificate = null;

		private int _ServiceMaximumRetryAttempts = FMChannelHelper.DefaultRetryAttempts;

		private int _ServiceRetryWaitTime = FMChannelHelper.DefaultRetryWaitTime;

		/// <summary>
		/// The _ offline synchronization working directory.
		/// </summary>
		private string _OfflineSynchronizationWorkingDirectory = @"C:\temp\fmsync";

		[NonSerialized]
		private NetworkCredential _NetworkCredentials = null;
		private bool _RefreshNetworkCredentials = false;
		#endregion Data Members

		#region Properties

		[DataMember]
		public bool EnableChangeTracking
		{
			get
			{
				return (_EnableChangeTracking);
			}
			set
			{
				_EnableChangeTracking = value;
			}
		}

		[DataMember]
		public bool Changed
		{
			get { return (_Changed); }
			set
			{
				if (value == _Changed)
					return;

				_Changed = value;

				RaisePropertyChanged("Changed", false);
			}
		}
		[DataMember]
		public Guid SyncNodeGuid
		{
			get { return _SyncNodeGuid; }
			set
			{
				if (value == _SyncNodeGuid)
					return;

				_SyncNodeGuid = value;

				RaisePropertyChanged("SyncNodeGuid");
			}
		}
		[DataMember]
		public string RootSiteID
		{
			get { return _RootSiteID; }
			set
			{
				if (value == _RootSiteID)
					return;

				SetString("RootSiteID", 15, value, ref _RootSiteID);

				RaisePropertyChanged("RootSiteID");
			}
		}
		[DataMember]
		public string EnterpriseURL
		{
			get { return _EnterpriseURL; }
			set
			{
				if (value == _EnterpriseURL)
					return;

				SetString("EnterpriseURL", 512, value, ref _EnterpriseURL);

				RaisePropertyChanged("EnterpriseURL");
			}
		}
		[DataMember]
		public bool SuspendSynchronizationFlag
		{
			get { return _SuspendSynchronizationFlag; }
			set
			{
				if (value == _SuspendSynchronizationFlag)
					return;

				_SuspendSynchronizationFlag = value;

				RaisePropertyChanged("SuspendSynchronizationFlag");
			}
		}
		[DataMember]
		public string ServerAuthUserName
		{
			get { return _ServerAuthUserName; }
			set
			{
				if (value == _ServerAuthUserName)
					return;

				SetString("ServerAuthUserName", 128, value, ref _ServerAuthUserName);

				_RefreshNetworkCredentials = true;

				RaisePropertyChanged("ServerAuthUserName");
			}
		}
		[DataMember]
		public string ServerAuthPassword
		{
			get { return _ServerAuthPassword; }
			set
			{
				if (value == _ServerAuthPassword)
					return;

				SetString("ServerAuthPassword", 256, value, ref _ServerAuthPassword);

				_RefreshNetworkCredentials = true;

				RaisePropertyChanged("ServerAuthPassword");
			}
		}
		[DataMember]
		public string ServerAuthDomain
		{
			get { return _ServerAuthDomain; }
			set
			{
				if (value == _ServerAuthDomain)
					return;

				SetString("ServerAuthDomain", 128, value, ref _ServerAuthDomain);

				_RefreshNetworkCredentials = true;

				RaisePropertyChanged("ServerAuthDomain");
			}
		}
		[DataMember]
		public string ServerAuthClientCertificate
		{
			get { return _ServerAuthClientCertificate; }
			set
			{
				if (value == _ServerAuthClientCertificate)
					return;

				SetString("ServerAuthClientCertificate", 384, value, ref _ServerAuthClientCertificate);

				RaisePropertyChanged("ServerAuthClientCertificate");
			}
		}
		[DataMember]
		public string FMAuthUserName
		{
			get { return _FMAuthUserName; }
			set
			{
				if (value == _FMAuthUserName)
					return;

				SetString("FMAuthUserName", 128, value, ref _FMAuthUserName);

				RaisePropertyChanged("FMAuthUserName");
			}
		}
		[DataMember]
		public string FMAuthPassword
		{
			get { return _FMAuthPassword; }
			set
			{
				if (value == _FMAuthPassword)
					return;

				SetString("FMAuthPassword", 256, value, ref _FMAuthPassword);

				RaisePropertyChanged("FMAuthPassword");
			}
		}
		[DataMember]
		public string FMAuthClientCertificate
		{
			get { return _FMAuthClientCertificate; }
			set
			{
				if (value == _FMAuthClientCertificate)
					return;

				SetString("FMAuthClientCertificate", 384, value, ref _FMAuthClientCertificate);

				RaisePropertyChanged("FMAuthClientCertificate");
			}
		}
		[DataMember]
		public string MessageSecuritySigningCertificate
		{
			get { return _MessageSecuritySigningCertificate; }
			set
			{
				if (value == _MessageSecuritySigningCertificate)
					return;

				SetString("MessageSecuritySigningCertificate", 384, value, ref _MessageSecuritySigningCertificate);

				RaisePropertyChanged("MessageSecuritySigningCertificate");
			}
		}
		[DataMember]
		public string MessageSecurityOfflineEncryptionCertificate
		{
			get { return _MessageSecurityOfflineEncryptionCertificate; }
			set
			{
				if (value == _MessageSecurityOfflineEncryptionCertificate)
					return;

				SetString("MessageSecurityOfflineEncryptionCertificate", 384, value, ref _MessageSecurityOfflineEncryptionCertificate);

				RaisePropertyChanged("MessageSecurityOfflineEncryptionCertificate");
			}
		}
		[DataMember]
		public string MessageSecurityOfflineDecryptionCertificate
		{
			get { return _MessageSecurityOfflineDecryptionCertificate; }
			set
			{
				if (value == _MessageSecurityOfflineDecryptionCertificate)
					return;

				SetString("MessageSecurityOfflineDecryptionCertificate", 384, value, ref _MessageSecurityOfflineDecryptionCertificate);

				RaisePropertyChanged("MessageSecurityOfflineDecryptionCertificate");
			}
		}
		[IgnoreDataMember]
		public NetworkCredential NetworkCredentials
		{
			get
			{
				if (_RefreshNetworkCredentials || null == _NetworkCredentials)
				{
					if (!string.IsNullOrEmpty(ServerAuthUserName) && !string.IsNullOrEmpty(ServerAuthPassword))
						_NetworkCredentials = new NetworkCredential(ServerAuthUserName, ServerAuthPassword, ServerAuthDomain);
					else
						_NetworkCredentials = null;

					_RefreshNetworkCredentials = false;
				}

				return (_NetworkCredentials);
			}
		}

		[DataMember]
		public int ServiceMaximumRetryAttempts
		{
			get
			{
				return (this._ServiceMaximumRetryAttempts);
			}
			set
			{
				this._ServiceMaximumRetryAttempts = value;
			}
		}

		[DataMember]
		public int ServiceRetryWaitTime
		{
			get
			{
				return (this._ServiceRetryWaitTime);
			}
			set
			{
				this._ServiceRetryWaitTime = value;
			}
		}

		[IgnoreDataMember]
		public bool HasServerAuthenticationCredentials
		{
			get
			{
				return
					!(string.IsNullOrEmpty(this.ServerAuthClientCertificate)
					&& string.IsNullOrEmpty(this.ServerAuthDomain) 
					&& string.IsNullOrEmpty(this.ServerAuthUserName)
					&& (string.IsNullOrEmpty(this.ServerAuthPassword)
						|| (this.ServerAuthPassword != null && this.ServerAuthPassword.Equals(GeneralConstants.PasswordPlaceholder))));
			}
		}

		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the SyncClientConfiguration Settings class.
		/// </summary>
		public SyncClientConfigurationDO()
			: base()
		{
			this.Reset();
		}

		#endregion Constructors

		#region Public methods
		public override void Reset()
		{
			base.Reset();
			this._Changed = false;

			this._IdentityGuid = Guid.Empty;
			this._RootSiteID = string.Empty;
			this._EnterpriseURL = string.Empty;
			this._SuspendSynchronizationFlag = false;
			this._ServerAuthUserName = string.Empty;
			this._ServerAuthPassword = string.Empty;
			this._ServerAuthDomain = string.Empty;
			this._ServerAuthClientCertificate = string.Empty;
			this._FMAuthUserName = string.Empty;
			this._FMAuthPassword = string.Empty;
			this._FMAuthClientCertificate = string.Empty;
			this._MessageSecuritySigningCertificate = string.Empty;
			this._MessageSecurityOfflineEncryptionCertificate = string.Empty;
			this._MessageSecurityOfflineDecryptionCertificate = string.Empty;

			this._ServiceMaximumRetryAttempts = FMChannelHelper.DefaultRetryAttempts;
			this._ServiceRetryWaitTime = FMChannelHelper.DefaultRetryWaitTime;

			this._SyncNodeGuid = Guid.Empty;

			this._CreatedDate = DateTimeOffset.Now;
			this._UpdatedDate = DateTimeOffset.Now;
		}

		public void Load(DataRow row)
		{
			this._IdentityGuid = DataObject.getGuid(row["SyncClientConfigurationGuid"]);
			this._RootSiteID = DataObject.getString(row["RootSiteID"]);
			this._EnterpriseURL = DataObject.getString(row["EnterpriseURL"]);
			this._SuspendSynchronizationFlag = DataObject.getValue<bool>(row["SuspendSynchronizationFlag"], false);
			this._ServerAuthUserName = DataObject.getString(row["ServerAuthUserName"]);
			this._ServerAuthPassword = row.IsNull("ServerAuthPassword") ? string.Empty : UserClass.decode((byte[])row["ServerAuthPassword"], Guids.SiteAdminGuid);
			this._ServerAuthDomain = DataObject.getString(row["ServerAuthDomain"]);
			this._ServerAuthClientCertificate = DataObject.getString(row["ServerAuthClientCertificate"]);
			this._FMAuthUserName = DataObject.getString(row["FMAuthUserName"]);
			this._FMAuthPassword = row.IsNull("FMAuthPassword") ? string.Empty : UserClass.decode((byte[])row["FMAuthPassword"], Guids.SiteAdminGuid);
			this._FMAuthClientCertificate = DataObject.getString(row["FMAuthClientCertificate"]);
			this._MessageSecuritySigningCertificate = DataObject.getString(row["MessageSecuritySigningCertificate"]);
			this._MessageSecurityOfflineEncryptionCertificate = DataObject.getString(row["MessageSecurityOfflineEncryptionCertificate"]);
			this._MessageSecurityOfflineDecryptionCertificate = DataObject.getString(row["MessageSecurityOfflineDecryptionCertificate"]);
			this._OfflineSynchronizationWorkingDirectory = DataObject.getString(row["OfflineSynchronizationWorkingDirectory"]);

			this._ServiceMaximumRetryAttempts = DataObject.getInt(row["ServiceMaximumRetryAttempts"]);
			this._ServiceRetryWaitTime = DataObject.getInt(row["ServiceRetryWaitTime"]);

			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getString(row["CreatedBy"]);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getString(row["UpdatedBy"]);

			this._Changed = false;
		}
		#endregion Public Methods

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#region STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES
		protected void RaisePropertyChanged(string propertyName)
		{
			RaisePropertyChanged(propertyName, true);
		}
		protected void RaisePropertyChanged(string propertyName, bool trackChangesFlag)
		{
			if (trackChangesFlag && _EnableChangeTracking)
				_Changed = true;

			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		protected void RaiseMultiplePropertyChanged(params string[] propertyNames)
		{
			foreach (var each in propertyNames)
			{
				RaisePropertyChanged(each);
			}
		}
		#endregion STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES

		#endregion INotifyPropertyChanged Members
	}
}
