using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Xml;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ReserveLevelsClass : IReserveLevels, IDependency
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		private const string MSG001 = "Error newing ConsolidatedDAClass";
		private const string MSG002 = "Security is null";
		private const string MSG003 = "Reserve Level is null";
		private const string MSG005 = "Reserve Level has no ID";
		private const string MSG006 = "Reserve Level Exists";
		private const string MSG007 = "Reserve Level Not Found";
		private const string MSG008 = "IDependency Object is null";
		private const string MSG009 = "Reserve Level record exist, cannot delete";
		private const string MSG010 = "Could not update associated transactions";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Reserve Levels Class.
		/// </summary>
		public ReserveLevelsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public Methods
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ReserveLevelClass reserveLevel)
		{
			// Validate the security and reserve level objects
			if (security == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG002);
			}
			if (reserveLevel == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG003);
			}

			// Throw an exception of the same record exists.
			if (this.GetIdentityGuid(security, reserveLevel.ProductID) != Guid.Empty)
			{
				throw (new Exception(ReserveLevelsClass.MSG006));
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				reserveLevel.IdentityGuid = Guid.NewGuid();
				reserveLevel.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			return reserveLevel.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ReserveLevelClass reserveLevel)
		{
			// Ensure the security and reserve level objects are valid.
			if (security == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG002);
			}
			if (reserveLevel == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG003);
			}

			// Ensure that reserve level object does not update
			// another reserve level record.
			Guid identityGuid = GetIdentityGuid(security, reserveLevel.ProductID);
			if ((identityGuid != Guid.Empty) && (identityGuid != reserveLevel.IdentityGuid))
			{
				throw (new Exception(ReserveLevelsClass.MSG006));
			}

			// Ensure that the existing reserve level exists.
			ReserveLevelClass oldClass = this.GetByIdentityGuid(security, reserveLevel.IdentityGuid);
			if (oldClass.IdentityGuid == Guid.Empty)
			{
				throw (new Exception(ReserveLevelsClass.MSG007));
			}

			reserveLevel.UpdatedDate = DateTimeOffset.Now;
			reserveLevel.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				reserveLevel.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		/// <summary>
		/// This method will return the object's identityGuid given an ID. It will return
		/// Guid.empty if not found.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="productID"></param>
		/// <returns></returns>
		public Guid GetIdentityGuid(SecurityClass security, string productID)
		{
			if (security == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG002);
			}

			ReserveLevelClass reserveLevel = new ReserveLevelClass();

			reserveLevel.ProductID = productID;
			reserveLevel.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				reserveLevel.SelectByProductIDSQL(cmd, ContextUtil.IsInTransaction);
				reserveLevel.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			if (reserveLevel != null)
			{
				return reserveLevel.IdentityGuid;
			}

			return Guid.Empty;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid reserveLevelGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			ReserveLevelClass reserveLevel = this.GetByIdentityGuid(security, reserveLevelGuid);
			if (reserveLevel.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				reserveLevel.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method will return a list of reserve level objects.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public ReserveLevelCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG002);
			}

			ReserveLevelClass reserveLevel = new ReserveLevelClass();

			DataSet dataSet;
			using (SqlCommand cmd = new SqlCommand())
			{
				reserveLevel.EnumerateSQL(cmd, security);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			ReserveLevelCollectionClass reserveLevelCollection = new ReserveLevelCollectionClass();

			DataTable table = dataSet.Tables[0];
			while (table.Rows.Count != 0)
			{
				reserveLevel = new ReserveLevelClass();
				reserveLevel.Load(dataSet);
				reserveLevelCollection.Add(reserveLevel);
				table.Rows.RemoveAt(0);
			}

			return reserveLevelCollection;
		}

		/// <summary>
		/// This method will return the reserve level class given the security object
		/// and identity Guid.
		/// </summary>
		/// <param name="Security"></param>
		/// <param name="identityGuid"></param>
		/// <returns></returns>
		public ReserveLevelClass GetByIdentityGuid(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG002);
			}

			ReserveLevelClass reserveLevel = new ReserveLevelClass();
			reserveLevel.IdentityGuid = identityGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				reserveLevel.SelectSQL(cmd, ContextUtil.IsInTransaction);
				reserveLevel.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return reserveLevel;
		}

		/// <summary>
		/// This method will return the reserve level class given the security object
		/// and IdentityGuid.
		/// </summary>
		/// <param name="Security"></param>
		/// <param name="productID"></param>
		/// <returns></returns>
		public ReserveLevelClass GetByProductID(SecurityClass security, string productID)
		{
			if (security == null)
			{
				throw new ArgumentNullException(ReserveLevelsClass.MSG002);
			}

			ReserveLevelClass reserveLevel = new ReserveLevelClass();
			reserveLevel.ProductID = productID;
			reserveLevel.SiteGuid = security.SiteGuid;
	
			using (SqlCommand cmd = new SqlCommand())
			{
				reserveLevel.SelectByProductIDSQL(cmd, ContextUtil.IsInTransaction);
				reserveLevel.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return reserveLevel;
		}
		#endregion

		#region Dependency methods
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

		}

		/// <summary>
		/// This method is a place holder for validaing the Reserve Level Class.
		/// </summary>
		/// <param name="reserveLevel"></param>
		private void Validate(ReserveLevelClass reserveLevel)
		{
		}
		#endregion
	}
}