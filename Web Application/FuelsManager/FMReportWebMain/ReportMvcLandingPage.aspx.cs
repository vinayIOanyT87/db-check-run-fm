namespace FuelsManager.FMReportWebMain
{
	using global::FuelsManager.FMReportWebMain;

	public partial class ReportMvcLandingPage : ReportLandingPage
	{
		// The MVC landing page inherits from the Report Landing Page.
		// The reason is that we do not want the main FuelsManager menu
		// be displayed in the IFrame that the asp page is rendered in.

		// The Page_Load method is in the base class.
	}
}