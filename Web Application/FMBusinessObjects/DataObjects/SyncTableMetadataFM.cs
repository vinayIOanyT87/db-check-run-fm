using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    [Serializable]
    [DataContract]
    public class SyncTableMetadataFM
    {
        #region Attributes
        private string _TableName = "";
        private SYNCDIRECTION _SynchronizationDirection = SYNCDIRECTION.DOWNLOADONLY;
        private byte[] _LastReceivedAnchor = null;
        private byte[] _LastSentAnchor = null;
        private byte[] _MaxReceivedAnchor = null;
        private byte[] _MaxSentAnchor = null;
        #endregion Attributes

        #region Properties

        #region TableName property
        [DataMember]
        public string TableName
        {
            get { return (this._TableName); }
            set { this._TableName = value; }
        }
        #endregion TableName property

        #region SynchronizationDirection property
        [DataMember]
        public SYNCDIRECTION SynchronizationDirection
        {
            get { return (this._SynchronizationDirection); }
            set { this._SynchronizationDirection = value; }
        }
        #endregion SynchronizationDirection property

        #region LastReceivedAnchor property
        [DataMember]
        public byte[] LastReceivedAnchor
        {
            get { return (this._LastReceivedAnchor); }
            set { this._LastReceivedAnchor = value; }
        }
        #endregion LastReceivedAnchor property

        #region LastSentAnchor property
        [DataMember]
        public byte[] LastSentAnchor
        {
            get { return (this._LastSentAnchor); }
            set { this._LastSentAnchor = value; }
        }
        #endregion LastSentAnchor property

        #region MaxReceivedAnchor property
        [DataMember]
        public byte[] MaxReceivedAnchor
        {
            get { return (this._MaxReceivedAnchor); }
            set { this._MaxReceivedAnchor = value; }
        }
        #endregion MaxReceivedAnchor property

        #region MaxSentAnchor property
        [DataMember]
        public byte[] MaxSentAnchor
        {
            get { return (this._MaxSentAnchor); }
            set { this._MaxSentAnchor = value; }
        }
        #endregion MaxSentAnchor property

        #endregion Properties

        #region Constructor
        public SyncTableMetadataFM()
        {
            this._TableName = string.Empty;
        }
        public SyncTableMetadataFM(string tableName)
        {
            this._TableName = tableName;
        }

        public SyncTableMetadataFM(SyncTableMetadataFM value)
        {
            if (null != value.TableName)
            {
                this._TableName = (string)(value.TableName.Clone());
            }

            this._SynchronizationDirection = value.SynchronizationDirection;

            if (null != value.LastReceivedAnchor)
            {
                this._LastReceivedAnchor = (byte[])(value.LastReceivedAnchor.Clone());
            }

            if (null != value.LastSentAnchor)
            {
                this._LastSentAnchor = (byte[])(value._LastSentAnchor.Clone());
            }

            if (null != value.MaxReceivedAnchor)
            {
                this._MaxReceivedAnchor = (byte[])(value.MaxReceivedAnchor.Clone());
            }

            if (null != value.MaxSentAnchor)
            {
                this._MaxSentAnchor = (byte[])(value._MaxSentAnchor.Clone());
            }
        }

        #endregion Constructor
    }
}
