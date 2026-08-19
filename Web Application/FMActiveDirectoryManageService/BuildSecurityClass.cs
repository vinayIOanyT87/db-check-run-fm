namespace FMActiveDirectoryManageService
{
    using System;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    public class BuildSecurityClass
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public BuildSecurityClass()
        {
            
        }
        #endregion

        /// <summary>
        /// This method will build the security object in order to use
        /// for FM Business Services.
        /// </summary>
        /// <returns>Returns a new security object.</returns>
        public SecurityClass BuildSecurity(Guid siteGuid)
        {
            var newSecurity = new SecurityClass
            {
                UserGuid = Guids.UserAdminGuid,
                LoginSiteGuid = siteGuid,
                SiteGuid = siteGuid,
                UserID = "Administrator"
            };

            newSecurity.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
            newSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
            newSecurity.AddRight(RIGHT.MODIFY_USER_GROUPS);
            newSecurity.AddRight(RIGHT.VIEW_USER_GROUPS);
            newSecurity.AddRight(RIGHT.MODIFY_USERS);
            newSecurity.AddRight(RIGHT.VIEW_USERS);

            return newSecurity;
        }
    }
}
