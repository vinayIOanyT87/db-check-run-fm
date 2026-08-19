namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class ProcessVariablesBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public ProcessVariablesBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProcessVariablesBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int ProcessVariableType { get; set; }
        public int InstanceNumber { get; set; }
        public int UnitIndex { get; set; }
        public int UnitType { get; set; }
        public int? OpcConnectionIndex { get; set; }
        public string OpcItemId { get; set; }
        public int? DataType { get; set; }
        public int? ServerEngineeringUnitsIndex { get; set; }
        public short? Quality { get; set; }
        public object SIValue { get; set; }
        public DateTime? DateTimeStamp { get; set; }
        public object Maximum { get; set; }
        public object Minimum { get; set; }
        public bool DataTypeEnabled { get; set; }
        public bool Input { get; set; }
        public bool InputEnabled { get; set; }
        public int? MessageIndex { get; set; }
        public string OpcUrl { get; set; }
        public string OpcProgID { get; set; }
        public string ApplicationStringID { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        /// <param name="unitIndex">The unit that is associated to a process variable.</param>
        public virtual void EnumerateProcessVariableSql(SqlCommand command, int unitIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT PV.*"
                + ", OC.URL AS OpcUrl"
                + ", OC.ProgID AS OpcProgID"
                + ", APS.ID AS ApplicationStringID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblProcessVariables PV"
                        + " LEFT JOIN " + this.SourceDbName + ".dbo.tblOPCConnections OC ON OC.[Index] = PV.OPCConnectionIndex"
                        + " LEFT JOIN " + this.SourceDbName + ".dbo.tblApplicationString APS ON APS.[Index] = PV.MessageIndex";

            // If unit index is -9999, get all the process variable information.
            if(unitIndex == -9999)
            {
                command.CommandText = select + from;
            }
            else
            {
                string where = " WHERE PV.UnitIndex = " + unitIndex;
                command.CommandText = select + from + where;
            }
        }

        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateProcessVariableSql(SqlCommand command)
        {
            this.EnumerateProcessVariableSql(command, -9999);
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index                          = row.IsNull("Index") ? -99 : (int)row["Index"];
            this.ProcessVariableType            = row.IsNull("ProcessVariableType") ? -99 : (int)row["ProcessVariableType"];
            this.InstanceNumber                 = row.IsNull("InstanceNumber") ? -99 : (int)row["InstanceNumber"];
            this.UnitIndex                      = row.IsNull("UnitIndex") ? -99 : (int)row["UnitIndex"];
            this.UnitType                       = row.IsNull("UnitType") ? -99 : (int)row["UnitType"];
            this.OpcConnectionIndex             = row.IsNull("OPCConnectionIndex") ? null : (int?)row["OPCConnectionIndex"];
            this.OpcItemId                      = row.IsNull("OPCItemId") ? string.Empty : (string)row["OPCItemId"];
            this.DataType                       = row.IsNull("DataType") ? null : (int?)row["DataType"];
            this.ServerEngineeringUnitsIndex    = row.IsNull("ServerEngineeringUnitsIndex") ? null : (int?)row["ServerEngineeringUnitsIndex"];
            this.Quality                        = row.IsNull("Quality") ? null : (short?)row["Quality"];
            this.DateTimeStamp                  = row.IsNull("DateTimeStamp") ? null : (DateTime?)row["DateTimeStamp"];
            this.DataTypeEnabled                = row.IsNull("DataTypeEnabled") ? false : (bool)row["DataTypeEnabled"];
            this.Input                          = row.IsNull("Input") ? false : (bool)row["Input"];
            this.InputEnabled                   = row.IsNull("InputEnabled") ? false : (bool)row["InputEnabled"];
            this.MessageIndex                   = row.IsNull("MessageIndex") ? null : (int?)row["MessageIndex"];
            this.OpcUrl                         = row.IsNull("OpcUrl") ? string.Empty : (string)row["OpcUrl"];
            this.OpcProgID                      = row.IsNull("OpcProgID") ? string.Empty : (string)row["OpcProgID"];
            this.ApplicationStringID            = row.IsNull("ApplicationStringID") ? string.Empty : (string)row["ApplicationStringID"];

            if(row.IsNull("Maximum") == false)
            {
                this.Maximum = row["Maximum"];
            }

            if (row.IsNull("Minimum") == false)
            {
                this.Minimum = row["Minimum"];
            }

            if (row.IsNull("SIValue") == false)
            {
                this.SIValue = row["SIValue"];
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index                          = -99;
            this.ProcessVariableType            = -99;
            this.InstanceNumber                 = -99;
            this.UnitIndex                      = -99;
            this.UnitType                       = -99;
            this.OpcConnectionIndex             = null;
            this.OpcItemId                      = string.Empty;
            this.DataType                       = null;
            this.ServerEngineeringUnitsIndex    = null;
            this.Quality                        = null;
            this.SIValue                        = 0.0;
            this.DateTimeStamp                  = null;
            this.Maximum                        = null;
            this.Minimum                        = null;
            this.DataTypeEnabled                = false;
            this.Input                          = false;
            this.InputEnabled                   = false;
            this.MessageIndex                   = null;
            this.OpcUrl                         = string.Empty;
            this.OpcProgID                      = string.Empty;
            this.ApplicationStringID            = string.Empty;
            this.SIValue                        = null;
        }
        #endregion
    }
}
