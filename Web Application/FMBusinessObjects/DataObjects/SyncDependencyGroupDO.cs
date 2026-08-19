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
    [KnownType(typeof(SyncDependencyGroupDO))]
    public class SyncDependencyGroupCollection : List<SyncDependencyGroupDO>
    {
    }

    [XmlType("SyncDependencyGroup")]
    [DataContract]
    [Serializable]
    public class SyncDependencyGroupDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data members
        private string _FriendlyName = string.Empty;
        private string _LongDescription = string.Empty;
        private int _DependencyLevel = 1;
        #endregion Data members

        #region Properties
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
        public int DependencyLevel
        {
            get { return _DependencyLevel; }
            set
            {
                if (value == _DependencyLevel)
                    return;

                _DependencyLevel = value;

                RaisePropertyChanged("DependencyLevel");
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the Line Item DO class.
        /// </summary>
        public SyncDependencyGroupDO()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public Methods
        public override void Reset()
        {
            base.Reset();
            this._IdentityGuid = Guid.NewGuid();
            this._FriendlyName = string.Empty;
            this._LongDescription = string.Empty;
            this._DependencyLevel = 1;
            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }

        public void Load(DataRow row)
        {
            this._IdentityGuid = DataObject.getGuid(row["SynchronizationProfileGuid"]);
            this._ID = DataObject.getString(row["SynchronizationProfileID"]);
            this._FriendlyName = DataObject.getString(row["FriendlyName"]);
            this._LongDescription = DataObject.getString(row["LongDescription"]);
            this._DependencyLevel = DataObject.getInt(row["DependencyLevel"]);

            this._CreatedBy = DataObject.getString(row["CreatedBy"]);
            this._UpdatedBy = DataObject.getString(row["UpdatedBy"]);
        }
        #endregion Public Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #region STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES
        protected void RaisePropertyChanged(string propertyName)
        {
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
