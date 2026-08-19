using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for UnloadPopulater.
	/// </summary>
	public class UnloadPopulater : TransactionPopulater
	{
		public UnloadPopulater()
		{
			
		}

		#region Overrides
		protected override string TransactionTypeID
		{
			get
			{
				return "Unload";
			}
		}

		protected override void Populate()
		{

		}
		protected override void PopulateLineItem()
		{

		}

		#endregion Overrides
	}
}
