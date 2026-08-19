using System;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AlarmPriorityEmailGroupMapsClass : IAlarmPriorityEmailGroupMaps
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public AlarmPriorityEmailGroupMapsClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Add( SecurityClass security, AlarmPriorityEmailGroupMapClass alarmPriorityEmailGroupMap )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (alarmPriorityEmailGroupMap == null)
			{
				throw new ArgumentNullException ( "alarmPriorityEmailGroupMap" );
			}

			if (!security.HasRight ( RIGHT.MODIFY_SITES_AND_SITE_GROUPS ))
			{
				throw new FMInsufficientRightsException();
			}

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				alarmPriorityEmailGroupMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables[0].Rows.Count != 0)
			{
				return;
			}

			alarmPriorityEmailGroupMap.SiteGuid = security.SiteGuid;
			alarmPriorityEmailGroupMap.CreatedDate	= DateTimeOffset.Now;
			alarmPriorityEmailGroupMap.CreatedBy	= security.UserID;
			alarmPriorityEmailGroupMap.UpdatedDate	= alarmPriorityEmailGroupMap.CreatedDate;
			alarmPriorityEmailGroupMap.UpdatedBy	= security.UserID;
			alarmPriorityEmailGroupMap.Deleted		= false;

			using (var cmd = new SqlCommand())
			{
				alarmPriorityEmailGroupMap.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge(SecurityClass security, string id, Guid emailGroupGuid, Guid alarmPriorityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}

			if (!security.HasRight ( RIGHT.MODIFY_SITES_AND_SITE_GROUPS ))
			{
				throw new FMInsufficientRightsException();
			}

			AlarmPriorityEmailGroupMapClass alarmPriorityEmailGroupMap = new AlarmPriorityEmailGroupMapClass ( );
			alarmPriorityEmailGroupMap.ID					= id;
			alarmPriorityEmailGroupMap.EmailGroupGuid = emailGroupGuid;
			alarmPriorityEmailGroupMap.AlarmPriorityGuid = alarmPriorityGuid;

			using (var cmd = new SqlCommand())
			{
				alarmPriorityEmailGroupMap.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}