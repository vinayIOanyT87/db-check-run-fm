namespace BusinessObjects.MigrationObjects
{
    public class LoadArm753ToV12Do : LoadArmBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public LoadArm753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LoadArm753ToV12Do()
        {
        }
        #endregion
    }
}
