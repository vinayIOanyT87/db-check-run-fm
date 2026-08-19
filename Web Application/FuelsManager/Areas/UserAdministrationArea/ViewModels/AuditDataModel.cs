namespace FuelsManager.Areas.UserAdministrationArea.ViewModels
{
    using System;

    [Serializable]
    public class AuditDataRecordModel
    {
        #region Data members
        private DateTimeOffset? auditDateTime;
        #endregion

        #region Properties
        public string ActionId { get; set; }
        public string TypeId { get; set; }
        public string Id { get; set; }
        public string PropertyId { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string SiteId { get; set; }
        public string Source { get; set; }
        public string AuditDateTimeStr { get; set; }

        public DateTimeOffset? AuditDateTime
        {
            get { return this.auditDateTime; }
            set
            {
                this.AuditDateTimeStr = string.Empty;
                this.auditDateTime = null;

                if (value != null)
                {
                    this.auditDateTime = value;
                    this.AuditDateTimeStr = this.auditDateTime.ToString();
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public AuditDataRecordModel()
        {
            this.Init();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the objects to its initial state.
        /// </summary>
        private void Init()
        {
            this.AuditDateTime  = null;
            this.ActionId       = string.Empty;
            this.TypeId         = string.Empty;
            this.Id             = string.Empty;
            this.PropertyId     = string.Empty;
            this.NewValue       = string.Empty;
            this.OldValue       = string.Empty;
            this.SiteId         = string.Empty;
            this.Source         = string.Empty;
        }
        #endregion
    }
}