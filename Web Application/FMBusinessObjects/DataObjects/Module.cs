namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Data;
	using System.Xml.Serialization;
	using System.IO;
	using System.Xml;

	using FMBusinessObjects.UtilityObjects;
    using System.Text;

    [EntityImportExportWorksheetAttribute("MODULES")]
	[DataContract]
	[Serializable]
	[KnownType(typeof(ModuleData))]
	public sealed class Module : BaseDataObject
	{
		[DataMember]
		[FMPersistedField("PointGuid", ReadOnly = true)]
		public Guid PointGuid { get; private set; }

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.MODULE; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

      [EntityImportExportAttribute("MODULEID*", 100, "MODULEID")]
      [FMExposedSetting("Module ID", ModifyDisabled = true)]
      public string ModuleID { get { return this.ID; } set { this.ID = value; } }

      [EntityImportExportAttribute("MODULEGUID", 200, "MODULEGUID")]
		[FMPersistedField]
		public Guid ModuleGuid
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

		[EntityImportExportAttribute("DESCRIPTION*", 200, "DESCRIPTION")]
		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }

		[EntityImportExportAttribute("STANDARD*", 200, "STANDARD")]
		[DataMember]
		[FMPersistedField]
		public bool Standard { get; set; }

		[EntityImportExportAttribute("MODULECALCULATION", 200, "MODULECALCULATION")]
		[DataMember]
		[FMPersistedField]
		public string ModuleCalculation { get; set; }

		[EntityImportExportAttribute("MODULETYPENAME", 200, "MODULETYPENAME")]
		[DataMember]
		[FMPersistedField]
		public string ModuleTypeName { get; set; }

		[DataMember]
		public ModuleData ModuleData { get; set; }

		[EntityImportExportAttribute("MODULEDATA", 200, "MODULEDATA")]
		[FMPersistedField("ModuleData")]
		public string ModuleDataXml
		{
			get
			{
				if (ModuleData == null)
				{
					return null;
				}

				var xmlserializer = CachingXmlSerializerFactory.Create(typeof(ModuleData));

				var stringWriter = new StringWriter();
				var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

				// explicitly remove the xml declaration
				string retValue;
				var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
				using (var writer = XmlWriter.Create(stringWriter, settings))
				{
					xmlserializer.Serialize(writer, ModuleData, emptyNameSpaces);
					retValue = stringWriter.ToString();
				}

				return retValue;
			}
			set
			{
				if (value == null
				|| string.IsNullOrEmpty(value))
				{
					this.ModuleData = null;
					return;
				}

				var serializer = CachingXmlSerializerFactory.Create(typeof(ModuleData));
				var tempReader = new StringReader(value);
				this.ModuleData = (ModuleData)serializer.Deserialize(tempReader);
			}
		}


		[EntityImportExportAttribute("MODULESCRIPT", 200, "MODULESCRIPT")]
		[DataMember]
		[FMPersistedField]
		public string ModuleScript { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public int AssociatedTemplateCount { get; set; }

		public Module()
		{
			this.ModuleCalculation = string.Empty;
		}

		public Module Clone()
		{
			var module = (Module)this.MemberwiseClone();
			return module;
		}

		public void EnumerateByModuleGuidSQL(SqlCommand cmd, Guid moduleGuid)
		{
			cmd.CommandText = String.Format("SELECT m.*,(SELECT COUNT(*) FROM map.tblModuletoPointTemplate PT WHERE PT.ModuleGuid = m.ModuleGuid) AS AssociatedTemplateCount FROM dbo.tblModule m WHERE m.ModuleGuid = @ModuleGuid");
			cmd.Parameters.AddWithValue("@ModuleGuid", moduleGuid);
		}

		public static void EnumeratePointTemplatesByAnyModuleTypeNamesSQL(SqlCommand cmd, string[] moduleTypeNames)
		{
			string moduleTypeNameList = "(";

			foreach (string moduleTypeName in moduleTypeNames)
			{
				moduleTypeNameList += "'" + moduleTypeName + "',";
			}

			if (moduleTypeNameList.Length > 1)
			{
				moduleTypeNameList = moduleTypeNameList.Remove(moduleTypeNameList.Length - 1);
			}

			moduleTypeNameList += ")";

			cmd.CommandText = "SELECT DISTINCT mtpt.PointTemplateGuid FROM dbo.tblModule m" +
									" INNER JOIN map.tblModuleToPointTemplate mtpt ON mtpt.ModuleGuid = m.ModuleGuid" +
									" WHERE m.ModuleTypeName IN " + moduleTypeNameList;
		}

		public static void EnumeratePointTemplatesByAllModuleTypeNamesSQL(SqlCommand cmd, string[] moduleTypeNames)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT t.PointTemplateGuid FROM ( ");
			sb.Append("SELECT mtpt.PointTemplateGuid, ");
			sb.Append("ROW_NUMBER() OVER(PARTITION BY mtpt.PointTemplateGuid ORDER BY mtpt.PointTemplateGuid DESC) AS RowNumber ");
			sb.Append("FROM dbo.tblModule m ");
			sb.Append("INNER JOIN map.tblModuleToPointTemplate mtpt ON mtpt.ModuleGuid = m.ModuleGuid WHERE ");
			sb.Append("m.ModuleTypeName IN (");
			for (int i = 0; i < moduleTypeNames.Length;)
			{
				sb.Append("'" + moduleTypeNames[i] + "'");
				if(++i < moduleTypeNames.Length) sb.Append(", ");
			}
			sb.Append(")) t WHERE t.RowNumber >= ");
			sb.Append(moduleTypeNames.Length);

			cmd.CommandText = sb.ToString();
		}

		public void EnumerateByPointTemplateGuidSQL(SqlCommand cmd, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SELECT m.* FROM dbo.tblModule m where m.ModuleGuid IN (SELECT DISTINCT ModuleGuid FROM map.tblModuleToPointTemplate WHERE PointTemplateGuid = @PointTemplateGuid)";
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}


		public void EnumerateBySiteGuidSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "SELECT m.* FROM dbo.tblModule m"
			                  + " INNER JOIN map.tblEntityModuleToSite ESM ON ESM.ModuleGuid = m.ModuleGuid"
			                  + " WHERE ESM.SiteGuid = @SiteGuid ORDER BY m.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public void EnumerateForAddToPointTemplateGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SELECT m.* from dbo.tblModule m"
									+ " INNER JOIN map.tblEntityModuleToSite ESM ON ESM.ModuleGuid = m.ModuleGuid"
			                  + " WHERE ESM.SiteGuid = @SiteGuid"
			                  + " AND (m.ModuleGuid NOT IN (SELECT ModuleGuid FROM map.tblModuleToPointTemplate WHERE PointTemplateGuid = @PointTemplateGuid)"
			                  + " OR m.ModuleData.value('(/ModuleData/MultipleInstances)[1]', 'BIT') = CAST(1 AS BIT))";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}
	}
}
