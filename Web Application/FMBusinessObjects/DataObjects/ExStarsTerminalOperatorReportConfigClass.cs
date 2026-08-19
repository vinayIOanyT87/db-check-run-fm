namespace FMBusinessObjects.DataObjects
{
	using System;

	public class ExStarsTerminalOperatorReportConfigClass
	{
		//Reporting State Value
		// use ExStarsSiteConfigExpanded.Site.State
		//public string ReportingState { get; set; }

		//Sequence Error Number
		public bool SEIN_RTTI_Required { get; set; }
		public string SEIN_RTTI_REF02_Value { get; set; }

		//No Business Activity
		public bool NoBusinessActivity { get; set; }

		//Ending Inventory Date
		public DateTime EndingInventoryDate { get { return this.endingInventoryDate; } }

		//Product Code
		public Guid ProductGuid { get; set; }

		//Sequence Error Number (Ending Inventory Loop by Product Code)
		public bool SEIN_EILBPC_Required { get; set; }
		public string SEIN_EILBPC_REF02_Value { get; set; }

		protected DateTime endingInventoryDate;

		public ExStarsTerminalOperatorReportConfigClass(DateTime endingInventoryDate)
		{
			this.NoBusinessActivity = true;
			this.endingInventoryDate = endingInventoryDate;
			this.SEIN_EILBPC_Required = false;
			this.SEIN_EILBPC_REF02_Value = "";
			this.SEIN_RTTI_Required = false;
			this.SEIN_RTTI_REF02_Value = "";
		}
	}

}
