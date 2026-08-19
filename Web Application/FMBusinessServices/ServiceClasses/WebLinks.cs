namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Summary description for WebLinks.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class WebLinks : IWebLinks, IDependency
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Web Links service.
		/// </summary>
		public WebLinks()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		/// <summary>
		/// This method will enumerate all the records that start with WebLink in the 
		/// configuration setting key.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a collection of web link objects.</returns>
		public WebLinkCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var webLinkCollection = new WebLinkCollectionClass();

			using(var command = new SqlCommand())
			{
				var webLink = new WebLink();
				webLink.EnumerateSQL(command, ContextUtil.IsInTransaction);

				DataSet dataSet = this.consolidatedDa.GetDataSet(command, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
						
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						webLink = new WebLink();
						webLink.Load(row);

						webLinkCollection.Add(webLink);
					}
				}
			}

			return webLinkCollection;
		}

		/// <summary>
		/// This method will insert a new web link record into the
		/// configuration setting table.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="webLink">The web link object to insert.</param>
		/// <returns>Returns the newly insert GUID.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, WebLink webLink)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (webLink == null)
			{
				throw new ArgumentNullException("webLink");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_WEB_LINKS))
			{
				throw (new Exception("Access Denied"));
			}

			if (webLink.IdentityGuid != Guid.Empty)
			{
				throw (new Exception("WebLink Exists with Link Name: " + webLink.LinkName));
			}

			if (string.IsNullOrEmpty(webLink.LinkName))
			{
				throw (new Exception("Link Name required!"));
			}

			WebLink originalWebLink = this.GetByKey(security, webLink);

			if (originalWebLink != null && originalWebLink.IdentityGuid != Guid.Empty)
			{
				throw (new Exception("WebLink Exists with Link Name: " + webLink.LinkName));
			}

			using(var command = new SqlCommand())
			{
				webLink.CreatedBy	= security.UserID;
				webLink.CreatedDate = DateTimeOffset.UtcNow;
				webLink.UpdatedBy	= security.UserID;
				webLink.UpdatedDate = DateTimeOffset.UtcNow;

				webLink.InsertSQL(command);
				this.consolidatedDa.ExecuteQuery(security, command);
			}

			return webLink.IdentityGuid;
		}

		/// <summary>
		/// This method will modify an existing web link record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="webLink">The web link object to update.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, WebLink webLink)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (webLink == null)
			{
				throw new ArgumentNullException("webLink");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_WEB_LINKS))
			{
				throw (new Exception("Access Denied"));
			}

			if (string.IsNullOrEmpty(webLink.LinkName))
			{
				throw (new Exception("Link Name required!"));
			}

			WebLink originalWebLink = this.GetPreviousSetting(security, webLink);

			if (originalWebLink == null || originalWebLink.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("WebLink not found"));
			}

			// Since the name value is used as part of the setting key value, check
			// to see if it changed.  If it did not, then update the record. If it
			// did, then purge the old record and create a new one.
			if (originalWebLink.LinkName == webLink.LinkName)
			{
				using (var command = new SqlCommand())
				{
					webLink.UpdatedBy = security.UserID;
					webLink.UpdatedDate = DateTimeOffset.UtcNow;

					webLink.ModifySQL(command);
					this.consolidatedDa.ExecuteQuery(security, command);
				}
			}
			else
			{
				// Delete the existing record.
				this.Purge(security, webLink.IdentityGuid);

				// Create a new record with the link name
				// as part of the setting key.
				webLink.IdentityGuid = Guid.Empty;
				this.Add(security, webLink);
			}
		}

		/// <summary>
		/// This method will purge a web link configuration setting.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="webLinkGuid"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid webLinkGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.CONFIGURE_WEB_LINKS))
			{
				throw (new Exception("Access Denied"));
			}

			using(var command = new SqlCommand())
			{
				var webLink = new WebLink { IdentityGuid = webLinkGuid };
				webLink.PurgeSQL(command);

				this.consolidatedDa.ExecuteQuery(security, command);
			}
		}

		/// <summary>
		/// This method will retrieve the web link record based on the 
		/// web link GUID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="webLinkGuid">The web link GUID.</param>
		/// <returns>Returns the web link object or a null.</returns>
		public WebLink Get(SecurityClass security, Guid webLinkGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using(var command = new SqlCommand())
			{
				var localWebLink = new WebLink { IdentityGuid = webLinkGuid };
				localWebLink.GetByGuid(command, ContextUtil.IsInTransaction);

				DataSet dataSet = this.consolidatedDa.GetDataSet(command, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];

					var webLink = new WebLink();
					webLink.Load(row);

					return webLink;
				}
			}

			return null;
		}

		/// <summary>
		/// This method will retrieve the web link by a given name.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="linkName">The link name to search.</param>
		/// <returns>Returns the web link object or null.</returns>
		public WebLink GetByName(SecurityClass security, string linkName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var localWebLink = new WebLink { LinkName = linkName };

			using(var command = new SqlCommand())
			{
				localWebLink.SelectByNameSQL(command, ContextUtil.IsInTransaction);
				DataSet dataSet = this.consolidatedDa.GetDataSet(command, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];
					var webLink = new WebLink();
					webLink.Load(row);

					return webLink;
				}
			}

			return null;
		}

		#region Private methods
		/// <summary>
		/// This method will retrieve the configuration setting record by the setting
		/// key.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="webLink">The web link object.</param>
		/// <returns>Returns either null or the configuration setting object.</returns>
		private WebLink GetByKey(SecurityClass security, WebLink webLink)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var command = new SqlCommand())
			{
				webLink.GetByKey(command, ContextUtil.IsInTransaction);
				DataSet dataSet = this.consolidatedDa.GetDataSet(command, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];
					var originalWebLink = new WebLink();
					originalWebLink.Load(row);

					return originalWebLink;
				}
			}

			return null;
		}

		/// <summary>
		/// This method will retrieve the previous configuration setting
		/// object from the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="webLink"></param>
		/// <returns></returns>
		private WebLink GetPreviousSetting(SecurityClass security, WebLink webLink)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (webLink == null || webLink.IdentityGuid == Guid.Empty)
			{
				return null;
			}

			using (var command = new SqlCommand())
			{
				webLink.GetByGuid(command, ContextUtil.IsInTransaction);
				DataSet dataSet = this.consolidatedDa.GetDataSet(command, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];
					var originalWebLink = new WebLink();
					originalWebLink.Load(row);

					return originalWebLink;
				}
			}

			return null;
		}
		#endregion

		#region IDependency methods
		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
		}
		#endregion
	}
}