namespace MirgrationToolProcessing
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using System;

    public class SecurityHandler
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SecurityHandler()
        {
            this.BuildSecurity();
        }
        #endregion

        #region Properties
        public SecurityClass Security { get; private set; }

        public Guid SiteAdminGuid
        {
            get { return Guids.SiteAdminGuid; }
        }
        #endregion

        #region Public methods
        public Guid GetSiteGuidById(string siteID)
        {
            this.Security.SiteGuid = Guids.SiteAdminGuid;
            SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(this.Security, siteID, true));

            if(site == null)
            {
                return Guid.Empty;
            }

            return site.SiteGuid;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will create a security object with site set to site admin.
        /// </summary>
        private void BuildSecurity()
        {
            this.Security = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
            this.Security.UserID = "Administrator";
            this.Security.UserGuid = Guid.Parse("00000000-0000-0000-0000-000000000002");

            this.Security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
            this.Security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);

            this.Security.AddRight(RIGHT.VIEW_USERS);
            this.Security.AddRight(RIGHT.MODIFY_USERS);

            this.Security.AddRight(RIGHT.VIEW_USER_GROUPS);
            this.Security.AddRight(RIGHT.MODIFY_USER_GROUPS);

            this.Security.AddRight(RIGHT.VIEW_EQUIPMENT_DATA);
            this.Security.AddRight(RIGHT.MODIFY_EQUIPMENT_DATA);

            this.Security.AddRight(RIGHT.VIEW_PERSONNEL_DATA);
            this.Security.AddRight(RIGHT.MODIFY_PERSONNEL_DATA);

            this.Security.AddRight(RIGHT.VIEW_QUALITY_TESTS);
            this.Security.AddRight(RIGHT.MODIFY_QUALITY_TESTS);

            this.Security.AddRight(RIGHT.VIEW_TRAINING_QUAL_HISTORY);
            this.Security.AddRight(RIGHT.MODIFY_TRAINING_QUAL_HISTORY);

            this.Security.AddRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS);
            this.Security.AddRight(RIGHT.MODIFY_PERSON_TRAINING);

            this.Security.AddRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS);

            this.Security.AddRight(RIGHT.VIEW_LOAD_RACK_DATA);
            this.Security.AddRight(RIGHT.MODIFY_LOAD_RACK_DATA);

            this.Security.AddRight(RIGHT.VIEW_METERS);
            this.Security.AddRight(RIGHT.MODIFY_METERS);

            this.Security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
            this.Security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);

            this.Security.AddRight(RIGHT.VIEW_PRODUCTS);
            this.Security.AddRight(RIGHT.MODIFY_PRODUCTS);

            this.Security.AddRight(RIGHT.VIEW_ALLOCATIONS);
            this.Security.AddRight(RIGHT.MODIFY_ALLOCATIONS);

            this.Security.AddRight(RIGHT.VIEW_COMPANY_DATA);
            this.Security.AddRight(RIGHT.MODIFY_COMPANY_DATA);

            this.Security.AddRight(RIGHT.VIEW_FUEL_CARD_DATA);
            this.Security.AddRight(RIGHT.MODIFY_FUEL_CARD_DATA);

            this.Security.AddRight(RIGHT.ENABLEDISABLE_STATIONS);

            this.Security.AddRight(RIGHT.MODIFY_SYSTEM_SETTINGS);
            this.Security.AddRight(RIGHT.IMPORT_ENTERPRISE_DATA);
        }
        #endregion
    }
}
