namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Data;
	using System.Linq;
	using Interfaces;
	using System.IO;
	using System.Xml.Serialization;
	using System.Xml;

	using FMBusinessObjects.UtilityObjects;

	[DataContract]
	[Serializable]
	public class ModuleToPointTemplateMap : BaseDataObject
	{

		[EntityImportExportAttribute("MODULEID*", 200, "MODULEID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9 ]*$")) { base.ID = value; } else throw new Exception("Module Instance ID must be Alphanumeric"); } }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string ModuleID { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string ModuleTypeName { get; set; }


		[EntityImportExport("ORDER", 100, "ORDER")]
		[DataMember]
		[FMPersistedField]
		public int Order { get; set; }


		[DataMember]
		public ModuleToPointTemplateData ModuleToPointTemplateData { get; set; }

		[EntityImportExport("MODULETOPOINTTEMPLATEDATA", 250, "MODULETOPOINTTEMPLATEDATA")]
		[FMPersistedField("ModuleToPointTemplateData")]
		public string ModuleToPointTemplateDataXml
		{
			get
			{
				if (ModuleToPointTemplateData == null)
				{
					return null;
				}

				var xmlserializer = CachingXmlSerializerFactory.Create(typeof(ModuleToPointTemplateData));

				var stringWriter = new StringWriter();
				var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

				// explicitly remove the xml declaration
				string retValue;
				var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
				using (var writer = XmlWriter.Create(stringWriter, settings))
				{
					xmlserializer.Serialize(writer, ModuleToPointTemplateData, emptyNameSpaces);
					retValue = stringWriter.ToString();
				}

				return retValue;
			}
			set
			{
				if (value == null
				|| string.IsNullOrEmpty(value))
				{
					this.ModuleToPointTemplateData = null;
					return;
				}

				var serializer = CachingXmlSerializerFactory.Create(typeof(ModuleToPointTemplateData));
				var tempReader = new StringReader(value);
				this.ModuleToPointTemplateData = (ModuleToPointTemplateData)serializer.Deserialize(tempReader);
			}
		}

		[DataMember]
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

		[EntityImportExport("MODULETOPOINTTEMPLATEGUID", 100, "MODULETOPOINTTEMPLATEGUID")]
		[FMPersistedField]
		public Guid ModuleToPointTemplateGuid
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

		[EntityImportExport("MODULEGUID", 100, "MODULEGUID")]
		[FMPersistedField]
		[DataMember]
		public Guid ModuleGuid { get; set; }

		[EntityImportExport("POINTTEMPLATEGUID", 100, "POINTTEMPLATEGUID")]
		[FMPersistedField]
		[DataMember]
		public Guid PointTemplateGuid { get; set; }


		public ModuleToPointTemplateMap()
		{
		}

		public ModuleToPointTemplateMap Clone()
		{
			var module = (ModuleToPointTemplateMap)this.MemberwiseClone();

			return module;
		}

		public void EnumerateByTemplateGuidSQL(SqlCommand cmd, Guid templateGuid)
		{
			cmd.CommandText = "SELECT mtpt.*, m.ID as ModuleID, m.ModuleTypeName FROM map.tblModuleToPointTemplate mtpt"
									+ " LEFT JOIN dbo.tblModule m ON m.ModuleGuid = mtpt.ModuleGuid"
									+ " WHERE PointTemplateGuid = @PointTemplateGuid"
								   + " ORDER BY [Order]";
			cmd.Parameters.AddWithValue("@PointTemplateGuid", templateGuid);
		}

		public void EnumerateByPointGuidSQL(SqlCommand cmd, Guid pointGuid)
		{
			cmd.CommandText = "SELECT mtpt.*, m.ID as ModuleID, m.ModuleTypeName FROM map.tblModuleToPointTemplate mtpt"
									+ " LEFT JOIN dbo.tblModule m ON m.ModuleGuid = mtpt.ModuleGuid"
									+ " INNER JOIN tblPoint p ON p.PointTemplateGuid = mtpt.PointTemplateGuid "
									+ " WHERE p.PointGuid = @PointGuid ORDER BY [Order]";
			cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
		}

		public void EnumerateByModuleToPointTemplateGuidSQL(SqlCommand cmd, Guid ModuleToPointTemplateGuid)
		{
			cmd.CommandText = "SELECT mtpt.*,  m.ID as ModuleID, m.ModuleTypeName FROM map.tblModuleToPointTemplate mtpt"
									+ " LEFT JOIN dbo.tblModule m ON m.ModuleGuid = mtpt.ModuleGuid"
									+ " WHERE mtpt.ModuleToPointTemplateGuid = @ModuleToPointTemplateGuid";
			cmd.Parameters.AddWithValue("@ModuleToPointTemplateGuid", ModuleToPointTemplateGuid);
		}
	}
}
