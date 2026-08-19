namespace FuelsManager.Areas.UserAdministrationArea.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    [Serializable]
    public class UserAdministrationModel
    {
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserAdministrationModel()
        {
            this.Init();
        }

        public List<UserModel> UserList { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public Guid UserGuid { get; set; }
        public SiteGroupRightModel PermissionGroupModel { get; set; }
        public string ShortDatePattern { get; set; }
        public string TimePattern { get; set; }
        public NumberFormatInfo Format { get; set; }
        public string TimeZone { get; set; }
        public DateTimeFormatInfo DateTimeFormatInfo { get; set; }

        public string UserGuidStr
        {
            get { return this.UserGuid.ToString(); }
            set
            {
                Guid newGuid;
                this.UserGuid = Guid.Empty;

                if (Guid.TryParse(value, out newGuid))
                {
                    this.UserGuid = newGuid;
                }
            }
        }

        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.UserList = new List<UserModel>();
            this.UserId = string.Empty;
            this.UserName = string.Empty;
            this.EmailAddress = string.Empty;
            this.UserGuid = Guid.Empty;
            this.PermissionGroupModel = new SiteGroupRightModel();
        }
    }

    [Serializable]
    public class UserModel
    {
        public Guid UserGuid { get; set; }
        public string UserId { get; set; }

        public string UserGuidStr
        {
            get
            {
                return this.UserGuid.ToString();
            }
            set
            {
                Guid newGuid;
                this.UserGuid = Guid.Empty;

                if (Guid.TryParse(value, out newGuid))
                {
                    this.UserGuid = newGuid;
                }
            }
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserModel()
        {
            this.Init();
        }

        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.UserGuid = Guid.Empty;
            this.UserId = string.Empty;
        }
    }
}