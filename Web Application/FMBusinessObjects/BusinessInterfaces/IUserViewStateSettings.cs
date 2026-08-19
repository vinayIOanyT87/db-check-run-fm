namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;
    [ServiceContract]
    public interface IUserViewStateSettings
    {
        /// <summary>
        /// Adds the specified user view state setting.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="setting">The user view state setting.</param>
        /// <returns></returns>
        [OperationContract]
        Guid Add(SecurityClass security, UserViewStateSetting setting);

        /// <summary>
        /// Purges the specified user view state setting from the database.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="settingGuid">The user view state setting unique identifier.</param>
        /// <returns></returns>
        [OperationContract]
        void Purge(SecurityClass security, Guid settingGuid);

        /// <summary>
        /// modifies the specified user view state setting.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="setting">The user view state setting.</param>
        /// <returns></returns>
        [OperationContract]
        void Modify(SecurityClass security, UserViewStateSetting setting);

        /// <summary>
        /// Gets the specified security.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="settingGuid">The user view state setting unique identifier.</param>
        /// <returns></returns>
        [OperationContract]
        UserViewStateSetting Get(SecurityClass security, Guid settingGuid);

        /// <summary>
        /// Enumerates the user view state settings based on the unique site guid.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="siteGuid">The site unique identifier.</param>
        /// <returns>
        /// A collection of UserViewStateSetting objects.
        /// </returns>
        [OperationContract]
        UserViewStateSettingCollection EnumerateBySite(SecurityClass security, Guid siteGuid);
        /// <summary>
        /// Enumerates the user view state settings based on the unique user guid.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="userGuid">The user unique identifier.</param>
        /// <returns>
        /// A collection of UserViewStateSetting objects.
        /// </returns>
        [OperationContract]
        UserViewStateSettingCollection EnumerateByUser(SecurityClass security, Guid userGuid);

        /// <summary>
        /// Enumerates the user view state settings based on the unique user guid.
        /// </summary>
        /// <param name="security">The security.</param>
        ///  /// <param name="siteGuid">The site unique identifier.</param>
        /// <param name="userGuid">The user unique identifier.</param>
        /// <returns>
        /// A collection of UserViewStateSetting objects.
        /// </returns>
        [OperationContract]
        UserViewStateSettingCollection EnumerateBySiteAndUser(SecurityClass security, Guid siteGuid, Guid userGuid);

        /// <summary>
        /// Enumerates the user view state settings based on the unique user guid.
        /// </summary>
        /// <param name="security">The security.</param>
        ///  /// <param name="siteGuid">The site unique identifier.</param>
        /// <param name="userGuid">The user unique identifier.</param>
        /// <param name="viewID">The view identifier.</param>
        /// <returns>
        /// A collection of UserViewStateSetting objects.
        /// </returns>
        [OperationContract]
        UserViewStateSettingCollection EnumerateBySiteUserClientIpAddressWindowNameAndViewID(
            SecurityClass security,
            Guid siteGuid,
            Guid userGuid,
				string windowName,
            string viewID);
    }
}
