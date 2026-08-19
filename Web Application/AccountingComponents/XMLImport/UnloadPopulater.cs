using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
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
		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T10_Unload;
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
