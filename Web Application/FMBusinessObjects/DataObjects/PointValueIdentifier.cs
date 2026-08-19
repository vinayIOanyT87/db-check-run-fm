namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class PointValueIdentifier : IEquatable<PointValueIdentifier>
	{
		[DataMember]
		public Guid IdentityGuid { get; set; }

		[DataMember]
		public PointValueType PointValueType { get; set; }


		[DataMember]
		// propertyid is null because json marshals string.empty as null and the PointValueIdentifier is marshalled as json
		private string propertyid;

		public string PropertyID
		{
			get { return propertyid; }
			set { propertyid = (string.IsNullOrEmpty(value)) ? null : value; }
		}

		[DataMember]
		public bool IncludeAlarmLimits { get; set; }

		[DataMember]
		public Guid WellKnownIdentityGuid { get; set; }

		[DataMember]
		public Guid SiteGuid { get; set; }

		[DataMember]
		public Guid SubIdentifierGuid { get; set; }

		[DataMember]
		public Int64 UtcTicks { get; set; }


		public PointValueIdentifier()
		{
			this.IdentityGuid = Guid.Empty;
			this.PointValueType = PointValueType.Tag;
			this.PropertyID = null;
			this.IncludeAlarmLimits = false;
			this.WellKnownIdentityGuid = Guid.Empty;
			this.SiteGuid = Guid.Empty;
			this.SubIdentifierGuid = Guid.Empty;
		}

		public PointValueIdentifier(Guid identityGuid, PointValueType pointValueType, string propertyID)
		{
			this.IdentityGuid = identityGuid;
			this.PointValueType = pointValueType;
			this.PropertyID = propertyID;
			this.IncludeAlarmLimits = false;
			this.WellKnownIdentityGuid = Guid.Empty;
			this.SiteGuid = Guid.Empty;
			this.SubIdentifierGuid= Guid.Empty;
		}

		public PointValueIdentifier(Guid identityGuid, PointValueType pointValueType, string propertyID, DateTimeOffset serverTimeStamp)
		{
			this.IdentityGuid = identityGuid;
			this.PointValueType = pointValueType;
			this.PropertyID = propertyID;
			this.IncludeAlarmLimits = false;
			this.WellKnownIdentityGuid = Guid.Empty;
			this.SiteGuid = Guid.Empty;
			this.SubIdentifierGuid = Guid.Empty;
			this.UtcTicks = serverTimeStamp.UtcTicks;
		}


		public PointValueIdentifier(Guid identityGuid, PointValueType pointValueType, string propertyID, Guid wellKnownIdentityGuid)
		{
			this.IdentityGuid = identityGuid;
			this.PointValueType = pointValueType;
			this.PropertyID = propertyID;
			this.IncludeAlarmLimits = false;
			this.WellKnownIdentityGuid = wellKnownIdentityGuid;
			this.SiteGuid = Guid.Empty;
			this.SubIdentifierGuid = Guid.Empty;
		}

		public PointValueIdentifier(Guid identityGuid, PointValueType pointValueType, string propertyID, Guid wellKnownIdentityGuid, Guid subIdentiferGuid)
		{
			this.IdentityGuid = identityGuid;
			this.PointValueType = pointValueType;
			this.PropertyID = propertyID;
			this.IncludeAlarmLimits = false;
			this.WellKnownIdentityGuid = wellKnownIdentityGuid;
			this.SiteGuid = Guid.Empty;
			this.SubIdentifierGuid = subIdentiferGuid;
		}

		public PointValueIdentifier(PointValue pointValue)
		{
			if (pointValue == null)
			{
				return;
			}

			this.IdentityGuid = pointValue.PointValueIdentifier.IdentityGuid;
			this.PointValueType = pointValue.PointValueIdentifier.PointValueType;
			this.PropertyID = pointValue.PointValueIdentifier.PropertyID;
			this.IncludeAlarmLimits = pointValue.AlarmLimitList.Count > 0;
			this.WellKnownIdentityGuid = pointValue.WellKnownIdentityGuid;
			this.SiteGuid = pointValue.PointValueIdentifier.SiteGuid;
			this.SubIdentifierGuid = pointValue.PointValueIdentifier.SubIdentifierGuid;
			this.UtcTicks = pointValue.ServerTimeStamp.UtcTicks;
		}

		public PointValueIdentifier(PointTag pointTag)
		{
			if (pointTag == null)
			{
				return;
			}

			this.IdentityGuid = pointTag.IdentityGuid;
			this.PointValueType = PointValueType.Tag;
			this.PropertyID = null;
			this.IncludeAlarmLimits = false;
			this.WellKnownIdentityGuid = pointTag.WellKnownIdentityGuid;
			this.SiteGuid = pointTag.SiteGuid;
			this.SubIdentifierGuid = Guid.Empty;
			this.UtcTicks = pointTag.ServerTimeStamp.UtcTicks;
		}

		public PointValueIdentifier(PointTemplateTag pointTemplateTag)
		{
			if (pointTemplateTag == null)
			{
				return;
			}

			this.IdentityGuid = pointTemplateTag.IdentityGuid;
			this.PointValueType = PointValueType.Tag;
			this.PropertyID = null;
			this.IncludeAlarmLimits = false;
			this.WellKnownIdentityGuid =pointTemplateTag.WellKnownIdentityGuid;
			this.SiteGuid = pointTemplateTag.SiteGuid;
			this.SubIdentifierGuid = Guid.Empty;
		}

		public PointValueIdentifier(string identifierString)
		{
			var parsedString = identifierString.Split(' ');
			this.IdentityGuid = Guid.Parse(parsedString[0]);
			this.PointValueType = (PointValueType) Enum.Parse(PointValueType.GetType(), parsedString[1]);
			this.PropertyID = parsedString.Length > 2 ? parsedString[2] : null;
			this.IncludeAlarmLimits = false;
			this.SiteGuid = Guid.Empty;
			this.SubIdentifierGuid = Guid.Empty;
		}

		public bool Equals(PointValueIdentifier pointValueIdentifier)
		{
			if (pointValueIdentifier == null)
			{
				return false;
			}

			return pointValueIdentifier.IdentityGuid == this.IdentityGuid
						&& pointValueIdentifier.PointValueType == this.PointValueType
						&& pointValueIdentifier.PropertyID == this.PropertyID;
					
		}

		public override string ToString()
		{
			return this.IdentityGuid.ToString() + " " + this.PointValueType + (string.IsNullOrEmpty(PropertyID) ? "" : " " + PropertyID);
		}

		public PointValueIdentifier(PointValueType pointValueType)
		{
			this.PointValueType = pointValueType;
			this.PropertyID = null;
			this.IncludeAlarmLimits = false;
		}


		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var pointValueIdentifier = obj as PointValueIdentifier;
			return pointValueIdentifier != null && Equals(pointValueIdentifier);
		}

		public override int GetHashCode()
		{
			return new { this.IdentityGuid, this.PointValueType, this.PropertyID }.GetHashCode();
		}

		public static bool operator == (PointValueIdentifier pointValueIdentifier1, PointValueIdentifier pointValueIdentifier2)
		{
			if (((object) pointValueIdentifier1) == null || ((object)pointValueIdentifier2) == null)
			{
				return object.Equals(pointValueIdentifier1, pointValueIdentifier2);
			}

			return pointValueIdentifier1.Equals(pointValueIdentifier2);
		}

		public static bool operator != (PointValueIdentifier pointValueIdentifier1, PointValueIdentifier pointValueIdentifier2)
		{
			if (((object)pointValueIdentifier1) == null || ((object)pointValueIdentifier2) == null)
			{
				return !object.Equals(pointValueIdentifier1, pointValueIdentifier2);
			}

			return !pointValueIdentifier1.Equals(pointValueIdentifier2);
		}
	}
}
