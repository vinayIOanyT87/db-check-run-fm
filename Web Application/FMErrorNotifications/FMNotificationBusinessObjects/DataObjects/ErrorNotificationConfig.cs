using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMNotificationBusinessObjects.DataObjects
{
    public class ErrorNotificationConfig
    {

        #region Private Fields

        private Guid siteGuid;
        private string siteId;
        private string emailAddresses;
        private string errorFolder;
        private string createdBy;
        private DateTimeOffset? createdDate;
        private string updatedBy;
        private DateTimeOffset? updatedDate;

        #endregion

        #region Public Properties

        public Guid SiteGuid
        {
            get { return siteGuid; }
            set { siteGuid = value; }
        }
        public string SiteId
        {
            get { return siteId; }
            set { siteId = value; }
        }
        public string EmailAddresses
        {
            get { return emailAddresses; }
            set { emailAddresses = value; }
        }
        public string ErrorFolder
        {
            get { return errorFolder; }
            set { errorFolder = value; }
        }
        public string CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }
        public DateTimeOffset? CreatedDate
        {
            get { return createdDate; }
            set { createdDate = value; }
        }
        public string UpdatedBy
        {
            get { return updatedBy; }
            set { updatedBy = value; }
        }
        public DateTimeOffset? UpdatedDate
        {
            get { return updatedDate; }
            set { updatedDate = value; }
        }

        #endregion

        public ErrorNotificationConfig() { }
    }
}
