using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessServices.ServiceClasses
{
    using System.Reflection;

    using FMBusinessObjects.ChannelFactories;

    public interface IPriceCalculatorDiscovery
	{
		bool Calculate(	SecurityClass inSecurity,
						TransactionDO inTrans,
						List<LineItemDO> inOrigLineItems,
						bool bForceRecalculation);

		TransactionDO GetTransactionDO();
	}

	public class PriceCalculatorInvokerClass : IPriceCalculatorInvoker
	{
		#region Attributes
		private const string ERR_MSG_001 = "Null Security Object Reference";
		private const string ERR_MSG_002 = "Null Transaction Object Reference";
		#endregion

		static Assembly dll = null;

		#region Construction
		public PriceCalculatorInvokerClass ( )
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will invoke the Price Calculator with the newly updated (from screen) data and
		/// a comparison set of old data.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="trans"></param>
		/// <param name="origLineItems"></param>
		public TransactionDO CalculateWithLineItems ( SecurityClass security, TransactionDO trans, List<LineItemDO> origLineItems )
		{
			return this.StartCalculate ( security, trans, origLineItems, true );
		}

		/// <summary>
		/// This method will invoke the Price Calculator with the transaction DO information.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="trans"></param>
		public TransactionDO Calculate ( SecurityClass security, TransactionDO trans )
		{
			return this.StartCalculate ( security, trans, null, true );
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will return null if there isn't a price calculator assembly name.
		/// Otherwise, it returns the assembly name from the App_FMBusinessService configuration
		/// file.
		/// </summary>
		/// <returns></returns>
		private string GetPriceCalculatorAssemblyPath ( )
		{
			return AppSettingsHelper.GetKeyValue<string>("PriceCalculator", null);
		}

		/// <summary>
		/// This method will actually call the Price Calculator to perform the calculations.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="trans"></param>
		/// <param name="origLineItems"></param>
		/// <param name="forceRecal"></param>
		private TransactionDO StartCalculate ( SecurityClass security, TransactionDO trans, List<LineItemDO> origLineItems, bool forceRecal )
		{
			TransactionDO transDO = null;

			try
			{
				if (security == null)
				{
					throw new NullReferenceException ( PriceCalculatorInvokerClass.ERR_MSG_001 );
				}

				if (trans == null)
				{
					throw new NullReferenceException ( PriceCalculatorInvokerClass.ERR_MSG_002 );
				}

				if (trans.LineItems == null || trans.LineItems.Count == 0)
				{
					return transDO;
				}

				string assemblyName = this.GetPriceCalculatorAssemblyPath ( );

				if (string.IsNullOrEmpty ( assemblyName ) == true)
				{
					return transDO;
				}

				if (assemblyName.Equals("ADFPriceCalculator") == true)
				{
					// Create ADF price calculator
					ADFPriceCalculatorClass priceCalculator = new ADFPriceCalculatorClass ( );
					priceCalculator.Calculate ( security, trans, origLineItems, forceRecal );
					transDO = priceCalculator.TransDO;
				}

				return transDO;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// This method will find and load the price calculator if found.  Otherwise,
		/// it return null.
		/// </summary>
		/// <param name="security">Security object.</param>
		/// <returns>Price calculator assembly if found, otherwise it returns null.</returns>
		private IPriceCalculatorDiscovery LoadPriceCalculatorAssembly(SecurityClass security)
		{
			string assemblyName =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					configSettings => configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_PriceCalculatorInterface));

			if (string.IsNullOrEmpty(assemblyName) == false)
			{
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

				if (dll == null)
				{
					try
					{
						dll = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
					}
					catch
					{
						dll = Assembly.Load(assemblyName);
					}
				}

				if (dll != null)
				{

					Type[] types = dll.GetTypes();

					foreach (Type module in types)
					{
						if (!module.IsClass)
						{
							continue;
						}

						Type priceCalculatorInterface = module.GetInterface("IPriceCalculatorDiscovery");

						if (priceCalculatorInterface == null)
						{
							continue;
						}

						object engine = Activator.CreateInstance(module);
						var priceCalculator = (IPriceCalculatorDiscovery) engine;

						return priceCalculator;
					}
				}
			}

			return null;
		}
		#endregion
	}
}