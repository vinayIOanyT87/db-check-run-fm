using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	[KnownType ( typeof ( ListViewColumnDO ) )]
	public class ListViewDO : DataObject
	{
		#region Attributes
		[DataMember] private string site;
		[DataMember] private long listId;
		[DataMember] private string listName;
		[DataMember] private ArrayList listViewColumns;
		#endregion

		#region Constructor
		public ListViewDO ( )
		{
			this.init ( );
		}

		public ListViewDO ( ListViewClass ListView )
		{
			this.init ( );

			this.listName = ListView.ID;

			foreach (ListViewFieldClass listViewField in ListView.ListViewFieldCollection)
			{				
				listViewColumns.Add ( new ListViewColumnDO(listViewField));
			}
		}
		#endregion

		#region Properties
		public string Site
		{
			get { return site; }
			set { site = value; }
		}

		public long ListId
		{
			get { return listId; }
			set { listId = value; }
		}

		public string ListName
		{
			get { return listName; }
			set { listName = value; }
		}

		public int ColumnCount
		{
			get
			{
				return listViewColumns.Count;
			}
		}
		public ListViewColumnDO this[int index]
		{
			get
			{
				if (index >= listViewColumns.Count)
				{
					return null;
				}
				return (ListViewColumnDO) listViewColumns[index];
			}
			set
			{
				if (index >= listViewColumns.Count)
				{
					listViewColumns.Add ( value );
				}
				else
				{
					listViewColumns[index] = value;
				}
			}
		}
		#endregion

		#region Methods
		override public string getInsertCommand ( )
		{
			return null;
		}

		private void init ( )
		{
			this.listViewColumns = new System.Collections.ArrayList ( );
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getSelectCommand ( )
		{
			return null;
		}

		public void addListViewColumn ( ListViewColumnDO listViewColumn )
		{
			this.listViewColumns.Add ( listViewColumn );
		}

		public ListViewColumnDO getListViewColumn ( int index )
		{
			if (index < listViewColumns.Count)
			{
				return (ListViewColumnDO) this.listViewColumns[index];
			}
			return null;
		}
		#endregion
	}
}
