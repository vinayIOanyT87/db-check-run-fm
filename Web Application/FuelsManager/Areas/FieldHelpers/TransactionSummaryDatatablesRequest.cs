namespace FuelsManager.Areas.FieldHelpers
{
    using DataTables.Mvc;

    /// <summary>
    /// The request sent by datatables to get data to display on the Transaction Summary screen.
    /// </summary>
    public class TransactionSummaryDatatablesRequest : DefaultDataTablesRequest
    {
        /// <summary>
        /// The beginning inventory date provided by the user
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// The ending inventory date provided by the user
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// The alias name provided by the user
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// The short date pattern used by the site.
        /// </summary>
        public string ShortDatePattern { get; set; }

        /// <summary>
        /// The time pattern used by the site.
        /// </summary>
        public string TimePattern { get; set; }
    }
}