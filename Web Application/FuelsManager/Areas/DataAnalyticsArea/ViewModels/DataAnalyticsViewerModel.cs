namespace FuelsManager.Areas.DataAnalyticsArea.ViewModels
{
    /// <summary>
    /// The model for the Data Analytics Viewer Page
    /// </summary>
    public class DataAnalyticsViewerModel
    {
        /// <summary>
        /// The first part of the URL we will use to display a dashboard or worksheet. 
        /// E.g. http://10.33.19.177
        /// </summary>
        public string DataAnalyticsServerUrl { get; set; }

        /// <summary>
        /// The second part of the URL we will use to display a dashboard or worksheet
        /// E.g. /views/Aviation/MainDashboard
        /// </summary>
        public string MainWorksheetUrl { get; set; }
    }
}