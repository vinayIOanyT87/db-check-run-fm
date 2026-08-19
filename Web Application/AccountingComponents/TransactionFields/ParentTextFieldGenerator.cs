namespace TransactionFields
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	public class ParentTextFieldGenerator : TextFieldGenerator
	{
		#region Attributes
		private List<TransactionDO> parentTransactions;
		private bool multipleFlag;
		#endregion // Attributes

		public ParentTextFieldGenerator()
		{
			this.multipleFlag = false;
		}

		public ParentTextFieldGenerator(bool multiple)
		{
			this.multipleFlag = multiple;
		}

		public override void Generate(bool editable)
		{
			base.Generate(editable);

			// post processing if showing multi-line
			if (this.multipleFlag)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

					if (textBox != null)
					{
						// make it multi-line and re-structure the contents
						textBox.TextMode = TextBoxMode.MultiLine;

						var valueList = this.GetDataValue() as List<string>;

						if (valueList != null)
						{
							foreach (string value in valueList)
							{
								textBox.Text += value + "\n";
							}
						}
					}
				}
			}
		}

		protected TransactionDO GetFirstMatchingParent(List<TransactionDO> parentTransList, LineItemDO inLineItem)
		{
			TransactionDO match = null;

			List<TransactionDO> matchingParentList = this.GetMatchingParents(parentTransList, inLineItem);

			if (matchingParentList != null)
			{
				match = matchingParentList[0];
			}

			return match;
		}

		protected List<TransactionDO> GetMatchingParents(List<TransactionDO> parentTransList, LineItemDO inLineItem)
		{
			var result = new List<TransactionDO>();

			foreach (TransactionDO parentTrans in parentTransList)
			{
				foreach (LineItemDO li in parentTrans.LineItems)
				{
					foreach (AssociatedTxDO atx in li.AssociatedTransactions)
					{
						// match the child trans ID to parent's assoc tx trans ID
						if (atx.TransactionLineItemGuid == inLineItem.TransactionLineItemGuid)
						{
							result.Add(parentTrans);
						}
					}
				}
			}

			return result;
		}

		protected List<TransactionDO> LoadParentTransactions(TransactionTypes transactionType, TransactionDO childTransaction)
		{
			// check if the parent has already been loaded
			if (null == this.parentTransactions)
			{
				// load the parent if it has been associated with an invoice
				var sr = new AssociatedTxSR
					        {
						        Security = this.transContext.security,
						        RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions,
						        TransID = childTransaction.TransID
					        };

				AssociatedTxListDO atxList = FMChannelHelper.MakeCall<IAssociatedTxProcessor, AssociatedTxListDO>(x => x.Process(sr));

				var parentTransID = new List<string>();

				if (atxList != null)
				{
					if (atxList.AssociatedTransactions.Tables.Count > 0)
					{
						foreach (DataRow dr in atxList.AssociatedTransactions.Tables[0].Rows)
						{
							if (transactionType == TransactionTypes.T_Maximum)
							{
								parentTransID.Add(dr["TransID"] as string);
							}

							if (dr["LookupTransTypeIndex"] != null && ((TransactionTypes) dr["LookupTransTypeIndex"]) == transactionType)
							{
								parentTransID.Add(dr["TransID"] as string);
							}
						}
					}
				}

				if (parentTransID.Count > 0)
				{
					this.parentTransactions = new List<TransactionDO>();

					foreach (string transID in parentTransID)
					{
						Guid origSiteGuid = this.transContext.security.SiteGuid;

						var transactionSr = new TransactionSR { Security = this.transContext.security };

						// This used to set the sr.Security.SiteIndex to 3, which is the JFLA site.
						transactionSr.Security.SiteGuid = childTransaction.SiteGuid;

						transactionSr.Security.AddRight(RIGHT.VIEW_TRANSACTION_DATA);
						transactionSr.TransID = transID;

						this.parentTransactions.Add(
							FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(transactionSr)));

						// It is necessary to reset the security object with the original
						// site index or else all following transactions will be incorrect.
						this.transContext.security.SiteGuid = origSiteGuid;
					}
				}
			}

			return this.parentTransactions;
		}

		public override object GetNewValue(WebControl control)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					string stringValue = textBox.Text;

					if (this.Required)
					{
						this.cell.BackColor = System.Drawing.Color.Red;

						if (string.IsNullOrEmpty(stringValue))
						{
							throw new FMFieldRequiredException();
						}

						this.cell.BackColor = System.Drawing.Color.Transparent;
					}

					return stringValue;
				}
			}

			return string.Empty;
		}

		#region Abstracts
		public override string FieldID
		{
			get { return string.Empty; }
		}

		protected override short MaxColumns
		{
			get { return (short)this.GetFieldLength(FieldID, 30); }
		}
		#endregion // Abstracts
	}
}
