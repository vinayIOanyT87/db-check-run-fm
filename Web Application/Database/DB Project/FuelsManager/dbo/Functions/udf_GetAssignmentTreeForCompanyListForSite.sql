CREATE FUNCTION [dbo].[udf_GetAssignmentTreeForCompanyListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblEntityAssignmentTree TABLE
(
	[CompanyToSiteGuid] [uniqueidentifier]
	,[TrackedCompanyToSiteGuid] [uniqueidentifier]
	,[IncludeChangeTrackingFlag] [bit]
)
AS
BEGIN
	DECLARE @entityAssignmentList AS TABLE
	(
		EntityToSiteGuid uniqueidentifier
		,EntityGuid uniqueidentifier
		,MasterRecordGuid uniqueidentifier
		,AssignedFromSiteGuid uniqueidentifier
		,AssignedToSiteGuid uniqueidentifier
		,IncludeChangeTrackingFlag bit
	)

	-- Find any entity assigment that pertains to the specified sync_context_site_guid.  We're not looking for the specific CompanyGuid that will be used, we're just interested in the
	-- entity assignment records.  If the AssignedFromSiteGuid equals the SiteGuid (AssignedTo) then the record is a self owned entity mapping so the CompanyGuid is the real entity.
	INSERT INTO @entityAssignmentList
		SELECT [map].[tblEntityCompanyToSite].[CompanyToSiteGuid] 'EntityToSiteGuid'
				,CASE WHEN [map].[tblEntityCompanyToSite].[AssignedFromSiteGuid] = [map].[tblEntityCompanyToSite].[SiteGuid] THEN [map].[tblEntityCompanyToSite].[CompanyGuid] ELSE NULL END 'EntityGuid'
				,[map].[tblEntityCompanyToSite].[CompanyGuid] 'MasterRecordGuid'
				,[map].[tblEntityCompanyToSite].[AssignedFromSiteGuid]
				,[map].[tblEntityCompanyToSite].[SiteGuid] 'AssignedToSiteGuid'
				,1 'IncludeChangeTrackingFlag'
		FROM [map].[tblEntityCompanyToSite]
		WHERE ([map].[tblEntityCompanyToSite].[SiteGuid] = @sync_context_site_guid)

	INSERT INTO @tblEntityAssignmentTree SELECT EntityToSiteGuid, EntityToSiteGuid 'TrackedCompanyToSiteGuid', IncludeChangeTrackingFlag FROM @entityAssignmentList WHERE AssignedToSiteGuid = AssignedFromSiteGuid;

	-- Get a list of entity assignment records which came from another site.  Iterate through this list and walk up until we find the top.
	DECLARE @EntityRecGuid uniqueidentifier
	DECLARE @MasterRecGuid uniqueidentifier
	DECLARE @AssignedFromSiteGuid uniqueidentifier
	DECLARE @AssignedToSiteGuid uniqueidentifier
	DECLARE @EntityToSiteGuid uniqueidentifier
	DECLARE @IncludeChangeTrackingFlag bit
	
	DECLARE entity_assignment_cursor CURSOR 
	FOR SELECT EntityToSiteGuid, EntityGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid, IncludeChangeTrackingFlag FROM @entityAssignmentList WHERE AssignedToSiteGuid <> AssignedFromSiteGuid;

	OPEN entity_assignment_cursor;
	FETCH NEXT FROM entity_assignment_cursor INTO @EntityToSiteGuid, @EntityRecGuid, @MasterRecGuid, @AssignedFromSiteGuid, @AssignedToSiteGuid, @IncludeChangeTrackingFlag

	WHILE (@@FETCH_STATUS <> -1)
	BEGIN
		DECLARE @targetSiteGuid uniqueidentifier
		DECLARE @newTargetSiteGuid uniqueidentifier
		DECLARE @newSiteGuid uniqueidentifier
		DECLARE @newEntityToSiteGuid uniqueidentifier

		IF (@@FETCH_STATUS <> -2)
		BEGIN
			-- We can go ahead and insert the entity assignment that we're starting with. No need to get it again.
			INSERT INTO @tblEntityAssignmentTree SELECT @EntityToSiteGuid, @EntityToSiteGuid 'TrackedCompanyToSiteGuid', @IncludeChangeTrackingFlag;

			-- Now, let's go ahead and move to the next targetSite since we already know this information.
			SET @targetSiteGuid = @AssignedFromSiteGuid -- This will start at sync_context_site_guid since our list was restricted to this.

			-- Now we can continue to walk up the entity assignment tree, inserting those entity assignment record guids into our final results.
			-- We don't care about the true equipment entities applicable at each level
			WHILE ((SELECT COUNT(*) FROM map.tblEntityCompanyToSite WHERE CompanyGuid = @MasterRecGuid AND SiteGuid = @targetSiteGuid) > 0)
			BEGIN
				SELECT @newEntityToSiteGuid = CompanyToSiteGuid, @newTargetSiteGuid = AssignedFromSiteGuid FROM map.tblEntityCompanyToSite WHERE CompanyGuid = @MasterRecGuid AND SiteGuid = @targetSiteGuid
				
				-- If it was assigned to us, we're interested in the change tracking information.
				IF (@targetSiteGuid <> @sync_context_site_guid)
				BEGIN
					SET @IncludeChangeTrackingFlag = 0;
				END
				ELSE
				BEGIN
					SET @IncludeChangeTrackingFlag = 1;
				END
				
				-- This should indicate that we've reached the owner of the master record.
				IF (@newTargetSiteGuid = @targetSiteGuid)
				BEGIN
					-- Add this record to the results and get out
					INSERT INTO @tblEntityAssignmentTree SELECT @newEntityToSiteGuid, @EntityToSiteGuid 'TrackedCompanyToSiteGuid', @IncludeChangeTrackingFlag

					GOTO GOTONEXT
				END
				ELSE
				BEGIN
					-- Add this record to the results and keep moving
					INSERT INTO @tblEntityAssignmentTree SELECT @newEntityToSiteGuid, @EntityToSiteGuid 'TrackedCompanyToSiteGuid', @IncludeChangeTrackingFlag

					SET @targetSiteGuid = @newTargetSiteGuid
				END
			END
		END

	GOTONEXT:
		FETCH NEXT FROM entity_assignment_cursor INTO @EntityToSiteGuid, @EntityRecGuid, @MasterRecGuid, @AssignedFromSiteGuid, @AssignedToSiteGuid, @IncludeChangeTrackingFlag
	END

	CLOSE entity_assignment_cursor
	DEALLOCATE entity_assignment_cursor

	RETURN;
END
