
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
	public class PointGroupColumn : BaseDataObject
	{
		#region Constructors and Destructors

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroupColumn()
		{
			this.Init();
		}
		#endregion


		#region Properties
		[FMPersistedField]
		public Guid PointGroupColumnsGuid
		{
			get { return this.IdentityGuid; }
			set { this.IdentityGuid = value; }
		}

		[DataMember]
		[FMPersistedField]
		public Guid PointGroupGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public string ColumnsDefinition { get; set; }

		[DataMember]
		[FMPersistedField(DefaultValue = 14)]
		public int FontSize { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid OwnerUserGuid { get; set; }

		#endregion


		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			base.Reset();
			this.ColumnsDefinition = string.Empty;
			this.OwnerUserGuid = Guid.Empty;
		} 

		#endregion
	}
}
