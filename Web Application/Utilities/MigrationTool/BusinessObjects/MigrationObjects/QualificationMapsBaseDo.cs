namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class QualificationMapsBaseDo
    {
        #region Data members
        public enum QualificationMapTypes 
        { 
            COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY = 0
            , EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT = 1
            , EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT = 2
            , PERSON_QUALIFICATION_TO_PERSON = 3
            , PERSON_LICENSE_TO_PERSON = 4
            , PERSON_TRAINING_TO_PERSON = 5
            , PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE = 6
            , PERSON_TRAINING_TO_EQUIPMENT_TYPE = 7
            , PERSON_QUALIFICATION_TO_STATION = 8
            , PERSON_TRAINING_TO_STATION = 9
            , EQUIPMENT_TEST_AND_INSPECTION_TO_STATION = 10
            , PERSON_LICENSE_TO_STATION = 11
            , MAX_QUALIFICATION_MAP_TYPE = 12
        };
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public QualificationMapsBaseDo()
        {
            this.Init();
        }

        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        /// <param name="sourceDbName"></param>
        /// <param name="targetDbName"></param>
        public QualificationMapsBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }
        #endregion

        #region Properties
        // Index is the person/equipment/etc. index.
        public int Index { get; set; }

        // Assigned Index is the qualification index.
        public int AssignedIndex { get; set; }
        public int Type { get; set; }
        public int Sequence { get; set; }
        public string Instructor { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime? DateDue { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Id { get; set; }
        public string Rating { get; set; }
        public bool HistoricalRecord { get; set; }
        public string CompanyId { get; set; }
        public string EquipmentId { get; set; }
        public string PersonId { get; set; }
        public string QualificationId { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// This method enumerates the qualification map records.
        /// </summary>
        /// <param name="command">The SQL command to populate.</param>
        /// <param name="qualificationIndex"></param>
        /// <param name="entityIndex"></param>
        public virtual void EnumerateQuantityMapsSql(SqlCommand command)
        {
            string select = " SELECT QM.[Index] AS EntityIndex"
                + ", QM.AssignedIndex AS QualificationIndex"
                + ", QM.Type AS QualificationMapType"
                + ", QM.Sequence"
                + ", QM.Instructor"
                + ", QM.DateCompleted"
                + ", QM.DateDue"
                + ", QM.ExpirationDate"
                + ", QM.Rating"
                + ", QM.HistoricalRecord"
                + ", QM.ID AS QualificationMapID"
                + ", (SELECT DISTINCT ID FROM " + this.SourceDbName + ".dbo.tblCompanies WHERE CompanyIndex = QM.[Index]) AS CompanyID"
                + ", (SELECT DISTINCT ID FROM " + this.SourceDbName + ".dbo.tblEquipment WHERE [Index] = QM.[Index]) AS EquipmentID"
                + ", (SELECT DISTINCT PersonID FROM " + this.SourceDbName + ".dbo.tblPersonnel WHERE PersonIndex = QM.[Index]) AS PersonID"
                + ", Q.ID AS QualificationID";

            string from = " FROM " + this.SourceDbName + ".dbo.tblQualificationsMap QM"
                        + " LEFT JOIN " + this.SourceDbName + ".dbo.tblQualifications Q ON Q.[Index] = QM.AssignedIndex";

            command.CommandText = select + from;
        }

        /// <summary>
        /// This method will load the qualification map records.
        /// </summary>
        /// <param name="row">The row to load.</param>
        public virtual void Load(DataRow row)
        {
            this.Index              = row.IsNull("EntityIndex") ? -99 : (int)row["EntityIndex"];
            this.AssignedIndex      = row.IsNull("QualificationIndex") ? -99 : (int)row["QualificationIndex"];
            this.Type               = row.IsNull("QualificationMapType") ? -99 : (int)row["QualificationMapType"];
            this.Sequence           = row.IsNull("Sequence") ? -99 : (int)row["Sequence"];
            this.Id                 = row.IsNull("QualificationMapID") ? string.Empty : (string)row["QualificationMapID"];
            this.Instructor         = row.IsNull("Instructor") ? string.Empty : (string)row["Instructor"];
            this.DateCompleted      = row.IsNull("DateCompleted") ? null : (DateTime?)row["DateCompleted"];
            this.DateDue            = row.IsNull("DateDue") ? null : (DateTime?)row["DateDue"];
            this.ExpirationDate     = row.IsNull("ExpirationDate") ? null : (DateTime?)row["ExpirationDate"];
            this.Rating             = row.IsNull("Rating") ? string.Empty : (string)row["Rating"];
            this.HistoricalRecord   = row.IsNull("HistoricalRecord") ? false : (bool)row["HistoricalRecord"];
            this.CompanyId          = row.IsNull("CompanyID") ? string.Empty : (string)row["CompanyID"];
            this.EquipmentId        = row.IsNull("EquipmentID") ? string.Empty : (string)row["EquipmentID"];
            this.PersonId           = row.IsNull("PersonID") ? string.Empty : (string)row["PersonID"];
            this.QualificationId    = row.IsNull("QualificationID") ? string.Empty : (string)row["QualificationID"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index              = -99;
            this.AssignedIndex      = -99;
            this.Type               = -99;
            this.Sequence           = -99;
            this.Instructor         = string.Empty;
            this.DateCompleted      = null;
            this.DateDue            = null;
            this.ExpirationDate     = null;
            this.Id                 = string.Empty;
            this.HistoricalRecord   = false;
            this.CompanyId          = string.Empty;
            this.EquipmentId        = string.Empty;
            this.PersonId           = string.Empty;
            this.QualificationId    = string.Empty;
        }
        #endregion
    }
}
