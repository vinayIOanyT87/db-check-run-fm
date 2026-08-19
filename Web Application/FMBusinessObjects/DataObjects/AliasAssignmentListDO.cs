using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AliasAssignmentListDO : DataObject
	{
		#region Public data members
		[DataMember]
		public ArrayList aliasAssignmentList;
		#endregion Attributes

		#region Protected data members
		[DataMember]
		protected string ownerSite;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Alias Assignment List data object class.
		/// </summary>
		public AliasAssignmentListDO ( )
		{
			this.aliasAssignmentList = new ArrayList ( );
		}
		#endregion

		#region Properties

		public string OwnerSite
		{
			get { return this.ownerSite; }
			set { this.ownerSite = value; }
		}

		public ArrayList AliasAssignmentList
		{
			get { return this.aliasAssignmentList; }
			private set { this.aliasAssignmentList = value; }
		}
		#endregion Properties

		#region Overrides

		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}

		public override void GetSelectCommand(SqlCommand cmd)
		{
			System.Diagnostics.Debug.Assert(ownerSite != null);

			cmd.CommandText = "SELECT a.TransactionAliasGuid, a.AliasName, a.SiteOwner, b.Site, b.AliasCustomName " +
					"FROM tblTransactionAliases a, tblAliasAssignment b " +
					"WHERE a.TransactionAliasGuid = b.TransactionAliasGuid AND a.SiteOwner = @OwnerSite " +
					"ORDER BY a.TransactionAliasGuid";

			cmd.Parameters.Add("@OwnerSite", SqlDbType.NVarChar, 30);
			cmd.Parameters["@OwnerSite"].Value = ownerSite;
		}

		#endregion Overrides
	}
}

