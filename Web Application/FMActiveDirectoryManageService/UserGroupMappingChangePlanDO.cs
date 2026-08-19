namespace FMActiveDirectoryManageService
{
    using System;

    [Serializable]
    public class UserGroupMappingChangePlanDO
    {
        #region Data members
        public enum MappingChangeActionTypes
        {
            NoAction,
            Add,
            Delete,
            DeleteMappingMissingUser
        }

        private int mappingChangeActionInt;
        private MappingChangeActionTypes mappingChangeAction;
        #endregion

        #region Properties
        public string UserId { get; set; }
        public Guid UserGuid { get; set; }
        public Guid SiteGuid { get; set; }
        public Guid UserGroupGuid { get; set; }
        public string ErrorMessage { get; set; }

        public int MappingChangeActionInt
        {
            get { return this.mappingChangeActionInt; }
            set
            {
                this.mappingChangeActionInt = value;
                switch (value)
                {
                    case 0:
                        this.mappingChangeAction = MappingChangeActionTypes.NoAction;
                        break;
                    case 1:
                        this.mappingChangeAction = MappingChangeActionTypes.Add;
                        break;
                    case 2:
                        this.mappingChangeAction = MappingChangeActionTypes.Delete;
                        break;
                    case 3:
                        this.mappingChangeAction = MappingChangeActionTypes.DeleteMappingMissingUser;
                        break;
                    default:
                        this.mappingChangeAction = MappingChangeActionTypes.NoAction;
                        break;
                }
            }
        }

        public MappingChangeActionTypes MappingChangeAction
        {
            get { return this.mappingChangeAction; }
            set
            {
                this.mappingChangeAction = value;
                this.mappingChangeActionInt = (int)value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public UserGroupMappingChangePlanDO()
        {
            this.Initialize();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.UserId = string.Empty;
            this.UserGuid = Guid.Empty;
            this.MappingChangeActionInt = 0;
            this.SiteGuid = Guid.Empty;
            this.UserGroupGuid = Guid.Empty;
            this.ErrorMessage = string.Empty;
        }
        #endregion
    }
}
