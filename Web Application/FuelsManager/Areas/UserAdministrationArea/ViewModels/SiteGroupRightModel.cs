namespace FuelsManager.Areas.UserAdministrationArea.ViewModels
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SiteGroupRightModel
    {
        public List<SiteModel> SiteGroupRightList { get; set; }

        public SiteGroupRightModel()
        {
            this.Init();
        }

        private void Init()
        {
            this.SiteGroupRightList = new List<SiteModel>();
        }
    }

    [Serializable]
    public class SiteModel
    {
        public List<GroupModel> GroupList { get; set; } 
        public string SiteName { get; set; }
        public Guid SiteGuid { get; set; }
        public string SiteGuidStr
        {
            get { return this.SiteGuid.ToString(); }
            set
            {
                Guid newGuid;
                this.SiteGuid = Guid.Empty;

                if (Guid.TryParse(value, out newGuid))
                {
                    this.SiteGuid = newGuid;
                }
            }
        }

        public SiteModel()
        {
            this.Init();
        }

        private void Init()
        {
            this.GroupList = new List<GroupModel>();
            this.SiteGuid = Guid.Empty;
            this.SiteName = string.Empty;
        }
    }

    [Serializable]
    public class GroupModel
    {
        public List<RightModel> RightList { get; set; } 
        public string GroupName { get; set; }
        public Guid GroupGuid { get; set; }

        public string GroupGuidStr
        {
            get { return this.GroupGuid.ToString(); }
            set
            {
                Guid newGuid;
                this.GroupGuid = Guid.Empty;

                if (Guid.TryParse(value, out newGuid))
                {
                    this.GroupGuid = newGuid;
                }
            }
        }

        public GroupModel()
        {
            this.Init();
        }

        private void Init()
        {
            this.RightList = new List<RightModel>();
            this.GroupGuid = Guid.Empty;
            this.GroupName = string.Empty;
        }
    }

    [Serializable]
    public class RightModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RightIndex { get; set; }

        public string RightIndexStr
        {
            get { return this.RightIndex.ToString(); }
            set
            {
                int newInt;
                this.RightIndex = 0;

                if (int.TryParse(value, out newInt))
                {
                    this.RightIndex = newInt;
                }
            }
        }

        public RightModel()
        {
            this.Init();
        }

        private void Init()
        {
            this.Description = string.Empty;
            this.Name = string.Empty;
            this.RightIndex = 0;
        }
    }
}