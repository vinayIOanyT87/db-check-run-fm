using System;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for FromOwnerFG.
	/// </summary>
	public class FromOwnerFG : OwnerFG
	{
		public FromOwnerFG()
		{

		}

		public override string FieldID { get { return "FromOwnerID"; } }
		public override bool Required { get { return true; } }

	}
}
