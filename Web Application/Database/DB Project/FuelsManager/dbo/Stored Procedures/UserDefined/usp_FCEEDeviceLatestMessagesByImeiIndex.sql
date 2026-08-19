create procedure [dbo].[usp_FCEEDeviceLatestMessagesByImeiIndex]
	@fcedeviceguid uniqueidentifier
 ,@index int
 ,@validity bit
as
begin
  set nocount on    
	begin try
		select 
  		[message].[ImeiNumber]
		 ,[message].[MsgType]
		 ,[message].[Timestamp]
		 ,[message].[Index]
		 ,[message].[Device]
		 ,[message].[BinaryData]
		 ,[message].[EdgeData]
		 ,[message].[Validity]
		 ,[message].[FCEEMessageGuid]
		 ,[message].[CreatedDate] INTO #tempTable
		from (
			select 
				[imeinumber]
			 ,[msgtype]
			 ,[timestamp]
			 ,[index]
			 ,[device]
			 ,[binarydata]
			 ,[edgedata]
			 ,[SoftwareVersion]
			 ,[Validity]
			 ,[FCEEMessageGuid]
			 ,[createddate]
			 ,row_number() over (partition by [imeinumber], [index], [msgtype] order by [timestamp] desc, [createddate] desc) as [rownum] 
			from  [tblFCEEMessage]
			where [msgtype] > 3
				and [msgtype] <= 20
		) [message]
		inner join [tblfcedevice] [device] on [device].[imeinumber] = [message].[imeinumber]
			where [rownum] = 1 
			and [device].[fcedeviceguid] = @fcedeviceguid
			and [index] = @index
			order by [message].[imeinumber]
		 ,[index]
		 ,[msgtype]

		 MERGE INTO [tblFCEEMessage] USING #tempTable ON [tblFCEEMessage].FCEEMessageGuid = #tempTable.FCEEMessageGuid
		 WHEN MATCHED THEN
			UPDATE SET [tblFCEEMessage].Validity = @validity,
						  [tblFCEEMessage].UpdatedDate = Sysdatetimeoffset();
		 SELECT * FROM #tempTable


	end try
	begin catch
		declare	@_errmessage nvarchar(2048)      
				, @_errnumber int           
				, @_errprocname nvarchar(126)           
				, @_errlinenumber int;            
		set @_errmessage = error_message();        
		set @_errnumber = error_number();        
		set @_errprocname= error_procedure();        
		set @_errlinenumber = error_line();            
		set @_errmessage = 'Error: ' + @_errmessage + char(13)+char(10)                 
						+ 'Number: ' + cast(@_errnumber as varchar(20)) + char(13)+char(10)                 
						+ 'Procedure Name: usp_FCEEDeviceLatestMessagesByImeiIndex' + char(13)+char(10)                  
						+ 'Line Number: ' + isnull(cast(@_errlinenumber as varchar(20)),'') + char(13)+char(10);         
		raiserror(@_errmessage,18,1);      
	end catch
end