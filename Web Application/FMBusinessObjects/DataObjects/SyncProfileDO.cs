using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using System.ComponentModel;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SyncProfileDO))]
    public class SyncProfileCollection : List<SyncProfileDO>
    {
    }

    [XmlType("SyncProfile")]
    [DataContract]
    [Serializable]
    public class SyncProfileDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data members
        private bool _Changed = false;
        private string _FriendlyName = string.Empty;
        private string _LongDescription = string.Empty;

        private BaseCollections _SyncScopes = null;
        #endregion Data members

        #region Properties
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
        public string FriendlyName
        {
            get { return _FriendlyName; }
            set
            {
                if (value == _FriendlyName)
                    return;

                SetString("FriendlyName", 100, value, ref _FriendlyName);

                RaisePropertyChanged("FriendlyName");
            }
        }
        [DataMember]
        public string LongDescription
        {
            get { return _LongDescription; }
            set
            {
                if (value == _LongDescription)
                    return;

                SetString("LongDescription", 1024, value, ref _LongDescription);

                RaisePropertyChanged("LongDescription");
            }
        }
        [DataMember]
        public BaseCollections SyncScopes
        {
            get { return this._SyncScopes; }
            set { this._SyncScopes = value; }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the Line Item DO class.
        /// </summary>
        public SyncProfileDO()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public Methods
        public override void Reset()
        {
            base.Reset();
            this._Changed = false;

            this._IdentityGuid = Guid.NewGuid();
            this._FriendlyName = string.Empty;
            this._LongDescription = string.Empty;
            this._SyncScopes = new BaseCollections();
            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }

        public void Load(DataRow row)
        {
            this._IdentityGuid = DataObject.getGuid(row["SynchronizationProfileGuid"]);
            this._ID = DataObject.getString(row["SynchronizationProfileID"]);
            this._FriendlyName = DataObject.getString(row["FriendlyName"]);
            this._LongDescription = DataObject.getString(row["LongDescription"]);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
            this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);

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
            if (trackChangesFlag)
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
