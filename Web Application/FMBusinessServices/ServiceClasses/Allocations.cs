using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.UtilityObjects;

using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;
    using System.Collections.Generic;

    /// <summary>
    /// Summary description for Allocations.
    /// </summary>
    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AllocationsClass : IDependency, IAllocations
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
		private List<GroupClass> userGroupList;

		protected void UpdateLineItems(SecurityClass security, AllocationClass allocation)
		{
			var allocationLineItems = new AllocationLineItemsClass();
			AllocationLineItemCollectionClass existingLineItems = allocationLineItems.EnumerateByAllocationGuid(security, allocation.IdentityGuid);
			AllocationLineItemCollectionClass newLineItems = allocation.LineItemCollection;

			if (newLineItems != null)
			{
				foreach (AllocationLineItemClass newLineItem in newLineItems)
				{
					int existingItem;

					newLineItem.AllocationGuid = allocation.IdentityGuid;

					for (existingItem = 0; existingItem < existingLineItems.Count; existingItem++)
					{
						AllocationLineItemClass existingLineItem = existingLineItems.Item(existingItem);

						if (existingLineItem.Type == newLineItem.Type
						&& existingLineItem.AssignedGuid == newLineItem.AssignedGuid
						&& existingLineItem.ResetPeriod == newLineItem.ResetPeriod)
						{
							newLineItem.IdentityGuid = existingLineItem.IdentityGuid;

						    if (!existingLineItem.Limit.Equals(newLineItem.Limit) || !existingLineItem.Next.Equals(newLineItem.Next)
						        || existingLineItem.ResetMultiple != newLineItem.ResetMultiple
						        || existingLineItem.ResetMethod != newLineItem.ResetMethod
						        || !existingLineItem.ResetDate.Equals(newLineItem.ResetDate))

						    {
						        allocationLineItems.Modify(security, newLineItem);
						    }

							break;
						}
					}

				    if (existingItem == existingLineItems.Count)
				    {
				        allocationLineItems.Add(security, newLineItem);
				    }
				    else
				    {
				        existingLineItems.Remove(existingItem);
				    }
				}
			}

		    foreach (AllocationLineItemClass existingLineItem in existingLineItems)
		    {
		        allocationLineItems.Purge(security, existingLineItem.IdentityGuid);
		    }
		}

		/// <summary>
		/// Determines whether this allocation can be viewed by the user in security.  
		/// <para />
		/// This function verifies that the user has access to view at least one of the groups
		/// that the grants the user view allocation rights also grants access to at least one of the 
		/// companies in the allocation.
		/// </summary>
		/// <param name="security">security context of the user requesting to view the allocation.</param>
		/// <param name="companyMap">Company map that the allcoation is for.</param>
		/// <param name="mapType">Type of the company map.</param>
		/// <param name="inUserGroupList">A list of user groups.</param>
		/// <returns>true if the allocation is viewable by the user, false if not</returns>
		public bool CanViewAllocation(SecurityClass security, CompanyMapClass companyMap, COMPANY_MAP_TYPE mapType, List<GroupClass> inUserGroupList)
		{
			this.userGroupList = inUserGroupList;
			return companyMap != null && this.CanViewAllocation(security, companyMap, mapType);
		}

        /// <summary>
        /// Determines whether this allocation can be viewed by the user in security.  
        /// <para />
        /// This function verifies that the user has access to view at least one of the groups
        /// that the grants the user view allocation rights also grants access to at least one of the 
        /// companies in the allocation.
        /// </summary>
        /// <param name="security">security context of the user requesting to view the allocation.</param>
        /// <param name="companyMap">Company map that the allcoation is for.</param>
        /// <param name="mapType">Type of the company map.</param>
        /// <returns>true if the allocation is viewable by the user, false if not</returns>
        public bool CanViewAllocation(SecurityClass security, CompanyMapClass companyMap, COMPANY_MAP_TYPE mapType)
        {
            if (security.UserGuid == Guid.Empty)
            {
                // UserGuid of empty guid indicates that this request comes from the load rack.
                // Permit this request.
                return true;
            }

            // for each group that the user belongs to
            foreach (GroupClass groupWithCompanyMaps in this.userGroupList)
            {
                foreach (CompanyMapClass groupCompanyMap in groupWithCompanyMaps.CompanyMapCollection)
                {
                    if (groupCompanyMap.AssignedGuid == Guid.Empty)
                    {
                        // we have a group that allows access to all companies,
                        return true;
                    }

                    if (groupCompanyMap.AssignedGuid == companyMap.AssignedGuid
                         || groupCompanyMap.AssignedGuid == companyMap.AssignedToGuid)
                    {
                        // We have access based on a particular company
                        return true;
                    }
                }

                // We don't have access yet; try one step further up the company hierarchy if we aren't at the top already
                var companyMaps = new CompanyMapsClass();
                switch (mapType)
                {
                    case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
                            if (this.CanViewAllocation(security, link, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                    case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
                            if (this.CanViewAllocation(security, link, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                    case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
                            if (this.CanViewAllocation(security, link, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                    case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP);
                            if (this.CanViewAllocation(security, link, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                }
            }

            return false;
        }

		/// <summary>
		/// Determines whether this allocation can be viewed by the user in security.  
		/// <para />
		/// This function verifies that the user has access to view at least one of the groups
		/// that the grants the user view allocation rights also grants access to at least one of the 
		/// companies in the allocation.
		/// </summary>
		/// <param name="security">security context of the user requesting to view the allocation.</param>
		/// <param name="companyMap">The company map that the allcoation is for.</param>
		/// <param name="mapType">Type of the company map.</param>
		/// <returns>true if the allocation is viewable by the user, false if not</returns>
		public bool CanModifyAllocation(SecurityClass security, CompanyMapClass companyMap, COMPANY_MAP_TYPE mapType, List<GroupClass> inUserGroupList)
        {
            this.userGroupList = inUserGroupList;
			return companyMap != null && this.CanModifyAllocation(security, companyMap, mapType);
        }

        /// <summary>
        /// Determines whether this allocation can be viewed by the user in security.  
        /// <para />
        /// This function verifies that the user has access to view at least one of the groups
        /// that the grants the user view allocation rights also grants access to at least one of the 
        /// companies in the allocation.
        /// </summary>
        /// <param name="security">security context of the user requesting to view the allocation.</param>
        /// <param name="companyMap">Company map that the allcoation is for.</param>
        /// <param name="mapType">Type of the company map.</param>
        /// <returns>true if the allocation is viewable by the user, false if not</returns>
        private bool CanModifyAllocation(SecurityClass security, CompanyMapClass companyMap, COMPANY_MAP_TYPE mapType)
        {
            if (security.UserGuid == Guid.Empty)
            {
                // UserGuid of empty guid indicates that this request comes from the load rack.
                // Permit this request.
                return true;
            }

            // for each group that the user belongs to
            foreach (GroupClass groupWithCompanyMaps in this.userGroupList)
            {
                foreach (CompanyMapClass groupCompanyMap in groupWithCompanyMaps.CompanyMapCollection)
                {
                    if (groupCompanyMap.AssignedGuid == Guid.Empty)
                    {
                        // we have a group that allows access to all companies,
                        return true;
                    }

                    if (groupCompanyMap.AssignedGuid == companyMap.AssignedGuid
                         || groupCompanyMap.AssignedGuid == companyMap.AssignedToGuid)
                    {
                        // We have access based on a particular company
                        return true;
                    }
                }

                // We don't have access yet; try one step further up the company hierarchy if we aren't at the top already
                var companyMaps = new CompanyMapsClass();
                switch (mapType)
                {
                    case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
                            if (this.CanModifyAllocation(security, link, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                    case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
                            if (this.CanModifyAllocation(security, link, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                    case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
                            if (this.CanModifyAllocation(security, link, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                    case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
                        {
                            CompanyMapClass link = companyMaps.Get(security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP);
                            if (this.CanModifyAllocation(security, link, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP))
                            {
                                return true;
                            }
                        }

                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// This method will retrieve the groups associated to the user.
        /// </summary>
        /// <param name="security">The security class.</param>
        /// <returns>Return a list of user groups.</returns>
        public List<GroupClass> GetUserGroups(SecurityClass security)
		{
			var groupList = new List<GroupClass>();

			var usersSvr = new UsersClass();
			UserClass user = usersSvr.Get(security, security.UserGuid);

			foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
			{
				var groupsSvr = new GroupsClass();
				GroupClass groupWithCompanyMaps = groupsSvr.Get(security, userGroupMap.GroupGuid);
				groupList.Add(groupWithCompanyMaps);
			}

			return groupList;
		}

		/// <summary>
		/// This method determines whether a user has allocation rights and if a user's user groups 
		/// has access to all companies.
		/// </summary>
		/// <param name="security">The security class</param>
		/// <returns>Returns enumeration indicating the user group status.</returns>
		public AllocationClass.UserAllocationStatus UserHasAllocationRightsAndCompanyMapCollection(SecurityClass security)
		{
			bool hasAllocationRights = false;
			bool hasAllGroupCompanyMapping = false;

			var usersSvr = new UsersClass();
			UserClass user = usersSvr.Get(security, security.UserGuid);

			foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
			{
				var rightsSvr = new RightsClass();
				RightCollectionClass groupRights = rightsSvr.EnumerateByGroup(security, userGroupMap.GroupGuid);

				// If this group doesn't grant access to allocations, then authorized companies
				// in this group don't apply.
				if (groupRights.RightInCollection(RIGHT.VIEW_ALLOCATIONS) == false
					 && groupRights.RightInCollection(RIGHT.MODIFY_ALLOCATIONS) == false)
				{
					continue;
				}

				hasAllocationRights = true;

				var groupsSvr = new GroupsClass();
				GroupClass groupWithCompanyMaps = groupsSvr.Get(security, userGroupMap.GroupGuid);

				if (groupWithCompanyMaps.CompanyMapCollection == null || groupWithCompanyMaps.CompanyMapCollection.Count == 0)
				{
					// if any group that the user belongs to doesn't have a list of specific companies it is limited to,
					// then that user has access to all companies by virtue of that one group being unlimited.
					hasAllGroupCompanyMapping = true;
					break;
				}
			}

			if (hasAllocationRights == false)
			{
				return AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights;
			}

			if (hasAllGroupCompanyMapping)
			{
				return AllocationClass.UserAllocationStatus.HasGroupCompanyMappingToAll;
			}

			return AllocationClass.UserAllocationStatus.HasGroupMappingToSome;
		}

		private void Validate(SecurityClass security, AllocationClass allocation)
		{
			// Verify that the Effective and Expiration Date do not overlap
			// with another Allocation
			AllocationCollectionClass allocationCollection = this.EnumerateByCompanyMapGuid(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

			foreach (AllocationClass existingAllocation in allocationCollection)
			{
			    if (existingAllocation.IdentityGuid == allocation.IdentityGuid)
			    {
			        continue;
			    }

			    if (allocation._EffectiveDate.Value > allocation._ExpirationDate.Value)
			    {
			        throw new Exception("Effective Date must precede Expiration Date");
			    }

			    if (allocation._EffectiveDate.Value >= existingAllocation._EffectiveDate.Value
			        && allocation._EffectiveDate.Value < existingAllocation._ExpirationDate.Value)
			    {
			        throw new Exception("Allocation Overlaps with existing Allocation Effective = " + existingAllocation.EffectiveDate + " Expiration = " + existingAllocation.ExpirationDate);
			    }

			    if (allocation._ExpirationDate.Value > existingAllocation._EffectiveDate.Value
			        && (allocation._ExpirationDate.Value <= existingAllocation._ExpirationDate.Value
			            || allocation._EffectiveDate.Value < existingAllocation._EffectiveDate.Value))
			    {
			        throw new Exception("Allocation Overlaps with existing Allocation Effective = " + existingAllocation.EffectiveDate + " Expiration = " + existingAllocation.ExpirationDate);
			    }
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AllocationClass allocation)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (allocation == null)
		    {
		        throw new ArgumentNullException("allocation");
		    }

		    if (!security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
		    {
		        throw new FMInsufficientRightsException();
		    }

			AllocationClass.UserAllocationStatus userGroupAllocationStatus = this.UserHasAllocationRightsAndCompanyMapCollection(security);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
			{
				throw new FMInsufficientRightsException("Access Denied");
			}

			var companyMaps = new CompanyMapsClass();
			CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
			{
				List<GroupClass> userGroupList = this.GetUserGroups(security);
				bool canModifyAlloc = this.CanModifyAllocation(security, companyMap, allocation.CompanyMapType, userGroupList);

				if (canModifyAlloc == false)
				{
					throw new FMInsufficientRightsException("Access Denied");
				}
			}

			this.Validate(security, allocation);

		    if (this.GetIdentityGuid(
		        security,
		        allocation.CompanyMapGuid,
		        allocation._EffectiveDate.Value,
		        allocation._ExpirationDate.Value,
		        allocation.CompanyMapType) != Guid.Empty)
		    {
		        throw new Exception("Allocation Exists");
		    }

			allocation.SiteGuid = security.SiteGuid;
			allocation.CreatedDate = DateTimeOffset.Now;
			allocation.CreatedBy = security.UserID;
			allocation.UpdatedDate = allocation.CreatedDate;
			allocation.UpdatedBy = security.UserID;
			allocation.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				allocation.InsertSQL(cmd);
            this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

         this.UpdateLineItems(security, allocation);

			return allocation.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, AllocationClass allocation)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (allocation == null)
            {
                throw new ArgumentNullException("allocation");
            }

            if (!security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
                && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
            {
                throw new FMInsufficientRightsException();
            }

            AllocationClass.UserAllocationStatus userGroupAllocationStatus = this.UserHasAllocationRightsAndCompanyMapCollection(security);

            if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            var companyMaps = new CompanyMapsClass();
            CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

            if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
            {
                List<GroupClass> userGroupList = this.GetUserGroups(security);
                bool canModifyAlloc = this.CanModifyAllocation(security, companyMap, allocation.CompanyMapType, userGroupList);

                if (canModifyAlloc == false)
                {
                    throw new FMInsufficientRightsException("Access Denied");
                }
            }

            this.Validate(security, allocation);

            Guid identityGuid = this.GetIdentityGuid(security,
                                        allocation.CompanyMapGuid,
                                        allocation._EffectiveDate.Value,
                                        allocation._ExpirationDate.Value, allocation.CompanyMapType);

            if (identityGuid != Guid.Empty && identityGuid != allocation.IdentityGuid)
            {
                throw new Exception("Allocation Exists");
            }

            AllocationClass oldAllocation = this.Get(security, allocation.IdentityGuid, STATION_TYPE.MAX_STATION_TYPE, "");

            if (oldAllocation.IdentityGuid == Guid.Empty)
            {
                throw new Exception("Allocation Not Found");
            }

            companyMap = companyMaps.Get(security, oldAllocation.CompanyMapGuid, oldAllocation.CompanyMapType);

            if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
            {
                List<GroupClass> userGroupList = this.GetUserGroups(security);
                bool canModifyAlloc = this.CanModifyAllocation(security, companyMap, allocation.CompanyMapType, userGroupList);

                if (canModifyAlloc == false)
                {
                    throw new FMInsufficientRightsException("Access Denied");
                }
            }

            allocation.UpdatedDate = DateTimeOffset.Now;
            allocation.UpdatedBy = security.UserID;

            using (var cmd = new SqlCommand())
            {
                allocation.UpdateSQL(cmd);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }

            this.UpdateLineItems(security, allocation);
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (!security.HasRight(RIGHT.MODIFY_ALLOCATIONS) 
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
		        && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
		    {
		        throw new FMInsufficientRightsException();
		    }

			AllocationClass allocation = this.Get(security, identityGuid, STATION_TYPE.MAX_STATION_TYPE, "");
			
			if (allocation.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Allocation Not Found");
			}


			AllocationClass.UserAllocationStatus userGroupAllocationStatus = this.UserHasAllocationRightsAndCompanyMapCollection(security);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
			{
				throw new FMInsufficientRightsException("Access Denied");
			}

			var companyMaps = new CompanyMapsClass();
			CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
			{
				List<GroupClass> userGroupList = this.GetUserGroups(security);
				bool canModifyAlloc = this.CanModifyAllocation(security, companyMap, allocation.CompanyMapType, userGroupList);

				if (canModifyAlloc == false)
				{
					throw new FMInsufficientRightsException("Access Denied");
				}
			}

			allocation.LineItemCollection = null;

         this.UpdateLineItems(security, allocation);

			using (var cmd = new SqlCommand())
			{
				allocation.PurgeSQL(cmd);
            this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public AllocationClass Get(SecurityClass security, Guid identityGuid, STATION_TYPE stationType, string transactionID)
		{
			return this.GetBySiteGuid(security, identityGuid, security.SiteGuid, stationType, transactionID);
		}

        public AllocationClass GetBySiteGuid(SecurityClass security, Guid identityGuid, Guid siteGuid, STATION_TYPE stationType, string transactionID)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS)
                && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
            {
                throw new FMInsufficientRightsException();
            }

            var sites = new SitesClass();
            SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

            var allocation = new AllocationClass(site)
            {
                IdentityGuid = identityGuid
            };

            using (var cmd = new SqlCommand())
            {
                allocation.SelectSQL(cmd);
                allocation.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
            }

			AllocationClass.UserAllocationStatus userGroupAllocationStatus = this.UserHasAllocationRightsAndCompanyMapCollection(security);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
			{
				throw new FMInsufficientRightsException("Access Denied");
			}

			var companyMaps = new CompanyMapsClass();
			CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
			{
				List<GroupClass> userGroupList = this.GetUserGroups(security);
				bool canViewAlloc = this.CanViewAllocation(security, companyMap, allocation.CompanyMapType, userGroupList);

				if (canViewAlloc == false)
				{
					throw new FMInsufficientRightsException("Access Denied");
				}
			}

			allocation.ID = companyMap.AssignedToID + "->" + companyMap.AssignedID;
			
            var allocationLineItems = new AllocationLineItemsClass();
            allocation.LineItemCollection = allocationLineItems.EnumerateByAllocationGuid(security, allocation.IdentityGuid);
            DateTimeOffset expirationDate = allocation._ExpirationDate.Value;
            DateTimeOffset effectiveDate = allocation._EffectiveDate.Value;

            DateTimeOffset siteTimeToday = TimeConverter.Today(site);

            foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
            {
                //Get amounts loaded since last reset but excluding current period.
                lineItem.Loaded.Value = 0.0;
                DateTimeOffset nextResetDate;
                bool resetDateUpdated = false;

                for (DateTimeOffset resetDate = lineItem.ResetDate.Value;
                    resetDate < siteTimeToday && resetDate < expirationDate;
                    resetDate = nextResetDate
                    )
                {

                    switch (lineItem.ResetPeriod)
                    {
                        case ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD:
                            nextResetDate = resetDate.AddDays(lineItem.ResetMultiple);
                            break;
                        case ALLOCATION_RESET_PERIOD.WEEK_RESET_PERIOD:
                            nextResetDate = resetDate.AddDays(lineItem.ResetMultiple * 7);
                            break;
                        case ALLOCATION_RESET_PERIOD.MONTH_RESET_PERIOD:
                            nextResetDate = resetDate.AddMonths(lineItem.ResetMultiple);
                            break;
                        case ALLOCATION_RESET_PERIOD.YEAR_RESET_PERIOD:
                            nextResetDate = resetDate.AddYears(lineItem.ResetMultiple);
                            break;
                        default:
                            nextResetDate = expirationDate;
                            break;
                    }

                    if (nextResetDate > allocation.LastAllocationResetDate.Value)
                    {
                        lineItem.Loaded.Value = allocationLineItems.GetAmountLoaded(security,
                                                                                 allocation.ID,
                                                                                 lineItem.AssignedGuid,
                                                                                 lineItem.Type,
                                                                                 lineItem.ResetPeriod,
                                                                                 lineItem.ResetMultiple,
                                                                                 resetDate,
                                                                                 allocation.LastAllocationResetDate.Value,
                                                                                 expirationDate,
                                                                                 siteGuid,
                                                                                 stationType,
                                                                                 transactionID);
                        if (nextResetDate < siteTimeToday)
                        {
                            if (lineItem.SetResetDate(effectiveDate, expirationDate, nextResetDate))
                            {
                                resetDateUpdated = true;
                            }
                        }
                        else
                        {
                            if (lineItem.SetResetDate(effectiveDate, expirationDate, siteTimeToday))
                            {
                                resetDateUpdated = true;
                            }
                        }
                    }

                }
                if (resetDateUpdated)

                {
                    allocationLineItems.Modify(security, lineItem);
                }

                lineItem.Loaded.Value = allocationLineItems.GetAmountLoaded(security,
                                                                         allocation.ID,
                                                                         lineItem.AssignedGuid,
                                                                         lineItem.Type,
                                                                         lineItem.ResetPeriod,
                                                                         lineItem.ResetMultiple,
                                                                         lineItem.ResetDate.Value,
                                                                         allocation.LastAllocationResetDate.Value,
                                                                         expirationDate,
                                                                         siteGuid,
                                                                         stationType,
                                                                         transactionID);

				if (lineItem.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
				{
					CompanyMapClass OwnerManagerMap = new CompanyMapsClass().Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

					ProductMapCollectionClass UnavailableProductMapCollection = new ProductMapsClass().EnumerateByAssignedToGuidAndType(
								security, OwnerManagerMap.AssignedGuid, PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP);

					Guid companyGuid = new FieldLevelConfigMapsClass().GetRecordVersionGuid(security, "COMPANY", OwnerManagerMap.AssignedGuid, security.SiteGuid);

					Guid productGuid = new FieldLevelConfigMapsClass().GetRecordVersionGuid(security, "PRODUCT", lineItem.AssignedGuid, security.SiteGuid);

					//OwnerManagerMap: Assigned: company/owner; AssignedTo: Manager
					//UnavailableProductMapCollection: Assigned: product; AssignedTo: company/owner
					ProductMapClass UnavailableProductMap = UnavailableProductMapCollection.Find(x => x.AssignedToGuid == companyGuid && x.AssignedGuid == productGuid);

					// Force UserGuid to Guid.Empty all companies authorized
					//this.Security.UserGuid = Guid.Empty;

					// Get the ledger data
					var ledgerSR = new LedgerSR
					{
						Security = security,
						Site = security.SiteID,
						CurrentSiteGuid = security.SiteGuid
					};
					ledgerSR.SetRequestType(LedgerSR.LedgerRequests.Refresh);
					ledgerSR.Manager = OwnerManagerMap.AssignedToID;
					ledgerSR.Owner = OwnerManagerMap.AssignedID;
					ledgerSR.Product = lineItem.AssignedID;
					ledgerSR.Month = lineItem.ResetDate.Value.ToString("MMMM yyyy");
					ledgerSR.Units = QuantityDisplay.NET;
					ledgerSR.ShowCost = false;

					LedgerProcessorClass ledgerProcessor = new LedgerProcessorClass();
					LedgerDO ledgerDO = ledgerProcessor.Process(ledgerSR);

					var ledgerLineItemDO = ledgerDO.LedgerLineItems[lineItem.ResetDate.Value.Day - 1] as LedgerLineItemDO;
					lineItem.Limit.Value = ledgerLineItemDO.BookInventory.NetInventoryChange;
					lineItem.Limit.Value += lineItem.Loaded.Value;

					if (UnavailableProductMap != null)
					{
						lineItem.Limit.Value -= UnavailableProductMap._UnavailableInventoryNet.Value;
					}
				}
			}

            return allocation;
        }

        public AllocationClass GetByInventoryDate(SecurityClass security, Guid identityGuid, Guid siteGuid, STATION_TYPE stationType, string transactionID, DateTimeOffset transactionDate)
		{
			DateTimeOffset temporarydatetime;
			bool bFound = false;

		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) 
                && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
		        && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
                && !security.HasRight(RIGHT.MODIFY_DISPATCH)
		        && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
		    {
		        throw new FMInsufficientRightsException();
		    }

			var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			var allocation = new AllocationClass(site) { IdentityGuid = identityGuid };

		    using (var cmd = new SqlCommand())
			{
				allocation.SelectSQL(cmd);
				allocation.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			AllocationClass.UserAllocationStatus userGroupAllocationStatus = this.UserHasAllocationRightsAndCompanyMapCollection(security);

			if(userGroupAllocationStatus == AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
            {
				throw new FMInsufficientRightsException("Access Denied");
			}

			var companyMaps = new CompanyMapsClass();
			CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
            {
				List<GroupClass> userGroupList = this.GetUserGroups(security);
				bool canViewAlloc = this.CanViewAllocation(security, companyMap, allocation.CompanyMapType, userGroupList);

				if (canViewAlloc == false)
                {
					throw new FMInsufficientRightsException("Access Denied");
				}
            }

			allocation.ID = companyMap.AssignedToID + "->" + companyMap.AssignedID;
			var allocationLineItems = new AllocationLineItemsClass();
			allocation.LineItemCollection = allocationLineItems.EnumerateByAllocationGuid(security, allocation.IdentityGuid);

			foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
			{
				if (lineItem.ResetMethod != ALLOCATION_RESET_METHOD.REPEAT_METHOD)
            {
               continue;
            }

            if (lineItem.ResetPeriod == ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD)
				{
					lineItem.ResetDate.Value = transactionDate.AddDays(1);
				}
				else if (lineItem.ResetPeriod == ALLOCATION_RESET_PERIOD.WEEK_RESET_PERIOD)
				{
					// this is a little confusing but I think it needs to be set at Sunday
					temporarydatetime = transactionDate;
					while (temporarydatetime.DayOfWeek != DayOfWeek.Sunday)
					{
						temporarydatetime = temporarydatetime.AddDays(1);
					}
					lineItem.ResetDate.Value = temporarydatetime;
				}
				else if (lineItem.ResetPeriod == ALLOCATION_RESET_PERIOD.MONTH_RESET_PERIOD)
				{
					int iDaysToAdd = 0;
					int iDaysInMonth = 0;

					iDaysInMonth = DateTime.DaysInMonth(transactionDate.Year, transactionDate.Month);

					iDaysToAdd = iDaysInMonth - transactionDate.Day;

					temporarydatetime = transactionDate.AddDays(iDaysToAdd);

					lineItem.ResetDate.Value = temporarydatetime;
				}
				else if (lineItem.ResetPeriod == ALLOCATION_RESET_PERIOD.YEAR_RESET_PERIOD)
				{
					temporarydatetime = new DateTimeOffset(transactionDate.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
					lineItem.ResetDate.Value = temporarydatetime;
				}
				else
            {
               continue;
            }

            lineItem.Loaded.Value = allocationLineItems.GetAmountLoaded(security,
					allocation.ID,
					lineItem.AssignedGuid,
					lineItem.Type,
					lineItem.ResetPeriod,
					lineItem.ResetMultiple,
					lineItem.ResetDate.Value,
					allocation.LastAllocationResetDate.Value,
					allocation._ExpirationDate.Value,
					siteGuid,
					stationType,
					transactionID);
				bFound = true;
			}

         return bFound == false ? null : allocation;
      }

      public Guid GetIdentityGuid(SecurityClass security, Guid companyMapGuid, DateTimeOffset effectiveDate, DateTimeOffset expirationDate, COMPANY_MAP_TYPE companyMapType)
		{
			if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) 
                && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
		        && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) 
                && !security.HasRight(RIGHT.MODIFY_DISPATCH)
		        && !security.HasRight(RIGHT.VIEW_DISPATCH) 
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
		    {
		        throw new FMInsufficientRightsException();
		    }

		    var allocation = new AllocationClass
		                     {
		                         CompanyMapGuid = companyMapGuid,
		                         _EffectiveDate = { Value = effectiveDate },
		                         _ExpirationDate = { Value = expirationDate },
		                         CompanyMapType = companyMapType,
		                         SiteGuid = security.SiteGuid
		                     };

		    using (var cmd = new SqlCommand())
			{
				allocation.SelectByCompanyMapGuidAndDatesSQL(cmd, ContextUtil.IsInTransaction);
				allocation.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return allocation.IdentityGuid;
		}

	    public AllocationCollectionClass Enumerate(SecurityClass security)
	    {
	        if (security == null)
	        {
	            throw new ArgumentNullException("security");
	        }

	        if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) 
                && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
	            && !security.HasRight(RIGHT.MODIFY_PRODUCTS) 
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
	        {
	            throw new FMInsufficientRightsException();
	        }

	        var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			var allocation = new AllocationClass(site);

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				allocation.EnumerateSQL(cmd, security);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var allocationCollection = new AllocationCollectionClass();
			var companyMaps = new CompanyMapsClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				try
				{
					allocation = new AllocationClass(site);

					allocation.Load(set);
					CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);
					allocation.ID = companyMap.AssignedToID + "->" + companyMap.AssignedID;
					allocationCollection.Add(allocation);
				}
				finally
				{
					table.Rows.RemoveAt(0);
				}
			}

			return allocationCollection;
		}

	    public AllocationCollectionClass EnumerateByCompanyMapGuid(
	        SecurityClass security,
	        Guid companyMapGuid,
	        COMPANY_MAP_TYPE companyMapType)
	    {
	        if (security == null)
	        {
	            throw new ArgumentNullException("security");
	        }

	        if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) 
                && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
	            && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) 
                && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)) //this allows the user to remove a company from CompanyGroups with only MODIFY_COMPANY_DATA right
	        {
	            throw new FMInsufficientRightsException();
	        }


	        var sites = new SitesClass();
	        SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			this.userGroupList = this.GetUserGroups(security);

			var allocation = new AllocationClass(site)
			{
				CompanyMapGuid = companyMapGuid,
				CompanyMapType = companyMapType
			};



			DataSet set;

	        using (var cmd = new SqlCommand())
	        {
	            allocation.EnumerateByCompanyMapGuidSQL(cmd, security);
	            set = this.ConsolidatedDA.GetDataSet(cmd, security);
	        }

	        var allocationCollection = new AllocationCollectionClass();
	        var companyMaps = new CompanyMapsClass();

	        DataTable table = set.Tables[0];
	        while (table.Rows.Count != 0)
	        {
				try
				{
					allocation = new AllocationClass(site)
					{
						CompanyMapType = companyMapType
					};

					allocation.Load(set);
					CompanyMapClass companyMap = companyMaps.Get(
							 security,
							 allocation.CompanyMapGuid,
							 allocation.CompanyMapType);
					if (!this.CanViewAllocation(security, companyMap, allocation.CompanyMapType))
					{
						continue;
					}

					allocation.ID = companyMap.AssignedToID + "->" + companyMap.AssignedID;
					allocationCollection.Add(allocation);
				}
				finally
				{
					table.Rows.RemoveAt(0);
				}
	        }

	        return allocationCollection;
	    }

	    public AllocationCollectionClass EnumerateByCompanyMapType(SecurityClass security, COMPANY_MAP_TYPE type)
	    {
	        if (security == null)
	        {
	            throw new ArgumentNullException("security");
	        }

	        if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
	        {
	            throw new FMInsufficientRightsException();
	        }

			var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			var allocation = new AllocationClass(site);
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				allocation.EnumerateByCompanyMapTypeSQL(cmd, security, type);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var allocationCollection = new AllocationCollectionClass();
			var companyMaps = new CompanyMapsClass();

			AllocationClass.UserAllocationStatus userGroupAllocationStatus = this.UserHasAllocationRightsAndCompanyMapCollection(security);

			if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
			{
				throw new FMInsufficientRightsException("Access Denied");
			}

			List<GroupClass> userGroupList = this.GetUserGroups(security);
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				try
				{
					allocation = new AllocationClass(site)
					{
						CompanyMapType = type
					};

					allocation.Load(set);
					CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);

					if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
					{
						bool canViewAlloc = this.CanViewAllocation(security, companyMap, allocation.CompanyMapType, userGroupList);

						if (canViewAlloc == false)
						{
							continue;
						}
					}

					allocation.ID = companyMap.AssignedToID + "->" + companyMap.AssignedID;

					// Load the ID for the Allocation Group Guid
					var strings = new ApplicationStringsClass();
					ApplicationStringClass String = strings.Get(security, allocation.AllocationGroupGuid);
					allocation.AllocationGroupID = String.ID;

					allocationCollection.Add(allocation);
				}
				finally
				{
					table.Rows.RemoveAt(0);
				}
			}

			return allocationCollection;
		}

		public AllocationCollectionClass EnumerateByAllocationGroupGuid(SecurityClass security, Guid allocationGroupGuid)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) 
                && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
		    {
		        throw new FMInsufficientRightsException();
		    }

			var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			this.userGroupList = this.GetUserGroups(security);

			var allocation = new AllocationClass(site)
			{
				AllocationGroupGuid = allocationGroupGuid,
				SiteGuid = security.SiteGuid
			};
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				allocation.EnumerateByAllocationGroupGuidSQL(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var allocationCollection = new AllocationCollectionClass();
			var allocationLineItems = new AllocationLineItemsClass();

			var companyMaps = new CompanyMapsClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				try
				{
					allocation = new AllocationClass(site);

					allocation.Load(set);
					CompanyMapClass companyMap = companyMaps.Get(security, allocation.CompanyMapGuid, allocation.CompanyMapType);
					if (!this.CanViewAllocation(security, companyMap, allocation.CompanyMapType))
					{
						continue;
					}

					allocation.ID = companyMap.AssignedToID + "->" + companyMap.AssignedID;
					allocation.LineItemCollection = allocationLineItems.EnumerateByAllocationGuid(security, allocation.IdentityGuid);
					allocationCollection.Add(allocation);
				}
				finally
				{
					table.Rows.RemoveAt(0);
				}
			}

			return allocationCollection;
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (Object == null)
		    {
		        throw new ArgumentNullException("Object");
		    }
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (Object == null)
		    {
		        throw new ArgumentNullException("Object");
		    }

		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
		    if (security == null)
		    {
		        throw new ArgumentNullException("security");
		    }

		    if (Object == null)
		    {
		        throw new ArgumentNullException("Object");
		    }

			if (typeof(CompanyMapClass).IsInstanceOfType(Object))
			{
				var companyMap = (CompanyMapClass)Object;
				if (companyMap.Type == COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP
				|| companyMap.Type == COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP
				|| companyMap.Type == COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP
				|| companyMap.Type == COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
				{
					AllocationCollectionClass allocationCollection = this.EnumerateByCompanyMapGuid(security, companyMap.IdentityGuid, companyMap.Type);
				    foreach (AllocationClass allocation in allocationCollection)
				    {
                  this.Purge(security, allocation.IdentityGuid);
				    }
				}
				return;
			}

			if (typeof(ApplicationStringClass).IsInstanceOfType(Object))
			{
				var applicationString = (ApplicationStringClass)Object;
			    if (applicationString.Type != STRING_TYPE.ALLOCATION_GROUP)
			    {
			        return;
			    }

				AllocationCollectionClass allocationCollection = this.EnumerateByAllocationGroupGuid(security, applicationString.IdentityGuid);
				foreach (AllocationClass allocation in allocationCollection)
				{
					allocation.AllocationGroupGuid = Guid.Empty;
               this.Modify(security, allocation);
				}
				return;
			}

			if (typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				var entityToSiteMap = (EntityToSiteMapClass)Object;

				if ( entityToSiteMap.TypeID == ENTITY_TYPE.PRODUCT
				|| entityToSiteMap.TypeID == ENTITY_TYPE.PRODUCT_GROUP )
				{
					Guid siteGuid = security.SiteGuid;
					security.SiteGuid = entityToSiteMap.SiteGuid;
					AllocationCollectionClass allocationCollection = this.Enumerate(security);
					var allocationLineItems = new AllocationLineItemsClass();
					foreach (AllocationClass allocation in allocationCollection)
					{
						allocation.LineItemCollection = allocationLineItems.EnumerateByAllocationGuid(security, allocation.IdentityGuid);
						bool found = false;
						for (int index = 0; index < allocation.LineItemCollection.Count; index++)
						{
							var product = new ProductClass();
							var productGroup = new ProductGroupClass();

							AllocationLineItemClass lineItem = allocation.LineItemCollection.Item(index);
							if (lineItem.AssignedGuid == entityToSiteMap.IdentityGuid
							&& ((entityToSiteMap.TypeID == product.EntityType
							&& lineItem.Type == ALLOCATION_TYPE.PRODUCT_ALLOCATION)
							|| (entityToSiteMap.TypeID == productGroup.EntityType
							&& lineItem.Type == ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION)))
							{
								allocation.LineItemCollection.Remove(index);
								index--;
								found = true;
							}
						}

						if (found)
                  {
                     this.Modify(security, allocation);
                  }
               }
					security.SiteGuid = siteGuid;
				}
			}
		}
	}
}
