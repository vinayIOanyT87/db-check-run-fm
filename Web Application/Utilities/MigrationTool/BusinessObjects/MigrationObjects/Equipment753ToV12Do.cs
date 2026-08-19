namespace BusinessObjects.MigrationObjects
{
    public class Equipment753ToV12Do : EquipmentBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public Equipment753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Equipment753ToV12Do()
        {
        }
        #endregion
    }
}
