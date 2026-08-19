namespace FuelsManager.Areas.UserAdministrationArea.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.Areas.Controllers;
    using FuelsManager.Areas.UserAdministrationArea.ViewModels;

    using Newtonsoft.Json;

    public class UserConfigurationController : FMBaseController
    {

        /// <summary>
        /// This method returns the string version of the model.
        /// </summary>
        /// <param name="model">The model to serialize.</param>
        /// <returns>Returns the string version of the model.</returns>
        [NonAction]
        public static string SerializeModel(UserAdministrationModel model)
        {
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// This method will deserialize the model string into an object.
        /// </summary>
        /// <param name="modelStr">The string version of the model.</param>
        /// <returns>Returns the model as an object.</returns>
        [NonAction]
        public static UserAdministrationModel DeserializeModel(string modelStr)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var model = JsonConvert.DeserializeObject<UserAdministrationModel>(modelStr, jsonSerializerSettings);

            return model;
        }

        // GET: UserAdministrationArea/UserConfiguration
        public ActionResult UserConfigurationView()
        {
            var userAdminModel = new UserAdministrationModel { UserList = this.GetUserList() };

            try
            {
                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
                userAdminModel.Format = new NumberFormatInfo
                                        {
                                            NumberGroupSizes = site.GetNumberGroupSizes(),
                                            NumberGroupSeparator = site.NumberGroupSeparator,
                                            NumberDecimalSeparator = site.NumberDecimalSeparator,
                                        };

                userAdminModel.ShortDatePattern     = site.ShortDatePattern;
                userAdminModel.TimePattern          = site.TimePattern;
                userAdminModel.TimeZone             = site.TimeZone;
                userAdminModel.DateTimeFormatInfo   = site.GetDateTimeFormatInfo();

                var userDo = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, this.Security.UserGuid));
                if (userDo != null)
                {
                    userAdminModel.UserId       = userDo.ID;
                    userAdminModel.UserName     = userDo.Name;
                    userAdminModel.EmailAddress = userDo.EmailAddress;
                    userAdminModel.UserGuid     = userDo.IdentityGuid;

                    userAdminModel.PermissionGroupModel = this.GetSiteGroupRight(userDo.IdentityGuid);
                }
            }
            catch(Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return this.View(userAdminModel);
        }

        #region
        /// <summary>
        /// This method will get a list of user to place in the User Configuration dropdown.
        /// Currently, the list is only going to be the current user.
        /// </summary>
        /// <returns></returns>
        private List<UserModel> GetUserList()
        {
            var userList = new List<UserModel>();
            var userModel = new UserModel { UserGuid = this.Security.UserGuid, UserId = this.Security.UserID };
            userList.Add(userModel);

            return userList;
        }

        /// <summary>
        /// This method will retrieve the sites/groups/rights collections and return the Site Group Right
        /// model to the UI.
        /// </summary>
        /// <param name="userGuid">The user Guid to retrieve the information</param>
        /// <returns>Return the SiteGroupRight model.</returns>
        private SiteGroupRightModel GetSiteGroupRight(Guid userGuid)
        {
            var siteGroupRightModel = new SiteGroupRightModel();

            var groupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(
                                                    x => x.EnumerateByUserForSiteHierarchy(this.Security, userGuid, this.Security.SiteGuid));

            if (groupCollection == null || groupCollection.Count == 0)
            {
                return siteGroupRightModel;
            }

            Guid previousSiteGuid = Guid.Empty;
            var siteModel = new SiteModel();

            foreach (GroupClass group in groupCollection)
            {
                if (group.SiteGuid != previousSiteGuid)
                {
                    previousSiteGuid = group.SiteGuid;
                    siteModel = new SiteModel { SiteGuid = @group.SiteGuid, SiteName = @group.SiteID };
                    siteGroupRightModel.SiteGroupRightList.Add(siteModel);
                }

                var groupModel = new GroupModel { GroupGuid = @group.IdentityGuid, GroupName = @group.ID };

                foreach (RightClass right in group.RightCollectionExt)
                {
                    if (string.IsNullOrEmpty(right.Name) == false)
                    {
                        var rightEnglishName = SecurityClass.RightID((RIGHT)right.RightIndex);
                        var rightModel = new RightModel
                        {
                            Description = right.Description,
                            Name = rightEnglishName,
                            RightIndex = right.RightIndex
                        };

                        groupModel.RightList.Add(rightModel);
                    }
                }

                siteModel.GroupList.Add(groupModel);
            }

            return siteGroupRightModel;
        }
        #endregion
    }
}