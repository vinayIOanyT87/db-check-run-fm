namespace FuelsManager.Areas.UserAdministrationArea.ViewModels
{
    using System;

    [Serializable]
    public class AuditViewFilterModel
    {
        #region Properties
        public string SiteGuidStr { get; set; }
        public string ActionId { get; set; }
        public string TypeId { get; set; }
        public string Id { get; set; }
        public string UserGuidStr { get; set; }
        public string Source { get; set; }
        public string BeginDateStr { get; set; }
        public string EndDateStr { get; set; }
        public bool HasDate { get; set; }
        public bool IncludeMemberSites { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuditViewFilterModel()
        {
            this.Init();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.SiteGuidStr    = string.Empty;
            this.ActionId       = string.Empty;
            this.TypeId         = string.Empty;
            this.Id             = string.Empty;
            this.UserGuidStr    = string.Empty;
            this.Source         = string.Empty;
            this.BeginDateStr   = string.Empty;
            this.EndDateStr     = string.Empty;
            this.HasDate        = false;
            this.IncludeMemberSites = false;
        }
        #endregion
    }
}