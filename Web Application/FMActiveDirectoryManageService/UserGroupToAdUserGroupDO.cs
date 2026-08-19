namespace FMActiveDirectoryManageService
{
    using System;

    [Serializable]
    public class UserGroupToAdUserGroupDO
    {
        public string UserGroupId { get; set; }
        public Guid UserGroupGuid { get; set; }
        public string ActiveDirectoryUserGroupID { get; set; }

        public UserGroupToAdUserGroupDO()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.UserGroupId = string.Empty;
            this.UserGroupGuid = Guid.Empty;
            this.ActiveDirectoryUserGroupID = string.Empty;
        }
    }
}
