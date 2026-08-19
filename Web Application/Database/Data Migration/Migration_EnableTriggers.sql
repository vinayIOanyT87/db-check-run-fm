USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_EnableTriggers]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_EnableTriggers') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_EnableTriggers
GO

CREATE PROCEDURE [dbo].Migration_EnableTriggers
 /*=============================================
 Author:			Eric Simmons
 Create date:		4/7/2010
Description:		Disable triggers in database to prevent firing during data migration process
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migration_EnableTriggers 0,NULL 

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarchar(MAX) = NULL 

AS

ALTER TABLE dbo.tblTransactionLineItems ENABLE TRIGGER TR_tblTransactionLineItems_IU_UpdateEquipmentVolume

GO