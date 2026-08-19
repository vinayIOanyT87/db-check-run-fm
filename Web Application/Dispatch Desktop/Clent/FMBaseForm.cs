namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Drawing;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.ServiceModel;
	using System.Windows.Forms;
	using System.Windows.Forms.VisualStyles;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public class FMBaseForm : Form
	{
		protected SecurityClass Security;
		protected DateTimeFormatInfo SiteDateTimeFormatInfo;

		private static object applicationExiting = false;

		public bool ApplicationExitingFromError
		{
			get
			{
				bool toRet;

				lock (applicationExiting)
				{
					toRet = (bool) applicationExiting;
				}

				return toRet;
			}
		}

		public FMBaseForm()
		{
			ToolStripManager.Renderer = new ToolStripAeroRenderer(ToolbarTheme.Toolbar);
		}

		protected void GetSecurity()
		{
			this.Security = (SecurityClass) AppDomain.CurrentDomain.GetData("Security");
			if (this.Security == null)
			{
				throw new ApplicationException("Security object is null");
			}
		}

		// added to retrieve the site date time format information (IGO 2010-Aug-13)
		protected void GetSiteDateTimeFormatInfo()
		{
			this.SiteDateTimeFormatInfo = (DateTimeFormatInfo) AppDomain.CurrentDomain.GetData("SiteDateTimeFormatInfo");

			if (null == this.SiteDateTimeFormatInfo)
			{
				throw new ApplicationException("SiteDateTimeFormatInfo object is null");
			}
		}

		protected void LoadEquipment(DataSet set, EquipmentCollectionClass equipmentCollection)
		{
			if (set != null && set.Tables.Count != 0)
			{
				var site =
					FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(this.Security, this.Security.SiteGuid));

				foreach (DataRow row in set.Tables[0].Rows)
				{
					var equipment = new EquipmentClass(site)
									{
										IdentityGuid = (Guid) row["EquipmentGuid"],
										SiteGuid = (Guid) row["SiteGuid"],
										ID = row["ID"] as string,
										Xref = row["Xref"] as string,
										FuelingType = (FUELING_TYPES) (short) row["FuelingType"],
										ProductGuid = row.IsNull("ProductGuid") ? Guid.Empty : (Guid) row["ProductGuid"],
										FuelCardGuid = row.IsNull("FuelCardGuid") ? Guid.Empty : (Guid) row["FuelCardGuid"],
										MasterRecordGuid = (Guid) row["_MasterRecordGuid"]
									};
					equipmentCollection.Add(equipment);
				}
			}
		}

		protected void ErrorHandler(Exception except, bool bFatalError)
		{
			ISynchronizeInvoke synchronizeInvoke = this;

			// Check if the event was generated from another
			// thread and needs invoke instead
			if (synchronizeInvoke.InvokeRequired)
			{
				var tempDelegate = new DispatchDataAccess.OnErrorHandler(this.ErrorHandler);
				var results = new Object[] { except, bFatalError };
				synchronizeInvoke.Invoke(tempDelegate, results);
				return;
			}


			lock (applicationExiting)
			{
				if ((bool) applicationExiting == false)
				{
					if (bFatalError)
					{
						string message = except.Message;

						if (except.InnerException != null)
						{

							LogErrorMessage(except.InnerException.Message);
							LogErrorMessage(except.InnerException.StackTrace);
							message = except.InnerException.Message;
						}

						var fatalErrorEx = except as FMFatalErrorException;

						if (fatalErrorEx != null)
						{
							throw new NotImplementedException("Needs FMBusinessObjects to be merged");
							//message += " " + FMFatalErrorHandlerClass.ShutdownMessage + " " + FMFatalErrorHandlerClass.ContactMessage;
						}

						LogErrorMessage(except.Message);
						LogErrorMessage(except.StackTrace);

						LogErrorMessage(message);

						applicationExiting = true;

						var shutDown = new ShutdownForm
						{
							ErrorMessage = message
						};
						shutDown.ShowDialog(this);
						this.Close();

						Application.Exit();
						return;
					}
				}
			}

			this.ErrorHandler(except);
		}

		protected void ErrorHandler(Exception except)
		{

			// Process unhandled FMFatalErrorException type and if Dispatch has been
			// shut down as a result then notify the user and stop all processing.
			var fatalErrorEx = except as FMFatalErrorException;

			if (fatalErrorEx == null)
			{
				var fatalErr2 = except as FaultException<FMFatalErrorException>;
				if (fatalErr2 != null)
				{
					fatalErrorEx = fatalErr2.Detail;
				}
			}

			if (fatalErrorEx != null)
			{
				if (this.Security == null)
				{
					this.GetSecurity();
				}
				bool shutdownDispatch = FMChannelHelper.MakeCall<IClientDispatchService, Boolean>(x => x.ProcessFatalError(this.Security, fatalErrorEx));
				if (shutdownDispatch)
				{
					this.ErrorHandler(except, true);
					return;
				}
			}

			string message = except.Message;

			if (except is SqlException)
			{
				LogErrorMessage(except.Message);
				MessageBox.Show(this, "Sql Exception; see application event log for details.", "FuelsManager Dispatch");
			}
			else if (except.GetType() == typeof(InvalidOperationException) && except.Message.Contains("Invoke or BeginInvoke cannot"))
			{
				//do nothing, just prevent from getting to user
			}
			else if (except.GetType() == typeof(DataException) && except.Message.Contains("DataTable must be set prior to using DataView"))
			{
				//do nothing, just prevent from getting to user
			}
			else
			{
				if (except.InnerException != null)
				{
					LogErrorMessage(except.InnerException.Message);
					LogErrorMessage(except.InnerException.StackTrace);
					message = except.InnerException.Message;
				}

				LogErrorMessage(except.Message);
				LogErrorMessage(except.StackTrace);

				this.ErrorHandler(message);
			}
		}

		protected void ErrorHandler(string message)
		{
			LogErrorMessage(message);

			lock (applicationExiting)
			{
				if ((bool) applicationExiting == false)
				{
					MessageBox.Show(this, message, "FuelsManager Dispatch");
				}
			}
		}

		private static void LogErrorMessage(string message)
		{
			var log = new EventLog
			{
				Source = "FuelsManager"
			};
			log.WriteEntry(message, EventLogEntryType.Error);
		}

		protected void CheckLockDates(SiteClass site, TransactionDO transaction)
		{
			if (transaction.InventoryDate <= site._AdministrativeLockDate.Value)
			{
				throw new Exception("Inventory date must be after the Administrative Lock Date.");
			}

			// The user must not have the Perform Closeout and the Configure Accounting
			// right for the operational lock date to take effect. If the user has either
			// one or the other, then the operational lock date check is ignored.
			if ((this.Security.HasRight(RIGHT.CONFIGURE_ACCOUNTING) == false) &&
				(this.Security.HasRight(RIGHT.PERFORM_CLOSEOUT) == false) &&
				(transaction.InventoryDate <= site._OperationalLockDate.Value))
			{
				throw new Exception("Inventory date must be after the Operational Lock Date.");
			}
		}

		protected void CheckForAndDisplayWarningMessages(SaveTransactionsResultDO resultDO)
		{
			if (resultDO.Results.Count > 0)
			{
				bool found = false;

				string msg = "Save transaction warnings";
				msg = msg + "! ";

				foreach (TransactionValidationResult result in resultDO.Results)
				{
					foreach (string error in result.WarningList)
					{
						msg += "\n\r" + error;
						found = true;
					}
				}

				if (found)
				{
					throw new Exception(msg);
				}
			}
		}

		protected void UndispatchTransaction(TransactionDO transaction)
		{
			this.UndispatchTransaction(transaction, true);
		}

		protected void UndispatchTransaction(TransactionDO transaction, bool saveTransaction)
		{
			PersonClass Operator = null;

			if (transaction.OperatorPersonnelGuid != Guid.Empty)
			{
				Operator = FMChannelHelper.MakeCall<IClientDispatchService, PersonClass>(x => x.GetPerson(this.Security, transaction.OperatorPersonnelGuid));

				if (Operator.IdentityGuid != Guid.Empty)
				{
					Operator.Status = PersonClass.STATUS.In;
				}
			}

			LineItemDO lineItem = transaction.LineItems[0];

			transaction.Status = TransactionStatus.Requested;
			lineItem.Status = TransactionStatus.Requested;

			transaction.OperatorPersonnelGuid = Guid.Empty;
			transaction.OperatorID = string.Empty;
			transaction.OperatorName = string.Empty;
			transaction.DispatchedDateTime = null;
			transaction.IssuePoint = String.Empty;
			transaction.IssuePointNumber = String.Empty;

			switch (transaction.TransTypeID)
			{
				case TransactionTypes.T4_SecondaryDefuel:
				case TransactionTypes.T7_FillStand:
					transaction.DestinationEQ1 = new EquipmentDO();
					lineItem.DestinationEQ = new EquipmentDO();
					break;

				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T10_Unload:
				case TransactionTypes.T12_InventoryNotAffected:
					transaction.SourceEQ1 = new EquipmentDO();
					lineItem.SourceEQ = new EquipmentDO();
					break;

				default:
					throw new ApplicationException("Unhandled transaction type passed to dispatch.");

			}

			// Finish and save
			if (saveTransaction)
			{
				this.SaveTransaction(transaction, Operator);
			}
		}

		protected TransactionDO GetTransaction(string transID)
		{

			TransactionDO transaction = FMChannelHelper.MakeCall<IClientDispatchService, TransactionDO>(x => x.GetTransactionByTransID(Security, transID));

			if (transaction == null)
			{
				throw new ArgumentException("Transaction not found");
			}

			return transaction;
		}

		/// <summary>
		/// Gets the transaction.
		/// </summary>
		/// <param name="transactionGuid">The transaction GUID.</param>
		/// <returns>The requested transaction data object.</returns>
		/// <exception cref="System.ArgumentException">Transaction not found.</exception>
		protected TransactionDO GetTransaction(Guid transactionGuid)
		{

			var transaction = FMChannelHelper.MakeCall<IClientDispatchService, TransactionDO>(x => x.GetTransactionByTransactionGuid(Security, transactionGuid));

			if (transaction == null)
			{
				throw new ArgumentException("Transaction not found");
			}

			return transaction;
		}

		/// <summary>
		/// This method will auto generated a document number.
		/// </summary>
		protected string GenerateDocumentNumbers(TransactionTypes transTypeId)
		{
			return FMChannelHelper.MakeCall<IClientDispatchService, string>(
						x => x.GenerateDocumentNumbers(this.Security, transTypeId));
		}

		protected SaveTransactionsResultDO SaveTransaction(TransactionDO transaction)
		{
			return this.SaveTransaction(transaction, null);
		}

		protected SaveTransactionsResultDO SaveTransaction(object transactions, PersonClass person)
		{
			SaveTransactionsResultDO results = null;
			List<TransactionDO> transactionList;

			if (transactions is List<TransactionDO>)
			{
				transactionList = transactions as List<TransactionDO>;
			}
			else if (transactions is TransactionDO)
			{
				transactionList = new List<TransactionDO> { transactions as TransactionDO };
			}
			else
			{
				throw new Exception("Invalid Transaction Object passed to SaveTransaction");
			}

			try
			{
				SiteClass site =
					FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(
						x =>
						x.GetSite(
							this.Security,
							this.Security.SiteGuid
							));

				var saveSR = new SaveTransactionsSR
				             {
					             IndividualDbTransaction = false,
					             Security = this.Security,
					             CurrentSiteGuid = this.Security.SiteGuid,
					             ConvertUnits = true,
					             Operator = person
				             };

				foreach (TransactionDO transaction in transactionList)
				{
					// Check the aviation and capitalize flags against the product configuration
					LineItemDO lineItem = transaction.LineItems[0];

					ProductClass product =
						FMChannelHelper.MakeCall<IClientDispatchService, ProductClass>(x => x.GetProduct(this.Security, lineItem.ProductGuid));

					transaction.Flag02 = product.UserData1.Equals("YES", StringComparison.CurrentCultureIgnoreCase);
					transaction.Flag01 = product.UserData2.Equals("YES", StringComparison.CurrentCultureIgnoreCase);

					transaction.UserData[TransactionDO.USER_DATA_KEY_09] = "9 (LOCAL)";

					var transactionAlias =
						FMChannelHelper.MakeCall<IClientDispatchService, TransactionAliasClass>(
							x => x.GetTransactionAliasFromAliasGuid(this.Security, transaction.TransactionAliasGuid, false));

					var unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, null);
					unitsHelper.SetUnits(transaction, 0);

					foreach (LineItemDO item in transaction.LineItems)
					{
						ProductClass prod = FMChannelHelper.MakeCall<IClientDispatchService, ProductClass>(x => x.GetProduct(this.Security, item.ProductGuid));

						unitsHelper.SetUnits(item, prod.ProductType, product);
					}

					saveSR.Transactions.Add(transaction);

					if (saveSR.Transactions.Count >= 5)
					{
						results = FMChannelHelper.MakeCall<IClientDispatchService, SaveTransactionsResultDO>(x => x.SaveTransactions(saveSR));

						this.CheckForAndDisplayWarningMessages(results);
						saveSR.Transactions.Clear();
					}
				}

				if (saveSR.Transactions.Count > 0)
				{
					results = FMChannelHelper.MakeCall<IClientDispatchService, SaveTransactionsResultDO>(x => x.SaveTransactions(saveSR));
					this.CheckForAndDisplayWarningMessages(results);
				}
			}
			catch (FaultException<SaveTransactionsException> saveExcept)
			{
				string errorMessage = "Save Transaction Failed!";

				foreach (TransactionValidationResult result in saveExcept.Detail.Results)
				{
					foreach (string error in result.ErrorList)
					{
						errorMessage += "\n\r" + error;
					}
				}

				this.ErrorHandler(errorMessage);

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return results;
		}

		protected Guid SaveTransactionNote(Guid transGuid, string note, string transactionNote)
		{
			throw new NotImplementedException("Needs FMBusinessObjects to be merged");
			//var sr = new TransactionNoteSR { TransGuid = transGuid, Security = this.Security};
			//var sr = new TransactionNoteSR { Security = this.Security };

			//if (transactionNote.Length == 0)
			//{
			//	sr.Note = note;
			//}
			//else
			//{
			//	sr.Note = note + " - " + transactionNote;
			//}

			//sr.UpdatedBy = this.Security.UserID;
			//Guid notesGuid = FMChannelHelper.MakeCall<IClientDispatchService, Guid>(x => x.ProcessTransactionNoteServiceRequest(sr));

			//return notesGuid;
		}

		protected bool UpdatedItemInList(ComboBox sender, KeyPressEventArgs e)
		{
			if (sender == null)
			{
				return false;
			}

			if (e.KeyChar == '\b')
			{
				return true;
			}

			string currentText = sender.Text;
			currentText = currentText.Remove(sender.SelectionStart, sender.SelectionLength);
			string newText = currentText.Insert(sender.SelectionStart, new String(e.KeyChar, 1));

			if (sender.FindString(newText) == -1)
			{
				return false;
			}

			return true;
		}

		protected void RenumberGrid(DataGridView grid)
		{
			// reset the numbers in the rowheaders.  Can't use UpdateView, as that changes the selected index
			// of the vehicleComboBox, causing unintended, infinite recursion
			int index = 1;

			foreach (DataGridViewRow row in grid.Rows)
			{
				row.HeaderCell.Value = index.ToString(CultureInfo.InvariantCulture);
				++index;
			}
		}

		protected void SaveTransactionWithServiceRequest(SecurityClass security, SaveTransactionsSR serviceRequest)
		{
			try
			{
				SaveTransactionsResultDO results = FMChannelHelper.MakeCall<IClientDispatchService, SaveTransactionsResultDO>(
					x => x.SaveTransactions(serviceRequest));

				this.CheckForAndDisplayWarningMessages(results);
			}
			catch (Exception exception)
			{
				var faultException = exception as FaultException<SaveTransactionsException>;

				if (faultException != null)
				{
					FaultException<SaveTransactionsException> saveTransactionsException = faultException;

					if (saveTransactionsException.Detail.Results.Count >= 1
						 && saveTransactionsException.Detail.Results[0] != null
						 && saveTransactionsException.Detail.Results[0].ErrorList.Count >= 1)
					{
						MessageBox.Show(saveTransactionsException.Detail.Results[0].ErrorList[0], "Error");
					}
					else
					{
						MessageBox.Show("Unknown SaveTransactionException", "Error");
					}
				}
			}

		}
	}

	//*****code to render Windows in Vista Theme******
	public enum ToolbarTheme
	{
		Toolbar,
		MediaToolbar,
		CommunicationsToolbar,
		BrowserTabBar,
		HelpBar
	}

	/// <summary>Renders a toolstrip using the UxTheme API via VisualStyleRenderer and a specific style.</summary> 
	/// <remarks>Perhaps surprisingly, this does not need to be disposable.</remarks> 
	public class ToolStripAeroRenderer : ToolStripSystemRenderer
	{
		VisualStyleRenderer renderer;

		public ToolStripAeroRenderer(ToolbarTheme theme)
		{
			this.Theme = theme;
		}

		/// <summary> 
		/// It shouldn't be necessary to P/Invoke like this, however VisualStyleRenderer.GetMargins 
		/// misses out a parameter in its own P/Invoke. 
		/// </summary> 
		static internal class NativeMethods
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct Margins
			{
				public int cxLeftWidth;
				public int cxRightWidth;
				public int cyTopHeight;
				public int cyBottomHeight;
			}

			[DllImport("uxtheme.dll")]
			public extern static int GetThemeMargins(IntPtr hTheme, 
													IntPtr hdc, 
													int iPartId, 
													int iStateId, 
													int iPropId, 
													IntPtr rect, 
													out Margins pMargins);
		}

		// See http://msdn2.microsoft.com/en-us/library/bb773210.aspx - "Parts and States" 
		// Only menu-related parts/states are needed here, VisualStyleRenderer handles most of the rest. 
		enum MenuParts
		{
			ItemTMSchema = 1,
			DropDownTMSchema = 2,
			BarItemTMSchema = 3,
			BarDropDownTMSchema = 4,
			ChevronTMSchema = 5,
			SeparatorTMSchema = 6,
			BarBackground = 7,
			BarItem = 8,
			PopupBackground = 9,
			PopupBorders = 10,
			PopupCheck = 11,
			PopupCheckBackground = 12,
			PopupGutter = 13,
			PopupItem = 14,
			PopupSeparator = 15,
			PopupSubmenu = 16,
			SystemClose = 17,
			SystemMaximize = 18,
			SystemMinimize = 19,
			SystemRestore = 20
		}

		enum MenuBarStates
		{
			Active = 1,
			Inactive = 2
		}

		enum MenuBarItemStates
		{
			Normal = 1,
			Hover = 2,
			Pushed = 3,
			Disabled = 4,
			DisabledHover = 5,
			DisabledPushed = 6
		}

		enum MenuPopupItemStates
		{
			Normal = 1,
			Hover = 2,
			Disabled = 3,
			DisabledHover = 4
		}

		enum MenuPopupCheckStates
		{
			CheckmarkNormal = 1,
			CheckmarkDisabled = 2,
			BulletNormal = 3,
			BulletDisabled = 4
		}

		enum MenuPopupCheckBackgroundStates
		{
			Disabled = 1,
			Normal = 2,
			Bitmap = 3
		}

		enum MenuPopupSubMenuStates
		{
			Normal = 1,
			Disabled = 2
		}

		enum MarginTypes
		{
			Sizing = 3601,
			Content = 3602,
			Caption = 3603
		}

		private const int RebarBackground = 6;

		Padding GetThemeMargins(IDeviceContext dc, MarginTypes marginType)
		{
			try
			{
				IntPtr hDc = dc.GetHdc();
				NativeMethods.Margins margins;

				if (0 == NativeMethods.GetThemeMargins(
														this.renderer.Handle,
														hDc,
														this.renderer.Part,
														this.renderer.State,
														(int)marginType,
														IntPtr.Zero,
														out margins))
				{
					return new Padding(margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight);
				}

				return new Padding(0);
			}
			finally
			{
				dc.ReleaseHdc();
			}
		}

		private static int GetItemState(ToolStripItem item)
		{
			bool hot = item.Selected;

			if (item.IsOnDropDown)
			{
				if (item.Enabled)
					return hot ? (int) MenuPopupItemStates.Hover : (int) MenuPopupItemStates.Normal;
				return hot ? (int) MenuPopupItemStates.DisabledHover : (int) MenuPopupItemStates.Disabled;
			}

			if (item.Pressed)
			{
				return item.Enabled ? (int) MenuBarItemStates.Pushed : (int) MenuBarItemStates.DisabledPushed;
			}

			if (item.Enabled)
			{
				return hot ? (int) MenuBarItemStates.Hover : (int) MenuBarItemStates.Normal;
			}

			return hot ? (int) MenuBarItemStates.DisabledHover : (int) MenuBarItemStates.Disabled;
		}

		public ToolbarTheme Theme
		{
			get;
			set;
		}

		private string RebarClass
		{
			get
			{
				return this.SubclassPrefix + "Rebar";
			}
		}

		private string MenuClass
		{
			get
			{
				return this.SubclassPrefix + "Menu";
			}
		}

		private string SubclassPrefix
		{
			get
			{
				switch (this.Theme)
				{
					case ToolbarTheme.MediaToolbar: return "Media::";
					case ToolbarTheme.CommunicationsToolbar: return "Communications::";
					case ToolbarTheme.BrowserTabBar: return "BrowserTabBar::";
					case ToolbarTheme.HelpBar: return "Help::";
					default: return string.Empty;
				}
			}
		}

		private bool EnsureRenderer()
		{
			if (!this.IsSupported)
			{
				return false;
			}

			if (this.renderer == null)
			{
				this.renderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);
			}

			return true;
		}

		// Gives parented ToolStrips a transparent background. 
		protected override void Initialize(ToolStrip toolStrip)
		{
			if (toolStrip.Parent is ToolStripPanel)
			{
				toolStrip.BackColor = Color.Transparent;
			}

			base.Initialize(toolStrip);
		}

		// Using just ToolStripManager.Renderer without setting the Renderer individually per ToolStrip means 
		// that the ToolStrip is not passed to the Initialize method. ToolStripPanels, however, are. So we can  
		// simply initialize it here too, and this should guarantee that the ToolStrip is initialized at least  
		// once. Hopefully it isn't any more complicated than this. 
		protected override void InitializePanel(ToolStripPanel toolStripPanel)
		{
			foreach (Control control in toolStripPanel.Controls)
			{
				var toolStrip = control as ToolStrip;

				if (toolStrip != null)
				{
					this.Initialize(toolStrip);
				}
			}

			base.InitializePanel(toolStripPanel);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				this.renderer.SetParameters(this.MenuClass, (int) MenuParts.PopupBorders, 0);

				if (e.ToolStrip.IsDropDown)
				{
					Region oldClip = e.Graphics.Clip;

					// Tool strip borders are rendered *after* the content, for some reason. 
					// So we have to exclude the inside of the popup otherwise we'll draw over it. 
					Rectangle insideRect = e.ToolStrip.ClientRectangle;
					insideRect.Inflate(-1, -1);
					e.Graphics.ExcludeClip(insideRect);

					this.renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);

					// Restore the old clip in case the Graphics is used again (does that ever happen?) 
					e.Graphics.Clip = oldClip;
				}
			}
			else
			{
				base.OnRenderToolStripBorder(e);
			}
		}

		Rectangle GetBackgroundRectangle(ToolStripItem item)
		{
			if (!item.IsOnDropDown)
			{
				return new Rectangle(new System.Drawing.Point(), item.Bounds.Size);
			}

			// For a drop-down menu item, the background rectangles of the items should be touching vertically. 
			// This ensures that's the case. 
			Rectangle rect = item.Bounds;

			// The background rectangle should be inset two pixels horizontally (on both sides), but we have  
			// to take into account the border. 
			rect.X = item.ContentRectangle.X + 1;
			rect.Width = item.ContentRectangle.Width - 1;

			// Make sure we're using all of the vertical space, so that the edges touch. 
			rect.Y = 0;
			return rect;
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				int partID = e.Item.IsOnDropDown ? (int) MenuParts.PopupItem : (int) MenuParts.BarItem;
				this.renderer.SetParameters(this.MenuClass, partID, GetItemState(e.Item));

				Rectangle bgRect = this.GetBackgroundRectangle(e.Item);
				this.renderer.DrawBackground(e.Graphics, bgRect, bgRect);
			}
			else
			{
				base.OnRenderMenuItemBackground(e);
			}
		}

		protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				// Draw the background using Rebar & RP_BACKGROUND (or, if that is not available, fall back to 
				// Rebar.Band.Normal) 
				if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(this.RebarClass, RebarBackground, 0)))
				{
					this.renderer.SetParameters(this.RebarClass, RebarBackground, 0);
				}
				else
				{
					this.renderer.SetParameters(this.RebarClass, 0, 0);
				}

				if (this.renderer.IsBackgroundPartiallyTransparent())
				{
					this.renderer.DrawParentBackground(e.Graphics, e.ToolStripPanel.ClientRectangle, e.ToolStripPanel);
				}

				this.renderer.DrawBackground(e.Graphics, e.ToolStripPanel.ClientRectangle);
				e.Handled = true;
			}
			else
			{
				base.OnRenderToolStripPanelBackground(e);
			}
		}

		// Render the background of an actual menu bar, dropdown menu or toolbar. 
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				if (e.ToolStrip.IsDropDown)
				{
					this.renderer.SetParameters(this.MenuClass, (int) MenuParts.PopupBackground, 0);
				}
				else
				{
					// It's a MenuStrip or a ToolStrip. If it's contained inside a larger panel, it should have a 
					// transparent background, showing the panel's background. 

					if (e.ToolStrip.Parent is ToolStripPanel)
					{
						// The background should be transparent, because the ToolStripPanel's background will be visible. 
						// (Of course, we assume the ToolStripPanel is drawn using the same theme, but it's not my fault 
						// if someone does that.) 
						return;
					}
					
					// A lone toolbar/menubar should act like it's inside a toolbox, I guess. 
					// Maybe I should use the MenuClass in the case of a MenuStrip, although that would break 
					// the other themes... 
					if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement(this.RebarClass, RebarBackground, 0)))
					{
						this.renderer.SetParameters(this.RebarClass, RebarBackground, 0);
					}
					else
					{
						this.renderer.SetParameters(this.RebarClass, 0, 0);
					}
				}

				if (this.renderer.IsBackgroundPartiallyTransparent())
				{
					this.renderer.DrawParentBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.ToolStrip);
				}

				this.renderer.DrawBackground(e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds);
			}
			else
			{
				base.OnRenderToolStripBackground(e);
			}
		}

		// The only purpose of this override is to change the arrow colour. 
		// It's OK to just draw over the default arrow since we also pass down arrow drawing to the system renderer. 
		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				var toolStripSplitButton = (ToolStripSplitButton) e.Item;
				base.OnRenderSplitButtonBackground(e);

				// It doesn't matter what colour of arrow we tell it to draw. OnRenderArrow will compute it from the item anyway. 
				this.OnRenderArrow(new ToolStripArrowRenderEventArgs(e.Graphics, toolStripSplitButton, toolStripSplitButton.DropDownButtonBounds, Color.Red, ArrowDirection.Down));
			}
			else
			{
				base.OnRenderSplitButtonBackground(e);
			}
		}

		Color GetItemTextColor(ToolStripItem item)
		{
			int partId = item.IsOnDropDown ? (int) MenuParts.PopupItem : (int) MenuParts.BarItem;
			this.renderer.SetParameters(this.MenuClass, partId, GetItemState(item));
			return this.renderer.GetColor(ColorProperty.TextColor);
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				e.TextColor = this.GetItemTextColor(e.Item);
			}

			base.OnRenderItemText(e);
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				if (e.ToolStrip.IsDropDown)
				{
					this.renderer.SetParameters(this.MenuClass, (int) MenuParts.PopupGutter, 0);
					// The AffectedBounds is usually too small, way too small to look right. Instead of using that, 
					// use the AffectedBounds but with the right width. Then narrow the rectangle to the correct edge 
					// based on whether or not it's RTL. (It doesn't need to be narrowed to an edge in LTR mode, but let's 
					// do that anyway.) 
					// Using the DisplayRectangle gets roughly the right size so that the separator is closer to the text. 
					Padding margins = this.GetThemeMargins(e.Graphics, MarginTypes.Sizing);
					int extraWidth = (e.ToolStrip.Width - e.ToolStrip.DisplayRectangle.Width - margins.Left - margins.Right - 1) - e.AffectedBounds.Width;
					Rectangle rect = e.AffectedBounds;
					rect.Y += 2;
					rect.Height -= 4;
					int sepWidth = this.renderer.GetPartSize(e.Graphics, ThemeSizeType.True).Width;

					if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
					{
						rect = new Rectangle(rect.X - extraWidth, rect.Y, sepWidth, rect.Height);
						rect.X += sepWidth;
					}
					else
					{
						rect = new Rectangle(rect.Width + extraWidth - sepWidth, rect.Y, sepWidth, rect.Height);
					}

					this.renderer.DrawBackground(e.Graphics, rect);
				}
			}
			else
			{
				base.OnRenderImageMargin(e);
			}
		}

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			if (e.ToolStrip.IsDropDown && this.EnsureRenderer())
			{
				this.renderer.SetParameters(this.MenuClass, (int) MenuParts.PopupSeparator, 0);
				var rect = new Rectangle(e.ToolStrip.DisplayRectangle.Left, 0, e.ToolStrip.DisplayRectangle.Width, e.Item.Height);
				this.renderer.DrawBackground(e.Graphics, rect, rect);
			}
			else
			{
				base.OnRenderSeparator(e);
			}
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				Rectangle bgRect = this.GetBackgroundRectangle(e.Item);
				bgRect.Width = bgRect.Height;

				// Now, mirror its position if the menu item is RTL. 
				if (e.Item.RightToLeft == RightToLeft.Yes)
				{
					bgRect = new Rectangle(e.ToolStrip.ClientSize.Width - bgRect.X - bgRect.Width, bgRect.Y, bgRect.Width, bgRect.Height);
				}

				this.renderer.SetParameters(this.MenuClass, (int) MenuParts.PopupCheckBackground, e.Item.Enabled ? (int) MenuPopupCheckBackgroundStates.Normal : (int) MenuPopupCheckBackgroundStates.Disabled);
				this.renderer.DrawBackground(e.Graphics, bgRect);

				Rectangle checkRect = e.ImageRectangle;
				checkRect.X = bgRect.X + bgRect.Width / 2 - checkRect.Width / 2;
				checkRect.Y = bgRect.Y + bgRect.Height / 2 - checkRect.Height / 2;

				// I don't think ToolStrip even supports radio box items, so no need to render them. 
				this.renderer.SetParameters(this.MenuClass, (int) MenuParts.PopupCheck, e.Item.Enabled ? (int) MenuPopupCheckStates.CheckmarkNormal : (int) MenuPopupCheckStates.CheckmarkDisabled);

				this.renderer.DrawBackground(e.Graphics, checkRect);
			}
			else
			{
				base.OnRenderItemCheck(e);
			}
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			// The default renderer will draw an arrow for us (the UXTheme API seems not to have one for all directions), 
			// but it will get the colour wrong in many cases. The text colour is probably the best colour to use. 
			if (this.EnsureRenderer())
			{
				e.ArrowColor = this.GetItemTextColor(e.Item);
			}

			base.OnRenderArrow(e);
		}

		protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
		{
			if (this.EnsureRenderer())
			{
				// BrowserTabBar::Rebar draws the chevron using the default background. Odd. 
				string rebarClass = this.RebarClass;

				if (this.Theme == ToolbarTheme.BrowserTabBar)
				{
					rebarClass = "Rebar";
				}

				int state = VisualStyleElement.Rebar.Chevron.Normal.State;

				if (e.Item.Pressed)
				{
					state = VisualStyleElement.Rebar.Chevron.Pressed.State;
				}
				else if (e.Item.Selected)
				{
					state = VisualStyleElement.Rebar.Chevron.Hot.State;
				}

				this.renderer.SetParameters(rebarClass, VisualStyleElement.Rebar.Chevron.Normal.Part, state);
				this.renderer.DrawBackground(e.Graphics, new Rectangle(System.Drawing.Point.Empty, e.Item.Size));
			}
			else
			{
				base.OnRenderOverflowButtonBackground(e);
			}
		}

		public bool IsSupported
		{
			get
			{
				if (!VisualStyleRenderer.IsSupported)
				{
					return false;
				}

				// Needs a more robust check. It seems mono supports very different style sets. 
				return
						VisualStyleRenderer.IsElementDefined(
								VisualStyleElement.CreateElement("Menu",
										(int) MenuParts.BarBackground,
										(int) MenuBarStates.Active));
			}
		}
	}
}
