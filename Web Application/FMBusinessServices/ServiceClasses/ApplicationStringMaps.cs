using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Summary description for ApplicationStringMapsClass.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ApplicationStringMapsClass : IDependency, IApplicationStringMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, ApplicationStringMapClass applicationStringMap)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (applicationStringMap == null)
		    {
		        throw new ArgumentNullException("applicationStringMap");
		    }

			applicationStringMap.CreatedDate = DateTimeOffset.Now;
			applicationStringMap.CreatedBy = security.UserID;
			applicationStringMap.UpdatedDate = applicationStringMap.CreatedDate;
			applicationStringMap.UpdatedBy = security.UserID;
			applicationStringMap.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				applicationStringMap.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ApplicationStringMapClass applicationStringMap)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (applicationStringMap == null)
		    {
		        throw new ArgumentNullException("applicationStringMap");
		    }

			applicationStringMap.UpdatedDate = DateTimeOffset.Now;
			applicationStringMap.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				applicationStringMap.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid, STRING_MAP_TYPE type)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

			var applicationStringMap = new ApplicationStringMapClass();
			applicationStringMap.Type = type;
			applicationStringMap.IdentityGuid = identityGuid;

			using (var cmd = new SqlCommand())
			{
				applicationStringMap.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public ApplicationStringMapClass Get(SecurityClass security, Guid assignedGuid, Guid applicationStringGuid, STRING_MAP_TYPE type)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

			var applicationStringMap = new ApplicationStringMapClass
			                           {
			                               IdentityGuid = assignedGuid,
			                               ApplicationStringGuid = applicationStringGuid,
			                               Type = type
			                           };

		    using (var cmd = new SqlCommand())
			{
				applicationStringMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				applicationStringMap.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return applicationStringMap;
		}

		public ApplicationStringMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass security, Guid assignedToGuid, STRING_MAP_TYPE type)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			var applicationStringMap = new ApplicationStringMapClass
			                           {
			                               AssignedToGuid = assignedToGuid,
			                               Type = type,
			                               SiteGuid = security.SiteGuid
			                           };
		    DataSet set;

            using (var cmd = new SqlCommand())
            {
					switch (type)
					{
                        case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT: //Product RecordVersioning-aware query.
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.CommandText = "map.usp_GetFootnoteToProductMapsByProduct";
							cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
							cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
							if (assignedToGuid != Guid.Empty)
								cmd.Parameters["@ProductGuid"].Value = assignedToGuid;
							break;
                        case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO: //Company RecordVersioning-aware query.
							cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "map.usp_GetFootnoteToShipToMapsByShipTo";
							cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@ShipToGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                            cmd.Parameters["@ShipToGuid"].Value = DBNull.Value;
							if (assignedToGuid != Guid.Empty)
                                cmd.Parameters["@ShipToGuid"].Value = assignedToGuid;
							break;
                        case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:  //Company RecordVersioning-aware query.
							cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "map.usp_GetFootnoteToShipperMapsByShipper";
							cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@ShipperGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
                            cmd.Parameters["@ShipperGuid"].Value = DBNull.Value;
							if (assignedToGuid != Guid.Empty)
                                cmd.Parameters["@ShipperGuid"].Value = assignedToGuid;
							break;
						default:
							applicationStringMap.EnumerateByAssignedToGuidAndTypeSQL(cmd);
							break;
					}
                set = ConsolidatedDA.GetDataSet(cmd, security);
            }


			var applicationStringMapCollection = new ApplicationStringMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				applicationStringMap = new ApplicationStringMapClass();
				applicationStringMap.Type = type;
				applicationStringMap.Load(set);
				applicationStringMapCollection.Add(applicationStringMap);
				table.Rows.RemoveAt(0);
			}

			return applicationStringMapCollection;
		}

		public ApplicationStringMapCollectionClass EnumerateByApplicationStringGuidAndType(SecurityClass security, Guid applicationStringGuid, STRING_MAP_TYPE type)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

			var applicationStringMap = new ApplicationStringMapClass();
			applicationStringMap.ApplicationStringGuid = applicationStringGuid;
			applicationStringMap.Type = type;
			applicationStringMap.SiteGuid = security.SiteGuid;
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				switch (type)
				{
					case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:  //Product RecordVersioning-aware query.
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "map.usp_GetFootnoteToProductMapsByFootnote";
						cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);

						cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
						cmd.Parameters["@ApplicationStringGuid"].Value = applicationStringGuid;
						break;
               case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO: //Company RecordVersioning-aware query.
						cmd.CommandType = CommandType.StoredProcedure;
                  cmd.CommandText = "map.usp_GetFootnoteToShipToMapsByFootnote";
						cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
						cmd.Parameters["@ApplicationStringGuid"].Value = applicationStringGuid;
						break;
               case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER: //Company RecordVersioning-aware query.
						cmd.CommandType = CommandType.StoredProcedure;
                  cmd.CommandText = "map.usp_GetFootnoteToShipperMapsByFootnote";
 						cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
						cmd.Parameters["@ApplicationStringGuid"].Value = applicationStringGuid;
						break;
					default:
						applicationStringMap.EnumerateByApplicationStringGuidAndTypeSQL(cmd, ContextUtil.IsInTransaction);
						break;
				}
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			var applicationStringMapCollection = new ApplicationStringMapCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				applicationStringMap = new ApplicationStringMapClass();
				applicationStringMap.Type = type;
				applicationStringMap.Load(set);
				applicationStringMapCollection.Add(applicationStringMap);
				table.Rows.RemoveAt(0);
			}

			return applicationStringMapCollection;
		}

		// Note: STRING_TYPE.EMAIL_ADDRESS is handled in a special manner
		//			When added, the corresponding String is added.  When purged
		//       the corresponding String is purged.
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security, Guid identityGuid, ApplicationStringMapCollectionClass newApplicationStringMapCollection, ApplicationStringMapCollectionClass existingApplicationStringMapCollection)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

			var applicationStrings = new ApplicationStringsClass();
			var applicationStringMaps = new ApplicationStringMapsClass();

			if (newApplicationStringMapCollection != null)
			{
				foreach (ApplicationStringMapClass newApplicationStringMap in newApplicationStringMapCollection)
				{
				    if (newApplicationStringMap.Type == STRING_MAP_TYPE.FOOT_NOTE_PRODUCT
				        || newApplicationStringMap.Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPPER
				        || newApplicationStringMap.Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPTO
				        || newApplicationStringMap.Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE
                        || newApplicationStringMap.Type == STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE)
				    {
				        newApplicationStringMap.ApplicationStringGuid = identityGuid;
				    }
				    else
				    {
				        newApplicationStringMap.AssignedToGuid = identityGuid;
				    }

					// Automatically add email addresses to string table
					if (newApplicationStringMap.Type == STRING_MAP_TYPE.EMAIL_ADDRESS
					&& newApplicationStringMap.ApplicationStringGuid == Guid.Empty)
					{
						newApplicationStringMap.ApplicationStringGuid = applicationStrings.GetIdentityGuid(security, STRING_TYPE.EMAIL_ADDRESS, newApplicationStringMap.ID);
						if (newApplicationStringMap.ApplicationStringGuid == Guid.Empty)
						{
							var applicationString = new ApplicationStringClass();
							applicationString.Type = STRING_TYPE.EMAIL_ADDRESS;
							applicationString.ID = newApplicationStringMap.ID;
							newApplicationStringMap.ApplicationStringGuid = applicationStrings.Add(security, applicationString);
						}
					}

					// Automatically add Ship To States to string table
					if (newApplicationStringMap.Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE
					&& newApplicationStringMap.AssignedToGuid == Guid.Empty
					&& newApplicationStringMap.AssignedToID != "{All}")
					{
						newApplicationStringMap.AssignedToGuid = applicationStrings.GetIdentityGuid(security, STRING_TYPE.SHIPTO_STATE, newApplicationStringMap.AssignedToID);
						if (newApplicationStringMap.AssignedToGuid == Guid.Empty)
						{
							var applicationString = new ApplicationStringClass();
							applicationString.Type = STRING_TYPE.SHIPTO_STATE;
							applicationString.ID = newApplicationStringMap.AssignedToID;
							newApplicationStringMap.AssignedToGuid = applicationStrings.Add(security, applicationString);
						}
					}


				    if (existingApplicationStringMapCollection != null)
				    {
				        int item = 0;
				        foreach (ApplicationStringMapClass existingApplicationStringMap in existingApplicationStringMapCollection)
				        {
				            if (existingApplicationStringMap.AssignedToGuid == newApplicationStringMap.AssignedToGuid
				                && existingApplicationStringMap.ApplicationStringGuid == newApplicationStringMap.ApplicationStringGuid)
				            {
				                if (existingApplicationStringMap.Sequence != newApplicationStringMap.Sequence)
				                {
				                    newApplicationStringMap.IdentityGuid = existingApplicationStringMap.IdentityGuid;
				                    applicationStringMaps.Modify(security, newApplicationStringMap);
				                }
				                break;
				            }
				            item++;
				        }

				        if (item == existingApplicationStringMapCollection.Count)
				        {
				            applicationStringMaps.Add(security, newApplicationStringMap);
				        }
				        else
				        {
				            existingApplicationStringMapCollection.RemoveAt(item);
				        }
				    }
				    else
				    {
				        applicationStringMaps.Add(security, newApplicationStringMap);
				    }

				}
			}

			if (existingApplicationStringMapCollection != null)
			{
				foreach (ApplicationStringMapClass applicationStringMap in existingApplicationStringMapCollection)
				{
					applicationStringMaps.Purge(security, applicationStringMap.IdentityGuid, applicationStringMap.Type);
					if (applicationStringMap.Type == STRING_MAP_TYPE.EMAIL_ADDRESS)
					{
					    ApplicationStringMapCollectionClass applicationStringMapCollection = this.EnumerateByApplicationStringGuidAndType(security, applicationStringMap.ApplicationStringGuid, applicationStringMap.Type);
					    if (applicationStringMapCollection.Count == 0)
					    {
					        applicationStrings.Purge(security, applicationStringMap.ApplicationStringGuid);
					    }
					}

				    if (applicationStringMap.Type == STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE && applicationStringMap.AssignedToID != "{All}")
					{                     
					    ApplicationStringMapCollectionClass applicationStringMapCollection = this.EnumerateByAssignedToGuidAndType(security, applicationStringMap.AssignedToGuid, applicationStringMap.Type);
					   
                        if (applicationStringMapCollection.Count == 0)
					    {
					        applicationStrings.Purge(security, applicationStringMap.AssignedToGuid);
					    }
					}
				}
			}
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (Object == null)
		    {
		        throw new ArgumentNullException("Object");
		    }
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (Object == null)
		    {
		        throw new ArgumentNullException("Object");
		    }
		}


		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (Object == null)
		    {
		        throw new ArgumentNullException("Object");
		    }


			if (typeof(ProductClass).IsInstanceOfType(Object))
			{
				var product = (ProductClass)Object;
			    ApplicationStringMapCollectionClass applicationStringMapCollection = this.EnumerateByAssignedToGuidAndType(security, product.IdentityGuid, STRING_MAP_TYPE.PRODUCT_MESSAGE);
				foreach (ApplicationStringMapClass applicationStringMap in applicationStringMapCollection)
                {
                    Purge(security, applicationStringMap.IdentityGuid, applicationStringMap.Type);
                }
			}

			// Delete Alarm Category
			if (typeof(ApplicationStringClass).IsInstanceOfType(Object))
			{
				var applicationString = (ApplicationStringClass)Object;
				if (applicationString.Type == STRING_TYPE.ALARM_EVENT_CATEGORY)
				{
					ApplicationStringMapCollectionClass applicationStringMapCollection = this.EnumerateByApplicationStringGuidAndType(security, applicationString.IdentityGuid, STRING_MAP_TYPE.ALARM_EVENT_CATEGORY);
					foreach (ApplicationStringMapClass categoryMap in applicationStringMapCollection)
					{
						if (categoryMap.ApplicationStringGuid == applicationString.IdentityGuid)
						{
							{
                                Purge(security, categoryMap.IdentityGuid, categoryMap.Type);
                            }
						}
					}
				}
			}
		}
	}
}
