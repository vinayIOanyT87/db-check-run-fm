USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetStandardTransAliasesForInventoryReconciliation]    Script Date: 09/14/2012 15:22:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetStandardTransAliasesForInventoryReconciliation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetStandardTransAliasesForInventoryReconciliation]
GO

USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetStandardTransAliasesForInventoryReconciliation]    Script Date: 09/14/2012 15:22:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================================================================
-- Author:		Wayne Keadle
-- Create date: 9/14/2012
-- Version:		7.5.2.0
-- Description:	This stored procedure returns the configured transaction aliases for the 
--				Inventory Reconciliation View in FuelsManager.
-- =============================================================================================
CREATE PROCEDURE [dbo].[usp_GetStandardTransAliasesForInventoryReconciliation] 
	-- Add the parameters for the stored procedure here
	@SiteIndex INT = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Define local variables
	DECLARE @Tmp TABLE(ID INT NOT NULL IDENTITY(1,1),AliasName NVARCHAR(32))
	DECLARE @Cnt INT
	DECLARE @Total INT
	DECLARE @TransAliases NVARCHAR(300)
	
	INSERT INTO @Tmp 

		-- Transaction Alias Fields for Inventory Reconciliation Report
		SELECT DISTINCT ta.AliasName
		FROM tblListViewFields lvf
		INNER JOIN tblListViews lv ON lvf.ListViewIndex = lv.[Index]
		INNER JOIN tblTransactionAliases ta ON lvf.TypeIndex = ta.AliasID
		WHERE ta.SiteIndex = @SiteIndex
		AND lv.[Type] = 2						-- Standard View
		AND lv.[TypeIndex] = 4					-- Inventory Reconciliation View
		AND ta.TransTypeID IN (1,5,6,8,15)		-- Standard Transaction Alias Types for this report
	
	-- Assign counter variables
	SET @Cnt = 1
	SELECT  @Total = COUNT(*) 
	FROM @Tmp 
	
	-- Loop through temp table and create a concatenated string of Transaction Aliases
	WHILE @Cnt <= @Total
		BEGIN
			IF (@Cnt = 1)
				BEGIN
					SELECT @TransAliases = AliasName 
					FROM @Tmp 
					WHERE ID = @Cnt 
				END
			ELSE
				-- Concatenate a comma and the next value
				BEGIN
					SET @TransAliases = @TransAliases + ( SELECT ',' + AliasName
														  FROM @Tmp 
														  WHERE ID = @Cnt
														)
				END
				
			-- Increment counter
			SET @Cnt = @Cnt + 1
		END
	
	SELECT @TransAliases [Aliases];
	
END

GO

