using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
   [CollectionDataContract]
   [Serializable]
   [KnownType(typeof(SiteCloseoutTimeClass))]
   public class SiteCloseoutTimeCollectionClass : List<SiteCloseoutTimeClass> { }

   [KnownType(typeof(GregorianCalendar))]
   [DataContract]
   [Serializable]
   public class SiteCloseoutTimeClass : BaseDataObject
   {
      [DataMember]
      protected DateTimeOffset effectiveDate;

      [DataMember]
      protected DateTimeOffset expirationDate;

      [DataMember]
      protected TimeSpan? closeoutTime;

      protected bool pointsChanged;

      #region Constructors
      /// <summary>
      /// This is the default constructor for the Schedule class.
      /// </summary>
      public SiteCloseoutTimeClass()
      {
         this.closeoutTime = null;
         this.expirationDate = new DateTimeOffset();
         this.effectiveDate = new DateTimeOffset();
         this.Reset();
      }

      #endregion

      #region Properties
      public TimeSpan? CloseoutTime
      {
         get { return this.closeoutTime; }
         set { this.closeoutTime = value; }
      }

      public DateTimeOffset ExpirationDate
      {
         get { return this.expirationDate; }
         set { this.expirationDate = value; }
      }

      public DateTimeOffset EffectiveDate
      {
         get { return this.effectiveDate; }
         set { this.effectiveDate = value; }
      }

      public bool PointsChanged
      {
         get { return this.pointsChanged; }
         set { this.pointsChanged = value; }
      }
 

      #endregion

      public override void Reset()
      {
         base.Reset();


         this.closeoutTime = null;
         this.expirationDate = new DateTimeOffset();
         this.effectiveDate = new DateTimeOffset();
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

             _IdentityGuid = DataObject.getValue(Row["SiteCloseoutTimeGuid"], Guid.Empty);
            this.SiteGuid = DataObject.getValue(Row["SiteGuid"], Guid.Empty);
            if (Row["CloseoutTime"] != null)
            {
               this.closeoutTime = (TimeSpan) Row["CloseoutTime"] ;
            }
            
            this.expirationDate = DataObject.getValue(Row["ExpirationDate"], DateTimeOffset.Now);
            this.effectiveDate = DataObject.getValue(Row["EffectiveDate"], DateTimeOffset.Now);
            _CreatedDate = DataObject.getValue(Row["CreatedDate"], DateTimeOffset.Now);
            _CreatedBy = DataObject.getValue(Row["CreatedBy"], ADMIN);
            _UpdatedDate = DataObject.getValue(Row["UpdatedDate"], _CreatedDate);
            _UpdatedBy = DataObject.getValue(Row["UpdatedBy"], ADMIN);
         }
      }
      public void InsertSQL(SqlCommand cmd)
      {
         cmd.CommandText = "INSERT INTO [dbo].[tblSiteCloseoutTime] " +
               "(" +
               "SiteCloseoutTimeGuid," +
               "SiteGuid," +
               "CloseoutTime," +
               "ExpirationDate," +
               "EffectiveDate," +
               "CreatedDate," +
               "CreatedBy," +
               "UpdatedDate," +
               "UpdatedBy" +
               ") VALUES (" +
               "@SiteCloseoutTimeGuid," +
               "@SiteGuid," +
               "@CloseoutTime," +
               "@ExpirationDate," +
               "@EffectiveDate," +
               "@CreatedDate," +
               "@CreatedBy," +
               "@UpdatedDate," +
               "@UpdatedBy)";

         cmd.Parameters.AddWithValue("@SiteCloseoutTimeGuid", this.IdentityGuid);
         cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
         cmd.Parameters.AddWithValue("@CloseoutTime", this.closeoutTime == null ? (object)DBNull.Value : this.closeoutTime.Value);
         cmd.Parameters.AddWithValue("@ExpirationDate", this.ExpirationDate);
         cmd.Parameters.AddWithValue("@EffectiveDate", this.EffectiveDate);
         cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
         cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
         cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
         cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
      }

      public void UpdateSQL(SqlCommand cmd)
      {
         cmd.CommandText = "UPDATE [dbo].[tblSiteCloseoutTime] " +
               "SET " +
               "SiteCloseoutTimeGuid = @SiteCloseoutTimeGuid, " +
               "SiteGuid = @SiteGuid, " +
               "CloseoutTime = @CloseoutTime, " +
               "ExpirationDate = @ExpirationDate, " +
               "EffectiveDate = @EffectiveDate, " +
               "UpdatedDate = @UpdatedDate, " +
               "UpdatedBy = @UpdatedBy " +
               "WHERE SiteCloseoutTimeGuid = @SiteCloseoutTimeGuid";

         cmd.Parameters.AddWithValue("@SiteCloseoutTimeGuid", this.IdentityGuid);
         cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
         cmd.Parameters.AddWithValue("@CloseoutTime", this.closeoutTime == null ? (object)DBNull.Value : this.closeoutTime.Value);
         cmd.Parameters.AddWithValue("@ExpirationDate", this.ExpirationDate);
         cmd.Parameters.AddWithValue("@EffectiveDate", this.EffectiveDate);
         cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
         cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
         cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
         cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
      }

      public void SelectSQL(SqlCommand cmd, bool bInTransaction)
      {
         cmd.CommandText = "SELECT * FROM [dbo].[tblSiteCloseoutTime] " + SQLUpdateLock(bInTransaction) + " WHERE SiteCloseoutTimeGuid =  @SiteCloseoutTimeGuid";

         cmd.Parameters.AddWithValue("@SiteCloseoutTimeGuid", _IdentityGuid);
      }

      public void SelectBySiteAndDateSQL(SqlCommand cmd, DateTime date, bool bInTransaction)
      {
         cmd.CommandText = "SELECT * FROM [dbo].[tblSiteCloseoutTime] " + SQLUpdateLock(bInTransaction) + " WHERE SiteCloseoutTimeGuid =  @SiteCloseoutTimeGuid" +
               " AND @SiteGuid=SiteGuid AND @Date BETWEEN EffectiveDate and ExpirationDate";

         cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
         cmd.Parameters.AddWithValue("@Date", date);

      }

      public void PurgeSQL(SqlCommand cmd)
      {
         cmd.CommandText = "DELETE FROM [dbo].[tblSiteCloseoutTime] WHERE SiteCloseoutTimeGuid = @SiteCloseoutTimeGuid";
         cmd.Parameters.AddWithValue("@SiteCloseoutTimeGuid", _IdentityGuid);
      }
      public void PurgeBySiteGuidSQL(SqlCommand cmd)
      {
         cmd.CommandText = "DELETE FROM [dbo].[tblSiteCloseoutTime] WHERE SiteGuid = @SiteGuid";
         cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
      }

      public void EnumerateBySiteGuidSQL(SqlCommand cmd)
      {
         cmd.CommandText = "SELECT * " +
               " FROM [dbo].[tblSiteCloseoutTime] WHERE SiteGuid = @SiteGuid" +
               " ORDER BY EffectiveDate";

         cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
      }

      public void SetCloseoutTime(SqlCommand cmd)
      {
         cmd.CommandText = "dbo.usp_SetSiteCloseoutTime";
         cmd.CommandType = CommandType.StoredProcedure;
         cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
         cmd.Parameters.AddWithValue("@CloseoutTime", this.closeoutTime == null ? (object)DBNull.Value : this.closeoutTime.Value);
         cmd.Parameters.AddWithValue("@ExpirationDateTime", this.ExpirationDate);
         cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
         cmd.Parameters.AddWithValue("@PointsChanged", this.PointsChanged);

        }
      public void GetCloseoutTime(SqlCommand cmd, DateTimeOffset date)
      {
         cmd.CommandText = "SELECT dbo.udf_GetCloseoutTime(@SiteGuid, @CurrentDateTime) AS CloseoutTime";
         cmd.CommandType = CommandType.Text;
         cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
         cmd.Parameters.AddWithValue("@CurrentDateTime", date);
      }
   }
}
