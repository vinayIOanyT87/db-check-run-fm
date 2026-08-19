
namespace FMBusinessServices.InternalClasses
{
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;
	using Microsoft.SqlServer.Server;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	public class ExStarsConfigDAL
	{
		/// <summary>
		/// Allows access to the database
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public ExStarsConfigDAL()
		{
		}


	}
}