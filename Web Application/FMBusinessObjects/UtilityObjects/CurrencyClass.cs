using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ChannelFactories;

namespace FMBusinessObjects.UtilityObjects
{
	public class CurrencyClass
	{
		#region Private variables
		private const string CLIENT_SIDE_SCRIPT =
			  @"	function calculatePrice(nonDomesticPrice, exchangeRate)
					{
						if (exchangeRate == 0 )
							return nonDomesticPrice;
						return nonDomesticPrice/exchangeRate;
					}";

		private Hashtable rates = new Hashtable();
		private double nonDomesticPrice = 0.0;
		private double price = 0.0;
		private Guid currencyGuid = Guid.Empty;
		private DateTime inventoryDate = DateTime.Today;
		private SecurityClass security = null;
		#endregion

		#region Properties
		public double NonDomesticPrice
		{
			set { nonDomesticPrice = value; }
			get { return nonDomesticPrice; }
		}

		public double Price
		{
			set { price = value; }
			get { return (currencyGuid != Guid.Empty) ? nonDomesticPrice / ExchangeRate : price; }
		}

		public Guid CurrencyGuid
		{
			set { currencyGuid = value; }
			get { return currencyGuid; }
		}

		public double ExchangeRate
		{
			get { return GetRate(currencyGuid); }
		}

		public string ClientSideScript
		{
			get { return CLIENT_SIDE_SCRIPT; }
		}

		public DateTime InventoryDate
		{
			set { inventoryDate = value; BuildRates(); }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Currency BLL class.
		/// </summary>
		public CurrencyClass(SecurityClass inSecurity)
		{
			this.security = inSecurity;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Returns the exchange rate for a currency unit represented by the guid from table tblcurrencies.
		/// </summary>
		/// <param name="currencyGuid">Currency Guid value from table tblcurrency for the requested exchange rate.</param>
		/// <returns>Exchange rate as string.</returns>
		public double GetRate(Guid currencyGuid)
		{
			string rateStr = rates[currencyGuid.ToString()] as string;

			if (rateStr == null)
			{
				return 1.0;

			}
			return Double.Parse(rateStr);
		}

		/// <summary>
		/// Retrieves unit and their exchange rates as of current date and time and
		/// returns them as key value pair strings.
		/// </summary>
		public void BuildRates()
		{
			FMChannelHelper.MakeCall<ICurrencies>(this.BuildRates);
		}

		/// <summary>
		/// Retrieves unit and their exchange rates as of current date and time and
		/// returns them as key value pair strings.
		/// </summary>
		/// <param name="currencySevice">The currency Sevice.</param>
		private void BuildRates( ICurrencies currencySevice )
		{
			CurrencyDOCollectionClass currencies = currencySevice.GetCurrencies(this.security);
			rates = new Hashtable(currencies.Count);

			// Iterate through the context's currency collection
			foreach (CurrencyDO currency in currencies)
			{
				CurrencyDO currencyWithItems = currencySevice.Get(this.security, currency.IdentityGuid);
				CurrencyLineItemDO asOfDateLineItem = null;

				foreach (CurrencyLineItemDO currencyLineItem in currencyWithItems.LineItems)
				{
					if ((currencyLineItem.EffectiveDate <= inventoryDate) &&
						(asOfDateLineItem == null || currencyLineItem.EffectiveDate >= asOfDateLineItem.EffectiveDate))
					{
						asOfDateLineItem = currencyLineItem;
					}
				}
				if (asOfDateLineItem != null)
				{
					rates.Add(currency.IdentityGuid.ToString(), asOfDateLineItem.Rate.ToString());
				}
			}
		}
		#endregion
	}
}
