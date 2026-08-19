

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Web.Script.Serialization;
	using System.Xml;
	using System.Xml.Serialization;

	using FMBusinessObjects.UtilityObjects;

	[DataContract]
	[Serializable]
	public class AnimationPropertyVisualState
	{
		[DataMember]
		public Guid AnimationPropertyVisualStateGuid { get; set; }

		[DataMember]
		public string Value { get; set; }
	}

	[DataContract]
	[Serializable]
	public class AnimationProperty
	{
		[DataMember]
		public Guid AnimationPropertyGuid { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string LookupName { get; set; }

		[DataMember]
		public string gojsPropertyName { get; set; }
		
		[DataMember]
		public List<AnimationPropertyVisualState> VisualStates { get; set; }

	}

	[DataContract]
	[Serializable]
	public class AnimationTest
	{
		[DataMember]
		public Guid AnimationTestGuid { get; set; }

		[DataMember]
		public EAnimationTestComparisonOperators TestComparisonOperator { get; set; }

		[DataMember]
		public long Bitmask { get; set; }

		[DataMember]
		public string BitmaskStr { get; set; }

		[DataMember]
		public EAnimationTestBitmaskOperators BitmaskOperator { get; set; }

		[DataMember]
		public string ComparisonValue { get; set; }

		//Properties
		[DataMember]
		public List<AnimationProperty> PropertyList { get; set; }

	}

	[DataContract]
	[Serializable]
	public class AnimationTestGroup
	{
		[DataMember]
		public Guid AnimationTestGroupGuid { get; set; }

		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public string DataType { get; set; }

		[DataMember]
		public PointValueFieldType Field { get; set; }

		[DataMember]
		public List<AnimationTest> TestList { get; set; }

	}

	[DataContract]
	[Serializable]
	public class AnimationClass : BaseDataObject
	{
		[DataMember]
		[FMPersistedField]
		public Guid AnimationGuid
		{
			get
			{
				return base.IdentityGuid;
			}
			set
			{
				base.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public new Guid SiteGuid
		{
			get
			{
				return base.SiteGuid;
			}
			set
			{
				base.SiteGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField("UseCount",ReadOnly = true)]
		public int UseCount { get; set; }


		[DataMember]
		public List<AnimationTestGroup> AnimationTestGroupList { get; set; }

		[ScriptIgnore]
		[XmlIgnore]
		[FMPersistedField("AnimationTestGroupList")]
		public string AnimationTestGroupListXml
		{
			get
			{
				var retValue = "";
				if (this.AnimationTestGroupList == null)
				{
					retValue = null;
				}
				else
				{
					XmlSerializer xmlserializer = CachingXmlSerializerFactory.Create(typeof(List<AnimationTestGroup>));

					var stringWriter = new StringWriter();
					var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
					// explicitly remove the xml declaration
					var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
					using (var writer = XmlWriter.Create(stringWriter, settings))
					{
						xmlserializer.Serialize(writer, this.AnimationTestGroupList, emptyNameSpaces);
						retValue = stringWriter.ToString();
					}
				}

				return retValue;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.AnimationTestGroupList = null;
					return;
				}

				XmlSerializer serializer = CachingXmlSerializerFactory.Create(typeof(List<AnimationTestGroup>));

				var tempReader = new StringReader(value);
				this.AnimationTestGroupList = (List<AnimationTestGroup>)serializer.Deserialize(tempReader);
			}
		}

		public static void EnumerateBySiteGuidSQL(SqlCommand cmd, Guid siteGuid)
		{

			cmd.CommandText = "SELECT a.*, ISNULL(c.UseCount,0) AS UseCount from dbo.tblAnimation a"
								 + " LEFT OUTER JOIN("
								 + " select atd.AnimationGuid, COUNT(atd.DrawingGuid) AS UseCount from map.tblAnimationToDrawing atd"
								 + " INNER JOIN tblAnimation an ON an.AnimationGuid = atd.AnimationGuid"
								 + " Group By atd.AnimationGuid"
								 + " ) c ON a.AnimationGuid = c.AnimationGuid"
								 + " WHERE a.SiteGuid = @SiteGuid"
								 + " ORDER BY AnimationGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public static void EnumerateByAnimationGuidListSQL(SqlCommand cmd, List<Guid> animationGuidList)
		{
			cmd.CommandText = "SELECT a.*, ISNULL(c.UseCount,0) AS UseCount from dbo.tblAnimation a"
								 + " INNER JOIN @GuidTable gtbl"
 								 + " ON gtbl.Guid = a.AnimationGuid"
								 + " Left Outer Join("
								 + " select atd.AnimationGuid, COUNT(atd.DrawingGuid) AS UseCount from map.tblAnimationToDrawing atd"
								 + " INNER JOIN tblAnimation an"
								 + " ON an.AnimationGuid = atd.AnimationGuid"
								 + " Group By atd.AnimationGuid"
								 + " ) c"
								 + " ON a.AnimationGuid = c.AnimationGuid"
								 + " ORDER BY AnimationGuid";
			GenerateGuidListTable(cmd, animationGuidList);
		}

		public static void DeleteListSQL(SqlCommand cmd, List<Guid> animationGuidList)
		{
			cmd.CommandText = "DELETE a FROM dbo.tblAnimation a"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AnimationGuid";
			GenerateGuidListTable(cmd, animationGuidList);
		}

		protected static DataTable CreateAnimationListDataTable(List<AnimationClass> animationList, SecurityClass security)
		{

			var table = new DataTable();
			table.Columns.Add("AnimationGuid", typeof(Guid));
			table.Columns.Add("ID", typeof(string));
			table.Columns.Add("SiteGuid", typeof(Guid));
			table.Columns.Add("AnimationTestGroupList", typeof(string));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var animation in animationList)
			{
				var row = table.NewRow();
				row["AnimationGuid"] = animation.AnimationGuid;
				row["ID"] = animation.ID;
				row["SiteGuid"] = animation.SiteGuid;
				row["AnimationTestGroupList"] = animation.AnimationTestGroupListXml;
				row["UpdatedBy"] = security.UserID;
				table.Rows.Add(row);
			}

			return table;
		}

		public static void AddModifyStoredProcedure(SqlCommand cmd, List<AnimationClass> animationList, SecurityClass security, bool enableAdd, bool enableModify)
		{
			if (animationList == null || animationList.Count < 1)
			{
				return;
			}
			var table = CreateAnimationListDataTable(animationList, security);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_AnimationAddModify";

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AnimationTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.AnimationDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
		}
	}
}
