using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Data;
using System.Diagnostics;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class PersonnelProxy : IPersonnelProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;

        public PersonnelProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(PersonClass person)
        {
            throw new NotImplementedException();
        }

        public PersonCollectionClass Enumerate(bool hideHiddenPersonnel = false)
        {
            throw new NotImplementedException();
        }

        public PersonCollectionClass EnumerateBasicInformationOnly()
        {
            throw new NotImplementedException();
        }

        public PersonCollectionClass EnumerateByCompany(Guid companyGuid)
        {
            throw new NotImplementedException();
        }

        public PersonCollectionClass EnumerateByRole(PERSON_ROLE role, bool hideHiddenPersonnel = false)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateByRole1(PERSON_ROLE role)
        {
            throw new NotImplementedException();
        }

        public PersonCollectionClass EnumerateByRoleAndFilter(PERSON_ROLE role, string filter, string order, bool hideHiddenPersonnel = false)
        {
            throw new NotImplementedException();
        }

        public PersonCollectionClass EnumerateByRoleSortByName(PERSON_ROLE role)
        {
            throw new NotImplementedException();
        }

        public PersonCollectionClass EnumerateUndelegated()
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateUpdateVersions()
        {
            throw new NotImplementedException();
        }

        public PersonClass Get(Guid targetGuid)
        {
            throw new NotImplementedException();
        }

        public PersonClass GetBasicInfo(Guid personnelGuid, Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public PersonClass GetByID(string id)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
                    service => service.GetByID(currentSecurity, id));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public Guid GetGuidByCardNumber(string cardNumber)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuidByID(string ID)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuidByShortCardNumber(string shortCardNumber)
        {
            throw new NotImplementedException();
        }

        public string GetLatestRowVersionByRole(PERSON_ROLE role)
        {
            throw new NotImplementedException();
        }

        public Guid GetMasterRecordGuid(string id)
        {
            throw new NotImplementedException();
        }

        public string GetNextShortCardNumber()
        {
            throw new NotImplementedException();
        }

        public void Import(PersonClass person)
        {
            throw new NotImplementedException();
        }

        public void Modify(DATA_TYPE Type, PersonClass person)
        {
            throw new NotImplementedException();
        }

        public PersonClass PrepareForExport(PersonClass person)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid targetGuid)
        {
            throw new NotImplementedException();
        }
    }
}
