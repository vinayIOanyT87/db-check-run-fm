namespace BusinessObjects.MigrationObjects
{
    public class Stations753ToV12Do : StationsBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public Stations753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Stations753ToV12Do()
        {
        }
        #endregion
    }
}
