CREATE PROCEDURE [dbo].[usp_AlarmAddModify]
(
	@AlarmTempTable AlarmDataType READONLY,
	@EnableAdd BIT,
	@EnableModify BIT
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

MERGE dbo.tblAlarm at
USING @AlarmTempTable    temp        ON temp.[AlarmGuid] = at.[AlarmGuid]
              			 							                        
WHEN MATCHED AND	@EnableModify = 1 AND (
						temp.ID <> at.ID 
						OR temp.InputTagGuid <> at.InputTagGuid 
						OR temp.[Enabled] <> at.[Enabled] 
						OR temp.AlarmCategoryApplicationStringGuid <> at.AlarmCategoryApplicationStringGuid
						OR temp.[Order] <> at.[Order] 
						OR temp.NotAlarmState <> at.NotAlarmState 						
						OR ISNULL(temp.Comment,'') <> ISNULL(at.Comment,'') 
						OR ISNULL(temp.ShelvedStartTimeStamp,'') <> ISNULL(at.ShelvedStartTimeStamp,'') 
						OR ISNULL(temp.ShelvedEndTimeStamp,'') <> ISNULL(at.ShelvedEndTimeStamp,'') 
						OR temp.ShelvedOneShot <> at.ShelvedOneShot 
						OR ISNULL(temp.ShelvedBy,'') <> ISNULL(at.ShelvedBy,'') 
						OR temp.Suppressed <> at.Suppressed 
						OR temp.AlarmStateTagGuid <> at.AlarmStateTagGuid
						OR temp.ExclusiveAlarm <> at.ExclusiveAlarm
						OR ISNULL(temp.AlarmTemplateGuid,'00000000-0000-0000-0000-000000000000') <> ISNULL(at.AlarmTemplateGuid,'00000000-0000-0000-0000-000000000000')
						OR ISNULL(temp.[Notify],'') <> ISNULL(at.[Notify],'')
					)THEN
    UPDATE SET	ID = temp.ID  
				,InputTagGuid = temp.InputTagGuid  
				,[Enabled] = temp.[Enabled]  
				,AlarmCategoryApplicationStringGuid = temp.AlarmCategoryApplicationStringGuid  
				,[Order] = temp.[Order]  
				,NotAlarmState = temp.NotAlarmState 
				,Comment = temp.Comment  
				,ShelvedStartTimeStamp = temp.ShelvedStartTimeStamp  
				,ShelvedEndTimeStamp = temp.ShelvedEndTimeStamp  
				,ShelvedOneShot = temp.ShelvedOneShot
				,ShelvedBy = temp.ShelvedBy
				,Suppressed = temp.Suppressed   
				,UpdatedBy = temp.UpdatedBy 
				,UpdatedDate = Sysdatetimeoffset() 
				,AlarmStateTagGuid = temp.AlarmStateTagGuid
				,ExclusiveAlarm = temp.ExclusiveAlarm
				,AlarmTemplateGuid = temp.AlarmTemplateGuid
				,Notify = temp.Notify

WHEN NOT MATCHED AND @EnableAdd = 1 THEN
    INSERT 
	 (
			[AlarmGuid],
			[InputTagGuid],
			[ID],
			[Enabled],
			[AlarmCategoryApplicationStringGuid],
			[Order],
			[NotAlarmState],
			[Comment],
			[ShelvedStartTimeStamp],
			[ShelvedEndTimeStamp],
			[ShelvedOneShot],
			[ShelvedBy],
			[Suppressed],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy],
			[AlarmStateTagGuid],
			[ExclusiveAlarm],
			[AlarmTemplateGuid],
			[Notify]
	)
	VALUES
	(
		temp.AlarmGuid
		,temp.InputTagGuid
		,temp.ID
		,temp.[Enabled]
		,temp.AlarmCategoryApplicationStringGuid
		,temp.[Order]
		,temp.NotAlarmState
		,temp.Comment
		,temp.ShelvedStartTimeStamp
		,temp.ShelvedEndTimeStamp
		,temp.ShelvedOneShot
		,temp.ShelvedBy
		,temp.Suppressed
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,sysdatetimeoffset()
		,temp.UpdatedBy
		,temp.AlarmStateTagGuid
		,temp.ExclusiveAlarm
		,temp.AlarmTemplateGuid
		,temp.Notify
	);

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
						+ 'Procedure Name: usp_AlarmAddModify' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 