CREATE PROCEDURE [dbo].[usp_DeletePointTemplatePointCommandStatusList]
@PointTemplateGuid UNIQUEIDENTIFIER,
@listDeletedPointCommandStatusList StringListType READONLY
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_DeletePointTemplatePointCommandStatusList] 
	-- Author: Francisco Martin
	-- Version/Date: 1.0.001 / 2017-08-08
	-- Purpose: Insert into table [dbo].[usp_DeletePointTemplatePointCommandStatusList]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

	UPDATE pt
	SET  [Value] = '<PointCommandStatusListReference>
		<PointCommandStatusListGuid>00000000-0000-0000-0000-000000000000</PointCommandStatusListGuid>
		<CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
	</PointCommandStatusListReference>',
	updateddate = SYSDATETIMEOFFSET()
	FROM tblPointTemplateTag ptt
	JOIN tblPointTag pt
	ON pt.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	JOIN @listDeletedPointCommandStatusList dpcl
	ON pt.[Value].value( '(/PointCommandStatusListReference/PointCommandStatusListGuid)[1]', 'varchar(max)') = dpcl.[value]
	WHERE ptt.PointTemplateGuid = @PointTemplateGuid
	AND ptt.ValueType = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference'



	UPDATE ptt
	SET  [Value] = '<PointCommandStatusListReference>
		<PointCommandStatusListGuid>00000000-0000-0000-0000-000000000000</PointCommandStatusListGuid>
		<CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
	</PointCommandStatusListReference>',
	updateddate = SYSDATETIMEOFFSET()
	FROM tblPointTemplateTag ptt
	JOIN @listDeletedPointCommandStatusList dpcl
	ON ptt.[Value].value( '(/PointCommandStatusListReference/PointCommandStatusListGuid)[1]', 'varchar(max)') = dpcl.[value]
	WHERE ptt.PointTemplateGuid = @PointTemplateGuid
	AND ptt.ValueType = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference'


UPDATE pp
	SET  [Value] = '<PointCommandStatusListReference>
		<PointCommandStatusListGuid>00000000-0000-0000-0000-000000000000</PointCommandStatusListGuid>
		<CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
	</PointCommandStatusListReference>',
	updateddate = SYSDATETIMEOFFSET()
	FROM tblPointTemplateProperty ptp
	JOIN tblPointProperty pp
	ON pp.PointTemplatePropertyGuid = ptp.PointTemplatePropertyGuid
	JOIN @listDeletedPointCommandStatusList dpcl
	ON pp.[Value].value( '(/PointCommandStatusListReference/PointCommandStatusListGuid)[1]', 'varchar(max)') = dpcl.[value]
	WHERE ptp.PointTemplateGuid = @PointTemplateGuid
	AND ptp.ValueType = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference'



	UPDATE ptp
	SET  [Value] = '<PointCommandStatusListReference>
		<PointCommandStatusListGuid>00000000-0000-0000-0000-000000000000</PointCommandStatusListGuid>
		<CurrentValue xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />
	</PointCommandStatusListReference>',
	updateddate = SYSDATETIMEOFFSET()
	FROM tblPointTemplateProperty ptp
	JOIN @listDeletedPointCommandStatusList dpcl
	ON ptp.[Value].value( '(/PointCommandStatusListReference/PointCommandStatusListGuid)[1]', 'varchar(max)') = dpcl.[value]
	WHERE ptp.PointTemplateGuid = @PointTemplateGuid
	AND ptp.ValueType = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference'



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
						+ 'Procedure Name: usp_DeletePointTemplatePointCommandStatusList' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END