using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class ImportExportConfigurationDO : DataObject
	{
		#region Private data members
		private System.Collections.ArrayList filters;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the import export configuration
		/// data object class.
		/// </summary>
		public ImportExportConfigurationDO()
		{
			this.init();
		}
		#endregion

		#region Properties
		[DataMember]
		public string Site
		{
			get;
			set;
		}

		[DataMember]
		public long ConfigurationID
		{
			get;
			set;
		}

		[DataMember]
		public string ConfigurationName
		{
			get;
			set;
		}

		[DataMember]
		public string ImportExportType
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset FromDate
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset ToDate
		{
			get;
			set;
		}

		[DataMember]
		public string Format
		{
			get;
			set;
		}

		[DataMember]
		public bool MarkSentToEnterprise
		{
			get;
			set;
		}

		[DataMember]
		public bool IncludeDeleted
		{
			get;
			set;
		}

		[DataMember]
		public bool IgnoreDates
		{
			get;
			set;
		}
		#endregion

		#region Public methods
		public void addFilter(ImportExportFiltersDO filters)
		{
			this.filters.Add(filters);
		}

		public ImportExportFiltersDO getFilter(int index)
		{
			return (ImportExportFiltersDO)this.filters[index];
		}
		#endregion

		#region Private methods
		private void init()
		{
			this.filters = new ArrayList();
		}
		#endregion

		#region Public Override Methods
		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return null;
		}

		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}
		#endregion
	}
}
