namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Data;
	using System.Data.SqlClient;

	using Attributes;



	public class MovementSummaryColumnDefinition
	{
		public string name { get; set; }
		public string field { get; set; }
		public bool resizable { get; set; }
		public bool sortable { get; set; }
		public int minWidth { get; set; }
		public bool rerenderOnResize { get; set; }
		public string headerCssClass { get; set; }
		public bool defaultSortAsc { get; set; }
		public bool focusable { get; set; }
		public bool selectable { get; set; }
		public int width { get; set; }
		public string id { get; set; }
		public string cssClass { get; set; }
		public string header { get; set; }
		public int previousWidth { get; set; }

		public MovementSummaryColumnDefinition(string name, string field, Boolean resizable, bool sortable, int minWidth, bool rerenderOnResize, string headerCssClass, bool defaultSortAsc, bool selectable, int width, string id, string cssClass, string header, int previousWidth)
		{
			this.name = name;
			this.field = field;
			this.resizable = resizable;
			this.sortable = sortable;
			this.minWidth = minWidth;
			this.rerenderOnResize = rerenderOnResize;
			this.headerCssClass = headerCssClass;
			this.defaultSortAsc = defaultSortAsc;
			this.focusable = focusable;
			this.selectable = selectable;
			this.width = width;
			this.id = id;
			this.cssClass = cssClass;
			this.header = header;
			this.previousWidth = previousWidth;
		}
	}



	#region Movement Summary Collection Class
	[Serializable]
	[CollectionDataContract]
	public class MovementSummaryCollection : List<MovementSummary>
	{
		public MovementSummaryCollection Clone()
		{
			var movementSummaryCollection = new MovementSummaryCollection();
			foreach (var m in this)
			{
				movementSummaryCollection.Add(m.Clone());
			}
			return movementSummaryCollection;
		}
	}
	#endregion


	[DataContract]
	[Serializable]
	public class MovementSummary : BaseDataObject
	{
		public enum MovementSummaryVisibilityType
		{
			Public = 0,
			Private = 1,
			Shared = 2
		}

		public const string DefaultColumns = "[{\"name\":\"Name\",\"field\":\"PointId\",\"resizable\":true,\"sortable\":true,\"minWidth\":200,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":200,\"id\":\"PointId\",\"cssClass\":\"ui-state-default text-center grid-font-14\",\"header\":null,\"previousWidth\":80},"
													+ "{\"name\":\"Type\",\"field\":\"Type\",\"resizable\":true,\"sortable\":true,\"minWidth\":80,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"Type\",\"header\":null,\"previousWidth\":80},"
													+ "{\"name\":\"Direction\",\"field\":\"TransferDirection\",\"resizable\":true,\"sortable\":false,\"minWidth\":80,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"TransferDirection\",\"header\":null,\"previousWidth\":80},"
													+ "{\"name\":\"Status\",\"field\":\"Status\",\"resizable\":true,\"sortable\":true,\"minWidth\":80,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"Status\",\"header\":null,\"previousWidth\":80},"
													+ "{\"name\":\"Transferred GOV\",\"field\":\"TransferredGOV\",\"resizable\":true,\"sortable\":true,\"minWidth\":140,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":140,\"id\":\"TransferredGOV\",\"header\":null,\"previousWidth\":140},"
													+ "{\"name\":\"Transferred NSV\",\"field\":\"TransferredNSV\",\"resizable\":true,\"sortable\":true,\"minWidth\":140,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":140,\"id\":\"TransferredNSV\",\"header\":null,\"previousWidth\":140},"
													+ "{\"name\":\"Transfer Mode\",\"field\":\"TransferMode\",\"resizable\":true,\"sortable\":false,\"minWidth\":130,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":130,\"id\":\"TransferMode\",\"header\":null,\"previousWidth\":130},"
													+ "{\"name\":\"Transfer Status\",\"field\":\"TransferStatus\",\"resizable\":true,\"sortable\":false,\"minWidth\":130,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":130,\"id\":\"TransferStatus\",\"header\":null,\"previousWidth\":130},"
													+ "{\"name\":\"Transfer Target\",\"field\":\"TransferTarget\",\"resizable\":true,\"sortable\":false,\"minWidth\":130,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":130,\"id\":\"TransferTarget\",\"header\":null,\"previousWidth\":130},"
													+ "{\"name\":\"Transfer Start Time\",\"field\":\"TransferStartTime\",\"resizable\":true,\"sortable\":true,\"minWidth\":200,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":200,\"id\":\"TransferStartTime\",\"header\":null,\"previousWidth\":200},"
													+ "{\"name\":\"Transfer Time Remaining\",\"field\":\"TransferTimeRemaining\",\"resizable\":true,\"sortable\":true,\"minWidth\":200,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":200,\"id\":\"TransferTimeRemaining\",\"header\":null,\"previousWidth\":200},"
													+ "{\"name\":\"Product\",\"field\":\"Product\",\"resizable\":true,\"sortable\":false,\"minWidth\":80,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"Product\",\"header\":null,\"previousWidth\":80},"
													+ "{\"name\":\"Created By\",\"field\":\"CreatedBy\",\"resizable\":true,\"sortable\":true,\"minWidth\":100,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":100,\"id\":\"CreatedBy\",\"header\":null,\"previousWidth\":100},"
													+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\"	,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty9\",\"header\":null,\"previousWidth\":20}"
													+ "]";


		#region Constructors and Destructors

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementSummary()
		{
			this.Init();
		}

		public MovementSummary Clone()
		{
			MovementSummary m = (MovementSummary)this.MemberwiseClone();

			m.RowVersion = new byte[this.RowVersion.Length];
			for (int i = 0; i < this.RowVersion.Length; i++)
			{
				m.RowVersion[i] = this.RowVersion[i];
			}
			return m;
		}
		#endregion

		#region Properties
		[FMPersistedField]
		public Guid MovementSummaryGuid
		{
			get { return this.IdentityGuid; }
			set { this.IdentityGuid = value; }
		}

		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }

		[DataMember]
		[FMPersistedField(DefaultValue = MovementSummaryVisibilityType.Public)]
		public MovementSummaryVisibilityType MovementSummaryType { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid OwnerUserGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public string ColumnsDefinition { get; set; }

		[DataMember]
		[FMPersistedField(DefaultValue = 14)]
		public int FontSize { get; set; }

		[DataMember]
		[FMPersistedField]
		public string RowsDefinition { get; set; }

		#endregion


		public SqlCommand AddMovementSummary(SecurityClass security, MovementSummary movementSummary, SqlCommand cmd)
		{
			movementSummary.SetCreationStamp(security);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementSummaryInsert";
			cmd.Parameters.Clear();
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
			this.MovementSummaryType = MovementSummaryVisibilityType.Public;
			this.MovementSummaryGuid = Guid.Empty;
			this.OwnerUserGuid = Guid.Empty;
			this.ColumnsDefinition = string.Empty;
			this.FontSize = 14;
			this.RowsDefinition = string.Empty;
		}
		#endregion

	}
}
