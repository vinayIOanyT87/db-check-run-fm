// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityTypes.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the EntityTypes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;

namespace Nspa.Exchange
{

	public enum EntityTypes
	{
		Product,
		User,
		CompanyRoleMap,
		Company,
		Site,
		Equipment,
		EquipmentType,
		EquipmentTypeClass,
		TransactionAlias,
		UserDataFieldTransactionAlias,
		UserDataListValueTransactionAlias,
		ProductToTransactionAliasExclusion,
		Personnel,
		IATA,
		FuelCard,
        FuelCardLimit,
		Gate,
		ApplicationString,
		Meter,
		Tank,
		DocumentNumber,
		CountOrUnknown,
	}

	/// <summary>
	/// 
	/// </summary>
	public static class EntityTypesExtension
	{
		public static char Separator = ';';

		/// <summary>
		/// Returns the table name for the given entity.
		/// Names the specified type.
		/// </summary>		/// <returns></returns>
		
		public static string Name (this EntityTypes type)
		{
			return type.ToString();
		}
	
		/// <summary>
		/// Returns the table name for the given entity.
		/// </summary>
		/// <param name="entityType">Type of the entity.</param>
		/// <returns></returns>	
		public static string TableName (this EntityTypes entityType)
		{
			var tableName = string.Empty;
			switch (entityType)
			{
				case EntityTypes.ApplicationString:
					tableName = "tblApplicationString";
					break;
				case EntityTypes.Company:
					tableName = "tblCompanies";
					break;
				case EntityTypes.CompanyRoleMap:
					tableName = "tblCompanyToRole";
					break;
				case EntityTypes.DocumentNumber:
					tableName = "tblDocumentNumbers";
					break;
				case EntityTypes.Equipment:
					tableName = "tblEquipment";
					break;
				case EntityTypes.EquipmentType:
					tableName = "tblEquipmentTypes";
					break;
				case EntityTypes.EquipmentTypeClass:
					tableName = "tblEquipmentTypeClass";
					break;
				case EntityTypes.FuelCard:
					tableName = "tblFuelCards";
					break;
                case EntityTypes.FuelCardLimit:
                    tableName = "tblFuelCardLimits";
                    break;
                case EntityTypes.Gate:
					tableName = "tblGates";
					break;
				case EntityTypes.IATA:
					tableName = "tblIATA";
					break;
				case EntityTypes.Meter:
					tableName = "tblMeter";
					break;
				case EntityTypes.Personnel:
					tableName = "tblPersonnel";
					break;
				case EntityTypes.Product:
					tableName = "tblProducts";
					break;
				case EntityTypes.ProductToTransactionAliasExclusion:
					tableName = "tblProductToTransactionAliasExclusion";
					break;
				case EntityTypes.Site:
					tableName = "tblSites";
					break;
				case EntityTypes.Tank:
					tableName = "tblTanks";
					break;
				case EntityTypes.TransactionAlias:
					tableName = "tblTransactionAliases";
					break;
				case EntityTypes.User:
					tableName = "tblUsers";
					break;
				case EntityTypes.UserDataFieldTransactionAlias:
					tableName = "tblUserDataFieldTransactionAlias";
					break;
				case EntityTypes.UserDataListValueTransactionAlias:
					tableName = "tblUserDataListValueTransactionAlias";
					break;

			}
			return tableName;
		}
		
	}
}
