///***************************************************************************
/// Module Name:	MenuFavorites
/// Author:			Andy Hush
/// Copyright (c) Varec, Inc. All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using System.Data.SqlClient;
using System.Data;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Service class to allow addition, updating, enumeration, and
	/// removal of MenuFavoriteClass objects from tblMenuFavorites
	/// </summary>
	public class MenuFavorites : IMenuFavorites
	{
		#region Internal Fields

		/// <summary>
		/// For database access
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public MenuFavorites()
		{
		}

		/// <summary>
		/// Add a menu favorite to the database
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavorite">object to add</param>
		/// <returns>The GUID PK of the new DB record</returns>
		public Guid Add(SecurityClass security, MenuFavoriteClass menuFavorite)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (menuFavorite == null)
				throw new ArgumentNullException("menuFavorite");

			menuFavorite.SiteGuid = security.SiteGuid;
			menuFavorite.CreatedDate = DateTimeOffset.Now;
			menuFavorite.CreatedBy = security.UserID;
			menuFavorite.UpdatedDate = menuFavorite.CreatedDate;
			menuFavorite.UpdatedBy = security.UserID;
			menuFavorite.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				menuFavorite.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return menuFavorite.IdentityGuid;
		}

		/// <summary>
		/// Retrieve a collection of menu favorites by user
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="userGuid">PK of tblUsers</param>
		/// <returns>Collection of menu favorites</returns>
		public MenuFavoriteCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (userGuid == Guid.Empty)
				throw new ArgumentNullException("userGuid");

			var menuFavorite = new MenuFavoriteClass();
			menuFavorite.UserGuid = userGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				menuFavorite.EnumerateByUserSQL(cmd, ContextUtil.IsInTransaction);

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				var menuFavoriteCollection = new MenuFavoriteCollectionClass();

				DataTable table = set.Tables[0];
				foreach (DataRow row in table.Rows)
				{
					menuFavorite = new MenuFavoriteClass();
					menuFavorite.Load(row);
					menuFavoriteCollection.Add(menuFavorite);
				}

				return menuFavoriteCollection;
			}
		}

		/// <summary>
		/// Retrieve a collection of menu favorites by user, and whether it's a quick link or not
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="userGuid">PK of tblUsers</param>
		/// <param name="isQuickLink">To retrieve quick links or favorites</param>
		/// <returns>Collection of menu favorites</returns>
		public MenuFavoriteCollectionClass EnumerateByUserAndIsQuickLink(SecurityClass security, Guid userGuid, bool isQuickLink)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (userGuid == Guid.Empty)
				throw new ArgumentNullException("userGuid");

			var menuFavorite = new MenuFavoriteClass();
			menuFavorite.UserGuid = userGuid;
			menuFavorite.IsQuickLink = isQuickLink;
			using (SqlCommand cmd = new SqlCommand())
			{
				menuFavorite.EnumerateByUserAndIsQuickLinkSQL(cmd, ContextUtil.IsInTransaction);

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				var menuFavoriteCollection = new MenuFavoriteCollectionClass();

				DataTable table = set.Tables[0];
				foreach (DataRow row in table.Rows)
				{
					menuFavorite = new MenuFavoriteClass();
					menuFavorite.Load(row);
					menuFavoriteCollection.Add(menuFavorite);
				}

				return menuFavoriteCollection;
			}
		}

		/// <summary>
		/// Get a menu favorite by PK
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavoriteGuid">PK of tblMenuFavorites</param>
		/// <returns>Menu favorite object</returns>
		public MenuFavoriteClass Get(SecurityClass security, Guid menuFavoriteGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (menuFavoriteGuid == Guid.Empty)
				throw new ArgumentNullException("menuFavoriteGuid");

			var menuFavorite = new MenuFavoriteClass();
			menuFavorite.IdentityGuid = menuFavoriteGuid;
			using (SqlCommand cmd = new SqlCommand())
			{
				menuFavorite.SelectSQL(cmd, ContextUtil.IsInTransaction);
				menuFavorite.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return menuFavorite;
		}

		/// <summary>
		/// Modify a menu favorite in the database
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavorite">Menu favorite object</param>
		public void Modify(SecurityClass security, MenuFavoriteClass menuFavorite)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (menuFavorite == null)
				throw new ArgumentNullException("menuFavorite");

			MenuFavoriteClass oldMenuFavorite = Get(security, menuFavorite.IdentityGuid);
			if (oldMenuFavorite.IdentityGuid == Guid.Empty)
				throw new Exception("MenuFavorite Not Found");

			menuFavorite.UpdatedDate = DateTimeOffset.Now;
			menuFavorite.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				menuFavorite.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Purge menu favorite from the database
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavoriteGuid">PK of tblMenuFavorites</param>
		public void Purge(SecurityClass security, Guid menuFavoriteGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (menuFavoriteGuid == Guid.Empty)
				throw new ArgumentNullException("menuFavoriteGuid");

			MenuFavoriteClass menuFavorite = Get(security, menuFavoriteGuid);
			if (menuFavorite.IdentityGuid == Guid.Empty)
				throw new Exception("MenuFavorite Not Found");

			using (SqlCommand cmd = new SqlCommand())
			{
				menuFavorite.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Purge Users menu favorites from the database
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="userGuid">PK of tblMenuFavorites</param>
		public void PurgeByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid == Guid.Empty)
			{
				throw new ArgumentNullException("userGuid");
			}

			var menuFavorite = new MenuFavoriteClass();
			menuFavorite.UserGuid = userGuid;

			using (var cmd = new SqlCommand())
			{
				menuFavorite.PurgeByUserSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}