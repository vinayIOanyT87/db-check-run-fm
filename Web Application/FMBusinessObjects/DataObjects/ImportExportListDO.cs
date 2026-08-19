using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region Import Export List Item DO class
	[DataContract]
   [Serializable]
	public class ImportExportListItemDO
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the import export list item 
		/// data object class.
		/// </summary>
		public ImportExportListItemDO ()
		{
		}
		#endregion

		#region Properties

		[DataMember]
		public string PluginType { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string LastExported { get; set; }

		[DataMember]
		public bool ImportAllowed { get; set; }

		[DataMember]
		public bool ExportAllowed { get; set; }

		[DataMember]
		public bool Configured { get; set; }

		[DataMember]
		public string Site { get; set; }

		/// <summary>
		/// Gets or sets the import export config GUID.
		/// </summary>
		/// <value>
		/// The import export config GUID.
		/// </value>
		[DataMember]
		public Guid ImportExportConfigGuid { get; set; }

		#endregion
	}
	#endregion

   [Serializable]
   [CollectionDataContract]
	public class ImportExportListDOCollectionClass : List<ImportExportListItemDO> {}

	#region Import Export List DO class
	/// <summary>
	/// Summary description for ImportExportListDO.
	/// </summary>
	[DataContract]
   [Serializable]
	public class ImportExportListDO : DataObject
	{
		#region Protected data members

		[DataMember]
		protected string site;
		[DataMember]
		protected ImportExportListDOCollectionClass importExportList;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the import export list
		/// data object class.
		/// </summary>
		public ImportExportListDO ( )
		{
			this.importExportList = new ImportExportListDOCollectionClass();
		}
		#endregion

		#region Properties

		public ImportExportListDOCollectionClass ImportExportList
		{
			get { return this.importExportList; }
			private set { this.importExportList = value; }
		}

		public string Site
		{
			get { return this.site; }
			set { this.site = value; }
		}

		#endregion

		#region SQL methods
		public void InsertSQL(SqlCommand cmd)
		{
			this.GetInsertCommand(cmd);
		}

		//public void Delete ( System.Data.SqlClient.SqlConnection conn )
		//{
		//   System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand ( getDeleteCommand ( ), conn );
		//   cmd.CommandText = getDeleteCommand ( );
		//   cmd.Parameters.Add ( "@site", System.Data.SqlDbType.NVarChar, 50 );
		//   cmd.Parameters["@site"].Value = Site;
		//   cmd.ExecuteNonQuery ( );
		//}

		public override void GetSelectCommand(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT b.Site, b.ImportExportName, a.PluginType, a.ConfigURL, a.RunURL, a.Import, a.Export, " +
				"b.ConfigName, b.LastExported, b.ImportExportConfigGuid " +
				"FROM tblImportExportPlugins a, tblImportExportConfig b " +
				"WHERE a.PluginType = b.PluginType and b.Site = @Site " +
				"ORDER BY b.ImportExportName";

			cmd.Parameters.AddWithValue("@Site", site);
		}

		public override void GetInsertCommand(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblImportExportConfig " +
				"(Site, ImportExportName, PluginType, ConfigName, LastExported) " +
				"VALUES (@site, @importExportName, @pluginType, @configName, @lastExported)";

			cmd.Parameters.Add("@site", System.Data.SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@importExportName", System.Data.SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@pluginType", System.Data.SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@configName", System.Data.SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@lastExported", System.Data.SqlDbType.NVarChar, 50);
			cmd.Prepare();
		}

		public override void GetDeleteCommand(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE from tblImportExportConfig WHERE Site = @Site";

			cmd.Parameters.Add("@Site", System.Data.SqlDbType.NVarChar, 50);
		}

		public override string getInsertCommand()
		{
			return null;
		}
		public override string getDeleteCommand()
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
		#endregion SQL methods
	}
	#endregion
}
