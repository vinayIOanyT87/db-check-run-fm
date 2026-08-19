using FMBusinessObjects.DataObjects;

namespace BusinessObjects.MigrationObjects
{
    public class UserDataFields753ToV12Do : UserDataFieldsBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor.
        /// </summary>
        public UserDataFields753ToV12Do(string sourceDbName, string targetDbName) : base(sourceDbName, targetDbName)
        {
            base.Init();
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public UserDataFields753ToV12Do()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method will return the 7.5.3 entity type as a string.
        /// </summary>
        /// <param name="entityType">The V12 entity type</param>
        /// <returns>Returns an entity type string</returns>
        public string GetEntityTypeAsString(ENTITY_TYPE entityType)
        {
            switch(entityType)
            {
                case ENTITY_TYPE.EQUIPMENT:
                    return EntityTypeEquipment;
                case ENTITY_TYPE.PERSONNEL:
                    return EntityTypePersonnel;
                case ENTITY_TYPE.COMPANY:
                    return EntityTypeCompanies;
                case ENTITY_TYPE.PRODUCT:
                    return EntityTypeProducts;
                case ENTITY_TYPE.SITE:
                    return EntityTypeSites;
                case ENTITY_TYPE.TRANSACTION_ALIAS:
                    return EntityTypeTransactionAliases;
                default:
                    return string.Empty;              
            }
        }
        #endregion
    }
}
