using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.UtilityObjects
{

   /// <summary>
   /// ApplicationInformation - Contains methods that provide information about the application that rarely change.
   /// </summary>
   public class ApplicationInformation
   {

      public static string CustomApplicationType
      {
         get
         {
            string customAppType = string.Empty;
            try
            {
               var security = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
               string serviceLogin = FMChannelHelper.MakeCall<IDBAccess, string>(dbAccessChannel => dbAccessChannel.ServiceLogin(security));
               security.UserID = serviceLogin;

               customAppType = ConfigurationSettingsHelper.GetValue<string>("CustomApplicationType", string.Empty, security);
               using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
               {
                  eventLog.WriteEntry($"FMBusinessService - CustomApplicationType={customAppType}", EventLogEntryType.Information);
               }
               customAppType = customAppType?.Trim();

            }
            catch (Exception ex)
            {
               try
               {
                  customAppType = string.Empty;
                  using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
                  {
                     eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                     eventLog.WriteEntry(ex.StackTrace, EventLogEntryType.Error);
                  }
               }
               catch
               {
               }
            }
            return customAppType;
         }
      }

      public static bool IsFDSIM
      {
         get
         {
            return CustomApplicationType.Equals("fds-im", StringComparison.OrdinalIgnoreCase) || CustomApplicationType.Equals("fdsim", StringComparison.OrdinalIgnoreCase);
         }
      }
      public static bool isCITGO
      {
         get
         {
            return CustomApplicationType.Equals("citgo", StringComparison.OrdinalIgnoreCase);
         }
      }
      public static bool isSunoco
      {
         get
         {
            return CustomApplicationType.Equals("sunoco", StringComparison.OrdinalIgnoreCase);
         }
      }
      public static bool isMarathon
      {
         get
         {
            return CustomApplicationType.Equals("marathon", StringComparison.OrdinalIgnoreCase);
         }
      }
   }
}
