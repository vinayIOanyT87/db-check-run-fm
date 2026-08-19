using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FuelsManager.Accounting
{
    [RoutePrefix("api/AccountingOperations")]
    public class AccountingOperationsController : ApiController
    {
        private static List<MoveOverTransactionsToCompleteStatus> RunningTasks =
            new List<MoveOverTransactionsToCompleteStatus>();

        // GET api/<controller>
        [Route("TransactionCountToUpdateToEnterprise")]
        [HttpGet]
        public int TransactionCountToUpdate(string product, string manager, string stop, string start = "")
        {
            var session = HttpContext.Current.Session;
            var security = session["security"] as SecurityClass;

            var parsed = ParseHttpArguments(product, manager, stop, start, security);

            return FMChannelHelper.MakeCall<IPostToEnterpriseProcessor, int>(
                x => x.TransactionCountToUpdate(security, parsed.ProductGuid, parsed.ManagerGuid,
                    parsed.ParsedStop, parsed.ParsedStart));
        }

        // GET api/<controller>
        [Route("CheckUpdateTransactionsToEnterprise")]
        [HttpGet]
        public PostTransactionsToEnterpriseResponse TransactionCountToUpdate(Guid jobID)
        {
            var session = HttpContext.Current.Session;
            var security = session["security"] as SecurityClass;
            var job = RunningTasks.SingleOrDefault(x => x.ID == jobID);
            if (job == null)
            {
                throw new ArgumentException("Job id is incorrect");
            }

            if (job.Worker.IsCompleted)
            {
                RunningTasks.Remove(job);
            }
            return new PostTransactionsToEnterpriseResponse()
            {
               JobIdentifier = job.ID,
               TotalTransactionsToUpdate = job.ToComplete,
               Complete = job.Worker.IsCompleted,
               TotalTransactionsUpdated = job.TransactionsMoved
            };
        }

        [Route("StartUpdateTransactionsToEnterprise")]
        [HttpPost]
        public PostTransactionsToEnterpriseResponse StartPostTransactionsToEnterprise(string product, string manager, string stop, string start = "")
        {
            var session = HttpContext.Current.Session;
            var security = session["security"] as SecurityClass;

            var parsed = ParseHttpArguments(product, manager, stop, start, security);

            var job = new MoveOverTransactionsToCompleteStatus
            {
                ToComplete = FMChannelHelper.MakeCall<IPostToEnterpriseProcessor, int>(
                x => x.TransactionCountToUpdate(security, parsed.ProductGuid, parsed.ManagerGuid,
                    parsed.ParsedStop, parsed.ParsedStart))
            };
            RunningTasks.Add(job);
            job.Worker = Task.Run(() => LongRunningUpdateTransactionsToEnterprise(security, parsed, job.ID));
            var result = new PostTransactionsToEnterpriseResponse()
                        {
                            JobIdentifier = job.ID,
                            TotalTransactionsToUpdate = job.ToComplete,
                            Complete = job.Worker.IsCompleted,
                            TotalTransactionsUpdated = job.TransactionsMoved
                        };
            return result;
        }

        private void LongRunningUpdateTransactionsToEnterprise(SecurityClass security, ParsedHttpArgumentsDTO parsed, Guid ID)
        {
            try
            {
                int transactionsUpdated = 0;
                var job = RunningTasks.Single(x => x.ID == ID);
                if (job.ToComplete == 0)
                {
                    return;
                }
                do
                {
                    transactionsUpdated = FMChannelHelper.MakeCall<IPostToEnterpriseProcessor, int>(
                        x => x.PostTransactionsToEnterprise(
                            security,
                            parsed.ProductGuid,
                            parsed.ManagerGuid,
                            parsed.ParsedStop,
                            parsed.ParsedStart,
                            true));
                    job.TransactionsMoved += transactionsUpdated;
                } while (transactionsUpdated > 0);
            }
            catch (Exception)
            { /* swallow the exception and give up to avoid crashing the app pool 
                since this running process is not associated with a request */}
        }

        private ParsedHttpArgumentsDTO ParseHttpArguments(string product, string manager, string stop, string start, SecurityClass security)
        {
            product = HttpUtility.UrlDecode(product);
            manager = HttpUtility.UrlDecode(manager);
            start = HttpUtility.UrlDecode(start);
            stop = HttpUtility.UrlDecode(stop);

            var results = new ParsedHttpArgumentsDTO();

            results.ProductGuid = Guid.Empty;
            if (product != "{All}" && !string.IsNullOrWhiteSpace(product))
            {
                var allProducts =
                    FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(security));
                var selectedProduct = allProducts.SingleOrDefault(x => x.ID == product);
                if (selectedProduct == null)
                {
                    throw new ArgumentException("Could not find the correct product");
                }
                results.ProductGuid = selectedProduct.MasterRecordGuid;
            }

            var managers = FMChannelHelper.MakeCall<ICompanies, List<CompanyClass>>(
                    x => x.EnumerateByRole(security, COMPANY_ROLE.MANAGER, true, true));
            var selectedManager = managers.SingleOrDefault(x => x.ID == manager);
            if (selectedManager == null)
            {
                throw new ArgumentException("Could not find the correct manager");
            }
            results.ManagerGuid = selectedManager.IdentityGuid;
            var parsedStop = DateTime.Now;
            if (!DateTime.TryParse(stop, out parsedStop))
            {
                throw new ArgumentException("Stop time is not valid");
            }
            results.ParsedStop = parsedStop;

            results.ParsedStart = null;
            var tempParsedStart = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(start))
            {
                if (!DateTime.TryParse(start, out tempParsedStart))
                {
                    throw new ArgumentException("Start time is not valid");
                }
                else
                {
                    results.ParsedStart = tempParsedStart;
                }
            }

            return results;
        }

        private class MoveOverTransactionsToCompleteStatus
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public int TransactionsMoved { get; set; } = 0;
            public int ToComplete { get; set; } = 0;
            public Task Worker { get; set; }
        }

        private class ParsedHttpArgumentsDTO
        {
            public Guid ProductGuid { get; set; }
            public Guid ManagerGuid { get; set; }
            public DateTime ParsedStop { get; set; }
            public DateTime? ParsedStart { get; set; }
        }

        public class PostTransactionsToEnterpriseResponse
        {
            public Guid JobIdentifier { get; set; }
            public int TotalTransactionsToUpdate { get; set; }
            public int TotalTransactionsUpdated { get; set; }
            public bool Complete { get; set; }
        }

    }
}
