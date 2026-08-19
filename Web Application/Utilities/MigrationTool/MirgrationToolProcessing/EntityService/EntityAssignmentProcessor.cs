namespace MirgrationToolProcessing.EntityService
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using System;

    public class EntityAssignmentProcessor
    {
        #region Data members
        private const string DuplicateIdendifierMessage = "Operation would result in duplicate identifiers.";
        #endregion

        #region Constructors
        /// <summary>
        /// This method is the default constructor.
        /// </summary>
        public EntityAssignmentProcessor(SecurityHandler securityHandler)
        {
            this.Init();
            this.SecurityHndlr = securityHandler;
        }
        #endregion

        #region Properties
        public SecurityHandler SecurityHndlr { get; set; }
        public bool MessageFlag { get; set; }
        public string Message { get; set; }
        #endregion

        #region Public methods
        public void PerformEntityAssignment(Guid identityGuid, Guid assignedFromSiteGuid, Guid assignedToSiteGuid, Guid entityGuid, ENTITY_TYPE entityType)
        {
            // Assign entity to a site.
            var entityToSite = new EntityToSiteMapClass
            {
                SiteGuid = assignedToSiteGuid,
                AssignedFromSiteGuid = assignedFromSiteGuid,
                IdentityGuid = identityGuid,
                TypeID = entityType,
                IsAssigned = true
            };

            try
            {
                FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(this.SecurityHndlr.Security, entityToSite, entityGuid));
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals(DuplicateIdendifierMessage) == false)
                {
                    this.MessageFlag = true;
                    this.Message = "Error: Performing entity assignment. " + ex.Message + ". ";
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.Message = string.Empty;
            this.MessageFlag = false;
            this.SecurityHndlr = null;
        }
        #endregion
    }
}
