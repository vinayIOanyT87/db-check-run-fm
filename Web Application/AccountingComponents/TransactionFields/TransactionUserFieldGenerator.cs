namespace TransactionFields
{
	using System.Web.UI.WebControls;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.DataObjects;

	public class TransactionUserFieldGenerator
	{
		protected Logger logger;
		protected readonly FieldConfiguration fieldConfiguration;

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transaction User Field Generator class.
		/// </summary>
		/// <param name="inFieldConfig"></param>
		public TransactionUserFieldGenerator(FieldConfiguration inFieldConfig)
		{
			this.fieldConfiguration = inFieldConfig;
			this.Init();
		}
		#endregion

		public virtual FieldGenerator GenerateField(TableCell cell,
											UserDataFieldClass fieldClass,
											TransactionDO trans,
											TransactionContext transContext,
											bool editable,
											bool required)
		{
			FieldGenerator field = null;

			switch (fieldClass.UserDataType)
			{
				case USER_DATA_TYPE.TEXT:
					field = new UserDataTextFG(fieldClass.ID, fieldClass.DisplayName)
							{
								Required = fieldClass.FieldRequired,
								TransFieldConfiguration = this.fieldConfiguration
							};
					break;
				case USER_DATA_TYPE.LIST:
					field = new UserDataListFG(fieldClass.ID, fieldClass.DisplayName)
							{
								Required = fieldClass.FieldRequired,
								TransFieldConfiguration = this.fieldConfiguration
							};
					break;
			}

			if (field == null)
			{
				logger.Error("TransactionFieldGenerator.GenerateField() : No FieldGenerator found for field \"" + fieldClass.ID + "\".");
				return null;
			}

            // I have seen this referred to both ways... to avoid merge errors,
            // leave both checks
			if (fieldClass.ID.Contains ("Line Item") ||
                fieldClass.ID.Contains("TALUD"))
			{
				field.GenerateField(cell, trans, transContext, editable, 0);
			}
			else
			{
				field.GenerateField(cell, trans, transContext, editable);
			}

			return field;
		}

		public virtual void RetrieveField(TableCell cell,
								  string fieldKey,
								  TransactionDO trans,
								  TransactionContext transContext)
		{
			FieldGenerator field = null;

			foreach (FieldClass fieldClass in transContext.aliasClass.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY))
			{
				if (fieldClass.ID == fieldKey)
				{
					var userField = fieldClass as UserDataFieldClass;

					if (userField != null)
					{
						switch (userField.UserDataType)
						{
							case USER_DATA_TYPE.TEXT:
								field = new UserDataTextFG(fieldKey, fieldClass.DisplayName)
								        {
									        Required = fieldClass.FieldRequired,
									        TransFieldConfiguration = this.fieldConfiguration
								        };
								break;
							case USER_DATA_TYPE.LIST:
								field = new UserDataListFG(fieldKey, fieldClass.DisplayName)
								        {
									        Required = fieldClass.FieldRequired,
									        TransFieldConfiguration = this.fieldConfiguration
								        };
								break;
						}
					}
					break;
				}
			}

			if (field == null)
			{
				logger.Error("TransactionFieldGenerator.GenerateField() : No FieldGenerator found for field \"" + fieldKey + "\".");
				return;
			}

            // I have seen this referred to both ways... to avoid merge errors,
            // leave both checks
            if (fieldKey.Contains ("Line Item") ||
                fieldKey.Contains("TALUD"))
			{
				field.Retrieve(cell, trans, transContext, 0);
			}
			else
			{
				field.Retrieve(cell, trans, transContext);
			}
		}

		protected void Init()
		{
			logger = new Logger("Accounting");
		}
	}
}
