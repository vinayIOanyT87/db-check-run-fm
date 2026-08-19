namespace FuelsManager.Areas.Controllers
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using System;
    using System.Web.Mvc;

    public static class FMHelpers
    {
        public static string ConvertDateFormat( this HtmlHelper html, string format )
        {
            /*
			 *  Date used in this comment : 5th - Nov - 2009 (Thursday)
			 *
			 *  .NET    JQueryUI        Output      Comment
			 *  --------------------------------------------------------------
			 *  d       d               5           day of month(No leading zero)
			 *  dd      dd              05          day of month(two digit)
			 *  ddd     D               Thu         day short name
			 *  dddd    DD              Thursday    day long name
			 *  M       m               11          month of year(No leading zero)
			 *  MM      mm              11          month of year(two digit)
			 *  MMM     M               Nov         month name short
			 *  MMMM    MM              November    month name long.
			 *  yy      y               09          Year(two digit)
			 *  yyyy    yy              2009        Year(four digit)             *
			 */

			string currentFormat = format;

			// Convert the date
			currentFormat = currentFormat.Replace( "dddd", "DD" );
			currentFormat = currentFormat.Replace( "ddd", "D" );

			// Convert month
			if ( currentFormat.Contains( "MMMM" ) )
			{
				currentFormat = currentFormat.Replace( "MMMM", "MM" );
			}
			else if ( currentFormat.Contains( "MMM" ) )
			{
				currentFormat = currentFormat.Replace( "MMM", "M" );
			}
			else if ( currentFormat.Contains( "MM" ) )
			{
				currentFormat = currentFormat.Replace( "MM", "mm" );
			}
			else
			{
				currentFormat = currentFormat.Replace( "M", "m" );
			}

			// Convert year
			currentFormat = currentFormat.Contains("yyyy")
				? currentFormat.Replace("yyyy", "yy")
				: currentFormat.Replace("yy", "y");

			return currentFormat;
		}



		public static void GetLicenseStatusInfo(SecurityClass security,object licenseDaysLeftToExpire, object licenseExpirationDate,  out string licenseStatusMessage, out string licenseStatusStyle)
		{
            licenseStatusMessage = string.Empty;
            licenseStatusStyle = string.Empty;
            if(security == null)
            {
                return;
            }
            try
            {
                if (licenseDaysLeftToExpire != null)
                {
                    long daysLeft = (long)licenseDaysLeftToExpire;

                    if (daysLeft <= 90)
                    {
                        if (licenseExpirationDate != null && licenseExpirationDate is System.DateTime)
                        {
                            System.DateTime expirationDate = (System.DateTime)licenseExpirationDate;

                            // Load site to get SiteGroup value
                            SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                                                     x =>
                                                                                     x.GetUsingGuid(security, security.SiteGuid)
                                                                                );
                            var dateTimeFormat = site.GetDateTimeFormatInfo();
                            var formatedDate = expirationDate.ToString("d", dateTimeFormat);
                            ;
                            licenseStatusMessage = string.Format("FM License will expire in {0} day{1} on {2}", daysLeft, daysLeft == 1 ? string.Empty : "s", formatedDate);
                            if (daysLeft <= 30)
                            {
                                licenseStatusStyle = "#ff5d5d";// "red";
                                if (daysLeft <= 0)
                                {
                                    bool licenseExpired = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.GetLicenseExpired());

                                    if (licenseExpired)
                                    {
                                        licenseStatusMessage = string.Format("FM License expired on {0}", formatedDate);
                                    }
                                    else
                                    {
                                        licenseStatusMessage = "FM License expires today";
                                    }

                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry("LicenseExpirationDate missing from Session variable.", FMEventLogEntryType.Warning));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry("LicenseDaysLeftToExpire missing from Session variable.", FMEventLogEntryType.Warning));
                    }
                    catch
                    {
                    }
                }

            }
            catch (FMLicenseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(ex.Message, FMEventLogEntryType.Warning));
            }
        }
	}
}
