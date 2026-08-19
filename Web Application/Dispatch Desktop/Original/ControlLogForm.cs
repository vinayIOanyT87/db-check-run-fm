using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace DispatchPrototype
{
	public enum LISTVIEW_SORT_DIRECTION
	{
		DESCENDING = 0,
		ASCENDING = 1
	}
	public partial class ControlLogForm : FMBaseForm
	{
		public string UserID = "";
		private int ColumnSorted = 0;
		private LISTVIEW_SORT_DIRECTION ColumnSortDirection = LISTVIEW_SORT_DIRECTION.DESCENDING;

		private Font PrintFont = null;
		private Font PrintFontUnderline = null;
		private Font PrintFontBold = null;
		private Font PrintFontTitle = null;
		private int PrintIndex = 0;
		private int PrintPage = 0;

		// Finish ControllerLogCollectionClass for use here
		//private ControllerLogCollectionClass ControllerLogCollection = null;

		public ControlLogForm ( )
		{
			GetSecurity ( );

			InitializeComponent ( );

			// format date controls based on site configuration (IGO 2010-Aug-13)
			GetSiteDateTimeFormatInfo ( );
			StartDatePicker.CustomFormat = SiteDateTimeFormatInfo.ShortDatePattern;
			StartDatePicker.Format = DateTimePickerFormat.Custom;
			StartDatePicker.Value = System.DateTime.Now;
			StopDatePicker.CustomFormat = SiteDateTimeFormatInfo.ShortDatePattern;
			StopDatePicker.Format = DateTimePickerFormat.Custom;
			StopDatePicker.Value = System.DateTime.Now;

			ShowDeletedcheckBox.Checked = false;
			Editbutton.Enabled = false;
			Deletebutton.Enabled = false;

			StartDatePicker.TextChanged += new EventHandler ( StartDatePicker_TextChanged );
			StopDatePicker.TextChanged += new EventHandler ( StopDatePicker_TextChanged );
			Resize += new EventHandler ( ControlLogForm_Resize );
			ControlLogForm_Resize ( null, null );
			ControllersLogListView.DoubleClick += new EventHandler ( ControllersLogListView_DoubleClick );
		}

		void ControllersLogListView_DoubleClick ( object sender, EventArgs e )
		{
			try
			{
				if (ControllersLogListView.SelectedItems.Count > 0)
				{
					string selecteditemtext = ControllersLogListView.SelectedItems[0].Text;
					AddMemoForm AddMemo = new AddMemoForm ( );
					AddMemo.UserID = this.UserID;
					AddMemo.EditedItemIndex = System.Convert.ToInt32 ( selecteditemtext );
					AddMemo.ShowDialog ( this );
				}
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		void ControlLogForm_Resize ( object sender, EventArgs e )
		{
			try
			{
				ControllersLogListView.Width = Width - 125;
				ControllersLogListView.Height = Height - ControllersLogListView.Top - 50;
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		void StopDatePicker_TextChanged ( object sender, EventArgs e )
		{
			try
			{
				UpdateData ( );
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		void StartDatePicker_TextChanged ( object sender, EventArgs e )
		{
			try
			{
				UpdateData ( );
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		private void InitializeListViewDisplay ( )
		{
			ControllersLogListView.Clear ( );

			ControllersLogListView.View = View.Details;

			ControllersLogListView.Columns.Add ( "Index", 0, HorizontalAlignment.Left );

			ControllersLogListView.Columns.Add ( "Date/Time", 140, HorizontalAlignment.Left );

			ControllersLogListView.Columns.Add ( "Controller", 100, HorizontalAlignment.Left );

			ControllersLogListView.Columns.Add ( "Memo", 600, HorizontalAlignment.Left );

		}

		private void OnAddMemo ( object sender, EventArgs e )
		{
			if (!Security.HasRight ( RIGHT.MODIFY_DISPATCH ))
			{
				return;
			}

			AddMemoForm AddMemo = new AddMemoForm ( );
			AddMemo.UserID = this.UserID;
			AddMemo.EditedItemIndex = -1;
			AddMemo.ShowDialog ( this );
			UpdateData ( );
		}

		private void OnCancelClicked ( object sender, EventArgs e )
		{
			Close ( );
		}

		private void UpdateData ( )
		{
			try
			{
				bool Sorted = false;
				ListViewItem li;

				FMChannelFactory<IControllerLogs> cntrlLogsClient = new FMChannelFactory<IControllerLogs> ( );
				IControllerLogs controllerLogs = cntrlLogsClient.CreateProxy ( );

				ControllerLogClass controllerLog = new ControllerLogClass ( );

				InitializeListViewDisplay ( );

				if (ControllersLogListView.ListViewItemSorter != null)
				{
					Sorted = true;
					ControllersLogListView.ListViewItemSorter = null;
				}

				if (StartDatePicker.Value > StopDatePicker.Value)
				{
					StopDatePicker.Value = StartDatePicker.Value;
				}

				SecurityClass security = AppDomain.CurrentDomain.GetData ( "Security" ) as SecurityClass;
				if (security == null)
				{
					throw new Exception ( "Security not in AppDomain" );
				}

				List<ControllerLogClass> controllerLogCollection = controllerLogs.EnumerateByStartStopTime ( security, StartDatePicker.Value, StopDatePicker.Value, ShowDeletedcheckBox.Checked );
				foreach (ControllerLogClass controllerLogData in controllerLogCollection)
				{
					li = ControllersLogListView.Items.Add ( controllerLogData.Index.ToString ( ) );
					li.SubItems.Add ( controllerLogData.EventTime.ToString ( ) );
					li.SubItems.Add ( controllerLogData.Controller );
					li.SubItems.Add ( controllerLogData.Memo );
				}

				// reapply the sort
				if (Sorted == true)
				{
					this.ControllersLogListView.ListViewItemSorter = new ListViewItemComparer ( ColumnSorted, ColumnSortDirection );
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show ( this, exception.Message, this.Text );
			}
		}

		private void OnEditClicked ( object sender, EventArgs e )
		{
			if (ControllersLogListView.SelectedItems.Count == 0)
			{
				return;
			}

			string selecteditemtext = ControllersLogListView.SelectedItems[0].Text;
			AddMemoForm AddMemo = new AddMemoForm ( );
			AddMemo.UserID = this.UserID;
			AddMemo.EditedItemIndex = System.Convert.ToInt32 ( selecteditemtext );
			AddMemo.ShowDialog ( this );
		}

		private void OnDeleteButtonClicked ( object sender, EventArgs e )
		{
			if (!Security.HasRight ( RIGHT.MODIFY_DISPATCH ))
			{
				return;
			}

			try
			{
				int iLoop = 0;
				DialogResult result = DialogResult.No;

				if (ControllersLogListView.SelectedItems.Count == 0)
				{
					return;
				}

				int[] SelectedItems = new int[ControllersLogListView.SelectedItems.Count];
				SecurityClass security = AppDomain.CurrentDomain.GetData ( "Security" ) as SecurityClass;

				if (security == null)
				{
					throw new Exception ( "Security not in AppDomain" );
				}

				// due to the way .net works opening the following dialog will cayse an intem selection change notification
				// to the listview resulting in all items being unselected
				// here we will cheat and set the selected items into an int array
				for (iLoop = 0; iLoop < ControllersLogListView.SelectedItems.Count; iLoop++)
				{
					SelectedItems[iLoop] = System.Convert.ToInt32 ( ControllersLogListView.SelectedItems[iLoop].Text );
				}

				if (ShowDeletedcheckBox.Checked == false)
				{
					result = MessageBox.Show ( this, "Are You Sure You Want To Delete These Log(s)?", "Controllers Log", MessageBoxButtons.YesNo );
				}
				else
				{
					result = MessageBox.Show ( this, "Are You Sure You Want To Un-Delete These Log(s)?", "Controllers Log", MessageBoxButtons.YesNo );
				}

				if (result == DialogResult.Yes)
				{
					FMChannelFactory<IControllerLogs> cntrlLogsClient = new FMChannelFactory<IControllerLogs> ( );
					IControllerLogs controllerLogs = cntrlLogsClient.CreateProxy ( );

					foreach (int ItemToDelete in SelectedItems)
					{
						if (ShowDeletedcheckBox.Checked == false)
						{
							//controllerLogs.DeleteControllerLog ( security, ItemToDelete );
						}
						else
						{
							//controllerLogs.UnDeleteControllerLog ( security, ItemToDelete );
						}
					}

					this.UpdateData ( );
				}

			}
			catch (Exception exception)
			{
				MessageBox.Show ( this, exception.Message, this.Text );
			}
		}

		private void OnShowDeletedItemsCheckBoxStateChanged ( object sender, EventArgs e )
		{
			if (ShowDeletedcheckBox.Checked == false)
			{
				Deletebutton.Text = "Delete";
				Editbutton.Enabled = false;
				Deletebutton.Enabled = false;

				if (base.Security.HasRight ( RIGHT.MODIFY_DISPATCH ))
				{
					AddButton.Enabled = true;
				}
				else
				{
					AddButton.Enabled = false;
				}
			}
			else
			{
				Deletebutton.Text = "Un-Delete";
				Editbutton.Enabled = false;
				Deletebutton.Enabled = false;
				AddButton.Enabled = false;
			}

			UpdateData ( );
		}

		private void OnControllerLogColumnClick ( object sender, ColumnClickEventArgs e )
		{
			if (ColumnSorted != e.Column)
			{
				ColumnSorted = e.Column;
				ColumnSortDirection = LISTVIEW_SORT_DIRECTION.ASCENDING;
			}
			else
			{
				if (ColumnSortDirection == LISTVIEW_SORT_DIRECTION.ASCENDING)
					ColumnSortDirection = LISTVIEW_SORT_DIRECTION.DESCENDING;
				else
					ColumnSortDirection = LISTVIEW_SORT_DIRECTION.ASCENDING;
			}
			ControllersLogListView.ListViewItemSorter = new ListViewItemComparer ( e.Column, ColumnSortDirection );
		}

		private void SelectedIndexChanged ( object sender, EventArgs e )
		{
			if (ControllersLogListView.SelectedItems.Count == 1)
			{
				if (ShowDeletedcheckBox.Checked == true)
					Editbutton.Enabled = false;
				else
					Editbutton.Enabled = true;

				if (base.Security.HasRight ( RIGHT.MODIFY_DISPATCH ))
				{
					Deletebutton.Enabled = true;
				}
				else
				{
					Deletebutton.Enabled = false;
				}
			}
			else if (ControllersLogListView.SelectedItems.Count > 1)
			{
				Editbutton.Enabled = false;

				if (base.Security.HasRight ( RIGHT.MODIFY_DISPATCH ))
				{
					Deletebutton.Enabled = true;
				}
				else
				{
					Deletebutton.Enabled = false;
				}
			}
			else
			{
				Editbutton.Enabled = false;
				Deletebutton.Enabled = false;
			}

		}

		private void OnColumnWidthChanged ( object sender, ColumnWidthChangedEventArgs e )
		{
			// do not allow the user to display the index column
			if (e.ColumnIndex == 0)
			{
				if (ControllersLogListView.Columns[e.ColumnIndex].Width > 0)
					ControllersLogListView.Columns[e.ColumnIndex].Width = 0;
			}

		}

		private void PrintButton_Click ( object sender, EventArgs e )
		{
			try
			{
				PrintPreviewDialog PreviewDialog = new PrintPreviewDialog ( );
				PreviewDialog.Document = GetPrintDocument ( );
				PreviewDialog.Height = 600;
				PreviewDialog.Width = 800;
				PreviewDialog.ShowDialog ( this );
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}
		}

		private PrintDocument GetPrintDocument ( )
		{
			PrintDocument printDocument = new PrintDocument ( );
			printDocument.DefaultPageSettings.Margins.Top = 50;
			printDocument.DefaultPageSettings.Margins.Bottom = 50;
			printDocument.DefaultPageSettings.Margins.Left = 50;
			printDocument.DefaultPageSettings.Margins.Right = 50;
			printDocument.BeginPrint += new PrintEventHandler ( printDocument_BeginPrint );
			printDocument.PrintPage += new PrintPageEventHandler ( printDocument_PrintPage );
			return printDocument;
		}

		void printDocument_BeginPrint ( object sender, PrintEventArgs e )
		{
			try
			{
				PrintPage = 0;
				PrintIndex = 0;
				PrintFont = new Font ( "Arial", 8 );
				PrintFontUnderline = new Font ( "Arial", 8, FontStyle.Underline | FontStyle.Bold );
				PrintFontBold = new Font ( "Arial", 10, FontStyle.Bold );
				PrintFontTitle = new Font ( "Arial", 14, FontStyle.Bold );
			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}

		}

		void printDocument_PrintPage ( object sender, PrintPageEventArgs ev )
		{
			try
			{
				float linesPerPage = 0;
				float yPos = 0;
				int count = 0;
				float leftMargin = ev.MarginBounds.Left;
				float topMargin = ev.MarginBounds.Top;
				int Index = 0;

				++PrintPage;

				// Calculate the number of lines per page.
				linesPerPage = ev.MarginBounds.Height / PrintFont.GetHeight ( ev.Graphics ) - 1;

				yPos = topMargin - 10;
				CenterLine ( "Unclassified/For Official Use Only", ev, yPos, PrintFontBold );
				++count;

				yPos = topMargin + ( count * PrintFont.GetHeight ( ev.Graphics ) );
				ev.Graphics.DrawString ( "Controllers Log", PrintFontTitle, Brushes.Black, leftMargin, yPos, new StringFormat ( ) );
				count += 2;

				yPos = topMargin + ( count * PrintFont.GetHeight ( ev.Graphics ) );
				PrintHeader ( ev.Graphics, leftMargin, yPos );
				++count;

				for (Index = PrintIndex; Index < ControllersLogListView.Items.Count && count < linesPerPage; ++Index)
				{
					ListViewItem Item = ControllersLogListView.Items[Index];

					yPos = topMargin + ( count * PrintFont.GetHeight ( ev.Graphics ) );

					int lineCount = PrintLine ( ev.Graphics, leftMargin, yPos, Item );

					count += lineCount;

				}

				if (Index >= ControllersLogListView.Items.Count - 1)
				{
					ev.HasMorePages = false;
				}
				else
				{
					PrintIndex = Index;
					ev.HasMorePages = true;
				}

				yPos = ev.PageBounds.Bottom - 40;
				LeftLine ( DateTime.Now.ToShortDateString ( ), ev, yPos, PrintFont );
				CenterLine ( "Unclassified/For Official Use Only", ev, yPos, PrintFontBold );
				RightLine ( PrintPage.ToString ( ), ev, yPos, PrintFont );

			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}

		}

		private void LeftLine ( string p, PrintPageEventArgs ev, float yPos, Font printFont )
		{
			ev.Graphics.DrawString ( p, printFont, Brushes.Black, ev.PageBounds.Left + 50, yPos, new StringFormat ( ) );
		}

		private void CenterLine ( string p, PrintPageEventArgs ev, float yPos, Font printFont )
		{
			SizeF sizeOfText = ev.Graphics.MeasureString ( p, printFont );

			float xPos = ( ev.PageBounds.Width / 2 ) - ( sizeOfText.Width / 2 );

			ev.Graphics.DrawString ( p, printFont, Brushes.Black, xPos, yPos, new StringFormat ( ) );
		}

		private void RightLine ( string p, PrintPageEventArgs ev, float yPos, Font printFont )
		{
			SizeF sizeOfText = ev.Graphics.MeasureString ( p, printFont );

			float xPos = ev.PageBounds.Right - 50 - sizeOfText.Width;

			ev.Graphics.DrawString ( p, printFont, Brushes.Black, xPos, yPos, new StringFormat ( ) );

		}

		private int PrintLine ( Graphics graphics, float leftMargin, float yPos, ListViewItem Item )
		{
			int numberOfLines = 1;

			ColumnHeader Header = ControllersLogListView.Columns[1];
			graphics.DrawString ( Item.SubItems[1].Text, PrintFont, Brushes.Black, leftMargin, yPos, new StringFormat ( ) );
			leftMargin += Header.Width + 10;

			Header = ControllersLogListView.Columns[2];
			graphics.DrawString ( Item.SubItems[2].Text, PrintFont, Brushes.Black, leftMargin, yPos, new StringFormat ( ) );
			leftMargin += Header.Width + 10;

			Header = ControllersLogListView.Columns[3];
			string memoText = Item.SubItems[3].Text;
			graphics.DrawString ( memoText.Substring ( 0, Math.Min ( memoText.Length, 100 ) ), PrintFont, Brushes.Black, leftMargin, yPos, new StringFormat ( ) );
			yPos += PrintFont.GetHeight ( graphics );

			if (Item.SubItems[3].Text.Length > 100)
			{
				graphics.DrawString ( memoText.Substring ( 100 ), PrintFont, Brushes.Black, leftMargin, yPos, new StringFormat ( ) );
				++numberOfLines;
			}

			return numberOfLines;

		}

		private void PrintHeader ( Graphics graphics, float leftMargin, float yPos )
		{
			for (int Index = 1; Index < ControllersLogListView.Columns.Count; ++Index)
			{
				ColumnHeader Header = ControllersLogListView.Columns[Index];

				graphics.DrawString ( Header.Text, PrintFontUnderline, Brushes.Black, leftMargin, yPos, new StringFormat ( ) );

				leftMargin += Header.Width + 10;
			}

		}

		private void ControlLogForm_Load ( object sender, EventArgs e )
		{
			bool bEnable = Security.HasRight ( RIGHT.MODIFY_DISPATCH );

			AddButton.Enabled = AddButton.Enabled && bEnable;
			Editbutton.Enabled = Editbutton.Enabled && bEnable;
			Deletebutton.Enabled = Deletebutton.Enabled && bEnable;

			UpdateData ( );
		}

	}

	class ListViewItemComparer : System.Collections.IComparer
	{
		private int col;
		LISTVIEW_SORT_DIRECTION ColumnSortDirection;
		public ListViewItemComparer ( )
		{
			col = 0;
		}
		public ListViewItemComparer ( int column, LISTVIEW_SORT_DIRECTION SortDirection )
		{
			col = column;
			ColumnSortDirection = SortDirection;
		}
		public int Compare ( object x, object y )
		{
			if (col != 1)
			{
				if (ColumnSortDirection == LISTVIEW_SORT_DIRECTION.ASCENDING)
					return String.Compare ( ( (ListViewItem) x ).SubItems[col].Text, ( (ListViewItem) y ).SubItems[col].Text );
				else
					return String.Compare ( ( (ListViewItem) y ).SubItems[col].Text, ( (ListViewItem) x ).SubItems[col].Text );
			}
			else
			{
				DateTime xdatetime = System.Convert.ToDateTime ( ( (ListViewItem) x ).SubItems[col].Text );
				DateTime ydatetime = System.Convert.ToDateTime ( ( (ListViewItem) y ).SubItems[col].Text );
				long xvalue = xdatetime.Ticks;
				long yvalue = ydatetime.Ticks;

				if (ColumnSortDirection == LISTVIEW_SORT_DIRECTION.ASCENDING)
					return String.Compare ( xvalue.ToString ( ), yvalue.ToString ( ) );
				else
					return String.Compare ( yvalue.ToString ( ), xvalue.ToString ( ) );
			}
		}
	}

}
