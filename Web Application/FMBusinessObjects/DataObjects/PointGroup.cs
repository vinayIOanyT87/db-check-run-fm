namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	using Attributes;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System.Reflection;

	#region Point Group Collection Class
	[Serializable]
	[CollectionDataContract]
	public class PointGroupCollection : List<PointGroup>
	{
		public PointGroupCollection Clone()
		{
			var pointGroupCollection = new PointGroupCollection();
			foreach (var p in this)
			{
				pointGroupCollection.Add(p.Clone());
			}
			return pointGroupCollection;
		}
	}
	#endregion

	[DataContract]
	[Serializable]
	public class PointGroup : BaseDataObject
	{
		public enum PointGroupVisibilityType
		{			
			Public = 0,
			Private = 1,
			Shared = 2
		}

		#region Constructors and Destructors

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroup()
		{
			this.Init();
		}

		public PointGroup Clone()
		{
			PointGroup p = (PointGroup)this.MemberwiseClone();

			p.RowVersion = new byte[this.RowVersion.Length];
			for (int i = 0; i < this.RowVersion.Length; i++)
			{
				p.RowVersion[i] = this.RowVersion[i];
			}
			return p;
		}
		#endregion

		#region Properties
		[FMPersistedField]
		public Guid PointGroupGuid
		{
			get { return this.IdentityGuid; }
			set { this.IdentityGuid = value; }
		}

		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }

		[DataMember]
		[FMPersistedField(DefaultValue = PointGroupVisibilityType.Public)]
		public PointGroupVisibilityType PointGroupType { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid OwnerUserGuid { get; set; }

		[DataMember]
		public PointGroupColumn PointGroupColumn { get; set; }

		[DataMember]
		public PointGroupRow PointGroupRow { get; set; }

		#endregion


		public SqlCommand AddPointGroup(SecurityClass security, PointGroup pointGroup, SqlCommand cmd)
		{
			pointGroup.SetCreationStamp(security);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointGroupInsert";
			cmd.Parameters.Clear();
//			cmd.Parameters.AddWithValue("@" + column, property.GetValue(this).ToString());
			return cmd;
		}

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			base.Reset();
			this.Description = string.Empty;
			this.PointGroupType = PointGroupVisibilityType.Public;
			this.PointGroupGuid = Guid.Empty;
			this.OwnerUserGuid = Guid.Empty;
			this.PointGroupColumn = new PointGroupColumn();
			this.PointGroupRow = new PointGroupRow();
		} 
		#endregion

	}
}
