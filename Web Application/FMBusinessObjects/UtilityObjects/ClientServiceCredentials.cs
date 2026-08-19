using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.UtilityObjects
{
    using System.ComponentModel;
    using System.Net;
    using System.Runtime.Serialization;

    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    public class ClientServiceCredentials : BaseDTO
    {
        #region Data Members
        private bool enableChangeTracking = true;
        private bool changed = false;

        private string networkAuthUserName = null;
        private string networkAuthPassword = null;
        private string networkAuthDomain = null;
        private string networkAuthClientCertificate = null;

        private string applicationAuthUserName = null;
        private string applicationAuthPassword = null;
        private string applicationAuthClientCertificate = null;

        private string messageSecuritySigningCertificate = null;
        private string messageSecurityOfflineEncryptionCertificate = null;
        private string messageSecurityOfflineDecryptionCertificate = null;

        [NonSerialized]
        private NetworkCredential networkCredentials = null;
        private bool refreshNetworkCredentials = false;

        #endregion Data Members

        #region Properties

        [DataMember]
        public bool EnableChangeTracking
        {
            get
            {
                return (enableChangeTracking);
            }
            set
            {
                enableChangeTracking = value;
            }
        }

        [DataMember]
        public bool Changed
        {
            get { return (changed); }
            set
            {
                if (value == changed)
                    return;

                changed = value;

                RaisePropertyChanged("Changed", false);
            }
        }
        public string NetworkAuthUserName
        {
            get { return networkAuthUserName; }
            set
            {
                if (value == networkAuthUserName)
                    return;

                SetString("NetworkAuthUserName", 128, value, ref networkAuthUserName);

                refreshNetworkCredentials = true;

                RaisePropertyChanged("NetworkAuthUserName");
            }
        }
        [DataMember]
        public string NetworkAuthPassword
        {
            get { return networkAuthPassword; }
            set
            {
                if (value == networkAuthPassword)
                    return;

                SetString("NetworkAuthPassword", 256, value, ref networkAuthPassword);

                refreshNetworkCredentials = true;

                RaisePropertyChanged("NetworkAuthPassword");
            }
        }
        [DataMember]
        public string NetworkAuthDomain
        {
            get { return networkAuthDomain; }
            set
            {
                if (value == networkAuthDomain)
                    return;

                SetString("NetworkAuthDomain", 128, value, ref networkAuthDomain);

                refreshNetworkCredentials = true;

                RaisePropertyChanged("NetworkAuthDomain");
            }
        }
        [DataMember]
        public string NetworkAuthClientCertificate
        {
            get { return networkAuthClientCertificate; }
            set
            {
                if (value == networkAuthClientCertificate)
                    return;

                SetString("NetworkAuthClientCertificate", 384, value, ref networkAuthClientCertificate);

                RaisePropertyChanged("NetworkAuthClientCertificate");
            }
        }
        [DataMember]
        public string ApplicationAuthUserName
        {
            get { return applicationAuthUserName; }
            set
            {
                if (value == applicationAuthUserName)
                    return;

                SetString("ApplicationAuthUserName", 128, value, ref applicationAuthUserName);

                RaisePropertyChanged("ApplicationAuthUserName");
            }
        }
        [DataMember]
        public string ApplicationAuthPassword
        {
            get { return applicationAuthPassword; }
            set
            {
                if (value == applicationAuthPassword)
                    return;

                SetString("ApplicationAuthPassword", 256, value, ref applicationAuthPassword);

                RaisePropertyChanged("ApplicationAuthPassword");
            }
        }
        [DataMember]
        public string ApplicationAuthClientCertificate
        {
            get { return applicationAuthClientCertificate; }
            set
            {
                if (value == applicationAuthClientCertificate)
                    return;

                SetString("ApplicationAuthClientCertificate", 384, value, ref applicationAuthClientCertificate);

                RaisePropertyChanged("ApplicationAuthClientCertificate");
            }
        }
        [DataMember]
        public string MessageSecuritySigningCertificate
        {
            get { return messageSecuritySigningCertificate; }
            set
            {
                if (value == messageSecuritySigningCertificate)
                    return;

                SetString("MessageSecuritySigningCertificate", 384, value, ref messageSecuritySigningCertificate);

                RaisePropertyChanged("MessageSecuritySigningCertificate");
            }
        }
        [DataMember]
        public string MessageSecurityOfflineEncryptionCertificate
        {
            get { return messageSecurityOfflineEncryptionCertificate; }
            set
            {
                if (value == messageSecurityOfflineEncryptionCertificate)
                    return;

                SetString("MessageSecurityOfflineEncryptionCertificate", 384, value, ref messageSecurityOfflineEncryptionCertificate);

                RaisePropertyChanged("MessageSecurityOfflineEncryptionCertificate");
            }
        }
        [DataMember]
        public string MessageSecurityOfflineDecryptionCertificate
        {
            get { return messageSecurityOfflineDecryptionCertificate; }
            set
            {
                if (value == messageSecurityOfflineDecryptionCertificate)
                    return;

                SetString("MessageSecurityOfflineDecryptionCertificate", 384, value, ref messageSecurityOfflineDecryptionCertificate);

                RaisePropertyChanged("MessageSecurityOfflineDecryptionCertificate");
            }
        }
        [IgnoreDataMember]
        public NetworkCredential NetworkCredentials
        {
            get
            {
                if (refreshNetworkCredentials || null == networkCredentials)
                {
                    if (!string.IsNullOrEmpty(NetworkAuthUserName) && !string.IsNullOrEmpty(NetworkAuthPassword))
                        networkCredentials = new NetworkCredential(NetworkAuthUserName, NetworkAuthPassword, NetworkAuthDomain);
                    else
                        networkCredentials = null;

                    refreshNetworkCredentials = false;
                }

                return (networkCredentials);
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the SyncClientConfiguration Settings class.
        /// </summary>
        public ClientServiceCredentials()
            : base()
        {
        }

        #endregion Constructors

        #region Public methods
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
            if (trackChangesFlag && enableChangeTracking)
                changed = true;

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
