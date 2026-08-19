namespace BusinessObjects.MigrationObjects
{
    public class OpcConnection753ToV12Do : OpcConnectionBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public OpcConnection753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OpcConnection753ToV12Do()
        {
        }
        #endregion
    }
}
