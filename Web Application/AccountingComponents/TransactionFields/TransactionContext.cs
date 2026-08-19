namespace TransactionFields
{
	using System;
	using System.Collections;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// Summary description for TransactionContext.
	/// </summary>
	[Serializable]
	public class TransactionContext
	{
		#region Public Attributes
		public bool reload = false;
		public SecurityClass security;
		public AccountingSite accountingSite;
		public Mode mode;
		public bool useDataDictonary;
		public TransactionAliasClass aliasClass;
		public TransactionDO conjoinedTrans;
		public bool EnableAutoComplete { get; set; }
		public enum Mode { Add, Edit, View }
		#endregion

		#region Protected attributes
		protected string transAlias;
		#endregion Attributes

		#region Private attributes
		private CurrencyDOCollectionClass currencies;
		private string intermediateTransID;
		private Hashtable associatedDocNumFlags;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction context class.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="accountingSite"></param>
		/// <param name="transAlias"></param>
		/// <param name="mode"></param>
		/// <param name="useDataDictionary"></param>
		public TransactionContext ( SecurityClass security, AccountingSite accountingSite, string transAlias, Mode mode, bool useDataDictionary )
		{
			this.security = security;
			this.accountingSite = accountingSite;
			this.useDataDictonary = useDataDictionary;

			this.transAlias = transAlias;
			this.mode = mode;
		}
		#endregion

		#region Properties
		public int DefaultStatus
		{
			get;
			set;
		}

		/// <summary>
		/// This property will get and set the intermediate transaction ID.
		/// It is used for transition between controls.
		/// </summary>
		public string IntermediateTransID
		{
			get { return this.intermediateTransID; }
			set { this.intermediateTransID = value; }
		}

		/// <summary>
		/// This property will get and set the associated document number flags.
		/// It is used for transition between controls.
		/// </summary>
		public Hashtable AssociatedDocNumFlags
		{
			get { return this.associatedDocNumFlags; }
			set { this.associatedDocNumFlags = value; }
		}
		#endregion

		public void GetTransactionContext ( )
		{
			GetTransactionContext ( null );
		}

		/// <summary>
		/// This method will retrieve the transaction context if it does
		/// not exist.
		/// </summary>
		/// <param name="alias"></param>
		public void GetTransactionContext ( TransactionAliasClass alias )
		{
			this.aliasClass = alias;

			var timer = new StopWatch ( StopWatch.Appnames.Accounting, "GetTransactionContext() - GetTransactionAliasDefinition()" );
			this.GetTransactionAliasDefinition ( );
			timer.Stop ( );

			timer.Start ( "GetTransactionContext() - GetCurrencies()" );
			this.GetCurrencies ( );
			timer.Stop ( );
		}

		/// <summary>
		/// This method will retrieve the transaction alias definition only if
		/// it does not exist.
		/// </summary>
		private void GetTransactionAliasDefinition ( )
		{
			if (this.aliasClass == null)
			{
				this.aliasClass = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
										x => x.Get ( this.security, x.GetIdentityGuid ( this.security, this.transAlias ), true ));
			}

			DefaultStatus = aliasClass.LookupDefaultStatusIndex;
			this.EnableAutoComplete = this.aliasClass.EnableAutoCompleteControls;

			// Enforce Financial Data View
			// TODO: Temporary commented out so that QA does not test financial configuration features.
			//if (this.security.HasRight ( RIGHT.VIEW_FINANCIAL_DATA ) == false
			//   && this.security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA ) == false)
			//{
				for (int index = aliasClass.TransactionFieldCollection.Count - 1; index >= 0; --index)
				{
					TransactionAliasFieldClass field = aliasClass.TransactionFieldCollection[index];

					if (field.IsFinancialField)
					{
						aliasClass.TransactionFieldCollection.RemoveAt ( index );
					}
				}

				for (int index = aliasClass.LineItemFieldCollection.Count - 1; index >= 0; --index)
				{
					TransactionAliasFieldClass field = aliasClass.LineItemFieldCollection[index];

					if (field.IsFinancialField)
					{
						aliasClass.LineItemFieldCollection.RemoveAt ( index );
					}
				}
			//}
		}

		/// <summary>
		/// Populates the Currencies collection.
		/// </summary>
		/// <remarks>The Currencies collection represents currencies from other countries such as
		///  a Mexican Peso or British Pound</remarks>
		private void GetCurrencies ( )
		{
			try
			{
				currencies = FMChannelHelper.MakeCall<ICurrencies, CurrencyDOCollectionClass>( x => x.GetCurrencies ( security ));
			}
			catch
			{
				currencies = null;
			}
		}

		public CurrencyDOCollectionClass Currencies
		{
			get { return this.currencies; }
		}
	}
}
