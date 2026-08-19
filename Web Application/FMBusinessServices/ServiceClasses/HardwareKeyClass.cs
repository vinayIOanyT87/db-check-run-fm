namespace FMBusinessServices.ServiceClasses
{
   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.Exceptions;
   using FMBusinessServices.DataAccessLayer;
   using System;
   using System.Configuration;
   using System.Diagnostics;
   using System.IO;
   //using System.Linq;
   using System.Reflection;
   // declarations for reading a C++ dll
   using System.Runtime.InteropServices;
   //using System.Data.SqlClient;


   public class HardwareKeyClass : IHardwareKey
   {
      static private class LicenseInfoClass
      {
         private const string licenseInfo = "LicenseInfo";

         private static object lck = new object();
         private static DateTime dateLastRead = new DateTime(2024,1,1);
         private static bool isPerpetual = false;

         internal static bool isExpired = true;
         internal static long daysLeftToExpire = 9999;
         internal static DateTime expirationDate = DateTime.Now;

         internal static void ForceRefreshLicenseFile()
         {
            lock (lck)
            {
               RefreshLicenseFile();
            }
         }

         private static void RefreshLicenseFile()
         {
            // read hardware key to see if license file has been updated without using Cache.
            dateLastRead = DateTime.Now;
            ConsolidatedDAClass.ReadHardwareKey();
         }

         private static bool CheckIfInfoIsCurrent()
         {
            DateTime currentDate = DateTime.Now;
            bool current = currentDate.Year == dateLastRead.Year && currentDate.Month == dateLastRead.Month && currentDate.Day == dateLastRead.Day;
            if (current && isExpired)
            {
               //Force read hardware to see if license file has been updated today.
               RefreshLicenseFile();
            }
            return current;
         }

         public static DateTime DateLastRead {
            get
            {
               lock (lck)
               {
                  if (CheckIfInfoIsCurrent())
                  {
                     return dateLastRead;

                  }

                  RefreshLicenseFile();

                  return dateLastRead;
               }
            }
            set
            {
               dateLastRead = value;
            }
         }

         public static bool IsExpired
         {
            get
            {
               lock (lck)
               {
                  if (CheckIfInfoIsCurrent())
                  {
                     return isExpired;

                  }
                  RefreshLicenseFile();
                  return isExpired;
               }
            }
            set
            {
               isExpired = value;
            }
         }
         public static long DaysLeftToExpire
         {
            get
            {
               lock (lck)
               {
                  if (CheckIfInfoIsCurrent())
                  {
                     if (isExpired)
                     {
                        daysLeftToExpire = 0;
                     }
                     return daysLeftToExpire;

                  }
                  if (isExpired)
                  {
                     daysLeftToExpire = 0;
                     return daysLeftToExpire;
                  }

                  RefreshLicenseFile();
                  return daysLeftToExpire;
               }
            }
            set
            {
               daysLeftToExpire = value;
            }
         }
         public static bool IsPerpetual
         {
            get
            {
               lock (lck)
               {
                  if (CheckIfInfoIsCurrent())
                  {
                     return isPerpetual;

                  }
                  RefreshLicenseFile();
                  return isPerpetual;
               }
            }
            set
            {
               isPerpetual = value;
            }
         }
         public static DateTime ExpirationDate
         {
            get
            {
               lock (lck)
               {
                  if (CheckIfInfoIsCurrent())
                  {
                     return expirationDate;

                  }
                  RefreshLicenseFile();
                  return expirationDate;
               }
            }
            set
            {
               expirationDate = value;
            }
         }
      }

      public const string specialKeyCodesName = "FMSpecialKeyCodes";
      public const string optionsCellName = "FMOptionsCell";
      public const string opcAllowedFunctionsName = "FMOPCAllowedFunctions";
      public const string programVersionName = "FMProgramVersion";
      public const string programVersionNameLIN = "FMProgramVersionLIN";
      public const string word1LIN = "word1LIN";
      public const string word2LIN = "word2LIN";
      public const string licenseInfo = "LicenseInfo";
      public const ushort LicenseFileNotRead = 9999;

      public static Assembly fmUtil = null;
      public static object mainObject = null;
      private static object lck = new object();
      private static ushort UseNewLicenseFile = LicenseFileNotRead;


      [DllImport("kernel32.dll")]
      public static extern IntPtr LoadLibrary(string dllToLoad);

      [DllImport("kernel32.dll")]
      public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

      [DllImport("kernel32.dll")]
      public static extern bool FreeLibrary(IntPtr hModule);

      // fmutil entry definitions
      private delegate int VarecGetLinVersion();
      private delegate ushort VarecGetWord1ValueLIN();
      private delegate ushort VarecGetWord2ValueLIN();
      private delegate long VarecGetOptionsValueLI();
      private delegate long VarecGetSpecialKeycodesLI();
      private delegate long VarecfnGetOPCAllowedFunctionsFM();
      private delegate ushort VarecfnGetProgramVersion();
      private delegate int VarecGetExpiredFlagLIN();
      private delegate int VarecGetLicenseDaysToExpireLIN();
      private delegate void VarecGetExpirationDateLIN(ref long month, ref long day, ref long year);

      static private void ReadHardwareKeyAndSetUseNewLicenseFile()
      {
         lock (lck)
         {
              
            //WriteToEventLog("HardwareKeyClass started", EventLogEntryType.Information);
            object o = AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.UseNewLicenseFile);

            if (o == null)
            {
               WriteToEventLog("Reading HardwareKey", EventLogEntryType.Information);
               HardwareKeyClass hardwareKey = new HardwareKeyClass();
               hardwareKey.ReadHardwareKey();
               o = AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.UseNewLicenseFile);
               if (o == null)
               {
                  WriteToEventLog("HardwareKeyClass UseNewLicenseFile not found in application domain", EventLogEntryType.Error);
                  return;
               }
            }
            ushort oldUseNewLicenseFile = UseNewLicenseFile;
            UseNewLicenseFile = Convert.ToUInt16(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.UseNewLicenseFile));

            if (oldUseNewLicenseFile != UseNewLicenseFile)
            {
               WriteToEventLog(string.Format("HardwareKeyClass UseNewLicenseFile={0}, old UseNewLicenseFile={1}", UseNewLicenseFile, oldUseNewLicenseFile), EventLogEntryType.Information);
            }

         }

      }

      static HardwareKeyClass()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
      }
      /// <summary>
      /// Force a refresh of license key 
      /// </summary>
      public void ForceRefreshLicenseFile()
      {
         LicenseInfoClass.ForceRefreshLicenseFile();
      }

      /// <summary>
      /// Abstraction method that simply returns whether the license key is an Enterprise Key or not
      /// </summary>
      /// <returns>True if the key is a type of Enterprise key.</returns>

      public bool IsEnterpriseKey()
      {

         ReadHardwareKeyAndSetUseNewLicenseFile();
         if (UseNewLicenseFile == LicenseFileNotRead)
         {
            return false;
         }

         if (UseNewLicenseFile == 0)
         {
            if (this.IsDescEnterpriseKey() || this.IsNspaEnterpriseKey())
            {
               return true;
            }
            else
               return false;
         }
         else
         {
            if ((GetWord1ValueLIN() & 0x02) == 0x02)
               return true;
            else
               return false;
         }
      }

      /// <summary>
      /// This method returns true if the license key is marked as a Defense type key
      /// </summary>
      /// <returns>True if the key is marked as a defense type key.</returns>
      public bool IsDefenseKey()
      {
         if (IsDescKey() || IsADFKey() || IsMODKey() || IsTFMDKey())
         {
            return true;
         }

         return false;
      }

      public bool IsDescKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 0)
            {
               uint specialCodes = this.GetSpecialKeyCodes();
               bool descKey = ((specialCodes & 0x00020000) == 0x00020000) || ((specialCodes & 0x00000020) == 0x00000020);
               return descKey;
            }
            else
               return false;
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.SpecialKeyCodesName + " was not found in current application domain.", nre);
         }
      }

      public bool IsDescEnterpriseKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();

            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 0)
            {
               uint specialCodes = this.GetSpecialKeyCodes();
               bool descKey = ((specialCodes & 0x00020000) == 0x00020000);
               return descKey;
            }
            else
               return false;
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.SpecialKeyCodesName + " was not found in current application domain.", nre);
         }
      }

      public bool IsDescProfessionalKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 0)
            {
               uint specialCodes = this.GetSpecialKeyCodes();
               bool descKey = ((specialCodes & 0x00000020) == 0x00000020);
               return descKey;
            }
            else
               return false;
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.SpecialKeyCodesName + " was not found in current application domain.", nre);
         }
      }

      /// <summary>
      /// This method will return true if the Key has been configured for NSPA (NATO)
      /// defense professional key.
      /// </summary>
      /// <returns>Return true if the key is professional NSPA.</returns>
      public bool IsNspaProfessionalKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 0)
            {
               // Looking for x80 in the upper word.
               uint specialCodes = this.GetSpecialKeyCodes();
               bool nspaProfessionalKey = ((specialCodes & 0x08000000) == 0x08000000);
               return nspaProfessionalKey;
            }
            else
               return false;
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.SpecialKeyCodesName + " was not found in current application domain.", nre);
         }
      }

      /// <summary>
      /// This method will return true if the Key has been configured for NSPA (NATO)
      /// defense enterprise key.
      /// </summary>
      /// <returns>Return true if the key is enterprise NSPA.</returns>
      public bool IsNspaEnterpriseKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 0)
            {
               // Looking for x40 in the upper word.
               uint specialCodes = this.GetSpecialKeyCodes();
               bool nspaEnterpriseKey = ((specialCodes & 0x04000000) == 0x04000000);
               return nspaEnterpriseKey;
            }
            else
               return false;
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.SpecialKeyCodesName + " was not found in current application domain.", nre);
         }
      }

      public bool IsADFKey()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
         if (UseNewLicenseFile == LicenseFileNotRead)
         {
            return false;
         }

         if (UseNewLicenseFile == 0)
         {
            uint specialCodes = this.GetSpecialKeyCodes();
            if ((specialCodes & 0x8000) == 0x8000 ||
                (specialCodes & 0x4000) == 0x4000)
               return true;
            else
               return false;
         }
         else
            return false;
      }

      public bool IsMODKey()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
         // not sure what this is and there appears to be nothing in the license to control this. Just setting at false for now.
         return false;
         /*
			uint useNewKey = Convert.ToUInt32(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.UseNewLicenseFile));
			if (useNewKey == 0)
				 return AppSettingsHelper.GetKeyValue<bool>("HardwareKeyIsMODKey", false);
			else
				 return false;
				 */
      }

      public bool IsTFMDKey()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
         // not sure what this is and there appears to be nothing in the license to control this. Just setting at false for now.
         return false;
         /*
			uint useNewKey = Convert.ToUInt32(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.UseNewLicenseFile));
			if (useNewKey == 0)
				 return AppSettingsHelper.GetKeyValue<bool>("HardwareKeyIsTFMDKey", false);
			else
				 return false;
				 */
      }

      public bool IsTacFuelsKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 1)
            {
               if ((this.GetWord1ValueLIN() & 0x1000) == 0x1000)
               {
                  return true;
               }

            }

            return false;

         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.SpecialKeyCodesName + " was not found in current application domain.", nre);
         }
      }

      public bool IsAviationProduct()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
         if (UseNewLicenseFile == LicenseFileNotRead)
         {
            return false;
         }
         if (UseNewLicenseFile == 0)
         {
            uint specialKeyCodes = GetSpecialKeyCodes();
            return (specialKeyCodes & 0x400E) != 0;
         }
         else
            return false;
      }

      public bool IsAnOrderEntryKey()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
         if (UseNewLicenseFile == LicenseFileNotRead)
         {
            return false;
         }

         if (UseNewLicenseFile == 0)
         {
            uint options = this.GetOptionsCell();
            return (options & 0x1000000) == 0x1000000;
         }

         if ((this.GetWord1ValueLIN() & 0x10) == 0x10)
            return true;

         return false;
      }

      /// <summary>
      /// This method checks to see if the hardware has the 5th bit in word 
      /// 2 set which indicates movement capability.
      /// </summary>
      /// <returns>Return turn if the Movement Key is set, otherwise it returns false.</returns>
      public bool IsMovementKey()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
         if (UseNewLicenseFile == LicenseFileNotRead)
         {
            return false;
         }

         //uint useNewKey = Convert.ToUInt32(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.UseNewLicenseFile));

         //if (useNewKey == 0)
         //{
         //	// Look for the 5th bit in the upper word (0000 0000 0010 0000 0000 0000 0000 0000)
         //	uint options = this.GetOptionsCell();
         //	return (options & 0x00200000) == 0x00200000;
         //}

         // The movement key is the 5th (starting at 0) bit in word 2.
         if ((this.GetWord2ValueLIN() & 0x20) == 0x20)
         {
            return true;
         }

         return false;
      }

      /// <summary>
      /// This method checks to see if the hardware has the 3rd bit in word 2
      /// which indicates Leak Detection capability.
      /// </summary>
      /// <returns>Return turn if the Leak Detection Key is set, otherwise it returns false.</returns>
      public bool IsLeakDetectionKey()
      {
         ReadHardwareKeyAndSetUseNewLicenseFile();
         if (UseNewLicenseFile == LicenseFileNotRead)
         {
            return false;
         }
         // The Leak Detection key is the 3rd (starting at 0) bit in word 2.
         if ((this.GetWord2ValueLIN() & 0x8) == 0x8)
         {
            return true;
         }
         return false;
      }
      public bool IsDatawarehouseKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 1)
            {
               if ((this.GetWord1ValueLIN() & 0x4000) == 0x4000)
               {
                  return true;
               }

            }

            return false;

         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.UseNewLicenseFile + " was not found in current application domain.", nre);
         }
      }
      public bool IsDataAnalyticsKey()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 1)
            {
               ushort w = this.GetWord1ValueLIN();
               if ((w & 0x2000) == 0x2000)
               {
                  return true;
               }

            }

            return false;

         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.UseNewLicenseFile + " was not found in current application domain.", nre);
         }
      }
      public void CheckVersion()
      {
         ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();
         consolidatedDA.CheckVersion();
      }

      public ushort CheckActivatedLicenceVersion()
      {
         ReadHardwareKey();

         ushort version = GetProgramVersionLIN();

         // Depends on LoadRackService and WebInventory
         if (version == 0)
         {
            version = GetProgramVersion();
         }

         if (version == 0)
         {
            // this is a case where no key was found or it has not been activated
            throw new FMLicenseException(FMLicenseException.LicenseInvalidOrNotInstalled);
         }
         return version;
      }


      public bool GetLicenseExpired()
      {
         bool expired = true;
         try
         {
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               ReadHardwareKeyAndSetUseNewLicenseFile();
               if (UseNewLicenseFile == LicenseFileNotRead)
               {
                  return expired;
               }
            }
            if (UseNewLicenseFile == 0)
            {
               return false;
            }
            //LicenseInfoClass licenseInfoClass = LicenseInfoClass.GetLicenseInfo();
            expired = LicenseInfoClass.IsExpired;
         }
         catch //(NullReferenceException nre)
         {
            ;// throw new NullReferenceException("A value for name " + ConsolidatedDAClass.OptionsCellName + " was not found in current application domain.", nre);
         }
         return expired;
      }

      private void ValidateLicenseActivated()
      {
         try
         {
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               throw new FMLicenseException(FMLicenseException.LicenseInvalidOrNotInstalled);
            }

            if (UseNewLicenseFile == 1)
            {
               if (GetProgramVersionLIN() == 0)
               {
                  throw new FMLicenseException(FMLicenseException.LicenseInvalidOrNotInstalled);
               }
            }
            else
            {
               uint optionsCell = GetOptionsCellInternal();
               if (optionsCell == 0)
               {
                  throw new FMLicenseException(FMLicenseException.LicenseInvalidOrNotInstalled);
               }
            }
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.UseNewLicenseFile + " was not found in current application domain.", nre);
         }
      }

      private uint GetOptionsCellInternal()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return 0;
            }

            if (UseNewLicenseFile == 0)
               return Convert.ToUInt32(AppDomain.CurrentDomain.GetData(optionsCellName));
            else
               return 0;
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.OptionsCellName + " was not found in current application domain.", nre);
         }
      }


      public long GetLicenseDaysLeftToExpire()
      {
         try
         {

            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return 0;
            }

            return LicenseInfoClass.DaysLeftToExpire;
         }
         catch
         {
            ;// throw new NullReferenceException("A value for name " + ConsolidatedDAClass.OptionsCellName + " was not found in current application domain.", nre);
         }
         return 0;
      }
      public DateTime GetLicenseExpirationDate()
      {
         DateTime expirationDate = DateTime.MinValue;
         try
         {
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               ReadHardwareKeyAndSetUseNewLicenseFile();
               if (UseNewLicenseFile == LicenseFileNotRead)
               {
                  return expirationDate;
               }
            }
            if (UseNewLicenseFile == 0)
            {
               return expirationDate;
            }

            //LicenseInfoClass licenseInfoClass = LicenseInfoClass.GetLicenseInfo();
            expirationDate = LicenseInfoClass.ExpirationDate;
         }
         catch //(NullReferenceException nre)
         {
            ;// throw new NullReferenceException("A value for name " + ConsolidatedDAClass.OptionsCellName + " was not found in current application domain.", nre);
         }
         return expirationDate;
      }

      public uint GetSpecialKeyCodes()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return 0;
            }

            if (UseNewLicenseFile == 0)
               return (uint)AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.SpecialKeyCodesName);
            else
               return 0;
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.SpecialKeyCodesName + " was not found in current application domain.", nre);
         }
      }

      public bool IsMultipleSiteKey()
      {
         // this depends upon whether it is a new key or not
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return false;
            }

            if (UseNewLicenseFile == 1)
            {
               if ((GetWord1ValueLIN() & 0x01) == 0x01)
                  return true;
               else
                  return false;
            }
            else
            {
               return ((GetOptionsCell() & 0x800000) == 0x800000);
            }
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.UseNewLicenseFile + " was not found in current application domain.", nre);
         }
      }

      public uint GetOptionsCell()
      {
         ValidateLicenseActivated();
         return GetOptionsCellInternal();

      }

      public ushort GetProgramVersion()
      {
         ushort returnValue = 0;
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return 0;
            }

            if (UseNewLicenseFile == 1)  // use new file
            {
               returnValue = Convert.ToUInt16(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.ProgramVersionNameLIN));
            }
            else
            {
               returnValue = Convert.ToUInt16(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.ProgramVersionName));
            }

            return returnValue;


         }
         catch (Exception nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.ProgramVersionName + " was not found in current application domain.", nre);
         }
      }

      public ushort GetUseNewLicenseFile()
      {
         try
         {
            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return 0;
            }

            return UseNewLicenseFile;

         }
         catch (Exception nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.UseNewLicenseFile + " was not found in current application domain.", nre);
         }
      }
      public ushort GetProgramVersionLIN()
      {
         //ValidateLicenseActivated();
         try
         {
            if (AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.ProgramVersionNameLIN) == null)
            {
               ConsolidatedDAClass.ReadHardwareKey();
            }

            return Convert.ToUInt16(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.ProgramVersionNameLIN));

         }
         catch (Exception nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.ProgramVersionNameLIN + " was not found in current application domain.", nre);
         }
      }
      public ushort GetWord1ValueLIN()
      {
         ValidateLicenseActivated();
         try
         {
            if (AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.word1LIN) == null)
            {
               ConsolidatedDAClass.ReadHardwareKey();
            }

            return Convert.ToUInt16(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.word1LIN));

         }
         catch (Exception nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.word1LIN + " was not found in current application domain.", nre);
         }
      }
      public ushort GetWord2ValueLIN()
      {
         ValidateLicenseActivated();
         try
         {
            if (AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.word2LIN) == null)
            {
               ConsolidatedDAClass.ReadHardwareKey();
            }

            return Convert.ToUInt16(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.word2LIN));

         }
         catch (Exception nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.word2LIN + " was not found in current application domain.", nre);
         }
      }
      public uint GetOPCAllowedFunctions()
      {
         ValidateLicenseActivated();
         try
         {

            ReadHardwareKeyAndSetUseNewLicenseFile();
            if (UseNewLicenseFile == LicenseFileNotRead)
            {
               return 0;
            }

            if (UseNewLicenseFile == 0)
               return Convert.ToUInt32(AppDomain.CurrentDomain.GetData(ConsolidatedDAClass.OpcAllowedFunctionsName));
            else
               return 0x230;   // enables opc server and client for load rack stuff
         }
         catch (NullReferenceException nre)
         {
            throw new NullReferenceException("A value for name " + ConsolidatedDAClass.OpcAllowedFunctionsName + " was not found in current application domain.", nre);
         }
      }

      public static bool LicenseExpiredLIN(IntPtr hFMutilDLL)
      {

         IntPtr iplIPtrVarecGetExpiredFlagLIN = GetProcAddress(hFMutilDLL, "VarecGetExpiredFlagLIN");

         if (iplIPtrVarecGetExpiredFlagLIN == IntPtr.Zero)
         {
            throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
         }

         VarecGetExpiredFlagLIN getExpireFlag = (VarecGetExpiredFlagLIN)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecGetExpiredFlagLIN, typeof(VarecGetExpiredFlagLIN));

         long expired = (long)getExpireFlag();


         return expired == 1;

      }

      public static long LicenseDaysLeftToExpireLIN(IntPtr hFMutilDLL)
      {

         IntPtr iplIPtrVarecGetLicenseDaysToExpireLIN = GetProcAddress(hFMutilDLL, "VarecGetLicenseDaysToExpireLIN");

         if (iplIPtrVarecGetLicenseDaysToExpireLIN == IntPtr.Zero)
         {
            throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
         }

         VarecGetLicenseDaysToExpireLIN getDaysLeftToExpire = (VarecGetLicenseDaysToExpireLIN)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecGetLicenseDaysToExpireLIN, typeof(VarecGetLicenseDaysToExpireLIN));

         long daysLeftToExpire = (long)getDaysLeftToExpire();


         return daysLeftToExpire;

      }

      public static DateTime LicenseExpirationDateLIN(IntPtr hFMutilDLL)
      {

         IntPtr iplIPtrVarecGetExpirationDateLIN = GetProcAddress(hFMutilDLL, "VarecGetExpirationDateLIN");

         if (iplIPtrVarecGetExpirationDateLIN == IntPtr.Zero)
         {
            throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
         }

         VarecGetExpirationDateLIN getExpirationDate = (VarecGetExpirationDateLIN)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecGetExpirationDateLIN, typeof(VarecGetExpirationDateLIN));

         long day = 1, month = 1, year = 2024;
         getExpirationDate(ref month, ref day, ref year);

         if (year < 2024 || month <= 0 || month > 12 || day < 1 || day > 31)
         {
            return DateTime.MinValue;
         }
         DateTime expirationDate = new DateTime((int)year, (int)month, (int)day, 0, 0, 0);


         return expirationDate;

      }

      public void ReadHardwareKey()
      {
         AppDomain Domain = AppDomain.CurrentDomain;

         uint optionsCell = 0;
         uint specialKeyCodes = 0;
         uint OPCAllowedFunctions = 0;
         ushort programVersion = 0;
         ushort programVersionLIN = 0;
         ushort wordValue1 = 0;
         ushort wordValue2 = 0;
         ushort useNewLicense = 0;
         string fmutilPath = ConfigurationManager.AppSettings["FMUtilPath"];

         if (string.IsNullOrEmpty(fmutilPath) == true || Directory.Exists(fmutilPath) == false)
         {
            throw new Exception("Error retrieving FMUtil path.");
         }
         // have to love Micorsoft. You can specify a path when loading the Dll but it does not check that directory for any dependencies. So set the directory here first.
         // FMUtil.Dll is dependent on KeyLib32.dll
         string currentPath = Directory.GetCurrentDirectory();
         Directory.SetCurrentDirectory(fmutilPath);

         // load the fmutil.dll directly into the application
         IntPtr hFMutilDLL = LoadLibrary("fmutil.dll");

         if (hFMutilDLL == IntPtr.Zero)
         {
            Directory.SetCurrentDirectory(currentPath);
            throw new Exception("Error loading FMUtil.Dll.");
         }
         try
         {
            IntPtr iplIPtrVarecGetLinVersion = GetProcAddress(hFMutilDLL, "VarecGetLinVersion");

            if (iplIPtrVarecGetLinVersion == IntPtr.Zero)
            {
               throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
            }

            // get the lin file version
            VarecGetLinVersion getLINVersion = (VarecGetLinVersion)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecGetLinVersion, typeof(VarecGetLinVersion));

            programVersionLIN = (ushort)getLINVersion();

            if (programVersionLIN != 0) // 0 is returned if the new license file cannot be read
               useNewLicense = 1;

            lock (lck)
            {

               if (useNewLicense == 1)
               {
                  LicenseInfoClass.isExpired = LicenseExpiredLIN(hFMutilDLL);
                  LicenseInfoClass.daysLeftToExpire = LicenseDaysLeftToExpireLIN(hFMutilDLL);
                  LicenseInfoClass.expirationDate = LicenseExpirationDateLIN(hFMutilDLL);
               }
               else
               {
                  LicenseInfoClass.isExpired = false;
                  LicenseInfoClass.daysLeftToExpire = 99999;
                  LicenseInfoClass.expirationDate = new DateTime(2100, 1, 1, 0, 0, 0);
               }

               Domain.SetData(programVersionNameLIN, programVersionLIN);

               Domain.SetData(ConsolidatedDAClass.UseNewLicenseFile, useNewLicense);
            }

            if (useNewLicense == 1)
            {
               // set word 1 value VarecGetWord1ValueLIN
               IntPtr iplIPtrVarecGetWord1ValueLIN = GetProcAddress(hFMutilDLL, "VarecGetWord1ValueLIN");

               if (iplIPtrVarecGetWord1ValueLIN == IntPtr.Zero)
               {
                  throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
               }

               // get the lin file version
               VarecGetWord1ValueLIN getWord1Value = (VarecGetWord1ValueLIN)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecGetWord1ValueLIN, typeof(VarecGetWord1ValueLIN));

               wordValue1 = (ushort)getWord1Value();

               Domain.SetData(word1LIN, wordValue1);

               // set word 2 value VarecGetWord2ValueLIN
               IntPtr iplIPtrVarecGetWord2ValueLIN = GetProcAddress(hFMutilDLL, "VarecGetWord2ValueLIN");

               if (iplIPtrVarecGetWord2ValueLIN == IntPtr.Zero)
               {
                  throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
               }

               // get the lin file version
               VarecGetWord2ValueLIN getWord2Value = (VarecGetWord2ValueLIN)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecGetWord2ValueLIN, typeof(VarecGetWord2ValueLIN));

               wordValue2 = (ushort)getWord2Value();

               Domain.SetData(word2LIN, wordValue2);

            }
            else // old license key found. This needs to be removed after the old web apps have been updated just as a sample on how to do
            {
               // old file section

               // get the options cell
               IntPtr iplIPtrVarecVarecGetOptionsValueLILI = GetProcAddress(hFMutilDLL, "VarecGetOptionsValueLI");

               if (iplIPtrVarecVarecGetOptionsValueLILI == IntPtr.Zero)
               {
                  throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
               }
               // get the li option cell value
               VarecGetOptionsValueLI getOptionValue = (VarecGetOptionsValueLI)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecVarecGetOptionsValueLILI, typeof(VarecGetOptionsValueLI));

               optionsCell = (uint)getOptionValue();

               Domain.SetData(optionsCellName, optionsCell);

               if (optionsCell == 0)
               {
                  // if options cell is 0 just set the others and return no reason to read the others.
                  Domain.SetData(specialKeyCodesName, (uint)0);
                  Domain.SetData(opcAllowedFunctionsName, (uint)0);
                  Domain.SetData(programVersionName, (ushort)0);
               }
               else
               {
                  // get the special keycodes 
                  IntPtr iplIPtrVarecVarecGetSpecialValueLILI = GetProcAddress(hFMutilDLL, "VarecGetSpecialKeycodesLI");

                  if (iplIPtrVarecVarecGetSpecialValueLILI == IntPtr.Zero)
                  {
                     throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
                  }

                  VarecGetSpecialKeycodesLI getSpecialValue = (VarecGetSpecialKeycodesLI)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecVarecGetSpecialValueLILI, typeof(VarecGetSpecialKeycodesLI));

                  specialKeyCodes = (uint)getSpecialValue();
                  Domain.SetData(specialKeyCodesName, specialKeyCodes);

                  // get the allowed OPC configuration

                  IntPtr iplIPtrVarecVarecGetOPCConfValueLILI = GetProcAddress(hFMutilDLL, "VarecfnGetOPCAllowedFunctionsFMLI");

                  if (iplIPtrVarecVarecGetOPCConfValueLILI == IntPtr.Zero)
                  {
                     throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
                  }

                  VarecfnGetOPCAllowedFunctionsFM getOPCModulesValue = (VarecfnGetOPCAllowedFunctionsFM)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecVarecGetOPCConfValueLILI, typeof(VarecfnGetOPCAllowedFunctionsFM));

                  OPCAllowedFunctions = (uint)getOPCModulesValue();
                  Domain.SetData(opcAllowedFunctionsName, OPCAllowedFunctions);

                  // get the program version

                  IntPtr iplIPtrVarecVarecGetProgramVersionValueLI = GetProcAddress(hFMutilDLL, "VarecfnGetProgramVersionLI");

                  if (iplIPtrVarecVarecGetProgramVersionValueLI == IntPtr.Zero)
                  {
                     throw new Exception("FMUtil.Dll 4.0.0.5 or greater is required.");
                  }

                  VarecfnGetProgramVersion getProgramVersionValue = (VarecfnGetProgramVersion)Marshal.GetDelegateForFunctionPointer(iplIPtrVarecVarecGetProgramVersionValueLI, typeof(VarecfnGetProgramVersion));

                  programVersion = (ushort)getProgramVersionValue();
                  Domain.SetData(programVersionName, (ushort)programVersion);
 
               }
            }
         }
         catch
         {

         }
         finally
         {
            if (hFMutilDLL != IntPtr.Zero)
            {
               FreeLibrary(hFMutilDLL);
            }
            Directory.SetCurrentDirectory(currentPath);
         }
      }
      public static void WriteToEventLog(string message, EventLogEntryType entryType, int eventID = 0)
      {
         try
         {
            using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
            {
               eventLog.WriteEntry("EventLog-" + message, entryType, eventID);
            }
         }
         catch
         {
            var level = TraceLevel.Verbose;
            switch (entryType)
            {
               case EventLogEntryType.Error:
                  level = TraceLevel.Error;
                  break;
               case EventLogEntryType.Warning:
                  level = TraceLevel.Warning;
                  break;
               case EventLogEntryType.Information:
               case EventLogEntryType.SuccessAudit:
               case EventLogEntryType.FailureAudit:
                  level = TraceLevel.Info;
                  break;
            }
            MyTrace("[EventLog fallback]" + message, level);
         }
      }
      private static void MyTrace(string message, TraceLevel level)
      {
         try
         {
            switch (level)
            {
               case TraceLevel.Error:
                  Trace.TraceError(message);
                  break;

               case TraceLevel.Warning:
                  Trace.TraceWarning(message);
                  break;

               case TraceLevel.Info:
                  Trace.TraceInformation(message);
                  break;

               default:
                  Trace.WriteLine(message);
                  break;

            }
         }
         catch
         {
            ;
         }
      }
   }

}
