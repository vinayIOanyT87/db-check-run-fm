namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Runtime.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
	[DataContract]
	public struct Site
	{
		public Site(string name, Guid identityGuid)
		{
			this.Name = name;
			this.IdentityGuid = identityGuid;
		}

		[DataMember]
		public string Name;
		[DataMember]
		public Guid IdentityGuid;
	}

	[Serializable]
	[DataContract]
	[KnownType(typeof(Site))]
	public class AccountingSite
	{
		#region Public Attributes
		public enum ConversionUnits { LEVEL, VOLUME, TEMPERATURE, DENSITY, MASS, FLOW, PRESSURE, ADDITIVE_VOLUME }
		#endregion

		#region Private Attributes

		[DataMember]
		private ArrayList siteList;

		[DataMember]
		private List<Guid> userCompanyList;

		[DataMember]
		private bool hasViewPermissionForAllCompanies;

		[DataMember]
		private bool getUserCompanies;

		[DataMember]
		private string currentSiteName;

		[DataMember]
		private Guid currentSiteGuid;

		[DataMember]
		private SiteClass currentSite;

		[DataMember]
		private SiteClass loginSite;

		[DataMember]
		private SIDouble siVolume;

		[DataMember]
		private SIDouble siAdditiveVolume;

		[DataMember]
		private SIDouble siFlow;

		[DataMember]
		private SIDouble siLevel;

		[DataMember]
		private SIDouble siTemperature;

		[DataMember]
		private SIDouble siDensity;

		[DataMember]
		private SIDouble siMass;

		[DataMember]
		private SIDouble siPressure;
		#endregion

		/// <summary>
		/// This is the default constructor for the accounting site class.
		/// </summary>
		public AccountingSite()
		{
			this.Init();
		}

		/// <summary>
		/// This property gets the Site List.
		/// </summary>
		public ArrayList SiteList
		{
			get { return this.siteList; }
		}

		public EngineeringUnit VolumeUnits
		{
			get { return this.siVolume.Units; }
		}

		/// <summary>
		/// This property gets the flag that indicates
		/// whether the site is a site group (true) or not (false).
		/// </summary>
		public bool IsSiteGroup
		{
			get { return (this.CurrentSite != null) && this.CurrentSite.SiteGroup; }
		}

		/// <summary>
		/// This property sets and gets the main site name.
		/// </summary>
		public string CurrentSiteName
		{
			get { return this.currentSiteName; }
			set { currentSiteName = value; }
		}

		/// <summary>
		/// This property sets and gets the main site guid.
		/// </summary>
		public Guid CurrentSiteGuid
		{
			get { return this.currentSiteGuid; }
			set { currentSiteGuid = value; }
		}

		/// <summary>
		/// This property property sets and gets the current site object.
		/// </summary>
		public SiteClass CurrentSite
		{
			get { return this.currentSite; }
			set { currentSite = value; }
		}

		public SiteClass LoginSite
		{
			get { return this.loginSite; }
			set { loginSite = value; }
		}

		/// <summary>
		/// This property will return a list of companies that the user has
		/// permissions to view. It will be empty if the user has permissions
		/// to view all.
		/// </summary>
		public List<Guid> UserCompanyList
		{
			get { return this.userCompanyList; }
		}

		/// <summary>
		/// This property will return true if the user has permission to view
		/// all companies.  Otherwise, it returns false.
		/// </summary>
		public bool HasViewPermissionForAllCompanies
		{
			get { return this.hasViewPermissionForAllCompanies; }
			set { this.hasViewPermissionForAllCompanies = value; }
		}

		/// <summary>
		/// This property will return true if the load method needs to get
		/// the companies associated to a user. It will return false,
		/// indicating not to retrieve companies associated to a user.
		/// </summary>
		public bool GetUserCompanies
		{
			get { return this.getUserCompanies; }
			set { this.getUserCompanies = value; }
		}

		[DataMember]
		public SecurityClass Security { get; set; }

		/// <summary>
		/// This method will format a data object to a string based on its type.
		/// </summary>
		/// <param name="inObject"></param>
		/// <returns>Returns a string version.</returns>
		public string FormatDataObject(object inObject)
		{
			var type = inObject.GetType();

			if (type == typeof(string))
			{
				return (string)inObject;
			}

			if (type == typeof(double))
			{
				return ((double)inObject).ToString(CultureInfo.InvariantCulture);
			}

			if (type == typeof(bool)) 
			{
				return ((bool)inObject).ToString();
			}

			if (type == typeof(DateTime))
			{
				return this.FormatDate((DateTime)inObject);
			}

			if (type == typeof(DateTimeOffset))
			{
				return this.FormatDate((DateTimeOffset)inObject);
			}

			if (type == typeof(TransactionStatus) || type == typeof(bool)  || type == typeof(Guid))
			{
				return inObject.ToString();
			}

			throw new NotImplementedException("AccountingSite.format(" + type + ") not implemented.");
		}

		/// <summary>
		/// This method will convert the date from the site setting to the following format:
		/// yyyy-MM-dd. It returns a string representing the date.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		public string UnformatDate(string inDate)
		{
			string unFormattedDateStr = string.Empty;

			if (!string.IsNullOrEmpty(inDate))
			{
				DateTimeOffset unFormattedDate = DateTimeOffset.Parse(inDate, this.currentSite.GetDateTimeFormatInfo());
				unFormattedDateStr = unFormattedDate.ToString("yyyy-MM-dd");
			}

			return unFormattedDateStr;
		}

		/// <summary>
		/// This method will convert the datetime from the site setting to the following format:
		/// yyyy-MM-dd HH:mm:ss. It returns a string representing the datetime.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		public string UnformatDateTime(string inDate)
		{
			string unFormattedDateStr = string.Empty;

			if (!string.IsNullOrEmpty(inDate))
			{
				DateTimeOffset unFormattedDate = DateTimeOffset.Parse(inDate, this.currentSite.GetDateTimeFormatInfo());
				unFormattedDateStr = unFormattedDate.ToString("yyyy-MM-dd HH:mm:ss");
			}

			return unFormattedDateStr;
		}

		/// <summary>
		/// This method will return the date formatted to the site's settings in date only format.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		public string FormatDate(DateTimeOffset inDate)
		{
			DateTimeFormatInfo dateTimeFormat = this.currentSite.GetDateTimeFormatInfo();
			return inDate.ToString("d", dateTimeFormat);
		}

		/// <summary>
		/// This method will return the date formatted to the site's settings in date/time format.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		public string FormatDateTime(DateTimeOffset inDate)
		{
			var pattern = this.currentSite.ShortDatePattern + " " + this.currentSite.TimePattern;
			return inDate.ToString(pattern);
		}

		/// <summary>
		/// This method will return the time formatted to the site's settings in time only format.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		public string FormatTime(DateTimeOffset inDate)
		{
			DateTimeFormatInfo dateTimeFormat = this.currentSite.GetDateTimeFormatInfo();
			return inDate.ToString("T", dateTimeFormat);
		}

		/// <summary>
		/// This method will convert a double number to the SI
		/// engineering unit. The original number and unit must be specified.
		/// It returns a converted double value.
		/// </summary>
		/// <param name="origNumber"></param>
		/// <param name="whichUnit"></param>
		/// <returns></returns>
		public double ConvertToSi(double origNumber, ConversionUnits whichUnit)
		{
			switch (whichUnit)
			{
				case ConversionUnits.VOLUME:
					this.siVolume.Value = origNumber;
					return this.siVolume.SIValue;

				case ConversionUnits.ADDITIVE_VOLUME:
					this.siAdditiveVolume.Value = origNumber;
					return this.siAdditiveVolume.SIValue;

				case ConversionUnits.FLOW:
					this.siFlow.Value = origNumber;
					return this.siFlow.SIValue;

				case ConversionUnits.LEVEL:
					this.siLevel.Value = origNumber;
					return this.siLevel.SIValue;

				case ConversionUnits.TEMPERATURE:
					this.siTemperature.Value = origNumber;
					return this.siTemperature.SIValue;

				case ConversionUnits.DENSITY:
					this.siDensity.Value = origNumber;
					return this.siDensity.SIValue;

				case ConversionUnits.MASS:
					this.siMass.Value = origNumber;
					return this.siMass.SIValue;

				case ConversionUnits.PRESSURE:
					this.siPressure.Value = origNumber;
					return this.siPressure.SIValue;

				default:
					return origNumber;
			}
		}

		/// <summary>
		/// This method will convert a double number to the current site's
		/// engineering unit. The original number and unit must be specified.
		/// It returns a converted double value.
		/// </summary>
		/// <param name="origNumber"></param>
		/// <param name="whichUnit"></param>
		/// <returns></returns>
		public double ConvertFromSi(double origNumber, ConversionUnits whichUnit)
		{
			switch (whichUnit)
			{
				case ConversionUnits.VOLUME:
					this.siVolume.SIValue = origNumber;
					return this.siVolume.Value;

				case ConversionUnits.ADDITIVE_VOLUME:
					this.siAdditiveVolume.SIValue = origNumber;
					return this.siAdditiveVolume.Value;

				case ConversionUnits.FLOW:
					this.siFlow.SIValue = origNumber;
					return this.siFlow.Value;

				case ConversionUnits.LEVEL:
					this.siLevel.SIValue = origNumber;
					return this.siLevel.Value;

				case ConversionUnits.TEMPERATURE:
					this.siTemperature.SIValue = origNumber;
					return this.siTemperature.Value;

				case ConversionUnits.DENSITY:
					this.siDensity.SIValue = origNumber;
					return this.siDensity.Value;

				case ConversionUnits.MASS:
					this.siMass.SIValue = origNumber;
					return this.siMass.Value;

				case ConversionUnits.PRESSURE:
					this.siPressure.SIValue = origNumber;
					return this.siPressure.Value;

				default:
					return origNumber;
			}
		}

		/// <summary>
		/// This method will convert a double number to the SI
		/// engineering unit. The original number and unit must be specified.
		/// It returns a formatted string representation.
		/// </summary>
		/// <param name="origNumber"></param>
		/// <param name="whichUnit"></param>
		/// <returns></returns>
		public string ConvertToSiFormmatted(double origNumber, ConversionUnits whichUnit)
		{
			switch (whichUnit)
			{
				case ConversionUnits.VOLUME:
					this.siVolume.SIValue = origNumber;
					return this.siVolume.ToString();

				case ConversionUnits.ADDITIVE_VOLUME:
					this.siAdditiveVolume.SIValue = origNumber;
					return this.siAdditiveVolume.ToString();

				case ConversionUnits.FLOW:
					this.siFlow.SIValue = origNumber;
					return this.siFlow.ToString();

				case ConversionUnits.LEVEL:
					this.siLevel.SIValue = origNumber;
					return this.siLevel.ToString();

				case ConversionUnits.TEMPERATURE:
					this.siTemperature.SIValue = origNumber;
					return this.siTemperature.ToString();

				case ConversionUnits.DENSITY:
					this.siDensity.SIValue = origNumber;
					return this.siDensity.ToString();

				case ConversionUnits.MASS:
					this.siMass.SIValue = origNumber;
					return this.siMass.ToString();

				case ConversionUnits.PRESSURE:
					this.siPressure.SIValue = origNumber;
					return this.siPressure.ToString();

				default:
					return origNumber.ToString(CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// This method will convert a double number to the current site's
		/// engineering unit. The original number and unit must be specified.
		/// It returns a formatted string representation.
		/// </summary>
		/// <param name="origNumber"></param>
		/// <param name="whichUnit"></param>
		/// <returns></returns>
		public string ConvertFromSiFormatted(double origNumber, ConversionUnits whichUnit)
		{
			switch (whichUnit)
			{
				case ConversionUnits.VOLUME:
					this.siVolume.SIValue = origNumber;
					return this.siVolume.ToString();

				case ConversionUnits.ADDITIVE_VOLUME:
					this.siAdditiveVolume.SIValue = origNumber;
					return this.siAdditiveVolume.ToString();

				case ConversionUnits.FLOW:
					this.siFlow.SIValue = origNumber;
					return this.siFlow.ToString();

				case ConversionUnits.LEVEL:
					this.siLevel.SIValue = origNumber;
					return this.siLevel.ToString();

				case ConversionUnits.TEMPERATURE:
					this.siTemperature.SIValue = origNumber;
					return this.siTemperature.ToString();

				case ConversionUnits.DENSITY:
					this.siDensity.SIValue = origNumber;
					return this.siDensity.ToString();

				case ConversionUnits.MASS:
					this.siMass.SIValue = origNumber;
					return this.siMass.ToString();

				case ConversionUnits.PRESSURE:
					this.siPressure.SIValue = origNumber;
					return this.siPressure.ToString();

				default:
					return origNumber.ToString(CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// This method will return the volume factor. Just use the factor to 
		/// multiple against your value. It will use the current sites volume units
		/// to derive the conversion factor.
		/// </summary>
		/// <returns></returns>
		public double GetVolumeConversionFactor()
		{
			double conversionFactor;

			switch (this.currentSite.VolumeUnits)
			{
				case EngineeringUnit.FmvCm3:
					conversionFactor = 1 / .000001;
					break;
				case EngineeringUnit.FmvFeet3:
					conversionFactor = 1 / .02831685;
					break;
				case EngineeringUnit.FmvImpGal:
					conversionFactor = 1 / .004546092;
					break;
				case EngineeringUnit.FmvInch3:
					conversionFactor = 1 / .00001638706;
					break;
				case EngineeringUnit.FmvKl:
					conversionFactor = 1 / 1.0;
					break;
				case EngineeringUnit.FmvLitre:
					conversionFactor = 1 / .001;
					break;
				case EngineeringUnit.FmvMeter3:
					conversionFactor = 1 / 1.0;
					break;
				case EngineeringUnit.FmvUsGal:
					conversionFactor = 1 / .003785412;
					break;
				case EngineeringUnit.FmvYard3:
					conversionFactor = 1 / .7645549;
					break;
				case EngineeringUnit.FmvMsFt3:
					conversionFactor = 1 / 26.853;
					break;
				default:
					conversionFactor = 1 / 1.0;
					break;
			}

			return conversionFactor;
		}

		/// <summary>
		/// This method will return a number formatted with the correct separator and precision.
		/// There are two parameters: the number object and the unit types (volume, degrees, ...).
		/// </summary>
		/// <param name="dataValue"></param>
		/// <param name="unitType"></param>
		/// <returns></returns>
		public string GetFormattedValue(object dataValue, SITE_VARIABLE_TYPE unitType)
		{
			if ((dataValue.Equals(null)) || dataValue.Equals(string.Empty))
				return string.Empty;

			if (dataValue is double)
				return ((double)dataValue).ToString("N", this.currentSite.GetNumberFormatInfo(unitType));

			if (dataValue is int)
				return ((int)dataValue).ToString("G", this.currentSite.GetNumberFormatInfo(unitType));

			return string.Empty;
		}

		/// <summary>
		/// This method initialize the accounting site object to 
		/// its initial state.
		/// </summary>
		private void Init()
		{
			this.siteList = new ArrayList();
			this.currentSite = null;
			this.loginSite = null;
			this.currentSiteName = string.Empty;
			this.hasViewPermissionForAllCompanies = false;
			this.getUserCompanies = true;
			this.userCompanyList = new List<Guid>();
		}

		/// <summary>
		/// This method will create all the conversion objects that will convert from SI to the
		/// currently set units.
		/// </summary>
		public void CreateFromSiObjects()
		{
			this.siVolume = new SIDouble(this.currentSite.VolumeUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0);
			this.siAdditiveVolume = new SIDouble(this.currentSite.AdditiveVolumeUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME), 0);
			this.siFlow = new SIDouble(this.currentSite.FlowUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this.siLevel = new SIDouble(this.currentSite.LevelUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.LENGTH), 0);
			this.siTemperature = new SIDouble(this.currentSite.TemperatureUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE), 0);
			this.siDensity = new SIDouble(this.currentSite.DensityUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY), 0);
			this.siMass = new SIDouble(this.currentSite.MassUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS), 0);
			this.siPressure = new SIDouble(this.currentSite.PressureUnits, this.currentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.PRESSURE), 0);
		}
	}
}
