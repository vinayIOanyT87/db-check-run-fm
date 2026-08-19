
--------------------------------------------------------------------------------------------------------------------
-- This file is generated.  Don't update this directly unless you know what you're doing.  Thank.
-- (generated at 5/19/2012 8:36:56 PM)
--------------------------------------------------------------------------------------------------------------------

-- The following are all the tables involved: 
--[dbo].[tblAutoDistributionRule]
--[map].[tblEntityAutoDistributionRuleToSite]
--[map].[tblManagerGroupToAutoDistributionRule]
--[map].[tblManagerToAutoDistributionRule]
--[map].[tblOwnerGroupToAutoDistributionRule]
--[map].[tblOwnerToAutoDistributionRule]
--[map].[tblProductGroupToAutoDistributionRule]
--[map].[tblProductToAutoDistributionRule]
--[map].[tblTransactionAliasToAutoDistributionRule]

-- The following are all the objects created(They may be just prefixes, like *DeleteBy*): 
--[dbo].[tblAutoDistributionRule]
--[dbo].[usp_AutoDistributionRuleInsertByRowGuid]
--[dbo].[usp_AutoDistributionRuleDeleteBy]
--[dbo].[usp_AutoDistributionRuleUpdateByRowGuid]
--[dbo].[usp_AutoDistributionRuleSelect]
--[map].[tblEntityAutoDistributionRuleToSite]
--[map].[usp_AutoDistributionRuleToSiteInsertByRowGuid]
--[map].[usp_AutoDistributionRuleToSiteDeleteBy]
--[map].[usp_AutoDistributionRuleToSiteUpdateByRowGuid]
--[map].[usp_AutoDistributionRuleToSiteSelect]
--[map].[tblManagerGroupToAutoDistributionRule]
--[map].[usp_ManagerGroupToAutoDistributionRuleInsertByRowGuid]
--[map].[usp_ManagerGroupToAutoDistributionRuleDeleteBy]
--[map].[usp_ManagerGroupToAutoDistributionRuleUpdateByRowGuid]
--[map].[usp_ManagerGroupToAutoDistributionRuleSelect]
--[map].[usp_ManagerGroupToAutoDistributionRuleSelectManagerGroup]
--[map].[tblManagerToAutoDistributionRule]
--[map].[usp_ManagerToAutoDistributionRuleInsertByRowGuid]
--[map].[usp_ManagerToAutoDistributionRuleDeleteBy]
--[map].[usp_ManagerToAutoDistributionRuleUpdateByRowGuid]
--[map].[usp_ManagerToAutoDistributionRuleSelect]
--[map].[usp_ManagerToAutoDistributionRuleSelectManager]
--[map].[tblOwnerGroupToAutoDistributionRule]
--[map].[usp_OwnerGroupToAutoDistributionRuleInsertByRowGuid]
--[map].[usp_OwnerGroupToAutoDistributionRuleDeleteBy]
--[map].[usp_OwnerGroupToAutoDistributionRuleUpdateByRowGuid]
--[map].[usp_OwnerGroupToAutoDistributionRuleSelect]
--[map].[usp_OwnerGroupToAutoDistributionRuleSelectOwnerGroup]
--[map].[tblOwnerToAutoDistributionRule]
--[map].[usp_OwnerToAutoDistributionRuleInsertByRowGuid]
--[map].[usp_OwnerToAutoDistributionRuleDeleteBy]
--[map].[usp_OwnerToAutoDistributionRuleUpdateByRowGuid]
--[map].[usp_OwnerToAutoDistributionRuleSelect]
--[map].[usp_OwnerToAutoDistributionRuleSelectOwner]
--[map].[tblProductGroupToAutoDistributionRule]
--[map].[usp_ProductGroupToAutoDistributionRuleInsertByRowGuid]
--[map].[usp_ProductGroupToAutoDistributionRuleDeleteBy]
--[map].[usp_ProductGroupToAutoDistributionRuleUpdateByRowGuid]
--[map].[usp_ProductGroupToAutoDistributionRuleSelect]
--[map].[usp_ProductGroupToAutoDistributionRuleSelectProductGroup]
--[map].[tblProductToAutoDistributionRule]
--[map].[usp_ProductToAutoDistributionRuleInsertByRowGuid]
--[map].[usp_ProductToAutoDistributionRuleDeleteBy]
--[map].[usp_ProductToAutoDistributionRuleUpdateByRowGuid]
--[map].[usp_ProductToAutoDistributionRuleSelect]
--[map].[usp_ProductToAutoDistributionRuleSelectProduct]
--[map].[tblTransactionAliasToAutoDistributionRule]
--[map].[usp_TransactionAliasToAutoDistributionRuleInsertByRowGuid]
--[map].[usp_TransactionAliasToAutoDistributionRuleDeleteBy]
--[map].[usp_TransactionAliasToAutoDistributionRuleUpdateByRowGuid]
--[map].[usp_TransactionAliasToAutoDistributionRuleSelect]
--[map].[usp_TransactionAliasToAutoDistributionRuleSelectTransactionAlias]
--[dbo].[usp_AutoDistributionRuleDeleteApplication]

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleDeleteApplication] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleDeleteApplication]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteApplication]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_TransactionAliasToAutoDistributionRuleSelectTransactionAlias] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_TransactionAliasToAutoDistributionRuleSelectTransactionAlias]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleSelectTransactionAlias]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_TransactionAliasToAutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_TransactionAliasToAutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_TransactionAliasToAutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_TransactionAliasToAutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_TransactionAliasToAutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_TransactionAliasToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByTransactionAliasGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_TransactionAliasToAutoDistributionRuleDeleteByTransactionAliasGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByTransactionAliasGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_TransactionAliasToAutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_TransactionAliasToAutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblTransactionAliasToAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblTransactionAliasToAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblTransactionAliasToAutoDistributionRule]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductToAutoDistributionRuleSelectProduct] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductToAutoDistributionRuleSelectProduct]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductToAutoDistributionRuleSelectProduct]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductToAutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductToAutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductToAutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductToAutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductToAutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductToAutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductToAutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductToAutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductToAutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductToAutoDistributionRuleDeleteByProductGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductToAutoDistributionRuleDeleteByProductGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductToAutoDistributionRuleDeleteByProductGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductToAutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductToAutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductToAutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblProductToAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblProductToAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblProductToAutoDistributionRule]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductGroupToAutoDistributionRuleSelectProductGroup] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductGroupToAutoDistributionRuleSelectProductGroup]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleSelectProductGroup]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductGroupToAutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductGroupToAutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductGroupToAutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductGroupToAutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductGroupToAutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductGroupToAutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductGroupToAutoDistributionRuleDeleteByProductGroupGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductGroupToAutoDistributionRuleDeleteByProductGroupGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleDeleteByProductGroupGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ProductGroupToAutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ProductGroupToAutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblProductGroupToAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblProductGroupToAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblProductGroupToAutoDistributionRule]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerToAutoDistributionRuleSelectOwner] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerToAutoDistributionRuleSelectOwner]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerToAutoDistributionRuleSelectOwner]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerToAutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerToAutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerToAutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerToAutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerToAutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerToAutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerToAutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerToAutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerToAutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerToAutoDistributionRuleDeleteByOwnerGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerToAutoDistributionRuleDeleteByOwnerGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerToAutoDistributionRuleDeleteByOwnerGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerToAutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerToAutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerToAutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblOwnerToAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblOwnerToAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblOwnerToAutoDistributionRule]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerGroupToAutoDistributionRuleSelectOwnerGroup] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerGroupToAutoDistributionRuleSelectOwnerGroup]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleSelectOwnerGroup]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerGroupToAutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerGroupToAutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerGroupToAutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerGroupToAutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerGroupToAutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByOwnerGroupGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerGroupToAutoDistributionRuleDeleteByOwnerGroupGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByOwnerGroupGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_OwnerGroupToAutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_OwnerGroupToAutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblOwnerGroupToAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblOwnerGroupToAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblOwnerGroupToAutoDistributionRule]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerToAutoDistributionRuleSelectManager] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerToAutoDistributionRuleSelectManager]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerToAutoDistributionRuleSelectManager]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerToAutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerToAutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerToAutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerToAutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerToAutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerToAutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerToAutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerToAutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerToAutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerToAutoDistributionRuleDeleteByManagerGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerToAutoDistributionRuleDeleteByManagerGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerToAutoDistributionRuleDeleteByManagerGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerToAutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerToAutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerToAutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblManagerToAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblManagerToAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblManagerToAutoDistributionRule]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerGroupToAutoDistributionRuleSelectManagerGroup] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerGroupToAutoDistributionRuleSelectManagerGroup]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleSelectManagerGroup]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerGroupToAutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerGroupToAutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerGroupToAutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerGroupToAutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerGroupToAutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByManagerGroupGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerGroupToAutoDistributionRuleDeleteByManagerGroupGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByManagerGroupGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_ManagerGroupToAutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_ManagerGroupToAutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblManagerGroupToAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblManagerGroupToAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblManagerGroupToAutoDistributionRule]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_AutoDistributionRuleToSiteSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_AutoDistributionRuleToSiteSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_AutoDistributionRuleToSiteSelect]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_AutoDistributionRuleToSiteUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_AutoDistributionRuleToSiteUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_AutoDistributionRuleToSiteUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_AutoDistributionRuleToSiteDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_AutoDistributionRuleToSiteDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_AutoDistributionRuleToSiteDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_AutoDistributionRuleToSiteDeleteBySiteGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_AutoDistributionRuleToSiteDeleteBySiteGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_AutoDistributionRuleToSiteDeleteBySiteGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_AutoDistributionRuleToSiteDeleteByAutoDistributionRuleGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_AutoDistributionRuleToSiteDeleteByAutoDistributionRuleGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_AutoDistributionRuleToSiteDeleteByAutoDistributionRuleGuid]
GO	

-- ===================== Dropping PROCEDURE - [map].[usp_AutoDistributionRuleToSiteInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[usp_AutoDistributionRuleToSiteInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [map].[usp_AutoDistributionRuleToSiteInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [map].[tblEntityAutoDistributionRuleToSite] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[map].[tblEntityAutoDistributionRuleToSite]') 
			AND type in (N'U'))	
	DROP TABLE [map].[tblEntityAutoDistributionRuleToSite]
GO	

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleSelect] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleSelect]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleSelect]
GO	

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleUpdateByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleUpdateByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleUpdateByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleDeleteByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleDeleteByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteByRowGuid]
GO	

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleDeleteBySiteGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleDeleteBySiteGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteBySiteGuid]
GO	

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleDeleteByTransactionAliasGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleDeleteByTransactionAliasGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteByTransactionAliasGuid]
GO	

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleDeleteByDefaultReasonCodeGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleDeleteByDefaultReasonCodeGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteByDefaultReasonCodeGuid]
GO	

-- ===================== Dropping PROCEDURE - [dbo].[usp_AutoDistributionRuleInsertByRowGuid] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[usp_AutoDistributionRuleInsertByRowGuid]') 
			AND type in (N'P', N'PC'))	
	DROP PROCEDURE [dbo].[usp_AutoDistributionRuleInsertByRowGuid]
GO	

-- ===================== Dropping TABLE - [dbo].[tblAutoDistributionRule] ==========================================================
IF  EXISTS (SELECT * FROM sys.objects 
		WHERE object_id = OBJECT_ID(N'[dbo].[tblAutoDistributionRule]') 
			AND type in (N'U'))	
	DROP TABLE [dbo].[tblAutoDistributionRule]
GO	

-- ===================== Creating TABLE - [dbo].[tblAutoDistributionRule] ==========================================================
CREATE TABLE [dbo].[tblAutoDistributionRule] (
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_dbo_tblAutoDistributionRule_AutoDistributionRuleGuid] DEFAULT NEWID(),
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_dbo_tblAutoDistributionRule_SiteGuid]
		REFERENCES tblSites(SiteGuid),
	[RuleID] NVARCHAR(50) NOT NULL,
	[RuleDescription] NVARCHAR(255) NOT NULL,
	[RuleEnabled] BIT NOT NULL,
	[DefaultEOM] BIT NOT NULL,
	[TransactionAliasGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_dbo_tblAutoDistributionRule_TransactionAliasGuid]
		REFERENCES tblTransactionAliases(TransactionAliasGuid),
	[DefaultReasonCodeGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_dbo_tblAutoDistributionRule_DefaultReasonCodeGuid]
		REFERENCES tblAutoDistributionReasonCodes(AutoDistributionReasonCodeGuid),
	[DefaultNotes] NVARCHAR(1000) NOT NULL,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_dbo_tblAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_dbo_tblAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_dbo_tblAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_dbo_tblAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_dbo_tblAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[AutoDistributionRuleGuid] ASC
	),
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleInsertByRowGuid] (
	@SiteGuid UNIQUEIDENTIFIER,
	@RuleID NVARCHAR(50),
	@RuleDescription NVARCHAR(255),
	@RuleEnabled BIT,
	@DefaultEOM BIT,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@DefaultReasonCodeGuid UNIQUEIDENTIFIER,
	@DefaultNotes NVARCHAR(1000),
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [dbo].[tblAutoDistributionRule]
	( 
		AutoDistributionRuleGuid, SiteGuid, RuleID, 
		RuleDescription, RuleEnabled, DefaultEOM, TransactionAliasGuid, 
		DefaultReasonCodeGuid, DefaultNotes, CreatedDate, CreatedBy, 
		UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @SiteGuid, @RuleID, 
		@RuleDescription, @RuleEnabled, @DefaultEOM, @TransactionAliasGuid, 
		@DefaultReasonCodeGuid, @DefaultNotes, @CreatedDate, @CreatedBy, 
		@UpdatedDate, @UpdatedBy
	)
	SET @AutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteByRowGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [dbo].[tblAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteBySiteGuid]
	@SiteGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblAutoDistributionRule]
			WHERE [SiteGuid] = @SiteGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [dbo].[tblAutoDistributionRule] 
	WHERE [SiteGuid] = @SiteGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteByTransactionAliasGuid]
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblAutoDistributionRule]
			WHERE [TransactionAliasGuid] = @TransactionAliasGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [dbo].[tblAutoDistributionRule] 
	WHERE [TransactionAliasGuid] = @TransactionAliasGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteByDefaultReasonCodeGuid]
	@DefaultReasonCodeGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblAutoDistributionRule]
			WHERE [DefaultReasonCodeGuid] = @DefaultReasonCodeGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [dbo].[tblAutoDistributionRule] 
	WHERE [DefaultReasonCodeGuid] = @DefaultReasonCodeGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleUpdateByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@RuleID NVARCHAR(50),
	@RuleDescription NVARCHAR(255),
	@RuleEnabled BIT,
	@DefaultEOM BIT,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@DefaultReasonCodeGuid UNIQUEIDENTIFIER,
	@DefaultNotes NVARCHAR(1000),
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [dbo].[tblAutoDistributionRule]
	SET
		SiteGuid = @SiteGuid, RuleID = @RuleID, 
		RuleDescription = @RuleDescription, RuleEnabled = @RuleEnabled, DefaultEOM = @DefaultEOM, TransactionAliasGuid = @TransactionAliasGuid, 
		DefaultReasonCodeGuid = @DefaultReasonCodeGuid, DefaultNotes = @DefaultNotes, 
		UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@RuleID NVARCHAR(50) = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.AutoDistributionRuleGuid, MAIN.SiteGuid, MAIN.RuleID, 
		MAIN.RuleDescription, MAIN.RuleEnabled, MAIN.DefaultEOM, MAIN.TransactionAliasGuid, 
		MAIN.DefaultReasonCodeGuid, MAIN.DefaultNotes, MAIN.CreatedDate, MAIN.CreatedBy, 
		MAIN.UpdatedDate, MAIN.UpdatedBy, MAIN._RowVersion
	FROM 
		[dbo].[tblAutoDistributionRule] MAIN WITH (NOLOCK)
		
		INNER JOIN [map].[tblEntityAutoDistributionRuleToSite] MAP WITH (NOLOCK)
		ON MAIN.[AutoDistributionRuleGuid] = MAP.[AutoDistributionRuleGuid]
		
		/* The following are relevent only when Login and Current sites are different */
		LEFT JOIN [map].[tblEntityAutoDistributionRuleToSite] RMAPLOGIN WITH (NOLOCK)
		ON MAIN.[AutoDistributionRuleGuid]  = RMAPLOGIN.[AutoDistributionRuleGuid] 		
		AND @AreLoginCurrentSiteTheSame = 0
		AND RMAPLOGIN.SiteGuid = @LoginSiteGuid
	WHERE
		/* the site is assigned to the current site */
		MAP.SiteGuid = @SelectedSiteGuid
		AND
		( 
			@AreLoginCurrentSiteTheSame = 1			
			OR
			/* The following are relevent only when Login and Current sites are different */
			/* the site is owned by the current site*/
			MAIN.SiteGuid = @SelectedSiteGuid		
			OR RMAPLOGIN.[AutoDistributionRuleGuid] IS NOT NULL
		) AND 		
		((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@RuleID IS NULL) OR (@RuleID = MAIN.RuleID))
	ORDER BY
		MAIN.RuleID

END
GO

-- ===================== Creating TABLE - [map].[tblEntityAutoDistributionRuleToSite] ==========================================================
CREATE TABLE [map].[tblEntityAutoDistributionRuleToSite] (
	[AutoDistributionRuleToSiteGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblEntityAutoDistributionRuleToSite_AutoDistributionRuleToSiteGuid] DEFAULT NEWID(),
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblEntityAutoDistributionRuleToSite_SiteGuid]
		REFERENCES tblSites(SiteGuid),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblEntityAutoDistributionRuleToSite_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblEntityAutoDistributionRuleToSite_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblEntityAutoDistributionRuleToSite_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblEntityAutoDistributionRuleToSite_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblEntityAutoDistributionRuleToSite_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblEntityAutoDistributionRuleToSite] PRIMARY KEY CLUSTERED 
	(
		[AutoDistributionRuleToSiteGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblEntityAutoDistributionRuleToSite_SiteGuid_AutoDistributionRuleGuid]  UNIQUE
		([SiteGuid], [AutoDistributionRuleGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteInsertByRowGuid] (
	@SiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@AutoDistributionRuleToSiteGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblEntityAutoDistributionRuleToSite]
	( 
		AutoDistributionRuleToSiteGuid, SiteGuid, AutoDistributionRuleGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @SiteGuid, @AutoDistributionRuleGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @AutoDistributionRuleToSiteGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteDeleteByRowGuid]
	@AutoDistributionRuleToSiteGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblEntityAutoDistributionRuleToSite]
			WHERE [AutoDistributionRuleToSiteGuid] = @AutoDistributionRuleToSiteGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRuleToSite.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblEntityAutoDistributionRuleToSite] 
	WHERE [AutoDistributionRuleToSiteGuid] = @AutoDistributionRuleToSiteGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteDeleteBySiteGuid]
	@SiteGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblEntityAutoDistributionRuleToSite]
			WHERE [SiteGuid] = @SiteGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRuleToSite.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblEntityAutoDistributionRuleToSite] 
	WHERE [SiteGuid] = @SiteGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblEntityAutoDistributionRuleToSite]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRuleToSite.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblEntityAutoDistributionRuleToSite] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteUpdateByRowGuid] (
	@AutoDistributionRuleToSiteGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblEntityAutoDistributionRuleToSite]
	SET
		SiteGuid = @SiteGuid, AutoDistributionRuleGuid = @AutoDistributionRuleGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		AutoDistributionRuleToSiteGuid = @AutoDistributionRuleToSiteGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblEntityAutoDistributionRuleToSite]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_AutoDistributionRuleToSiteSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleToSiteGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.AutoDistributionRuleToSiteGuid, MAIN.SiteGuid, MAIN.AutoDistributionRuleGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblEntityAutoDistributionRuleToSite] MAIN WITH (NOLOCK)
	WHERE
		((@AutoDistributionRuleToSiteGuid IS NULL) OR (@AutoDistributionRuleToSiteGuid = MAIN.AutoDistributionRuleToSiteGuid))
		AND ((@SiteGuid IS NULL) OR (@SiteGuid = MAIN.SiteGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))

END
GO

-- ===================== Creating TABLE - [map].[tblManagerGroupToAutoDistributionRule] ==========================================================
CREATE TABLE [map].[tblManagerGroupToAutoDistributionRule] (
	[ManagerGroupToAutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblManagerGroupToAutoDistributionRule_ManagerGroupToAutoDistributionRuleGuid] DEFAULT NEWID(),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblManagerGroupToAutoDistributionRule_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid),
	[ManagerGroupGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblManagerGroupToAutoDistributionRule_ManagerGroupGuid]
		REFERENCES tblApplicationString(ApplicationStringGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblManagerGroupToAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblManagerGroupToAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblManagerGroupToAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblManagerGroupToAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblManagerGroupToAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[ManagerGroupToAutoDistributionRuleGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblManagerGroupToAutoDistributionRule_AutoDistributionRuleGuid_ManagerGroupGuid]  UNIQUE
		([AutoDistributionRuleGuid], [ManagerGroupGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ManagerGroupGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblManagerGroupToAutoDistributionRule]
	( 
		ManagerGroupToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ManagerGroupGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ManagerGroupGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ManagerGroupToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByRowGuid]
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblManagerGroupToAutoDistributionRule]
			WHERE [ManagerGroupToAutoDistributionRuleGuid] = @ManagerGroupToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ManagerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblManagerGroupToAutoDistributionRule] 
	WHERE [ManagerGroupToAutoDistributionRuleGuid] = @ManagerGroupToAutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblManagerGroupToAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ManagerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblManagerGroupToAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByManagerGroupGuid]
	@ManagerGroupGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblManagerGroupToAutoDistributionRule]
			WHERE [ManagerGroupGuid] = @ManagerGroupGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ManagerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblManagerGroupToAutoDistributionRule] 
	WHERE [ManagerGroupGuid] = @ManagerGroupGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleUpdateByRowGuid] (
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ManagerGroupGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblManagerGroupToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, ManagerGroupGuid = @ManagerGroupGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		ManagerGroupToAutoDistributionRuleGuid = @ManagerGroupToAutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ManagerGroupGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ManagerGroupToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ManagerGroupGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblManagerGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ManagerGroupToAutoDistributionRuleGuid IS NULL) OR (@ManagerGroupToAutoDistributionRuleGuid = MAIN.ManagerGroupToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ManagerGroupGuid IS NULL) OR (@ManagerGroupGuid = MAIN.ManagerGroupGuid))

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleSelectManagerGroup] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.ManagerGroupToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.ManagerGroupGuid,
		ASSIGNED.*
	FROM 
		[map].[tblManagerGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblApplicationString] ASSIGNED WITH (NOLOCK)
		ON MAIN.ManagerGroupGuid = ASSIGNED.ApplicationStringGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END
GO

-- ===================== Creating TABLE - [map].[tblManagerToAutoDistributionRule] ==========================================================
CREATE TABLE [map].[tblManagerToAutoDistributionRule] (
	[ManagerToAutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblManagerToAutoDistributionRule_ManagerToAutoDistributionRuleGuid] DEFAULT NEWID(),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblManagerToAutoDistributionRule_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid),
	[ManagerGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblManagerToAutoDistributionRule_ManagerGuid]
		REFERENCES tblCompanies(CompanyGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblManagerToAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblManagerToAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblManagerToAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblManagerToAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblManagerToAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[ManagerToAutoDistributionRuleGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblManagerToAutoDistributionRule_AutoDistributionRuleGuid_ManagerGuid]  UNIQUE
		([AutoDistributionRuleGuid], [ManagerGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ManagerGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@ManagerToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblManagerToAutoDistributionRule]
	( 
		ManagerToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ManagerGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ManagerGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ManagerToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleDeleteByRowGuid]
	@ManagerToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblManagerToAutoDistributionRule]
			WHERE [ManagerToAutoDistributionRuleGuid] = @ManagerToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ManagerToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblManagerToAutoDistributionRule] 
	WHERE [ManagerToAutoDistributionRuleGuid] = @ManagerToAutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblManagerToAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ManagerToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblManagerToAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleDeleteByManagerGuid]
	@ManagerGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblManagerToAutoDistributionRule]
			WHERE [ManagerGuid] = @ManagerGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ManagerToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblManagerToAutoDistributionRule] 
	WHERE [ManagerGuid] = @ManagerGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleUpdateByRowGuid] (
	@ManagerToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ManagerGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblManagerToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, ManagerGuid = @ManagerGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		ManagerToAutoDistributionRuleGuid = @ManagerToAutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ManagerToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ManagerGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ManagerToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ManagerGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblManagerToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ManagerToAutoDistributionRuleGuid IS NULL) OR (@ManagerToAutoDistributionRuleGuid = MAIN.ManagerToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ManagerGuid IS NULL) OR (@ManagerGuid = MAIN.ManagerGuid))

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblManagerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerToAutoDistributionRuleSelectManager] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.ManagerToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.ManagerGuid,
		ASSIGNED.*
	FROM 
		[map].[tblManagerToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblCompanies] ASSIGNED WITH (NOLOCK)
		ON MAIN.ManagerGuid = ASSIGNED.CompanyGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END
GO

-- ===================== Creating TABLE - [map].[tblOwnerGroupToAutoDistributionRule] ==========================================================
CREATE TABLE [map].[tblOwnerGroupToAutoDistributionRule] (
	[OwnerGroupToAutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblOwnerGroupToAutoDistributionRule_OwnerGroupToAutoDistributionRuleGuid] DEFAULT NEWID(),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblOwnerGroupToAutoDistributionRule_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid),
	[OwnerGroupGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblOwnerGroupToAutoDistributionRule_OwnerGroupGuid]
		REFERENCES tblApplicationString(ApplicationStringGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblOwnerGroupToAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblOwnerGroupToAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblOwnerGroupToAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblOwnerGroupToAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblOwnerGroupToAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[OwnerGroupToAutoDistributionRuleGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblOwnerGroupToAutoDistributionRule_AutoDistributionRuleGuid_OwnerGroupGuid]  UNIQUE
		([AutoDistributionRuleGuid], [OwnerGroupGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@OwnerGroupGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@OwnerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblOwnerGroupToAutoDistributionRule]
	( 
		OwnerGroupToAutoDistributionRuleGuid, AutoDistributionRuleGuid, OwnerGroupGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @OwnerGroupGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @OwnerGroupToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByRowGuid]
	@OwnerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerGroupToAutoDistributionRule]
			WHERE [OwnerGroupToAutoDistributionRuleGuid] = @OwnerGroupToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerGroupToAutoDistributionRule] 
	WHERE [OwnerGroupToAutoDistributionRuleGuid] = @OwnerGroupToAutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerGroupToAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerGroupToAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByOwnerGroupGuid]
	@OwnerGroupGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerGroupToAutoDistributionRule]
			WHERE [OwnerGroupGuid] = @OwnerGroupGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerGroupToAutoDistributionRule] 
	WHERE [OwnerGroupGuid] = @OwnerGroupGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleUpdateByRowGuid] (
	@OwnerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@OwnerGroupGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblOwnerGroupToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, OwnerGroupGuid = @OwnerGroupGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		OwnerGroupToAutoDistributionRuleGuid = @OwnerGroupToAutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@OwnerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@OwnerGroupGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.OwnerGroupToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.OwnerGroupGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblOwnerGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@OwnerGroupToAutoDistributionRuleGuid IS NULL) OR (@OwnerGroupToAutoDistributionRuleGuid = MAIN.OwnerGroupToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@OwnerGroupGuid IS NULL) OR (@OwnerGroupGuid = MAIN.OwnerGroupGuid))

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblOwnerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerGroupToAutoDistributionRuleSelectOwnerGroup] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.OwnerGroupToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.OwnerGroupGuid,
		ASSIGNED.*
	FROM 
		[map].[tblOwnerGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblApplicationString] ASSIGNED WITH (NOLOCK)
		ON MAIN.OwnerGroupGuid = ASSIGNED.ApplicationStringGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END
GO

-- ===================== Creating TABLE - [map].[tblOwnerToAutoDistributionRule] ==========================================================
CREATE TABLE [map].[tblOwnerToAutoDistributionRule] (
	[OwnerToAutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblOwnerToAutoDistributionRule_OwnerToAutoDistributionRuleGuid] DEFAULT NEWID(),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblOwnerToAutoDistributionRule_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid),
	[OwnerGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblOwnerToAutoDistributionRule_OwnerGuid]
		REFERENCES tblCompanies(CompanyGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblOwnerToAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblOwnerToAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblOwnerToAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblOwnerToAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblOwnerToAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[OwnerToAutoDistributionRuleGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblOwnerToAutoDistributionRule_AutoDistributionRuleGuid_OwnerGuid]  UNIQUE
		([AutoDistributionRuleGuid], [OwnerGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@OwnerGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@OwnerToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblOwnerToAutoDistributionRule]
	( 
		OwnerToAutoDistributionRuleGuid, AutoDistributionRuleGuid, OwnerGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @OwnerGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @OwnerToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleDeleteByRowGuid]
	@OwnerToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerToAutoDistributionRule]
			WHERE [OwnerToAutoDistributionRuleGuid] = @OwnerToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerToAutoDistributionRule] 
	WHERE [OwnerToAutoDistributionRuleGuid] = @OwnerToAutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerToAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerToAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleDeleteByOwnerGuid]
	@OwnerGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblOwnerToAutoDistributionRule]
			WHERE [OwnerGuid] = @OwnerGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of OwnerToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblOwnerToAutoDistributionRule] 
	WHERE [OwnerGuid] = @OwnerGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleUpdateByRowGuid] (
	@OwnerToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@OwnerGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblOwnerToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, OwnerGuid = @OwnerGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		OwnerToAutoDistributionRuleGuid = @OwnerToAutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@OwnerToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@OwnerGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.OwnerToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.OwnerGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblOwnerToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@OwnerToAutoDistributionRuleGuid IS NULL) OR (@OwnerToAutoDistributionRuleGuid = MAIN.OwnerToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@OwnerGuid IS NULL) OR (@OwnerGuid = MAIN.OwnerGuid))

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleSelectOwner] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.OwnerToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.OwnerGuid,
		ASSIGNED.*
	FROM 
		[map].[tblOwnerToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblCompanies] ASSIGNED WITH (NOLOCK)
		ON MAIN.OwnerGuid = ASSIGNED.CompanyGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END
GO

-- ===================== Creating TABLE - [map].[tblProductGroupToAutoDistributionRule] ==========================================================
CREATE TABLE [map].[tblProductGroupToAutoDistributionRule] (
	[ProductGroupToAutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblProductGroupToAutoDistributionRule_ProductGroupToAutoDistributionRuleGuid] DEFAULT NEWID(),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblProductGroupToAutoDistributionRule_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid),
	[ProductGroupGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblProductGroupToAutoDistributionRule_ProductGroupGuid]
		REFERENCES tblApplicationString(ApplicationStringGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblProductGroupToAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblProductGroupToAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblProductGroupToAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblProductGroupToAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblProductGroupToAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[ProductGroupToAutoDistributionRuleGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblProductGroupToAutoDistributionRule_AutoDistributionRuleGuid_ProductGroupGuid]  UNIQUE
		([AutoDistributionRuleGuid], [ProductGroupGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGroupGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@ProductGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblProductGroupToAutoDistributionRule]
	( 
		ProductGroupToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ProductGroupGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ProductGroupGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ProductGroupToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleDeleteByRowGuid]
	@ProductGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblProductGroupToAutoDistributionRule]
			WHERE [ProductGroupToAutoDistributionRuleGuid] = @ProductGroupToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ProductGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblProductGroupToAutoDistributionRule] 
	WHERE [ProductGroupToAutoDistributionRuleGuid] = @ProductGroupToAutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblProductGroupToAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ProductGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblProductGroupToAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleDeleteByProductGroupGuid]
	@ProductGroupGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblProductGroupToAutoDistributionRule]
			WHERE [ProductGroupGuid] = @ProductGroupGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ProductGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblProductGroupToAutoDistributionRule] 
	WHERE [ProductGroupGuid] = @ProductGroupGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleUpdateByRowGuid] (
	@ProductGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGroupGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblProductGroupToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, ProductGroupGuid = @ProductGroupGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		ProductGroupToAutoDistributionRuleGuid = @ProductGroupToAutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ProductGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ProductGroupGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ProductGroupToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ProductGroupGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblProductGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ProductGroupToAutoDistributionRuleGuid IS NULL) OR (@ProductGroupToAutoDistributionRuleGuid = MAIN.ProductGroupToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ProductGroupGuid IS NULL) OR (@ProductGroupGuid = MAIN.ProductGroupGuid))

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleSelectProductGroup] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.ProductGroupToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.ProductGroupGuid,
		ASSIGNED.*
	FROM 
		[map].[tblProductGroupToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblApplicationString] ASSIGNED WITH (NOLOCK)
		ON MAIN.ProductGroupGuid = ASSIGNED.ApplicationStringGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END
GO

-- ===================== Creating TABLE - [map].[tblProductToAutoDistributionRule] ==========================================================
CREATE TABLE [map].[tblProductToAutoDistributionRule] (
	[ProductToAutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblProductToAutoDistributionRule_ProductToAutoDistributionRuleGuid] DEFAULT NEWID(),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblProductToAutoDistributionRule_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid),
	[ProductGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblProductToAutoDistributionRule_ProductGuid]
		REFERENCES tblProducts(ProductGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblProductToAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblProductToAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblProductToAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblProductToAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblProductToAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[ProductToAutoDistributionRuleGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblProductToAutoDistributionRule_AutoDistributionRuleGuid_ProductGuid]  UNIQUE
		([AutoDistributionRuleGuid], [ProductGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@ProductToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblProductToAutoDistributionRule]
	( 
		ProductToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ProductGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ProductGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ProductToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleDeleteByRowGuid]
	@ProductToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblProductToAutoDistributionRule]
			WHERE [ProductToAutoDistributionRuleGuid] = @ProductToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ProductToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblProductToAutoDistributionRule] 
	WHERE [ProductToAutoDistributionRuleGuid] = @ProductToAutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblProductToAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ProductToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblProductToAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleDeleteByProductGuid]
	@ProductGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblProductToAutoDistributionRule]
			WHERE [ProductGuid] = @ProductGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ProductToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblProductToAutoDistributionRule] 
	WHERE [ProductGuid] = @ProductGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleUpdateByRowGuid] (
	@ProductToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblProductToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, ProductGuid = @ProductGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		ProductToAutoDistributionRuleGuid = @ProductToAutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@ProductToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@ProductGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.ProductToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.ProductGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblProductToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@ProductToAutoDistributionRuleGuid IS NULL) OR (@ProductToAutoDistributionRuleGuid = MAIN.ProductToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@ProductGuid IS NULL) OR (@ProductGuid = MAIN.ProductGuid))

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleSelectProduct] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.ProductToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.ProductGuid,
		ASSIGNED.*
	FROM 
		[map].[tblProductToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblProducts] ASSIGNED WITH (NOLOCK)
		ON MAIN.ProductGuid = ASSIGNED.ProductGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END
GO

-- ===================== Creating TABLE - [map].[tblTransactionAliasToAutoDistributionRule] ==========================================================
CREATE TABLE [map].[tblTransactionAliasToAutoDistributionRule] (
	[TransactionAliasToAutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [DF_map_tblTransactionAliasToAutoDistributionRule_TransactionAliasToAutoDistributionRuleGuid] DEFAULT NEWID(),
	[AutoDistributionRuleGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblTransactionAliasToAutoDistributionRule_AutoDistributionRuleGuid]
		REFERENCES tblAutoDistributionRule(AutoDistributionRuleGuid),
	[TransactionAliasGuid] UNIQUEIDENTIFIER NOT NULL
		CONSTRAINT [FK_map_tblTransactionAliasToAutoDistributionRule_TransactionAliasGuid]
		REFERENCES tblTransactionAliases(TransactionAliasGuid)
			ON DELETE CASCADE ON UPDATE CASCADE,
			
	[CreatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblTransactionAliasToAutoDistributionRule_CreatedDate] DEFAULT (SysDateTimeOffset()),	
	[CreatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblTransactionAliasToAutoDistributionRule_CreatedBy] DEFAULT '',	
	[UpdatedDate] DATETIMEOFFSET(7) NOT NULL
		CONSTRAINT [DF_map_tblTransactionAliasToAutoDistributionRule_UpdatedDate] DEFAULT (SysDateTimeOffset()),	
	[UpdatedBy] UdtUserId NOT NULL
		CONSTRAINT [DF_map_tblTransactionAliasToAutoDistributionRule_UpdatedBy] DEFAULT '',	
	[_RowVersion] RowVersion NOT NULL,	
			
	CONSTRAINT [PK_map_tblTransactionAliasToAutoDistributionRule] PRIMARY KEY CLUSTERED 
	(
		[TransactionAliasToAutoDistributionRuleGuid] ASC
	),
			
	CONSTRAINT [[UX_map_tblTransactionAliasToAutoDistributionRule_AutoDistributionRuleGuid_TransactionAliasGuid]  UNIQUE
		([AutoDistributionRuleGuid], [TransactionAliasGuid] )
	
) 	
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy UDTUSERID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL,
	@TransactionAliasToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblTransactionAliasToAutoDistributionRule]
	( 
		TransactionAliasToAutoDistributionRuleGuid, AutoDistributionRuleGuid, TransactionAliasGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @TransactionAliasGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @TransactionAliasToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByRowGuid]
	@TransactionAliasToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblTransactionAliasToAutoDistributionRule]
			WHERE [TransactionAliasToAutoDistributionRuleGuid] = @TransactionAliasToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of TransactionAliasToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblTransactionAliasToAutoDistributionRule] 
	WHERE [TransactionAliasToAutoDistributionRuleGuid] = @TransactionAliasToAutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByAutoDistributionRuleGuid]
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblTransactionAliasToAutoDistributionRule]
			WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of TransactionAliasToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblTransactionAliasToAutoDistributionRule] 
	WHERE [AutoDistributionRuleGuid] = @AutoDistributionRuleGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByTransactionAliasGuid]
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblTransactionAliasToAutoDistributionRule]
			WHERE [TransactionAliasGuid] = @TransactionAliasGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of TransactionAliasToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblTransactionAliasToAutoDistributionRule] 
	WHERE [TransactionAliasGuid] = @TransactionAliasGuid; 
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Update a record in the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleUpdateByRowGuid] (
	@TransactionAliasToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@TransactionAliasGuid UNIQUEIDENTIFIER,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy UDTUSERID,
	@_RowVersion VARBINARY(8) = NULL
) AS
BEGIN
	UPDATE [map].[tblTransactionAliasToAutoDistributionRule]
	SET
		AutoDistributionRuleGuid = @AutoDistributionRuleGuid, TransactionAliasGuid = @TransactionAliasGuid, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
	WHERE
		TransactionAliasToAutoDistributionRuleGuid = @TransactionAliasToAutoDistributionRuleGuid
END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleSelect] (
	@SelectedSiteGuid UNIQUEIDENTIFIER,
	@LoginSiteGuid UNIQUEIDENTIFIER,
	@TransactionAliasToAutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER = NULL,
	@TransactionAliasGuid UNIQUEIDENTIFIER = NULL
) AS
BEGIN
	DECLARE @AreLoginCurrentSiteTheSame BIT
	SET @AreLoginCurrentSiteTheSame = CASE WHEN @SelectedSiteGuid = @LoginSiteGuid THEN 1 ELSE 0 END
	SELECT 
		MAIN.TransactionAliasToAutoDistributionRuleGuid, MAIN.AutoDistributionRuleGuid, MAIN.TransactionAliasGuid, 
		MAIN.CreatedDate, MAIN.CreatedBy, MAIN.UpdatedDate, MAIN.UpdatedBy, 
		MAIN._RowVersion
	FROM 
		[map].[tblTransactionAliasToAutoDistributionRule] MAIN WITH (NOLOCK)
	WHERE
		((@TransactionAliasToAutoDistributionRuleGuid IS NULL) OR (@TransactionAliasToAutoDistributionRuleGuid = MAIN.TransactionAliasToAutoDistributionRuleGuid))
		AND ((@AutoDistributionRuleGuid IS NULL) OR (@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid))
		AND ((@TransactionAliasGuid IS NULL) OR (@TransactionAliasGuid = MAIN.TransactionAliasGuid))

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Select assigned record(s) from the [map].[tblTransactionAliasToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_TransactionAliasToAutoDistributionRuleSelectTransactionAlias] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER
) AS
BEGIN
	SELECT 
		MAIN.TransactionAliasToAutoDistributionRuleGuid,
		MAIN.AutoDistributionRuleGuid,
		MAIN.TransactionAliasGuid,
		ASSIGNED.*
	FROM 
		[map].[tblTransactionAliasToAutoDistributionRule] MAIN WITH (NOLOCK)
		INNER JOIN [dbo].[tblTransactionAliases] ASSIGNED WITH (NOLOCK)
		ON MAIN.TransactionAliasGuid = ASSIGNED.TransactionAliasGuid
	WHERE
		(@AutoDistributionRuleGuid = MAIN.AutoDistributionRuleGuid)

END
GO

-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete(cascade) a record from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteApplication] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@RowVersion TIMESTAMP = NULL
) AS
BEGIN
	EXEC [map].[usp_AutoDistributionRuleToSiteDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ManagerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_OwnerGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_OwnerToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ProductGroupToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_ProductToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC [map].[usp_TransactionAliasToAutoDistributionRuleDeleteByAutoDistributionRuleGuid] @AutoDistributionRuleGuid
	EXEC usp_AutoDistributionRuleDeleteByRowGuid @AutoDistributionRuleGuid
END
GO

