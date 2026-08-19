using System;

using FMBusinessObjects.DataObjects;

namespace FuelsManager.DispatchWebApp
{
	/// <summary>
	/// Common hidden fields used to preserve property values across postbacks
	/// </summary>
	public partial class FMDispatchHiddenFields : System.Web.UI.UserControl
	{

		/// <summary>
		/// Gets or sets the identity guid of the current dispatch configuration
		/// </summary>
		public Guid DispatchConfigurationGuid
		{
			get
			{
				Guid configGuid;
				bool valid = Guid.TryParse(dispatchConfigurationGuidField.Value, out configGuid);
				return valid ? configGuid : Guid.Empty;
			}

			set
			{
				dispatchConfigurationGuidField.Value = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized list of transaction alias names
		/// </summary>
		public string JsonTransactionAliasNames
		{
			get
			{
				return jsonTransactionAliasNamesField.Value;
			}

			set
			{
				jsonTransactionAliasNamesField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized list of transaction status values
		/// </summary>
		public string JsonTransactionStatusValues
		{
			get
			{
				return jsonTransactionStatusValuesField.Value;
			}

			set
			{
				jsonTransactionStatusValuesField.Value = value;
			}
		}


		/// <summary>
		/// Gets or sets the JSON serialized list of operational lock date values
		/// </summary>
		public string JsonOperationalLockDateValue
		{
			get
			{
				return this.jsonOperationalLockDateField.Value;
			}

			set
			{
				this.jsonOperationalLockDateField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized list of optional times arrival flag values
		/// </summary>
		public string JsonOptionalTimesArrivalFlagValue
		{
			get
			{
				return this.jsonOptionalTimesArrivalFlagField.Value;
			}

			set
			{
				this.jsonOptionalTimesArrivalFlagField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized list of optional times start flag values
		/// </summary>
		public string JsonOptionalTimesStartFlagValue
		{
			get
			{
				return this.jsonOptionalTimesStartFlagField.Value;
			}

			set
			{
				this.jsonOptionalTimesStartFlagField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized list of optional times start flag values
		/// </summary>
		public string JsonOptionalTimesStopFlagValue
		{
			get
			{
				return this.jsonOptionalTimesStopFlagField.Value;
			}

			set
			{
				this.jsonOptionalTimesStopFlagField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized tabular view grid column definitions
		/// </summary>
		public string JsonTabularGridColumnDefinitions
		{
			get
			{
				return jsonTabularGridColumnDefinitionsField.Value;
			}

			set
			{
				jsonTabularGridColumnDefinitionsField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized equipment grid column definitions
		/// </summary>
		public string JsonEquipmentGridColumnDefinitions
		{
			get
			{
				return jsonEquipmentGridColumnDefinitionsField.Value;
			}

			set
			{
				jsonEquipmentGridColumnDefinitionsField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized personnel grid column definitions
		/// </summary>
		public string JsonPersonnelGridColumnDefinitions
		{
			get
			{
				return jsonPersonnelGridColumnDefinitionsField.Value;
			}

			set
			{
				jsonPersonnelGridColumnDefinitionsField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized request grid column definitions
		/// </summary>
		public string JsonRequestGridColumnDefinitions
		{
			get
			{
				return jsonRequestGridColumnDefinitionsField.Value;
			}

			set
			{
				jsonRequestGridColumnDefinitionsField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the JSON serialized list of toolbar button names
		/// </summary>
		public string JsonToolbarButtonList
		{
			get
			{
				return jsonToolbarButtonListField.Value;
			}

			set
			{
				jsonToolbarButtonListField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the address of the dispatch request service
		/// </summary>
		public string DispatchRequestServiceAddress
		{
			get
			{
				return dispatchRequestServiceAddressField.Value;
			}

			set
			{
				dispatchRequestServiceAddressField.Value = value;
			}
		}

		/// <summary>
		/// Gets or sets the enable service requests flag
		/// </summary>
		public bool EnableServiceRequests
		{
			get
			{
				bool enableRequests;
				bool valid = bool.TryParse(enableServiceRequestsField.Value, out enableRequests);
				return valid && enableRequests;
			}

			set
			{
				enableServiceRequestsField.Value = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the dispatch service request refresh period
		/// </summary>
		public int ServiceRequestRefreshPeriod
		{
			get
			{
				int refreshPeriod;
				bool valid = int.TryParse(serviceRequestRefreshPeriodField.Value, out refreshPeriod);
				return valid ? refreshPeriod : DispatchConfigurationClass.DefaultDataRefreshPeriod;
			}

			set
			{
				serviceRequestRefreshPeriodField.Value = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the dispatch service request automatic restart delay
		/// </summary>
		public int ServiceRequestAutomaticRestartDelay
		{
			get
			{
				int restartDelayPeriod;
				bool valid = int.TryParse(serviceRequestAutomaticRestartDelayField.Value, out restartDelayPeriod);
				return valid ? restartDelayPeriod : DispatchConfigurationClass.DefaultAutomaticRestartDelay;
			}

			set
			{
				serviceRequestAutomaticRestartDelayField.Value = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the display current time flag
		/// </summary>
		public bool DisplayCurrentTime
		{
			get
			{
				bool displayTime;
				bool valid = bool.TryParse(displayCurrentTimeField.Value, out displayTime);
				return valid && displayTime;
			}

			set
			{
				displayCurrentTimeField.Value = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the DisplayMilitaryJulianDate flag
		/// </summary>
		public bool DisplayMilitaryJulianDate
		{
			get
			{
				bool displayJulianDate;
				bool valid = bool.TryParse(displayMilitaryJulianDateField.Value, out displayJulianDate);
				return valid && displayJulianDate;
			}

			set
			{
				displayMilitaryJulianDateField.Value = value.ToString();
			}
		}

	}
}