namespace FuelsManager.Areas.AccountingArea.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Areas.AccountingArea.ViewModels;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.FieldHelpers;

	[RouteArea( "AccountingArea" )]
	[RoutePrefix("TransactionEditor")]
	[Route( "{action}" )]
	public class TransactionEditorController : FMBaseController
    {

		[HttpGet]
		[Route( "TransactionEditor/{aliasName}" )]
		[Route( "TransactionEditor/{aliasName}/{transId}" )]
		public ActionResult TransactionEditor( string aliasName, string transId )
		{
			var model = new TransactionEditorViewModel();

			try
			{
				model.AliasName = aliasName;
				
				if (string.IsNullOrEmpty(transId))
				{
					this.CreateNewTransaction(model);
				}
				else
				{
					var transactionSr = new TransactionSR
					                    {
						                    TransID = transId, 
											Security = this.Security
					                    };

					model.Transaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(transactionSr));
					model.AliasName = model.Transaction.Alias;
				}

				this.PrepareModel(model, aliasName);

				this.Session[TransactionEditorContext.SessionKey] = new TransactionEditorContext(model);
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );

				// TODO: Set model up so error return works.
			}

			return this.View( model );
		}

		[NonAction]
		protected void PrepareModel(TransactionEditorViewModel model, string aliasName)
		{
			var aliasGuid = FMChannelHelper.MakeCall<ITransactionAliases, Guid>(x => x.GetIdentityGuid(this.Security, aliasName));

			var alias =
				FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
					x => x.Get( this.Security, aliasGuid, byUser: false ) );

			if ( alias == null )
			{
				throw new Exception( "Alias not found: " + aliasName );
			}

			this.GetFieldInfo( alias, model );
			this.GetFormattingInformation( model );
		}

		[HttpGet]
		[Route("{url}")]
		public ActionResult CloseButton(string url)
		{
         return this.RedirectToAspx(Url.RouteUrl("FuelsManagerForm"));
      }

      [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ButtonSubmit( TransactionEditorViewModel model, string command )
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					if (command == "closeButton")
					{
						return this.RedirectToAction("CloseButton");
					}

					// Fix problem with line items not binding directly to transaction object.
					model.Transaction.LineItems[0] = model.LineItem;

					// Send transaction for save.
					this.SaveTransaction(model);

					if ( command.Equals( "newButton", StringComparison.InvariantCultureIgnoreCase ) )
					{
						// TODO: redirect to new transaction
						return this.RedirectToAction( "TransactionEditor/" + model.AliasName + "/" );
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			this.PrepareModel( model, model.AliasName );
			return this.RedirectToAction("TransactionEditor", model);
		}

		[HttpGet]
		public ActionResult TransactionEditor(TransactionEditorViewModel model)
		{
			return this.View(model);
		}

		[NonAction]
		private void SaveTransaction(TransactionEditorViewModel model)
		{
			// TODO: Send transaction for save.

			// Retrieve full transaction
			var context = this.Session[TransactionEditorContext.SessionKey] as TransactionEditorContext;
			if (context == null)
			{
				throw new Exception("Transaction editor context was not found.");
			}

			// Save bound fields
			foreach (var field in context.Fields)
			{
				var helper = FMTransactionFieldFactory.GetFieldHelper(field);
				if (helper != null)
				{
					if (field.Type == TransactionFieldType.Transaction)
					{
						var property = context.Transaction.GetType().GetProperty(helper.FieldId);
						if (property != null)
						{
							object[] value = { property.GetMethod.Invoke(model.Transaction, null) };
							property.SetMethod.Invoke(context.Transaction, value);
						}
					}
					else if (field.Type == TransactionFieldType.LineItem)
					{
						var lineItem = context.Transaction.LineItems[0];
						var property = lineItem.GetType().GetProperty( helper.FieldId );
						if ( property != null )
						{
							object[] value = { property.GetMethod.Invoke( model.Transaction.LineItems[0], null ) };
							property.SetMethod.Invoke( lineItem, value );
						}
					}
				}
			}

			// Call FMBusinessServices
			FMChannelHelper.MakeCall<ITransactions>(x => x.SaveFastEntryTransaction(this.Security, context.Transaction));
		}

		[NonAction]
		private void GetFormattingInformation( TransactionEditorViewModel model )
		{
			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
						x.Get(
							this.Security,
							this.Security.SiteGuid,
							getMemberSites: false,
							getSchedulesAndProcessVariables: false,
							bGetAssociatedAliases: false));

			model.ShortDatePattern = site.ShortDatePattern;
			model.TimePattern = site.TimePattern;
			model.VolumeDecimalPlaces = site.GetSiteDecimalPlaces(SITE_VARIABLE_TYPE.VOLUME);
		}

		[NonAction]
		private void CreateNewTransaction( TransactionEditorViewModel model )
		{
			// TODO: Encapsulate basic transaction creating in the TransactionProcessor or the TransactionDO

			TransactionAliasClass alias = null;

			FMChannelHelper.MakeCall<ITransactionAliases>(
				aliases =>
				{
					Guid aliasGuid = aliases.GetMasterRecordGuid(this.Security, model.AliasName);
					alias = aliases.Get(this.Security, aliasGuid, true);
				});

			if (alias == null)
			{
				throw new Exception("Could not find specified alias.");
			}

			model.Transaction = new TransactionDO();
			model.Transaction.init();

			model.Transaction.TransID = FuelsManagerId.NewId();
			model.Transaction.Site = this.Security.SiteID;
			model.Transaction.SiteGuid = this.Security.SiteGuid;
			model.Transaction.Alias = alias.ID;
			model.Transaction.TransactionAliasGuid = alias.MasterRecordGuid;
			model.Transaction.SubmittedToAccounting = true;
			model.Transaction.OriginApplication = TransactionOrigin.Accounting;
			model.Transaction.InventoryDate = DateTime.Now;
			model.Transaction.TransactionDateTime = DateTime.Now;
			model.Transaction.UpdatedBy = this.Security.UserID;
			model.Transaction.UpdatedDate = DateTime.Now;
			model.Transaction.ShipmentNumber = string.Empty;

			if ((alias.MultipleLineItems == false) && (alias.LineItemFieldCollection.Count != 0))
			{
				LineItemDO lineItem;
				switch (model.Transaction.TransTypeID)
				{
						// When multiple line item is false we must create a Regrade Line Item DO instead
						// of a line item DO.  
					case TransactionTypes.T15_PrimaryRegrade:
					case TransactionTypes.T16_SecondaryRegrade:
						var regradeLineItemDO = new RegradeLineItemDO();
						model.Transaction.LineItems.Add(regradeLineItemDO);
						break;

					case TransactionTypes.T23_StorageTransfer:
						var transferLineItemDO = new StorageTransferLineItemDO();
						model.Transaction.LineItems.Add( transferLineItemDO );
						break;

					default:
						lineItem = new LineItemDO();
						model.Transaction.LineItems.Add( lineItem );
						break;
				}

				lineItem = ( model.Transaction.LineItems[0] );
				lineItem.Quantity.NullableGross = null;
				lineItem.Quantity.NullableNet = null;
				lineItem.LineNumber = 1;
			}

			model.LineItem = model.Transaction.LineItems[0];
		}

		[HttpGet]
		[Route( "TransactionEditorAdd/{aliasName}" )]
		public ActionResult TransactionEditorAdd( string aliasName )
		{
			return this.RedirectToAction("TransactionEditor/" + aliasName + "/");
		}

		[NonAction]
		private void GetFieldInfo(TransactionAliasClass alias, TransactionEditorViewModel model)
		{
			model.Fields = new List<TransactionAliasFieldClass>();
			
			var fields = alias.GetOrderedFields( TRANSACTION_SECTION_TYPE.BODY, dispatchFields: false );

			foreach ( FieldClass field in fields )
			{
				if (field is TransactionAliasFieldClass)
				{
					model.Fields.Add((TransactionAliasFieldClass) field);
				}
			}
		}
    }
}
