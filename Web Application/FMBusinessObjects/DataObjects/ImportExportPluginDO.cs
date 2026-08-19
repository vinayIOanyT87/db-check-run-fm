using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace FMBusinessObjects.DataObjects
{
	#region Import Export Plugin Item DO class
	[DataContract]
   [Serializable]
	public class ImportExportPluginItemDO
	{
		#region Protected data members
		[DataMember]
		protected string pluginType;
		[DataMember]
		protected string configURL;
		[DataMember]
		protected string runURL;
		[DataMember]
		protected bool import;
		[DataMember]
		protected bool export;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the import export plugin item 
		/// data object class.
		/// </summary>
		public ImportExportPluginItemDO ()
		{
		}
		#endregion

		#region Properties

		public string PluginType
		{
			get { return this.pluginType; }
			set { this.pluginType = value; }
		}

		public string ConfigURL
		{
			get { return this.configURL; }
			set { this.configURL = value; }
		}

		public string RunURL
		{
			get { return this.runURL; }
			set { this.runURL = value; }
		}

		public bool Import
		{
			get { return this.import; }
			set { this.import = value; }
		}

		public bool Export
		{
			get { return this.export; }
			set { this.export = value; }
		}
		#endregion
	}
	#endregion

	#region Import Export Plugin DO Class
	/// <summary>
	/// Summary description for ImportExportPluginDO.
	/// </summary>
	[DataContract]
   [Serializable]
	[KnownType ( typeof ( ImportExportPluginItemDO ) )]
	public class ImportExportPluginDO : DataObject
	{
		#region Protected data members
		[DataMember]
		protected ArrayList pluginList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the import export plugin data
		/// object class.
		/// </summary>
		public ImportExportPluginDO ( )
		{
			this.pluginList = new System.Collections.ArrayList ( );
		}
		#endregion

		#region Properties

		public ArrayList PluginList
		{
			get { return this.pluginList; }
			private set { this.pluginList = value; }
		}
		#endregion

		#region SQL methods
		override public string getSelectCommand ( )
		{
			return "SELECT PluginType, ConfigURL, RunURL, Import, Export FROM tblImportExportPlugins";
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}
		#endregion
	}
	#endregion
}
