namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Linq;
	using System.Text;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	[Serializable]
	public abstract class ExStarsTransactionReportLoopBase : ExStarsReportsBase
	{
		// For a session of creating an EDI report there should be only a single set of manager totals 
		// which is external to this class
		public ExStarsManagerTotals AllMgrTotals { get; protected set; }

		/// <summary>
		/// Required for serialization, do not use
		/// </summary>
		public ExStarsTransactionReportLoopBase() : base() { }

		/// <summary>
		/// Standard constructor
		/// </summary>	
		/// <param name="config"></param>
		/// <param name="description"></param>
		/// <param name="managerTotals"></param>
		/// <param name="validationErrors"></param>
		protected ExStarsTransactionReportLoopBase(ExStarsSiteConfigExpanded config, string description, ExStarsManagerTotals managerTotals, ref string validationErrors)
			: base(config, description, ref validationErrors)
		{
			this.AllMgrTotals = managerTotals;
		}

		public void Reset()
		{
			AllMgrTotals.Reset();
		}
	}
}