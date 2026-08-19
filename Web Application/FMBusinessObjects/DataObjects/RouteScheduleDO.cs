// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteScheduleDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RouteScheduleDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	/// <summary>
	/// The route schedule data object.
	/// </summary>
	[XmlType("RouteSchedule")]
	[Serializable]
	[DataContract]
	public class RouteScheduleDO : DataObject
	{
		#region Constants
		/// <summary>
		/// The time format.
		/// </summary>
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		#region Attributes
		/// <summary>
		/// The Schedule Time of Destination.
		/// </summary>
		[DataMember]
		private DateTimeOffset? std;

		/// <summary>
		/// The Estimated Time of Destination.
		/// </summary>
		[DataMember]
		private DateTimeOffset? etd;

		/// <summary>
		/// The Schedule Time of Arrival.
		/// </summary>
		[DataMember]
		private DateTimeOffset? sta;

		/// <summary>
		/// The Estimated Time of Arrival.
		/// </summary>
		[DataMember]
		private DateTimeOffset? eta;

		/// <summary>
		/// The SFT.
		/// </summary>
		[DataMember]
		private DateTimeOffset? sft;

		/// <summary>
		/// The FST.
		/// </summary>
		[DataMember]
		private DateTimeOffset? fst;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="RouteScheduleDO"/> class.
		/// </summary>
		public RouteScheduleDO( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the Schedule Time of Destination string.
		/// </summary>
		[XmlElement("STDString")]
		public string STDString
		{
			get
			{
				return this.std == null ? string.Empty : ((DateTimeOffset) this.std).ToString(TimeFormat);
			}

			set
			{
				this.std = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the Schedule Time of Destination.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? STD
		{
			get { return this.std; }
			set { this.std = value; }
		}

		/// <summary>
		/// Gets or sets the Estimated Time of Destination string.
		/// </summary>
		[XmlElement("ETDString")]
		public string ETDString
		{
			get
			{
				return this.etd == null ? string.Empty : ((DateTimeOffset) this.etd).ToString(TimeFormat);
			}

			set
			{
				this.etd = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the Estimated Time of Destination.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? ETD
		{
			get { return this.etd; }
			set { this.etd = value; }
		}

		/// <summary>
		/// Gets or sets the Schedule Time of Arrival string.
		/// </summary>
		[XmlElement("STAString")]
		public string STAString
		{
			get
			{
				return this.sta == null ? string.Empty : ((DateTimeOffset) this.sta).ToString(TimeFormat);
			}

			set
			{
				this.sta = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the Schedule Time of Arrival.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? STA
		{
			get { return this.sta; }
			set { this.sta = value; }
		}

		/// <summary>
		/// Gets or sets the Estimated Time of Arrival string.
		/// </summary>
		[XmlElement("ETAString")]
		public string ETAString
		{
			get
			{
				return this.eta == null ? string.Empty : ((DateTimeOffset) this.eta).ToString(TimeFormat);
			}

			set
			{
				this.eta = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the Estimated Time of Arrival.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? ETA
		{
			get { return this.eta; }
			set { this.eta = value; }
		}

		/// <summary>
		/// Gets or sets the SFT string.
		/// </summary>
		[XmlElement("SFTString")]
		public string SFTString
		{
			get
			{
				return this.sft == null ? string.Empty : ((DateTimeOffset) this.sft).ToString(TimeFormat);
			}

			set
			{
				this.sft = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the SFT.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? SFT
		{
			get { return this.sft; }
			set { this.sft = value; }
		}

		/// <summary>
		/// Gets or sets the FST string.
		/// </summary>
		[XmlElement("FSTString")]
		public string FSTString
		{
			get
			{
				return this.fst == null ? string.Empty : ((DateTimeOffset) this.fst).ToString(TimeFormat);
			}

			set
			{
				this.fst = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets the FST.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? FST
		{
			get { return this.fst; }
			set { this.fst = value; }
		}
		#endregion Properties

		#region Overrides
		public override string getDeleteCommand( )
		{
			return null;
		}
		public override string getInsertCommand( )
		{
			return null;
		}
		public override string getSelectCommand( )
		{
			return null;
		}
		public override string getUpdateCommand( )
		{
			return null;
		}
		#endregion Overrides
	}
}
