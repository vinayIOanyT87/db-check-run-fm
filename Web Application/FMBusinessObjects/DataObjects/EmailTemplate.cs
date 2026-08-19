using DocumentFormat.OpenXml.Drawing;
using FMBusinessObjects.UtilityObjects;
using FMCore;
using iTextSharp.text.pdf.parser.clipper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
   [DataContract]
   [Serializable]
   public class EmailTemplateClass : BaseDataObject
   {
      #region Properties
      [DataMember]
      public string Subject {  get; set; }
      [DataMember]
      public string Body { get; set; }

      //[DataMember]
      //public Guid EmailGroupGuid { get; set; }
      #endregion

      #region Public and Internal methods

      public EmailTemplateClass() 
      {
         this.Reset();
      }

      public void InsertSQL(SqlCommand cmd)
      {
         cmd.CommandText = "INSERT INTO tblEmailTemplate " +
                                       "(Subject," +
                                       "Body," +
                                       "CreatedDate," +
                                       "CreatedBy," +
                                       "UpdatedDate," +
                                       "UpdatedBy," +
                                       "EmailTemplateGuid" +
                                       //         "EmailGroupGuid" +
                                       ") VALUES (" +
                                       "@Subject," +
                                       "@Body," +
                                       "@CreatedDate," +
                                       "@CreatedBy," +
                                       "@UpdatedDate," +
                                       "@UpdatedBy," +
                                       "@EmailTemplateGuid)";

         cmd.Parameters.Add("@Subject", SqlDbType.NVarChar, 1024);
         cmd.Parameters.Add("@Body", SqlDbType.NVarChar, 8196);
         cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
         cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
         cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
         cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
         cmd.Parameters.Add("@EmailTemplateGuid", SqlDbType.UniqueIdentifier);
 
         cmd.Parameters["@Subject"].Value = Subject.DefaultIfNull(string.Empty);
         cmd.Parameters["@Body"].Value = Body.DefaultIfNull(string.Empty); 
         cmd.Parameters["@CreatedDate"].Value = CreatedDate;
         cmd.Parameters["@CreatedBy"].Value = CreatedBy;
         cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
         cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
         cmd.Parameters["@EmailTemplateGuid"].Value = _IdentityGuid;
      }

      public void UpdateSQL(SqlCommand cmd)
      {

         cmd.CommandText = "UPDATE tblEmailTemplate " +
                                 "SET Subject = @Subject," +
                                 "Body = @Body," +
                                 "UpdatedDate = @UpdatedDate," +
                                 "UpdatedBy = @UpdatedBy" +
                                 " WHERE EmailTemplateGuid = @EmailTemplateGuid";

         cmd.Parameters.Add("@Subject", SqlDbType.NVarChar, 1024);
         cmd.Parameters.Add("@Body", SqlDbType.NVarChar, 8196);
         cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
         cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
         cmd.Parameters.Add("@EmailTemplateGuid", SqlDbType.UniqueIdentifier);

         cmd.Parameters["@Subject"].Value = Subject.DefaultIfNull(string.Empty);
         cmd.Parameters["@Body"].Value = Body.DefaultIfNull(string.Empty);
         cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
         cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
         cmd.Parameters["@EmailTemplateGuid"].Value = _IdentityGuid;
      }

      public void PurgeSQL(SqlCommand cmd)
      {
         cmd.CommandText = "DELETE FROM tblEmailTemplate WHERE EmailTemplateGuid = @EmailTemplateGuid";
         cmd.Parameters.Add("@EmailTemplateGuid", SqlDbType.UniqueIdentifier);
         cmd.Parameters["@EmailTemplateGuid"].Value = IdentityGuid;
      }

      //public override ENTITY_TYPE EntityType
      //{
      //    get { return ENTITY_TYPE.EMAIL_GROUP; }
      //    set {; }
      //}

      public override ENTITY_TYPE ParentEntityType
      {
         get { return ENTITY_TYPE.ALARM_AND_EVENT; }
      }

      public override void Reset()
      {
         base.Reset();

         base.ID = string.Empty;
         this.Subject = string.Empty;
         this.Body = string.Empty;

      }

      public override void Load(Object o)
      {
         this.Reset();

         if (typeof(DataSet).IsInstanceOfType(o))
         {
            DataSet Set = (DataSet)o;
            DataTable Table = Set.Tables[0];

            if (Table.Rows.Count == 0)
            {
               return;
            }

            DataRow Row = Table.Rows[0];

            base._IdentityGuid = DataObject.getValue<Guid>(Row["EmailTemplateGuid"], Guid.Empty);
            //  this.EmailGroupGuid = DataObject.getValue<Guid>(Row["EmailGroupGuid"], Guid.Empty);
            this.Subject = DataObject.getValue<string>(Row["Subject"], string.Empty);
            this.Body = DataObject.getValue<string>(Row["Body"], string.Empty);

            base._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
            base._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
            base._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
            base._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
         }
         else if (typeof(EmailTemplateClass).IsInstanceOfType(o))
         {
            EmailTemplateClass emailTemplate = (EmailTemplateClass)o;

            base._IdentityGuid = emailTemplate.IdentityGuid;
            this.Subject = emailTemplate.Subject;
            this.Body = emailTemplate.Body;

            base._CreatedDate = emailTemplate.CreatedDate;
            base._CreatedBy = emailTemplate.CreatedBy;
            base._UpdatedDate = emailTemplate.UpdatedDate;
            base._UpdatedBy = emailTemplate.UpdatedBy;

         }
         else
         {
            base.Load(o);
         }
      }

      public void SelectSQL(SqlCommand cmd, bool bInTransaction)
      {
         cmd.CommandText = "SELECT * FROM tblEmailTemplate " + SQLUpdateLock(bInTransaction) + " WHERE EmailTemplateGuid = @EmailTemplateGuid";
         cmd.Parameters.Add("@EmailTemplateGuid", SqlDbType.UniqueIdentifier);
         cmd.Parameters["@EmailTemplateGuid"].Value = IdentityGuid;
      }
      public void SelectSQL(SqlCommand cmd, Guid alarmndEventGuid, bool bInTransaction)
      {
         cmd.CommandText = "SELECT e.* FROM [dbo].[tblEmailTemplate ] e JOIN [map].[tblEmailTemplateToAlarmAndEvent] m ON e.EmailTemplateGuid=m.EmailTemplateGuid " + SQLUpdateLock(bInTransaction) + " WHERE m.AlarmAndEventGuid = @AlarmAndEventGuid";
         cmd.Parameters.Add("@AlarmAndEventGuid", SqlDbType.UniqueIdentifier);
         cmd.Parameters["@AlarmAndEventGuid"].Value = alarmndEventGuid;
      }

      #endregion

   }
}
