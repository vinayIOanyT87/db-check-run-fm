namespace BusinessObjects.MigrationObjects
{
    public class EquipmentType753ToV12Do : EquipmentTypeBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public EquipmentType753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public EquipmentType753ToV12Do()
        {
        }
        #endregion
    }
}
