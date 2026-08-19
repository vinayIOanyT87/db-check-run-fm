

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading.Tasks;

	using FMBusinessObjects.Attributes;

	[DataContract]
	[Serializable]
	public class PointGroupRow : BaseDataObject
	{
		#region Constructors and Destructors

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroupRow()
		{
			this.Init();
		}
		#endregion


		#region Properties
		[FMPersistedField]
		public Guid PointGroupRowsGuid
		{
			get { return this.IdentityGuid; }
			set { this.IdentityGuid = value; }
		}

		[DataMember]
		[FMPersistedField]
		public Guid PointGroupGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public string RowsDefinition { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid OwnerUserGuid { get; set; }

		#endregion

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			base.Reset();
			this.RowsDefinition = string.Empty;
			this.OwnerUserGuid = Guid.Empty;
		}
	}
}
