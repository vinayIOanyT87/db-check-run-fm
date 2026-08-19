namespace FMActiveDirectoryManageService
{
    using System;

    [Serializable]
    public class SiteToAdSiteDO
    {
        public string SiteId { get; set; }
        public Guid SiteGuid { get; set; }
        public string ActiveDirectorySiteId { get; set; }

        public SiteToAdSiteDO()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.SiteId = string.Empty;
            this.SiteGuid = Guid.Empty;
            this.ActiveDirectorySiteId = string.Empty;
        }
    }
}
