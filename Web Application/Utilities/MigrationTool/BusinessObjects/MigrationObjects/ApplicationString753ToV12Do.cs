namespace BusinessObjects.MigrationObjects
{
    public class ApplicationString753ToV12Do : ApplicationStringBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public ApplicationString753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ApplicationString753ToV12Do()
        {
        }
        #endregion
    }
}
