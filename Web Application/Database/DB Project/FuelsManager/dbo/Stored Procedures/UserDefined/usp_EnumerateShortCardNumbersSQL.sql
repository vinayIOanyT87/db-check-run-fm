
/*
	EXEC [dbo].[usp_EnumerateShortCardNumbersSQL] '00000000-0000-0000-0000-000000000001'

*/



CREATE PROCEDURE [dbo].[usp_EnumerateShortCardNumbersSQL]
(
	@TargetSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_EnumerateShortCardNumbersSQL] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Personnel records that have a given Personnel Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Personnel that have been assigned to this site/sitegroup only
	-- 2. This stored procedure replaces the PersonnelClass.EnumerateShortCardNumbersSQL inline SQL.  
	-- 3. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		SELECT b.ShortCardNumber
		FROM 
		[erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) a 
		INNER JOIN tblPersonnel b 
		ON a.PersonnelGuid = b.PersonnelGuid
		ORDER BY ShortCardNumber ASC
	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_EnumerateShortCardNumbersSQL' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END