namespace BusinessObjects.MigrationObjects
{
    public class ProcessVariables753ToV12Do : ProcessVariablesBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public ProcessVariables753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ProcessVariables753ToV12Do()
        {
        }
        #endregion
    }
}
