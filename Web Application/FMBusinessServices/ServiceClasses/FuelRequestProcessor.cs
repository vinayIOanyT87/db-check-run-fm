// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Receives transaction data from the Fuel Request Form and creates the transaction record
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Receives transaction data from the Fuel Request Form and 
	/// creates the transaction record
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class FuelRequestProcessor : IFuelRequestProcessor
	{
		/// <summary>
		/// Using data entered on the form, create or update a transaction record.
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="serviceRequest">Contains the transaction and other information we need to process the data</param>
		/// <returns>A result object which contains any warnings we need to display</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public FuelRequestResult Process(SecurityClass security, FuelRequestSR serviceRequest)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (serviceRequest == null)
			{
				throw new ArgumentNullException("serviceRequest");
			}

			if (serviceRequest.Transaction == null)
			{
				throw new ArgumentNullException("serviceRequest.Transaction");
			}

			if (!security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			SaveTransactionsSR saveTransactionServiceRequest = new SaveTransactionsSR
				                                                   {
					                                                   Security = security,
					                                                   ConvertUnits = true
				                                                   };

			saveTransactionServiceRequest.Transactions.Add(serviceRequest.Transaction);
			saveTransactionServiceRequest.CurrentSiteGuid = serviceRequest.Transaction.SiteGuid;

			SaveTransactionsProcessor transactionClient = new SaveTransactionsProcessor();
			transactionClient.SaveTransactions(saveTransactionServiceRequest);

			FuelRequestResult result = new FuelRequestResult();

			bool isFuelRequest = serviceRequest.RequestType != FuelRequestType.FillStand 
				&& serviceRequest.RequestType != FuelRequestType.FastLogFillStand;

			if (!isFuelRequest && !serviceRequest.TransactionOriginallyCompleted)
			{
				result.AlertMessage = UpdateTransactionNotesIfVarianceOutOfTolerance(security, serviceRequest);
			}

			return result;
		}

		/// <summary>
		/// If the variance is out of tolerance for the first or second time, update the transaction notes
		/// with a warning.
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="serviceRequest">Contains information we need to determine if the variance is out of tolerance, like the transaction</param>
		/// <returns>The warning message if we determined that we should display one</returns>
		private static string UpdateTransactionNotesIfVarianceOutOfTolerance(SecurityClass security, FuelRequestSR serviceRequest)
		{
			TransactionDO transaction = serviceRequest.Transaction;

			LineItemDO lineItem = transaction.LineItems.Find(matchingLineItem => matchingLineItem.DeleteFlag == false);

			bool isReturnToBulk = (serviceRequest.RequestSubType == FuelRequestSR.ReturnToBulkRequestSubType
			                       || serviceRequest.RequestSubType == FuelRequestSR.PartialReturnToBulkSubType);

			if (string.IsNullOrEmpty(transaction.Notes)
				&& serviceRequest.RequestSubType != FuelRequestSR.PartialFillRequestSubType
                && serviceRequest.RequestSubType != FuelRequestSR.PartialReturnToBulkSubType
				&& transaction.Status != TransactionStatus.Cancelled)
			{
				// If this is a return to bulk request, the fueling vehicle is the source equipment record.
				// If this is a fill or partial fill, the fueling vehicle is the destination equipment record.
				EquipmentClass fillstandEquipment = null;

				EquipmentsClass equipments = new EquipmentsClass();

				if (!isReturnToBulk)
				{
					fillstandEquipment = equipments.Get(security, lineItem.DestinationEQ.EquipmentGuid);
				}
				else
				{
					fillstandEquipment = equipments.Get(security, lineItem.SourceEQ.EquipmentGuid);
				}

				// We may write a comment to the notes record, but only if the consecutive oos variance value hasn't changed
				// since we save the transaction
				if (fillstandEquipment != null && fillstandEquipment.IdentityGuid != Guid.Empty
					&& serviceRequest.CurrentConsecutiveOOSVariance != fillstandEquipment.Consecutive_OOS_Variance)
				{
					int checkInterval = fillstandEquipment.Consecutive_OOS_Variance % 3;

					string message = string.Empty;

					if (checkInterval == 1 || checkInterval == -1)
					{
						message = string.Format("First occurrence of 2% {0}.", (checkInterval > 0) ? "gain" : "loss");
					}
					else if (checkInterval == 2 || checkInterval == -2)
					{
						message = string.Format("Second occurrence of 2% {0}.", (checkInterval > 0) ? "gain" : "loss");
                    }
                    // the third occurrence has to be dealt with as we must launch an alert for this when a message has already
                    // been entered. this is an improvement over the previous working of the page
                    else if ((fillstandEquipment.Consecutive_OOS_Variance != 0) && ((fillstandEquipment.Consecutive_OOS_Variance % 3) == 0))
                    {
                        message = string.Format("Deviation is >= 2 % for three consecutive actions.");
                    }

                    if (!string.IsNullOrEmpty(message))
					{
						TransactionNoteSR noteRequest = new TransactionNoteSR
							                                {
								                                TransGuid = transaction.TransactionGuid,
								                                UpdatedBy = security.UserID,
								                                Security = security
							                                };

                        if (string.IsNullOrEmpty(transaction.Notes))
                        {
                            noteRequest.Note = message;
                        }
                        else
                        {
                            noteRequest.Note = message + Environment.NewLine + transaction.Notes;
                        }

						TransactionNoteProcessorClass notes = new TransactionNoteProcessorClass();
						notes.Process(noteRequest);

						return message;
					}
				}
			}

			return string.Empty;
		}
	}
}