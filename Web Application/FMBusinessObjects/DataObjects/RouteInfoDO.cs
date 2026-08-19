// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteInfoDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RouteInfoDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	/// <summary>
	/// The route info do.
	/// </summary>
	[XmlType("RouteInfo")]
	[Serializable]
	[DataContract]
	public class RouteInfoDO : DataObject
	{
		#region Attributes
		/// <summary>
		/// The time format.
		/// </summary>
		private const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

		/// <summary>
		/// The routing ID.
		/// </summary>
		[DataMember]
		private string routingId;

		/// <summary>
		/// The route origination date.
		/// </summary>
		[DataMember]
		private DateTimeOffset? routeOriginationDate;

		/// <summary>
		/// The international route indicator.
		/// </summary>
		[DataMember]
		private bool internationalRouteIndicator;

		/// <summary>
		/// The previous routing ID.
		/// </summary>
		[DataMember]
		private string previousRoutingId;

		/// <summary>
		/// The final station GUID.
		/// </summary>
		[DataMember]
		private Guid finalStationGuid;

		/// <summary>
		/// The final station IATA ID.
		/// </summary>
		[DataMember]
		private string finalStationIataid;

		/// <summary>
		/// The previous station IATA GUID.
		/// </summary>
		[DataMember]
		private Guid previousStationIataGuid;

		/// <summary>
		/// The previous station IATA ID.
		/// </summary>
		[DataMember]
		private string previousStationIataid;

		/// <summary>
		/// The next station IATA GUID.
		/// </summary>
		[DataMember]
		private Guid nextStationIataGuid;

		/// <summary>
		/// The next station IATA ID.
		/// </summary>
		[DataMember]
		private string nextStationIataid;

		/// <summary>
		/// The origin station IATA GUID.
		/// </summary>
		[DataMember]
		private Guid originStationIataGuid;

		/// <summary>
		/// The origin station IATA ID.
		/// </summary>
		[DataMember]
		private string originStationIataid;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="RouteInfoDO"/> class.
		/// </summary>
		public RouteInfoDO( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the routing ID.
		/// </summary>
		public string RoutingID
		{
			get { return this.routingId; }
			set { this.routingId = value; }
		}

		/// <summary>
		/// Gets or sets the route origination date.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? RouteOriginationDate
		{
			get { return this.routeOriginationDate; }
			set { this.routeOriginationDate = value; }
		}

		/// <summary>
		/// Gets or sets the route origination date string.
		/// </summary>
		[XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
		public string RouteOriginationDateString
		{
			get
			{
				return this.routeOriginationDate == null ? string.Empty : ((DateTimeOffset) this.routeOriginationDate).ToString(TimeFormat);
			}

			set
			{
				this.routeOriginationDate = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether international route indicator.
		/// </summary>
		public bool InternationalRouteIndicator
		{
			get { return this.internationalRouteIndicator; }
			set { this.internationalRouteIndicator = value; }
		}

		/// <summary>
		/// Gets or sets the previous routing ID.
		/// </summary>
		public string PreviousRoutingID
		{
			get { return this.previousRoutingId; }
			set { this.previousRoutingId = value; }
		}

		/// <summary>
		/// Gets or sets the final station IATA GUID.
		/// </summary>
		[XmlIgnore]
		public Guid FinalStationIATAGuid
		{
			get { return this.finalStationGuid; }
			set { this.finalStationGuid = value; }
		}

		/// <summary>
		/// Gets or sets the final station IATA ID.
		/// </summary>
		public string FinalStationIATAID
		{
			get { return this.finalStationIataid; }
			set { this.finalStationIataid = value; }
		}

		/// <summary>
		/// Gets or sets the previous station IATA GUID.
		/// </summary>
		[XmlIgnore]
		public Guid PreviousStationIATAGuid
		{
			get { return this.previousStationIataGuid; }
			set { this.previousStationIataGuid = value; }
		}

		/// <summary>
		/// Gets or sets the previous station IATA ID.
		/// </summary>
		public string PreviousStationIATAID
		{
			get { return this.previousStationIataid; }
			set { this.previousStationIataid = value; }
		}

		/// <summary>
		/// Gets or sets the next station IATA GUID.
		/// </summary>
		[XmlIgnore]
		public Guid NextStationIATAGuid
		{
			get { return this.nextStationIataGuid; }
			set { this.nextStationIataGuid = value; }
		}

		/// <summary>
		/// Gets or sets the next station IATA ID.
		/// </summary>
		public string NextStationIATAID
		{
			get { return this.nextStationIataid; }
			set { this.nextStationIataid = value; }
		}

		/// <summary>
		/// Gets or sets the origin station IATA GUID.
		/// </summary>
		[XmlIgnore]
		public Guid OriginStationIATAGuid
		{
			get { return this.originStationIataGuid; }
			set { this.originStationIataGuid = value; }
		}

		/// <summary>
		/// Gets or sets the origin station IATA ID.
		/// </summary>
		public string OriginStationIATAID
		{
			get { return this.originStationIataid; }
			set { this.originStationIataid = value; }
		}
		#endregion Properties

		#region Inherited abstract methods
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
		#endregion Inherited abstract methods
	}
}
