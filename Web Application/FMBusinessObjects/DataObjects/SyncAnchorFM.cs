using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

using System.ComponentModel;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    [Serializable]
    [DataContract]
    public class SyncAnchorFM : BaseDataObject, INotifyPropertyChanged
    {
        #region Attributes
        private bool _Changed = false;
        private string _TableName = string.Empty;
        private byte[] _LastReceivedAnchor = null;
        private byte[] _LastReceivedAnchor2 = null;
        private byte[] _LastSentAnchor1 = null;
        private byte[] _LastSentAnchor2 = null;
        private int _CurrentBatchSegment = 0;
        private int _MaxBatchSegment = 0;
        private DateTimeOffset _LastDateRangeStart = DateTimeOffset.MinValue;
        private DateTimeOffset _LastDateRangeEnd = DateTimeOffset.MinValue;
        private DateTimeOffset _LastDateRangeDateSynchronized = DateTimeOffset.MinValue;
        private DateTimeOffset _LastSynchronizedDate = DateTimeOffset.MinValue;
        #endregion Attributes

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

                RaisePropertyChanged("Changed");
            }
        }
        [DataMember]
        public string TableName
        {
            get { return (_TableName); }
            set
            {
                if (value == _TableName)
                    return;

                _TableName = value;

                RaisePropertyChanged("TableName");
            }
        }
        [DataMember]
        public byte[] LastReceivedAnchor
        {
            get { return (_LastReceivedAnchor); }
            set
            {
                if (value == _LastReceivedAnchor)
                    return;

                _LastReceivedAnchor = value;

                RaisePropertyChanged("LastReceivedAnchor");
            }
        }
        [DataMember]
        public byte[] LastReceivedAnchor2
        {
            get { return (_LastReceivedAnchor2); }
            set
            {
                if (value == _LastReceivedAnchor2)
                    return;

                _LastReceivedAnchor2 = value;

                RaisePropertyChanged("LastReceivedAnchor2");
            }
        }
        [DataMember]
        public byte[] LastSentAnchor1
        {
            get { return (_LastSentAnchor1); }
            set
            {
                if (value == _LastSentAnchor1)
                    return;

                _LastSentAnchor1 = value;

                RaisePropertyChanged("LastSentAnchor1");
            }
        }
        [DataMember]
        public byte[] LastSentAnchor2
        {
            get { return (_LastSentAnchor2); }
            set
            {
                if (value == _LastSentAnchor2)
                    return;

                _LastSentAnchor2 = value;

                RaisePropertyChanged("LastSentAnchor2");
            }
        }
        [DataMember]
        public int CurrentBatchSegment
        {
            get { return (_CurrentBatchSegment); }
            set
            {
                if (value == _CurrentBatchSegment)
                    return;

                _CurrentBatchSegment = value;

                RaisePropertyChanged("CurrentBatchSegment");
            }
        }
        [DataMember]
        public int MaxBatchSegment
        {
            get { return (_MaxBatchSegment); }
            set
            {
                if (value == _MaxBatchSegment)
                    return;

                _MaxBatchSegment = value;

                RaisePropertyChanged("MaxBatchSegment");
            }
        }
        [DataMember]
        public DateTimeOffset LastDateRangeStart
        {
            get { return (_LastDateRangeStart); }
            set
            {
                if (value == _LastDateRangeStart)
                    return;

                _LastDateRangeStart = value;

                RaisePropertyChanged("LastDateRangeStart");
            }
        }
        [DataMember]
        public DateTimeOffset LastDateRangeEnd
        {
            get { return (_LastDateRangeEnd); }
            set
            {
                if (value == _LastDateRangeEnd)
                    return;

                _LastDateRangeEnd = value;

                RaisePropertyChanged("LastDateRangeEnd");
            }
        }
        [DataMember]
        public DateTimeOffset LastDateRangeDateSynchronized
        {
            get { return (_LastDateRangeDateSynchronized); }
            set
            {
                if (value == _LastDateRangeDateSynchronized)
                    return;

                _LastDateRangeDateSynchronized = value;

                RaisePropertyChanged("LastDateRangeDateSynchronized");
            }
        }
        [DataMember]
        public DateTimeOffset LastSynchronizedDate
        {
            get { return (_LastSynchronizedDate); }
            set
            {
                if (value == _LastSynchronizedDate)
                    return;

                _LastSynchronizedDate = value;

                RaisePropertyChanged("LastSynchronizedDate");
            }
        }
        #endregion Properties

        #region Constructors
        public SyncAnchorFM()
        {
            this.Reset();
        }
        #endregion Constructors

        #region Public Commands
        public void InsertSQL(SqlCommand cmd)
        {
            cmd.CommandText = "INSERT INTO [sync].[tblSyncAnchor] " +
                    "(SiteID," +
                    "TableName," +
                    "LastReceivedAnchor," +
                    "LastReceivedAnchor2," +
                    "LastSentAnchor1," +
                    "LastSentAnchor2," +
                    "CurrentBatchSegment," +
                    "MaxBatchSegment," +
                    "LastDateRangeStart," +
                    "LastDateRangeEnd," +
                    "LastDateRangeDateSynchronized," +
                    "LastSynchronizedDate) " +
                    "VALUES (" +
                    "@SiteID," +
                    "@TableName," +
                    "@LastReceivedAnchor," +
                    "@LastReceivedAnchor2," +
                    "@LastSentAnchor1," +
                    "@LastSentAnchor2," +
                    "@CurrentBatchSegment," +
                    "@MaxBatchSegment," +
                    "@LastDateRangeStart," +
                    "@LastDateRangeEnd," +
                    "@LastDateRangeDateSynchronized," +
                    "@LastSynchronizedDate )";

            cmd.Parameters.AddWithValue("SiteID", _SiteID);
            cmd.Parameters.AddWithValue("TableName", _TableName);
            cmd.Parameters.AddWithValue("LastReceivedAnchor", _LastReceivedAnchor);
            cmd.Parameters.AddWithValue("LastReceivedAnchor2", _LastReceivedAnchor2);
            cmd.Parameters.AddWithValue("LastSentAnchor1", _LastSentAnchor1);
            cmd.Parameters.AddWithValue("LastSentAnchor2", _LastSentAnchor2);
            cmd.Parameters.AddWithValue("CurrentBatchSegment", _CurrentBatchSegment);
            cmd.Parameters.AddWithValue("MaxBatchSegment", _MaxBatchSegment);
            cmd.Parameters.AddWithValue("LastDateRangeStart", _LastDateRangeStart);
            cmd.Parameters.AddWithValue("LastDateRangeEnd", _LastDateRangeEnd);
            cmd.Parameters.AddWithValue("LastDateRangeDateSynchronized", _LastDateRangeDateSynchronized);
            cmd.Parameters.AddWithValue("LastSynchronizedDate", _LastSynchronizedDate);
        }

        public void UpdateSQL(SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE [sync].[tblSynchronizationAnchor] " +
                    "SET LastReceivedAnchor = @LastReceivedAnchor, " +
                    "LastReceivedAnchor2 = @LastReceivedAnchor2, " +
                    "LastSentAnchor1 = @LastSentAnchor1, " +
                    "LastSentAnchor2 = @LastSentAnchor2, " +
                    "CurrentBatchSegment = @CurrentBatchSegment, " +
                    "MaxBatchSegment = @MaxBatchSegment, " +
                    "LastDateRangeStart = @LastDateRangeStart, " +
                    "LastDateRangeEnd = @LastDateRangeEnd, " +
                    "LastDateRangeDateSynchronized = @LastDateRangeDateSynchronized, " +
                    "LastSynchronizedDate = @LastSynchronizedDate " +
                    " WHERE SiteID = @SiteID " +
                    "  AND TableName = @TableName ";

            cmd.Parameters.AddWithValue("SiteID", _SiteID);
            cmd.Parameters.AddWithValue("TableName", _TableName);
            cmd.Parameters.AddWithValue("LastReceivedAnchor", _LastReceivedAnchor);
            cmd.Parameters.AddWithValue("LastReceivedAnchor2", _LastReceivedAnchor2);
            cmd.Parameters.AddWithValue("LastSentAnchor1", _LastSentAnchor1);
            cmd.Parameters.AddWithValue("LastSentAnchor2", _LastSentAnchor2);
            cmd.Parameters.AddWithValue("CurrentBatchSegment", _CurrentBatchSegment);
            cmd.Parameters.AddWithValue("MaxBatchSegment", _MaxBatchSegment);
            cmd.Parameters.AddWithValue("LastDateRangeStart", _LastDateRangeStart);
            cmd.Parameters.AddWithValue("LastDateRangeEnd", _LastDateRangeEnd);
            cmd.Parameters.AddWithValue("LastDateRangeDateSynchronized", _LastDateRangeDateSynchronized);
            cmd.Parameters.AddWithValue("LastSynchronizedDate", _LastSynchronizedDate);
        }
        #endregion Public Commands

         #region Public methods
        public override void Reset()
        {
            base.Reset();
            _IdentityGuid = Guid.NewGuid();
            _ID = "";
            _SiteID = string.Empty;
            _TableName = string.Empty;
            _LastReceivedAnchor = null;
            _LastReceivedAnchor2 = null;
            _LastSentAnchor1 = null;
            _LastSentAnchor2 = null;
            _CurrentBatchSegment = 0;
            _MaxBatchSegment = 0;
            _LastDateRangeStart = DateTimeOffset.MinValue;
            _LastDateRangeEnd = DateTimeOffset.MinValue;
            _LastDateRangeDateSynchronized = DateTimeOffset.MinValue;
            _LastSynchronizedDate = DateTimeOffset.MinValue;
        }

        public void Load(DataSet set)
        {
            if (set == null)
            {
                throw new ArgumentNullException("set");
            }

            this.Reset();
            DataTable Table = set.Tables[0];

            if (Table.Rows.Count == 0)
            {
                return;
            }

            DataRow Row = Table.Rows[0];

            this._IdentityGuid = DataObject.getGuid(Row["SyncAnchorGuid"]);
            this._SiteID = DataObject.getString(Row["SiteID"]);
            this._TableName = DataObject.getString(Row["TableName"]);
            this._LastReceivedAnchor = DataObject.getValue<byte[]>(Row["LastReceivedAnchor"], null);
            this._LastReceivedAnchor2 = DataObject.getValue<byte[]>(Row["LastReceivedAnchor2"], null);
            this._LastSentAnchor1 = DataObject.getValue<byte[]>(Row["LastSentAnchor1"], null);
            this._LastSentAnchor2 = DataObject.getValue<byte[]>(Row["LastSentAnchor2"], null);
            this._CurrentBatchSegment = DataObject.getValue<int>(Row["CurrentBatchSegment"], 0);
            this._MaxBatchSegment = DataObject.getValue<int>(Row["MaxBatchSegment"], 0);
            this._LastDateRangeStart = DataObject.getValue<DateTimeOffset>(Row["LastDateRangeStart"], DateTimeOffset.MinValue);
            this._LastDateRangeEnd = DataObject.getValue<DateTimeOffset>(Row["LastDateRangeEnd"], DateTimeOffset.MinValue);
            this._LastDateRangeDateSynchronized = DataObject.getValue<DateTimeOffset>(Row["LastDateRangeDateSynchronized"], DateTimeOffset.MinValue);
            this._LastSynchronizedDate = DataObject.getValue<DateTimeOffset>(Row["LastSynchronizedDate"], DateTimeOffset.MinValue);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], BaseDataObject.ADMIN);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
            this._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], BaseDataObject.ADMIN);

            this.RaiseMultiplePropertyChanged("IdentityGuid");
        }

        public void SelectSQL(SqlCommand cmd, bool bInTransaction)
        {
            cmd.CommandText = "SELECT * FROM [sync].[tblSyncAnchor] " + BaseDataObject.SQLUpdateLock(bInTransaction) +
                                    " WHERE SiteID = @SiteID" +
                                    " AND TableName = @TableName ";
            cmd.Parameters.AddWithValue("SyncNodeGuid", _SiteID);
            cmd.Parameters.AddWithValue("TableName", _TableName);
        }
        #endregion

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
