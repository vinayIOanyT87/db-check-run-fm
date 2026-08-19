// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentInfoDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PaymentInfoDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	/// <summary>
	/// The payment info do.
	/// </summary>
	[XmlType("PaymentInfo")]
   [Serializable]
   [DataContract]
	public class PaymentInfoDO
	{
		#region Attributes
		/// <summary>
		/// The time format.
		/// </summary>
		private const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

		/// <summary>
		/// The bill to.
		/// </summary>
		[DataMember]
		private string billTo;

		/// <summary>
		/// The cash amount.
		/// </summary>
		[DataMember]
		private double? cashAmount;

		/// <summary>
		/// The cash currency type.
		/// </summary>
		[DataMember]
		private string cashCurrencyType;

		/// <summary>
		/// The credit card amount.
		/// </summary>
		[DataMember]
		private double? creditCardAmount;

		/// <summary>
		/// The credit card currency type.
		/// </summary>
		[DataMember]
		private string creditCardCurrencyType;

		/// <summary>
		/// The credit card name.
		/// </summary>
		[DataMember]
		private string creditCardName;

		/// <summary>
		/// The credit card type.
		/// </summary>
		[DataMember]
		private string creditCardType;

		/// <summary>
		/// The credit card number.
		/// </summary>
		[DataMember]
		private string creditCardNumber;

		/// <summary>
		/// The credit card expiration.
		/// </summary>
		[DataMember]
		private DateTimeOffset? creditCardExpiration;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="PaymentInfoDO"/> class.
		/// </summary>
		public PaymentInfoDO( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the bill to.
		/// </summary>
		public string BillTo
		{
			get { return this.billTo; }
			set { this.billTo = value; }
		}

		/// <summary>
		/// Gets or sets the cash amount.
		/// </summary>
		public double? CashAmount
		{
			get { return this.cashAmount; }
			set { this.cashAmount = value; }
		}

		/// <summary>
		/// Gets or sets the cash currency type.
		/// </summary>
		public string CashCurrencyType
		{
			get { return this.cashCurrencyType; }
			set { this.cashCurrencyType = value; }
		}

		/// <summary>
		/// Gets or sets the credit card amount.
		/// </summary>
		public double? CreditCardAmount
		{
			get { return this.creditCardAmount; }
			set { this.creditCardAmount = value; }
		}

		/// <summary>
		/// Gets or sets the credit card currency type.
		/// </summary>
		public string CreditCardCurrencyType
		{
			get { return this.creditCardCurrencyType; }
			set { this.creditCardCurrencyType = value; }
		}

		/// <summary>
		/// Gets or sets the credit card name.
		/// </summary>
		public string CreditCardName
		{
			get { return this.creditCardName; }
			set { this.creditCardName = value; }
		}

		/// <summary>
		/// Gets or sets the credit card type.
		/// </summary>
		public string CreditCardType
		{
			get { return this.creditCardType; }
			set { this.creditCardType = value; }
		}

		/// <summary>
		/// Gets or sets the credit card number.
		/// </summary>
		public string CreditCardNumber
		{
			get { return this.creditCardNumber; }
			set { this.creditCardNumber = value; }
		}

		/// <summary>
		/// Gets or sets the credit card expiration.
		/// </summary>
		[XmlIgnore]
		public DateTimeOffset? CreditCardExpiration
		{
			get { return this.creditCardExpiration; }
			set { this.creditCardExpiration = value; }
		}

		/// <summary>
		/// Gets or sets the credit card expiration string.
		/// </summary>
		[XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
		public string CreditCardExpirationString
		{
			get
			{
				return this.creditCardExpiration == null ? string.Empty : ((DateTimeOffset) this.creditCardExpiration).ToString(TimeFormat);
			}

			set
			{
				this.creditCardExpiration = string.IsNullOrEmpty(value) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}
		#endregion Properties

		#region Methods to handle whether a property should be serialized.
		/// <summary>
		/// This method causes the Cash Amount property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeCashAmount( )
		{
			return this.cashAmount.HasValue;
		}

		/// <summary>
		/// This method causes the Credit Card Amount property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeCreditCardAmount( )
		{
			return this.creditCardAmount.HasValue;
		}
		#endregion
	}
}
