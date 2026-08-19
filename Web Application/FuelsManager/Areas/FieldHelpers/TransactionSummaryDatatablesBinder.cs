namespace FuelsManager.Areas.FieldHelpers
{
    using System.Collections.Specialized;
    using System.Web.Mvc;

    using DataTables.Mvc;

    /// <summary>
    /// A custom binder to handle datatables requests from the Transaction Summary screen.
    /// Bind datatables request values and extra values specific to the Transaction Summary screen that are sent along with the request
    /// </summary>
    public class TransactionSummaryDatatablesBinder : DataTablesBinder
    {
        /// <summary>
        /// Binds a new model with the DataTables request parameters and the additional request values for the Transaction Summary Screen
        /// </summary>
        /// <param name="controllerContext">The context for the controller.</param>
        /// <param name="bindingContext">The context for the binding.</param>
        /// <returns>A populated TransactionSummaryDatatablesRequest model </returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return this.Bind(controllerContext, bindingContext, typeof(TransactionSummaryDatatablesRequest));
        }

        /// <summary>
        /// Map aditional properties sent along with the datatables request for the Transaction Summary Screen
        /// </summary>
        /// <param name="requestModel">The request model which will receive the additional fields</param>
        /// <param name="requestParameters">Parameters sent with the request.</param>
        protected override void MapAditionalProperties(IDataTablesRequest requestModel, NameValueCollection requestParameters)
        {
            var transactionSummaryRequestModel = (TransactionSummaryDatatablesRequest)requestModel;
            transactionSummaryRequestModel.BeginDate = this.Get<string>(requestParameters, "BeginDate");
            transactionSummaryRequestModel.EndDate = this.Get<string>(requestParameters, "EndDate");
            transactionSummaryRequestModel.AliasName = this.Get<string>(requestParameters, "AliasName");
            transactionSummaryRequestModel.ShortDatePattern = this.Get<string>(requestParameters, "ShortDatePattern");
            transactionSummaryRequestModel.TimePattern = this.Get<string>(requestParameters, "TimePattern");
        }
    }
}