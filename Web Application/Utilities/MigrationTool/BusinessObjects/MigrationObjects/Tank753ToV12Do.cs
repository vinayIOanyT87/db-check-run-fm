namespace BusinessObjects.MigrationObjects
{
    public class Tank753ToV12Do : TankBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public Tank753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Tank753ToV12Do()
        {
        }
        #endregion
    }
}
