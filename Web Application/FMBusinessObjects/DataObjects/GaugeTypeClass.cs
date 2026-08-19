using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
    [Serializable]
    [CollectionDataContract]
    public class GaugeTypeCollectionClass : List<GaugeTypeClass> { }

    [DataContract]
    [Serializable]
    public class GaugeTypeClass : BaseDataObject
    {
        #region Attributes
        [DataMember]
        int _GaugeIndex { get; set; }

        [DataMember]
        string _Name { get; set; }

        [DataMember]
        int _Type { get; set; }

        [DataMember]
        double? _DeltaTemp { get; set; }

        [DataMember]
        double? _Threshold { get; set; }

        [DataMember]
        double? _CertificationLeakRate { get; set; }

        [DataMember]
        int? _MinHours { get; set; }

        #endregion Attributes;

        #region Properties

        public Guid GaugeTypeGuid
        {
            get
            {
                return this.IdentityGuid;
            }

            set
            {
                this.IdentityGuid = value;
            }
        }

        public int GaugeIndex { get { return _GaugeIndex; } }

        public string Name { get { return _Name; } }

        public int Type { get { return _Type; } }

        public double? DeltaTemp { get { return _DeltaTemp; } }

        public double? Threshold { get { return _Threshold; } }

        public double? CertificationLeakRate { get { return _CertificationLeakRate; } }

        public int? MinHours { get { return _MinHours; } }

        #endregion Properties

        #region Contructor
        public GaugeTypeClass()
        {
            this.init();
        }

        /// <summary>
        /// This is the deserialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //public LedgerDO ( System.Runtime.Serialization.SerializationInfo info,
        //   System.Runtime.Serialization.StreamingContext context )
        //{
        //}

        public GaugeTypeClass(System.Data.DataSet dataSet)
        {
            init();
            this.Load(dataSet);
        }
        #endregion

        #region Properties

        #endregion



        #region Private Methods
        public void SelectSQL(SqlCommand cmd, Guid identityGuid)
        {
            cmd.CommandText = "SELECT * FROM tblGaugeType WHERE GaugeTypeGuid = @GaugeTypeGuid";
            cmd.Parameters.Add("@GaugeTypeGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@GaugeTypeGuid"].Value = identityGuid;
        }


        public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
        {
            cmd.CommandText = "SELECT * FROM tblGaugeType" +
                              " ORDER BY GaugeTypeIndex";
        }


        public void SelectByIDSQL(SqlCommand cmd, string id)
        {
            cmd.CommandText = "SELECT * FROM tblGaugeType " + 
                " WHERE  ID = @ID";

            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 32);
            cmd.Parameters["@ID"].Value = id;
        }

        public void SelectByIindexSQL(SqlCommand cmd, int gaugeTypeIndex)
        {
            cmd.CommandText = "SELECT * FROM tblGaugeType " +
                " WHERE  GaugeTypeIndex = @GaugeTypeIndex";

            cmd.Parameters.Add("@GaugeTypeIndex", SqlDbType.Int);
            cmd.Parameters["@GaugeTypeIndex"].Value = gaugeTypeIndex;
        }


        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="o">
        /// The object to load.
        /// </param>
        public override void Load(object o)
        {
            this.Reset();

            if (typeof(DataSet).IsInstanceOfType(o))
            {
                DataSet Set = (DataSet)o;
                DataTable Table = Set.Tables[0];
                if (Table.Rows.Count == 0)
                    return;

                DataRow Row = Table.Rows[0];

                _IdentityGuid = DataObject.getValue<Guid>(Row["GaugeTypeGuid"], Guid.Empty);
                _GaugeIndex = DataObject.getValue<int>(Row["GaugeTypeIndex"], -1);
                _ID = DataObject.getValue<string>(Row["ID"], "");
                _Name = DataObject.getValue<string>(Row["Name"], "");
                _Type = DataObject.getValue<int>(Row["Type"], 1);
                _DeltaTemp = DataObject.getOptionalDouble(Row["DeltaTemp"]);
                _Threshold = DataObject.getOptionalDouble(Row["Threshold"]);
                _CertificationLeakRate = DataObject.getOptionalDouble(Row["CertificationLeakRate"]);
                _MinHours = DataObject.getOptionalInt(Row["MinHours"]);
                _CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
                _CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
                _UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
                _UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
            }

        }

        public override void Reset()
        {
            base.Reset();
            _GaugeIndex = -1;
            _ID = string.Empty;
            _Name = string.Empty;
            _Type = 1;
            _DeltaTemp = null;
            _Threshold = null;
            _CertificationLeakRate = null;
            _MinHours = null;
        }

        /// <summary>
        /// This methods initializes the Ledger DO object.
        /// </summary>
        private void init()
        {

        }
        #endregion
    }
}
