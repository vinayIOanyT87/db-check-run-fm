namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	public class BulkPaymentNumberFG : NumericTextFieldGenerator, IHeaderField
	{
		#region Construction
		public BulkPaymentNumberFG ( )
		{
			virtualField = true;
		}
		#endregion // Construction

		#region Overrides
		public override string FieldID
		{
			get { return "BulkPaymentNumber"; }
		}

		public override ENumericType NumericType
		{
			get { return ENumericType.Integer; }
		}

		public override SITE_VARIABLE_TYPE UnitType
		{
			get { return SITE_VARIABLE_TYPE.LENGTH; }
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}
		#endregion // Overrides

		#region IHeaderField Members
		public object GetDataValue ( TransactionDO transaction )
		{
			SecurityClass security = transContext.security;

			BulkPaymentInvoiceMappingClass mapping = FMChannelHelper.MakeCall<IBulkPaymentInvoiceMappings, BulkPaymentInvoiceMappingClass>(
																	 x =>
																	 x.EnumerateByInvoiceTransID ( security, transaction.TransID )
																);
			if (mapping != null)
			{
				return mapping.BulkPaymentID;
			}

			// else
			return null;
		}

		public string GetDataText ( TransactionDO transaction )
		{
			object value = GetDataValue ( transaction );

			if (null == value)
			{
				return string.Empty;
			}

			return value.ToString ( );
		}

		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
			// cannot be set
		}
		#endregion // IHeaderFieldMembers
	}
}
