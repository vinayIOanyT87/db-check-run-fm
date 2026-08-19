using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class ListViewColumnDO : DataObject
	{
		#region Attributes
		[DataMember] private string columnName;
		[DataMember] private string dataPath;
		[DataMember] private bool isLink;
		[DataMember] private bool rowParameter;
		[DataMember] private bool dataDictionaryType;
		#endregion

		#region Constructor
		public ListViewColumnDO ( )
		{
			this.init ( );
		}

		public ListViewColumnDO(ListViewFieldClass srcField)
			: this()
		{
			this.init();
			ColumnName = srcField.ID;
			DataPath = srcField.DataPath;
			IsLink = srcField.IsLink;
			RowParameter = srcField.RowParameter;
			DataDictionaryType = srcField.DataDictionaryType;
			IsAggregateField = (srcField.Type == LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD);
			AggregateType = srcField.AggregateType;
			DataTypeName = srcField.DataType.FullName;
			IsColumnWrapped = srcField.IsColumnWrapped;
		}
		#endregion

		#region Properties
		public string ColumnName
		{
			get { return this.columnName; }
			set { this.columnName = value; }
		}

		public string DataPath
		{
			get { return this.dataPath; }
			set { this.dataPath = value; }
		}

		public bool IsLink
		{
			get { return this.isLink; }
			set { this.isLink = value; }
		}

		public bool RowParameter
		{
			get { return this.rowParameter; }
			set { this.rowParameter = value; }
		}

		public bool DataDictionaryType
		{
			get { return this.dataDictionaryType; }
			set { this.dataDictionaryType = value; }
		}


		[DataMember]
		public bool IsAggregateField 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public LedgerAggregateColumnClass.AggregateType AggregateType 
		{ 
			get; 
			set; 
		}

		/// <summary>
		/// Indicates whether the column should be wrapped or not
		/// </summary>
		[DataMember]
		public bool IsColumnWrapped
		{
			get;
			set;
		}

		/// <summary>
		/// DataTypeName of the column.  
		/// Using the name instead of the type, easier to marshall than the actual type.
		/// </summary>
		[DataMember]
		public string DataTypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Get the data type from the DataTypeName
		/// </summary>
		public Type DataType
		{
			get
			{
				return Type.GetType(DataTypeName, false, true);
			}
		}
		#endregion

		#region Methods
		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		private void init ( )
		{
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getSelectCommand ( )
		{
			return null;
		}
		#endregion
	}
}
