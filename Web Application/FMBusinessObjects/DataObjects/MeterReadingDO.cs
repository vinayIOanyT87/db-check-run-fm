// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeterReadingDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MeterReadingDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	/// <summary>
	/// The meter reading data object.
	/// </summary>
	[XmlType("MeterReading")]
	[Serializable]
	[DataContract]
	public class MeterReadingDO : DataObject
	{
		#region Constants
		/// <summary>
		/// The time format.
		/// </summary>
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		#region Attributes
		/// <summary>
		/// The meter factor.
		/// </summary>
		[DataMember]
		private double? meterFactor;

		/// <summary>
		/// The meter start.
		/// </summary>
		[DataMember]
		private double? meterStart;

		/// <summary>
		/// The meter stop.
		/// </summary>
		[DataMember]
		private double? meterStop;

		/// <summary>
		/// The start date time.
		/// </summary>
		[DataMember]
		private DateTimeOffset? startDateTime;

		/// <summary>
		/// The stop date time.
		/// </summary>
		[DataMember]
		private DateTimeOffset? stopDateTime;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MeterReadingDO"/> class. 
		/// This is the default constructor for the meter reading data
		/// object class.
		/// </summary>
		public MeterReadingDO( )
		{
		}

        public MeterReadingDO(MeterReadingDO meterReadingDO)
        {
            if (meterReadingDO == null)
            {
                throw new ArgumentNullException(nameof(meterReadingDO));
            }

            this.meterFactor = meterReadingDO.meterFactor;
            this.meterStart = meterReadingDO.meterStart;
            this.meterStop = meterReadingDO.meterStop;
            this.startDateTime = meterReadingDO.startDateTime;
            this.stopDateTime = meterReadingDO.stopDateTime;
        }
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether meter start bad quality logged.
		/// </summary>
		[DataMember]
		public bool MeterStart_BadQualityLogged { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether meter stop bad quality logged.
		/// </summary>
		[DataMember]
		public bool MeterStop_BadQualityLogged { get; set; }

		/// <summary>
		/// Gets or sets the meter factor.
		/// </summary>
		public double? MeterFactor
		{
			get { return this.meterFactor; }
			set { this.meterFactor = value; }
		}

		/// <summary>
		/// Gets or sets the meter start.
		/// </summary>
		public double? MeterStart
		{
			get { return this.meterStart; }
			set { this.meterStart = value; }
		}

		/// <summary>
		/// Gets or sets the meter stop.
		/// </summary>
		public double? MeterStop
		{
			get { return this.meterStop; }
			set { this.meterStop = value; }
		}

		/// <summary>
		/// Gets or sets the start date time string.
		/// </summary>
		[XmlElement("StartDateTimeString")]
		public string StartDateTimeString
		{
			get
			{
				return this.startDateTime == null ? string.Empty : ((DateTimeOffset)this.startDateTime).ToString(TimeFormat);
			}

			set
			{
				this.startDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the start date time.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? StartDateTime
		{
			get { return this.startDateTime; }
			set { this.startDateTime = value; }
		}

		/// <summary>
		/// Gets or sets the stop date time string.
		/// </summary>
		[XmlElement("StopDateTimeString")]
		public string StopDateTimeString
		{
			get
			{
				return this.stopDateTime == null ? string.Empty : ((DateTimeOffset)this.stopDateTime).ToString(TimeFormat);
			}

			set
			{
				this.stopDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the stop date time.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? StopDateTime
		{
			get { return this.stopDateTime; }
			set { this.stopDateTime = value; }
		}
		#endregion Properties

		#region Methods to handle whether a property should be serialized.
		/// <summary>
		/// This method causes the Meter Factor property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeMeterFactor( )
		{
			return this.meterFactor.HasValue;
		}

		/// <summary>
		/// This method causes the Meter Start property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeMeterStart( )
		{
			return this.meterStart.HasValue;
		}

		/// <summary>
		/// This method causes the Meter Stop property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeMeterStop( )
		{
			return this.meterStop.HasValue;
		}
		#endregion

		#region Abstract methods
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion Abstract methods
	}
}
