namespace BusinessObjects.MigrationObjects
{
    public class Personnel753ToV12Do : PersonnelBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public Personnel753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Personnel753ToV12Do()
        {
        }
        #endregion
    }
}
