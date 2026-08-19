using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class QueryDefaultFieldCollectionClass : List<QueryDefaultFieldClass> { }

   [Serializable]
   [DataContract]
	public class QueryDefaultFieldClass : BaseDataObject
	{
		[DataMember]
		public string Topic { get; set; }

		[DataMember]
		public string FieldName { get; set; }

		[DataMember]
		public int Order { get; set; }

		public override string ID
		{
			get
			{
				return Topic + "/" + FieldName;
			}
		}

		public QueryDefaultFieldClass()
		{
			Reset();
		}


		public QueryDefaultFieldClass(QueryWriterField Field)
		{
			Reset();

			Topic = Field.Topic.ObjectType.ToString();
			FieldName = Field.FieldName;
		}


		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.QUERY_DEFAULT_FIELD;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		public override void Reset()
		{
			base.Reset();

			Topic = "";
			FieldName = "";
		}

		public override void Load(object o)
		{
			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;

				Reset();

				DataTable Table = Set.Tables[0];
				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				_IdentityGuid = DataObject.getValue<Guid>(Row["QueryDefaultFieldGuid"], Guid.Empty);
				_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				Topic = DataObject.getValue<string>(Row["Topic"], "");
				FieldName = DataObject.getValue<string>(Row["FieldName"], "");
				Order = DataObject.getValue<int>(Row["Order"], 0);
				_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			}
			else
			{
				base.Load(o);
			}

		}

		#region Parameterized SQL
		
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblQueryDefaultFields " +
				"(SiteGuid, " +
				"Topic, " +
				"FieldName, " +
				"[Order], " +
				"CreatedDate, " +
				"CreatedBy, " +
				"UpdatedDate, " +
				"UpdatedBy, " +
				"QueryDefaultFieldGuid) " +
				" VALUES ( " +
				"@SiteGuid, " +
				"@Topic, " +
				"@FieldName, " +
				"@Order, " +
				"@CreatedDate, " +
				"@CreatedBy, " +
				"@UpdatedDate, " +
				"@UpdatedBy, " +
				"@QueryDefaultFieldGuid)";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier); 
			cmd.Parameters.Add("@Topic", SqlDbType.NVarChar, 100); 
			cmd.Parameters.Add("@FieldName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Order", SqlDbType.Int);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset); 
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@QueryDefaultFieldGuid", SqlDbType.UniqueIdentifier); 

			cmd.Parameters["@SiteGuid"].Value = _SiteGuid; 
			cmd.Parameters["@Topic" ].Value = Topic;
			cmd.Parameters["@FieldName"].Value = FieldName;
			cmd.Parameters["@Order"].Value = Order;
			cmd.Parameters["@CreatedDate"].Value = _CreatedDate; 
			cmd.Parameters["@CreatedBy"].Value = _CreatedBy; 
			cmd.Parameters["@UpdatedDate"].Value = _UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = _UpdatedBy;
			cmd.Parameters["@QueryDefaultFieldGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblQueryDefaultFields SET " +
				" SiteGuid = @SiteGuid, " + 
				" Topic = @Topic, " + 
				" FieldName = @FieldName, " + 
				" [Order] = @Order, " +
				" UpdatedDate = @UpdatedDate, " +
				" UpdatedBy = @UpdatedBy " +
				" WHERE QueryDefaultFieldGuid = @QueryDefaultFieldGuid";
			
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Topic", SqlDbType.NVarChar, 100);   
			cmd.Parameters.Add("@FieldName", SqlDbType.NVarChar, 50); 
			cmd.Parameters.Add("@Order", SqlDbType.Int); 
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset); 
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100); 
			cmd.Parameters.Add("@QueryDefaultFieldGuid", SqlDbType.UniqueIdentifier); 

			cmd.Parameters["@SiteGuid"].Value = _SiteGuid ;  
			cmd.Parameters["@Topic"].Value = Topic;  
			cmd.Parameters["@FieldName"].Value = FieldName;  
			cmd.Parameters["@Order"].Value = Order;
			cmd.Parameters["@UpdatedDate"].Value = _UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = _UpdatedBy;
			cmd.Parameters["@QueryDefaultFieldGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblQueryDefaultFields WHERE QueryDefaultFieldGuid = @QueryDefaultFieldGuid";
			cmd.Parameters.Add("@QueryDefaultFieldGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@QueryDefaultFieldGuid"].Value = IdentityGuid;  
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblQueryDefaultFields " + SQLUpdateLock(bInTransaction) +
				"WHERE  QueryDefaultFieldGuid = @QueryDefaultFieldGuid";
			cmd.Parameters.Add("@QueryDefaultFieldGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@QueryDefaultFieldGuid"].Value = IdentityGuid; 
		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblQueryDefaultFields" +
				  " WHERE (SiteGuid = @SiteGuid" + 
				  " OR SiteGuid = (SELECT SiteGuid FROM map.tblEntityQuerySettingToSite" +
				  " WHERE MapToSiteGuid = @SiteGuid))" +
				  " ORDER BY [Order]";
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = _SiteGuid; 
		}

		public void EnumerateBySiteSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblQueryDefaultFields" +
				  " WHERE SiteGuid = @SiteGuid ORDER BY [Order]";
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = _SiteGuid; 
		}

		#endregion

	}

}
