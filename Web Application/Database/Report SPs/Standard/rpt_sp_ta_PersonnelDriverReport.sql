USE [ConsolidatedDB]
GO

/****** Object:  StoredProcedure [dbo].[rpt_sp_ta_PersonnelDriverReport]    Script Date: 09/27/2011 13:26:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT ROUTINE_NAME from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'rpt_sp_ta_PersonnelDriverReport')
BEGIN
DROP PROCEDURE [dbo].[rpt_sp_ta_PersonnelDriverReport]
END
GO



CREATE PROCEDURE [dbo].[rpt_sp_ta_PersonnelDriverReport]
 /*=============================================
 Author:	 			UNKNOWN
 Create date: 			
 Description: 
 Version:				7.5.2.1
 Execution:				
				execute rpt_sp_ta_PersonnelDriverReport 1,1,2,'<All>',90,2
 Modification History:
	Date		by		Description
	6/11/2009	UP		Rename from fm_PersonnelDriverReport to rpt_sp_ta_PersonnelDriverReport
    10/6/2009	KF		Add the ability to select Expiration dates between 30,60,90 days.
	12/9/2009	KF		Version 7.5.1.0
	09/27/2011	W.Gray	Revised to test for expired value when @Expired = 0
	11/05/2012	C. Knight 	Do not check for expired when @Expired = 0; @Expired = 0 means show
					All users.  Suppress historical records; we just want current
 =============================================*/
	@LoginSiteIndex int,
	@SiteIndex		int,
	@UserIndex		int,
	@Carrier		nvarchar(200) = NULL,
	@Expired		int,
	@Header			int

AS 
BEGIN


IF @Header = 0 --Header Info

					BEGIN

					select distinct 
							 case when c.[Name] = '' then '<Not Assigned>' 
																else c.[Name] end as CarrierName
							,p.LastName + ','+ ' ' +	p.FirstName as DriverName 
							,case when p.CardNumber IS NULL then '<Not Assigned>' 
																else p.CardNumber end as CardNumber 
							,p.ShortCardNumber as ShortCardNo
							,case when p.OnFileSignature IS NULL then 'No' 
																else 'Yes' end as SignatureOnFile
							,q.ID as LicenseName
							,Case when qm.type = 3 then 'Qualifications' 
																else 'License' end as [Type]
							,qm.ID as Number
							,convert(char(10),qm.ExpirationDate,110) as LicenseExpiration
							,DATEDIFF(day, getdate(), qm.ExpirationDate) as diff
							,case when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 0 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30    then 30 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60     then 60 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=90     then 90 
																else 0 end as Days

					from tblPersonnel p with (nolock)
								 join tblPersonRoleMap prm with (nolock) on
												p.PersonIndex = prm.PersonIndex
								JOIN tblCompanyMap map WITH (NOLOCK)
									ON map.AssignedToIndex = p.PersonIndex
								 join tblCompanies c with (nolock) on
												map.AssignedIndex = c.CompanyIndex
								 join  tblQualificationsMap qm  with (nolock) on
												p.PersonIndex = qm.[Index] 
								 join tblQualifications q  with (nolock) on 
												q.[Index] = qm.AssignedIndex

					where 	 	 (@Carrier = '<All>' OR c.[Name] = @Carrier)
						     and ((DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30 and 
								   @Expired = 30) or
								  (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and 
								   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60 and 
								   @Expired = 60) or
								  (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and 
								   DATEDIFF(day, getdate(), qm.ExpirationDate) <=90 and 
								   @Expired = 90) or
							      (@Expired = 0))
							 and qm.type in(3,4)
							 and p.SiteIndex = @SiteIndex
							 and prm.[Role] in (0,1,2) -- DRIVER_ROLE = 0, SUPERVISOR_ROLE = 1, OFFLOADER_ROLE = 2
							 and qm.HistoricalRecord = 0


					order by CarrierName, DriverName,CardNumber,ShortCardNo,SignatureOnFile,LicenseName,
							 [Type],Number, LicenseExpiration
							

					END 
						ELSE

IF @Header = 1 ---Qualification Info
						BEGIN  

					select	
							 case when c.[Name] = '' then '<Not Assigned>' 
														else c.[Name] end as CarrierName
							,p.LastName + ','+ ' ' +	p.FirstName as DriverName 
							,case when p.CardNumber IS NULL then '<Not Assigned>'	
														else p.CardNumber end as CardNumber
							,p.ShortCardNumber as ShortCardNo
							,case when p.OnFileSignature IS NULL then 'No' 
														else 'Yes' end as SignatureOnFile 
							,q.ID as LicenseName
							,Case when qm.type = 3 then 'Qualifications' 
														else 'License' end as [Type]
							,qm.ID as Number
							,convert(char(10),qm.ExpirationDate,110) as LicenseExpiration
							,DATEDIFF(day, getdate(), qm.ExpirationDate) as diff
							,case when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 0 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30    then 30 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60     then 60 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=90     then 90 
														else 0 end as Days

					from tblPersonnel p with (nolock)
								 join tblPersonRoleMap prm with (nolock) on
												p.PersonIndex = prm.PersonIndex
								join tblCompanyMap map WITH(NOLOCK)
									ON map.AssignedToIndex = p.PersonIndex
								 join tblCompanies c with (nolock) on
												map.AssignedIndex = c.CompanyIndex
								 join  tblQualificationsMap qm  with (nolock) on
												p.PersonIndex = qm.[Index] 
								 join tblQualifications q  with (nolock) on 
												q.[Index] = qm.AssignedIndex

					where   (@Carrier = '<All>' OR c.[Name] = @Carrier)
						 and ((DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30 and 
                               @Expired = 30) or
						      (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and 
							   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60 and 
                               @Expired = 60) or
							  (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and 
                               DATEDIFF(day, getdate(), qm.ExpirationDate) <=90 and 
                               @Expired = 90)or
							   (@Expired = 0))
						 and qm.type = 3
						 and p.SiteIndex = @SiteIndex
						 and prm.[Role] in (0,1,2) -- DRIVER_ROLE = 0, SUPERVISOR_ROLE = 1, OFFLOADER_ROLE = 2
						 and map.[Type] = '15'
						 and qm.HistoricalRecord = 0


					order by c.[Name], p.LastName

			END 
				ELSE
 ---@Header 2 License Info
						BEGIN 

					select	
                             case when c.[Name] = '' then '<Not Assigned>' 
																else c.[Name] end as CarrierName
							,p.LastName + ','+ ' ' +	p.FirstName as DriverName 
							,case when p.CardNumber IS NULL then '<Not Assigned>' 
																else p.CardNumber end as CardNumber 
							,p.ShortCardNumber as ShortCardNo
							,case when p.OnFileSignature IS NULL then 'No' 
																else 'Yes' end as SignatureOnFile 
							,q.ID as LicenseName
							,Case when qm.type = 3 then 'Qualifications' 
																else 'License' end as [Type]
							,qm.ID as Number
							,convert(char(10),qm.ExpirationDate,110) as LicenseExpiration
							,DATEDIFF(day, getdate(), qm.ExpirationDate) as diff
							,case when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 0 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30    then 30 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60     then 60 
								  when DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and
									   DATEDIFF(day, getdate(), qm.ExpirationDate) <=90     then 90 
																else 0 end as Days

					from tblPersonnel p with (nolock)
								 join tblPersonRoleMap prm with (nolock) on
												p.PersonIndex = prm.PersonIndex
								join tblCompanyMap map WITH(NOLOCK)
									on map.AssignedToIndex = p.PersonIndex
								 join tblCompanies c with (nolock) on
												map.AssignedIndex = c.CompanyIndex
								 join  tblQualificationsMap qm  with (nolock) on
												p.PersonIndex = qm.[Index] 
								 join tblQualifications q  with (nolock) on 
												q.[Index] = qm.AssignedIndex

					where    (@Carrier = '<All>' OR c.[Name] = @Carrier)
						 and ((DATEDIFF(day, getdate(), qm.ExpirationDate) <= 30 and 
                               @Expired = 30) or
						      (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 30 and 
							   DATEDIFF(day, getdate(), qm.ExpirationDate) <=60 and 
                               @Expired = 60) or
							  (DATEDIFF(day, getdate(), qm.ExpirationDate) >= 60 and 
                               DATEDIFF(day, getdate(), qm.ExpirationDate) <=90 and 
                               @Expired = 90)or
							   (@Expired = 0))
						 and qm.type = 4
						 and p.SiteIndex = @SiteIndex
						 and prm.[Role] in (0,1,2) -- DRIVER_ROLE = 0, SUPERVISOR_ROLE = 1, OFFLOADER_ROLE = 2
						 and	map.[Type] = '15'
						 and qm.HistoricalRecord = 0


					order by c.[Name], p.LastName

					END
							END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_PersonnelDriverReport] TO [public]
GO
