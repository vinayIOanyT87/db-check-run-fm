namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.EnterpriseServices;
	using System.Web;
	using System.Web.Services;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using TransactionFields;

	public partial class TransactionDetailBase
	{
		private static void CheckWebMethodSecurity(string token)
		{
			CheckWebMethodSecurity(null, token);
		}

		private static void CheckWebMethodSecurity(SecurityClass security, string token)
		{
			if (security == null)
			{
				security = (SecurityClass)HttpContext.Current.Session["Security"];
			}

			var tokenGuid = new Guid(token);

			if (security == null || security.Token != tokenGuid)
			{
				security = FMChannelHelper.MakeCall<ISites, SecurityClass>(x => x.GetSecurity(token));
			}

			if (security.Token != tokenGuid)
			{
				throw new FMInsufficientRightsException();
			}
		}

		[WebMethod(EnableSession = true, TransactionOption = TransactionOption.NotSupported)]
		public static List<string> GetCompaniesAutoComplete(string token, int maxRows, string startsWith, string fieldKey, string dependFieldVal)
		{
			try
			{
				CheckWebMethodSecurity(token);

				var transContext = HttpContext.Current.Session["TransactionDetail.TransactionContext"] as TransactionContext;
				var trans = HttpContext.Current.Session[TransKey] as TransactionDO;

				var generator = new TransactionFieldGenerator(transContext, trans);

				// Get the field generator so we can ask for its entries
				var fieldGenerator = generator.GetFieldGenerator(fieldKey) as CompanyTextButtonGenerator;

				if (fieldGenerator == null)
				{
					throw new Exception("Could not find field generator (" + fieldKey + ")");
				}

				fieldGenerator.SetTransactionContext(transContext);
				fieldGenerator.SetTransaction(trans);
				List<string> companyList = fieldGenerator.GetBaseEntries(startsWith, maxRows);

				// If no entries, give a blank entry as feedback that communication is occuring correctly.
				if (companyList.Count == 0)
				{
					companyList.Add(string.Empty);
				}

				return companyList;
			}
			catch (Exception except)
			{
				LogErrorMessage(except.Message);
				throw;
			}
		}

		[WebMethod(EnableSession = true, TransactionOption = TransactionOption.NotSupported)]
		public static List<string> GetFuelCardAutoComplete(string token, int maxRows, string startsWith, string fieldKey, string dependFieldVal)
		{
			try
			{
				CheckWebMethodSecurity(token);

				var transContext = HttpContext.Current.Session["TransactionDetail.TransactionContext"] as TransactionContext;
				var trans = HttpContext.Current.Session[TransKey] as TransactionDO;

				var generator = new TransactionFieldGenerator(transContext, trans);

				// Get the field generator so we can ask for its entries
				var fieldGenerator = generator.GetFieldGenerator(fieldKey) as FuelCardFG;

				if (fieldGenerator == null)
				{
					throw new Exception("Could not find field generator (" + fieldKey + ")");
				}

				fieldGenerator.SetTransactionContext(transContext);
				fieldGenerator.SetTransaction(trans);
				List<string> cardList = fieldGenerator.GetBaseEntries(startsWith, maxRows);

				// If no entries, give a blank entry as feedback that communication is occuring correctly.
				if (cardList.Count == 0)
				{
					cardList.Add(string.Empty);
				}

				return cardList;
			}
			catch (Exception except)
			{
				LogErrorMessage(except.Message);
				throw;
			}
		}

		[WebMethod(EnableSession = true, TransactionOption = TransactionOption.NotSupported)]
		public static List<string> GetProductsAutoComplete(	string token, 
															int maxRows, 
															string startsWith, 
															string fieldKey, 
															string dependFieldVal, 
															string lineItemID)
		{
			var productList = new List<string>();

			try
			{
				CheckWebMethodSecurity(token);

				var transContext = HttpContext.Current.Session["TransactionDetail.TransactionContext"] as TransactionContext;
				var trans = HttpContext.Current.Session[TransKey] as TransactionDO;

				if (trans != null)
				{
					var lineItem = trans.FindLineItem(lineItemID);

					var generator = new TransactionFieldGenerator(transContext, trans);

					// Get the field generator so we can ask for its entries
					var fieldGenerator = generator.GetFieldGenerator(fieldKey) as ProductTextButtonGenerator;

					if (fieldGenerator == null)
					{
						throw new Exception("Could not find field generator (" + fieldKey + ")");
					}

					fieldGenerator.LineItem = lineItem;
					fieldGenerator.SetTransactionContext(transContext);
					fieldGenerator.SetTransaction(trans);
					productList = fieldGenerator.GetBaseEntries(startsWith, maxRows);

					// If no entries, give a blank entry as feedback that communication is occuring correctly.
					if (productList.Count == 0)
					{
						productList.Add(string.Empty);
					}

					return productList;
				}
			}
			catch (Exception except)
			{
				LogErrorMessage(except.Message);
				throw;
			}

			return productList;
		}

		/// <summary>
		/// This method handles the auto complete event for the Equipment fields
		/// (DestinationEquipmentID1, DestinationEquipmentID2, DestinationEquipmentID3,
		///  SoruceEquipmentID1, SoruceEquipmentID2, and SoruceEquipmentID3).
		/// </summary>
		/// <param name="token">This is the security token.</param>
		/// <param name="maxRows">The max rows to be returned.</param>
		/// <param name="startsWith">Search the results that start with a particular string.</param>
		/// <param name="fieldKey">The field generator.</param>
		/// <param name="dependFieldVal">The dependent field value.</param>
		/// <param name="lineItemID">Line item ID.</param>
		/// <returns>Returns a list of equipment.</returns>

		[WebMethod(EnableSession = true, TransactionOption = TransactionOption.NotSupported)]
		public static List<string> GetEquipmentAutoComplete(string token, 
															int maxRows, 
															string startsWith, 
															string fieldKey, 
															string dependFieldVal, 
															string lineItemID)
		{
			try
			{
				CheckWebMethodSecurity(token);

				var transContext = HttpContext.Current.Session["TransactionDetail.TransactionContext"] as TransactionContext;
				var trans = HttpContext.Current.Session[TransKey] as TransactionDO;

				LineItemDO lineItem = null;
				if (lineItemID != "na")
				{
					if (trans != null)
					{
						lineItem = trans.FindLineItem(lineItemID);
					}
				}

				var generator = new TransactionFieldGenerator(transContext, trans);

				// Get the field generator so we can ask for its entries
				var fieldGenerator = generator.GetFieldGenerator(fieldKey) as EquipmentTextButtonGenerator;

				if (lineItem != null)
				{
					if (fieldGenerator != null)
					{
						fieldGenerator.LineItem = lineItem;
					}
				}

				if (fieldGenerator == null)
				{
					throw new Exception("Could not find field generator (" + fieldKey + ")");
				}

				fieldGenerator.SetTransactionContext(transContext);
				fieldGenerator.SetTransaction(trans);

				List<string> equipmentList = fieldGenerator.GetBaseEntries(startsWith, maxRows);

				// If no entries, give a blank entry as feedback that communication is occuring correctly.
				if (equipmentList.Count == 0)
				{
					equipmentList.Add(string.Empty);
				}
				
				return equipmentList;
			}
			catch (Exception except)
			{
				LogErrorMessage(except.Message);
				throw;
			}
		}

		[WebMethod(EnableSession = true, TransactionOption = TransactionOption.NotSupported)]
		public static List<string> GetOperatorAutoComplete(	string token, 
															int maxRows, 
															string startsWith, 
															string fieldKey, 
															string dependFieldVal)
		{
			try
			{
				var security = (SecurityClass)HttpContext.Current.Session["Security"];

				CheckWebMethodSecurity(security, token);

				var persons = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>( personnel => personnel.Enumerate( security, hideHiddenPersonnel: true ) );

				var personList = new List<string>();

				for (int index = 0, count = 0; index < persons.Count && count < maxRows; ++index)
				{
					var person = persons[index];

                    if (person.ID.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
					{
						personList.Add(person.ID);
						++count;
					}
				}

				// If no entries, give a blank entry as feedback that communication is occuring correctly.
				if (personList.Count == 0)
				{
					personList.Add(string.Empty);
				}

				return personList;
			}
			catch (Exception except)
			{
				LogErrorMessage(except.Message);
				throw;
			}
		}
	}
}
