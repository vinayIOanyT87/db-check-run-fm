///***************************************************************************
/// Module Name:	IMenuFavorites
/// Author:			Andy Hush
/// Copyright (c) Varec, Inc. All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	/// <summary>
	/// An interface for access to the MenuFavorites service class
	/// </summary>
	[ServiceContract]
	public interface IMenuFavorites
	{
		/// <summary>
		/// Add a menu favorite to the database
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavorite">object to add</param>
		/// <returns>The GUID PK of the new DB record</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, MenuFavoriteClass menuFavorite);

		/// <summary>
		/// Retrieve a collection of menu favorites by user
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="userGuid">PK of tblUsers</param>
		/// <returns>Collection of menu favorites</returns>
		[OperationContract]
		MenuFavoriteCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid);

		/// <summary>
		/// Retrieve a collection of menu favorites by user, and whether it's a quick link or not
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="userGuid">PK of tblUsers</param>
		/// <param name="isQuickLink">To retrieve quick links or favorites</param>
		/// <returns>Collection of menu favorites</returns>
		[OperationContract]
		MenuFavoriteCollectionClass EnumerateByUserAndIsQuickLink(SecurityClass security, Guid userGuid, bool isQuickLink);

		/// <summary>
		/// Get a menu favorite by PK
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavoriteGuid">PK of tblMenuFavorites</param>
		/// <returns>Menu favorite object</returns>
		[OperationContract]
		MenuFavoriteClass Get(SecurityClass security, Guid menuFavoriteGuid);

		/// <summary>
		/// Modify a menu favorite in the database
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavorite">Menu favorite object</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, MenuFavoriteClass menuFavorite);

		/// <summary>
		/// Purge menu favorite from the database
		/// </summary>
		/// <param name="security">Security object</param>
		/// <param name="menuFavoriteGuid">PK of tblMenuFavorites</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid menuFavoriteGuid);
	}
}
