namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Linq;
    using System.Web.Configuration;
	using System.Xml.Linq;

	using FMBusinessObjects.LogClient;

    public class EnumerationLimits
	{
		#region Options
		public enum EnumerationOptions
		{
			DEFAULT,
			FUEL_CARD,
			EQUIPMENT,
			COMPANY,
			PRODUCT,
			PERSON,
			STANDING_OFFER,
			FUEL_CARD_ASSIGNMENT,
			ASSET_TRACKING_DEVICE
		}
		#endregion // Options

		#region Attributes
		protected XDocument m_doc;
		protected Logger m_logger;
		#endregion // Attributes

		#region Construction
		public EnumerationLimits ( )
		{
			this.m_doc = null;
			this.m_logger = new Logger ( "Accounting" );
		}
		#endregion // Construction

		#region Enumeration
		public int GetDefault ( )
		{
			return GetLimit ( EnumerationOptions.DEFAULT );
		}

		public int GetLimit ( EnumerationOptions a_option )
		{
			string key;
			
			// figure out the key to look for
			switch (a_option)
			{
				case EnumerationOptions.EQUIPMENT:
					key = "Equipmentlimit";
					break;
				case EnumerationOptions.FUEL_CARD:
					key = "FuelCardlimit";
					break;
				case EnumerationOptions.COMPANY:
					key = "Companylimit";
					break;
				case EnumerationOptions.PRODUCT:
					key = "Productlimit";
					break;
				case EnumerationOptions.PERSON:
					key = "Personlimit";
					break;
				case EnumerationOptions.STANDING_OFFER:
					key = "StandingOfferlimit";
					break;
				case EnumerationOptions.FUEL_CARD_ASSIGNMENT:
					key = "FuelCardlimitAssignmentLimit";
					break;
				case EnumerationOptions.ASSET_TRACKING_DEVICE:
					key = "AssetTrackingDeviceLimit";
					break;
				default:
					key = "Defaultlimit";
					break;
			}
			string appPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

			Configuration config = WebConfigurationManager.OpenWebConfiguration (appPath);
            string value = "1500";
			int ret = 0;

			// Make sure that the key is contained in the collection before extracting 
			// the data.
			if (config.AppSettings.Settings.AllKeys.Contains<string> ( key ))
			{
				value = config.AppSettings.Settings[key].Value;
			}

			if (string.IsNullOrEmpty ( value ) == false)
			{
				ret = Convert.ToInt32 ( value );
			}
			else
			{
				this.m_logger.Error ( "EnumerationLimits.GetLimit() was not able to retrieve valid limit for ID " + key );
			}

			return ret;
		}

		public Hashtable Enumerate ( )
		{
			var result = new Hashtable ( );
			foreach (EnumerationOptions option in Enum.GetValues ( typeof ( EnumerationOptions ) ))
			{
				result.Add ( option, this.GetLimit ( option ) );
			}

			return result;
		}
		#endregion // Enumeration
	}
}
