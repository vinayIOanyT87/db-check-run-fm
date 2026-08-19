using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FuelsManager.Afss.WebApp.InternalClasses
{
	using System.Diagnostics;

	using FMBusinessObjects.LogClient;

	internal class CardSwipeHelper
	{
		private static bool CreditCardPrefixAndCheckDigitValid(string cardNumber)
		{
			bool success = cardNumber.StartsWith("789682")
						&& cardNumber.Length == 16
						&& CreditCardCheckDigitIsGood(cardNumber);

			return success;
		}

		private static bool CreditCardCheckDigitIsGood(string cardNumber)
		{
			// http://web.eecs.umich.edu/~bartlett/credit_card_number.html
			bool success = false;
			try
			{
				List<int> sumList = new List<int>();
				char[] digits = cardNumber.ToCharArray();
				for (int i = digits.Length - 1; i >= 0; i--)
				{
					int num = digits.Length - i - 1;
					int digit = digits[i] - 48;
					if (num % 2 == 0)
					{
						sumList.Add(digit);
					}
					else
					{
						int timesTwo = digit * 2;
						if (timesTwo < 10)
						{
							sumList.Add(timesTwo);
						}
						else
						{
							sumList.Add(1);
							sumList.Add(timesTwo % 10);
						}
					}
				}

				int sum = sumList.Sum();

				if (sum % 10 == 0)
				{
					success = true;
				}
			}
			catch (Exception)
			{
				success = false;
			}

			return success;
		}

		internal static bool IsCreditCardNumberValid(string cardNumber)
		{
			bool isValid = (string.IsNullOrEmpty(cardNumber) == false
						 && CreditCardPrefixAndCheckDigitValid(cardNumber));

			return isValid;
		}

		internal static bool IsExpirationDateValid(DateTime expirationDate)
		{
			bool isValid = expirationDate >= DateTime.Today.AddDays(1 - DateTime.Today.Day);
			return isValid;
		}

		internal static bool IsCardValid(string cardNumber, DateTime expirationDate)
		{
			bool valid = IsCreditCardNumberValid(cardNumber) && IsExpirationDateValid(expirationDate);
			return valid;
		}

		internal static bool ParseMagneticStripe(
			string trackData,
			out string cardHolderName,
			out string cardNumber,
			out DateTime expirationDate)
		{
			bool retValue = true;

			cardHolderName = string.Empty;
			cardNumber = string.Empty;
			expirationDate = new DateTime();
			string[] segments = trackData.Split('^');
			if (segments.Length < 3)
			{
				Trace.WriteLine("Magnetic stripe data does not have three segments. No action taken.");
				retValue = false;
			}
			else
			{
				string personName;
				int cardExpirationYear;
				int cardExpirationMonth;

				ParseMagneticStripe(
					trackData, out personName, out cardNumber, out cardExpirationYear, out cardExpirationMonth);

				int year = 2000 + cardExpirationYear;
				int month = cardExpirationMonth;
				expirationDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
				cardHolderName = personName;
			}
			return retValue;
		}

		internal static void ParseMagneticStripe(
			string trackData,
			out string personName,
			out string cardNumber,
			out int cardExpirationYear,
			out int cardExpirationMonth)
		{
			personName = string.Empty;
			cardNumber = string.Empty;
			cardExpirationYear = 0;
			cardExpirationMonth = 0;
			int cardAddedInfo = 0;

			bool CaretPresent = false;
			bool EqualPresent = false;

			CaretPresent = trackData.Contains("^");
			EqualPresent = trackData.Contains("=");

			if (CaretPresent)
			{
				string[] CardData = trackData.Split('^');
				//B1234123412341234^CardUser/John^030510100000019301000000877000000?

				personName = FormatName(CardData[1]);
				cardNumber = FormatCardNumber(CardData[0]);
				cardExpirationYear = int.Parse(CardData[2].Substring(0, 2));
				cardExpirationMonth = int.Parse(CardData[2].Substring(2, 2));
			}
			else if (EqualPresent)
			{
				string[] CardData = trackData.Split('=');
				string CardNo = string.Empty;
				//1234123412341234=0305101193010877?

				cardNumber = string.Format("{0}{1}", FormatCardNumber(CardData[0]), CardData[1].Substring(7, 5));
				cardExpirationYear = int.Parse(CardData[1].Substring(0, 2));
				cardExpirationMonth = int.Parse(CardData[1].Substring(2, 2));
				cardAddedInfo = int.Parse(CardData[1].Substring(4, 3));
			}
		}

		private static string FormatCardNumber(string o)
		{
			string result = string.Empty;

			result = Regex.Replace(o, "[^0-9]", string.Empty);

			return result;
		}

		private static string FormatName(string o)
		{
			string result = string.Empty;

			if (o.Contains("/"))
			{
				string[] NameSplit = o.Split('/');

				result = NameSplit[1].Trim() + " " + NameSplit[0].Trim();
			}
			else
			{
				result = o.Trim();
			}

			return result;
		}
	}
}
