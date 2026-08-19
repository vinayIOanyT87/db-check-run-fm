namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class ScheduleBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public ScheduleBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ScheduleBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int EntityIndex { get; set; }
        public int Type { get; set; }
        public int Day { get; set; }
        public bool Enabled { get; set; }
        public DateTime OpeningTime { get; set; }
        public DateTime ClosingTime { get; set; }
        public bool EndOfDayEnabled { get; set; }
        public DateTime EndOfDayTime { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateScheduleSql(SqlCommand command, int scheduleType)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT SC.*";
            string from = " FROM " + this.SourceDbName + ".dbo.tblSchedules SC";
            string where = " WHERE SC.Type = " + scheduleType;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index              = row.IsNull("Index") ? -99 : (int)row["Index"];
            this.EntityIndex        = row.IsNull("EntityIndex") ? -99 : (int)row["EntityIndex"];
            this.Type               = row.IsNull("Type") ? -99 : (int)row["Type"];
            this.Day                = row.IsNull("Day") ? 0 : (int)row["Day"];
            this.Enabled            = row.IsNull("Enabled") ? false : (bool)row["Enabled"];
            this.OpeningTime        = row.IsNull("OpeningTime") ? DateTime.Now : (DateTime)row["OpeningTime"];
            this.ClosingTime        = row.IsNull("ClosingTime") ? DateTime.Now : (DateTime)row["ClosingTime"];
            this.EndOfDayEnabled    = row.IsNull("EndOfDayEnabled") ? false : (bool)row["EndOfDayEnabled"];
            this.EndOfDayTime       = row.IsNull("EndOfDayTime") ? DateTime.Now : (DateTime)row["EndOfDayTime"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index              = -99;
            this.EntityIndex        = -99;
            this.Type               = -99;
            this.Day                = 0;
            this.Enabled            = false;
            this.OpeningTime        = DateTime.Now;
            this.ClosingTime        = DateTime.Now;
            this.EndOfDayEnabled    = false;
            this.EndOfDayTime       = DateTime.Now;
    }
        #endregion
    }
}
