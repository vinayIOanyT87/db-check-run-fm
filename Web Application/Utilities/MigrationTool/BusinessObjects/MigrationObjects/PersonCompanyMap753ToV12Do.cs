namespace BusinessObjects.MigrationObjects
{
    public class PersonCompanyMap753ToV12Do : PersonCompanyMapBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public PersonCompanyMap753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PersonCompanyMap753ToV12Do()
        {
        }
        #endregion
    }
}
