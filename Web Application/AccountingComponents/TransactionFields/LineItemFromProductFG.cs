using System;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemFromProductFG.
	/// </summary>
	public class LineItemFromProductFG : LineItemProductFG
	{
		public LineItemFromProductFG()
		{
			
		}

		public override string FieldID { get { return "LineItem FromProduct"; } }
		public override bool Required { get { return true; } }

	}
}
