using System;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using System.Security;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;
using System.Collections.Generic;
using crypto;

namespace FMBusinessServices.ServiceClasses
{
   [SecuritySafeCriticalAttribute]
   [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
   public class EmailTemplatesClass : IDependency, IEmailTemplatesClass
   {
      #region Private data members
      private ConsolidatedDAClass consolidatedDA;
      #endregion

      public EmailTemplatesClass() 
      {
         this.consolidatedDA = new ConsolidatedDAClass();
      }

      [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
      public Guid Add(SecurityClass security, EmailTemplateClass emailTemplate)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (emailTemplate == null)
         {
            throw new ArgumentNullException("emailTemplate");
         }

         if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
         {
            throw new FMInsufficientRightsException();
         }

         this.Validate(emailTemplate);

         var oldEmailTemplate = Get(security, emailTemplate.IdentityGuid);
         if (null != oldEmailTemplate && oldEmailTemplate.IdentityGuid != Guid.Empty)
         {
            throw (new Exception("E-mail Template Exists"));
         }

         emailTemplate.CreatedDate = DateTimeOffset.Now;
         emailTemplate.CreatedBy = security.UserID;
         emailTemplate.UpdatedDate = emailTemplate.CreatedDate;
         emailTemplate.UpdatedBy = security.UserID;
         emailTemplate.IdentityGuid = Guid.NewGuid();

         using (var cmd = new SqlCommand())
         {
         emailTemplate.InsertSQL(cmd);
         this.consolidatedDA.ExecuteQuery(security, cmd);
         }

         return emailTemplate.IdentityGuid;
      }

      [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
      public void Modify(SecurityClass security, EmailTemplateClass emailTemplate)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (emailTemplate == null)
         {
            throw new ArgumentNullException("emailTemplate");
         }

         if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
         {
            throw new FMInsufficientRightsException();
         }

         this.Validate(emailTemplate);

         // Verify email template does not exist
         if (null == Get(security, emailTemplate.IdentityGuid))
         {
            throw (new Exception("E-mail template does not exist."));
         }

         EmailTemplateClass oldEmailTemplate = this.Get(security, emailTemplate.IdentityGuid);

         if (oldEmailTemplate.IdentityGuid == Guid.Empty)
         {
            throw (new Exception("E-mail Template Not Found"));
         }

         emailTemplate.UpdatedDate = DateTimeOffset.Now;
         emailTemplate.UpdatedBy = security.UserID;

         using (SqlCommand cmd = new SqlCommand())
         {
            emailTemplate.UpdateSQL(cmd);
            this.consolidatedDA.ExecuteQuery(security, cmd);
         }
      }

      private void Validate(EmailTemplateClass emailTemplate)
      {
         if (emailTemplate.Subject.Length > 1024)
         {
            throw (new Exception("Maximum number of characters must not exceed 1024"));
         }

         if (emailTemplate.Subject.Length > 1024 * 8)
         {
            throw (new Exception("Maximum number of characters must not exceed 8192"));
         }
      }


      public EmailTemplateClass Get(SecurityClass security, Guid emailTemplateGuid)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
         {
            throw new FMInsufficientRightsException();
         }

         var emailTemplate = new EmailTemplateClass();
         emailTemplate.IdentityGuid = emailTemplateGuid;

         using (var cmd = new SqlCommand())
         {
            emailTemplate.SelectSQL(cmd, ContextUtil.IsInTransaction);
            emailTemplate.Load(this.consolidatedDA.GetDataSet(cmd, security));
         }

         return emailTemplate;
      }
      public EmailTemplateClass GetByAlarmAndEvent(SecurityClass security, Guid alarmAndEventGuid)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
         {
            throw new FMInsufficientRightsException();
         }

         var emailTemplate = new EmailTemplateClass();
         
         using (var cmd = new SqlCommand())
         {
            emailTemplate.SelectSQL(cmd, alarmAndEventGuid, ContextUtil.IsInTransaction);
            emailTemplate.Load(this.consolidatedDA.GetDataSet(cmd, security));
         }

         return emailTemplate;
      }

      [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
      public void Purge(SecurityClass security, Guid emailTemplateGuid)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
         {
            throw new FMInsufficientRightsException();
         }

         EmailTemplateClass emailTemplate = this.Get(security, emailTemplateGuid);

         if (emailTemplate.IdentityGuid == Guid.Empty)
         {
            throw (new Exception("E-mail Template Not Found"));
         }

         // Purge Dependencies
         var dependencies = new DependenciesClass(security);
         dependencies.Purge(security, emailTemplate);

         using (SqlCommand cmd = new SqlCommand())
         {
            emailTemplate.PurgeSQL(cmd);
            this.consolidatedDA.ExecuteQuery(security, cmd);
         }
      }

      void IDependency.Insert(SecurityClass Security, BaseDataObject Object, bool preOperation)
      {
         if (Security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (Object == null)
         {
            throw new ArgumentNullException("Object");
         }
      }

      void IDependency.Purge(SecurityClass Security, BaseDataObject Object)
      {
         if (Security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (Object == null)
         {
            throw new ArgumentNullException("Object");
         }
      }

      void IDependency.Update(SecurityClass Security, BaseDataObject Object)
      {
         if (Security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (Object == null)
         {
            throw new ArgumentNullException("Object");
         }
      }

   }
}