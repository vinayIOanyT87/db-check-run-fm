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
   public class EmailTemplateToAlarmAndEventMapClass : BaseDataObject
   {
      #region Properties
      [DataMember]
      public Guid EmailTemplateGuid {  get; set; }
      [DataMember]
      public Guid AlarmAndEventGuid { get; set; }
      #endregion

      #region Public and Internal methods

      public EmailTemplateToAlarmAndEventMapClass() 
      {
         this.Reset();
      }

      public void InsertSQL(SqlCommand cmd)
      {
         cmd.CommandText = "INSERT INTO [map].[tblEmailTemplateToAlarmAndEvent] " +
                                       "(EmailTemplateToAlarmAndEventGuid," +
                                       "EmailTemplateGuid," +
                                       "AlarmAndEventGuid," +
                                       "CreatedDate," +
                                       "CreatedBy," +
                                       "UpdatedDate," +
                                       "UpdatedBy" +
                                       ") VALUES (@EmailTemplateToAlarmAndEventGuid," +
                                       "@EmailTemplateGuid," +
                                       "@AlarmAndEventGuid," +
                                       "@CreatedDate," +
                                       "@CreatedBy," +
                                       "@UpdatedDate," +
                                       "@UpdatedBy)";

         cmd.Parameters.Add("@EmailTemplateGuid", SqlDbType.UniqueIdentifier);
         cmd.Parameters.Add("@AlarmAndEventGuid", SqlDbType.UniqueIdentifier);
         cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
         cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
         cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
         cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
         cmd.Parameters.Add("@EmailTemplateToAlarmAndEventGuid", SqlDbType.UniqueIdentifier);


         cmd.Parameters["@EmailTemplateGuid"].Value = EmailTemplateGuid;
         cmd.Parameters["@AlarmAndEventGuid"].Value = AlarmAndEventGuid; 
         cmd.Parameters["@CreatedDate"].Value = CreatedDate;
         cmd.Parameters["@CreatedBy"].Value = CreatedBy;
         cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
         cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
         cmd.Parameters["@EmailTemplateToAlarmAndEventGuid"].Value = _IdentityGuid;
      }

      public void UpdateSQL(SqlCommand cmd)
      {

         cmd.CommandText = "UPDATE [map].[tblEmailTemplateToAlarmAndEvent] " +
                                 "SET EmailTemplateGuid = @EmailTemplateGuid," +
                                 "AlarmAndEventGuid = @AlarmAndEventGuid," +
                                 "UpdatedDate = @UpdatedDate," +
                                 "UpdatedBy = @UpdatedBy" +
                                 " WHERE EmailTemplateGuid = @EmailTemplateGuid";

         cmd.Parameters.Add("@EmailTemplateGuid", SqlDbType.NVarChar, 1024);
         cmd.Parameters.Add("@AlarmAndEventGuid", SqlDbType.NVarChar, 8196);
         cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
         cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
         cmd.Parameters.Add("@EmailTemplateToAlarmAndEventGuid", SqlDbType.UniqueIdentifier);

         cmd.Parameters["@EmailTemplateGuid"].Value = EmailTemplateGuid;
         cmd.Parameters["@AlarmAndEventGuid"].Value = AlarmAndEventGuid;
         cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
         cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
         cmd.Parameters["@EmailTemplateToAlarmAndEventGuid"].Value = _IdentityGuid;
      }

      public void PurgeSQL(SqlCommand cmd)
      {
         cmd.CommandText = "DELETE FROM [map].[tblEmailTemplateToAlarmAndEvent] WHERE EmailTemplateToAlarmAndEventGuid = @EmailTemplateToAlarmAndEventGuid";
         cmd.Parameters.Add("@EmailTemplateToAlarmAndEventGuid", SqlDbType.UniqueIdentifier);
         cmd.Parameters["@EmailTemplateToAlarmAndEventGuid"].Value = IdentityGuid;
      }


      public override ENTITY_TYPE ParentEntityType
      {
         get { return ENTITY_TYPE.ALARM_AND_EVENT; }
      }

      public override void Reset()
      {
         base.Reset();

         base.ID = string.Empty;
         this.EmailTemplateGuid = Guid.Empty;
         this.AlarmAndEventGuid = Guid.Empty;

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

            this.EmailTemplateGuid = DataObject.getValue<Guid>(Row["EmailTemplateGuid"], Guid.Empty);
            this.AlarmAndEventGuid = DataObject.getValue<Guid>(Row["AlarmAndEventGuid"], Guid.Empty);

            base._IdentityGuid = DataObject.getValue<Guid>(Row["EmailTemplateToAlarmAndEventGuid"], Guid.Empty);
            base._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
            base._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
            base._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
            base._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
         }
         else if (typeof(EmailTemplateToAlarmAndEventMapClass).IsInstanceOfType(o))
         {
            EmailTemplateToAlarmAndEventMapClass emailTemplateToAlarmAndEvent = (EmailTemplateToAlarmAndEventMapClass)o;

            base._IdentityGuid = emailTemplateToAlarmAndEvent.IdentityGuid;

            this.EmailTemplateGuid = emailTemplateToAlarmAndEvent.EmailTemplateGuid;
            this.AlarmAndEventGuid = emailTemplateToAlarmAndEvent.AlarmAndEventGuid;

            base._CreatedDate = emailTemplateToAlarmAndEvent.CreatedDate;
            base._CreatedBy = emailTemplateToAlarmAndEvent.CreatedBy;
            base._UpdatedDate = emailTemplateToAlarmAndEvent.UpdatedDate;
            base._UpdatedBy = emailTemplateToAlarmAndEvent.UpdatedBy;

         }
         else
         {
            base.Load(o);
         }
      }

      public void SelectSQL(SqlCommand cmd, bool bInTransaction)
      {
         cmd.CommandText = "SELECT * FROM [map].[tblEmailTemplateToAlarmAndEvent] " + SQLUpdateLock(bInTransaction) ;
         if (this.EmailTemplateGuid == Guid.Empty && this.AlarmAndEventGuid == Guid.Empty)
         {
            cmd.Parameters.Add("@EmailTemplateToAlarmAndEventGuid", SqlDbType.UniqueIdentifier);
            cmd.CommandText += " WHERE EmailTemplateToAlarmAndEventGuid = @EmailTemplateToAlarmAndEventGuid";
         }
         else
         {
            string op = " WHERE ";
            if (this.EmailTemplateGuid != Guid.Empty)
            {
               cmd.Parameters["@EmailTemplateGuid"].Value = this.EmailTemplateGuid;
               op = " AND ";
               cmd.CommandText += " WHERE EmailTemplateGuid = @EmailTemplateGuid";
            }
            if (this.AlarmAndEventGuid != Guid.Empty)
            {
               cmd.Parameters["@AlarmAndEventGuid"].Value = this.AlarmAndEventGuid;
               cmd.CommandText += op + " AlarmAndEventGuid = @AlarmAndEventGuid";
            }
         }
      }


      #endregion

   }
}
