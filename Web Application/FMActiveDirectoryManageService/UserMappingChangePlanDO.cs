namespace FMActiveDirectoryManageService
{
    using System;

    [Serializable]
    public class UserMappingChangePlanDO
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
        public int RunningIndex { get; set; }
        public string UserId { get; set; }
        public Guid UserGuid { get; set; }
        public Guid AssignedFromSiteGuid { get; set; }
        public Guid AssignedToSiteGuid { get; set; }
        public int AssignedToJierarchyLevel { get; set; }
        public string ErrorMessage { get; set; }

        public int MappingChangeActionInt
        {
            get  { return this.mappingChangeActionInt; }
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
        public UserMappingChangePlanDO()
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
            this.RunningIndex = 0;
            this.UserId = string.Empty;
            this.UserGuid = Guid.Empty;
            this.MappingChangeActionInt = 0;
            this.AssignedFromSiteGuid = Guid.Empty;
            this.AssignedToSiteGuid = Guid.Empty;
            this.AssignedToJierarchyLevel = 0;
            this.ErrorMessage = string.Empty;
        }
        #endregion
    }
}
