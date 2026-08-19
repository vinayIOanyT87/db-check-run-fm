using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MessagesClass : IDependency, IMessages
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public MessagesClass()
		{
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, MessageClass Message)
		{


			if (security == null)
				throw new ArgumentNullException("Security");

			if (Message == null)
				throw new ArgumentNullException("Message");

			if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			if (GetIdentityGuid(security, Message.ID, Message.CompanyGuid, Message.PersonnelGuid) != Guid.Empty)
				throw (new Exception("Message Exists"));

			Message.SiteGuid = security.SiteGuid;
			Message.CreatedDate = DateTimeOffset.Now;
			Message.CreatedBy = security.UserID;
			Message.UpdatedDate = Message.CreatedDate;
			Message.UpdatedBy = security.UserID;
		
			using (SqlCommand cmd = new SqlCommand())
			{
				Message.IdentityGuid = Guid.NewGuid();
				Message.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return Message.IdentityGuid;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, MessageClass Message)
		{


			if (security == null)
				throw new ArgumentNullException("Security");

			if (Message == null)
				throw new ArgumentNullException("Message");

			if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			Guid identityGuid = GetIdentityGuid(security, Message.ID, Message.CompanyGuid, Message.PersonnelGuid);
			if (identityGuid != Guid.Empty
			&& identityGuid != Message.IdentityGuid)
				throw (new Exception("Message Exists"));

			MessageClass OldMessage = Get(security, Message.IdentityGuid);
			if (Message.IdentityGuid == Guid.Empty)
				throw (new Exception("Message Not Found"));

			MessageLogsClass MessageLogs = new MessageLogsClass();
			MessageLogs.Purge(security, Message.IdentityGuid);

			Message.UpdatedDate = DateTimeOffset.Now;
			Message.UpdatedBy = security.UserID;
		
			using (SqlCommand cmd = new SqlCommand())
			{
				Message.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}


		public MessageClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessageClass Message = new MessageClass();
			Message.IdentityGuid = identityGuid;
            Message.SiteGuid = security.SiteGuid;
		
			using (SqlCommand cmd = new SqlCommand())
			{
				Message.SelectSQL(cmd, ContextUtil.IsInTransaction);
				Message.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return Message;
		}


		public Guid GetIdentityGuid(SecurityClass security, string ID, Guid companyGuid, Guid personnelGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessageClass Message = new MessageClass();
			Message.SiteGuid = security.SiteGuid;
			Message.ID = ID;
			Message.CompanyGuid = companyGuid;
			Message.PersonnelGuid = personnelGuid;
			
			using (SqlCommand cmd = new SqlCommand())
			{
				Message.SelectByIDAndGuidsSQL(cmd, ContextUtil.IsInTransaction);
				Message.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return Message.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid messageGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			MessageClass Message = Get(security, messageGuid);
			if (Message.IdentityGuid == Guid.Empty)
				throw (new Exception("Message Not Found"));

			MessageLogsClass MessageLogs = new MessageLogsClass();
			MessageLogs.Purge(security, messageGuid);

			using (SqlCommand cmd = new SqlCommand())
			{
				Message.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public MessageCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			MessageClass Message = new MessageClass();
			
			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				Message.EnumerateSQL(cmd, security);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			MessageCollectionClass MessageCollection = new MessageCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				Message = new MessageClass();
				Message.Load(Set);
				MessageCollection.Add(Message);
				Table.Rows.RemoveAt(0);
			}

			return MessageCollection;
		}

		public MessageCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessageClass Message = new MessageClass();
			Message.CompanyGuid = companyGuid;

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				Message.EnumerateByCompanySQL(cmd, security);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			MessageCollectionClass MessageCollection = new MessageCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				Message = new MessageClass();
				Message.Load(Set);
				MessageCollection.Add(Message);
				Table.Rows.RemoveAt(0);
			}

			return MessageCollection;
		}

		public MessageCollectionClass EnumerateByGuids(SecurityClass security, Guid companyGuid, Guid personnelGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessageClass Message = new MessageClass();
			Message.CompanyGuid = companyGuid;
			Message.PersonnelGuid = personnelGuid;
		
			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				Message.EnumerateByGuidsSQL(cmd, security);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			
			MessageCollectionClass MessageCollection = new MessageCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				Message = new MessageClass();
				Message.Load(Set);
				MessageCollection.Add(Message);
				Table.Rows.RemoveAt(0);
			}

			return MessageCollection;
		}

		#region IDependency methods
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			// Purge Messages
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				MessageCollectionClass MessageCollection = Enumerate(security);
				foreach (MessageClass Message in MessageCollection)
					Purge(security, Message.IdentityGuid);
			}
		}
		#endregion IDependecy methods
	}
}
