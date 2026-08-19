namespace BusinessObjects.MigrationObjects
{
    public class Schedule753ToV12Do : ScheduleBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public Schedule753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Schedule753ToV12Do()
        {
        }
        #endregion
    }
}
