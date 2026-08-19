USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetSiteUnits]    Script Date: 09/06/2012 22:48:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetSiteUnits]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetSiteUnits]
GO

USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetSiteUnits]    Script Date: 09/06/2012 22:48:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:			Wayne Keadle
-- Create date:		June 22, 2012
-- Version:		7.5.2.0
-- Description:		This stored procedure is to obtain the units that the site is configured for their products.
-- Execution:		
-- Modification:	Changed stored procedure to use the Site table instead of the units from the tblProducts table.
--					This stored procedure uses the dbo.GetUnitAbbrev function to retrieve the site units.
--	NOTE:			The function will not work with SQL 64 bit instances, ONLY 32 bit SQL instances.
-- =============================================

CREATE PROCEDURE [dbo].[usp_GetSiteUnits] 

@SiteIndex INT

AS

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Define local variables
	DECLARE @temp NVARCHAR(100)
	DECLARE @VolumeUnitIndex INT
	DECLARE @AdditiveUnitIndex INT
	
	-- Get Volume and Additive Unit Indexes
	SELECT @VolumeUnitIndex = VolumeUnitIndex
			, @AdditiveUnitIndex = AdditiveVolumeUnitIndex
	FROM tblSites WITH (NOLOCK)
	WHERE SiteIndex = @SiteIndex

	-- Compare Unit Indexes
	IF @VolumeUnitIndex = @AdditiveUnitIndex 
		BEGIN
			SET @temp = 'All Volume Units: ' + CAST(dbo.GetUnitAbbrev(@VolumeUnitIndex,0) AS NVARCHAR(100))
		END
	ELSE
		BEGIN
			SET @temp = 'Volume Units: ' + CAST(dbo.GetUnitAbbrev(@VolumeUnitIndex,0) AS NVARCHAR(100)) + 
			'      Additive Units: ' + CAST(dbo.GetUnitAbbrev(@AdditiveUnitIndex,0) AS NVARCHAR(100))
		END
	
	-- Return string
	SELECT @temp AS [SiteUnits]
	
END


GO

