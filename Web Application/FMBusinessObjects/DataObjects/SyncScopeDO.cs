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
    [KnownType(typeof(SyncScopeDO))]
    public class SyncScopeCollection : List<SyncScopeDO>
    {
    }

    [XmlType("SyncScope")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(SYNCSCOPETYPE))]
    [KnownType(typeof(SyncProfileDO))]
    [KnownType(typeof(SyncTableToScopeMapDO))]
    [KnownType(typeof(SyncTableToScopeMapCollection))]
    public class SyncScopeDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members
        private bool _Changed = false;
        private SYNCSCOPETYPE _SyncScopeTypeIndex = SYNCSCOPETYPE.UKNOWN;
        private string _FriendlyName = string.Empty;
        private string _LongDescription = string.Empty;
        private Guid _SyncProfileGuid = Guid.Empty;
        private int _SyncOrder = 0;
		private bool _SyncSinglePass = false;

        private SyncTableToScopeMapCollection _SyncScopeTables = null;
        #endregion Data Members

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
        public SYNCSCOPETYPE SyncScopeTypeIndex
        {
            get { return _SyncScopeTypeIndex; }
            set
            {
                if (value == _SyncScopeTypeIndex)
                    return;

                _SyncScopeTypeIndex = value;

                RaisePropertyChanged("SyncScopeTypeIndex");
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
        public Guid SyncProfileGuid
        {
            get { return _SyncProfileGuid; }
            set
            {
                if (value == _SyncProfileGuid)
                    return;

                _SyncProfileGuid = value;

                RaisePropertyChanged("SyncProfileGuid");
            }
        }
        [DataMember]
        public int SyncOrder
        {
            get { return _SyncOrder; }
            set
            {
                if (value == _SyncOrder)
                    return;

                _SyncOrder = value;

                RaisePropertyChanged("SyncOrder");
            }
        }
		[DataMember]
		public bool SyncSinglePass
		{
			get { return _SyncSinglePass; }
			set
			{
				if (value == _SyncSinglePass)
					return;

				_SyncSinglePass = value;

				RaisePropertyChanged("SyncSinglePass");
			}
		}
		[DataMember]
        public SyncTableToScopeMapCollection SyncScopeTables
        {
            get
            {
                return _SyncScopeTables;
            }
            set
            {
                _SyncScopeTables = value;
            }
        }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the Line Item DO class.
        /// </summary>
        public SyncScopeDO()
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

            this._IdentityGuid = Guid.NewGuid();
            this._SyncScopeTypeIndex = SYNCSCOPETYPE.UKNOWN;
            this._FriendlyName = string.Empty;
            this._LongDescription = string.Empty;
            this._SyncProfileGuid = Guid.Empty;
            this._SyncOrder = 0;
	        this._SyncSinglePass = false;
            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;

            this._SyncScopeTables = new SyncTableToScopeMapCollection();
        }
        public void Load(DataRow row)
        {
            this._IdentityGuid = DataObject.getGuid(row["SyncScopeGuid"]);
            this._ID = DataObject.getString(row["SyncScopeID"]);
            this._SyncScopeTypeIndex = (SYNCSCOPETYPE)DataObject.getInt(row["SyncScopeTypeIndex"]);
            this._FriendlyName = DataObject.getString(row["FriendlyName"]);
            this._LongDescription = DataObject.getString(row["LongDescription"]);
            this._SyncProfileGuid = DataObject.getGuid(row["SyncProfileGuid"]);
            this._SyncOrder = DataObject.getInt(row["SyncOrder"]);
	        this._SyncSinglePass = DataObject.getBool(row["SyncSinglePass"]);

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
