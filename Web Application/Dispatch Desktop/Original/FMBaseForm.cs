/// <summary>
/// **************************************************************************************************************
/// File name:	FMBaseForm 
/// 
/// Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 2009 
///            This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///				
/// Author(s):	Gray, Kendall, Schaal, Sang
/// 
/// Modification History:
///	 Date:			By:						Reason:
///	 ----------		--------------------	--------------------------------------------
///  2009-10-22     C. Knight       WI# 8675 - Update UpdatedItemInList to check for
///                                     possibility of null combo box being passed in - just
///                                     return false.
/// **************************************************************************************************************
/// </summary>

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.UtilityObjects;

namespace DispatchPrototype
{
	public class FMBaseForm : Form
	{
		protected enum TargetCustomer : int
		{
			BSME = 1,
			MOD = 2
		}

		protected SecurityClass Security = null;
		protected DateTimeFormatInfo SiteDateTimeFormatInfo = null;

		private static object applicationExiting = false;

		public FMBaseForm ( )
		{
			ToolStripManager.Renderer = new ToolStripAeroRenderer ( ToolbarTheme.Toolbar );
		}

		protected void GetSecurity ( )
		{
			Security = (SecurityClass) AppDomain.CurrentDomain.GetData ( "Security" );
			if (Security == null)
			{
				throw new ApplicationException ( "Security object is null" );
			}
		}

		// added to retrieve the site date time format information (IGO 2010-Aug-13)
		protected void GetSiteDateTimeFormatInfo ( )
		{
			SiteDateTimeFormatInfo = (DateTimeFormatInfo) AppDomain.CurrentDomain.GetData ( "SiteDateTimeFormatInfo" );
			if (null == SiteDateTimeFormatInfo)
			{
				throw new ApplicationException ( "SiteDateTimeFormatInfo object is null" );
			}
		}

		// added to retrieve the target customer read from the .config file (IGO 2010-Sep-13)
		protected TargetCustomer GetTargetCustomer ( )
		{
			string targetcustomer = (string) AppDomain.CurrentDomain.GetData ( "TargetCustomer" );
			if (string.IsNullOrEmpty ( targetcustomer ))
			{
				throw new ApplicationException ( "TargetCustomer object is null" );
			}

			// currently only two versions are supported, always default to BSME
			TargetCustomer target;
			if ("MOD" == targetcustomer)
			{
				target = TargetCustomer.MOD;
			}
			else
			{
				target = TargetCustomer.BSME;
			}

			return target;
		}

		protected void ErrorHandler ( Exception except, bool bFatalError )
		{
			lock (applicationExiting)
			{
				if ((bool) applicationExiting == false)
				{
					if (bFatalError)
					{
						string message = except.Message;

						if (except.InnerException != null)
						{
							message = except.InnerException.Message;
						}

						LogErrorMessage ( message );

						applicationExiting = true;

						ShutdownForm shutDown = new ShutdownForm ( );
						shutDown.ErrorMessage = message;
						shutDown.ShowDialog ( this );

						Application.Exit ( );
						return;
					}
				}

				ErrorHandler ( except );

			}
		}

		protected void ErrorHandler ( Exception except )
		{
			if (except.GetType ( ) == typeof ( SqlException ))
			{
			}

			string message = except.Message;

			if (except.InnerException != null)
			{
				message = except.InnerException.Message;
			}

			ErrorHandler ( message );

		}

		protected void ErrorHandler ( string message )
		{
			LogErrorMessage ( message );

			MessageBox.Show ( this, message, "FuelsManager Dispatch" );
		}

		private static void LogErrorMessage ( string message )
		{
			EventLog log = new EventLog ( );
			log.Source = "FuelsManager";
			log.WriteEntry ( message, EventLogEntryType.Error );
		}

		protected void CheckLockDates ( SiteClass site, TransactionDO transaction )
		{
			if (transaction.InventoryDate <= site._AdministrativeLockDate.Value)
			{
				throw new Exception ( "Inventory date must be after the Administrative Lock Date." );
			}

			// The user must not have the Perform Closeout and the Configure Accounting
			// right for the operational lock date to take effect. If the user has either
			// one or the other, then the operational lock date check is ignored.
			if (( Security.HasRight ( RIGHT.CONFIGURE_ACCOUNTING ) == false ) &&
		 ( Security.HasRight ( RIGHT.PERFORM_CLOSEOUT ) == false ) &&
		 ( transaction.InventoryDate <= site._OperationalLockDate.Value ))
			{
				throw new Exception ( "Inventory date must be after the Operational Lock Date." );
			}
		}

		protected void CheckForAndDisplayWarningMessages ( SaveTransactionsResultDO resultDO )
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
					throw new Exception ( msg );
			}
		}

		protected void UndispatchTransaction ( string TransID )
		{
			TransactionDO Transaction = GetTransaction ( TransID );
			UndispatchTransaction ( Transaction );
		}

		protected void UndispatchTransaction ( TransactionDO Transaction )
		{
			PersonClass Operator = null;
			if (Transaction.OperatorPersonnelGuid != Guid.Empty)
			{
				FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel> ( );
				IPersonnel personnel = personnelClient.CreateProxy ( );

				Operator = personnel.Get(Security, Transaction.OperatorPersonnelGuid );
				if (!Operator.IdentityGuid.IsEmpty())
				{
					Operator.Status = PersonClass.STATUS.In;
				}
			}

			LineItemDO LineItem = (LineItemDO) Transaction.LineItems[0];

			Transaction.Status = TransactionStatus.Requested;
			LineItem.Status = TransactionStatus.Requested;

			//Transaction.OperatorPersonnelGuid = 0;
			Transaction.OperatorID = String.Empty;
			Transaction.DispatchedDateTime = null;
			Transaction.IssuePoint = String.Empty;
			Transaction.IssuePointNumber = String.Empty;

			switch (Transaction.TransTypeID)
			{
				case TransactionTypes.T4_SecondaryDefuel:
				case TransactionTypes.T7_FillStand:
					Transaction.DestinationEQ1 = new EquipmentDO ( );
					LineItem.DestinationEQ = new EquipmentDO ( );
					break;

				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T10_Unload:
				case TransactionTypes.T12_InventoryNotAffected:
					Transaction.SourceEQ1 = new EquipmentDO ( );
					LineItem.SourceEQ = new EquipmentDO ( );
					break;

				default:
					throw new ApplicationException ( "Unhandled transaction type passed to dispatch." );

			}

			// Finish and save
			SaveTransaction ( Transaction, Operator );
		}

		protected TransactionDO GetTransaction ( string TransID )
		{
			if (String.IsNullOrEmpty ( TransID ))
			{
				throw new ArgumentNullException ( "TransID" );
			}

			TransactionSR sr = new TransactionSR ( );
			sr.Security = Security;
			sr.TransID = TransID;

			FMChannelFactory<ITransactionProcessor> transactionProcessorClient = new FMChannelFactory<ITransactionProcessor> ( );
			ITransactionProcessor service = transactionProcessorClient.CreateProxy ( );

			TransactionDO Transaction = service.Process ( sr );

			if (Transaction == null)
			{
				throw new ArgumentException ( "Transaction not found" );
			}

			return Transaction;

		}

		protected SaveTransactionsResultDO SaveTransaction ( TransactionDO Transaction )
		{
			return SaveTransaction ( Transaction, null );
		}

		protected SaveTransactionsResultDO SaveTransaction ( TransactionDO Transaction, PersonClass Operator )
		{
			SaveTransactionsResultDO results = null;

			try
			{
				// Check the aviation and capitalize flags against the product configuration
				LineItemDO lineItem = Transaction.LineItems[0] as LineItemDO;

				FMChannelFactory<IProducts> productsClient = new FMChannelFactory<IProducts> ( );
				IProducts products = productsClient.CreateProxy ( );

				ProductClass product = products.GetByInfoAuthorizedCompanies(Security, lineItem.ProductGuid,  false,  true );
				Transaction.Flag02 = false;
				if (product.UserData1.Equals ( "YES", StringComparison.CurrentCultureIgnoreCase ))
				{
					Transaction.Flag02 = true;
				}

				Transaction.Flag01 = false;
				if (product.UserData2.Equals ( "YES", StringComparison.CurrentCultureIgnoreCase ))
				{
					Transaction.Flag01 = true;
				}

				Transaction.UserData[TransactionDO.USER_DATA_KEY_09] = "9 (LOCAL)";

				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
				ISites sites = sitesClient.CreateProxy ( );

				SiteClass site = sites.Get(Security, Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true);

				FMChannelFactory<ITransactionAliases> transactionAliasesClient = new FMChannelFactory<ITransactionAliases> ( );
				ITransactionAliases aliases = transactionAliasesClient.CreateProxy ( );

				TransactionAliasClass transactionAlias = aliases.Get(Security, Transaction.TransactionAliasGuid, false );

				UnitsHelperClass unitsHelper = new UnitsHelperClass ( Security, site, transactionAlias, null );
				unitsHelper.SetUnits ( Transaction, 0 );

				foreach (LineItemDO item in Transaction.LineItems)
				{
					ProductClass prod = products.GetByInfoAuthorizedCompanies(Security, item.ProductGuid, true, false );
					unitsHelper.SetUnits( item, prod.ProductType, prod );
				}

				SaveTransactionsSR saveSR = new SaveTransactionsSR ( );
				saveSR.IndividualDbTransaction = false;
				saveSR.Security = Security;
				saveSR.CurrentSiteGuid = Security.SiteGuid;
				saveSR.ConvertUnits = true;
				saveSR.Transactions.Add ( Transaction );

				saveSR.Operator = Operator;

				FMChannelFactory<ISaveTransactionsProcessor> saveTransactionsProcessorClient = new FMChannelFactory<ISaveTransactionsProcessor> ( );
				ISaveTransactionsProcessor service = saveTransactionsProcessorClient.CreateProxy ( );

				results = service.SaveTransactions ( saveSR );
				CheckForAndDisplayWarningMessages ( results );
			}
			catch (SaveTransactionsException saveExcept)
			{
				string errorMessage = "Save Transaction Failed!";
				foreach (TransactionValidationResult result in saveExcept.Results)
				{
					foreach (string error in result.ErrorList)
					{
						errorMessage += "\n\r" + error;
					}
				}

				ErrorHandler ( errorMessage );

			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}

			return results;

		}

		protected void SaveTransactionNote ( Guid transGuid, string note, string TransactionNote )
		{
			TransactionNoteSR sr = new TransactionNoteSR ( );
			sr.TransactionGuid = transGuid;

			if (TransactionNote.Length == 0)
				sr.Note = note;
			else
				sr.Note = note + " - " + TransactionNote;
			sr.UpdatedBy = Security.UserID;

			FMChannelFactory<ITransactionNoteProcessor> transactionNoteProcessorClient = new FMChannelFactory<ITransactionNoteProcessor> ( );
			ITransactionNoteProcessor notes = transactionNoteProcessorClient.CreateProxy ( );
			notes.Process ( sr );
		}

		protected bool UpdatedItemInList ( ComboBox sender, KeyPressEventArgs e )
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
			currentText = currentText.Remove ( sender.SelectionStart, sender.SelectionLength );
			string newText = currentText.Insert ( sender.SelectionStart, new String ( e.KeyChar, 1 ) );

			if (sender.FindString ( newText ) == -1)
			{
				return false;
			}

			return true;
		}

		protected void RenumberGrid ( DataGridView grid )
		{
			// reset the numbers in the rowheaders.  Can't use UpdateView, as that changes the selected index
			// of the vehicleComboBox, causing unintended, infinite recursion
			int index = 1;
			foreach (DataGridViewRow Row in grid.Rows)
			{
				Row.HeaderCell.Value = index.ToString ( );
				++index;
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

		public ToolStripAeroRenderer ( ToolbarTheme theme )
		{
			Theme = theme;
		}

		/// <summary> 
		/// It shouldn't be necessary to P/Invoke like this, however VisualStyleRenderer.GetMargins 
		/// misses out a parameter in its own P/Invoke. 
		/// </summary> 
		static internal class NativeMethods
		{
			[StructLayout ( LayoutKind.Sequential )]
			public struct MARGINS
			{
				public int cxLeftWidth;
				public int cxRightWidth;
				public int cyTopHeight;
				public int cyBottomHeight;
			}

			[DllImport ( "uxtheme.dll" )]
			public extern static int GetThemeMargins ( IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, IntPtr rect, out MARGINS pMargins );
		}

		// See http://msdn2.microsoft.com/en-us/library/bb773210.aspx - "Parts and States" 
		// Only menu-related parts/states are needed here, VisualStyleRenderer handles most of the rest. 
		enum MenuParts : int
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

		enum MenuBarStates : int
		{
			Active = 1,
			Inactive = 2
		}

		enum MenuBarItemStates : int
		{
			Normal = 1,
			Hover = 2,
			Pushed = 3,
			Disabled = 4,
			DisabledHover = 5,
			DisabledPushed = 6
		}

		enum MenuPopupItemStates : int
		{
			Normal = 1,
			Hover = 2,
			Disabled = 3,
			DisabledHover = 4
		}

		enum MenuPopupCheckStates : int
		{
			CheckmarkNormal = 1,
			CheckmarkDisabled = 2,
			BulletNormal = 3,
			BulletDisabled = 4
		}

		enum MenuPopupCheckBackgroundStates : int
		{
			Disabled = 1,
			Normal = 2,
			Bitmap = 3
		}

		enum MenuPopupSubMenuStates : int
		{
			Normal = 1,
			Disabled = 2
		}

		enum MarginTypes : int
		{
			Sizing = 3601,
			Content = 3602,
			Caption = 3603
		}

		static readonly int RebarBackground = 6;

		Padding GetThemeMargins ( IDeviceContext dc, MarginTypes marginType )
		{
			NativeMethods.MARGINS margins;
			try
			{
				IntPtr hDC = dc.GetHdc ( );
				if (0 == NativeMethods.GetThemeMargins ( renderer.Handle, hDC, renderer.Part, renderer.State, (int) marginType, IntPtr.Zero, out margins ))
					return new Padding ( margins.cxLeftWidth, margins.cyTopHeight, margins.cxRightWidth, margins.cyBottomHeight );
				return new Padding ( 0 );
			}
			finally
			{
				dc.ReleaseHdc ( );
			}
		}

		private static int GetItemState ( ToolStripItem item )
		{
			bool hot = item.Selected;

			if (item.IsOnDropDown)
			{
				if (item.Enabled)
					return hot ? (int) MenuPopupItemStates.Hover : (int) MenuPopupItemStates.Normal;
				return hot ? (int) MenuPopupItemStates.DisabledHover : (int) MenuPopupItemStates.Disabled;
			}
			else
			{
				if (item.Pressed)
					return item.Enabled ? (int) MenuBarItemStates.Pushed : (int) MenuBarItemStates.DisabledPushed;
				if (item.Enabled)
					return hot ? (int) MenuBarItemStates.Hover : (int) MenuBarItemStates.Normal;
				return hot ? (int) MenuBarItemStates.DisabledHover : (int) MenuBarItemStates.Disabled;
			}
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
				return SubclassPrefix + "Rebar";
			}
		}

		private string ToolbarClass
		{
			get
			{
				return SubclassPrefix + "ToolBar";
			}
		}

		private string MenuClass
		{
			get
			{
				return SubclassPrefix + "Menu";
			}
		}

		private string SubclassPrefix
		{
			get
			{
				switch (Theme)
				{
					case ToolbarTheme.MediaToolbar: return "Media::";
					case ToolbarTheme.CommunicationsToolbar: return "Communications::";
					case ToolbarTheme.BrowserTabBar: return "BrowserTabBar::";
					case ToolbarTheme.HelpBar: return "Help::";
					default: return string.Empty;
				}
			}
		}

		private VisualStyleElement Subclass ( VisualStyleElement element )
		{
			return VisualStyleElement.CreateElement ( SubclassPrefix + element.ClassName,
					element.Part, element.State );
		}

		private bool EnsureRenderer ( )
		{
			if (!IsSupported)
				return false;

			if (renderer == null)
				renderer = new VisualStyleRenderer ( VisualStyleElement.Button.PushButton.Normal );

			return true;
		}

		// Gives parented ToolStrips a transparent background. 
		protected override void Initialize ( ToolStrip toolStrip )
		{
			if (toolStrip.Parent is ToolStripPanel)
				toolStrip.BackColor = Color.Transparent;

			base.Initialize ( toolStrip );
		}

		// Using just ToolStripManager.Renderer without setting the Renderer individually per ToolStrip means 
		// that the ToolStrip is not passed to the Initialize method. ToolStripPanels, however, are. So we can  
		// simply initialize it here too, and this should guarantee that the ToolStrip is initialized at least  
		// once. Hopefully it isn't any more complicated than this. 
		protected override void InitializePanel ( ToolStripPanel toolStripPanel )
		{
			foreach (Control control in toolStripPanel.Controls)
				if (control is ToolStrip)
					Initialize ( (ToolStrip) control );

			base.InitializePanel ( toolStripPanel );
		}

		protected override void OnRenderToolStripBorder ( ToolStripRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				renderer.SetParameters ( MenuClass, (int) MenuParts.PopupBorders, 0 );
				if (e.ToolStrip.IsDropDown)
				{
					Region oldClip = e.Graphics.Clip;

					// Tool strip borders are rendered *after* the content, for some reason. 
					// So we have to exclude the inside of the popup otherwise we'll draw over it. 
					Rectangle insideRect = e.ToolStrip.ClientRectangle;
					insideRect.Inflate ( -1, -1 );
					e.Graphics.ExcludeClip ( insideRect );

					renderer.DrawBackground ( e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds );

					// Restore the old clip in case the Graphics is used again (does that ever happen?) 
					e.Graphics.Clip = oldClip;
				}
			}
			else
			{
				base.OnRenderToolStripBorder ( e );
			}
		}

		Rectangle GetBackgroundRectangle ( ToolStripItem item )
		{
			if (!item.IsOnDropDown)
				return new Rectangle ( new Point ( ), item.Bounds.Size );

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

		protected override void OnRenderMenuItemBackground ( ToolStripItemRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				int partID = e.Item.IsOnDropDown ? (int) MenuParts.PopupItem : (int) MenuParts.BarItem;
				renderer.SetParameters ( MenuClass, partID, GetItemState ( e.Item ) );

				Rectangle bgRect = GetBackgroundRectangle ( e.Item );
				renderer.DrawBackground ( e.Graphics, bgRect, bgRect );
			}
			else
			{
				base.OnRenderMenuItemBackground ( e );
			}
		}

		protected override void OnRenderToolStripPanelBackground ( ToolStripPanelRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				// Draw the background using Rebar & RP_BACKGROUND (or, if that is not available, fall back to 
				// Rebar.Band.Normal) 
				if (VisualStyleRenderer.IsElementDefined ( VisualStyleElement.CreateElement ( RebarClass, RebarBackground, 0 ) ))
				{
					renderer.SetParameters ( RebarClass, RebarBackground, 0 );
				}
				else
				{
					renderer.SetParameters ( RebarClass, 0, 0 );
				}

				if (renderer.IsBackgroundPartiallyTransparent ( ))
					renderer.DrawParentBackground ( e.Graphics, e.ToolStripPanel.ClientRectangle, e.ToolStripPanel );

				renderer.DrawBackground ( e.Graphics, e.ToolStripPanel.ClientRectangle );

				e.Handled = true;
			}
			else
			{
				base.OnRenderToolStripPanelBackground ( e );
			}
		}

		// Render the background of an actual menu bar, dropdown menu or toolbar. 
		protected override void OnRenderToolStripBackground ( System.Windows.Forms.ToolStripRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				if (e.ToolStrip.IsDropDown)
				{
					renderer.SetParameters ( MenuClass, (int) MenuParts.PopupBackground, 0 );
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
					else
					{
						// A lone toolbar/menubar should act like it's inside a toolbox, I guess. 
						// Maybe I should use the MenuClass in the case of a MenuStrip, although that would break 
						// the other themes... 
						if (VisualStyleRenderer.IsElementDefined ( VisualStyleElement.CreateElement ( RebarClass, RebarBackground, 0 ) ))
							renderer.SetParameters ( RebarClass, RebarBackground, 0 );
						else
							renderer.SetParameters ( RebarClass, 0, 0 );
					}
				}

				if (renderer.IsBackgroundPartiallyTransparent ( ))
					renderer.DrawParentBackground ( e.Graphics, e.ToolStrip.ClientRectangle, e.ToolStrip );

				renderer.DrawBackground ( e.Graphics, e.ToolStrip.ClientRectangle, e.AffectedBounds );
			}
			else
			{
				base.OnRenderToolStripBackground ( e );
			}
		}

		// The only purpose of this override is to change the arrow colour. 
		// It's OK to just draw over the default arrow since we also pass down arrow drawing to the system renderer. 
		protected override void OnRenderSplitButtonBackground ( ToolStripItemRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				ToolStripSplitButton sb = (ToolStripSplitButton) e.Item;
				base.OnRenderSplitButtonBackground ( e );

				// It doesn't matter what colour of arrow we tell it to draw. OnRenderArrow will compute it from the item anyway. 
				OnRenderArrow ( new ToolStripArrowRenderEventArgs ( e.Graphics, sb, sb.DropDownButtonBounds, Color.Red, ArrowDirection.Down ) );
			}
			else
			{
				base.OnRenderSplitButtonBackground ( e );
			}
		}

		Color GetItemTextColor ( ToolStripItem item )
		{
			int partId = item.IsOnDropDown ? (int) MenuParts.PopupItem : (int) MenuParts.BarItem;
			renderer.SetParameters ( MenuClass, partId, GetItemState ( item ) );
			return renderer.GetColor ( ColorProperty.TextColor );
		}

		protected override void OnRenderItemText ( ToolStripItemTextRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
				e.TextColor = GetItemTextColor ( e.Item );

			base.OnRenderItemText ( e );
		}

		protected override void OnRenderImageMargin ( ToolStripRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				if (e.ToolStrip.IsDropDown)
				{
					renderer.SetParameters ( MenuClass, (int) MenuParts.PopupGutter, 0 );
					// The AffectedBounds is usually too small, way too small to look right. Instead of using that, 
					// use the AffectedBounds but with the right width. Then narrow the rectangle to the correct edge 
					// based on whether or not it's RTL. (It doesn't need to be narrowed to an edge in LTR mode, but let's 
					// do that anyway.) 
					// Using the DisplayRectangle gets roughly the right size so that the separator is closer to the text. 
					Padding margins = GetThemeMargins ( e.Graphics, MarginTypes.Sizing );
					int extraWidth = ( e.ToolStrip.Width - e.ToolStrip.DisplayRectangle.Width - margins.Left - margins.Right - 1 ) - e.AffectedBounds.Width;
					Rectangle rect = e.AffectedBounds;
					rect.Y += 2;
					rect.Height -= 4;
					int sepWidth = renderer.GetPartSize ( e.Graphics, ThemeSizeType.True ).Width;
					if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
					{
						rect = new Rectangle ( rect.X - extraWidth, rect.Y, sepWidth, rect.Height );
						rect.X += sepWidth;
					}
					else
					{
						rect = new Rectangle ( rect.Width + extraWidth - sepWidth, rect.Y, sepWidth, rect.Height );
					}
					renderer.DrawBackground ( e.Graphics, rect );
				}
			}
			else
			{
				base.OnRenderImageMargin ( e );
			}
		}

		protected override void OnRenderSeparator ( ToolStripSeparatorRenderEventArgs e )
		{
			if (e.ToolStrip.IsDropDown && EnsureRenderer ( ))
			{
				renderer.SetParameters ( MenuClass, (int) MenuParts.PopupSeparator, 0 );
				Rectangle rect = new Rectangle ( e.ToolStrip.DisplayRectangle.Left, 0, e.ToolStrip.DisplayRectangle.Width, e.Item.Height );
				renderer.DrawBackground ( e.Graphics, rect, rect );
			}
			else
			{
				base.OnRenderSeparator ( e );
			}
		}

		protected override void OnRenderItemCheck ( ToolStripItemImageRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				Rectangle bgRect = GetBackgroundRectangle ( e.Item );
				bgRect.Width = bgRect.Height;

				// Now, mirror its position if the menu item is RTL. 
				if (e.Item.RightToLeft == RightToLeft.Yes)
					bgRect = new Rectangle ( e.ToolStrip.ClientSize.Width - bgRect.X - bgRect.Width, bgRect.Y, bgRect.Width, bgRect.Height );

				renderer.SetParameters ( MenuClass, (int) MenuParts.PopupCheckBackground, e.Item.Enabled ? (int) MenuPopupCheckBackgroundStates.Normal : (int) MenuPopupCheckBackgroundStates.Disabled );
				renderer.DrawBackground ( e.Graphics, bgRect );

				Rectangle checkRect = e.ImageRectangle;
				checkRect.X = bgRect.X + bgRect.Width / 2 - checkRect.Width / 2;
				checkRect.Y = bgRect.Y + bgRect.Height / 2 - checkRect.Height / 2;

				// I don't think ToolStrip even supports radio box items, so no need to render them. 
				renderer.SetParameters ( MenuClass, (int) MenuParts.PopupCheck, e.Item.Enabled ? (int) MenuPopupCheckStates.CheckmarkNormal : (int) MenuPopupCheckStates.CheckmarkDisabled );

				renderer.DrawBackground ( e.Graphics, checkRect );
			}
			else
			{
				base.OnRenderItemCheck ( e );
			}
		}

		protected override void OnRenderArrow ( ToolStripArrowRenderEventArgs e )
		{
			// The default renderer will draw an arrow for us (the UXTheme API seems not to have one for all directions), 
			// but it will get the colour wrong in many cases. The text colour is probably the best colour to use. 
			if (EnsureRenderer ( ))
				e.ArrowColor = GetItemTextColor ( e.Item );
			base.OnRenderArrow ( e );
		}

		protected override void OnRenderOverflowButtonBackground ( ToolStripItemRenderEventArgs e )
		{
			if (EnsureRenderer ( ))
			{
				// BrowserTabBar::Rebar draws the chevron using the default background. Odd. 
				string rebarClass = RebarClass;
				if (Theme == ToolbarTheme.BrowserTabBar)
					rebarClass = "Rebar";

				int state = VisualStyleElement.Rebar.Chevron.Normal.State;
				if (e.Item.Pressed)
					state = VisualStyleElement.Rebar.Chevron.Pressed.State;
				else if (e.Item.Selected)
					state = VisualStyleElement.Rebar.Chevron.Hot.State;

				renderer.SetParameters ( rebarClass, VisualStyleElement.Rebar.Chevron.Normal.Part, state );
				renderer.DrawBackground ( e.Graphics, new Rectangle ( Point.Empty, e.Item.Size ) );
			}
			else
			{
				base.OnRenderOverflowButtonBackground ( e );
			}
		}

		public bool IsSupported
		{
			get
			{
				if (!VisualStyleRenderer.IsSupported)
					return false;

				// Needs a more robust check. It seems mono supports very different style sets. 
				return
						VisualStyleRenderer.IsElementDefined (
								VisualStyleElement.CreateElement ( "Menu",
										(int) MenuParts.BarBackground,
										(int) MenuBarStates.Active ) );
			}
		}
	}
}
