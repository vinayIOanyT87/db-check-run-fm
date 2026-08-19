
CREATE FUNCTION [dbo].[udf_CheckTWICConstraint]
(@Guid UNIQUEIDENTIFIER, @AssignedGuid UNIQUEIDENTIFIER, @Type INT, @ID NVARCHAR (50))
RETURNS INT
AS
BEGIN
	
	-- Faster in T-SQL to DECLARE everything up front. 
	DECLARE @AssignedID		NVARCHAR(80)		SET @AssignedID	  = ''	-- Qualification Name 
	DECLARE @Count				INT					SET @Count			  = 0
	DECLARE @SUCCESS			INT					SET @SUCCESS		  = 0
	DECLARE @PERSON_LICENSE INT					SET @PERSON_LICENSE = 4
	
	-- We're only checking PERSON_LICENSEs here, so any other qualification type 
	-- we don't care about: passes as far as we're concerned. Techically redundant 
	-- with checking @AssignedGuid, but it's a good performance enhancement. 
	IF @Type <> @PERSON_LICENSE
	BEGIN
		RETURN @SUCCESS									-- Success - Not a PERSON_LICENSE at all 
	END

	-- Okay, you asked for a PERSON_LICENSE, but what *kind* of PERSON_LICENSE? 
	-- We only care about 'TWIC'. 
	SELECT @AssignedID = ID								-- Qualification name - is it 'TWIC'? 
	  FROM dbo.tblQualifications						-- Lookup table of all 12 qualifications we currently track. 
	 WHERE QualificationGuid = @AssignedGuid					-- Find the row you asked for, like 17 for 'TWIC'. 

	-- We don't care about not 'Drivers License', or 'Driver Signature', etc. 
	-- Passes as far as we're concerned. 
	IF @AssignedID <> 'TWIC'
	BEGIN
		RETURN @SUCCESS									-- Success - A PERSON_LICENSE, but not a TWIC one 
	END

	-- Now that we know you have a TWIC card, let's see if it's any good. 
	SELECT @Count = COUNT(*)							-- Does there exist... 
	FROM map.tblQualificationCompanyCertificateAndPermitToCompany	AS qm1			-- in the equipment-qualification mappings... 
	,	map.tblQualificationEquipmentTestAndInspectionToEquipment	AS qm2
	,	map.tblQualificationEquipmentTagAndLicenseToEquipment AS qm3
	,	map.tblQualificationPersonQualificationToPerson AS qm4
	,	map.tblQualificationPersonLicenseToPerson AS qm5
	,	map.tblQualificationPersonTrainingToPerson AS qm6
	,	map.tblQualificationPersonQualificationToEquipmentType as qm7
	,	map.tblQualificationPersonTrainingToEquipmentType as qm8
	,	map.tblQualificationPersonQualificationToStation as qm9
	,	map.tblQualificationPersonTrainingToStation as qm10
	,	map.tblQualificationEquipmentTestAndInspectionToStation as qm11
	WHERE qm1.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
	   AND qm1.ID             = @ID					-- with this card id number... 
	   AND qm1.CompanyGuid        <> @Guid             -- and is somebody else; the application will prevent multiple
	                                             -- qualifications of the same variety for the same person
		AND qm2.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm2.ID             = @ID					-- with this card id number... 
		AND qm2.EquipmentGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm3.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm3.ID             = @ID					-- with this card id number... 
		AND qm3.EquipmentGuid        <> @Guid             -- and is somebody else; the application will prevent multiple
	
		AND qm4.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm4.ID             = @ID					-- with this card id number... 
		AND qm4.PersonnelGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm5.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm5.ID             = @ID					-- with this card id number... 
		AND qm5.PersonnelGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm6.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm6.ID             = @ID					-- with this card id number... 
		AND qm6.PersonnelGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm7.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm7.ID             = @ID					-- with this card id number... 
		AND qm7.EquipmentTypeGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm8.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm8.ID             = @ID					-- with this card id number... 
		AND qm8.EquipmentTypeGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm9.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm9.ID             = @ID					-- with this card id number... 
		AND qm9.StationGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm10.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm10.ID             = @ID					-- with this card id number... 
		AND qm10.StationGuid        <> @Guid             -- and is somebody else; the application will prevent multiple

		AND qm11.QualificationGuid  = @AssignedGuid      -- specifically, for the 'TWIC' qualification... 
		AND qm11.ID             = @ID					-- with this card id number... 
		AND qm11.StationGuid        <> @Guid             -- and is somebody else; the application will prevent multiple
		
	-- Done. 
	IF @Count = 0										-- 
		RETURN @SUCCESS									-- Success - No other person uses this TWIC card. 

	RETURN @Count											-- ERROR! matching TWIC card! 
END