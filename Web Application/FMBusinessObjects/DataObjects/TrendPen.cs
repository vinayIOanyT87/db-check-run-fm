using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using Varec.CommonComponents.EngineeringUnitsLibrary;


	[DataContract]
	[Serializable]
	public class TrendPen : BaseDataObject
	{
		public new Guid SiteGuid
		{
			get
			{
				return base.SiteGuid;
			}
			set
			{
				base.SiteGuid = value;
			}
		}

		public new string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
			}
		}



		[DataMember]
		[FMPersistedField]
		public Guid TrendPenToTrendGuid
		{
			get
			{
				return this.IdentityGuid;
			}
			set
			{
				this.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public Guid PointTagGuid { get; set;}

		[DataMember]
		[FMPersistedField]
		public Guid PointTemplateTagGuid { get; set; }


		[DataMember]
		[FMPersistedField]
		public Guid TrendGuid { get; set; }


		[DataMember]
		[FMPersistedField]
		public string PenColor { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly =true)] 
		public EngineeringUnitType EngineeringUnitsType { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public EngineeringUnit Units { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public byte DecimalPlaces { get; set; }

		[DataMember]
		public string UnitString { get; set; }


		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string PointID { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string TagID { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public Guid PointGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public double Maximum { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public double Minimum { get; set; }


		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string ValueType { get; set; }


	}
}
