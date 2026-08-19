using System;
using System.Data;
using System.Data.SqlClient;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.DataAccessLayer;
using static Opc.ResultID;


namespace FMBusinessServices.ServiceClasses
{
   public class SiteCloseoutTimes : ISiteCloseoutTimes
   {
      public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();
      public SiteCloseoutTimes() 
      { 
      }

      public void SetCloseoutTime(SecurityClass security, SiteCloseoutTimeClass siteCloseoutTime)
      {

         using (var cmd = new SqlCommand())
         {
            siteCloseoutTime.SetCloseoutTime(cmd);
            this.ConsolidatedDa.ExecuteQuery(security, cmd);
         }
      }
      public TimeSpan GetCloseoutTime(SecurityClass security, DateTimeOffset date)
      {

         using (var cmd = new SqlCommand())
         {
            SiteCloseoutTimeClass siteCloseoutTime = new SiteCloseoutTimeClass();
            siteCloseoutTime.SiteGuid = security.SiteGuid;
            siteCloseoutTime.GetCloseoutTime(cmd, date);
            Object closeoutDate = this.ConsolidatedDa.ExecuteScalar(cmd, security);
            if (closeoutDate != null && closeoutDate is TimeSpan)
            {
               return (TimeSpan)closeoutDate;
            }
            return TimeSpan.Zero;
         }

      }

      public void Purge(SecurityClass security, Guid siteCloseoutTimeGuid)
      {
         using (var cmd = new SqlCommand())
         {
            SiteCloseoutTimeClass siteCloseoutTime = new SiteCloseoutTimeClass();
            siteCloseoutTime.IdentityGuid = siteCloseoutTimeGuid;
            siteCloseoutTime.PurgeSQL(cmd);
            this.ConsolidatedDa.ExecuteQuery(security, cmd);
         }
         return;
      }
      public void PurgeBySiteGuid(SecurityClass security, Guid siteGuid)
      {
         using (var cmd = new SqlCommand())
         {
            SiteCloseoutTimeClass siteCloseoutTime = new SiteCloseoutTimeClass();
            siteCloseoutTime.SiteGuid = siteGuid;
            siteCloseoutTime.PurgeBySiteGuidSQL(cmd);
            this.ConsolidatedDa.ExecuteQuery(security, cmd);
         }
         return;
      }      
      public Guid Add(SecurityClass security, SiteCloseoutTimeClass siteCloseoutTime)
      {
         throw new NotImplementedException();

      }      
      public void Modify(SecurityClass security, SiteCloseoutTimeClass siteCloseoutTime)
      {
         throw new NotImplementedException();

      }

      public SiteCloseoutTimeClass Get(SecurityClass security, Guid siteCloseoutTimeGuid)
      {
         throw new NotImplementedException();

      }

      public SiteCloseoutTimeCollectionClass EnumerateBySiteGuid(SecurityClass security, Guid siteGuid)
      {
         throw new NotImplementedException();

      }

      public SiteCloseoutTimeCollectionClass EnumerateBySiteGuidAndDate(SecurityClass security, Guid siteGuid, DateTime date)
      {
         throw new NotImplementedException();

      }

      public SiteCloseoutTimeCollectionClass EnumerateBySiteGuidAndDate(SecurityClass security, Guid siteGuid, DateTimeOffset date)
      {
         throw new NotImplementedException();
      }
   }
}