USE [FM Archive]


GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_MonthToDateTankChange]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_MonthToDateTankChange]
GO

CREATE PROCEDURE [dbo].[rpt_sp_ta_MonthToDateTankChange]
 /*=============================================
 Author:		Kimberly Foote	 
 Create date:	6/14/2010
 Description:	Tank Report
 Version:		7.0.1.1
 Execution: 
	
	Execute rpt_sp_ta_MonthToDateTankChange

 Modification History:
 	Date		by		Description
	6/14/2010	KF		New sp - Version 1 due to report already existing.
						Union was created due to reporting services
						select statement hanging.
						Could not alias the table name had to keep as is.
 =============================================*/

AS


SELECT 
			  fm_tank_data.Point_Tag
			, fm_tank_description.Description
			, fm_tank_data.Product_Code
			, fm_tank_data.time_stamp
			, fm_tank_data.Tank_Level
			, fm_tank_data.Volume_Net
			, fm_tank_data.Volume_Remaining_Net
			, fm_tank_data.Volume_Available_Net
			, fm_tank_data.Tank_Level_Status
			, fm_tank_data.Volume_Gross_Status
			, fm_tank_data.Volume_Available_Net_Status
			, fm_tank_data.Volume_Remaining_Net_Status
			, fm_tank_data.Volume_Gross
			, fm_tank_data.Volume_Net_Status
			, fm_tank_data.Temperature
			, fm_tank_data.Temperature_Status
			, fm_tank_description.Temperature_Units
			, fm_tank_description.Level_Units
			, fm_tank_description.Volume_Units
 
FROM   fm_tank_data with(nolock) 
		Inner Join fm_tank_description with(nolock) on  
			fm_tank_data.Point_Tag = fm_tank_description.Point_Tag AND 
			fm_tank_data.System_Name = fm_tank_description.System_Name AND 
			fm_tank_data.Description_Time_stamp = fm_tank_description.time_stamp 

WHERE (fm_tank_data.time_stamp= (SELECT DISTINCT Max(fm_tank_data.time_stamp) FROM fm_tank_data))
	 

UNION ALL

SELECT 
			  fm_tank_data.Point_Tag
			, fm_tank_description.Description
			, fm_tank_data.Product_Code
			, fm_tank_data.time_stamp
			, fm_tank_data.Tank_Level
			, fm_tank_data.Volume_Net
			, fm_tank_data.Volume_Remaining_Net
			, fm_tank_data.Volume_Available_Net
			, fm_tank_data.Tank_Level_Status
			, fm_tank_data.Volume_Gross_Status
			, fm_tank_data.Volume_Available_Net_Status
			, fm_tank_data.Volume_Remaining_Net_Status
			, fm_tank_data.Volume_Gross
			, fm_tank_data.Volume_Net_Status
			, fm_tank_data.Temperature
			, fm_tank_data.Temperature_Status
			, fm_tank_description.Temperature_Units
			, fm_tank_description.Level_Units
			, fm_tank_description.Volume_Units
 
FROM   fm_tank_data with(nolock)
		Inner Join fm_tank_description with(nolock) on 
			fm_tank_data.Point_Tag = fm_tank_description.Point_Tag AND 
			fm_tank_data.System_Name = fm_tank_description.System_Name AND 
			fm_tank_data.Description_Time_stamp = fm_tank_description.time_stamp 

WHERE (fm_tank_data.time_stamp= (SELECT DISTINCT Min(fm_tank_data.time_stamp) FROM fm_tank_data 
								  WHERE ({ fn MONTH(fm_tank_data.time_stamp) }= { fn MONTH( { fn CURDATE() } ) } ) ))

ORDER BY fm_tank_data.Product_Code ASC, fm_tank_data.Point_Tag ASC, fm_tank_data.time_stamp ASC






GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_MonthToDateTankChange] TO [public]
GO