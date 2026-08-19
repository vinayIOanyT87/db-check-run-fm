namespace BusinessObjects.MigrationObjects
{
    public class TankGroups753ToV12Do :TankGroupsBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public TankGroups753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public TankGroups753ToV12Do()
        {
        }
        #endregion
    }
}
