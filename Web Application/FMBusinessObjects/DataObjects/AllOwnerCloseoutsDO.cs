using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AllOwnerCloseoutsDO : DataObject
	{
		#region Attributes
		[DataMember]
		protected ArrayList closeoutList;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// This is the default constructor for the All Owner Closeouts data object class.
		/// </summary>
		public AllOwnerCloseoutsDO ( )
		{
			this.closeoutList = new ArrayList ( );
		}
		#endregion

		#region Properties

		public ArrayList CloseoutList
		{
			get { return this.closeoutList; }
			private set { this.closeoutList = value; }
		}
		#endregion Properties

		#region Overrides

		public override string getSelectCommand ( )
		{
			return null;
		}

		public override string getDeleteCommand ( )
		{
			return null;
		}

		public override string getInsertCommand ( )
		{
			return null;
		}

		public override string getUpdateCommand ( )
		{
			return null;
		}

		public override void GetSelectCommand(SqlCommand cmd)
		{
			cmd.CommandText = 
				"SELECT Site, SiteGuid, CloseoutDate, ProductName, ProductGuid, ManagerName, ManagerCompanyGuid, GrossBookInventory, NetBookInventory " +
				"FROM tblOwnerCloseout a " +
				"WHERE CloseoutDate = " +
				" (SELECT MAX(CloseoutDate) " +
				"  FROM tblOwnerCloseout b " +
				"  WHERE a.Site = b.Site AND a.ProductName = b.ProductName AND a.ManagerName = b.ManagerName) " +
				"ORDER BY Site, ProductName, ManagerName, CloseoutDate DESC";	
		}

		#endregion Overrides
	}
}
