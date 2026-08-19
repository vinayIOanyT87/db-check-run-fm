namespace FuelsManager.Areas.UserAdministrationArea.ViewModels
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AuditSourceFilterDataModel
    {
        #region Properties
        public List<AuditSourceSiteFilterModel> SiteList { get; set; }
        public List<AuditSourceActionIdFilterModel> ActionIdList { get; set; }
        public List<AuditSourceTypeIdFilterModel> TypeIdList { get; set; }
        public List<AuditSourceIdFilterModel> IdList { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuditSourceFilterDataModel()
        {
            this.Initialize();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.SiteList = new List<AuditSourceSiteFilterModel>();
            this.ActionIdList = new List<AuditSourceActionIdFilterModel>();
            this.TypeIdList = new List<AuditSourceTypeIdFilterModel>();
            this.IdList = new List<AuditSourceIdFilterModel>();
        }
        #endregion
    }

    [Serializable]
    public class AuditSourceSiteFilterModel
    {
        #region Properties
        public string SiteId { get; set; }
        public string SiteGuidStr { get; set; }

        public Guid SiteGuid
        {
            get { return Guid.Parse(this.SiteGuidStr); }
            set { this.SiteGuidStr = value.ToString(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuditSourceSiteFilterModel()
        {
            this.Initialize();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.SiteGuidStr = Guid.Empty.ToString();
            this.SiteId = string.Empty;
        }
        #endregion
    }

    [Serializable]
    public class AuditSourceActionIdFilterModel
    {
        #region Properties
        public string ActionId { get; set; }
        public string ActionIdValue { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuditSourceActionIdFilterModel()
        {
            this.Initialize();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.ActionId = string.Empty;
            this.ActionIdValue = string.Empty;
        }
        #endregion
    }

    [Serializable]
    public class AuditSourceTypeIdFilterModel
    {
        #region Properties
        public string TypeId { get; set; }
        public string TypeIdValue { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuditSourceTypeIdFilterModel()
        {
            this.Initialize();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.TypeId = string.Empty;
            this.TypeIdValue = string.Empty;
        }
        #endregion
    }

    [Serializable]
    public class AuditSourceIdFilterModel
    {
        #region Properties
        public string Id { get; set; }
        public string IdValue { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public AuditSourceIdFilterModel()
        {
            this.Initialize();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.Id = string.Empty;
            this.IdValue = string.Empty;
        }
        #endregion
    }
}