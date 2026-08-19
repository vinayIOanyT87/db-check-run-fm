using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.Interfaces
{
	public interface IPaymentInfo
	{
		string BillTo { get; set; }
		string CashAmount { get; set; }
		string CashCurrencyType { get; set; }
		string CreditCardAmount { get; set; }
		string CreditCardCurrencyType { get; set; }
		string CreditCardName { get; set; }
		string CreditCardType { get; set; }
		string CreditCardNumber { get; set; }
		string CreditCardExpiration { get; set; }
	}
}
