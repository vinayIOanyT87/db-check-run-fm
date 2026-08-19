namespace BusinessObjects.MigrationObjects
{
    public class ProductMap753ToV12Do : ProductMapBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public ProductMap753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ProductMap753ToV12Do()
        {
        }
        #endregion
    }
}
