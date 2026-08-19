using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// A collection, in the form of a Dictionary, of information from
	/// HelpMappingClass objects
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class HelpMappingDictionary : Dictionary<string, string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HelpMappingDictionary"/> class.
		/// </summary>
		public HelpMappingDictionary()
		{
		}

		/// <summary>
		/// This constructor is required when SessionState = StateServer to deserialize the object.
		/// The reason it is required is because this class inherits from Dictionary, which implements ISerializable.
		/// </summary>
		/// <param name="info">SerializationInfo to populate with data to represent the object</param>
		/// <param name="context">Destination for serialization</param>
		public HelpMappingDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public string GetHelpPage(string helpContextKey)
		{
			string page;
			if (!this.TryGetValue(helpContextKey, out page))
				return "";

			return page;
		}
	}

	/// <summary>
	/// A class that encapsulates a record from tblHelpMapping. This
	/// object represents a value representing the current context of the 
	/// application with the appropriate help page.
	/// </summary>
	[Serializable]
	[DataContract]
	public class HelpMappingClass : BaseDataObject
	{
		#region Public Properties

		/// <summary>
		/// The key that represents the context, often the relative
		/// path of an ASPX page
		/// </summary>
		public string HelpContextKey { get; set; }

		/// <summary>
		/// The relative path of the help page
		/// </summary>
		public string HelpPage { get; set; }

		/// <summary>
		/// Override of BaseDataObject.EntityType
		/// </summary>
		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		/// <summary>
		/// Override of BaseDataObject.ParentEntityType
		/// </summary>
		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Clear out the data object by assigning default values
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			HelpContextKey = "";
			HelpPage = "";
		}

		/// <summary>
		/// Load the object from a DataRow, DataSet, HelpMappingClass,
		/// or XmlNode
		/// </summary>
		/// <param name="o">Object to load from</param>
		public override void Load(object o)
		{
			Reset();

			DataRow row = null;

			if (typeof(DataRow).IsInstanceOfType(o))
			{
				row = (DataRow)o;
			}

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet set = (DataSet)o;

				DataTable table = set.Tables[0];
				if (table.Rows.Count == 0)
					return;

				row = table.Rows[0];
			}

			if (row != null)
			{
				_IdentityGuid = DataObject.getValue<Guid>(row["HelpMappingGuid"], Guid.Empty);
				HelpContextKey = DataObject.getValue<string>(row["HelpContextKey"], ""); ;
				HelpPage = DataObject.getValue<string>(row["HelpPage"], ""); ;
			}
			else if (typeof(HelpMappingClass).IsInstanceOfType(o))
			{
				HelpMappingClass mapping = (HelpMappingClass)o;
				this._IdentityGuid = mapping.IdentityGuid;
				this.HelpContextKey = mapping.HelpContextKey;
				this.HelpPage = mapping.HelpPage;
			}
			else
			{
				base.Load(o);
			}
		}

		/// <summary>
		/// Provide SQL to enumerate all records
		/// </summary>
		/// <param name="cmd">SqlCommand to be used</param>
		/// <param name="bInTransaction">Whether to use locking</param>
		public void EnumerateSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT *" +
				" FROM tblHelpMapping " + SQLUpdateLock(bInTransaction) +
				" ORDER BY HelpContextKey";
		}

		#endregion
	}
}
