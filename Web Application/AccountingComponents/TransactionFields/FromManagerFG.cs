using System;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for FromManagerFG.
	/// </summary>
	public class FromManagerFG : ManagerFG
	{
		public FromManagerFG()
		{
			
		}

		public override string FieldID { get { return "FromManagerID"; } }
		public override bool Required { get { return true; } }

		//From fields stay on the 1st transaction, not the conjoined transaction;
		//Therefore there is no need to override the get and set methods for company code and id.
	}
}
