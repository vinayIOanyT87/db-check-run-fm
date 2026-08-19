// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionFieldGenerator.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionFieldGenerator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.UtilityObjects;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
   using System.Web;
   using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	/// <summary>
	/// The transaction field generator.
	/// </summary>
	public class TransactionFieldGenerator
	{
		#region Protected data members
		public static readonly string RetrieveExceptionPrefix = "Retrieve Error Field ";
		public static readonly string RetrieveExceptionDelimiter = " : ";

		protected TransactionUserFieldGenerator userFieldGenerator;
		protected TransactionContext transContext;
		protected Table fieldTable;
		protected TransactionDO trans;
		protected Hashtable fieldHashTable;
		protected byte maxColumns;
		protected byte columnIndex;
        private string glossaryFileName = string.Empty;
		#endregion

		#region Internal data members
		internal Logger logger;
		internal AccountingSite accountingSite;
		#endregion

		#region Private data members
		/// <summary>
		/// The page.
		/// </summary>
		private readonly Page page;

		/// <summary>
		/// The field configuration.
		/// </summary>
		private FieldConfiguration fieldConfiguration;
        #endregion
        public TransactionDO Trans
        {
            set
            {
                this.trans = value;
            }
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionFieldGenerator"/> class.
        /// </summary>
        /// <param name="transContext">
        /// The transaction context.
        /// </param>
        /// <param name="fieldTable">
        /// The field table.
        /// </param>
        /// <param name="trans">
        /// The transaction.
        /// </param>
        /// <param name="accountingSite">
        /// The accounting site.
        /// </param>
        /// <param name="inPage">
        /// The in page.
        /// </param>
        public TransactionFieldGenerator ( 
											TransactionContext transContext,
											Table fieldTable,
											TransactionDO trans,
											AccountingSite accountingSite,
											Page inPage )
		{
			this.transContext	= transContext;
			this.fieldTable		= fieldTable;
			this.trans			= trans;
			this.accountingSite = accountingSite;
			this.page			= inPage;

			this.Init ( );
			this.SetFieldConfiguration( );
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionFieldGenerator"/> class.
		/// </summary>
		/// <param name="transContext">
		/// The transaction context.
		/// </param>
		/// <param name="fieldTable">
		/// The field table.
		/// </param>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		/// <param name="accountingSite">
		/// The accounting site.
		/// </param>
		/// <param name="inPage">
		/// The page.
		/// </param>
		/// <param name="setFieldConfig">
		/// The set field configuration.
		/// </param>
		public TransactionFieldGenerator(
										TransactionContext transContext,
										Table fieldTable,
										TransactionDO trans,
										AccountingSite accountingSite,
										Page inPage,
										bool setFieldConfig)
		{
			this.transContext = transContext;
			this.fieldTable = fieldTable;
			this.trans = trans;
			this.accountingSite = accountingSite;
			this.page = inPage;

			this.Init( );

			if ( setFieldConfig )
			{
				this.SetFieldConfiguration( );
			}
		}

		public TransactionFieldGenerator( TransactionContext transContext, TransactionDO trans )
		{
			this.transContext = transContext;
			this.trans = trans;
			this.RegisterGenerators();
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets the page.
		/// </summary>
		public Page Page
		{
			get { return this.page; }
		}

        public string GlossaryFileName
        {
            get { return glossaryFileName; }
            set { glossaryFileName = value; }
        }
		#endregion

		/// <summary>
		/// The bind controls.
		/// </summary>
		public virtual void BindControls ( )
		{
			this.maxColumns = 2;
			this.columnIndex = 0;

			var fieldList							= new ArrayList ( );
			FieldClass notesField					= null;
			FieldClass additionalInformationField	= null; 
			FieldClass errorTextField				= null;
			TableRow row							= null;

			this.fieldConfiguration = new FieldConfiguration ( this.Page );
			this.fieldConfiguration.LoadConfigurationData ( );

			// Iterate through the transaction body fields pulled from the alias' display order
			// Set the fieldClass' ID based on whether the field is a line item or weight
			// reading field
			foreach (FieldClass fieldClass in this.transContext.aliasClass.DisplayOrder ( TRANSACTION_SECTION_TYPE.BODY ))
			{
				string id = fieldClass.ID;

				// Notes are always displayed last, regardless of configured order. 
				// Hold on to the Field Class, and process it after the loop completes.
				if (id.Equals("Notes"))
				{
					notesField = fieldClass;
					continue;
				}

				// Additional Information field is always displayed next to last, regardless of configured order. 
				// Hold on to the Field Class, and process it after the loop completes.
				if ( id.Equals("AdditionalInformation") )
				{
					additionalInformationField = fieldClass;
					continue;
				}

				// The ID is the database column name.  The Error field should always be
				// below the Notes field. The error field will only be displayed if 
				// there is an error on the transaction.
				if (id.Equals("Error"))
				{
					errorTextField = fieldClass;
					continue;
				}

				var tafc = fieldClass as TransactionAliasFieldClass;

				// If the fieldClass is a line item field, prepend "LineItem " to the ID
				if ( tafc != null && tafc.Type == TransactionFieldType.LineItem && id.IndexOf("LineItem") == -1)
				{
					if (id.IndexOf ( "Model" ) != -1)
					{
						id = id.Replace ( "Model", string.Empty );
					}
					else if (id.IndexOf ( "SerialNumber" ) != -1)
					{
						id = id.Replace ( "SerialNumber", string.Empty );
					}
					else if (id.IndexOf ( "Type" ) != -1)
					{
						id = id.Replace ( "Type", string.Empty );
					}
					else if (id.IndexOf ( "CompanyEquipmentID" ) != -1)
					{
						id = id.Replace ( "CompanyEquipmentID", "Equipment" );
					}

					id = "LineItem " + id;
				}

				// If the fieldClass is a weight_reading field prepend "AGR " to the id
				if (tafc != null && tafc.Type == TransactionFieldType.WeightReading && id.IndexOf("AGR") == -1)
				{
					id = "AGR " + id;
				}

				// If the field class is a transport line Item, prepend "TransportLineItem "
				// to the ID.
				if (tafc != null && tafc.Type == TransactionFieldType.TransportInfo && id.IndexOf("TransportLineItem") == -1)
				{
					id = "TransportLineItem " + id;
				}

				fieldClass.ID = id;
				fieldList.Add ( fieldClass );
			}

			foreach (FieldClass fieldClass in fieldList)
			{
				// Determine if the field is a user data field
				if (fieldClass is UserDataFieldClass)
				{
					this.CreateField ( ref row, fieldClass.ID, fieldClass.DisplayName, fieldClass.FieldRequired, fieldClass as UserDataFieldClass );
				}
				else
				{
					this.CreateField ( ref row, fieldClass.ID, fieldClass.DisplayName, fieldClass.FieldRequired );
				}
			}

			// Now that other fields are displayed, Additional Information field can be added.
			if ( additionalInformationField != null )
			{
				row = null;
				this.columnIndex = 0;
				this.CreateField(
								ref row,
								additionalInformationField.ID,
								additionalInformationField.DisplayName,
								additionalInformationField.FieldRequired);
			}

			// Now that other fields are displayed, Notes can be added.
			if (notesField != null)
			{
				row = null;
				this.columnIndex = 0;
				this.CreateField ( ref row, notesField.ID, notesField.DisplayName, notesField.FieldRequired );
			}

			// Only create the Error field if the transaction has an error (errorflag = true) and
			// the error field has been configured.
			if ((errorTextField != null) && (this.trans.ErrorFlag == true))
			{
				row = null;
				this.columnIndex = 0;
				this.CreateField(ref row, errorTextField.ID, errorTextField.DisplayName, errorTextField.FieldRequired);
			}
		}

		/// <summary>
		/// Retrieves the field generator object based on the fieldID passed
		/// and generates the field
		/// </summary>
		/// <param name="cell">The table cell containing the field</param>
		/// <param name="fieldID">The ID of the field generator</param>
		/// <param name="editable">Specifies whether or not the field value can be changed</param>
		/// <param name="required">Specifies whether or not the field is required</param>
		public void GenerateField ( TableCell cell, string fieldID, bool editable, bool required )
		{
			string fieldKey = fieldID;

			var field = this.fieldHashTable[fieldKey] as FieldGenerator;

			// Set the current page in the field generator that will be used
			// to fetch the active update panel.
			if (field != null)
			{
				field.Page = this.Page;
			}

			// Some fields, such as Ship To, are required by the system so
			// if the field is already set as required don't change it
			if (field != null && !field.bFieldRequired)
			{
				field.bFieldRequired = required;
			}

			if (( field is ILineItemField )
			   || ( field is ISublineItemField )
			   || ( field is IWeightReadingField )
			   || ( field is ITransportLineItemField ))
			{
				const int LineItemIndex = 0;
				const int SublineItemIndex = -1;
				field.GenerateField ( cell, this.trans, this.transContext, editable, LineItemIndex, SublineItemIndex );
				return;
			}

			if (field != null)
			{
				field.GenerateField ( cell, this.trans, this.transContext, editable );
				return;
			}

			this.logger.Warn ( "TransactionFieldGenerator.GenerateField() : No FieldGenerator found for field \"" + fieldKey + "\"." );
		}

		/// <summary>
		/// The retrieve field.
		/// </summary>
		/// <param name="cell">
		/// The cell.
		/// </param>
		/// <param name="fieldKey">
		/// The field key.
		/// </param>
		public void RetrieveField ( TableCell cell, string fieldKey )
		{
			var field = this.fieldHashTable[fieldKey] as FieldGenerator;

			if (field is ILineItemField
			   || field is ISublineItemField
			   || field is IWeightReadingField
			   || field is ITransportLineItemField)
			{
				const int LineItemIndex = 0;
				const int SublineItemIndex = -1;
				field.Retrieve ( cell, this.trans, this.transContext, LineItemIndex, SublineItemIndex );
			}
			else if (field != null)
			{
				field.Retrieve ( cell, this.trans, this.transContext );
			}
			else
			{
				this.logger.Warn ( "TransactionFieldGenerator.RetrieveField() : No FieldGenerator found for FieldID \"" + fieldKey + "\"." );
			}
		}

		/// <summary>
		/// The create field.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="fieldId">
		/// The field ID.
		/// </param>
		/// <param name="displayName">
		/// The display name.
		/// </param>
		/// <param name="required">
		/// The required.
		/// </param>
		private void CreateField ( ref TableRow row, string fieldId, string displayName, bool required )
		{
			this.CreateField ( ref row, fieldId, displayName, required, null );
		}

		/// <summary>
		/// The create field.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="fieldId">
		/// The field ID.
		/// </param>
		/// <param name="displayName">
		/// The display name.
		/// </param>
		/// <param name="required">
		/// The required.
		/// </param>
		/// <param name="userField">
		/// The user field.
		/// </param>
		private void CreateField ( ref TableRow row, string fieldId, string displayName, bool required, UserDataFieldClass userField )
		{
			if (this.columnIndex++ == this.maxColumns)
			{
				this.columnIndex = 1;
				row = null;
			}

			if (row == null)
			{
				row = new TableRow ( );
				this.fieldTable.Rows.Add ( row );
			}

			// Create the field cell now so we can read the configuration information.  We will
			// add the cell to the table later so it it after the label cell.
			var fieldCell = new TableCell ( );
			row.Cells.Add ( fieldCell );
			fieldCell.ID = "FieldValue " + fieldId;
			fieldCell.CssClass = "formfield";
		    fieldCell.Style["padding-bottom"] = "5px";

            var cell = new TableCell ( );
			row.Cells.Add ( cell );
			cell.CssClass = "formfieldtitle";
			cell.Wrap = true;
			cell.ID = "FieldLabel " + fieldId;
            fieldCell.Style["padding-bottom"] = "5px";

            // If a glossary file has been configured in web.config, we need to generate a link control with the 
            // cell ID instead of just having text in the cell.  Note: fields can be exempted in the transaction
            // field configuration file.
            var field = this.fieldHashTable[fieldId] as FieldGenerator;
         string htmlEncodeDisplayName = HttpUtility.HtmlEncode(displayName);
         if (field != null)
			{
				field.DisplayName = htmlEncodeDisplayName;
			}

			if (( string.IsNullOrEmpty ( this.GlossaryFileName ) == false ) && ( field == null || field.GenerateGlossaryEntry ))
			{
				string linkFieldId = string.Format ( "FieldLabel {0} Link", fieldId );
				string functionCall = string.Format ( "javascript:CallGlossary('{0}','{1}')", this.GlossaryFileName, linkFieldId );
 				cell.Text = string.Format ( "<a id=\"{0}\" tabindex=\"-1\" class=\"formfieldtitle\" href=\"{1}\">{2}</a>", linkFieldId, functionCall, htmlEncodeDisplayName);
			}
			else
			{
				cell.Text = htmlEncodeDisplayName;
         }

			cell = new TableCell ( );
			row.Cells.Add ( cell );
			cell.ID = "FieldRequiredLabel " + fieldId;

			// Now add the field cell to the table
			row.Cells.Add ( fieldCell );
			this.CreateFieldValueControl ( fieldCell, fieldId, required, userField );

			foreach (Control control in fieldCell.Controls)
			{
				var webControl = control as WebControl;

				if (webControl != null && string.IsNullOrEmpty(webControl.CssClass))
				{
					webControl.CssClass = "formfield";
				}

				var htmlControl = control as HtmlControl;

				if (htmlControl != null)
				{
					htmlControl.Attributes.Add ( "class", "formfield" );
				}
			}
		}

		/// <summary>
		/// The create field value control.
		/// </summary>
		/// <param name="cell">
		/// The cell.
		/// </param>
		/// <param name="fieldId">
		/// The field ID.
		/// </param>
		/// <param name="required">
		/// The required.
		/// </param>
		/// <param name="userField">
		/// The user field.
		/// </param>
		private void CreateFieldValueControl ( TableCell cell, string fieldId, bool required, UserDataFieldClass userField )
		{
			bool editable = this.transContext.mode != TransactionContext.Mode.View;

			// If the transaction is a partial closeout, we cannot edit the header fields
			if (this.trans.PartialCloseout)
			{
				editable = false;
			}

			if (this.trans.ReversalType == TransactionDO.Reversal || this.trans.ReversalType == TransactionDO.Original)
			{
				editable = false;
			}

			if (userField != null)
			{
				FieldGenerator field = this.userFieldGenerator.GenerateField ( cell, userField, this.trans, this.transContext, editable, required );
				
				if (field != null)
				{
					this.Register ( field );
				}
			}
			else
			{
				this.GenerateField ( cell, fieldId, editable, required );
			}
		}

		/// <summary>
		/// The retrieve.
		/// </summary>
		/// <exception cref="RetrieveException">
		/// </exception>
		/// <exception cref="FMFieldRequiredException">
		/// </exception>
		public void Retrieve ( )
		{
			string errormessage = string.Empty;

			foreach (TableRow row in this.fieldTable.Rows)
			{
				foreach (TableCell cell in row.Cells)
				{
					string fieldKey = "Unknown Field";

					// CSI 4910, add try and catch and throw the exception after all field values have been set; otherwise, 
					// it will stop setting values at the first exception
					try  
					{
						if (cell.ID.StartsWith("FieldValue") == false)
						{
							continue;
						}

						fieldKey = cell.ID.Replace ( "FieldValue ", string.Empty );

						if (fieldKey.ToUpper().StartsWith(TransactionDO.UserDataKeyPrefix.ToUpper())
							|| fieldKey.ToUpper().StartsWith(BaseTransactionLineItemDO.UserDataLineItemKeyPrefix.ToUpper()))
						{
							this.userFieldGenerator.RetrieveField ( cell, fieldKey, trans, this.transContext );
						}
						else
						{
							this.RetrieveField ( cell, fieldKey );
						}
					}
					catch (FMFieldRequiredException e)
					{
						errormessage = e.Message;
					}
					catch (Exception e)
					{
						throw new RetrieveException("Retrieve Error Field " + fieldKey + " : " + e.Message);
					}
				}
			}

			if (errormessage != string.Empty)
			{
				throw new FMFieldRequiredException ( );
			}
		}

		/// <summary>
		/// The get field generator.
		/// </summary>
		/// <param name="fieldKey">
		/// The field key.
		/// </param>
		/// <returns>
		/// The <see cref="FieldGenerator"/>.
		/// </returns>
		public FieldGenerator GetFieldGenerator ( string fieldKey )
		{
			return this.fieldHashTable[fieldKey] as FieldGenerator;
		}

		/// <summary>
		/// This method initialize the transaction field generator object to its initial
		/// state. It registers all the transaction fields to be generated.
		/// </summary>
		private void Init ( )
		{
			this.logger = new Logger ( "Accounting" );

			// Load custom field configuration such as field sizes.
			this.fieldConfiguration = new FieldConfiguration ( this.Page );
			this.fieldConfiguration.LoadConfigurationData ( );

			this.userFieldGenerator = new TransactionUserFieldGenerator ( fieldConfiguration );

			this.RegisterGenerators();
		}

		private void RegisterGenerators()
		{
			this.fieldHashTable = new Hashtable();

			// Register generators
			this.Register(new PONFG());
			this.Register(new ManagerFG());
			this.Register(new OwnerFG());
			this.Register(new BillToFG());
			this.Register(new ShipToFG());
			this.Register(new CarrierFG());
			this.Register(new SCACCodeFG());
			this.Register(new SupplierFG());
			this.Register(new ShipperFG());
			this.Register(new InventoryDateFG());
			this.Register(new TransactionDateTimeFG());
			this.Register(new TransAliasFG());
			this.Register(new DocumentNumberFG());
			this.Register(new ShipmentNumberFG());
			this.Register(new ShippingDocumentNumberFG());
			this.Register(new ETA_FG());
			this.Register(new ETD_FG());
			this.Register(new STA_FG());
			this.Register(new STD_FG());
			this.Register(new SFT_FG());
			this.Register(new FST_FG());
			this.Register(new TimeInFG());
			this.Register(new TimeOutFG());
			this.Register(new TimeEndFG());
			this.Register(new EstimatedFuelingDurationFG());
			this.Register(new RequestedDeliveryDateFG());
			this.Register(new RoutingID_FG());
			this.Register(new PreviousRoutingID_FG());
			this.Register(new RouteOriginationDateFG());
			this.Register(new InternationalRouteFG());
			this.Register(new OriginStationFG());
			this.Register(new PreviousStationFG());
			this.Register(new NextStationFG());
			this.Register(new FinalStationFG());
			this.Register(new CardTypeFG());
			this.Register(new CardNumberFG());
			this.Register(new CardNameFG());
			this.Register(new CardExpirationFG());
			this.Register(new CreditAmountFG());
			this.Register(new CashAmountFG());
			this.Register(new SiteFG());
			this.Register(new LoadID_FG());
			this.Register(new NotesFG());
			this.Register(new AdditionalInformationFG());
			this.Register(new TransactionTypeFG());
			this.Register(new TicketSourceFG());
			this.Register(new TicketMode());
			this.Register(new LinkedDocumentNumberFG());
			this.Register(new TransactionStatusFG());
			this.Register(new TransReferenceID_FG());
			this.Register(new ReversalTypeFG());
			this.Register(new DriverIdentificationNumberFG());
			this.Register(new CloseoutDateFG());
			this.Register(new TransID_FG());
			this.Register(new ConjoinedTransIdFG());
			this.Register(new ReversedTransID_FG());
			this.Register(new DestinationEquipmentFG(1));
			this.Register(new DestinationEquipmentFG(2));
			this.Register(new DestinationEquipmentFG(3));
			this.Register(new DestinationSerialNumberFG(1));
			this.Register(new DestinationSerialNumberFG(2));
			this.Register(new DestinationSerialNumberFG(3));
			this.Register(new DestinationEquipmentModelFG(1));
			this.Register(new DestinationEquipmentModelFG(2));
			this.Register(new DestinationEquipmentModelFG(3));
			this.Register(new DestinationEquipmentTypeFG(1));
			this.Register(new DestinationEquipmentTypeFG(2));
			this.Register(new DestinationEquipmentTypeFG(3));
			this.Register(new SourceEquipmentFG(1));
			this.Register(new SourceEquipmentFG(2));
			this.Register(new SourceEquipmentFG(3));
			this.Register(new SourceSerialNumberFG(1));
			this.Register(new SourceSerialNumberFG(2));
			this.Register(new SourceSerialNumberFG(3));
			this.Register(new SourceEquipmentModelFG(1));
			this.Register(new SourceEquipmentModelFG(2));
			this.Register(new SourceEquipmentModelFG(3));
			this.Register(new SourceEquipmentTypeFG(1));
			this.Register(new SourceEquipmentTypeFG(2));
			this.Register(new SourceEquipmentTypeFG(3));
			this.Register(new OperatorID_FG());
			this.Register(new ExpirationDateFG());
			this.Register(new EffectiveDateFG());
			this.Register(new ScheduledDateFG());
			this.Register(new AutoCompleteFG());
			this.Register(new Flag01FG());
			this.Register(new Flag02FG());
			this.Register(new Flag03FG());
			this.Register(new Flag04FG());
			this.Register(new Flag05FG());
			this.Register(new Flag06FG());
			this.Register(new Number01FG());
			this.Register(new Number02FG());
			this.Register(new Number03FG());
			this.Register(new Number04FG());
			this.Register(new Number05FG());
			this.Register(new Number06FG());
			this.Register(new ContactFirstNameFG());
			this.Register(new ContactSurnameFG());
			this.Register(new Date01FG());
			this.Register(new Date02FG());
			this.Register(new Date03FG());
			this.Register(new Date04FG());
			this.Register(new LegacyNumberFG());
			this.Register(new CountryFG());
			this.Register(new ContactInfoFG());
			this.Register(new AssociatedDocumentNumberFG());
			this.Register(new AssociatedCLINFG());
			this.Register(new AssociatedTransportOrderNumberFG());
			this.Register(new SubmittedToAccountingFG());
			this.Register(new OriginApplicationFG());
			this.Register(new FuelCardFG());
			this.Register(new RequestedDateTimeFG());
			this.Register(new DispatchedDateTimeFG());
			this.Register(new DeleteFlagFG());
			this.Register(new FuelAdditiveFlagFG());
			this.Register(new IssuePointFG());
			this.Register(new IssuePointNumberFG());
			this.Register(new RadioNumberFG());
			this.Register(new GateFG());

			// Read only fields
			this.Register(new CreatedByFG());
			this.Register(new CreatedDateFG());
			this.Register(new UpdatedByFG());
			this.Register(new UpdatedDateFG());
			this.Register(new ErrorFlagFG());

			// Virtual fields
			this.Register(new TotalPriceAmountFG());
			this.Register(new TotalExciseFG());
			this.Register(new TotalGSTFG());
			this.Register(new TotalMarkupFG());
			this.Register(new TotalPriceWithTaxFG());
			this.Register(new TotalGrossQuantityFG());
			this.Register(new TotalNetQuantityFG());
			this.Register(new TotalMassQuantityFG());
			this.Register(new LineItemMeterTotalFG());
			this.Register(new VolumeUnitFG());
			this.Register(new LevelUnitFG());
			this.Register(new TemperatureUnitFG());
			this.Register(new DensityUnitFG());
			this.Register(new MassUnitFG());
			this.Register(new FlowUnitFG());
			this.Register(new PressureUnitFG());
			this.Register(new ReasonCodeFG());
			this.Register(new LineItemLevelUnitFG());
			this.Register(new LineItemVolumeUnitFG());
			this.Register(new LineItemTemperatureUnitFG());
			this.Register(new LineItemDensityUnitFG());
			this.Register(new LineItemMassUnitFG());
			this.Register(new LineItemFlowUnitFG());
			this.Register(new LineItemPressureUnitFG());
			this.Register(new ResponseTimeFG());
			this.Register(new FuelTimeFG());

			// Aviation Gauge Readings
			this.Register(new AGR_CompartmentID());
			this.Register(new AGRBeginQuantity());
			this.Register(new AGRRequestedQuantity());
			this.Register(new AGRFinalQuantity());

			// Both LineItems and Sub-line Items
			this.Register(new LineItemProductFG());
			this.Register(new LineItemGrossQuantityFG());
			this.Register(new LineItemDeliveredGrossQuantityFG());
			this.Register(new LineItemNetQuantityFG());
			this.Register(new LineItemDeliveredNetQuantityFG());
			this.Register(new LineItemTemperatureFG());
			this.Register(new LineItemPressureFG());
			this.Register(new LineItemDensityFG());
			this.Register(new LineItemVcfFG());
			this.Register(new LineItemMeterFactorFG());
			this.Register(new LineItemMeterStartFG());
			this.Register(new LineItemMeterStopFG());
			this.Register(new LineItemMeterStartTimeFG());
			this.Register(new LineItemMeterStopTimeFG());
			this.Register(new LineItemBottomVolumeFG());
			this.Register(new LineItemLineFillFG());
			this.Register(new LineItemDifferentialPressureFG());
			this.Register(new LineItemFreezePointFG());
			this.Register(new LineItemArmNumberFG());
			this.Register(new LineItemLineNumberFG());
			this.Register(new LineItemBatchNumberFG());
			this.Register(new LineItemTankStatusFG());
			this.Register(new LineItemTransactionStatusFG());
			this.Register(new LineItemNetCapacityFG());
			this.Register(new LineItemStorageLocationFG());
			this.Register(new LineItemMeterIDFG());
			this.Register(new LineItemTransactionQualityFG());
			this.Register(new LineItemTax1FG());
			this.Register(new LineItemTax2FG());
			this.Register(new LineItemTax3FG());
			this.Register(new LineItemTax4FG());
			this.Register(new LineItemTax5FG());
			this.Register(new ImproperAdditizationFG());
			this.Register(new BrokenBlendFG());
			this.Register(new LineItemFlag01FG());
			this.Register(new LineItemFlag02FG());
			this.Register(new LineItemFlag03FG());
			this.Register(new LineItemFlag04FG());
			this.Register(new LineItemFlag05FG());
			this.Register(new LineItemFlag06FG());
			this.Register(new LineItemNumber01FG());
			this.Register(new LineItemNumber02FG());
			this.Register(new LineItemNumber03FG());
			this.Register(new LineItemNumber04FG());
			this.Register(new LineItemNumber05FG());
			this.Register(new LineItemNumber06FG());
			this.Register(new LineItemDestinationEquipmentFG());
			this.Register(new LineItemDestinationCompartmentID_FG());
			this.Register(new LineItemSourceEquipmentFG());
			this.Register(new LineItemSourceCompartmentID_FG());
			this.Register(new LineItemDocumentNumberFG());
			this.Register(new LineItemRequestedDateTimeFG());
			this.Register(new LineItemAcknowledgedDateTime());
			this.Register(new LineItemValidationDateTimeFG());
			this.Register(new LineItemOnLocationTimeFG());
			this.Register(new LineItemCompletionDateTimeFG());
			this.Register(new LineItemDispatchedDateTimeFG());
			this.Register(new LineItemRequestedByFG());
			this.Register(new LineItemPitFG());
			this.Register(new LineItemReceiptVarianceFG());
			this.Register(new LineItemLoadRackVarianceFG());
			this.Register(new LineItemContractNumberFG());
			this.Register(new LineItemCLIN_FG());
			this.Register(new LineItemSequenceNumberFG());
			this.Register(new LineItemOperatorFG());
			this.Register(new LineItemProductPriceFG());
			this.Register(new LineItemEngineeringUnitsIndexFG());
			this.Register(new LineItemCOAWaiverFG());
			this.Register(new LineItemCOANoteFG());
			this.Register(new LineItemCOAIDFG());
			this.Register(new LineItemAdditiveProfileIDFG());
			this.Register(new LineItemSpecialInstructionFG());
			this.Register(new LineItemCloseoutDateFG());
			this.Register(new LineItemLoadingLocationFG());
			this.Register(new LineItemOdometerHoursFG());
			this.Register(new LineItemEndDeliveryDateFG());
			this.Register(new LineItemRequestedDeliveryDateFG());
			this.Register(new LineItemInvoiceNumberFG());
			this.Register(new LineItemInvoiceLineNumberFG());
			this.Register(new LineItemAlternativeGrossVolumeFG());
			this.Register(new LineItemAlternativeNetVolumeFG());
			this.Register(new LineItemAlternativeUnitsFG());
			this.Register(new LineItemTankLevelFG());
			this.Register(new LineItemTankLevelUnitsFG());
			this.Register(new LineItemDate01FG());
			this.Register(new LineItemDate02FG());
			this.Register(new LineItemDate03FG());
			this.Register(new LineItemDate04FG());
			this.Register(new LineItemNonDomesticPriceFG());
			this.Register(new LineItemCurrencyUnitFG());
			this.Register(new LineItemExchangeRateFG());
			this.Register(new LineItemQualityTestNumberFG());
			this.Register(new LineItemOdometerFG());
			this.Register(new LineItemTotalPriceWithTaxFG());
			this.Register(new LineItemDeliveryLocationFG());
			this.Register(new LineItemContaminatePromptFG());
			this.Register(new LineItemCompartmentsPreviouslyLoadedFG());
			this.Register(new LineItemCompartmentsEmptyFG());
			this.Register(new LineItemVarianceFG());
			this.Register(new LineItemPartialFillFG());
			this.Register(new LineItemNetVolumeIndicatorFG());

			// Order Specific
			this.Register(new LineItemGrossQuantityReceivedFG());
			this.Register(new LineItemGrossQuantityRemainingFG());
			this.Register(new LineItemNetQuantityReceivedFG());
			this.Register(new LineItemNetQuantityRemainingFG());
			this.Register(new LineItemMassQuantityRemainingFG());
			this.Register(new LineItemMassQuantityReceivedFG());

			// Supply Order Specific
			this.Register(new LineItemTotalValueFG());
			this.Register(new LineItemValueRemainingFG());

			// Sub-line Items
			this.Register(new LineItemPresetAmountFG());
			this.Register(new LineItemCustomsFG());

			// Conjoined (Transfer) transaction fields
			this.Register(new FromManagerFG());
			this.Register(new ToManagerFG());
			this.Register(new FromOwnerFG());
			this.Register(new ToOwnerFG());
			this.Register(new FromCarrierFG());
			this.Register(new ToCarrierFG());
			this.Register(new LineItemFromProductFG());
			this.Register(new LineItemToProductFG());
			this.Register(new ToBillToFG());
			this.Register(new FromBillToFG());
			this.Register(new ToShipToFG());
			this.Register(new FromShipToFG());
			this.Register(new LineItemToStorageLocationFG());
			this.Register(new LineItemFromStorageLocationFG());

			// Transport Line Item fields
			this.Register(new TransportOrderNumberFG());
			this.Register(new TransportLocationNameFG());
			this.Register(new TransportAddress1FG());
			this.Register(new TransportAddress2FG());
			this.Register(new TransportCityFG());
			this.Register(new TransportStateFG());
			this.Register(new TransportZipFG());
			this.Register(new TransportPOCNameFG());
			this.Register(new TransportPOCPhoneFG());

			this.Register(new InvoiceQueryFG());
			this.Register(new LineItemCustomProductsLabelFG());
			this.Register(new ADFTransactionDateTimeFG());
			this.Register(new LineItemSelectedQualityFG());
			this.Register(new BulkPaymentNumberFG());
			this.Register(new LineItemDeliveryLocationLabelFG());
			this.Register(new LineItemBaseCostFG());
			this.Register(new LineItemParentDocumentNumberFG());
			this.Register(new ParentUserData03FG());
			this.Register(new LineItemParentFuelOrderNumberFG());
			this.Register(new LineItemParentReceiptNumberFG());
			this.Register(new AssocTxControl());
			this.Register(new TotalOnCostFG());
			this.Register(new ROSupplierFG());
			this.Register(new TotalForeignPriceFG());
			this.Register(new LineItemCurrencyUnitLabelFG());
			this.Register(new LineItemTotalForeignPriceFG());
			this.Register(new LineItemTotalOnCostFG());
			this.Register(new LineItemOnCostFG());

			// missile fuel 
			this.Register(new LineItemMassQuantityFG());
			this.Register(new LineItemMassPackageSizeFG());
			this.Register(new LineItemVolumePackageSizeFG());
			this.Register(new LineItemPackageQuantityFG());
			this.Register(new LineItemVcfManualValueFlagFG());
			this.Register(new LineItemGrossManualValueFlagFG());
			this.Register(new LineItemDeliveredGrossManualValueFlagFG());
			this.Register(new LineItemNetManualValueFlagFG());
			this.Register(new LineItemDeliveredNetManualValueFlagFG());
			this.Register(new LineItemMassManualValueFlagFG());
			this.Register(new LineItemPackageManualValueFlagFG());

			// New fields for Line Flush Tracking
			this.Register(new LineItemCleanLineDeductItemFG());
			this.Register(new LineItemCleanLineDeductQuantityFG());
			this.Register(new LineItemCleanLineItemFG());
			this.Register(new LineItemCleanLinePackQuantityFG());

			// Interface result data fields
			this.Register(new InterfaceData01FG());
			this.Register(new InterfaceData02FG());
			this.Register(new InterfaceData03FG());
			this.Register(new InterfaceData04FG());
			this.Register(new InterfaceData05FG());
			this.Register(new InterfaceData06FG());
			this.Register(new InterfaceData07FG());
			this.Register(new InterfaceData08FG());
			this.Register(new TransactionErrorFG());

			this.Register(new OperatorNameFG());
		}

		/// <summary>
		/// The register.
		/// </summary>
		/// <param name="field">
		/// The field.
		/// </param>
		protected void Register ( FieldGenerator field )
		{
			this.fieldHashTable.Add ( field.FieldID, field );
		}

		
		/// <summary>
		/// This method will replace an existing field generator control with a custom
		/// field generator control.  It is used by the derived class.
		/// </summary>
		/// <param name="fieldGeneratorList">
		/// The field generator list.
		/// </param>
		protected void ReplaceFieldGeneratorControl(List<FieldGenerator> fieldGeneratorList)
		{
			if (fieldGeneratorList == null || fieldGeneratorList.Count < 1)
			{
				return;
			}

			foreach (FieldGenerator customFieldGenerator in fieldGeneratorList)
			{
				if (this.fieldHashTable.Contains(customFieldGenerator.FieldID))
				{
					this.fieldHashTable[customFieldGenerator.FieldID] = customFieldGenerator;
				}
			}
		}

		/// <summary>
		/// The set field configuration.
		/// </summary>
		protected void SetFieldConfiguration()
		{
			foreach (FieldGenerator field in this.fieldHashTable.Values)
			{
				field.SetFieldGenerator(this);
				field.TransFieldConfiguration = this.fieldConfiguration;
			}
		}
	}
}
